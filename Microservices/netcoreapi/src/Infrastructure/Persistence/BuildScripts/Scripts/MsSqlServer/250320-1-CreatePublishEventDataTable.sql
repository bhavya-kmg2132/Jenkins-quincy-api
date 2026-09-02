IF NOT EXISTS (
    SELECT * 
    FROM sys.tables 
    WHERE name = 'PublishEventData' 
      AND schema_id = SCHEMA_ID('dbo')
)
BEGIN
    CREATE TABLE dbo.PublishEventData
    (
        Id varchar(100) DEFAULT NEWID() PRIMARY KEY,
        EventName varchar(200) NOT NULL,
        OperationType varchar(200) NOT NULL,
        OperationDateTimeUtc datetime DEFAULT GETUTCDATE() NOT NULL,
        ApiName varchar(200) NOT NULL,
        CollectionName varchar(200) NOT NULL,
        [Data] nvarchar(max) NOT NULL
    )
END
