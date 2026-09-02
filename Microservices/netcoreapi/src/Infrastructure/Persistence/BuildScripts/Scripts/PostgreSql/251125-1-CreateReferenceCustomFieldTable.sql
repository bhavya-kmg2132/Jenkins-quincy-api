CREATE TABLE IF NOT EXISTS public."ReferenceCustomField"
	"Id" varchar(100) primary key default gen_random_uuid(),
	"TableName" varchar(200) NOT NULL,
	"CustomFields" text NULL
	) ;