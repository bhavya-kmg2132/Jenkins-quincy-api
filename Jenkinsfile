// Jenkinsfile — netcoreapi CI/CD
//
// Scope: Microservices/netcoreapi only (Api + Application + Domain + Infrastructure + Resources).
// Agent: Windows Jenkins agent running on the same box as the target IIS server.
// Deploy: Microsoft Web Deploy (msdeploy.exe) sync into an existing IIS site/application.
//
// Test execution is intentionally NOT part of this pipeline yet (build + deploy only).
//
// REQUIRED Jenkins Credentials (Manage Jenkins > Credentials) — only needed when DEPLOY=true,
// since they're bound inside the "Inject Runtime Secrets" stage rather than pipeline-wide:
//   quincy-prod-sql-connstring   (Secret text) SQL connection string IIS should use at runtime
//   quincy-prod-pg-connstring    (Secret text) PostgreSQL connection string IIS should use at runtime
//   quincy-prod-jwt-key          (Secret text) JwtConfig:Key used at runtime
//   quincy-prod-azuread-secret   (Secret text) AzureAd:SecretValue used at runtime
//
// REQUIRED environment values below — update to match the real IIS install before first run:
//   IIS_APP_POOL, IIS_SITE_APP, IIS_PHYSICAL_PATH, HEALTH_CHECK_URL, MSDEPLOY_EXE
//
// TRIGGER: this job is configured as a single-branch Pipeline job ("Pipeline script from SCM").
// Currently using pollSCM (core Jenkins, no plugin required) since this Jenkins instance isn't
// yet publicly reachable for a GitHub webhook. Once it is, swap this for `triggers { githubPush() }`
// (requires the GitHub plugin) and check "GitHub hook trigger for GITScm polling" on the job.

pipeline {
    agent { label 'windows-iis' }

    options {
        disableConcurrentBuilds()
        buildDiscarder(logRotator(numToKeepStr: '20'))
    }

    triggers {
        pollSCM('H/2 * * * *')
    }

    parameters {
        choice(name: 'BUILD_CONFIGURATION', choices: ['Release', 'Debug'], description: 'dotnet build/publish configuration')
        booleanParam(name: 'DEPLOY', defaultValue: true, description: 'Deploy to IIS after a successful build. Turn off for build-only runs (e.g. feature branches).')
    }

    environment {
        // --- Project paths (scope: netcoreapi only) ---
        API_PROJECT = 'Microservices\\netcoreapi\\src\\Api\\Api.csproj'
        PUBLISH_DIR = "${WORKSPACE}\\_publish"

        // --- IIS / Web Deploy target — UPDATE THESE for the real server ---
        IIS_APP_POOL      = 'QuincyApiPool'
        IIS_SITE_APP      = 'Default Web Site/QuincyApi'          // msdeploy -dest:iisApp value
        IIS_PHYSICAL_PATH = 'C:\\inetpub\\wwwroot\\QuincyApi'     // must match IIS_SITE_APP's physical path
        HEALTH_CHECK_URL  = 'http://localhost/QuincyApi/api/health'
        MSDEPLOY_EXE      = 'C:\\Program Files\\IIS\\Microsoft Web Deploy V3\\msdeploy.exe'
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Restore') {
            steps {
                bat "dotnet restore \"%API_PROJECT%\""
            }
        }

        stage('Build') {
            steps {
                bat "dotnet build \"%API_PROJECT%\" --configuration %BUILD_CONFIGURATION% --no-restore"
            }
        }

        stage('Publish') {
            steps {
                bat """
                    dotnet publish "%API_PROJECT%" ^
                        --configuration %BUILD_CONFIGURATION% ^
                        --no-restore ^
                        -o "%PUBLISH_DIR%"
                """
            }
        }

        stage('Harden Published Config') {
            // Blanks the secrets that are committed in appsettings.json in the SOURCE-CONTROLLED
            // publish output so real credentials are never copied to the IIS box as flat text.
            // Real values are injected into web.config's <environmentVariables> in the Deploy stage.
            steps {
                powershell '''
                    $ErrorActionPreference = "Stop"
                    $settingsPath = Join-Path $env:PUBLISH_DIR "appsettings.json"
                    $json = Get-Content $settingsPath -Raw | ConvertFrom-Json

                    $json.ConnectionStrings.SqlDBConnection = ""
                    $json.ConnectionStrings.PostgreSqlDBConnection = ""
                    $json."NetAuth.ConnectionStrings".SqlDBConnection = ""
                    $json."NetAuth.ConnectionStrings".PostgreSqlDBConnection = ""
                    if ($json.Jwt) { $json.Jwt.Key = "" }
                    if ($json.AzureAd) { $json.AzureAd.SecretValue = "" }

                    $json | ConvertTo-Json -Depth 20 | Set-Content $settingsPath -Encoding UTF8
                    Write-Host "Blanked secret fields in published appsettings.json"
                '''
            }
        }

        stage('Stop App Pool') {
            when { expression { return params.DEPLOY } }
            steps {
                powershell '''
                    Import-Module WebAdministration
                    if ((Get-WebAppPoolState -Name $env:IIS_APP_POOL).Value -ne "Stopped") {
                        Stop-WebAppPool -Name $env:IIS_APP_POOL
                    }
                '''
            }
        }

        stage('Deploy via Web Deploy') {
            when { expression { return params.DEPLOY } }
            steps {
                bat """
                    "%MSDEPLOY_EXE%" -verb:sync ^
                        -source:contentPath="%PUBLISH_DIR%" ^
                        -dest:contentPath="%IIS_SITE_APP%" ^
                        -enableRule:AppOffline ^
                        -retryAttempts:3 ^
                        -retryInterval:2000 ^
                        -skip:objectName=filePath,absolutePath=".*[\\\\/]Logs[\\\\/].*"
                """
            }
        }

        stage('Inject Runtime Secrets') {
            when { expression { return params.DEPLOY } }
            // Credentials are bound here (not at the top-level environment block) so build-only
            // runs (DEPLOY=false) never require these to exist in Jenkins.
            environment {
                PROD_SQL_CONNSTRING = credentials('quincy-prod-sql-connstring')
                PROD_PG_CONNSTRING  = credentials('quincy-prod-pg-connstring')
                PROD_JWT_KEY        = credentials('quincy-prod-jwt-key')
                PROD_AZUREAD_SECRET = credentials('quincy-prod-azuread-secret')
            }
            steps {
                powershell '''
                    $ErrorActionPreference = "Stop"
                    $webConfigPath = Join-Path $env:IIS_PHYSICAL_PATH "web.config"
                    [xml]$xml = Get-Content $webConfigPath

                    $aspNetCore = $xml.configuration.location.'system.webServer'.aspNetCore
                    $envVarsNode = $aspNetCore.environmentVariables
                    if (-not $envVarsNode) {
                        $envVarsNode = $xml.CreateElement("environmentVariables")
                        $aspNetCore.AppendChild($envVarsNode) | Out-Null
                    } else {
                        $envVarsNode.RemoveAll()
                    }

                    function Set-EnvVar($name, $value) {
                        $node = $xml.CreateElement("environmentVariable")
                        $node.SetAttribute("name", $name)
                        $node.SetAttribute("value", $value)
                        $envVarsNode.AppendChild($node) | Out-Null
                    }

                    Set-EnvVar "ConnectionStrings__SqlDBConnection" $env:PROD_SQL_CONNSTRING
                    Set-EnvVar "ConnectionStrings__PostgreSqlDBConnection" $env:PROD_PG_CONNSTRING
                    Set-EnvVar "Jwt__Key" $env:PROD_JWT_KEY
                    Set-EnvVar "AzureAd__SecretValue" $env:PROD_AZUREAD_SECRET

                    $xml.Save($webConfigPath)
                    Write-Host "Injected runtime secrets into web.config environmentVariables"
                '''
            }
        }

        stage('Start App Pool') {
            when { expression { return params.DEPLOY } }
            steps {
                powershell '''
                    Import-Module WebAdministration
                    Start-WebAppPool -Name $env:IIS_APP_POOL
                '''
            }
        }

        stage('Health Check') {
            when { expression { return params.DEPLOY } }
            steps {
                powershell '''
                    $ErrorActionPreference = "Stop"
                    $maxAttempts = 10
                    $delaySeconds = 5
                    $healthy = $false

                    for ($i = 1; $i -le $maxAttempts; $i++) {
                        try {
                            $response = Invoke-WebRequest -Uri $env:HEALTH_CHECK_URL -UseBasicParsing -TimeoutSec 10
                            if ($response.StatusCode -eq 200) {
                                Write-Host "Health check passed on attempt $i"
                                $healthy = $true
                                break
                            }
                        } catch {
                            Write-Host "Attempt $i failed: $($_.Exception.Message)"
                        }
                        Start-Sleep -Seconds $delaySeconds
                    }

                    if (-not $healthy) {
                        throw "Health check did not return 200 from $env:HEALTH_CHECK_URL after $maxAttempts attempts"
                    }
                '''
            }
        }
    }

    post {
        always {
            archiveArtifacts artifacts: '_publish/**', allowEmptyArchive: true, onlyIfSuccessful: false
        }
        failure {
            echo 'Build/deploy failed. If the app pool was stopped for deployment, verify it was restarted before leaving this build.'
        }
        cleanup {
            cleanWs()
        }
    }
}
