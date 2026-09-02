CREATE TABLE IF NOT EXISTS public."VersionTrack"
(
    "Id" varchar(100) PRIMARY KEY DEFAULT gen_random_uuid(),
    "PlatformType" varchar(50),
    "VersionNumber" varchar(50),
    "ReleaseDate" timestamp,
    "CreatedDate" timestamp,
    "ReleaseNotes" text,
    "ReleasedBy" varchar(100),
    "ReleasedTo" varchar(100),
    "IsActive" boolean DEFAULT TRUE
);
