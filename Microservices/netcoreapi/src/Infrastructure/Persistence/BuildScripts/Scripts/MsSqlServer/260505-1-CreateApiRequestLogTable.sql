IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'ApiRequestLog' AND xtype = 'U')
BEGIN
    CREATE TABLE ApiRequestLog (
        Id            BIGINT IDENTITY(1,1) PRIMARY KEY,
        CorrelationId NVARCHAR(64)  NOT NULL,
        Method        NVARCHAR(10)  NOT NULL,
        Path          NVARCHAR(500) NOT NULL,
        StatusCode    INT           NOT NULL,
        ElapsedMs     BIGINT        NOT NULL,
        Source        NVARCHAR(20)  NOT NULL,
        CreatedOn     DATETIME2     NOT NULL DEFAULT GETUTCDATE()
    );

    CREATE INDEX IX_ApiRequestLog_CorrelationId ON ApiRequestLog(CorrelationId);
    CREATE INDEX IX_ApiRequestLog_CreatedOn ON ApiRequestLog(CreatedOn DESC);
END
