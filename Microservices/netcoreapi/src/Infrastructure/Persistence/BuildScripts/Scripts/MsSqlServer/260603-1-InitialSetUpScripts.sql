IF NOT EXISTS (
    SELECT *
    FROM sys.tables
    WHERE name = 'VersionTrack'
      AND schema_id = SCHEMA_ID('dbo')
)
BEGIN
    CREATE TABLE [dbo].[VersionTrack]
    (
        [Id] VARCHAR(100) NOT NULL DEFAULT CONVERT(VARCHAR(100), NEWID()),
        [PlatformType] VARCHAR(50) NULL,
        [VersionNumber] VARCHAR(50) NULL,
        [ReleaseDate] DATETIME NULL,
        [CreatedDate] DATETIME NULL,
        [ReleaseNotes] VARCHAR(MAX) NULL,
        [ReleasedBy] VARCHAR(100) NULL,
        [ReleasedTo] VARCHAR(100) NULL,
        [IsActive] BIT NULL DEFAULT 1,
        CONSTRAINT [PK_VersionTrack] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.tables
    WHERE name = 'ReferenceCustomField'
      AND schema_id = SCHEMA_ID('dbo')
)
BEGIN
    CREATE TABLE [dbo].[ReferenceCustomField]
    (
        [Id] VARCHAR(100) NOT NULL DEFAULT CONVERT(VARCHAR(100), NEWID()),
        [TableName] VARCHAR(200) NOT NULL,
        [CustomFields] NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_ReferenceCustomField] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.tables
    WHERE name = 'Cache'
      AND schema_id = SCHEMA_ID('dbo')
)
BEGIN
    CREATE TABLE [dbo].[Cache]
    (
        [Id] NVARCHAR(449) NOT NULL,
        [Value] VARBINARY(MAX) NOT NULL,
        [ExpiresAtTime] DATETIMEOFFSET(7) NOT NULL,
        [SlidingExpirationInSeconds] BIGINT NULL,
        [AbsoluteExpiration] DATETIMEOFFSET(7) NULL,
        CONSTRAINT [PK_TestCache] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.tables
    WHERE name = 'DeletedInMemoryCacheLog'
      AND schema_id = SCHEMA_ID('dbo')
)
BEGIN
    CREATE TABLE [dbo].[DeletedInMemoryCacheLog]
    (
        [CacheKey] NVARCHAR(449) NOT NULL,
        [DeletionTimeInUTC] DATETIME NOT NULL,
        CONSTRAINT [PK_DeletedInMemoryCacheLog] PRIMARY KEY CLUSTERED ([CacheKey] ASC)
    )
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.tables
    WHERE name = 'McaPolicy'
      AND schema_id = SCHEMA_ID('dbo')
)
BEGIN
    CREATE TABLE [dbo].[McaPolicy]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        [PolicyNumber] NVARCHAR(50) NOT NULL,
        [PolicyName] NVARCHAR(200) NULL,
        [LineOfBusinessCode] NVARCHAR(20) NOT NULL,
        [PolicyType] NVARCHAR(50) NULL,
        [StatusCode] NVARCHAR(50) NULL,
        [TransactionType] NVARCHAR(50) NULL,
        [QuoteId] NVARCHAR(100) NULL,
        [RenewalStatus] NVARCHAR(50) NULL,
        [InsuredId] NVARCHAR(100) NULL,
        [InsuredName] NVARCHAR(200) NULL,
        [InsuredAddress] NVARCHAR(500) NULL,
        [EffectiveDate] DATE NULL,
        [ExpirationDate] DATE NULL,
        [OriginalEffectiveDate] DATE NULL,
        [AccountingDate] DATE NULL,
        [CancellationDate] DATE NULL,
        [CancelReasonDescription] NVARCHAR(500) NULL,
        [TotalPremium] DECIMAL(18,2) NULL,
        [SumInsured] DECIMAL(18,2) NULL,
        [Deductible] DECIMAL(18,2) NULL,
        [Currency] NVARCHAR(10) NULL,
        [ProducerCode] NVARCHAR(50) NULL,
        [ProducerName] NVARCHAR(200) NULL,
        [UnderwriterId] NVARCHAR(100) NULL,
        [UnderwriterName] NVARCHAR(200) NULL,
        [AgentCode] NVARCHAR(50) NULL,
        [VesselName] NVARCHAR(200) NULL,
        [VesselType] NVARCHAR(100) NULL,
        [CargoType] NVARCHAR(200) NULL,
        [RouteFrom] NVARCHAR(200) NULL,
        [RouteTo] NVARCHAR(200) NULL,
        [AircraftRegistration] NVARCHAR(50) NULL,
        [FlightNumber] NVARCHAR(50) NULL,
        [RiskDescription] NVARCHAR(1000) NULL,
        [SurveyorName] NVARCHAR(200) NULL,
        [Remarks] NVARCHAR(2000) NULL,
        [CreatedBy] NVARCHAR(200) NULL,
        [CreatedById] NVARCHAR(100) NULL,
        [CreatedDateTime] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedBy] NVARCHAR(200) NULL,
        [UpdatedById] NVARCHAR(100) NULL,
        [UpdatedDateTime] DATETIME2(7) NULL,
        [CorrelationId] NVARCHAR(100) NULL,
        [AuditableRequestId] NVARCHAR(100) NULL,
        [AuditableRequestName] NVARCHAR(200) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [IsDeleted] BIT NOT NULL DEFAULT 0,
        [CustomFields] NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_McaPolicy] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.tables
    WHERE name = 'SchemaVersions'
      AND schema_id = SCHEMA_ID('dbo')
)
BEGIN
    CREATE TABLE [dbo].[SchemaVersions]
    (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [ScriptName] NVARCHAR(255) NOT NULL,
        [Applied] DATETIME NOT NULL,
        CONSTRAINT [PK_SchemaVersions_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

IF NOT EXISTS (
    SELECT *
    FROM sys.tables
    WHERE name = 'ui_config'
      AND schema_id = SCHEMA_ID('dbo')
)
BEGIN
    CREATE TABLE [dbo].[ui_config]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        [ComponentId] VARCHAR(100) NOT NULL,
        [DivId] VARCHAR(100) NOT NULL,
        [Config] NVARCHAR(MAX) NOT NULL,
        CONSTRAINT [PK_ui_config] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [UQ_ui_config_Component_Div] UNIQUE ([ComponentId], [DivId]),
        CONSTRAINT [CHK_ValidConfigJson] CHECK (ISJSON([Config]) > 0)
    )
END
GO
