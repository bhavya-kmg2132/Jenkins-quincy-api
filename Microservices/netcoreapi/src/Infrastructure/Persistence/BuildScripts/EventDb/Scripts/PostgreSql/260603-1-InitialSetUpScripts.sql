

CREATE TABLE IF NOT EXISTS "EventStore_Quincy"
(
    "Id" character varying(100) NOT NULL DEFAULT gen_random_uuid()::text,
    "CorrelationId" character varying(100) NOT NULL,
    "AuditableRequestId" character varying(100) NOT NULL,
    "AuditableRequestName" character varying(100) NOT NULL,
    "AuditableAssemblyQualifiedName" character varying(100) NOT NULL,
    "AuditableSourceEventName" character varying(200),
    "OperationType" character varying(200) NOT NULL,
    "CreatedDateTime" timestamp with time zone NOT NULL DEFAULT (now() AT TIME ZONE 'UTC'),
    "ApiName" character varying(200),
    "CollectionName" character varying(200),
    "EventData" text NOT NULL,
    "UserId" uuid,
    CONSTRAINT "EventStore_Quincy_pkey" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS "FailedMassTransitMessage"
(
    "Id" uuid NOT NULL,
    "QueueName" text,
    "MessageType" text,
    "Payload" text,
    "Exception" text,
    "FailedDateTime" timestamp without time zone,
    CONSTRAINT "FailedMassTransitMessage_pkey" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS "NotificationRule"
(
    "Id" character varying(50),
    "NotificationName" text,
    "LastExecutionDate" timestamp with time zone,
    "Frequency" text,
    "ExecutionTime" time without time zone,
    "Role" text,
    "ExecutionDay" text,
    "IsNotificationPaused" boolean,
    "ExecutionMonth" integer
);

CREATE TABLE IF NOT EXISTS "InSystemNotification"
(
    "Id" character varying(100) NOT NULL DEFAULT gen_random_uuid()::text,
    "Message" text,
    "IsShown" boolean,
    "CreatedBy" character varying(200),
    "CreatedById" character varying(100),
    "CreatedDateTime" timestamp without time zone DEFAULT now(),
    "UpdatedBy" character varying(200),
    "UpdatedById" character varying(100),
    "UpdatedDateTime" timestamp without time zone,
    "UpdateReason" text,
    "OwnerId" character varying(100),
    "IsActive" boolean DEFAULT true,
    "IsDeleted" boolean DEFAULT false,
    "IsApproved" boolean,
    "ApproverId" character varying(100),
    "ApprovedDateTime" timestamp without time zone,
    "IsAuthorized" boolean,
    "AuthorizedById" character varying(100),
    "AuthorizedDateTime" timestamp without time zone,
    "SysData" text,
    "TenantId" text,
    "AssociatedUserId" text,
    "SubTenantId" text,
    "CustomFields" jsonb,
    CONSTRAINT "InSystemNotification_pkey" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS "NotificationUserMapping"
(
    "UserId" character varying(100) NOT NULL,
    "NotificationId" character varying(100) NOT NULL,
    "IsRead" boolean DEFAULT false,
    "ReadAt" timestamp without time zone,
    "Id" character varying(100) DEFAULT gen_random_uuid()::text,
    "CreatedBy" character varying(200),
    "CreatedById" character varying(100),
    "CreatedDateTime" timestamp without time zone DEFAULT now(),
    "UpdatedBy" character varying(200),
    "UpdatedById" character varying(100),
    "UpdatedDateTime" timestamp without time zone,
    "UpdateReason" text,
    "OwnerId" character varying(100),
    "IsActive" boolean DEFAULT true,
    "IsDeleted" boolean DEFAULT false,
    "IsApproved" boolean,
    "ApproverId" character varying(100),
    "ApprovedDateTime" timestamp without time zone,
    "IsAuthorized" boolean,
    "AuthorizedById" character varying(100),
    "AuthorizedDateTime" timestamp without time zone,
    "SysData" text,
    "TenantId" text,
    "AssociatedUserId" text,
    "SubTenantId" text,
    "CustomFields" jsonb,
    CONSTRAINT "NotificationUserMapping_pkey" PRIMARY KEY ("UserId", "NotificationId"),
    CONSTRAINT "NotificationUserMapping_NotificationId_fkey"
        FOREIGN KEY ("NotificationId")
        REFERENCES "InSystemNotification" ("Id")
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "NotificationUserSubscription"
(
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "UserId" character varying(100),
    "NotificationId" character varying(100) NOT NULL,
    "OptOut" boolean NOT NULL DEFAULT false,
    "CreatedBy" character varying(100) NOT NULL,
    "UpdatedBy" character varying(100),
    "CreatedAt" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "NotificationUserSubscription_pkey" PRIMARY KEY ("Id"),
    CONSTRAINT uq_user_notification UNIQUE ("UserId", "NotificationId")
);

CREATE TABLE IF NOT EXISTS "NotificationRequest"
(
    "Id" text NOT NULL DEFAULT gen_random_uuid()::text,
    "ApiKey" text,
    "CreatedDateTime" timestamp without time zone,
    "UpdatedDateTime" timestamp without time zone,
    "EmailFrom" text,
    "EmailTo" text,
    "EmailCc" text,
    "EmailBcc" text,
    "EmailSubject" text,
    "EmailBody" text,
    "EmailAttachments" jsonb NOT NULL,
    "NotificationErrorMessage" text,
    "NotificationDelivery" jsonb,
    "ScheduledDateTime" timestamp without time zone,
    "NotificationResponseDateTime" timestamp without time zone,
    "EntityJson" jsonb,
    "CreatedBy" text,
    "CreatedById" text,
    "UpdatedBy" text,
    "UpdatedById" text,
    "UpdateReason" text,
    "OwnerId" text,
    "IsActive" boolean DEFAULT true,
    "IsDeleted" boolean DEFAULT false,
    "IsApproved" boolean,
    "ApproverId" text,
    "ApprovedDateTime" timestamp without time zone,
    "IsAuthorized" boolean,
    "AuthorizedById" text,
    "AuthorizedDateTime" timestamp without time zone,
    "SysData" text,
    "TenantId" text,
    "AssociatedUserId" text,
    "SubTenantId" text,
    "CustomFields" jsonb,
    "EventStoreId" character varying(100),
    CONSTRAINT "PK_NotificationRequest_Id" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS "NotificationResponse"
(
    "Id" text,
    "ApiKey" text,
    "CreatedDateTime" timestamp without time zone,
    "UpdatedDateTime" timestamp without time zone,
    "EmailFrom" text,
    "EmailTo" text,
    "EmailCc" text,
    "EmailBcc" text,
    "EmailSubject" text,
    "EmailBody" text,
    "EmailAttachments" jsonb,
    "NotificationErrorMessage" text,
    "ScheduledDateTime" timestamp without time zone,
    "NotificationResponseDateTime" timestamp without time zone,
    "EntityJson" jsonb,
    "CreatedBy" text,
    "CreatedById" text,
    "UpdatedBy" text,
    "UpdatedById" text,
    "UpdateReason" text,
    "OwnerId" text,
    "IsActive" boolean DEFAULT true,
    "IsDeleted" boolean DEFAULT false,
    "IsApproved" boolean,
    "ApproverId" text,
    "ApprovedDateTime" timestamp without time zone,
    "IsAuthorized" boolean,
    "AuthorizedById" text,
    "AuthorizedDateTime" timestamp without time zone,
    "SysData" text,
    "TenantId" text,
    "AssociatedUserId" text,
    "SubTenantId" text,
    "CustomFields" jsonb,
    "NotificationDelivery" jsonb
);
