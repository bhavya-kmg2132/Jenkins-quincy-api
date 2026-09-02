IF NOT EXISTS (
    SELECT * 
    FROM sys.tables 
    WHERE name = 'VersionTrack' 
      AND schema_id = SCHEMA_ID('dbo')
)
BEGIN

CREATE TABLE [dbo].[VersionTrack](
	[Id] [varchar](100) NOT NULL primary key default newId(),
	[PlatformType] [varchar](50) NULL,
	[VersionNumber] [varchar](50) NULL,
	[ReleaseDate] [datetime] NULL,
	[CreatedDate] [datetime] NULL,
	[ReleaseNotes] [varchar](max) NULL,
	[ReleasedBy] [varchar](100) NULL,
	[ReleasedTo] [varchar](100) NULL,
	[IsActive] [bit] NULL default 1,
)
END

