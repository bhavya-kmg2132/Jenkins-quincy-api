# Quincy API — Server Preparation & CI/CD Configuration

This document describes how `netcoreapi` gets from a `git push` to a running IIS site, and
exactly how to configure a Quincy deployment server (Windows) to reproduce this setup. It
reflects the actual working configuration on `KMG-JPR-004`, verified end-to-end (Jenkins build
#21, `Finished: SUCCESS`).

## 1. Architecture overview

```
git push (feature_Api_dev or main)
        │
        ▼
GitHub Actions (self-hosted runner, same box as Jenkins)
        │  curls the local Jenkins REST API
        ▼
Jenkins job "quincy-api-netcoreapi"  (also auto-polls GitHub every ~2 min as a fallback)
        │
        ├─ Restore / Build / Publish  (Microservices/netcoreapi/src/Api/Api.csproj)
        ├─ Harden Published Config    (blanks secret fields in the published appsettings.json)
        ├─ Stop App Pool
        ├─ Deploy via Web Deploy      (msdeploy sync → IIS site "QuincyApi")
        ├─ Inject Runtime Secrets     (writes real secrets into web.config <environmentVariables>)
        ├─ Start App Pool
        └─ Health Check               (GET /api/health, retries until 200)
```

Jenkins and IIS run on the **same machine** as the deployment target, and the GitHub Actions
runner also runs there — this avoids needing to expose anything to the public internet. The
runner makes an *outbound* connection to GitHub to receive jobs; nothing needs to be port-forwarded.

Two pipeline definitions live in the repo and both drive the same Jenkins job:

- [`Jenkinsfile`](Jenkinsfile) — the actual build/deploy pipeline.
- [`.github/workflows/trigger-jenkins.yml`](.github/workflows/trigger-jenkins.yml) — fires the Jenkins job on push via the self-hosted runner.

## 2. Prerequisites on the server

Install these before configuring anything else:

| Software | Why | Notes |
|---|---|---|
| .NET 10 SDK | Build/publish/run the app | `dotnet --version` should report `10.0.x` |
| IIS (Web Server role) + ASP.NET Core Hosting Bundle | Hosts the deployed app (`OutOfProcess` model, ASP.NET Core Module V2) | |
| **Web Deploy V3** | Jenkins syncs the publish output into IIS | Installed at `C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe` — install via the Web Platform Installer or the standalone MSI from Microsoft |
| Jenkins (Windows install, LTS) | Runs the pipeline | Confirmed working: Jenkins 2.568.2 |
| Git for Windows | Jenkins checks out the repo | `git.exe` on PATH |
| GitHub CLI (`gh`) | Used once during setup to create runner tokens / repo secrets | Only needed on the machine doing the initial setup, not required at pipeline runtime |

Not required, despite being commonly bundled with server IIS setups: **IIS URL Rewrite** /
**Application Request Routing (ARR)** — these are only needed if you later expose Jenkins itself
through a reverse proxy (see §7). They are *not* installed on `KMG-JPR-004` today.

## 3. IIS configuration

Create the site once, by hand, before the pipeline ever runs — Web Deploy syncs *into* an
existing site, it does not create one.

| Setting | Value used on KMG-JPR-004 |
|---|---|
| Site name | `QuincyApi` |
| App pool | `QuincyApi` (.NET CLR version: **No Managed Code** — ASP.NET Core manages its own runtime) |
| Physical path | `C:\inetpub\wwwroot\QuincyApi` |
| Binding | `http://*:8081` |
| Hosting model | Out-of-process (set via the deployed `web.config`, already correct in the repo) |

The site's `web.config` is deployed by the pipeline itself (from
[`Microservices/netcoreapi/src/Api/web.config`](Microservices/netcoreapi/src/Api/web.config)) —
you don't need to hand-write it, just make sure the site/app pool/binding above exist so
`msdeploy` has somewhere to sync to.

## 4. Jenkins setup

### 4.1 Plugins
Core Jenkins (2.568.2) was sufficient — no GitHub plugin, no Timestamper plugin needed. The
Jenkinsfile deliberately avoids `timestamps()` and `githubPush()` for this reason (both require
plugins that aren't installed).

### 4.2 Job configuration
- **New Item → Pipeline**, name `quincy-api-netcoreapi`.
- **Build Triggers**: check **Poll SCM**, schedule `H/2 * * * *` (also declared in the
  Jenkinsfile's `triggers { pollSCM(...) }` block — belt-and-suspenders fallback in case the
  GitHub Actions trigger ever fails).
- **Pipeline → Definition**: `Pipeline script from SCM` → **Git** → repository URL
  `https://github.com/bhavya-kmg2132/Jenkins-quincy-api` → credentials: a Git credential with
  read access → **Branch Specifier**: `*/feature_Api_dev` (case-sensitive — the real branch is
  `feature_Api_dev`, not `feature_api_dev`) → **Script Path**: `Jenkinsfile`.
- **Agent**: the Jenkinsfile requests `agent { label 'windows-iis' }`. Add that label to
  whichever Jenkins node should run the build (**Manage Jenkins → Nodes → (node) → Configure →
  Labels**) — on a single-machine setup this is the built-in node.

### 4.3 Jenkins credentials
**Manage Jenkins → Credentials → System → Global credentials**. All four are **Secret text**,
scope **Global**, and are consumed inside the Jenkinsfile's `Inject Runtime Secrets` stage (not
at pipeline start), so a build with `DEPLOY=false` never requires them to exist.

| Credential ID | Value source |
|---|---|
| `quincy-prod-sql-connstring` | SQL Server connection string the deployed app should use |
| `quincy-prod-pg-connstring` | PostgreSQL connection string the deployed app should use |
| `quincy-prod-jwt-key` | `JwtConfig:Key` |
| `quincy-prod-azuread-secret` | `AzureAd:SecretValue` |

**Important:** these must point at hosts *reachable from the deployment server*, not a
developer's workstation. `appsettings.json`'s dev value for `SqlDBConnection` originally pointed
at `QMQOL2-D` — a workstation hostname unreachable from `KMG-JPR-004` — which caused the app to
hang during dbup's automatic startup migration and made the health check fail. It's since been
corrected to a reachable IP-based server; if you rotate this credential, re-verify reachability
first:

```powershell
Test-NetConnection -ComputerName <host> -Port 1433   # SQL Server
Test-NetConnection -ComputerName <host> -Port 5432   # PostgreSQL
```

The pipeline never writes these secrets into the committed `appsettings.json`. The `Harden
Published Config` stage blanks the equivalent fields in the *published output* before deploy;
`Inject Runtime Secrets` writes the real values into the deployed `web.config`'s
`<aspNetCore><environmentVariables>`, which ASP.NET Core's configuration system reads as
overrides (`ConnectionStrings__SqlDBConnection`, `Jwt__Key`, etc.).

## 5. GitHub Actions self-hosted runner

Installed at `C:\actions-runner` on the same machine, registered against this repo.

```powershell
# One-time setup (already done on KMG-JPR-004; repeat on a new server):
mkdir C:\actions-runner; cd C:\actions-runner
Invoke-WebRequest -Uri "https://github.com/actions/runner/releases/download/v2.337.0/actions-runner-win-x64-2.337.0.zip" -OutFile actions-runner-win-x64.zip
Expand-Archive actions-runner-win-x64.zip -DestinationPath .

# Get a fresh registration token (expires in ~1 hour):
gh api -X POST repos/bhavya-kmg2132/Jenkins-quincy-api/actions/runners/registration-token

.\config.cmd --url "https://github.com/bhavya-kmg2132/Jenkins-quincy-api" --token <TOKEN> --name "<hostname>-runner" --labels "windows-iis" --work "_work" --unattended
```

**Current state on KMG-JPR-004: running as a detached background process, not a Windows
service.** It will not survive a reboot or logoff. To make it persistent, from an **elevated**
PowerShell session:

```powershell
cd C:\actions-runner\bin
.\RunnerService.exe install
.\RunnerService.exe start
```

(This requires a genuinely UAC-elevated session — a non-elevated "Administrator" account isn't
enough; the installer checks the process token, not group membership.)

To start it manually in the meantime (non-elevated, run after every reboot until the service
install is done):

```powershell
Start-Process -FilePath "C:\actions-runner\run.cmd" -WorkingDirectory "C:\actions-runner" -WindowStyle Hidden
```

### 5.1 GitHub repository secrets
**Repo → Settings → Secrets and variables → Actions**:

| Secret | Value |
|---|---|
| `JENKINS_USER` | Jenkins username used for the trigger (`admin`) |
| `JENKINS_API_TOKEN` | A dedicated Jenkins **API token** (Jenkins → user → Configure → API Token → Add new Token) — not the account password. Currently named `github-actions-trigger` in Jenkins's token list. |

Set via CLI: `gh secret set JENKINS_USER` / `gh secret set JENKINS_API_TOKEN`.

## 6. First-time verification checklist

On a new server, after completing §2–§5:

1. `dotnet build Microservices\netcoreapi\src\Api\Api.csproj -c Release` succeeds locally.
2. Jenkins job builds manually (**Build Now**, `DEPLOY=false`) — confirms Checkout/Restore/Build/Publish work before touching IIS.
3. Jenkins job builds manually with `DEPLOY=true` — confirms Web Deploy, secret injection, app pool restart, and the health check all succeed.
4. `git push` a trivial commit — confirm a GitHub Actions run appears (`gh run list`) and a matching Jenkins build starts within seconds.
5. `Invoke-WebRequest http://localhost:8081/api/health` returns `200` with `"status":"Healthy"`.

## 7. Known follow-ups (not yet done)

- **GitHub Actions runner isn't a Windows service** — see §5, needs one elevated command to fix.
- **No public GitHub webhook** — Jenkins isn't exposed to the internet. Triggering relies on the self-hosted runner (near-instant) and `pollSCM` (≤2 min fallback). If you later want a real webhook: register a free DNS hostname (e.g. DuckDNS) pointed at the server's public IP, forward ports 80/443 on the router to this machine, install IIS URL Rewrite + ARR (**requires an elevated session**, both installers explicitly check for admin privilege) as a reverse proxy to `localhost:8080`, get a Let's Encrypt cert via `win-acme`, then register the GitHub webhook against the new HTTPS URL.
- **Committed secrets in `appsettings.json`** — the dev config has real-looking SQL/Postgres passwords, an AWS key, an Azure AD client secret, and a JWT key committed in plaintext (a pre-existing issue, not introduced by this pipeline). The pipeline itself never ships these to IIS as plaintext (see §4.3), but they're still in git history — worth rotating and migrating to `dotnet user-secrets` / Key Vault for local dev.
- **`netnotificationapi` and `ApiGateway`** still carry the outdated, vulnerable package versions that were fixed in `netcoreapi` (`AutoMapper`, `Oracle.ManagedDataAccess.Core`, `System.Security.Cryptography.Xml`, etc.) — out of scope for this pipeline but worth a follow-up pass.
