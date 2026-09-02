IF NOT EXISTS (
    SELECT * 
    FROM sys.tables 
    WHERE name = 'ReferenceCustomField' 
      AND schema_id = SCHEMA_ID('dbo')
)
BEGIN

CREATE TABLE [dbo].[ReferenceCustomField]
(
	[Id] varchar(100) primary key default newId(),
	[TableName] varchar (200) NOT NULL,
	[CustomFields] [nvarchar](max) NULL,
)

END
