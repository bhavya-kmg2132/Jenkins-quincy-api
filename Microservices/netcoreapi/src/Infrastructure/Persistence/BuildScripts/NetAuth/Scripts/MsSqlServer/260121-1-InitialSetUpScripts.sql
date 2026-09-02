-- ============================================================
-- NetAuth Initial Setup Script
-- Source of truth: MsSqlServer/InitialSetup.xml
-- All tables wrapped with IF NOT EXISTS; SPs use CREATE OR ALTER.
-- Creation order respects FK dependencies.
-- ============================================================

-- ============================================================
-- 1. UserAccessLevel  (no FK deps — must precede [User])
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserAccessLevel]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[UserAccessLevel] (
        [AccessLevel]        VARCHAR(50)   NOT NULL,
        [DisplayName]        VARCHAR(200)  NOT NULL,
        [Hierarchy]          INT           NOT NULL,
        [CreatedBy]          VARCHAR(100)  NULL,
        [CreatedDateTime]    DATETIME  DEFAULT GETUTCDATE(),
        [UpdatedBy]          VARCHAR(100)  NULL,
        [UpdatedDateTime]    DATETIME  DEFAULT GETUTCDATE(),
        [UpdateReason]       VARCHAR(100)  NULL,
        [IsDeleted]          BIT NULL  DEFAULT 0,
        [IsActive]           BIT NULL  DEFAULT 0,
        [OwnerId]            VARCHAR(100)  NULL,
        [IsApproved]         BIT NULL  DEFAULT 0,
        [ApproverId]         VARCHAR(100)  NULL,
        [ApprovedDateTime]   DATETIME  NULL,
        [IsAuthorized]       BIT NULL  DEFAULT 0,
        [AuthorizedById]     VARCHAR(100)  NULL,
        [AuthorizedDateTime] DATETIME  NULL,
        [TenantId]           VARCHAR(100)  NULL,
        [SubTenantId]        VARCHAR(100)  NULL,
        [SysData]            NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_UserAccessLevel]             PRIMARY KEY ([AccessLevel]),
        CONSTRAINT [UQ_UserAccessLevel_DisplayName] UNIQUE ([DisplayName])
    );
END
GO

EXEC('
    INSERT INTO [dbo].[UserAccessLevel] (
        [AccessLevel],[DisplayName],[Hierarchy],[CreatedDateTime],[UpdatedDateTime],
        [IsDeleted],[IsActive],[IsApproved],[IsAuthorized]
    )
    SELECT v.AccessLevel, v.DisplayName, v.Hierarchy, GETUTCDATE(), GETUTCDATE(), 0, 1, 0, 0
    FROM (VALUES
        (''L1'',''Junior Executive'',1),
        (''L2'',''Executive'',       2),
        (''L3'',''Senior Executive'',3)
    ) AS v(AccessLevel, DisplayName, Hierarchy)
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[UserAccessLevel] WHERE [AccessLevel] = v.AccessLevel
    );
')
GO

-- ============================================================
-- 2. AuthReferenceLookup  (no FK deps)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuthReferenceLookup]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[AuthReferenceLookup] (
        [Id]          VARCHAR(100) NOT NULL DEFAULT CONVERT(VARCHAR(100), NEWID()),
        [Name]        VARCHAR(100) NULL,
        [DisplayName] VARCHAR(100) NULL,
        [Type]        VARCHAR(100) NULL,
        CONSTRAINT [PK_AuthReferenceLookup]           PRIMARY KEY ([Id]),
        CONSTRAINT [UQ_AuthReferenceLookup_Name_Type] UNIQUE ([Name], [Type])
    );
END
GO

EXEC('
    INSERT INTO [dbo].[AuthReferenceLookup] ([Name],[DisplayName],[Type])
    SELECT v.[Name], v.[DisplayName], v.[Type]
    FROM (VALUES
        (''TodoItemModule'',      ''TodoItem Module'',      ''Module''),
        (''MasterUpdate'',        ''MasterUpdate'',         ''PermissionSet''),
        (''AcmeProductModule'',   ''AcmeProduct Module'',   ''Module''),
        (''SystemDesignModule'',  ''SystemDesign Module'',  ''Module''),
        (''SystemManagerModule'', ''SystemManager Module'', ''Module''),
        (''OtherModule'',         ''Other Module'',         ''Module''),
        (''PermissionView'',      ''PermissionView'',       ''PermissionSet''),
        (''UserRoleUpdate'',      ''UserRoleUpdate'',       ''PermissionSet''),
        (''TodoListView'',        ''TodoListView'',         ''PermissionSet''),
        (''SystemDesign'',        ''SystemDesign'',         ''PermissionSet''),
        (''Field'',               ''Field'',                ''UIPermissionType''),
        (''TodoListModule'',      ''TodoList Module'',      ''Module''),
        (''RoleView'',            ''RoleView'',             ''PermissionSet''),
        (''AcmeProductUpdate'',   ''AcmeProductUpdate'',    ''PermissionSet''),
        (''SystemManager'',       ''SystemManager'',        ''PermissionSet''),
        (''UserCreate'',          ''UserCreate'',           ''PermissionSet''),
        (''AcmeProductCreate'',   ''AcmeProductCreate'',    ''PermissionSet''),
        (''AcmeOrderView'',       ''AcmeOrderView'',        ''PermissionSet''),
        (''PermissionModule'',    ''Permission Module'',    ''Module''),
        (''https://netauthapi.azurewebsites.net/'',''netauthapi-timetrack'',''InternalApiUrl''),
        (''UserUpdate'',          ''UserUpdate'',           ''PermissionSet''),
        (''Other'',               ''Other'',                ''PermissionSet''),
        (''UserModule'',          ''User Module'',          ''Module''),
        (''Screen'',              ''Screen'',               ''UIPermissionType''),
        (''AcmeProductView'',     ''AcmeProductView'',      ''PermissionSet''),
        (''TodoItemCreate'',      ''TodoItemCreate'',       ''PermissionSet''),
        (''RoleModule'',          ''Role Module'',          ''Module''),
        (''MasterModule'',        ''Master Module'',        ''Module''),
        (''TodoItemUpdate'',      ''TodoItemUpdate'',       ''PermissionSet''),
        (''PermissionUpdate'',    ''PermissionUpdate'',     ''PermissionSet''),
        (''MasterView'',          ''MasterView'',           ''PermissionSet''),
        (''TodoListCreate'',      ''TodoListCreate'',       ''PermissionSet''),
        (''MasterCreate'',        ''MasterCreate'',         ''PermissionSet''),
        (''AcmeOrderCreate'',     ''AcmeOrderCreate'',      ''PermissionSet''),
        (''UserView'',            ''UserView'',             ''PermissionSet''),
        (''AcmeOrderUpdate'',     ''AcmeOrderUpdate'',      ''PermissionSet''),
        (''Menu'',                ''Menu'',                 ''UIPermissionType''),
        (''TodoItemView'',        ''TodoItemView'',         ''PermissionSet''),
        (''AcemOrderModule'',     ''AcmeOrder Module'',     ''Module''),
        (''TodoListUpdate'',      ''TodoListUpdate'',       ''PermissionSet'')
    ) AS v([Name],[DisplayName],[Type])
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[AuthReferenceLookup] a
        WHERE a.[Name] = v.[Name] AND a.[Type] = v.[Type]
    );
')
GO

-- ============================================================
-- 3. Role  (no FK deps)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Role]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Role] (
        [Id]                 VARCHAR(100)  NOT NULL DEFAULT CONVERT(VARCHAR(100), NEWID()),
        [RoleName]           VARCHAR(200)  NULL,
        [RoleValue]          VARCHAR(200)  NULL,
        [DisplayName]        VARCHAR(200)  NULL,
        [AzureRoleGuid]      VARCHAR(100)  NULL DEFAULT CONVERT(VARCHAR(100), NEWID()),
        [CreatedBy]          VARCHAR(100)  NULL,
        [CreatedDateTime]    DATETIME  DEFAULT GETUTCDATE(),
        [UpdatedBy]          VARCHAR(100)  NULL,
        [UpdatedDateTime]    DATETIME  DEFAULT GETUTCDATE(),
        [UpdateReason]       VARCHAR(100)  NULL,
        [IsDeleted]          BIT NULL  DEFAULT 0,
        [IsActive]           BIT NULL  DEFAULT 0,
        [OwnerId]            VARCHAR(100)  NULL,
        [IsApproved]         BIT NULL  DEFAULT 0,
        [ApproverId]         VARCHAR(100)  NULL,
        [ApprovedDateTime]   DATETIME  NULL,
        [IsAuthorized]       BIT NULL  DEFAULT 0,
        [AuthorizedById]     VARCHAR(100)  NULL,
        [AuthorizedDateTime] DATETIME  NULL,
        [TenantId]           VARCHAR(100)  NULL,
        [SubTenantId]        VARCHAR(100)  NULL,
        [SysData]            NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_Role]                   PRIMARY KEY ([Id]),
        CONSTRAINT [UQ_Role_Display_AzureGuid] UNIQUE ([DisplayName], [AzureRoleGuid])
    );
END
GO

EXEC('
    INSERT INTO [dbo].[Role] (
        [AzureRoleGuid],[RoleName],[RoleValue],[DisplayName],
        [IsDeleted],[IsAuthorized],[IsActive],[OwnerId],
        [CreatedBy],[CreatedDateTime],[UpdatedBy],[UpdatedDateTime],[SysData],[TenantId],[SubTenantId]
    )
    SELECT v.AzureRoleGuid, v.RoleName, v.RoleValue, v.DisplayName,
           v.IsDeleted, v.IsAuthorized, v.IsActive, v.OwnerId,
           NULL, GETUTCDATE(), NULL, GETUTCDATE(), NULL, NULL, NULL
    FROM (VALUES
        (NULL,''System Manager'',''System Manager'',''System Manager'',0,0,1,NULL),
        (NULL,''Admin'',         ''Admin'',          ''Admin'',         0,0,1,''debe9ba5-92f1-475c-9eaa-d8f86cce89cc''),
        (NULL,''User'',          ''User'',           ''User'',          0,0,1,NULL),
        (NULL,''Power'',         ''Power'',          ''Power'',         0,0,1,''3eeca4cb-3e85-4b5c-9147-ce6e5f0868f6'')
    ) AS v(AzureRoleGuid, RoleName, RoleValue, DisplayName, IsDeleted, IsAuthorized, IsActive, OwnerId)
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[Role] WHERE [DisplayName] = v.DisplayName
    );
')
GO

-- ============================================================
-- 4. User  (FK → UserAccessLevel)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[User] (
        [Id]                       VARCHAR(100)  NOT NULL DEFAULT CONVERT(VARCHAR(100), NEWID()),
        [UserName]                 VARCHAR(200)  NOT NULL,
        [EmpId]                    VARCHAR(6)    NULL,
        [EmpType]                  VARCHAR(200)  NULL,
        [FirstName]                VARCHAR(200)  NOT NULL,
        [LastName]                 VARCHAR(200)  NOT NULL,
        [Email]                    VARCHAR(200)  NULL,
        [SecondaryEmail]           VARCHAR(200)  NULL,
        [PhoneNumber]              VARCHAR(20)   NULL,
        [Extension]                VARCHAR(20)   NULL,
        [mobile]                   VARCHAR(20)   NULL,
        [oid]                      VARCHAR(100)  NULL,
        [preferred_username]       VARCHAR(200)  NULL,
        [display_name]             VARCHAR(200)  NULL,
        [given_name]               VARCHAR(200)  NULL,
        [family_name]              VARCHAR(200)  NULL,
        [Position]                 VARCHAR(200)  NULL,
        [BusinessUnit]             VARCHAR(200)  NULL,
        [ManagerId]                VARCHAR(100)  NULL,
        [Designation]              VARCHAR(200)  NULL,
        [Department]               VARCHAR(200)  NULL,
        [Location]                 VARCHAR(200)  NULL,
        [Organization]             VARCHAR(200)  NULL,
        [CreatedBy]                VARCHAR(100)  NULL,
        [CreatedDateTime]          DATETIME  DEFAULT GETUTCDATE(),
        [UpdatedBy]                VARCHAR(100)  NULL,
        [UpdatedDateTime]          DATETIME  DEFAULT GETUTCDATE(),
        [UpdateReason]             VARCHAR(100)  NULL,
        [IsDeleted]                BIT NULL  DEFAULT 0,
        [IsActive]                 BIT NULL  DEFAULT 0,
        [OwnerId]                  VARCHAR(100)  NULL,
        [IsApproved]               BIT NULL  DEFAULT 0,
        [ApproverId]               VARCHAR(100)  NULL,
        [ApprovedDateTime]         DATETIME  NULL,
        [IsAuthorized]             BIT NULL  DEFAULT 0,
        [AuthorizedById]           VARCHAR(100)  NULL,
        [AuthorizedDateTime]       DATETIME  NULL,
        [TenantId]                 VARCHAR(100)  NULL,
        [SubTenantId]              VARCHAR(100)  NULL,
        [SysData]                  NVARCHAR(MAX) NULL,
        [AccessLevel]              VARCHAR(50)   NULL,
        [CorrelationId]            VARCHAR(50)   NULL,
        [AuditableRequestId]       VARCHAR(50)   NULL,
        [AuditableRequestName]     VARCHAR(100)  NULL,
        [AuditableSourceEventName] VARCHAR(100)  NULL,
        [auth_type]                VARCHAR(200)  NULL,
        CONSTRAINT [PK_User]             PRIMARY KEY ([Id]),
        CONSTRAINT [UQ_User_Email]       UNIQUE ([Email]),
        CONSTRAINT [UQ_User_UserName]    UNIQUE ([UserName]),
        CONSTRAINT [FK_User_AccessLevel] FOREIGN KEY ([AccessLevel]) REFERENCES [dbo].[UserAccessLevel]([AccessLevel])
    );
END
GO

EXEC('
    IF NOT EXISTS (SELECT 1 FROM [dbo].[User] WHERE [UserName] = ''test_Admin_User@systemdesign.com'')
        INSERT INTO [dbo].[User] (
            [UserName],[FirstName],[LastName],[Email],[SecondaryEmail],[PhoneNumber],
            [oid],[preferred_username],[display_name],[given_name],[family_name],
            [IsDeleted],[IsAuthorized],[IsActive],[CreatedBy],[CreatedDateTime],[UpdatedDateTime],[SysData],[AccessLevel]
        ) VALUES (
            ''test_Admin_User@systemdesign.com'',''Test Admin'',''User'',
            ''test_Admin_User@systemdesign.com'',''test_Admin_User@systemdesign.com'',''77889988'',
            ''test_Admin_User@systemdesign.com'',''test_Admin_User@systemdesign.com'',''Test AdminUser'',''Test Admin'',''User'',
            0,1,1,''test_Admin_User@systemdesign.com'',GETUTCDATE(),GETUTCDATE(),''db'',''L1''
        );

    IF NOT EXISTS (SELECT 1 FROM [dbo].[User] WHERE [UserName] = ''test_Level_1_User@systemdesign.com'')
        INSERT INTO [dbo].[User] (
            [UserName],[FirstName],[LastName],[Email],[SecondaryEmail],[PhoneNumber],
            [oid],[preferred_username],[display_name],[given_name],[family_name],
            [IsDeleted],[IsAuthorized],[IsActive],[CreatedBy],[CreatedDateTime],[UpdatedDateTime],[SysData],[AccessLevel]
        ) VALUES (
            ''test_Level_1_User@systemdesign.com'',''Test Level 1'',''User'',
            ''test_Level_1_User@systemdesign.com'',''test_Level_1_User@systemdesign.com'',''1212121212'',
            ''test_Level_1_User@systemdesign.com'',''test_Level_1_User@systemdesign.com'',''Test Level 1User'',''Test Level 1'',''User'',
            0,1,1,''test_Level_1_User@systemdesign.com'',GETUTCDATE(),GETUTCDATE(),''db'',''L1''
        );

    IF NOT EXISTS (SELECT 1 FROM [dbo].[User] WHERE [UserName] = ''test_Level_2_User@systemdesign.com'')
        INSERT INTO [dbo].[User] (
            [UserName],[FirstName],[LastName],[Email],[SecondaryEmail],[PhoneNumber],
            [oid],[preferred_username],[display_name],[given_name],[family_name],
            [IsDeleted],[IsAuthorized],[IsActive],[CreatedBy],[CreatedDateTime],[UpdatedDateTime],[SysData],[AccessLevel]
        ) VALUES (
            ''test_Level_2_User@systemdesign.com'',''Test Level 2'',''User'',
            ''test_Level_2_User@systemdesign.com'',''test_Level_2_User@systemdesign.com'',''12121212121'',
            ''test_Level_2_User@systemdesign.com'',''test_Level_2_User@systemdesign.com'',''Test Level 2User'',''Test Level 2'',''User'',
            0,1,1,''test_Level_2_User@systemdesign.com'',GETUTCDATE(),GETUTCDATE(),''db'',''L1''
        );
')
GO

-- ============================================================
-- 5. AppUser  (no FK deps)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AppUser]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[AppUser] (
        [Id]              VARCHAR(100)  NOT NULL,
        [UserName]        VARCHAR(200)  NOT NULL,
        [PasswordHash]    VARCHAR(500)  NOT NULL,
        [CreatedDateTime] DATETIME2(7)  DEFAULT GETUTCDATE(),
        [UpdatedDateTime] DATETIME2(7)  DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_AppUser]              PRIMARY KEY ([Id]),
        CONSTRAINT [UQ_AppUser_UserName_key] UNIQUE ([UserName])
    );
END
GO

EXEC('
    IF NOT EXISTS (SELECT 1 FROM [dbo].[AppUser] WHERE [UserName] = ''john_doe'')
        INSERT INTO [dbo].[AppUser] ([Id],[UserName],[PasswordHash],[CreatedDateTime],[UpdatedDateTime])
        VALUES (CONVERT(VARCHAR(100),NEWID()),''john_doe'',''hashed_password_123'',GETUTCDATE(),GETUTCDATE());

    IF NOT EXISTS (SELECT 1 FROM [dbo].[AppUser] WHERE [UserName] = ''jane_smith'')
        INSERT INTO [dbo].[AppUser] ([Id],[UserName],[PasswordHash],[CreatedDateTime],[UpdatedDateTime])
        VALUES (CONVERT(VARCHAR(100),NEWID()),''jane_smith'',''hashed_password_456'',GETUTCDATE(),GETUTCDATE());
')
GO

-- ============================================================
-- 6. Permission  (FK → AuthReferenceLookup)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Permission]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Permission] (
        [Id]                    VARCHAR(100)  NOT NULL DEFAULT CONVERT(VARCHAR(100), NEWID()),
        [PermissionValue]       VARCHAR(200)  NOT NULL,
        [PermissionDisplayName] VARCHAR(200)  NOT NULL,
        [CreatedBy]             VARCHAR(100)  NULL,
        [CreatedDateTime]       DATETIME  DEFAULT GETUTCDATE(),
        [UpdatedBy]             VARCHAR(100)  NULL,
        [UpdatedDateTime]       DATETIME  DEFAULT GETUTCDATE(),
        [UpdateReason]          VARCHAR(100)  NULL,
        [IsDeleted]             BIT NULL  DEFAULT 0,
        [IsActive]              BIT NULL  DEFAULT 0,
        [OwnerId]               VARCHAR(100)  NULL,
        [IsApproved]            BIT NULL  DEFAULT 0,
        [ApproverId]            VARCHAR(100)  NULL,
        [ApprovedDateTime]      DATETIME  NULL,
        [IsAuthorized]          BIT NULL  DEFAULT 0,
        [AuthorizedById]        VARCHAR(100)  NULL,
        [AuthorizedDateTime]    DATETIME  NULL,
        [TenantId]              VARCHAR(100)  NULL,
        [SubTenantId]           VARCHAR(100)  NULL,
        [SysData]               NVARCHAR(MAX) NULL,
        [ModuleId]              VARCHAR(100)  NOT NULL,
        [PermissionSetId]       VARCHAR(100)  NOT NULL,
        [ApiName]               VARCHAR(200)  NULL,
        [PermissionType]        VARCHAR(200)  NULL,
        CONSTRAINT [PK_Permission]                PRIMARY KEY ([Id]),
        CONSTRAINT [UQ_Permission_Value]           UNIQUE ([PermissionValue]),
        CONSTRAINT [UQ_Permission_DisplayName]     UNIQUE ([PermissionDisplayName]),
        CONSTRAINT [FK_Permission_ModuleId]        FOREIGN KEY ([ModuleId])        REFERENCES [dbo].[AuthReferenceLookup]([Id]),
        CONSTRAINT [FK_Permission_PermissionSetId] FOREIGN KEY ([PermissionSetId]) REFERENCES [dbo].[AuthReferenceLookup]([Id])
    );
END
GO

EXEC('
    INSERT INTO [dbo].[Permission] (
        [PermissionValue],[PermissionDisplayName],[CreatedBy],[CreatedDateTime],
        [UpdatedBy],[UpdatedDateTime],[IsDeleted],[IsActive],[IsApproved],[IsAuthorized],
        [ModuleId],[PermissionSetId],[ApiName]
    )
    SELECT v.PermissionValue, v.PermissionDisplayName, v.CreatedBy, GETUTCDATE(),
           NULL, GETUTCDATE(), 0, 1, 0, 1,
           m.Id, ps.Id, v.ApiName
    FROM (VALUES
        (''GetUsersQuery'',           ''Get Users Query'',          NULL,                           ''UserModule'',       ''UserView'',         ''User/GetUsersAsync''),
        (''GetAcmeProductListQuery'', ''GetAcmeProductListQuery'',  ''john.smith@kmgus.com'',       ''AcmeProductModule'',''AcmeProductView'',  NULL),
        (''GetUsersByStatusQuery'',   ''GetUsersByStatusQuery'',    ''john.smith@kmgus.com'',       ''UserModule'',       ''UserView'',         NULL),
        (''GetRolesQuery'',           ''GetRolesQuery'',            ''john.smith@kmgus.com'',       ''RoleModule'',       ''RoleView'',         NULL),
        (''UpdateAcmeProductRequest'',''UpdateAcmeProductRequest'', ''john.smith@kmgus.com'',       ''AcmeProductModule'',''AcmeProductUpdate'',NULL),
        (''GetUserActivitiesQuery'',  ''GetUserActivitiesQuery'',   ''john.smith@kmgus.com'',       ''UserModule'',       ''UserView'',         NULL),
        (''GetAcmeProductByIdQuery'', ''GetAcmeProductByIdQuery'',  ''john.smith@kmgus.com'',       ''AcmeProductModule'',''AcmeProductView'',  NULL),
        (''DeleteAcmeProductRequest'',''DeleteAcmeProductRequest'', ''john.smith@kmgus.com'',       ''AcmeProductModule'',''AcmeProductUpdate'',NULL),
        (''CreateAcmeProductRequest'',''CreateAcmeProductRequest'', ''john.smith@kmgus.com'',       ''AcmeProductModule'',''AcmeProductCreate'',NULL),
        (''GetPermissionsQueryAsync'',''GetPermissionsQueryAsync'',  ''john.smith@kmgus.com'',       ''PermissionModule'', ''PermissionView'',   NULL),
        (''GetAcmeOrderListQuery'',   ''GetAcmeOrderListQuery'',    ''leeladhar.kumawat@kmgus.com'',''AcemOrderModule'',  ''AcmeOrderView'',    NULL),
        (''GetAcmeOrderByIdQuery'',   ''GetAcmeOrderByIdQuery'',    ''leeladhar.kumawat@kmgus.com'',''AcemOrderModule'',  ''AcmeOrderView'',    NULL),
        (''CreateAcmeOrderRequest'',  ''CreateAcmeOrderRequest'',   ''leeladhar.kumawat@kmgus.com'',''AcemOrderModule'',  ''AcmeOrderCreate'',  NULL),
        (''UpdateAcmeOrderRequest'',  ''UpdateAcmeOrderRequest'',   ''leeladhar.kumawat@kmgus.com'',''AcemOrderModule'',  ''AcmeOrderUpdate'',  NULL),
        (''DeleteAcmeOrderRequest'',  ''DeleteAcmeOrderRequest'',   ''leeladhar.kumawat@kmgus.com'',''AcemOrderModule'',  ''AcmeOrderUpdate'',  NULL)
    ) AS v(PermissionValue, PermissionDisplayName, CreatedBy, ModuleName, PermissionSetName, ApiName)
    JOIN [dbo].[AuthReferenceLookup] m  ON m.[Name] = v.ModuleName       AND m.[Type] = ''Module''
    JOIN [dbo].[AuthReferenceLookup] ps ON ps.[Name] = v.PermissionSetName AND ps.[Type] = ''PermissionSet''
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[Permission] WHERE [PermissionValue] = v.PermissionValue
    );
')
GO

-- ============================================================
-- 7. PermissionDenied  (FK → Permission, User)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PermissionDenied]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[PermissionDenied] (
        [Id]                 VARCHAR(100)  NOT NULL DEFAULT CONVERT(VARCHAR(100), NEWID()),
        [UserId]             VARCHAR(100)  NOT NULL,
        [PermissionId]       VARCHAR(100)  NOT NULL,
        [CreatedBy]          VARCHAR(100)  NULL,
        [CreatedDateTime]    DATETIME  DEFAULT GETUTCDATE(),
        [UpdatedBy]          VARCHAR(100)  NULL,
        [UpdatedDateTime]    DATETIME  DEFAULT GETUTCDATE(),
        [UpdateReason]       VARCHAR(100)  NULL,
        [IsDeleted]          BIT NULL  DEFAULT 0,
        [IsActive]           BIT NULL  DEFAULT 0,
        [OwnerId]            VARCHAR(100)  NULL,
        [IsApproved]         BIT NULL  DEFAULT 0,
        [ApproverId]         VARCHAR(100)  NULL,
        [ApprovedDateTime]   DATETIME  NULL,
        [IsAuthorized]       BIT NULL  DEFAULT 0,
        [AuthorizedById]     VARCHAR(100)  NULL,
        [AuthorizedDateTime] DATETIME  NULL,
        [TenantId]           VARCHAR(100)  NULL,
        [SubTenantId]        VARCHAR(100)  NULL,
        [SysData]            NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_PermissionDenied]                 PRIMARY KEY ([Id]),
        CONSTRAINT [UC_PermissionDenied_User_Permission] UNIQUE ([UserId], [PermissionId]),
        CONSTRAINT [FK_PermissionDenied_Permission]      FOREIGN KEY ([PermissionId]) REFERENCES [dbo].[Permission]([Id]),
        CONSTRAINT [FK_PermissionDenied_User]            FOREIGN KEY ([UserId])       REFERENCES [dbo].[User]([Id])
    );
END
GO

-- ============================================================
-- 8. PermissionGranted  (FK → Permission, User)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PermissionGranted]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[PermissionGranted] (
        [Id]                 VARCHAR(100)  NOT NULL DEFAULT CONVERT(VARCHAR(100), NEWID()),
        [UserId]             VARCHAR(100)  NOT NULL,
        [PermissionId]       VARCHAR(100)  NOT NULL,
        [CreatedBy]          VARCHAR(100)  NULL,
        [CreatedDateTime]    DATETIME  DEFAULT GETUTCDATE(),
        [UpdatedBy]          VARCHAR(100)  NULL,
        [UpdatedDateTime]    DATETIME  DEFAULT GETUTCDATE(),
        [UpdateReason]       VARCHAR(100)  NULL,
        [IsDeleted]          BIT NULL  DEFAULT 0,
        [IsActive]           BIT NULL  DEFAULT 0,
        [OwnerId]            VARCHAR(100)  NULL,
        [IsApproved]         BIT NULL  DEFAULT 0,
        [ApproverId]         VARCHAR(100)  NULL,
        [ApprovedDateTime]   DATETIME  NULL,
        [IsAuthorized]       BIT NULL  DEFAULT 0,
        [AuthorizedById]     VARCHAR(100)  NULL,
        [AuthorizedDateTime] DATETIME  NULL,
        [TenantId]           VARCHAR(100)  NULL,
        [SubTenantId]        VARCHAR(100)  NULL,
        [SysData]            NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_PermissionGranted]                PRIMARY KEY ([Id]),
        CONSTRAINT [UC_PermissionGranted_User_Permission] UNIQUE ([UserId], [PermissionId]),
        CONSTRAINT [FK_PermissionGranted_Permission]     FOREIGN KEY ([PermissionId]) REFERENCES [dbo].[Permission]([Id]),
        CONSTRAINT [FK_PermissionGranted_User]           FOREIGN KEY ([UserId])       REFERENCES [dbo].[User]([Id])
    );
END
GO

-- ============================================================
-- 9. RolePermission  (FK → Role, Permission)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RolePermission]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[RolePermission] (
        [Id]                 VARCHAR(100)  NOT NULL DEFAULT CONVERT(VARCHAR(100), NEWID()),
        [RoleId]             VARCHAR(100)  NULL,
        [PermissionId]       VARCHAR(100)  NULL,
        [CreatedBy]          VARCHAR(100)  NULL,
        [CreatedDateTime]    DATETIME  DEFAULT GETUTCDATE(),
        [UpdatedBy]          VARCHAR(100)  NULL,
        [UpdatedDateTime]    DATETIME  DEFAULT GETUTCDATE(),
        [UpdateReason]       VARCHAR(100)  NULL,
        [IsDeleted]          BIT NULL  DEFAULT 0,
        [IsActive]           BIT NULL  DEFAULT 0,
        [OwnerId]            VARCHAR(100)  NULL,
        [IsApproved]         BIT NULL  DEFAULT 0,
        [ApproverId]         VARCHAR(100)  NULL,
        [ApprovedDateTime]   DATETIME  NULL,
        [IsAuthorized]       BIT NULL  DEFAULT 0,
        [AuthorizedById]     VARCHAR(100)  NULL,
        [AuthorizedDateTime] DATETIME  NULL,
        [TenantId]           VARCHAR(100)  NULL,
        [SubTenantId]        VARCHAR(100)  NULL,
        [SysData]            NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_RolePermission]            PRIMARY KEY ([Id]),
        CONSTRAINT [UC_RolePermission]            UNIQUE ([RoleId], [PermissionId]),
        CONSTRAINT [FK_RolePermission_Permission] FOREIGN KEY ([PermissionId]) REFERENCES [dbo].[Permission]([Id]),
        CONSTRAINT [FK_RolePermission_Role]       FOREIGN KEY ([RoleId])       REFERENCES [dbo].[Role]([Id])
    );
END
GO

EXEC('
    INSERT INTO [dbo].[RolePermission] (
        [RoleId],[PermissionId],[CreatedBy],[CreatedDateTime],
        [UpdatedBy],[UpdatedDateTime],[IsDeleted],[IsActive],[IsApproved],[IsAuthorized]
    )
    SELECT r.[Id], p.[Id], NULL, GETUTCDATE(), NULL, GETUTCDATE(), 0, 1, 0, 1
    FROM [dbo].[Role] r
    CROSS JOIN [dbo].[Permission] p
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[RolePermission] rp
        WHERE rp.[RoleId] = r.[Id] AND rp.[PermissionId] = p.[Id]
    );
')
GO

-- ============================================================
-- 10. RoleUiPermission  (FK → Role)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RoleUiPermission]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[RoleUiPermission] (
        [RoleId]             VARCHAR(100)  NOT NULL,
        [UiPermissionId]     VARCHAR(100)  NOT NULL,
        [CanView]            BIT NULL  DEFAULT 0,
        [CanEdit]            BIT NULL  DEFAULT 0,
        [CanCreate]          BIT NULL  DEFAULT 0,
        [CanDelete]          BIT NULL  DEFAULT 0,
        [CreatedBy]          VARCHAR(100)  NULL,
        [CreatedDateTime]    DATETIME  DEFAULT GETUTCDATE(),
        [UpdatedBy]          VARCHAR(100)  NULL,
        [UpdatedDateTime]    DATETIME  DEFAULT GETUTCDATE(),
        [UpdateReason]       VARCHAR(100)  NULL,
        [IsDeleted]          BIT NULL  DEFAULT 0,
        [IsActive]           BIT NULL  DEFAULT 1,
        [OwnerId]            VARCHAR(100)  NULL,
        [IsApproved]         BIT NULL  DEFAULT 0,
        [ApproverId]         VARCHAR(100)  NULL,
        [ApprovedDateTime]   DATETIME  NULL,
        [IsAuthorized]       BIT NULL  DEFAULT 0,
        [AuthorizedById]     VARCHAR(100)  NULL,
        [AuthorizedDateTime] DATETIME  NULL,
        [TenantId]           VARCHAR(100)  NULL,
        [SubTenantId]        VARCHAR(100)  NULL,
        [SysData]            NVARCHAR(MAX) NULL,
        CONSTRAINT [UC_RoleUiPermission]      UNIQUE ([RoleId], [UiPermissionId]),
        CONSTRAINT [FK_RoleUiPermission_Role] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role]([Id])
    );
END
GO

-- ============================================================
-- 11. UiPermission  (FK → AuthReferenceLookup)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UiPermission]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[UiPermission] (
        [Id]                    VARCHAR(100)  NOT NULL DEFAULT CONVERT(VARCHAR(100), NEWID()),
        [PermissionValue]       VARCHAR(200)  NOT NULL,
        [PermissionDisplayName] VARCHAR(200)  NOT NULL,
        [PermissionParentId]    VARCHAR(100)  NULL,
        [CreatedBy]             VARCHAR(100)  NULL,
        [CreatedDateTime]       DATETIME  DEFAULT GETUTCDATE(),
        [UpdatedBy]             VARCHAR(100)  NULL,
        [UpdatedDateTime]       DATETIME  DEFAULT GETUTCDATE(),
        [UpdateReason]          VARCHAR(100)  NULL,
        [IsDeleted]             BIT NULL  DEFAULT 0,
        [IsActive]              BIT NULL  DEFAULT 0,
        [OwnerId]               VARCHAR(100)  NULL,
        [IsApproved]            BIT NULL  DEFAULT 0,
        [ApproverId]            VARCHAR(100)  NULL,
        [ApprovedDateTime]      DATETIME  NULL,
        [IsAuthorized]          BIT NULL  DEFAULT 0,
        [AuthorizedById]        VARCHAR(100)  NULL,
        [AuthorizedDateTime]    DATETIME  NULL,
        [TenantId]              VARCHAR(100)  NULL,
        [SubTenantId]           VARCHAR(100)  NULL,
        [SysData]               NVARCHAR(MAX) NULL,
        [ModuleId]              VARCHAR(100)  NOT NULL,
        [UiPermissionTypeId]    VARCHAR(100)  NULL,
        CONSTRAINT [PK_UiPermission]                 PRIMARY KEY ([Id]),
        CONSTRAINT [UQ_UiPermission_PermissionValue] UNIQUE ([PermissionValue]),
        CONSTRAINT [FK_UiPermission_Type]            FOREIGN KEY ([UiPermissionTypeId]) REFERENCES [dbo].[AuthReferenceLookup]([Id]),
        CONSTRAINT [FK_UiPermission_Module]          FOREIGN KEY ([ModuleId])           REFERENCES [dbo].[AuthReferenceLookup]([Id])
    );
END
GO

-- ============================================================
-- 12. UiPermissionDenied  (no FK constraints in schema)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UiPermissionDenied]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[UiPermissionDenied] (
        [UserId]             VARCHAR(100)  NULL,
        [UiPermissionId]     VARCHAR(100)  NULL,
        [CanView]            BIT NULL  DEFAULT 0,
        [CanEdit]            BIT NULL  DEFAULT 0,
        [CanCreate]          BIT NULL  DEFAULT 0,
        [CanDelete]          BIT NULL  DEFAULT 0,
        [CreatedBy]          VARCHAR(100)  NULL,
        [CreatedDateTime]    DATETIME  DEFAULT GETUTCDATE(),
        [UpdatedBy]          VARCHAR(100)  NULL,
        [UpdatedDateTime]    DATETIME  DEFAULT GETUTCDATE(),
        [UpdateReason]       VARCHAR(100)  NULL,
        [IsDeleted]          BIT NULL  DEFAULT 0,
        [IsActive]           BIT NULL  DEFAULT 1,
        [OwnerId]            VARCHAR(100)  NULL,
        [IsApproved]         BIT NULL  DEFAULT 0,
        [ApproverId]         VARCHAR(100)  NULL,
        [ApprovedDateTime]   DATETIME  NULL,
        [IsAuthorized]       BIT NULL  DEFAULT 0,
        [AuthorizedById]     VARCHAR(100)  NULL,
        [AuthorizedDateTime] DATETIME  NULL,
        [TenantId]           VARCHAR(100)  NULL,
        [SubTenantId]        VARCHAR(100)  NULL,
        [SysData]            NVARCHAR(MAX) NULL,
        CONSTRAINT [UC_UserIdUiPermissionDenied] UNIQUE ([UserId], [UiPermissionId])
    );
END
GO

-- ============================================================
-- 13. UiPermissionGranted  (no FK constraints in schema)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UiPermissionGranted]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[UiPermissionGranted] (
        [UserId]             VARCHAR(100)  NULL,
        [UiPermissionId]     VARCHAR(100)  NULL,
        [CanView]            BIT NULL  DEFAULT 0,
        [CanEdit]            BIT NULL  DEFAULT 0,
        [CanCreate]          BIT NULL  DEFAULT 0,
        [CanDelete]          BIT NULL  DEFAULT 0,
        [CreatedBy]          VARCHAR(100)  NULL,
        [CreatedDateTime]    DATETIME  DEFAULT GETUTCDATE(),
        [UpdatedBy]          VARCHAR(100)  NULL,
        [UpdatedDateTime]    DATETIME  DEFAULT GETUTCDATE(),
        [UpdateReason]       VARCHAR(100)  NULL,
        [IsDeleted]          BIT NULL  DEFAULT 0,
        [IsActive]           BIT NULL  DEFAULT 1,
        [OwnerId]            VARCHAR(100)  NULL,
        [IsApproved]         BIT NULL  DEFAULT 0,
        [ApproverId]         VARCHAR(100)  NULL,
        [ApprovedDateTime]   DATETIME  NULL,
        [IsAuthorized]       BIT NULL  DEFAULT 0,
        [AuthorizedById]     VARCHAR(100)  NULL,
        [AuthorizedDateTime] DATETIME  NULL,
        [TenantId]           VARCHAR(100)  NULL,
        [SubTenantId]        VARCHAR(100)  NULL,
        [SysData]            NVARCHAR(MAX) NULL,
        CONSTRAINT [UC_UserIdUiPermissionGranted] UNIQUE ([UserId], [UiPermissionId])
    );
END
GO

-- ============================================================
-- 14. UserActivity  (FK → User)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserActivity]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[UserActivity] (
        [Id]                   VARCHAR(100)  NOT NULL DEFAULT CONVERT(VARCHAR(100), NEWID()),
        [UserId]               VARCHAR(100)  NOT NULL,
        [LastLoginDateTime]    DATETIME  NULL,
        [LastLogoutDateTime]   DATETIME  NULL,
        [LastActivityDateTime] DATETIME  NULL,
        [LastActivityModule]   VARCHAR(200)  NULL,
        [LastActionType]       VARCHAR(200)  NULL,
        [LastActivityDetail]   NVARCHAR(MAX) NULL,
        [CreatedBy]            VARCHAR(100)  NULL,
        [CreatedDateTime]      DATETIME  DEFAULT GETUTCDATE(),
        [UpdatedBy]            VARCHAR(100)  NULL,
        [UpdatedDateTime]      DATETIME  DEFAULT GETUTCDATE(),
        [UpdateReason]         VARCHAR(100)  NULL,
        [IsDeleted]            BIT NULL  DEFAULT 0,
        [IsActive]             BIT NULL  DEFAULT 1,
        [OwnerId]              VARCHAR(100)  NULL,
        [IsApproved]           BIT NULL  DEFAULT 0,
        [ApproverId]           VARCHAR(100)  NULL,
        [ApprovedDateTime]     DATETIME  NULL,
        [IsAuthorized]         BIT NULL  DEFAULT 0,
        [AuthorizedById]       VARCHAR(100)  NULL,
        [AuthorizedDateTime]   DATETIME  NULL,
        [TenantId]             VARCHAR(100)  NULL,
        [SubTenantId]          VARCHAR(100)  NULL,
        [SysData]              NVARCHAR(MAX) NULL,
        [CustomFields]         NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_UserActivity]                   PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserActivity_User]              FOREIGN KEY ([UserId]) REFERENCES [dbo].[User]([Id]),
        CONSTRAINT [ck_customfieldsUserActivity_json] CHECK (isjson([CustomFields])=(1))
    );
END
GO

-- ============================================================
-- 15. UserPasswordHash  (FK → User)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserPasswordHash]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[UserPasswordHash] (
        [UserId]          VARCHAR(100) NOT NULL,
        [PasswordHash]    VARCHAR(200) NULL,
        [UpdatedBy]       VARCHAR(200) NULL,
        [UpdatedDateTime] DATETIME NULL,
        [UpdateReason]    VARCHAR(200) NULL,
        CONSTRAINT [PK_UserPasswordHash]      PRIMARY KEY ([UserId]),
        CONSTRAINT [FK_UserPasswordHash_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User]([Id])
    );
END
GO

EXEC('
    INSERT INTO [dbo].[UserPasswordHash] ([UserId],[PasswordHash],[UpdatedBy],[UpdatedDateTime],[UpdateReason])
    SELECT u.[Id], v.PasswordHash, NULL, GETUTCDATE(), ''Initial BCrypt password''
    FROM (VALUES
        (''test_Admin_User@systemdesign.com'',   ''$2a$11$LzB1OPiO/FoyQ8IPlXKND.IzDmSloCb5IZZ7LcCCD55U36fWIfgAO''),
        (''test_Level_1_User@systemdesign.com'', ''$2a$11$8VhImHUy.k9ekQ.Jao3S6Oy3ZavD726tq8/Cyy.RsiDTuCLThH5nS''),
        (''test_Level_2_User@systemdesign.com'', ''$2a$11$CCFTX7LQ54m.RQ5IvlnCPuQOIhULF95IBvwEMB45WmGiGJQ6YCB56'')
    ) AS v(UserName, PasswordHash)
    JOIN [dbo].[User] u ON u.[UserName] = v.UserName
    WHERE NOT EXISTS (SELECT 1 FROM [dbo].[UserPasswordHash] WHERE [UserId] = u.[Id]);
')
GO

-- ============================================================
-- 16. UserProfile  (FK → User)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserProfile]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[UserProfile] (
        [Id]                     VARCHAR(100)  NOT NULL DEFAULT CONVERT(VARCHAR(100), NEWID()),
        [UserId]                 VARCHAR(100)  NOT NULL,
        [DOB]                    VARCHAR(200)  NULL,
        [Gender]                 VARCHAR(200)  NULL,
        [BloodGroup]             VARCHAR(200)  NULL,
        [PersonalEmail]          VARCHAR(200)  NULL,
        [DateOfJoining]          VARCHAR(200)  NULL,
        [PassportNumber]         VARCHAR(100)  NULL,
        [FatherName]             VARCHAR(200)  NULL,
        [MotherName]             VARCHAR(200)  NULL,
        [MaritalStatus]          VARCHAR(200)  NULL,
        [WeddingAnniversaryDate] VARCHAR(200)  NULL,
        [SpouseName]             VARCHAR(200)  NULL,
        [SpouseDOB]              VARCHAR(200)  NULL,
        [HomeAddress1]           VARCHAR(200)  NULL,
        [HomeAddress2]           VARCHAR(200)  NULL,
        [City]                   VARCHAR(200)  NULL,
        [State]                  VARCHAR(200)  NULL,
        [HomeAddressCity]        VARCHAR(200)  NULL,
        [HomeAddressState]       VARCHAR(200)  NULL,
        [HomeAddressCountry]     VARCHAR(200)  NULL,
        [HomePhoneNumber]        VARCHAR(200)  NULL,
        [EmergencyContactNumber] VARCHAR(200)  NULL,
        [EmergencyContactName]   VARCHAR(200)  NULL,
        [PrimarySkills]          VARCHAR(200)  NULL,
        [SecondarySkills]        VARCHAR(200)  NULL,
        [TertiarySkills]         VARCHAR(200)  NULL,
        [OtherSkills]            VARCHAR(200)  NULL,
        [Branch]                 VARCHAR(200)  NULL,
        [LookUpCode]             VARCHAR(200)  NULL,
        [OtherId]                VARCHAR(200)  NULL,
        [LinkedInUrl]            VARCHAR(200)  NULL,
        [UserPic]                VARCHAR(100)  NULL,
        [CreatedBy]              VARCHAR(100)  NULL,
        [CreatedDateTime]        DATETIME  DEFAULT GETUTCDATE(),
        [UpdatedBy]              VARCHAR(100)  NULL,
        [UpdatedDateTime]        DATETIME  DEFAULT GETUTCDATE(),
        [UpdateReason]           VARCHAR(100)  NULL,
        [IsDeleted]              BIT NULL  DEFAULT 0,
        [IsActive]               BIT NULL  DEFAULT 0,
        [OwnerId]                VARCHAR(100)  NULL,
        [IsApproved]             BIT NULL  DEFAULT 0,
        [ApproverId]             VARCHAR(100)  NULL,
        [ApprovedDateTime]       DATETIME  NULL,
        [IsAuthorized]           BIT NULL  DEFAULT 0,
        [AuthorizedById]         VARCHAR(100)  NULL,
        [AuthorizedDateTime]     DATETIME  NULL,
        [TenantId]               VARCHAR(100)  NULL,
        [SubTenantId]            VARCHAR(100)  NULL,
        [SysData]                NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_UserProfile]      PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserProfile_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User]([Id])
    );
END
GO

-- ============================================================
-- 17. UserRole  (FK → User, Role)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserRole]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[UserRole] (
        [Id]                       VARCHAR(100)  NOT NULL DEFAULT CONVERT(VARCHAR(100), NEWID()),
        [UserId]                   VARCHAR(100)  NOT NULL,
        [RoleId]                   VARCHAR(100)  NOT NULL,
        [CreatedBy]                VARCHAR(100)  NULL,
        [CreatedDateTime]          DATETIME  DEFAULT GETUTCDATE(),
        [UpdatedBy]                VARCHAR(100)  NULL,
        [UpdatedDateTime]          DATETIME  DEFAULT GETUTCDATE(),
        [UpdateReason]             VARCHAR(100)  NULL,
        [IsDeleted]                BIT NULL  DEFAULT 0,
        [IsActive]                 BIT NULL  DEFAULT 0,
        [OwnerId]                  VARCHAR(100)  NULL,
        [IsApproved]               BIT NULL  DEFAULT 0,
        [ApproverId]               VARCHAR(100)  NULL,
        [ApprovedDateTime]         DATETIME  NULL,
        [IsAuthorized]             BIT NULL  DEFAULT 0,
        [AuthorizedById]           VARCHAR(100)  NULL,
        [AuthorizedDateTime]       DATETIME  NULL,
        [TenantId]                 VARCHAR(100)  NULL,
        [SubTenantId]              VARCHAR(100)  NULL,
        [SysData]                  NVARCHAR(MAX) NULL,
        [CorrelationId]            VARCHAR(50)   NULL,
        [AuditableRequestId]       VARCHAR(50)   NULL,
        [AuditableRequestName]     VARCHAR(100)  NULL,
        [AuditableSourceEventName] VARCHAR(100)  NULL,
        CONSTRAINT [UserRole_pkey]         PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserRole_User]      FOREIGN KEY ([UserId]) REFERENCES [dbo].[User]([Id]),
        CONSTRAINT [FK_UserRole_Role]      FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role]([Id]),
        CONSTRAINT [UC_UserRole_User_Role] UNIQUE ([UserId], [RoleId])
    );
END
GO

EXEC('
    INSERT INTO [dbo].[UserRole] (
        [UserId],[RoleId],[CreatedBy],[CreatedDateTime],
        [UpdatedBy],[UpdatedDateTime],[IsDeleted],[IsActive],[IsApproved],[IsAuthorized]
    )
    SELECT u.[Id], r.[Id], NULL, GETUTCDATE(), NULL, GETUTCDATE(), 0, 1, 0, 1
    FROM [dbo].[User] u
    JOIN [dbo].[Role] r ON r.[RoleName] = ''User''
    WHERE NOT EXISTS (
        SELECT 1 FROM [dbo].[UserRole] ur
        WHERE ur.[UserId] = u.[Id] AND ur.[RoleId] = r.[Id]
    );
')
GO

-- ============================================================
-- 18. RefreshToken  (no FK deps)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RefreshToken]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[RefreshToken] (
        [Id]          VARCHAR(100) NOT NULL DEFAULT CONVERT(VARCHAR(100), NEWID()),
        [UserId]      VARCHAR(100) NULL,
        [Token]       VARCHAR(500) NULL,
        [ExpiryDate]  DATETIME     NULL,
        [IsRevoked]   BIT          NOT NULL DEFAULT 0,
        [RevokedDate] DATETIME     NULL,
        [CreatedDate] DATETIME     NULL,
        CONSTRAINT [PK_RefreshToken] PRIMARY KEY ([Id])
    );
END
GO

-- ============================================================
-- 19. SchemaVersions  (dbup tracking table)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SchemaVersions]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[SchemaVersions] (
        [Id]         INT           IDENTITY(1,1) NOT NULL,
        [ScriptName] NVARCHAR(255) NOT NULL,
        [Applied]    DATETIME      NOT NULL,
        CONSTRAINT [PK_SchemaVersions_Id] PRIMARY KEY ([Id])
    );
END
GO

-- ============================================================
-- STORED PROCEDURES  (CREATE OR ALTER = idempotent)
-- ============================================================

CREATE OR ALTER PROCEDURE [dbo].[sp_Identity_SelectUser]
    @userName_userId_userOid varchar(100)
AS
BEGIN
    Declare @UserId varchar(100) = (Select Id from [User] where (Id = @userName_userId_userOid OR oid = @userName_userId_userOid OR UserName = @userName_userId_userOid))

    Select U.Id as userId, U.EmpId, U.EmpType, U.UserName, U.FirstName, U.LastName, U.Email, U.SecondaryEmail, U.PhoneNumber, U.Extension,
     U.mobile, U.oid, U.preferred_username, U.display_name, U.given_name, U.family_name, U.Position, U.BusinessUnit,
     u.ManagerId, M.UserName As ManagerUserName, U.AccessLevel, U.IsActive, U.Designation, U.Department, U.Location, U.Organization,
     U.CorrelationId, U.AuditableRequestId, U.AuditableRequestName, U.AuditableSourceEventName, U.CreatedBy, U.CreatedDateTime, U.UpdatedBy,
     U.UpdatedDateTime, U.UpdateReason, U.OwnerId, U.IsDeleted, U.IsApproved, U.ApproverId, U.ApprovedDateTime,
     U.IsAuthorized, U.AuthorizedById, U.AuthorizedDateTime, U.SysData, U.TenantId, U.SubTenantId
     from [dbo].[User] as U
     Left JOIN [dbo].[User] as M On U.ManagerId = M.Id And (M.IsDeleted Is NUll Or M.IsDeleted = 0)
     where (U.Id = @UserId OR U.OID = @UserId)
     ORDER BY U.display_name ASC

    Select UR.Id As UserRoleId, R.Id As RoleId, R.RoleName, R.RoleValue, R.DisplayName, U.UserName, U.Id as UserId
     from UserRole UR
     JOIN Role R ON (R.IsDeleted Is NUll Or R.IsDeleted = 0) And Ur.RoleId = R.Id
     JOIN [dbo].[User] as U On (U.IsDeleted Is NUll Or U.IsDeleted = 0) and Ur.UserId = U.Id
     Where (UR.IsDeleted Is NUll Or UR.IsDeleted = 0) And ((@UserId Is Null Or @UserId='0') OR UR.UserId = @UserId OR U.OID = @UserId)
     ORDER BY R.RoleName ASC

    Select P.Id As PermissionId, PG.Id As PermissionGrantedId, P.PermissionValue,
     P.PermissionDisplayName, U.UserName, U.Id as UserId,
     P.PermissionSetId, ar.DisplayName as PermissionSetName,
     p.ModuleId, a.DisplayName as ModuleName, P.ApiName
     from PermissionGranted PG
     JOIN Permission P On P.Id = PG.PermissionId And (P.IsDeleted Is NUll Or P.IsDeleted = 0)
     JOIN [dbo].[User] as U On (U.IsDeleted Is NUll Or U.IsDeleted = 0) and PG.UserId = U.Id
     INNER JOIN AuthReferenceLookup as a ON a.Id = P.ModuleId
     INNER JOIN AuthReferenceLookup as ar ON ar.Id = P.PermissionSetId
     Where (PG.IsDeleted Is NUll Or PG.IsDeleted = 0) And ((@UserId Is Null Or @UserId='0') OR PG.UserId = @UserId OR U.OID = @UserId)
     ORDER BY P.PermissionDisplayName ASC

    Select P.Id As PermissionId, PD.Id As PermissionDeniedId, P.PermissionValue, P.PermissionDisplayName,
     U.UserName, U.Id as UserId,
     P.PermissionSetId, ar.DisplayName as PermissionSetName,
     p.ModuleId, a.DisplayName as ModuleName, P.ApiName
     from PermissionDenied PD
     JOIN Permission P On P.Id = PD.PermissionId And (P.IsDeleted Is NUll Or P.IsDeleted = 0)
     JOIN [dbo].[User] as U On (U.IsDeleted Is NUll Or U.IsDeleted = 0) and PD.UserId = U.Id
     INNER JOIN AuthReferenceLookup as a ON a.Id = P.ModuleId
     INNER JOIN AuthReferenceLookup as ar ON ar.Id = P.PermissionSetId
     Where (PD.IsDeleted Is NUll Or PD.IsDeleted = 0) And ((@UserId Is Null Or @UserId='0') OR PD.UserId = @UserId OR U.OID = @UserId)
     ORDER BY P.PermissionDisplayName ASC

    Declare @RoleIdList nvarchar(max)
    Select @RoleIdList = STRING_AGG(CAST(RoleId as NVARCHAR(max)), ',')
    From UserRole
    Where (UserId = @UserId OR (@UserId Is Null Or @UserId='0'))
    If @RoleIdList Is Null
        Set @RoleIdList = ''

    Select distinct a.UserId, a.UIPermissionId as PermissionId,
     a.PermissionValue, a.PermissionDisplayName,
     a.PermissionTypeId,
     a.PermissionTypeName, a.PermissionParentId, a.PermissionParentName,
     a.ModuleId,
     a.ModuleName,
     MAX(CASE WHEN a.CanCreate = 1 THEN 1 ELSE 0 END) AS CanCreate,
     MAX(CASE WHEN a.CanEdit = 1 THEN 1 ELSE 0 END) AS CanEdit,
     MAX(CASE WHEN a.CanDelete = 1 THEN 1 ELSE 0 END) AS CanDelete,
     MAX(CASE WHEN a.CanView = 1 THEN 1 ELSE 0 END) AS CanView,
     MAX(CASE WHEN a.IsUiPermissionDenied = 1 THEN 1 ELSE 0 END) AS IsUiPermissionDenied,
     MAX(CASE WHEN a.IsUiPermissionGranted = 1 THEN 1 ELSE 0 END) AS IsUiPermissionGranted FROM
    (
       Select pd.UIPermissionId As UIPermissionId, pd.UserId As UserId,
       P.PermissionValue, P.PermissionDisplayName,
       P.UiPermissionTypeId as PermissionTypeId, a.DisplayName as PermissionTypeName, P.PermissionParentId,
       UP.PermissionDisplayName as PermissionParentName, P.ModuleId, ar.DisplayName as ModuleName,
       pd.CanCreate, pd.CanEdit, pd.CanDelete, pd.CanView, 1 as IsUiPermissionDenied, 0 as IsUiPermissionGranted
       from [dbo].[UIPermissionDenied] as pd
       INNER JOIN UIPermission P On pd.UIPermissionId = P.Id and P.IsActive = 1
       INNER JOIN AuthReferenceLookup a On a.Id = P.UiPermissionTypeId
       INNER JOIN AuthReferenceLookup aR On aR.Id = P.ModuleId
       LEFT JOIN UIPermission UP On UP.Id = P.PermissionParentId
       where (pd.UserId = @UserId OR (@UserId Is Null Or @UserId='0'))

       Union All

       Select pgr.UIPermissionId As UIPermissionId, pgr.UserId As UserId,
       P.PermissionValue, P.PermissionDisplayName,
       P.UiPermissionTypeId as PermissionTypeId, a.DisplayName as PermissionTypeName,
       P.ModuleId, ar.DisplayName as ModuleName, P.PermissionParentId, up.PermissionDisplayName as PermissionParentName,
       pgr.CanCreate, pgr.CanEdit, pgr.CanDelete, pgr.CanView, 0 as IsUiPermissionDenied, 1 as IsUiPermissionGranted
       from [dbo].[UIPermissionGranted] as pgr
       INNER JOIN UIPermission P On pgr.UIPermissionId = P.Id and P.IsActive = 1
       INNER JOIN AuthReferenceLookup a On a.Id = P.UiPermissionTypeId
       INNER JOIN AuthReferenceLookup ar On ar.Id = P.ModuleId
       Left JOIN UiPermission up On up.Id = P.PermissionParentId
       where (pgr.UserId = @UserId OR (@UserId Is Null Or @UserId='0'))

       Union All

       Select RP.UIPermissionId As UIPermissionId, u.UserId,
       P.PermissionValue, P.PermissionDisplayName,
       P.UiPermissionTypeId as PermissionTypeId, a.DisplayName as PermissionTypeName, P.PermissionParentId, UP.PermissionDisplayName as PermissionParentName,
       P.ModuleId, ar.DisplayName as ModuleName,
       MAX(CASE WHEN RP.CanCreate = 1 THEN 1 ELSE 0 END) AS CanCreate,
       MAX(CASE WHEN RP.CanEdit = 1 THEN 1 ELSE 0 END) AS CanEdit,
       MAX(CASE WHEN RP.CanDelete = 1 THEN 1 ELSE 0 END) AS CanDelete,
       MAX(CASE WHEN RP.CanView = 1 THEN 1 ELSE 0 END) AS CanView,
       0 as IsUiPermissionDenied, 0 as IsUiPermissionGranted
       from [dbo].[RoleUIPermission] as RP
       INNER JOIN UIPermission P On RP.UIPermissionId = P.Id and P.IsActive = 1
       INNER JOIN AuthReferenceLookup a On a.Id = P.UiPermissionTypeId
       INNER JOIN AuthReferenceLookup ar On ar.Id = P.ModuleId
       LEFT JOIN UIPermission UP On UP.Id = P.PermissionParentId
       INNER JOIN [dbo].[UserRole] as u On u.RoleId = RP.RoleId
       where RP.RoleId in (SELECT * FROM STRING_SPLIT(@RoleIdList, ','))
       GROUP BY
           u.UserId, RP.UIPermissionId, P.PermissionValue, P.PermissionDisplayName,
           P.UiPermissionTypeId, a.DisplayName, P.PermissionParentId, UP.PermissionDisplayName,
           P.ModuleId, ar.DisplayName
    ) as a
    Group by
       a.UserId, a.UIPermissionId, a.PermissionValue, a.PermissionDisplayName,
       a.PermissionTypeId, a.PermissionTypeName, a.PermissionParentId, a.PermissionParentName,
       a.ModuleId, a.ModuleName;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Permission_Save]
    @PermissionValue varchar(200),
    @PermissionDisplayName varchar(200),
    @PermissionSetId varchar(100),
    @ModuleId varchar(100),
    @IsAuthorized bit,
    @OwnerId varchar(100),
    @SysData nvarchar(max),
    @TenantId varchar(100),
    @SubTenantId varchar(100),
    @CreatedBy varchar(max)
AS
BEGIN
    INSERT INTO Permission(PermissionValue,PermissionDisplayName,PermissionSetId,ModuleId,
    CreatedBy,IsAuthorized,OwnerId,SysData,TenantId,SubTenantId,IsDeleted,IsActive,CreatedDateTime)
    Values (@PermissionValue,@PermissionDisplayName,@PermissionSetId,@ModuleId,
    @CreatedBy,@IsAuthorized,@OwnerId,@SysData,@TenantId,@SubTenantId,0,1,GETDATE())
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Permission_Select]
AS
BEGIN
    SELECT p.Id, p.PermissionValue, p.PermissionDisplayName,
     p.PermissionSetId, ar.DisplayName as PermissionSetName,
     p.ModuleId, a.DisplayName as ModuleName, p.ApiName
     FROM Permission AS p
     INNER JOIN AuthReferenceLookup a On a.Id = p.ModuleId
     INNER JOIN AuthReferenceLookup ar On ar.Id = p.PermissionSetId
     ORDER BY PermissionDisplayName desc
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Permission_Update]
    @PermissionId varchar(100),
    @PermissionValue varchar(200),
    @PermissionDisplayName varchar(200),
    @PermissionSetId varchar(100),
    @ModuleId varchar(100),
    @UpdatedDateTime DateTime,
    @UpdatedBy varchar(100)
AS
BEGIN
    Update Permission
    Set PermissionValue = @PermissionValue,
     PermissionDisplayName = @PermissionDisplayName,
     PermissionSetId = @PermissionSetId,
     ModuleId = @ModuleId,
     UpdatedDateTime = GETUTCDATE(),
     UpdatedBy = @UpdatedBy
    where Id = @PermissionId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Role_Permission_Save]
    @RoleId varchar(100),
    @PermissionIds varchar(max),
    @IsAuthorized bit,
    @CreatedBy varchar(100),
    @OwnerId varchar(100),
    @TenantId varchar(100),
    @SubTenantId varchar(100),
    @SysData nvarchar(max)
AS
BEGIN
    DELETE FROM RolePermission WHERE RoleId = @RoleId;
    if(@PermissionIds != '')
    Begin
        INSERT INTO RolePermission (RoleId,CreatedDateTime,CreatedBy,UpdatedDateTime,UpdatedBy,IsDeleted,IsActive,IsAuthorized,OwnerId,SysData,TenantId,SubTenantId,PermissionId)
        select @RoleId,GETUTCDATE(),@CreatedBy,GETUTCDATE(),@CreatedBy,0,1,@IsAuthorized,@OwnerId,@SysData,@TenantId,@SubTenantId,a.* from(SELECT * FROM STRING_SPLIT(@PermissionIds, ',')) a
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Role_UIPermission_Save]
    @RoleId varchar(100),
    @UiPermissionId varchar(100),
    @UiPermissionCanCreate bit,
    @UiPermissionCanEdit bit,
    @UiPermissionCanDelete bit,
    @UiPermissionCanView bit,
    @CreatedBy varchar(100)
AS
BEGIN
    INSERT INTO RoleUIPermission(RoleId,UiPermissionId,CanCreate,CanEdit,CanDelete,CanView,CreatedBy,CreatedDateTime)
    Values (@RoleId,@UiPermissionId,@UiPermissionCanCreate,@UiPermissionCanEdit,@UiPermissionCanDelete,@UiPermissionCanView,@CreatedBy,GetUtcDate())
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_RoleInfo_Select]
    @RoleId varchar(100)
AS
BEGIN
    Select r.Id, r.RoleName, r.RoleValue
     from Role as r
     where (r.IsDeleted Is NUll Or r.IsDeleted = 0) and ((@RoleId Is Null Or @RoleId='0') OR r.Id = @RoleId)

    Select rp.PermissionId, P.PermissionDisplayName, P.PermissionValue, P.PermissionSetId,
     ar.DisplayName as PermissionSetName, rp.RoleId, p.ModuleId, a.DisplayName as ModuleName
     from RolePermission as rp
     JOIN Permission P On P.Id = rp.PermissionId And (P.IsDeleted Is NUll Or P.IsDeleted = 0)
     Inner Join AuthReferenceLookup as a on a.Id = p.ModuleId
     Inner Join AuthReferenceLookup as ar on ar.Id = p.PermissionSetId
     Where (rp.IsDeleted Is NUll Or rp.IsDeleted = 0) And ((@RoleId Is Null Or @RoleId='0') OR rp.RoleId = @RoleId)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_RolePermissionInfo_Select]
    @RoleId varchar(100)
AS
BEGIN
    Select RP.PermissionId As PermissionId, RP.Id As RolePermissionId, R.Id As RoleId,
     P.PermissionValue, P.PermissionDisplayName,
     P.PermissionSetId, ar.DisplayName as PermissionSetName,
     P.ModuleId, a.DisplayName as ModuleName, P.ApiName,
     R.RoleName, R.RoleValue
     from [dbo].[RolePermission] as RP
     INNER JOIN Permission P On RP.PermissionId = P.Id
     INNER JOIN [dbo].[Role] as R On RP.RoleId = R.Id
     INNER JOIN AuthReferenceLookup a On a.Id = P.ModuleId
     INNER JOIN AuthReferenceLookup ar On ar.Id = P.PermissionSetId
     Where (RP.RoleId = @RoleId)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_RoleUiPermissionsInfo_Select]
AS
BEGIN
    Select RP.UIPermissionId As UIPermissionId, R.Id As RoleId, R.RoleName, R.RoleValue,
     P.PermissionValue, P.PermissionDisplayName,
     P.UiPermissionTypeId as PermissionTypeId, a.DisplayName as PermissionTypeName, P.PermissionParentId, UP.PermissionDisplayName as PermissionParentName,
     P.ModuleId, ar.DisplayName as ModuleName,
     RP.CanCreate, RP.CanEdit, RP.CanDelete, RP.CanView
     from [dbo].[RoleUIPermission] as RP
     INNER JOIN UIPermission P On RP.UIPermissionId = P.Id and P.IsActive = 1
     INNER JOIN AuthReferenceLookup a On a.Id = P.UiPermissionTypeId
     INNER JOIN AuthReferenceLookup ar On ar.Id = P.ModuleId
     LEFT JOIN UIPermission UP On UP.Id = P.PermissionParentId
     INNER JOIN [dbo].[Role] as R On RP.RoleId = R.Id
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_UIPermission_Activate]
    @PermissionId varchar(100),
    @PermissionDisplayName varchar(200),
    @IsActive bit,
    @UpdatedBy varchar(100),
    @UpdatedDateTime varchar(200)
AS
BEGIN
    Update UIPermission
    SET IsActive = @IsActive,
     PermissionDisplayName = @PermissionDisplayName,
     UpdatedDateTime = @UpdatedDateTime,
     UpdatedBy = @UpdatedBy
    where Id = @PermissionId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_UIPermission_Save]
    @PermissionValue varchar(200),
    @PermissionDisplayName varchar(200),
    @PermissionTypeId varchar(200),
    @PermissionParentId varchar(100),
    @ModuleId varchar(100),
    @IsAuthorized bit,
    @OwnerId varchar(100),
    @SysData nvarchar(max),
    @TenantId varchar(100),
    @SubTenantId varchar(100),
    @CreatedBy varchar(100)
AS
BEGIN
    INSERT INTO UIPermission(PermissionValue,PermissionDisplayName,UiPermissionTypeId,PermissionParentId,ModuleId,CreatedBy,IsAuthorized,OwnerId,SysData,TenantId,SubTenantId,IsDeleted,IsActive,CreatedDateTime)
    Output Inserted.Id
    Values (@PermissionValue,@PermissionDisplayName,@PermissionTypeId,@PermissionParentId,@ModuleId,@CreatedBy,@IsAuthorized,@OwnerId,@SysData,@TenantId,@SubTenantId,0,0,GETDATE())
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_UIPermission_Select]
AS
BEGIN
    SELECT p.Id, p.PermissionValue, p.PermissionDisplayName,
     P.UiPermissionTypeId as PermissionTypeId, a.DisplayName as PermissionTypeName,
     p.ModuleId, ar.DisplayName as ModuleName,
     P.PermissionParentId, ui.PermissionDisplayName as PermissionParentName,
     p.IsActive
     FROM UIPermission AS p
     Inner Join AuthReferenceLookup as a on a.Id = p.UiPermissionTypeId
     Inner Join AuthReferenceLookup as ar on ar.Id = p.ModuleId
     Left Join UiPermission as ui on ui.Id = p.PermissionParentId
     ORDER BY PermissionDisplayName desc
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_User_AddRole]
    @UserId varchar(100),
    @RoleId varchar(100),
    @IsAuthorized bit,
    @CreatedBy varchar(100),
    @OwnerId varchar(100),
    @TenantId varchar(100),
    @SubTenantId varchar(100),
    @SysData nvarchar(max)
AS
BEGIN
    IF NOT Exists(Select * FRom UserRole WHERE UserId = @UserId AND RoleId = @RoleId)
    BEGIN
        INSERT INTO UserRole (UserId,CreatedDateTime,CreatedBy,UpdatedDateTime,UpdatedBy,IsDeleted,IsActive,IsAuthorized,OwnerId,SysData,TenantId,SubTenantId,RoleId)
        SELECT DISTINCT @UserId,GETUTCDATE(),@CreatedBy,GETUTCDATE(),@CreatedBy,0,1,@IsAuthorized,@OwnerId,@SysData,@TenantId,@SubTenantId,@RoleId
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_User_AddRoles]
    @UserId varchar(100),
    @RoleIds varchar(max),
    @IsAuthorized bit,
    @CreatedBy varchar(100),
    @OwnerId varchar(100),
    @TenantId varchar(100),
    @SubTenantId varchar(100),
    @SysData nvarchar(max)
AS
BEGIN
    Declare @Id1 varchar(100), @Id2 varchar(100), @Id3 varchar(100), @Id4 varchar(100);
    SET @Id1 = (SELECT Id from dbo.[User] where UserName='Chris.Green@stai09.onmicrosoft.com');
    SET @Id2 = (SELECT Id from dbo.[User] where UserName='JohnSmith@stai09.onmicrosoft.com');
    SET @Id3 = (SELECT Id from dbo.[User] where UserName='Dave@stai09.onmicrosoft.com');
    SET @Id4 = (SELECT Id from dbo.[User] where UserName='Scott@stai09.onmicrosoft.com');

    DELETE FROM UserRole WHERE UserId = @UserId;
    BEGIN
        INSERT INTO UserRole (UserId,CreatedDateTime,CreatedBy,UpdatedDateTime,UpdatedBy,IsDeleted,IsActive,IsAuthorized,OwnerId,SysData,TenantId,SubTenantId,RoleId)
        SELECT DISTINCT @UserId,GETUTCDATE(),@CreatedBy,GETUTCDATE(),@CreatedBy,0,1,@IsAuthorized,@OwnerId,@SysData,@TenantId,@SubTenantId,a.*
        from(SELECT * FROM STRING_SPLIT(@RoleIds, ',') Union all SELECT Id from Role where RoleValue='User') a
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_User_DeleteRole]
    @UserId varchar(100),
    @RoleId varchar(100),
    @IsAuthorized bit,
    @CreatedBy varchar(100),
    @OwnerId varchar(100),
    @TenantId varchar(100),
    @SubTenantId varchar(100),
    @SysData nvarchar(max)
AS
BEGIN
    Delete From UserRole where UserId = @UserId And RoleId = @RoleId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_User_PermissionDenied_Save]
    @UserId varchar(100),
    @PermissionIds varchar(max),
    @IsAuthorized bit,
    @CreatedBy varchar(100),
    @OwnerId varchar(100),
    @TenantId varchar(100),
    @SubTenantId varchar(100),
    @SysData nvarchar(max)
AS
BEGIN
    DELETE FROM PermissionDenied WHERE UserId = @UserId;
    Declare @Id1 varchar(100), @Id2 varchar(100), @Id3 varchar(100), @Id4 varchar(100);
    SET @Id1 = (SELECT Id from dbo.[User] where UserName='Chris.Green@stai09.onmicrosoft.com');
    SET @Id2 = (SELECT Id from dbo.[User] where UserName='JohnSmith@stai09.onmicrosoft.com');
    SET @Id3 = (SELECT Id from dbo.[User] where UserName='Dave@stai09.onmicrosoft.com');
    SET @Id4 = (SELECT Id from dbo.[User] where UserName='Scott@stai09.onmicrosoft.com');
    if(NOT((@UserId=@Id1) OR(@UserId=@Id2) or (@UserId=@Id3) or (@UserId=@Id4)))
    BEGIN
        INSERT INTO PermissionDenied (UserId,CreatedDateTime,CreatedBy,UpdatedDateTime,UpdatedBy,IsDeleted,IsActive,IsAuthorized,OwnerId,SysData,TenantId,SubTenantId,PermissionId)
        select DISTINCT @UserId,GETUTCDATE(),@CreatedBy,GETUTCDATE(),@CreatedBy,0,1,@IsAuthorized,@OwnerId,@SysData,@TenantId,@SubTenantId,a.* from(SELECT * FROM STRING_SPLIT(@PermissionIds, ',')) a
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_User_PermissionGranted_Save]
    @UserId varchar(100),
    @PermissionIds varchar(max),
    @IsAuthorized bit,
    @CreatedBy varchar(100),
    @OwnerId varchar(100),
    @TenantId varchar(100),
    @SubTenantId varchar(100),
    @SysData nvarchar(max)
AS
BEGIN
    DELETE FROM PermissionGranted WHERE UserId = @UserId;
    Declare @Id1 varchar(100), @Id2 varchar(100), @Id3 varchar(100), @Id4 varchar(100);
    SET @Id1 = (SELECT Id from dbo.[User] where UserName='Chris.Green@stai09.onmicrosoft.com');
    SET @Id2 = (SELECT Id from dbo.[User] where UserName='JohnSmith@stai09.onmicrosoft.com');
    SET @Id3 = (SELECT Id from dbo.[User] where UserName='Dave@stai09.onmicrosoft.com');
    SET @Id4 = (SELECT Id from dbo.[User] where UserName='Scott@stai09.onmicrosoft.com');
    if(NOT((@UserId=@Id1) OR(@UserId=@Id2) or (@UserId=@Id3) or (@UserId=@Id4)))
    BEGIN
        INSERT INTO PermissionGranted (UserId,CreatedDateTime,CreatedBy,UpdatedDateTime,UpdatedBy,IsDeleted,IsActive,IsAuthorized,OwnerId,SysData,TenantId,SubTenantId,PermissionId)
        select DISTINCT @UserId,GETUTCDATE(),@CreatedBy,GETUTCDATE(),@CreatedBy,0,1,@IsAuthorized,@OwnerId,@SysData,@TenantId,@SubTenantId,a.* from (SELECT * FROM STRING_SPLIT(@PermissionIds, ',')) a
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_User_Save]
(
    @Id varchar(100) OUTPUT,
    @UserName varchar(200),
    @PasswordHash varchar(200),
    @auth_type varchar(200),
    @EmpId varchar(6),
    @EmpType varchar(200),
    @mobile varchar(20),
    @Email varchar(200),
    @Position varchar(200),
    @BusinessUnit varchar(200),
    @oid varchar(100),
    @given_name varchar(200),
    @family_name varchar(200),
    @preferred_username varchar(200),
    @FirstName varchar(200),
    @LastName varchar(200),
    @SecondaryEmail varchar(200),
    @PhoneNumber varchar(20),
    @Extension varchar(20),
    @display_name varchar(200),
    @ManagerId varchar(100),
    @Designation varchar(200),
    @Department varchar(200),
    @Location varchar(200),
    @Organization varchar(200),
    @AccessLevel varchar(50),
    @CreatedBy varchar(100),
    @IsAuthorized bit,
    @OwnerId varchar(100),
    @TenantId varchar(100),
    @SubTenantId varchar(100),
    @SysData nvarchar(max)
)
AS
BEGIN
    Declare @newId uniqueIdentifier = NEWID();
    SET @Id = (Select Cast(@newId as varchar(100)));

    INSERT INTO [dbo].[User] (Id,UserName,auth_type,EmpId,EmpType,mobile,Email,Position,BusinessUnit,oid,given_name,family_name,preferred_username,
     FirstName,LastName,SecondaryEmail,PhoneNumber,Extension,display_name,ManagerId,Designation,Department,[Location],Organization,
     CreatedDateTime,UpdatedDateTime,IsDeleted,IsActive,IsAuthorized,OwnerId,CreatedBy,UpdatedBy,SysData,TenantId,SubTenantId)
    VALUES(@Id,@UserName,@auth_type,@EmpId,@EmpType,@mobile,@Email,@Position,@BusinessUnit,@oid,@given_name,@family_name,@preferred_username,
     @FirstName,@LastName,@SecondaryEmail,@PhoneNumber,@Extension,@display_name,@ManagerId,@Designation,@Department,@Location,@Organization,
     GETDATE(),GETDATE(),0,1,1,@OwnerId,@CreatedBy,@CreatedBy,@SysData,@TenantId,@SubTenantId);

    If (@AccessLevel <> '' AND @AccessLevel is not null)
    BEGIN
        Update [dbo].[User] Set AccessLevel = @AccessLevel where UserName = @UserName
    END

    Insert Into UserRole(UserId,RoleId)
    select @Id, Id from Role where RoleName='User'

    Insert into UserPasswordHash(UserId,PasswordHash,UpdatedBy,UpdatedDateTime)
    values (@Id,@PasswordHash,@CreatedBy,GetUtcDate())

    SELECT @Id
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_User_UpdateAccessLevel]
    @UserId varchar(100),
    @AccessLevel varchar(50),
    @UpdatedBy varchar(100)
AS
BEGIN
    UPDATE [User] SET AccessLevel = @AccessLevel, UpdatedBy = @UpdatedBy WHERE Id = @UserId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_User_UpdateUserDetail]
    @Id varchar(100),
    @PhoneNumber varchar(20),
    @Extension varchar(20),
    @Email varchar(200),
    @UserRoleId varchar(100),
    @AccessLevel varchar(50),
    @UpdatedBy varchar(100),
    @UpdatedDateTime datetime
AS
BEGIN
    UPDATE [User] SET PhoneNumber=@PhoneNumber, Extension=@Extension, Email=@Email,
     AccessLevel=@AccessLevel, UpdatedBy=@UpdatedBy, UpdatedDateTime=@UpdatedDateTime WHERE Id=@Id;
    Update [UserRole] SET RoleId=@UserRoleId, UpdatedBy=@UpdatedBy, UpdatedDateTime=@UpdatedDateTime Where UserId=@Id;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_UserAccessLevel_Select]
AS
BEGIN
    Select AccessLevel, DisplayName FROM UserAccessLevel
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_UserInfo_Select]
    @UserId varchar(100)
AS
BEGIN
    Declare @UserId1 varchar(100) = (Select Id from [User] where (Id=@UserId OR oid=@UserId OR UserName=@UserId))

    Select U.Id as userId, U.EmpId, U.EmpType, U.UserName, U.auth_type, U.FirstName, U.LastName, U.Email, U.SecondaryEmail, U.PhoneNumber, U.Extension,
     U.mobile, U.oid, U.preferred_username, U.display_name, U.given_name, U.family_name, U.Position, U.BusinessUnit,
     u.ManagerId, M.UserName As ManagerUserName, U.AccessLevel, U.IsActive, U.Designation, U.Department, U.Location, U.Organization,
     U.CorrelationId, U.AuditableRequestId, U.AuditableRequestName, U.AuditableSourceEventName, U.CreatedBy, U.CreatedDateTime, U.UpdatedBy,
     U.UpdatedDateTime, U.UpdateReason, U.OwnerId, U.IsDeleted, U.IsApproved, U.ApproverId, U.ApprovedDateTime,
     U.IsAuthorized, U.AuthorizedById, U.AuthorizedDateTime, U.SysData, U.TenantId, U.SubTenantId
     from [dbo].[User] as U
     Left JOIN [dbo].[User] as M On U.ManagerId=M.Id And (M.IsDeleted Is NUll Or M.IsDeleted=0)
     where (U.IsDeleted Is NUll Or U.IsDeleted=0)
     and ((@UserId Is Null Or @UserId='0') OR (U.Id=@UserId1))
     ORDER BY U.display_name ASC

    Select UR.Id As UserRoleId, R.Id As RoleId, R.RoleName, R.RoleValue, R.DisplayName, U.UserName, U.Id as UserId
     from UserRole UR
     JOIN Role R ON (R.IsDeleted Is NUll Or R.IsDeleted=0) And Ur.RoleId=R.Id
     JOIN [dbo].[User] as U On (U.IsDeleted Is NUll Or U.IsDeleted=0) and Ur.UserId=U.Id
     Where (UR.IsDeleted Is NUll Or UR.IsDeleted=0) And
     ((@UserId Is Null Or @UserId='0') OR (UR.UserId=@UserId1))
     ORDER BY R.RoleName ASC

    Select P.Id As PermissionId, PG.Id As PermissionGrantedId, P.PermissionValue,
     P.PermissionDisplayName, U.UserName, U.Id as UserId,
     P.PermissionSetId, ar.DisplayName as PermissionSetName, p.ModuleId, a.DisplayName as ModuleName
     from PermissionGranted PG
     JOIN Permission P On P.Id=PG.PermissionId And (P.IsDeleted Is NUll Or P.IsDeleted=0)
     JOIN [dbo].[User] as U On (U.IsDeleted Is NUll Or U.IsDeleted=0) and PG.UserId=U.Id
     INNER JOIN AuthReferenceLookup as a ON a.Id=P.ModuleId
     INNER JOIN AuthReferenceLookup as ar ON ar.Id=P.PermissionSetId
     Where (PG.IsDeleted Is NUll Or PG.IsDeleted=0) And
     ((@UserId Is Null Or @UserId='0') OR PG.UserId=@UserId1)
     ORDER BY P.PermissionDisplayName ASC

    Select P.Id As PermissionId, PD.Id As PermissionDeniedId, P.PermissionValue, P.PermissionDisplayName,
     U.UserName, U.Id as UserId,
     P.PermissionSetId, ar.DisplayName as PermissionSetName, p.ModuleId, a.DisplayName as ModuleName
     from PermissionDenied PD
     JOIN Permission P On P.Id=PD.PermissionId And (P.IsDeleted Is NUll Or P.IsDeleted=0)
     JOIN [dbo].[User] as U On (U.IsDeleted Is NUll Or U.IsDeleted=0) and PD.UserId=U.Id
     INNER JOIN AuthReferenceLookup as a ON a.Id=P.ModuleId
     INNER JOIN AuthReferenceLookup as ar ON ar.Id=P.PermissionSetId
     Where (PD.IsDeleted Is NUll Or PD.IsDeleted=0) And
     ((@UserId Is Null Or @UserId='0') OR PD.UserId=@UserId1)
     ORDER BY P.PermissionDisplayName ASC

    Declare @RoleIdList nvarchar(max)
    Select @RoleIdList = STRING_AGG(CAST(RoleId as NVARCHAR(max)), ',')
    From UserRole
    Where (UserId=@UserId1 OR (@UserId Is Null Or @UserId='0'))
    If @RoleIdList Is Null Set @RoleIdList = ''

    Select distinct a.UserId, a.UIPermissionId as PermissionId,
     a.PermissionValue, a.PermissionDisplayName,
     a.PermissionTypeId, a.PermissionTypeName, a.PermissionParentId, a.PermissionParentName,
     a.ModuleId, a.ModuleName,
     MAX(CASE WHEN a.CanCreate=1 THEN 1 ELSE 0 END) AS CanCreate,
     MAX(CASE WHEN a.CanEdit=1 THEN 1 ELSE 0 END) AS CanEdit,
     MAX(CASE WHEN a.CanDelete=1 THEN 1 ELSE 0 END) AS CanDelete,
     MAX(CASE WHEN a.CanView=1 THEN 1 ELSE 0 END) AS CanView,
     MAX(CASE WHEN a.IsUiPermissionDenied=1 THEN 1 ELSE 0 END) AS IsUiPermissionDenied,
     MAX(CASE WHEN a.IsUiPermissionGranted=1 THEN 1 ELSE 0 END) AS IsUiPermissionGranted FROM
    (
       Select pd.UIPermissionId, pd.UserId,
       P.PermissionValue, P.PermissionDisplayName,
       P.UiPermissionTypeId as PermissionTypeId, a.DisplayName as PermissionTypeName, P.PermissionParentId,
       UP.PermissionDisplayName as PermissionParentName, P.ModuleId, ar.DisplayName as ModuleName,
       pd.CanCreate, pd.CanEdit, pd.CanDelete, pd.CanView, 1 as IsUiPermissionDenied, 0 as IsUiPermissionGranted
       from [dbo].[UIPermissionDenied] as pd
       INNER JOIN UIPermission P On pd.UIPermissionId=P.Id and P.IsActive=1
       INNER JOIN AuthReferenceLookup a On a.Id=P.UiPermissionTypeId
       INNER JOIN AuthReferenceLookup aR On aR.Id=P.ModuleId
       LEFT JOIN UIPermission UP On UP.Id=P.PermissionParentId
       where (pd.UserId=@UserId1 OR (@UserId Is Null Or @UserId='0'))

       Union All

       Select pgr.UIPermissionId, pgr.UserId,
       P.PermissionValue, P.PermissionDisplayName,
       P.UiPermissionTypeId as PermissionTypeId, a.DisplayName as PermissionTypeName,
       P.ModuleId, ar.DisplayName as ModuleName, P.PermissionParentId, up.PermissionDisplayName as PermissionParentName,
       pgr.CanCreate, pgr.CanEdit, pgr.CanDelete, pgr.CanView, 0 as IsUiPermissionDenied, 1 as IsUiPermissionGranted
       from [dbo].[UIPermissionGranted] as pgr
       INNER JOIN UIPermission P On pgr.UIPermissionId=P.Id and P.IsActive=1
       INNER JOIN AuthReferenceLookup a On a.Id=P.UiPermissionTypeId
       INNER JOIN AuthReferenceLookup ar On ar.Id=P.ModuleId
       Left JOIN UiPermission up On up.Id=P.PermissionParentId
       where (pgr.UserId=@UserId1 OR (@UserId Is Null Or @UserId='0'))

       Union All

       Select RP.UIPermissionId, u.UserId,
       P.PermissionValue, P.PermissionDisplayName,
       P.UiPermissionTypeId as PermissionTypeId, a.DisplayName as PermissionTypeName, P.PermissionParentId, UP.PermissionDisplayName as PermissionParentName,
       P.ModuleId, ar.DisplayName as ModuleName,
       MAX(CASE WHEN RP.CanCreate=1 THEN 1 ELSE 0 END) AS CanCreate,
       MAX(CASE WHEN RP.CanEdit=1 THEN 1 ELSE 0 END) AS CanEdit,
       MAX(CASE WHEN RP.CanDelete=1 THEN 1 ELSE 0 END) AS CanDelete,
       MAX(CASE WHEN RP.CanView=1 THEN 1 ELSE 0 END) AS CanView,
       0 as IsUiPermissionDenied, 0 as IsUiPermissionGranted
       from [dbo].[RoleUIPermission] as RP
       INNER JOIN UIPermission P On RP.UIPermissionId=P.Id and P.IsActive=1
       INNER JOIN AuthReferenceLookup a On a.Id=P.UiPermissionTypeId
       INNER JOIN AuthReferenceLookup ar On ar.Id=P.ModuleId
       LEFT JOIN UIPermission UP On UP.Id=P.PermissionParentId
       INNER JOIN [dbo].[UserRole] as u On u.RoleId=RP.RoleId
       where RP.RoleId in (SELECT * FROM STRING_SPLIT(@RoleIdList, ','))
       GROUP BY u.UserId, RP.UIPermissionId, P.PermissionValue, P.PermissionDisplayName,
           P.UiPermissionTypeId, a.DisplayName, P.PermissionParentId, UP.PermissionDisplayName,
           P.ModuleId, ar.DisplayName
    ) as a
    Group by a.UserId, a.UIPermissionId, a.PermissionValue, a.PermissionDisplayName,
       a.PermissionTypeId, a.PermissionTypeName, a.PermissionParentId, a.PermissionParentName,
       a.ModuleId, a.ModuleName;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_UserInfo_Select_gunjan]
    @UserId varchar(100)
AS
BEGIN
    SELECT U.Id AS userId, U.EmpId, U.EmpType, U.UserName, U.FirstName, U.LastName,
     U.Email, U.SecondaryEmail, U.PhoneNumber, U.Extension, U.mobile, U.oid,
     U.preferred_username, U.display_name, U.given_name, U.family_name,
     U.Position, U.BusinessUnit, U.ManagerId, M.UserName AS ManagerUserName,
     U.AccessLevel, U.IsActive, U.Designation, U.Department, U.Location,
     U.Organization, U.CorrelationId, U.AuditableRequestId, U.AuditableRequestName,
     U.AuditableSourceEventName, U.CreatedBy, U.CreatedDateTime, U.UpdatedBy,
     U.UpdatedDateTime, U.UpdateReason, U.OwnerId, U.IsDeleted, U.IsApproved,
     U.ApproverId, U.ApprovedDateTime, U.IsAuthorized, U.AuthorizedById,
     U.AuthorizedDateTime, U.SysData, U.TenantId, U.SubTenantId
     FROM [dbo].[User] AS U
     LEFT JOIN [dbo].[User] AS M ON U.ManagerId=M.Id AND (M.IsDeleted IS NULL OR M.IsDeleted=0)
     WHERE (U.IsDeleted IS NULL OR U.IsDeleted=0)
     AND ((@UserId IS NULL OR @UserId='0') OR U.Id=@UserId OR U.OID=@UserId)
     ORDER BY U.display_name ASC;

    SELECT UR.Id AS UserRoleId, R.Id AS RoleId, R.RoleName, R.RoleValue, R.DisplayName, U.UserName, U.Id AS UserId
     FROM UserRole UR
     JOIN Role R ON (R.IsDeleted IS NULL OR R.IsDeleted=0) AND UR.RoleId=R.Id
     JOIN [dbo].[User] AS U ON (U.IsDeleted IS NULL OR U.IsDeleted=0) AND UR.UserId=U.Id
     WHERE (UR.IsDeleted IS NULL OR UR.IsDeleted=0)
     AND ((@UserId IS NULL OR @UserId='0') OR UR.UserId=@UserId OR U.OID=@UserId)
     ORDER BY R.RoleName ASC;

    SELECT P.Id AS PermissionId, PG.Id AS PermissionGrantedId, P.PermissionValue,
     P.PermissionDisplayName, U.UserName, U.Id AS UserId, P.PermissionSetId,
     ar.DisplayName AS PermissionSetName, p.ModuleId, a.DisplayName AS ModuleName
     FROM PermissionGranted PG
     JOIN Permission P ON P.Id=PG.PermissionId AND (P.IsDeleted IS NULL OR P.IsDeleted=0)
     JOIN [dbo].[User] AS U ON (U.IsDeleted IS NULL OR U.IsDeleted=0) AND PG.UserId=U.Id
     INNER JOIN AuthReferenceLookup AS a ON a.Id=P.ModuleId
     INNER JOIN AuthReferenceLookup AS ar ON ar.Id=P.PermissionSetId
     WHERE (PG.IsDeleted IS NULL OR PG.IsDeleted=0)
     AND ((@UserId IS NULL OR @UserId='0') OR PG.UserId=@UserId OR U.OID=@UserId)
     ORDER BY P.PermissionDisplayName ASC;

    SELECT P.Id AS PermissionId, PD.Id AS PermissionDeniedId, P.PermissionValue,
     P.PermissionDisplayName, U.UserName, U.Id AS UserId, P.PermissionSetId,
     ar.DisplayName AS PermissionSetName, p.ModuleId, a.DisplayName AS ModuleName
     FROM PermissionDenied PD
     JOIN Permission P ON P.Id=PD.PermissionId AND (P.IsDeleted IS NULL OR P.IsDeleted=0)
     JOIN [dbo].[User] AS U ON (U.IsDeleted IS NULL OR U.IsDeleted=0) AND PD.UserId=U.Id
     INNER JOIN AuthReferenceLookup AS a ON a.Id=P.ModuleId
     INNER JOIN AuthReferenceLookup AS ar ON ar.Id=P.PermissionSetId
     WHERE (PD.IsDeleted IS NULL OR PD.IsDeleted=0)
     AND ((@UserId IS NULL OR @UserId='0') OR PD.UserId=@UserId OR U.OID=@UserId)
     ORDER BY P.PermissionDisplayName ASC;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_UserProfile_Get_ByUserId]
    @UserId varchar(100)
AS
BEGIN
    SELECT [UserId], DOB, Gender, BloodGroup, PersonalEmail, DateOfJoining, PassportNumber, FatherName,
     MotherName, MaritalStatus, WeddingAnniversaryDate, SpouseName, SpouseDOB, HomeAddress1, HomeAddress2,
     City, [State], HomeAddressCity, HomeAddressState, HomeAddressCountry, HomePhoneNumber,
     EmergencyContactNumber, EmergencyContactName, PrimarySkills, SecondarySkills, TertiarySkills, OtherSkills,
     [Branch],[LookUpCode],[LinkedInUrl], display_name
     FROM [User] as U
     inner join [DBO].[USERPROFILE] as Up on up.UserId=u.Id
     WHERE [UserId]=@UserId
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_UserRole_Select]
    @RoleId varchar(100)
AS
BEGIN
    Select r.Id, r.RoleName, r.RoleValue
     from Role as r
     where (r.IsDeleted Is NUll Or r.IsDeleted=0) and ((@RoleId Is Null Or @RoleId='0') OR r.Id=@RoleId)
END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Users_Select]
AS
BEGIN
    SELECT u.Id, u.UserName, r1.DisplayName as UserRoleName, u.AccessLevel,
     u.IsDeleted, u.oid, u.BusinessUnit, u.mobile, u.email, u.Position, u.given_name, u.family_name, u.preferred_username
     FROM [dbo].[User] u
     LEFT JOIN UserRole r on r.UserId=u.Id
     LEFT JOIN Role r1 on r.RoleId=u.Id
     AND (u.IsDeleted=0 OR u.IsDeleted IS NULL)
     ORDER BY u.UserName
END
GO
