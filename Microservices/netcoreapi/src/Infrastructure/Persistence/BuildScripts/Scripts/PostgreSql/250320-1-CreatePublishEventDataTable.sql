CREATE TABLE IF NOT EXISTS public."PublishEventData"
(
    "Id" varchar(100) PRIMARY KEY DEFAULT gen_random_uuid(),
    "EventName" varchar(200) NOT NULL,
    "OperationType" varchar(200) NOT NULL,
    "OperationDateTimeUtc" timestamp DEFAULT NOW() NOT NULL,
    "ApiName" varchar(200) NOT NULL,
    "CollectionName" varchar(200) NOT NULL,
    "Data" text NOT NULL
);
