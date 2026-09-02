
/****** Object:  Table [dbo].[AuthReferenceLookup]    Script Date: 21-01-2026 14:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuthReferenceLookup](
	[Id] [varchar](100) NOT NULL,
	[Name] [varchar](100) NULL,
	[DisplayName] [varchar](100) NULL,
	[Type] [varchar](100) NULL,
 CONSTRAINT [PK_AuthReferenceLookup_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Permission]    Script Date: 21-01-2026 14:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Permission](
	[Id] [varchar](100) NOT NULL,
	[PermissionValue] [varchar](200) NOT NULL,
	[PermissionDisplayName] [varchar](200) NOT NULL,
	[CreatedBy] [varchar](100) NULL,
	[CreatedDateTime] [datetime] NULL,
	[UpdatedBy] [varchar](100) NULL,
	[UpdatedDateTime] [datetime] NULL,
	[UpdateReason] [varchar](100) NULL,
	[IsDeleted] [bit] NULL,
	[IsActive] [bit] NULL,
	[OwnerId] [varchar](100) NULL,
	[IsApproved] [bit] NULL,
	[ApproverId] [varchar](100) NULL,
	[ApprovedDateTime] [datetime] NULL,
	[IsAuthorized] [bit] NULL,
	[AuthorizedById] [varchar](100) NULL,
	[AuthorizedDateTime] [datetime] NULL,
	[TenantId] [varchar](100) NULL,
	[SubTenantId] [varchar](100) NULL,
	[SysData] [nvarchar](max) NULL,
	[ModuleId] [varchar](100) NOT NULL,
	[PermissionSetId] [varchar](100) NOT NULL,
	[ApiName] [varchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PermissionDenied]    Script Date: 21-01-2026 14:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PermissionDenied](
	[Id] [varchar](100) NOT NULL,
	[UserId] [varchar](100) NOT NULL,
	[PermissionId] [varchar](100) NOT NULL,
	[CreatedBy] [varchar](100) NULL,
	[CreatedDateTime] [datetime] NULL,
	[UpdatedBy] [varchar](100) NULL,
	[UpdatedDateTime] [datetime] NULL,
	[UpdateReason] [varchar](100) NULL,
	[IsDeleted] [bit] NULL,
	[IsActive] [bit] NULL,
	[OwnerId] [varchar](100) NULL,
	[IsApproved] [bit] NULL,
	[ApproverId] [varchar](100) NULL,
	[ApprovedDateTime] [datetime] NULL,
	[IsAuthorized] [bit] NULL,
	[AuthorizedById] [varchar](100) NULL,
	[AuthorizedDateTime] [datetime] NULL,
	[TenantId] [varchar](100) NULL,
	[SubTenantId] [varchar](100) NULL,
	[SysData] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PermissionGranted]    Script Date: 21-01-2026 14:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PermissionGranted](
	[Id] [varchar](100) NOT NULL,
	[UserId] [varchar](100) NOT NULL,
	[PermissionId] [varchar](100) NOT NULL,
	[CreatedBy] [varchar](100) NULL,
	[CreatedDateTime] [datetime] NULL,
	[UpdatedBy] [varchar](100) NULL,
	[UpdatedDateTime] [datetime] NULL,
	[UpdateReason] [varchar](100) NULL,
	[IsDeleted] [bit] NULL,
	[IsActive] [bit] NULL,
	[OwnerId] [varchar](100) NULL,
	[IsApproved] [bit] NULL,
	[ApproverId] [varchar](100) NULL,
	[ApprovedDateTime] [datetime] NULL,
	[IsAuthorized] [bit] NULL,
	[AuthorizedById] [varchar](100) NULL,
	[AuthorizedDateTime] [datetime] NULL,
	[TenantId] [varchar](100) NULL,
	[SubTenantId] [varchar](100) NULL,
	[SysData] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 21-01-2026 14:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [varchar](100) NOT NULL,
	[RoleName] [varchar](200) NULL,
	[RoleValue] [varchar](200) NULL,
	[DisplayName] [varchar](200) NULL,
	[AzureRoleGuid] [varchar](100) NULL,
	[CreatedBy] [varchar](100) NULL,
	[CreatedDateTime] [datetime] NULL,
	[UpdatedBy] [varchar](100) NULL,
	[UpdatedDateTime] [datetime] NULL,
	[UpdateReason] [varchar](100) NULL,
	[IsDeleted] [bit] NULL,
	[IsActive] [bit] NULL,
	[OwnerId] [varchar](100) NULL,
	[IsApproved] [bit] NULL,
	[ApproverId] [varchar](100) NULL,
	[ApprovedDateTime] [datetime] NULL,
	[IsAuthorized] [bit] NULL,
	[AuthorizedById] [varchar](100) NULL,
	[AuthorizedDateTime] [datetime] NULL,
	[TenantId] [varchar](100) NULL,
	[SubTenantId] [varchar](100) NULL,
	[SysData] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RolePermission]    Script Date: 21-01-2026 14:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RolePermission](
	[Id] [varchar](100) NOT NULL,
	[RoleId] [varchar](100) NULL,
	[PermissionId] [varchar](100) NULL,
	[CreatedBy] [varchar](100) NULL,
	[CreatedDateTime] [datetime] NULL,
	[UpdatedBy] [varchar](100) NULL,
	[UpdatedDateTime] [datetime] NULL,
	[UpdateReason] [varchar](100) NULL,
	[IsDeleted] [bit] NULL,
	[IsActive] [bit] NULL,
	[OwnerId] [varchar](100) NULL,
	[IsApproved] [bit] NULL,
	[ApproverId] [varchar](100) NULL,
	[ApprovedDateTime] [datetime] NULL,
	[IsAuthorized] [bit] NULL,
	[AuthorizedById] [varchar](100) NULL,
	[AuthorizedDateTime] [datetime] NULL,
	[TenantId] [varchar](100) NULL,
	[SubTenantId] [varchar](100) NULL,
	[SysData] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RoleUiPermission]    Script Date: 21-01-2026 14:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleUiPermission](
	[RoleId] [varchar](100) NOT NULL,
	[UiPermissionId] [varchar](100) NOT NULL,
	[CanView] [bit] NULL,
	[CanEdit] [bit] NULL,
	[CanCreate] [bit] NULL,
	[CanDelete] [bit] NULL,
	[CreatedBy] [varchar](100) NULL,
	[CreatedDateTime] [datetime] NULL,
	[UpdatedBy] [varchar](100) NULL,
	[UpdatedDateTime] [datetime] NULL,
	[UpdateReason] [varchar](100) NULL,
	[IsDeleted] [bit] NULL,
	[IsActive] [bit] NULL,
	[OwnerId] [varchar](100) NULL,
	[IsApproved] [bit] NULL,
	[ApproverId] [varchar](100) NULL,
	[ApprovedDateTime] [datetime] NULL,
	[IsAuthorized] [bit] NULL,
	[AuthorizedById] [varchar](100) NULL,
	[AuthorizedDateTime] [datetime] NULL,
	[TenantId] [varchar](100) NULL,
	[SubTenantId] [varchar](100) NULL,
	[SysData] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UiPermission]    Script Date: 21-01-2026 14:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UiPermission](
	[Id] [varchar](100) NOT NULL,
	[PermissionValue] [varchar](200) NOT NULL,
	[PermissionDisplayName] [varchar](200) NOT NULL,
	[PermissionParentId] [varchar](100) NULL,
	[CreatedBy] [varchar](100) NULL,
	[CreatedDateTime] [datetime] NULL,
	[UpdatedBy] [varchar](100) NULL,
	[UpdatedDateTime] [datetime] NULL,
	[UpdateReason] [varchar](100) NULL,
	[IsDeleted] [bit] NULL,
	[IsActive] [bit] NULL,
	[OwnerId] [varchar](100) NULL,
	[IsApproved] [bit] NULL,
	[ApproverId] [varchar](100) NULL,
	[ApprovedDateTime] [datetime] NULL,
	[IsAuthorized] [bit] NULL,
	[AuthorizedById] [varchar](100) NULL,
	[AuthorizedDateTime] [datetime] NULL,
	[TenantId] [varchar](100) NULL,
	[SubTenantId] [varchar](100) NULL,
	[SysData] [nvarchar](max) NULL,
	[ModuleId] [varchar](100) NOT NULL,
	[UiPermissionTypeId] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UiPermissionDenied]    Script Date: 21-01-2026 14:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UiPermissionDenied](
	[UserId] [varchar](100) NULL,
	[UiPermissionId] [varchar](100) NULL,
	[CanView] [bit] NULL,
	[CanEdit] [bit] NULL,
	[CanCreate] [bit] NULL,
	[CanDelete] [bit] NULL,
	[CreatedBy] [varchar](100) NULL,
	[CreatedDateTime] [datetime] NULL,
	[UpdatedBy] [varchar](100) NULL,
	[UpdatedDateTime] [datetime] NULL,
	[UpdateReason] [varchar](100) NULL,
	[IsDeleted] [bit] NULL,
	[IsActive] [bit] NULL,
	[OwnerId] [varchar](100) NULL,
	[IsApproved] [bit] NULL,
	[ApproverId] [varchar](100) NULL,
	[ApprovedDateTime] [datetime] NULL,
	[IsAuthorized] [bit] NULL,
	[AuthorizedById] [varchar](100) NULL,
	[AuthorizedDateTime] [datetime] NULL,
	[TenantId] [varchar](100) NULL,
	[SubTenantId] [varchar](100) NULL,
	[SysData] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UiPermissionGranted]    Script Date: 21-01-2026 14:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UiPermissionGranted](
	[UserId] [varchar](100) NULL,
	[UiPermissionId] [varchar](100) NULL,
	[CanView] [bit] NULL,
	[CanEdit] [bit] NULL,
	[CanCreate] [bit] NULL,
	[CanDelete] [bit] NULL,
	[CreatedBy] [varchar](100) NULL,
	[CreatedDateTime] [datetime] NULL,
	[UpdatedBy] [varchar](100) NULL,
	[UpdatedDateTime] [datetime] NULL,
	[UpdateReason] [varchar](100) NULL,
	[IsDeleted] [bit] NULL,
	[IsActive] [bit] NULL,
	[OwnerId] [varchar](100) NULL,
	[IsApproved] [bit] NULL,
	[ApproverId] [varchar](100) NULL,
	[ApprovedDateTime] [datetime] NULL,
	[IsAuthorized] [bit] NULL,
	[AuthorizedById] [varchar](100) NULL,
	[AuthorizedDateTime] [datetime] NULL,
	[TenantId] [varchar](100) NULL,
	[SubTenantId] [varchar](100) NULL,
	[SysData] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 21-01-2026 14:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[Id] [varchar](100) NOT NULL,
	[UserName] [varchar](200) NOT NULL,
	[EmpId] [varchar](6) NULL,
	[EmpType] [varchar](200) NULL,
	[FirstName] [varchar](200) NOT NULL,
	[LastName] [varchar](200) NOT NULL,
	[Email] [varchar](200) NULL,
	[SecondaryEmail] [varchar](200) NULL,
	[PhoneNumber] [varchar](20) NULL,
	[Extension] [varchar](20) NULL,
	[mobile] [varchar](20) NULL,
	[oid] [varchar](100) NULL,
	[preferred_username] [varchar](200) NULL,
	[display_name] [varchar](200) NULL,
	[given_name] [varchar](200) NULL,
	[family_name] [varchar](200) NULL,
	[Position] [varchar](200) NULL,
	[BusinessUnit] [varchar](200) NULL,
	[ManagerId] [varchar](100) NULL,
	[Designation] [varchar](200) NULL,
	[Department] [varchar](200) NULL,
	[Location] [varchar](200) NULL,
	[Organization] [varchar](200) NULL,
	[CreatedBy] [varchar](100) NULL,
	[CreatedDateTime] [datetime] NULL,
	[UpdatedBy] [varchar](100) NULL,
	[UpdatedDateTime] [datetime] NULL,
	[UpdateReason] [varchar](100) NULL,
	[IsDeleted] [bit] NULL,
	[IsActive] [bit] NULL,
	[OwnerId] [varchar](100) NULL,
	[IsApproved] [bit] NULL,
	[ApproverId] [varchar](100) NULL,
	[ApprovedDateTime] [datetime] NULL,
	[IsAuthorized] [bit] NULL,
	[AuthorizedById] [varchar](100) NULL,
	[AuthorizedDateTime] [datetime] NULL,
	[TenantId] [varchar](100) NULL,
	[SubTenantId] [varchar](100) NULL,
	[SysData] [nvarchar](max) NULL,
	[AccessLevel] [varchar](50) NULL,
	[CorrelationId] [varchar](50) NULL,
	[AuditableRequestId] [varchar](50) NULL,
	[AuditableRequestName] [varchar](100) NULL,
	[AuditableSourceEventName] [varchar](100) NULL,
	[auth_type] [varchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserAccessLevel]    Script Date: 21-01-2026 14:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserAccessLevel](
	[AccessLevel] [varchar](50) NOT NULL,
	[DisplayName] [varchar](200) NOT NULL,
	[Hierarchy] [int] NOT NULL,
	[CreatedBy] [varchar](100) NULL,
	[CreatedDateTime] [datetime] NULL,
	[UpdatedBy] [varchar](100) NULL,
	[UpdatedDateTime] [datetime] NULL,
	[UpdateReason] [varchar](100) NULL,
	[IsDeleted] [bit] NULL,
	[IsActive] [bit] NULL,
	[OwnerId] [varchar](100) NULL,
	[IsApproved] [bit] NULL,
	[ApproverId] [varchar](100) NULL,
	[ApprovedDateTime] [datetime] NULL,
	[IsAuthorized] [bit] NULL,
	[AuthorizedById] [varchar](100) NULL,
	[AuthorizedDateTime] [datetime] NULL,
	[TenantId] [varchar](100) NULL,
	[SubTenantId] [varchar](100) NULL,
	[SysData] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[AccessLevel] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserActivity]    Script Date: 21-01-2026 14:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserActivity](
	[Id] [varchar](100) NOT NULL,
	[UserId] [varchar](100) NOT NULL,
	[LastLoginDateTime] [datetime] NULL,
	[LastLogoutDateTime] [datetime] NULL,
	[LastActivityDateTime] [datetime] NULL,
	[LastActivityModule] [varchar](200) NULL,
	[LastActionType] [varchar](200) NULL,
	[LastActivityDetail] [nvarchar](max) NULL,
	[CreatedBy] [varchar](100) NULL,
	[CreatedDateTime] [datetime] NULL,
	[UpdatedBy] [varchar](100) NULL,
	[UpdatedDateTime] [datetime] NULL,
	[UpdateReason] [varchar](100) NULL,
	[IsDeleted] [bit] NULL,
	[IsActive] [bit] NULL,
	[OwnerId] [varchar](100) NULL,
	[IsApproved] [bit] NULL,
	[ApproverId] [varchar](100) NULL,
	[ApprovedDateTime] [datetime] NULL,
	[IsAuthorized] [bit] NULL,
	[AuthorizedById] [varchar](100) NULL,
	[AuthorizedDateTime] [datetime] NULL,
	[TenantId] [varchar](100) NULL,
	[SubTenantId] [varchar](100) NULL,
	[SysData] [nvarchar](max) NULL,
	[CustomFields] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserPasswordHash]    Script Date: 21-01-2026 14:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserPasswordHash](
	[UserId] [varchar](100) NOT NULL,
	[PasswordHash] [varchar](200) NULL,
	[UpdatedBy] [varchar](200) NULL,
	[UpdatedDateTime] [datetime] NULL,
	[UpdateReason] [varchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserProfile]    Script Date: 21-01-2026 14:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserProfile](
	[Id] [varchar](100) NOT NULL,
	[UserId] [varchar](100) NOT NULL,
	[DOB] [varchar](200) NULL,
	[Gender] [varchar](200) NULL,
	[BloodGroup] [varchar](200) NULL,
	[PersonalEmail] [varchar](200) NULL,
	[DateOfJoining] [varchar](200) NULL,
	[PassportNumber] [varchar](100) NULL,
	[FatherName] [varchar](200) NULL,
	[MotherName] [varchar](200) NULL,
	[MaritalStatus] [varchar](200) NULL,
	[WeddingAnniversaryDate] [varchar](200) NULL,
	[SpouseName] [varchar](200) NULL,
	[SpouseDOB] [varchar](200) NULL,
	[HomeAddress1] [varchar](200) NULL,
	[HomeAddress2] [varchar](200) NULL,
	[City] [varchar](200) NULL,
	[State] [varchar](200) NULL,
	[HomeAddressCity] [varchar](200) NULL,
	[HomeAddressState] [varchar](200) NULL,
	[HomeAddressCountry] [varchar](200) NULL,
	[HomePhoneNumber] [varchar](200) NULL,
	[EmergencyContactNumber] [varchar](200) NULL,
	[EmergencyContactName] [varchar](200) NULL,
	[PrimarySkills] [varchar](200) NULL,
	[SecondarySkills] [varchar](200) NULL,
	[TertiarySkills] [varchar](200) NULL,
	[OtherSkills] [varchar](200) NULL,
	[Branch] [varchar](200) NULL,
	[LookUpCode] [varchar](200) NULL,
	[OtherId] [varchar](200) NULL,
	[LinkedInUrl] [varchar](200) NULL,
	[UserPic] [varchar](100) NULL,
	[CreatedBy] [varchar](100) NULL,
	[CreatedDateTime] [datetime] NULL,
	[UpdatedBy] [varchar](100) NULL,
	[UpdatedDateTime] [datetime] NULL,
	[UpdateReason] [varchar](100) NULL,
	[IsDeleted] [bit] NULL,
	[IsActive] [bit] NULL,
	[OwnerId] [varchar](100) NULL,
	[IsApproved] [bit] NULL,
	[ApproverId] [varchar](100) NULL,
	[ApprovedDateTime] [datetime] NULL,
	[IsAuthorized] [bit] NULL,
	[AuthorizedById] [varchar](100) NULL,
	[AuthorizedDateTime] [datetime] NULL,
	[TenantId] [varchar](100) NULL,
	[SubTenantId] [varchar](100) NULL,
	[SysData] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRole]    Script Date: 21-01-2026 14:47:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRole](
	[Id] [varchar](100) NOT NULL,
	[UserId] [varchar](100) NOT NULL,
	[RoleId] [varchar](100) NOT NULL,
	[CreatedBy] [varchar](100) NULL,
	[CreatedDateTime] [datetime] NULL,
	[UpdatedBy] [varchar](100) NULL,
	[UpdatedDateTime] [datetime] NULL,
	[UpdateReason] [varchar](100) NULL,
	[IsDeleted] [bit] NULL,
	[IsActive] [bit] NULL,
	[OwnerId] [varchar](100) NULL,
	[IsApproved] [bit] NULL,
	[ApproverId] [varchar](100) NULL,
	[ApprovedDateTime] [datetime] NULL,
	[IsAuthorized] [bit] NULL,
	[AuthorizedById] [varchar](100) NULL,
	[AuthorizedDateTime] [datetime] NULL,
	[TenantId] [varchar](100) NULL,
	[SubTenantId] [varchar](100) NULL,
	[SysData] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]





/****** Object:  Index [UQ_AuthReferenceLookup_Name_Type]    Script Date: 21-01-2026 14:47:37 ******/
ALTER TABLE [dbo].[AuthReferenceLookup] ADD  CONSTRAINT [UQ_AuthReferenceLookup_Name_Type] UNIQUE NONCLUSTERED 
(
	[Name] ASC,
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Permissi__45620F4150BFF1F6]    Script Date: 21-01-2026 14:47:37 ******/
ALTER TABLE [dbo].[Permission] ADD UNIQUE NONCLUSTERED 
(
	[PermissionValue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Permissi__45620F41BE63CE7F]    Script Date: 21-01-2026 14:47:37 ******/
ALTER TABLE [dbo].[Permission] ADD UNIQUE NONCLUSTERED 
(
	[PermissionValue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Permissi__D2CABA3A19F91D63]    Script Date: 21-01-2026 14:47:37 ******/
ALTER TABLE [dbo].[Permission] ADD UNIQUE NONCLUSTERED 
(
	[PermissionDisplayName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Permissi__D2CABA3A8E5434F2]    Script Date: 21-01-2026 14:47:37 ******/
ALTER TABLE [dbo].[Permission] ADD UNIQUE NONCLUSTERED 
(
	[PermissionDisplayName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ_Permission_PermissionValue]    Script Date: 21-01-2026 14:47:37 ******/
ALTER TABLE [dbo].[Permission] ADD  CONSTRAINT [UQ_Permission_PermissionValue] UNIQUE NONCLUSTERED 
(
	[PermissionValue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UC_PermissionDenied]    Script Date: 21-01-2026 14:47:37 ******/
ALTER TABLE [dbo].[PermissionDenied] ADD  CONSTRAINT [UC_PermissionDenied] UNIQUE NONCLUSTERED 
(
	[UserId] ASC,
	[PermissionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UC_PermissionGranted]    Script Date: 21-01-2026 14:47:37 ******/
ALTER TABLE [dbo].[PermissionGranted] ADD  CONSTRAINT [UC_PermissionGranted] UNIQUE NONCLUSTERED 
(
	[UserId] ASC,
	[PermissionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [Role_Info]    Script Date: 21-01-2026 14:47:37 ******/
ALTER TABLE [dbo].[Role] ADD  CONSTRAINT [Role_Info] UNIQUE NONCLUSTERED 
(
	[DisplayName] ASC,
	[AzureRoleGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UC_RolePermission]    Script Date: 21-01-2026 14:47:37 ******/
ALTER TABLE [dbo].[RolePermission] ADD  CONSTRAINT [UC_RolePermission] UNIQUE NONCLUSTERED 
(
	[RoleId] ASC,
	[PermissionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UC_RoleUiPermission]    Script Date: 21-01-2026 14:47:37 ******/
ALTER TABLE [dbo].[RoleUiPermission] ADD  CONSTRAINT [UC_RoleUiPermission] UNIQUE NONCLUSTERED 
(
	[RoleId] ASC,
	[UiPermissionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ_UiPermission_PermissionValue]    Script Date: 21-01-2026 14:47:37 ******/
ALTER TABLE [dbo].[UiPermission] ADD  CONSTRAINT [UQ_UiPermission_PermissionValue] UNIQUE NONCLUSTERED 
(
	[PermissionValue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UC_UserIdUiPermissionDenied]    Script Date: 21-01-2026 14:47:37 ******/
ALTER TABLE [dbo].[UiPermissionDenied] ADD  CONSTRAINT [UC_UserIdUiPermissionDenied] UNIQUE NONCLUSTERED 
(
	[UserId] ASC,
	[UiPermissionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UC_UserIdUiPermissionGranted]    Script Date: 21-01-2026 14:47:37 ******/
ALTER TABLE [dbo].[UiPermissionGranted] ADD  CONSTRAINT [UC_UserIdUiPermissionGranted] UNIQUE NONCLUSTERED 
(
	[UserId] ASC,
	[UiPermissionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UC_User_Email]    Script Date: 21-01-2026 14:47:37 ******/
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [UC_User_Email] UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UC_User_UserName]    Script Date: 21-01-2026 14:47:37 ******/
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [UC_User_UserName] UNIQUE NONCLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__User__C9F28456DF399871]    Script Date: 21-01-2026 14:47:37 ******/
ALTER TABLE [dbo].[User] ADD UNIQUE NONCLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__UserAcce__4E3E687D1854ADC0]    Script Date: 21-01-2026 14:47:37 ******/
ALTER TABLE [dbo].[UserAccessLevel] ADD UNIQUE NONCLUSTERED 
(
	[DisplayName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AuthReferenceLookup] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Permission] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Permission] ADD  DEFAULT (getutcdate()) FOR [CreatedDateTime]
GO
ALTER TABLE [dbo].[Permission] ADD  DEFAULT (getutcdate()) FOR [UpdatedDateTime]
GO
ALTER TABLE [dbo].[Permission] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Permission] ADD  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Permission] ADD  DEFAULT ((0)) FOR [IsApproved]
GO
ALTER TABLE [dbo].[Permission] ADD  DEFAULT ((0)) FOR [IsAuthorized]
GO
ALTER TABLE [dbo].[PermissionDenied] ADD  CONSTRAINT [DF_PermissionDenied_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[PermissionDenied] ADD  DEFAULT (getutcdate()) FOR [CreatedDateTime]
GO
ALTER TABLE [dbo].[PermissionDenied] ADD  DEFAULT (getutcdate()) FOR [UpdatedDateTime]
GO
ALTER TABLE [dbo].[PermissionDenied] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[PermissionDenied] ADD  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[PermissionDenied] ADD  DEFAULT ((0)) FOR [IsApproved]
GO
ALTER TABLE [dbo].[PermissionDenied] ADD  DEFAULT ((0)) FOR [IsAuthorized]
GO
ALTER TABLE [dbo].[PermissionGranted] ADD  CONSTRAINT [DF_PermissionGranted_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[PermissionGranted] ADD  DEFAULT (getutcdate()) FOR [CreatedDateTime]
GO
ALTER TABLE [dbo].[PermissionGranted] ADD  DEFAULT (getutcdate()) FOR [UpdatedDateTime]
GO
ALTER TABLE [dbo].[PermissionGranted] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[PermissionGranted] ADD  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[PermissionGranted] ADD  DEFAULT ((0)) FOR [IsApproved]
GO
ALTER TABLE [dbo].[PermissionGranted] ADD  DEFAULT ((0)) FOR [IsAuthorized]
GO
ALTER TABLE [dbo].[Role] ADD  CONSTRAINT [DF_Role_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Role] ADD  CONSTRAINT [DF_Role_AzureRoleGuid]  DEFAULT (CONVERT([varchar](100),newid())) FOR [AzureRoleGuid]
GO
ALTER TABLE [dbo].[Role] ADD  DEFAULT (getutcdate()) FOR [CreatedDateTime]
GO
ALTER TABLE [dbo].[Role] ADD  DEFAULT (getutcdate()) FOR [UpdatedDateTime]
GO
ALTER TABLE [dbo].[Role] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Role] ADD  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Role] ADD  DEFAULT ((0)) FOR [IsApproved]
GO
ALTER TABLE [dbo].[Role] ADD  DEFAULT ((0)) FOR [IsAuthorized]
GO
ALTER TABLE [dbo].[RolePermission] ADD  CONSTRAINT [DF_RolePermission_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[RolePermission] ADD  DEFAULT (getutcdate()) FOR [CreatedDateTime]
GO
ALTER TABLE [dbo].[RolePermission] ADD  DEFAULT (getutcdate()) FOR [UpdatedDateTime]
GO
ALTER TABLE [dbo].[RolePermission] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[RolePermission] ADD  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[RolePermission] ADD  DEFAULT ((0)) FOR [IsApproved]
GO
ALTER TABLE [dbo].[RolePermission] ADD  DEFAULT ((0)) FOR [IsAuthorized]
GO
ALTER TABLE [dbo].[RoleUiPermission] ADD  DEFAULT ((0)) FOR [CanView]
GO
ALTER TABLE [dbo].[RoleUiPermission] ADD  DEFAULT ((0)) FOR [CanEdit]
GO
ALTER TABLE [dbo].[RoleUiPermission] ADD  DEFAULT ((0)) FOR [CanCreate]
GO
ALTER TABLE [dbo].[RoleUiPermission] ADD  DEFAULT ((0)) FOR [CanDelete]
GO
ALTER TABLE [dbo].[RoleUiPermission] ADD  DEFAULT (getutcdate()) FOR [CreatedDateTime]
GO
ALTER TABLE [dbo].[RoleUiPermission] ADD  DEFAULT (getutcdate()) FOR [UpdatedDateTime]
GO
ALTER TABLE [dbo].[RoleUiPermission] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[RoleUiPermission] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[RoleUiPermission] ADD  DEFAULT ((0)) FOR [IsApproved]
GO
ALTER TABLE [dbo].[RoleUiPermission] ADD  DEFAULT ((0)) FOR [IsAuthorized]
GO
ALTER TABLE [dbo].[UiPermission] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[UiPermission] ADD  DEFAULT (getutcdate()) FOR [CreatedDateTime]
GO
ALTER TABLE [dbo].[UiPermission] ADD  DEFAULT (getutcdate()) FOR [UpdatedDateTime]
GO
ALTER TABLE [dbo].[UiPermission] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[UiPermission] ADD  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[UiPermission] ADD  DEFAULT ((0)) FOR [IsApproved]
GO
ALTER TABLE [dbo].[UiPermission] ADD  DEFAULT ((0)) FOR [IsAuthorized]
GO
ALTER TABLE [dbo].[UiPermissionDenied] ADD  DEFAULT ((0)) FOR [CanView]
GO
ALTER TABLE [dbo].[UiPermissionDenied] ADD  DEFAULT ((0)) FOR [CanEdit]
GO
ALTER TABLE [dbo].[UiPermissionDenied] ADD  DEFAULT ((0)) FOR [CanCreate]
GO
ALTER TABLE [dbo].[UiPermissionDenied] ADD  DEFAULT ((0)) FOR [CanDelete]
GO
ALTER TABLE [dbo].[UiPermissionDenied] ADD  DEFAULT (getutcdate()) FOR [CreatedDateTime]
GO
ALTER TABLE [dbo].[UiPermissionDenied] ADD  DEFAULT (getutcdate()) FOR [UpdatedDateTime]
GO
ALTER TABLE [dbo].[UiPermissionDenied] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[UiPermissionDenied] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[UiPermissionDenied] ADD  DEFAULT ((0)) FOR [IsApproved]
GO
ALTER TABLE [dbo].[UiPermissionDenied] ADD  DEFAULT ((0)) FOR [IsAuthorized]
GO
ALTER TABLE [dbo].[UiPermissionGranted] ADD  DEFAULT ((0)) FOR [CanView]
GO
ALTER TABLE [dbo].[UiPermissionGranted] ADD  DEFAULT ((0)) FOR [CanEdit]
GO
ALTER TABLE [dbo].[UiPermissionGranted] ADD  DEFAULT ((0)) FOR [CanCreate]
GO
ALTER TABLE [dbo].[UiPermissionGranted] ADD  DEFAULT ((0)) FOR [CanDelete]
GO
ALTER TABLE [dbo].[UiPermissionGranted] ADD  DEFAULT (getutcdate()) FOR [CreatedDateTime]
GO
ALTER TABLE [dbo].[UiPermissionGranted] ADD  DEFAULT (getutcdate()) FOR [UpdatedDateTime]
GO
ALTER TABLE [dbo].[UiPermissionGranted] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[UiPermissionGranted] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[UiPermissionGranted] ADD  DEFAULT ((0)) FOR [IsApproved]
GO
ALTER TABLE [dbo].[UiPermissionGranted] ADD  DEFAULT ((0)) FOR [IsAuthorized]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[User] ADD  DEFAULT (getutcdate()) FOR [CreatedDateTime]
GO
ALTER TABLE [dbo].[User] ADD  DEFAULT (getutcdate()) FOR [UpdatedDateTime]
GO
ALTER TABLE [dbo].[User] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[User] ADD  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[User] ADD  DEFAULT ((0)) FOR [IsApproved]
GO
ALTER TABLE [dbo].[User] ADD  DEFAULT ((0)) FOR [IsAuthorized]
GO
ALTER TABLE [dbo].[UserAccessLevel] ADD  DEFAULT (getutcdate()) FOR [CreatedDateTime]
GO
ALTER TABLE [dbo].[UserAccessLevel] ADD  DEFAULT (getutcdate()) FOR [UpdatedDateTime]
GO
ALTER TABLE [dbo].[UserAccessLevel] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[UserAccessLevel] ADD  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[UserAccessLevel] ADD  DEFAULT ((0)) FOR [IsApproved]
GO
ALTER TABLE [dbo].[UserAccessLevel] ADD  DEFAULT ((0)) FOR [IsAuthorized]
GO
ALTER TABLE [dbo].[UserActivity] ADD  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[UserActivity] ADD  DEFAULT (getutcdate()) FOR [CreatedDateTime]
GO
ALTER TABLE [dbo].[UserActivity] ADD  DEFAULT (getutcdate()) FOR [UpdatedDateTime]
GO
ALTER TABLE [dbo].[UserActivity] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[UserActivity] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[UserActivity] ADD  DEFAULT ((0)) FOR [IsApproved]
GO
ALTER TABLE [dbo].[UserActivity] ADD  DEFAULT ((0)) FOR [IsAuthorized]
GO
ALTER TABLE [dbo].[UserProfile] ADD  CONSTRAINT [DF_UserProfile_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[UserProfile] ADD  DEFAULT (getutcdate()) FOR [CreatedDateTime]
GO
ALTER TABLE [dbo].[UserProfile] ADD  DEFAULT (getutcdate()) FOR [UpdatedDateTime]
GO
ALTER TABLE [dbo].[UserProfile] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[UserProfile] ADD  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[UserProfile] ADD  DEFAULT ((0)) FOR [IsApproved]
GO
ALTER TABLE [dbo].[UserProfile] ADD  DEFAULT ((0)) FOR [IsAuthorized]
GO
ALTER TABLE [dbo].[UserRole] ADD  CONSTRAINT [DF_UserRole_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[UserRole] ADD  DEFAULT (getutcdate()) FOR [CreatedDateTime]
GO
ALTER TABLE [dbo].[UserRole] ADD  DEFAULT (getutcdate()) FOR [UpdatedDateTime]
GO
ALTER TABLE [dbo].[UserRole] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[UserRole] ADD  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[UserRole] ADD  DEFAULT ((0)) FOR [IsApproved]
GO
ALTER TABLE [dbo].[UserRole] ADD  DEFAULT ((0)) FOR [IsAuthorized]
GO
ALTER TABLE [dbo].[Permission]  WITH CHECK ADD  CONSTRAINT [FK_AuthReferenceLookup_Id] FOREIGN KEY([ModuleId])
REFERENCES [dbo].[AuthReferenceLookup] ([Id])
GO
ALTER TABLE [dbo].[Permission] CHECK CONSTRAINT [FK_AuthReferenceLookup_Id]
GO
ALTER TABLE [dbo].[Permission]  WITH CHECK ADD  CONSTRAINT [FK_Permission_AuthReferenceLookup_Id] FOREIGN KEY([PermissionSetId])
REFERENCES [dbo].[AuthReferenceLookup] ([Id])
GO
ALTER TABLE [dbo].[Permission] CHECK CONSTRAINT [FK_Permission_AuthReferenceLookup_Id]
GO
ALTER TABLE [dbo].[Permission]  WITH CHECK ADD  CONSTRAINT [FK_Permission_ModuleId] FOREIGN KEY([ModuleId])
REFERENCES [dbo].[AuthReferenceLookup] ([Id])
GO
ALTER TABLE [dbo].[Permission] CHECK CONSTRAINT [FK_Permission_ModuleId]
GO
ALTER TABLE [dbo].[PermissionDenied]  WITH CHECK ADD FOREIGN KEY([PermissionId])
REFERENCES [dbo].[Permission] ([Id])
GO
ALTER TABLE [dbo].[PermissionDenied]  WITH CHECK ADD FOREIGN KEY([PermissionId])
REFERENCES [dbo].[Permission] ([Id])
GO
ALTER TABLE [dbo].[PermissionDenied]  WITH CHECK ADD  CONSTRAINT [FK_PermissionDenied_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[PermissionDenied] CHECK CONSTRAINT [FK_PermissionDenied_UserId]
GO
ALTER TABLE [dbo].[PermissionGranted]  WITH CHECK ADD FOREIGN KEY([PermissionId])
REFERENCES [dbo].[Permission] ([Id])
GO
ALTER TABLE [dbo].[PermissionGranted]  WITH CHECK ADD FOREIGN KEY([PermissionId])
REFERENCES [dbo].[Permission] ([Id])
GO
ALTER TABLE [dbo].[PermissionGranted]  WITH CHECK ADD  CONSTRAINT [FK_PermissionGranted_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[PermissionGranted] CHECK CONSTRAINT [FK_PermissionGranted_UserId]
GO
ALTER TABLE [dbo].[RolePermission]  WITH CHECK ADD FOREIGN KEY([PermissionId])
REFERENCES [dbo].[Permission] ([Id])
GO
ALTER TABLE [dbo].[RolePermission]  WITH CHECK ADD FOREIGN KEY([PermissionId])
REFERENCES [dbo].[Permission] ([Id])
GO
ALTER TABLE [dbo].[RolePermission]  WITH CHECK ADD FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[RolePermission]  WITH CHECK ADD FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[RoleUiPermission]  WITH CHECK ADD FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[UiPermission]  WITH CHECK ADD FOREIGN KEY([UiPermissionTypeId])
REFERENCES [dbo].[AuthReferenceLookup] ([Id])
GO
ALTER TABLE [dbo].[UiPermission]  WITH CHECK ADD  CONSTRAINT [FK_UIPermission_ModuleId] FOREIGN KEY([ModuleId])
REFERENCES [dbo].[AuthReferenceLookup] ([Id])
GO
ALTER TABLE [dbo].[UiPermission] CHECK CONSTRAINT [FK_UIPermission_ModuleId]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD FOREIGN KEY([AccessLevel])
REFERENCES [dbo].[UserAccessLevel] ([AccessLevel])
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD FOREIGN KEY([AccessLevel])
REFERENCES [dbo].[UserAccessLevel] ([AccessLevel])
GO
ALTER TABLE [dbo].[UserActivity]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserProfile]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserProfile]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_RoleId]
GO
ALTER TABLE [dbo].[UserRole]  WITH NOCHECK ADD  CONSTRAINT [FK_UserRole_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_UserId]
GO
ALTER TABLE [dbo].[UserActivity]  WITH CHECK ADD  CONSTRAINT [ck_customfieldsUserActivity_json] CHECK  ((isjson([CustomFields])=(1)))
GO
ALTER TABLE [dbo].[UserActivity] CHECK CONSTRAINT [ck_customfieldsUserActivity_json]
GO
/****** Object:  StoredProcedure [dbo].[sp_Identity_SelectUser]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE    PROCEDURE [dbo].[sp_Identity_SelectUser]   --'vibhtti@spectraltech.ai'       
@userName_userId_userOid  varchar(100)       
                                                    
AS                                                    
BEGIN                      
              
 Declare @UserId varchar(100) = (Select Id from [User]  where ( Id = @userName_userId_userOid OR oid = @userName_userId_userOid OR UserName = @userName_userId_userOid))        
      
--User                                      
Select U.Id as userId, U.EmpId, U.EmpType, U.UserName, U.FirstName, U.LastName, U.Email, U.SecondaryEmail, U.PhoneNumber, U.Extension,                                                    
 U.mobile, U.oid, U.preferred_username, U.display_name, U.given_name, U.family_name, U.Position, U.BusinessUnit,                                                    
 u.ManagerId,M.UserName As ManagerUserName, U.AccessLevel ,U.IsActive, U.Designation ,U.Department ,U.Location ,U.Organization,                    
                     
 --Audit Fields                    
 U.CorrelationId, U.AuditableRequestId, U.AuditableRequestName, U.AuditableSourceEventName, U.CreatedBy, U.CreatedDateTime, U.UpdatedBy,                     
 U.UpdatedDateTime, U.UpdateReason, U.OwnerId, /*U.IsActive,*/ U.IsDeleted, U.IsApproved, U.ApproverId, U.ApprovedDateTime,                     
 U.IsAuthorized, U.AuthorizedById, U.AuthorizedDateTime, U.SysData, U.TenantId, U.SubTenantId --,                     
                    
 --Token                    
 --UToken.RcTokenObject,                     
 --UToken.ModifiedDateTime as RcTokenDateTime, UserPic,UProfile.DefaultProspectListId, UProfile.DefaultConatctListId,                     
 --UProfile.DefaultWBAListId                           
 from [dbo].[User] as U                                                    
 Left JOIN  [dbo].[User] as M On U.ManagerId=M.Id And (M.IsDeleted Is NUll Or M.IsDeleted =0)                               
 --Left JOIN  [dbo].[UserRCToken] as UToken On UToken.UserId=U.Id                            
 --Left Join [DBO].[USERPROFILE] as UProfile On UProfile.UserId=U.Id                            
 where (U.Id=@UserId OR U.OID=@UserId)                                           
 ORDER BY U.display_name ASC                                          
                                             
--User Role                                      
 Select UR.Id As UserRoleId,R.Id As RoleId,R.RoleName,R.RoleValue,R.DisplayName,U.UserName,U.Id as UserId                                                 
 from UserRole UR                                                     
 JOIN Role R ON   (R.IsDeleted Is NUll Or R.IsDeleted =0) And Ur.RoleId=R.Id                                                    
 JOIN [dbo].[User] as U On (U.IsDeleted Is NUll Or U.IsDeleted =0) and Ur.UserId=U.Id                                                    
 Where (UR.IsDeleted Is NUll Or UR.IsDeleted =0) And  ((@UserId Is Null Or @UserId='0') OR UR.UserId=@UserId OR U.OID=@UserId)                                                    
 ORDER BY R.RoleName ASC                                             
                                           
-- User Permissions Granted                                      
 Select P.Id As PermissionId,PG.Id As PermissionGrantedId,P.PermissionValue,                                      
 P.PermissionDisplayName,U.UserName,U.Id  as UserId,          
 P.PermissionSetId, ar.DisplayName as PermissionSetName,          
 p.ModuleId, a.DisplayName as ModuleName, P.ApiName            
 -- ,pgr.Id as PermissionGroupId,                                      
 --pgr.PermissionGroupDisplayName,pgr.Module,ps.Id as PermissionSetId                                       
 from PermissionGranted PG                                                    
 JOIN  Permission P On P.Id=PG.PermissionId And (P.IsDeleted Is NUll Or P.IsDeleted =0)                                                    
 JOIN [dbo].[User] as U On (U.IsDeleted Is NUll Or U.IsDeleted =0) and PG.UserId=U.Id              
 INNER JOIN AuthReferenceLookup as a ON  a.Id = P.ModuleId        
 INNER JOIN AuthReferenceLookup as ar ON  ar.Id = P.PermissionSetId               
          
-- INNER JOIN PermissionSet as ps ON  ps.Id = P.PermissionSetId             
 --INNER JOIN PermissionGroup as pgr ON pgr.Id = ps.PermissionGroupId AND (pgr.IsDeleted Is NUll Or pgr.IsDeleted =0)                                       
 Where (PG.IsDeleted Is NUll Or PG.IsDeleted =0) And  ((@UserId Is Null Or @UserId='0') OR PG.UserId=@UserId OR            
 U.OID=@UserId)                                                    
 ORDER BY P.PermissionDisplayName ASC                        
                                         
-- User Permissions Denied                                         
 Select  P.Id As PermissionId,PD.Id As PermissionDeniedId,P.PermissionValue,P.PermissionDisplayName,                                      
 U.UserName,U.Id as UserId ,            
 P.PermissionSetId, ar.DisplayName as PermissionSetName,          
 p.ModuleId, a.DisplayName as ModuleName, P.ApiName--,pgr.Id as PermissionGroupId,                 
 --pgr.PermissionGroupDisplayName,pgr.Module,ps.Id as PermissionSetId                                                   
 from PermissionDenied PD                             
 JOIN  Permission P On P.Id=PD.PermissionId And (P.IsDeleted Is NUll Or P.IsDeleted =0)                                                    
 JOIN [dbo].[User] as U On (U.IsDeleted Is NUll Or U.IsDeleted =0) and PD.UserId=U.Id              
  INNER JOIN AuthReferenceLookup as a ON  a.Id = P.ModuleId            
 INNER JOIN AuthReferenceLookup as ar ON  ar.Id = P.PermissionSetId               
            
  --INNER JOIN PermissionSet as ps ON  ps.Id = P.PermissionSetId                                      
 --INNER JOIN PermissionGroup as pgr ON pgr.Id = ps.PermissionGroupId AND (pgr.IsDeleted Is NUll Or pgr.IsDeleted =0)                                       
 Where (PD.IsDeleted Is NUll Or PD.IsDeleted =0) And  ((@UserId Is Null Or @UserId='0') OR PD.UserId=@UserId OR             
 U.OID=@UserId)                                                   
 ORDER BY P.PermissionDisplayName ASC             
         
        
        
        
 --************************************************************************************************************        
 --********************************  User UI Permission  ******************************************************        
 --************************************************************************************************************        
 Declare @RoleIdList nvarchar(max)        
        
-- This query concatenates multiple role IDs into a comma-separated string        
Select @RoleIdList = STRING_AGG(CAST(RoleId as NVARCHAR(max)), ',')         
From UserRole         
Where (UserId = @UserId  OR (@UserId Is Null Or @UserId='0'))        
        
-- If there are no roles for the given user, set the variable to an empty string        
If @RoleIdList Is Null        
    Set @RoleIdList = ''        
        
        
        
        
        
 Select distinct a.UserId,a.UIPermissionId as PermissionId,         
a.PermissionValue,a.PermissionDisplayName,                      
a.PermissionTypeId,         
 a.PermissionTypeName, a.PermissionParentId, a.PermissionParentName,            
 a.ModuleId ,        
 a.ModuleName,          
         
    MAX(CASE WHEN a.CanCreate = 1 THEN 1 ELSE 0 END) AS CanCreate,        
    MAX(CASE WHEN a.CanEdit = 1 THEN 1 ELSE 0 END) AS CanEdit,        
    MAX(CASE WHEN a.CanDelete = 1 THEN 1 ELSE 0 END) AS CanDelete,        
 MAX(CASE WHEN a.CanView = 1 THEN 1 ELSE 0 END) AS CanView,        
 MAX(CASE WHEN a.IsUiPermissionDenied = 1 THEN 1 ELSE 0 END) AS IsUiPermissionDenied,        
    MAX(CASE WHEN a.IsUiPermissionGranted = 1 THEN 1 ELSE 0 END) AS IsUiPermissionGranted FROM        
(         
   --1. UI PERMISSION DENIED                 
   Select pd.UIPermissionId As UIPermissionId,pd.UserId As UserId,             
   P.PermissionValue,P.PermissionDisplayName,                    
   P.UiPermissionTypeId as PermissionTypeId ,a.DisplayName as PermissionTypeName, P.PermissionParentId,        
   UP.PermissionDisplayName as PermissionParentName,P.ModuleId, ar.DisplayName as ModuleName,            
   pd.CanCreate,pd.CanEdit,pd.CanDelete, pd.CanView ,1 as IsUiPermissionDenied , 0 as IsUiPermissionGranted                           
   from [dbo].[UIPermissionDenied] as pd                                  
   INNER JOIN  UIPermission P On pd.UIPermissionId = P.Id  and P.IsActive = 1                     
   INNER JOIN  AuthReferenceLookup a On a.Id = P.UiPermissionTypeId                      
   INNER JOIN  AuthReferenceLookup aR On aR.Id = P.ModuleId             
   LEFT JOIN  UIPermission UP On UP.Id = P.PermissionParentId                      
 --  INNER JOIN [dbo].[User] as U On  pd.UserId= U.Id         
  where --pd.UserId = @UserId        
   (pd.UserId = @UserId  OR (@UserId Is Null Or @UserId='0'))        
          
         
  Union All            
                                
   --2.  UI PERMISSION GRANTED                  
   Select pgr.UIPermissionId As UIPermissionId,pgr.UserId As UserId,                            
   P.PermissionValue,P.PermissionDisplayName,                    
   P.UiPermissionTypeId as PermissionTypeId ,a.DisplayName as PermissionTypeName,            
   P.ModuleId, ar.DisplayName as ModuleName, P.PermissionParentId, up.PermissionDisplayName as PermissionParentName,            
   pgr.CanCreate,pgr.CanEdit,pgr.CanDelete, pgr.CanView  ,0 as IsUiPermissionDenied, 1 as IsUiPermissionGranted                                  
   from [dbo].[UIPermissionGranted] as pgr                                  
   INNER JOIN  UIPermission P On pgr.UIPermissionId = P.Id  and P.IsActive = 1              
   INNER JOIN  AuthReferenceLookup a On a.Id = P.UiPermissionTypeId                
   INNER JOIN  AuthReferenceLookup ar On ar.Id = P.ModuleId                   
   Left JOIN  UiPermission up On up.Id = P.PermissionParentId                  
   --INNER JOIN [dbo].[User] as U On  pgr.UserId= U.Id          
   where --pgr.UserId = @UserId        
    (pgr.UserId = @UserId  OR (@UserId Is Null Or @UserId='0'))        
         
   Union All                             
   -- 3. UI PERMISSIONS OF ROLE                     
   Select RP.UIPermissionId As UIPermissionId,u.UserId,                          
   P.PermissionValue,P.PermissionDisplayName,                      
   P.UiPermissionTypeId as PermissionTypeId, a.DisplayName as PermissionTypeName, P.PermissionParentId, UP.PermissionDisplayName as PermissionParentName,            
   P.ModuleId ,ar.DisplayName as ModuleName,          
   MAX(CASE WHEN RP.CanCreate = 1 THEN 1 ELSE 0 END) AS CanCreate,        
   MAX(CASE WHEN RP.CanEdit = 1 THEN 1 ELSE 0 END) AS CanEdit,        
   MAX(CASE WHEN RP.CanDelete = 1 THEN 1 ELSE 0 END) AS CanDelete,        
   MAX(CASE WHEN RP.CanView = 1 THEN 1 ELSE 0 END) AS CanView,        
   0 as IsUiPermissionDenied, 0 as IsUiPermissionGranted                                         
   from [dbo].[RoleUIPermission] as RP                                
   INNER JOIN  UIPermission P On RP.UIPermissionId = P.Id  and P.IsActive =1              
   INNER JOIN  AuthReferenceLookup a On a.Id = P.UiPermissionTypeId            
   INNER JOIN  AuthReferenceLookup ar On ar.Id = P.ModuleId            
   LEFT JOIN  UIPermission UP On UP.Id = P.PermissionParentId           
   INNER JOIN [dbo].[UserRole] as u On  u.RoleId= RP.RoleId        
   where RP.RoleId in ( SELECT *  FROM STRING_SPLIT(@RoleIdList, ',' ) )        
   GROUP BY        
       u.UserId,        
    RP.UIPermissionId,        
    P.PermissionValue,        
    P.PermissionDisplayName,        
    P.UiPermissionTypeId,        
    a.DisplayName,        
    P.PermissionParentId,        
    UP.PermissionDisplayName,        
    P.ModuleId,        
    ar.DisplayName        
        
 ) as a        
 Group by           
    a.UserId,       
    a.UIPermissionId,        
    a.PermissionValue,        
    a.PermissionDisplayName,        
    a.PermissionTypeId,        
    a.PermissionTypeName,        
    a.PermissionParentId,        
    a.PermissionParentName,        
    a.ModuleId,        
    a.ModuleName;        
                                             
END 
GO
/****** Object:  StoredProcedure [dbo].[sp_Permission_Save]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE        PROCEDURE [dbo].[sp_Permission_Save]          
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
        
  CreatedBy,IsAuthorized,OwnerId,SysData,        
 TenantId ,SubTenantId,        
 IsDeleted,IsActive,CreatedDateTime        
 )          
          
  Values (@PermissionValue,@PermissionDisplayName,@PermissionSetId,@ModuleId,      
  @CreatedBy,          
  @IsAuthorized , @OwnerId , @SysData,@TenantId ,@SubTenantId,        
  0,1,GETDATE()        
  )          
END   
GO
/****** Object:  StoredProcedure [dbo].[sp_Permission_Select]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE      PROCEDURE  [dbo].[sp_Permission_Select]                
AS                
BEGIN                
 SELECT p.Id,p.PermissionValue,p.PermissionDisplayName,        
 p.PermissionSetId, ar.DisplayName as PermissionSetName,      
 p.ModuleId ,a.DisplayName as ModuleName, p.ApiName           
 FROM Permission  AS p              
 INNER JOIN  AuthReferenceLookup a On a.Id = p.ModuleId      
 INNER JOIN  AuthReferenceLookup ar On ar.Id = p.PermissionSetId         
 ORDER BY PermissionDisplayName desc                
END 
GO
/****** Object:  StoredProcedure [dbo].[sp_Permission_Update]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE      PROCEDURE [dbo].[sp_Permission_Update]      
 @PermissionId varchar (100),    
 @PermissionValue varchar(200),      
 @PermissionDisplayName varchar(200),    
 @PermissionSetId varchar(100),    
 @ModuleId varchar(100),    
 @UpdatedDateTime DateTime,    
 @UpdatedBy varchar(100)      
AS      
BEGIN      
Update Permission     
Set    
    
 PermissionValue = @PermissionValue,     
 PermissionDisplayName = @PermissionDisplayName ,   
 PermissionSetId = @PermissionSetId,  
 ModuleId = @ModuleId,    
 UpdatedDateTime = GETUTCDATE(),    
 UpdatedBy= @UpdatedBy    
    
  where Id = @PermissionId     
END      
GO
/****** Object:  StoredProcedure [dbo].[sp_Role_Permission_Save]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_Role_Permission_Save]      
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
    
  --Delete all permissions for the Role    
  DELETE FROM RolePermission WHERE RoleId = @RoleId;    
    
  --If Permission exists    
  if(@PermissionIds != '')    
    Begin    
     --Insert all the permissions for a role    
     INSERT INTO RolePermission             
     (RoleId,CreatedDateTime,CreatedBy,UpdatedDateTime,UpdatedBy,IsDeleted,IsActive,IsAuthorized,OwnerId,SysData,    
     TenantId,SubTenantId,PermissionId)     
    
     select @RoleId,GETUTCDATE(),  @CreatedBy,GETUTCDATE(),@CreatedBy,0,1,@IsAuthorized,@OwnerId,@SysData,    
   @TenantId,@SubTenantId,a.* from(SELECT *  FROM STRING_SPLIT(@PermissionIds, ',')) a    
     END    
              
END    
GO
/****** Object:  StoredProcedure [dbo].[sp_Role_UIPermission_Save]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create   PROCEDURE [dbo].[sp_Role_UIPermission_Save]              
 @RoleId varchar(100),              
 @UiPermissionId varchar(100),              
 @UiPermissionCanCreate bit,              
 @UiPermissionCanEdit bit,          
 @UiPermissionCanDelete bit,     
 @UiPermissionCanView bit,            
 @CreatedBy varchar(100)              
AS              
BEGIN              
  INSERT INTO RoleUIPermission(RoleId,UiPermissionId,CanCreate,CanEdit,CanDelete, CanView,CreatedBy,CreatedDateTime            
 )              
      
  Values (@RoleId,@UiPermissionId,@UiPermissionCanCreate,@UiPermissionCanEdit,@UiPermissionCanDelete,    
  @UiPermissionCanView,@CreatedBy,GetUtcDate()          
  )              
END   
GO
/****** Object:  StoredProcedure [dbo].[sp_RoleInfo_Select]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create      PROCEDURE [dbo].[sp_RoleInfo_Select]                      
@RoleId varchar(100)                      
AS                      
BEGIN                      
Select r.Id,r.RoleName,r.RoleValue                  
 from Role as r                          
 where (r.IsDeleted Is NUll Or r.IsDeleted =0) and ((@RoleId Is Null Or @RoleId='0') OR r.Id=@RoleId)                      
               
                      
 Select rp.PermissionId  ,P.PermissionDisplayName,P.PermissionValue,P.PermissionSetId,  
 ar.DisplayName as PermissionSetName,  
 rp.RoleId  ,p.ModuleId,  a.DisplayName as ModuleName       
 from RolePermission as rp                      
 JOIN  Permission P On P.Id=rp.PermissionId And (P.IsDeleted Is NUll Or P.IsDeleted =0)     
 Inner Join AuthReferenceLookup as a on a.Id = p.ModuleId   
 Inner Join AuthReferenceLookup as ar on ar.Id = p.PermissionSetId    
   
 Where (rp.IsDeleted Is NUll Or rp.IsDeleted =0) And  ((@RoleId Is Null Or @RoleId='0') OR rp.RoleId=@RoleId)                                          
END   
GO
/****** Object:  StoredProcedure [dbo].[sp_RolePermissionInfo_Select]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
   CREATE       PROCEDURE [dbo].[sp_RolePermissionInfo_Select]               
@RoleId varchar(100)                      
AS                      
BEGIN                      
Select RP.PermissionId As PermissionId,RP.Id As RolePermissionId,R.Id As RoleId,                
P.PermissionValue,P.PermissionDisplayName,        
P.PermissionSetId, ar.DisplayName as PermissionSetName,      
P.ModuleId, a.DisplayName as ModuleName, P.ApiName,       
              
R.RoleName,R.RoleValue                      
 from [dbo].[RolePermission] as RP                      
 INNER JOIN  Permission P On RP.PermissionId = P.Id                  
 INNER JOIN [dbo].[Role] as R On  RP.RoleId= R.Id                   
 INNER JOIN  AuthReferenceLookup a On a.Id = P.ModuleId        
 INNER JOIN  AuthReferenceLookup ar On ar.Id = P.PermissionSetId        
 Where (RP.RoleId=@RoleId)                      
                      
END 
GO
/****** Object:  StoredProcedure [dbo].[sp_RoleUiPermissionsInfo_Select]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
   Create        PROCEDURE [dbo].[sp_RoleUiPermissionsInfo_Select]                                       
AS                            
BEGIN                            
 -- for ui permissions of Role             
 Select RP.UIPermissionId As UIPermissionId,R.Id As RoleId, R.RoleName,R.RoleValue,                      
P.PermissionValue,P.PermissionDisplayName,              
 P.UiPermissionTypeId as PermissionTypeId, a.DisplayName as PermissionTypeName, P.PermissionParentId, UP.PermissionDisplayName as PermissionParentName,    
 P.ModuleId ,ar.DisplayName as ModuleName,    
 RP.CanCreate,RP.CanEdit,RP.CanDelete, RP.CanView                                 
 from [dbo].[RoleUIPermission] as RP                            
 INNER JOIN  UIPermission P On RP.UIPermissionId = P.Id  and P.IsActive =1      
 INNER JOIN  AuthReferenceLookup a On a.Id = P.UiPermissionTypeId    
 INNER JOIN  AuthReferenceLookup ar On ar.Id = P.ModuleId    
 LEFT JOIN  UIPermission UP On UP.Id = P.PermissionParentId         
 INNER JOIN [dbo].[Role] as R On  RP.RoleId= R.Id        
   
END   
GO
/****** Object:  StoredProcedure [dbo].[sp_UIPermission_Activate]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE    PROCEDURE [dbo].[sp_UIPermission_Activate]                
 @PermissionId varchar (100),   
 @PermissionDisplayName varchar (200),              
 @IsActive bit,                        
 @UpdatedBy varchar(100),      
 @UpdatedDateTime varchar(200)                
       
AS                
BEGIN                
Update UIPermission               
SET                     
 IsActive = @IsActive,   
 PermissionDisplayName = @PermissionDisplayName,  
 UpdatedDateTime = @UpdatedDateTime,              
 UpdatedBy= @UpdatedBy                  
  where Id = @PermissionId               
END   
GO
/****** Object:  StoredProcedure [dbo].[sp_UIPermission_Save]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE     PROCEDURE [dbo].[sp_UIPermission_Save]              
 @PermissionValue varchar(200),              
 @PermissionDisplayName varchar(200),              
 @PermissionTypeId varchar(200),--,          
 @PermissionParentId varchar(100),--,       
 @ModuleId varchar(100),      
 @IsAuthorized bit,            
 @OwnerId varchar(100),            
 @SysData nvarchar(max),            
 @TenantId varchar(100),            
 @SubTenantId varchar(100),            
 @CreatedBy varchar(100)              
AS              
BEGIN              
  INSERT INTO UIPermission(PermissionValue,PermissionDisplayName,UiPermissionTypeId,PermissionParentId, ModuleId, CreatedBy,IsAuthorized,OwnerId,SysData,            
 TenantId ,SubTenantId,            
 IsDeleted,IsActive,CreatedDateTime            
 )              
 Output Inserted.Id      
      
  Values (@PermissionValue,@PermissionDisplayName,@PermissionTypeId,@PermissionParentId,@ModuleId,@CreatedBy,              
  @IsAuthorized , @OwnerId , @SysData,@TenantId ,@SubTenantId,            
  0,0,GETDATE()            
  )              
END   
GO
/****** Object:  StoredProcedure [dbo].[sp_UIPermission_Select]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  
CREATE      PROCEDURE  [dbo].[sp_UIPermission_Select]                  
AS                  
BEGIN                  
 SELECT p.Id,p.PermissionValue,p.PermissionDisplayName,          
 P.UiPermissionTypeId as PermissionTypeId,a.DisplayName as PermissionTypeName ,        
 p.ModuleId,ar.DisplayName as ModuleName,      
 P.PermissionParentId ,ui.PermissionDisplayName as PermissionParentName,  
 p.IsActive  
 FROM UIPermission  AS p           
 Inner  Join AuthReferenceLookup as a on a.Id = p.UiPermissionTypeId        
  Inner  Join AuthReferenceLookup as ar on ar.Id = p.ModuleId      
  Left  Join UiPermission as ui on ui.Id = p.PermissionParentId       
      
                
 ORDER BY PermissionDisplayName desc                  
END   
GO
/****** Object:  StoredProcedure [dbo].[sp_User_AddRole]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_User_AddRole]          
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
          
      
 IF NOT Exists( Select * FRom UserRole WHERE UserId = @UserId  AND RoleId =@RoleId )      
    BEGIN      
    --Insert the role  for a given user          
    INSERT INTO UserRole                   
    (UserId,CreatedDateTime,CreatedBy,UpdatedDateTime,UpdatedBy,IsDeleted,IsActive,IsAuthorized,OwnerId,SysData,          
    TenantId,SubTenantId,RoleId)           
          
    SELECT DISTINCT @UserId,GETUTCDATE(),@CreatedBy,GETUTCDATE(),@CreatedBy,0,1,@IsAuthorized,@OwnerId,@SysData,          
    @TenantId,@SubTenantId, @RoleId      
 END          
          
END    
GO
/****** Object:  StoredProcedure [dbo].[sp_User_AddRoles]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE     PROCEDURE [dbo].[sp_User_AddRoles]          
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
      
 --Check for Chris Green, John Smith, Dave, Scott (No changes are permitted for them)        
 Declare @Id1 varchar(100);        
 Declare @Id2 varchar(100);        
 Declare @Id3 varchar(100);        
 Declare @Id4 varchar(100);        
      
 SET @Id1 =(SELECT Id from dbo.[User] where UserName='Chris.Green@stai09.onmicrosoft.com');        
 SET @Id2 =(SELECT Id from dbo.[User] where  UserName='JohnSmith@stai09.onmicrosoft.com');        
 SET @Id3 =(SELECT Id from dbo.[User] where  UserName='Dave@stai09.onmicrosoft.com');        
 SET @Id4 =(SELECT Id from dbo.[User] where  UserName='Scott@stai09.onmicrosoft.com');        
       
 --Delete all roles for the User          
 DELETE FROM UserRole WHERE UserId = @UserId        
 --AND UserId NOT IN (@Id1,@Id2,@Id3,@Id4)    --comment these for testing purpose  
        
 --if(NOT((@UserId =@Id1) OR(@UserId = @Id2) or (@UserId = @Id3)or (@UserId = @Id4)))      
   BEGIN       
          
    --Insert all the roles  for a given user          
    INSERT INTO UserRole                   
    (UserId,CreatedDateTime,CreatedBy,UpdatedDateTime,UpdatedBy,IsDeleted,IsActive,IsAuthorized,OwnerId,SysData,          
    TenantId,SubTenantId,RoleId)           
          
    SELECT DISTINCT @UserId,GETUTCDATE(),@CreatedBy,GETUTCDATE(),@CreatedBy,0,1,@IsAuthorized,@OwnerId,@SysData,          
    @TenantId,@SubTenantId,  a.* from(SELECT *  FROM STRING_SPLIT(@RoleIds, ',')          
       Union all           
      SELECT Id from Role where RoleValue='User')          
     a          
   END          
          
END        
GO
/****** Object:  StoredProcedure [dbo].[sp_User_DeleteRole]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_User_DeleteRole]      
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
  
   Delete From UserRole where UserId =@UserId And RoleId = @RoleId    
   
END    
GO
/****** Object:  StoredProcedure [dbo].[sp_User_PermissionDenied_Save]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE     PROCEDURE [dbo].[sp_User_PermissionDenied_Save]         
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
      
 --Delete all permissions denied for the User      
 DELETE FROM PermissionDenied WHERE UserId = @UserId;      
      
 --Check for Chris Green, John Smith, Dave, Scott(No changes are permitted for them)      
 Declare @Id1 varchar(100);      
 Declare @Id2 varchar(100);      
 Declare @Id3 varchar(100);      
 Declare @Id4 varchar(100);      
    
 SET @Id1 =(SELECT Id from dbo.[User] where UserName='Chris.Green@stai09.onmicrosoft.com');      
 SET @Id2 =(SELECT Id from dbo.[User] where  UserName='JohnSmith@stai09.onmicrosoft.com');      
 SET @Id3 =(SELECT Id from dbo.[User] where  UserName='Dave@stai09.onmicrosoft.com');      
 SET @Id4 =(SELECT Id from dbo.[User] where  UserName='Scott@stai09.onmicrosoft.com');      
      
 if(NOT((@UserId =@Id1) OR(@UserId = @Id2) or (@UserId = @Id3)or (@UserId = @Id4)))      
  BEGIN      
   --Insert all the permissions denied for a given user      
   INSERT INTO PermissionDenied               
   (UserId,CreatedDateTime,CreatedBy,UpdatedDateTime,UpdatedBy,IsDeleted,IsActive,IsAuthorized,OwnerId,SysData,      
   TenantId,SubTenantId,PermissionId)       
      
   select DISTINCT @UserId,GETUTCDATE(),  @CreatedBy,GETUTCDATE(),@CreatedBy,0,1,@IsAuthorized,@OwnerId,@SysData,      
    @TenantId,@SubTenantId,a.* from(SELECT *  FROM STRING_SPLIT(@PermissionIds, ',')) a      
      
  END      
                     
END      
GO
/****** Object:  StoredProcedure [dbo].[sp_User_PermissionGranted_Save]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  
CREATE    PROCEDURE [dbo].[sp_User_PermissionGranted_Save]         
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
      
  --Delete all permissions granted for the User      
  DELETE FROM PermissionGranted WHERE UserId = @UserId;      
      
  --Check for Chris Green, John Smith, Dave, Scott (No changes are permitted for them)      
 Declare @Id1 varchar(100);      
 Declare @Id2 varchar(100);      
 Declare @Id3 varchar(100);      
 Declare @Id4 varchar(100);      
    
 SET @Id1 =(SELECT Id from dbo.[User] where UserName='Chris.Green@stai09.onmicrosoft.com');      
 SET @Id2 =(SELECT Id from dbo.[User] where  UserName='JohnSmith@stai09.onmicrosoft.com');      
 SET @Id3 =(SELECT Id from dbo.[User] where  UserName='Dave@stai09.onmicrosoft.com');      
 SET @Id4 =(SELECT Id from dbo.[User] where  UserName='Scott@stai09.onmicrosoft.com');      
      
 if(NOT((@UserId =@Id1) OR(@UserId = @Id2) or (@UserId = @Id3)or (@UserId = @Id4)))    
    BEGIN      
     -- Insert all permissions granted for a user      
     INSERT INTO PermissionGranted               
     (UserId,CreatedDateTime,CreatedBy,UpdatedDateTime,UpdatedBy,IsDeleted,IsActive,IsAuthorized,OwnerId,SysData,      
     TenantId,SubTenantId,PermissionId)       
      
     select DISTINCT @UserId,GETUTCDATE(),@CreatedBy,GETUTCDATE(),@CreatedBy,0,1,@IsAuthorized,@OwnerId,@SysData,      
   @TenantId,@SubTenantId,a.* from (SELECT * FROM STRING_SPLIT(@PermissionIds, ',')) a        
   END                   
END      
GO
/****** Object:  StoredProcedure [dbo].[sp_User_Save]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_User_Save]              

(    

@Id varchar(100) OUTPUT,              

@UserName varchar(200),  

@PasswordHash varchar(200),        

@auth_type varchar(200), 

@EmpId varchar (6),      

@EmpType varchar (200),      

@mobile varchar (20),            

@Email varchar (200),            

@Position varchar (200),            

@BusinessUnit varchar (200),            

@oid varchar (100),            

@given_name varchar (200),            

@family_name varchar (200),            

@preferred_username varchar (200),            

@FirstName varchar (200),            

@LastName varchar (200),            

@SecondaryEmail varchar (200),            

@PhoneNumber varchar (20),            

@Extension varchar (20),            

@display_name varchar (200),            

@ManagerId varchar (100),      

@Designation varchar(200),      

@Department varchar(200),      

@Location varchar(200),      

@Organization varchar(200),              

@AccessLevel varchar (50),                   

@CreatedBy varchar(100),            

@IsAuthorized bit,            

@OwnerId varchar(100),           

@TenantId varchar(100),            

@SubTenantId varchar(100),            

@SysData nvarchar(max))              

AS              

BEGIN           

Declare @newId uniqueIdentifier =NEWID();          

SET @Id = (Select Cast (@newId as varchar(100)));       

          

 INSERT INTO [dbo].[User] (Id,UserName,auth_type, EmpId, EmpType, mobile,Email,Position,BusinessUnit,oid,given_name,family_name,preferred_username,            

 FirstName,LastName,SecondaryEmail,PhoneNumber,Extension,display_name,ManagerId, Designation, Department, [Location], Organization, CreatedDateTime,UpdatedDateTime,IsDeleted,            

 IsActive,IsAuthorized,OwnerId,        

 --AccessLevel,          

 CreatedBy,UpdatedBy, SysData,TenantId,SubTenantId)             

            

 VALUES(@Id,@UserName,@auth_type, @EmpId, @EmpType, @mobile,@Email,@Position,@BusinessUnit,@oid,@given_name,@family_name,@preferred_username,            

 @FirstName,@LastName,@SecondaryEmail,@PhoneNumber,@Extension,@display_name,@ManagerId,@Designation,@Department,@Location, @Organization,            

 GETDATE(),GETDATE(),0,1,1,@OwnerId,           

 --@AccessLevel,          

 @CreatedBy,@CreatedBy,@SysData,@TenantId,@SubTenantId);             

          

If (@AccessLevel <> ''   AND @AccessLevel is not null)          

BEGIN         

 Update [dbo].[User]            

 Set AccessLevel = @AccessLevel            

 where UserName = @UserName           

END          

          

          

          

 -- Assign 'User' as role             

 Insert Into UserRole(UserId,RoleId)             

 select @Id, Id from Role where RoleName='User'            

   --Add PasswordHash  

   Insert into UserPasswordHash(UserId, PasswordHash, UpdatedBy, UpdatedDateTime) values  

   (@Id, @PasswordHash, @CreatedBy, GetUtcDate())

 SELECT @Id                

            

END
GO
/****** Object:  StoredProcedure [dbo].[sp_User_UpdateAccessLevel]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_User_UpdateAccessLevel]      
 @UserId varchar(100),      
 @AccessLevel varchar(50),      
 @UpdatedBy varchar(100)      
AS      
BEGIN       
  --Update User's Access Level      
  UPDATE [User] SET AccessLevel = @AccessLevel,      
  UpdatedBy = @UpdatedBy      
  WHERE Id = @UserId      
      
END      
GO
/****** Object:  StoredProcedure [dbo].[sp_User_UpdateUserDetail]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create   PROCEDURE [dbo].[sp_User_UpdateUserDetail]    
 @Id varchar(100),    
 @PhoneNumber varchar(20),   
 @Extension varchar(20),   
 @Email varchar(200),   
 @UserRoleId varchar(100),   
 @AccessLevel varchar (50),  
 @UpdatedBy varchar (100),  
 @UpdatedDateTime datetime  
AS    
BEGIN     
 --Update User's Access Level    
 UPDATE [User] SET  PhoneNumber = @PhoneNumber,  
 Extension = @Extension,  
 Email = @Email,  
 AccessLevel = @AccessLevel,  
 UpdatedBy = @UpdatedBy,  
 UpdatedDateTime = @UpdatedDateTime  
 WHERE Id = @Id  ;  
   
 --Update User's RoleId in UserRole  
 Update [UserRole] SET RoleId = @UserRoleId,  
 UpdatedBy = @UpdatedBy,  
 UpdatedDateTime = @UpdatedDateTime  
 Where UserId = @Id;  
END  
GO
/****** Object:  StoredProcedure [dbo].[sp_UserAccessLevel_Select]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create   PROCEDURE [dbo].[sp_UserAccessLevel_Select]    
AS    
BEGIN    
    
 Select  AccessLevel, DisplayName FROM  UserAccessLevel    
    
END  
GO
/****** Object:  StoredProcedure [dbo].[sp_UserInfo_Select]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 CREATE       PROCEDURE [dbo].[sp_UserInfo_Select]                                   
@UserId varchar(100)                                                          
AS                                                          
BEGIN                            
Declare @UserId1 varchar(100) = (Select Id from [User]  where ( Id = @UserId OR oid = @UserId OR UserName = @UserId))                                        
--User                                            
Select U.Id as userId, U.EmpId, U.EmpType, U.UserName, U.auth_type, U.FirstName, U.LastName, U.Email, U.SecondaryEmail, U.PhoneNumber, U.Extension,                                                          
 U.mobile, U.oid, U.preferred_username, U.display_name, U.given_name, U.family_name, U.Position, U.BusinessUnit,                                                          
 u.ManagerId,M.UserName As ManagerUserName, U.AccessLevel ,U.IsActive, U.Designation ,U.Department ,U.Location ,U.Organization,                          
                           
 --Audit Fields                          
 U.CorrelationId, U.AuditableRequestId, U.AuditableRequestName, U.AuditableSourceEventName, U.CreatedBy, U.CreatedDateTime, U.UpdatedBy,                           
 U.UpdatedDateTime, U.UpdateReason, U.OwnerId, /*U.IsActive,*/ U.IsDeleted, U.IsApproved, U.ApproverId, U.ApprovedDateTime,                           
 U.IsAuthorized, U.AuthorizedById, U.AuthorizedDateTime, U.SysData, U.TenantId, U.SubTenantId --,                           
                          
 --Token                          
 --UToken.RcTokenObject,                           
 --UToken.ModifiedDateTime as RcTokenDateTime, UserPic,UProfile.DefaultProspectListId, UProfile.DefaultConatctListId,                           
 --UProfile.DefaultWBAListId                                 
 from [dbo].[User] as U                                                          
 Left JOIN  [dbo].[User] as M On U.ManagerId=M.Id And (M.IsDeleted Is NUll Or M.IsDeleted =0)                                     
 --Left JOIN  [dbo].[UserRCToken] as UToken On UToken.UserId=U.Id                                  
 --Left Join [DBO].[USERPROFILE] as UProfile On UProfile.UserId=U.Id                                  
 where (U.IsDeleted Is NUll Or U.IsDeleted =0)         
 and ((@UserId Is Null Or @UserId='0') OR         
  (U.Id=@UserId1)) --OR U.OID=@UserId)                                                 
 ORDER BY U.display_name ASC                                                
                                                   
--User Role                                            
 Select UR.Id As UserRoleId,R.Id As RoleId,R.RoleName,R.RoleValue,R.DisplayName,U.UserName,U.Id as UserId                                                       
 from UserRole UR                                                           
 JOIN Role R ON   (R.IsDeleted Is NUll Or R.IsDeleted =0) And Ur.RoleId=R.Id                                                          
 JOIN [dbo].[User] as U On (U.IsDeleted Is NUll Or U.IsDeleted =0) and Ur.UserId=U.Id                                                          
 Where (UR.IsDeleted Is NUll Or UR.IsDeleted =0) And          
 ((@UserId Is Null Or @UserId='0') OR         
 (UR.UserId=@UserId1))-- OR U.OID=@UserId)                                                          
 ORDER BY R.RoleName ASC                                                   
                                                 
-- User Permissions Granted                                            
 Select P.Id As PermissionId,PG.Id As PermissionGrantedId,P.PermissionValue,                                            
 P.PermissionDisplayName,U.UserName,U.Id  as UserId,                
 P.PermissionSetId, ar.DisplayName as PermissionSetName,                
 p.ModuleId, a.DisplayName as ModuleName                  
 -- ,pgr.Id as PermissionGroupId,                                            
 --pgr.PermissionGroupDisplayName,pgr.Module,ps.Id as PermissionSetId            
 from PermissionGranted PG      
 JOIN  Permission P On P.Id=PG.PermissionId And (P.IsDeleted Is NUll Or P.IsDeleted =0)                                                          
 JOIN [dbo].[User] as U On (U.IsDeleted Is NUll Or U.IsDeleted =0) and PG.UserId=U.Id                    
 INNER JOIN AuthReferenceLookup as a ON  a.Id = P.ModuleId                     
 INNER JOIN AuthReferenceLookup as ar ON  ar.Id = P.PermissionSetId                     
                
-- INNER JOIN PermissionSet as ps ON  ps.Id = P.PermissionSetId                   
 --INNER JOIN PermissionGroup as pgr ON pgr.Id = ps.PermissionGroupId AND (pgr.IsDeleted Is NUll Or pgr.IsDeleted =0)                                             
 Where (PG.IsDeleted Is NUll Or PG.IsDeleted =0) And          
 ((@UserId Is Null Or @UserId='0') OR         
 PG.UserId=@UserId1  )      
 --OR                  
 --U.OID=@UserId)                                                          
 ORDER BY P.PermissionDisplayName ASC                              
                                               
-- User Permissions Denied                                               
 Select  P.Id As PermissionId,PD.Id As PermissionDeniedId,P.PermissionValue,P.PermissionDisplayName,                                            
 U.UserName,U.Id as UserId ,                  
 P.PermissionSetId, ar.DisplayName as PermissionSetName,                
 p.ModuleId, a.DisplayName as ModuleName--,pgr.Id as PermissionGroupId,                       
 --pgr.PermissionGroupDisplayName,pgr.Module,ps.Id as PermissionSetId                                                         
 from PermissionDenied PD                                   
 JOIN  Permission P On P.Id=PD.PermissionId And (P.IsDeleted Is NUll Or P.IsDeleted =0)                                                          
 JOIN [dbo].[User] as U On (U.IsDeleted Is NUll Or U.IsDeleted =0) and PD.UserId=U.Id                    
  INNER JOIN AuthReferenceLookup as a ON  a.Id = P.ModuleId                  
 INNER JOIN AuthReferenceLookup as ar ON  ar.Id = P.PermissionSetId                     
                  
  --INNER JOIN PermissionSet as ps ON  ps.Id = P.PermissionSetId                                            
 --INNER JOIN PermissionGroup as pgr ON pgr.Id = ps.PermissionGroupId AND (pgr.IsDeleted Is NUll Or pgr.IsDeleted =0)                                             
 Where (PD.IsDeleted Is NUll Or PD.IsDeleted =0) And          
 ((@UserId Is Null Or @UserId='0') OR         
 PD.UserId=@UserId1  )       
 --OR                   
-- U.OID=@UserId)                                                         
 ORDER BY P.PermissionDisplayName ASC                   
               
              
              
              
 --************************************************************************************************************              
 --********************************  User UI Permission  ******************************************************              
 --************************************************************************************************************              
 Declare @RoleIdList nvarchar(max)              
              
-- This query concatenates multiple role IDs into a comma-separated string              
Select @RoleIdList = STRING_AGG(CAST(RoleId as NVARCHAR(max)), ',')               
From UserRole               
Where (UserId = @UserId1 OR (@UserId Is Null Or @UserId='0'))              
              
-- If there are no roles for the given user, set the variable to an empty string              
If @RoleIdList Is Null              
    Set @RoleIdList = ''              
              
              
              
              
              
 Select distinct a.UserId,a.UIPermissionId as PermissionId,               
a.PermissionValue,a.PermissionDisplayName,                            
a.PermissionTypeId,               
 a.PermissionTypeName, a.PermissionParentId, a.PermissionParentName,                  
 a.ModuleId ,              
 a.ModuleName,                
               
    MAX(CASE WHEN a.CanCreate = 1 THEN 1 ELSE 0 END) AS CanCreate,              
    MAX(CASE WHEN a.CanEdit = 1 THEN 1 ELSE 0 END) AS CanEdit,              
    MAX(CASE WHEN a.CanDelete = 1 THEN 1 ELSE 0 END) AS CanDelete,              
 MAX(CASE WHEN a.CanView = 1 THEN 1 ELSE 0 END) AS CanView,              
 MAX(CASE WHEN a.IsUiPermissionDenied = 1 THEN 1 ELSE 0 END) AS IsUiPermissionDenied,              
    MAX(CASE WHEN a.IsUiPermissionGranted = 1 THEN 1 ELSE 0 END) AS IsUiPermissionGranted FROM              
(               
   --1. UI PERMISSION DENIED                       
   Select pd.UIPermissionId As UIPermissionId,pd.UserId As UserId,                                  
   P.PermissionValue,P.PermissionDisplayName,                          
   P.UiPermissionTypeId as PermissionTypeId ,a.DisplayName as PermissionTypeName, P.PermissionParentId,              
   UP.PermissionDisplayName as PermissionParentName,P.ModuleId, ar.DisplayName as ModuleName,                  
   pd.CanCreate,pd.CanEdit,pd.CanDelete, pd.CanView ,1 as IsUiPermissionDenied , 0 as IsUiPermissionGranted                                 
   from [dbo].[UIPermissionDenied] as pd                                        
   INNER JOIN  UIPermission P On pd.UIPermissionId = P.Id  and P.IsActive = 1                           
   INNER JOIN  AuthReferenceLookup a On a.Id = P.UiPermissionTypeId                            
   INNER JOIN  AuthReferenceLookup aR On aR.Id = P.ModuleId                   
   LEFT JOIN  UIPermission UP On UP.Id = P.PermissionParentId                            
 --  INNER JOIN [dbo].[User] as U On  pd.UserId= U.Id               
  where --pd.UserId = @UserId              
   (pd.UserId = @UserId1 OR (@UserId Is Null Or @UserId='0'))              
                
               
  Union All                  
                                      
   --2.  UI PERMISSION GRANTED                        
   Select pgr.UIPermissionId As UIPermissionId,pgr.UserId As UserId,                                  
   P.PermissionValue,P.PermissionDisplayName,                          
   P.UiPermissionTypeId as PermissionTypeId ,a.DisplayName as PermissionTypeName,                  
   P.ModuleId, ar.DisplayName as ModuleName, P.PermissionParentId, up.PermissionDisplayName as PermissionParentName,                  
   pgr.CanCreate,pgr.CanEdit,pgr.CanDelete, pgr.CanView  ,0 as IsUiPermissionDenied, 1 as IsUiPermissionGranted                                        
   from [dbo].[UIPermissionGranted] as pgr                                        
   INNER JOIN  UIPermission P On pgr.UIPermissionId = P.Id  and P.IsActive = 1                    
   INNER JOIN  AuthReferenceLookup a On a.Id = P.UiPermissionTypeId                      
   INNER JOIN  AuthReferenceLookup ar On ar.Id = P.ModuleId                         
   Left JOIN  UiPermission up On up.Id = P.PermissionParentId                        
   --INNER JOIN [dbo].[User] as U On  pgr.UserId= U.Id                
   where --pgr.UserId = @UserId              
    (pgr.UserId = @UserId1 OR (@UserId Is Null Or @UserId='0'))              
               
   Union All                                   
   -- 3. UI PERMISSIONS OF ROLE                           
   Select RP.UIPermissionId As UIPermissionId,u.UserId,                                
   P.PermissionValue,P.PermissionDisplayName,                            
   P.UiPermissionTypeId as PermissionTypeId, a.DisplayName as PermissionTypeName, P.PermissionParentId, UP.PermissionDisplayName as PermissionParentName,                  
   P.ModuleId ,ar.DisplayName as ModuleName,                
   MAX(CASE WHEN RP.CanCreate = 1 THEN 1 ELSE 0 END) AS CanCreate,              
   MAX(CASE WHEN RP.CanEdit = 1 THEN 1 ELSE 0 END) AS CanEdit,              
   MAX(CASE WHEN RP.CanDelete = 1 THEN 1 ELSE 0 END) AS CanDelete,              
   MAX(CASE WHEN RP.CanView = 1 THEN 1 ELSE 0 END) AS CanView,              
   0 as IsUiPermissionDenied, 0 as IsUiPermissionGranted                                               
   from [dbo].[RoleUIPermission] as RP                                      
   INNER JOIN  UIPermission P On RP.UIPermissionId = P.Id  and P.IsActive =1                    
   INNER JOIN  AuthReferenceLookup a On a.Id = P.UiPermissionTypeId                  
   INNER JOIN  AuthReferenceLookup ar On ar.Id = P.ModuleId                  
   LEFT JOIN  UIPermission UP On UP.Id = P.PermissionParentId                 
   INNER JOIN [dbo].[UserRole] as u On  u.RoleId= RP.RoleId              
   where RP.RoleId in ( SELECT *  FROM STRING_SPLIT(@RoleIdList, ',' ) )              
   GROUP BY              
       u.UserId,              
    RP.UIPermissionId,              
    P.PermissionValue,              
    P.PermissionDisplayName,              
    P.UiPermissionTypeId,              
    a.DisplayName,              
    P.PermissionParentId,              
    UP.PermissionDisplayName,              
    P.ModuleId,              
    ar.DisplayName              
              
 ) as a              
 Group by                 
    a.UserId,              
    a.UIPermissionId,              
    a.PermissionValue,              
    a.PermissionDisplayName,              
    a.PermissionTypeId,              
    a.PermissionTypeName,              
    a.PermissionParentId,              
    a.PermissionParentName,              
    a.ModuleId,              
    a.ModuleName;              
                                                   
END 
GO
/****** Object:  StoredProcedure [dbo].[sp_UserInfo_Select_gunjan]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UserInfo_Select_gunjan] -- '0'
    @UserId varchar(100)
AS
BEGIN
    -- User Information
    SELECT 
        U.Id AS userId, U.EmpId, U.EmpType, U.UserName, U.FirstName, U.LastName, 
        U.Email, U.SecondaryEmail, U.PhoneNumber, U.Extension, U.mobile, U.oid, 
        U.preferred_username, U.display_name, U.given_name, U.family_name, 
        U.Position, U.BusinessUnit, U.ManagerId, M.UserName AS ManagerUserName, 
        U.AccessLevel, U.IsActive, U.Designation, U.Department, U.Location, 
        U.Organization, U.CorrelationId, U.AuditableRequestId, U.AuditableRequestName, 
        U.AuditableSourceEventName, U.CreatedBy, U.CreatedDateTime, U.UpdatedBy, 
        U.UpdatedDateTime, U.UpdateReason, U.OwnerId, U.IsDeleted, U.IsApproved, 
        U.ApproverId, U.ApprovedDateTime, U.IsAuthorized, U.AuthorizedById, 
        U.AuthorizedDateTime, U.SysData, U.TenantId, U.SubTenantId
    FROM 
        [dbo].[User] AS U
    LEFT JOIN 
        [dbo].[User] AS M ON U.ManagerId = M.Id AND (M.IsDeleted IS NULL OR M.IsDeleted = 0)
    WHERE 
        (U.IsDeleted IS NULL OR U.IsDeleted = 0) 
        AND ((@UserId IS NULL OR @UserId = '0') OR U.Id = @UserId OR U.OID = @UserId)
    ORDER BY 
        U.display_name ASC;

    -- User Role
    SELECT 
        UR.Id AS UserRoleId, R.Id AS RoleId, R.RoleName, R.RoleValue, 
        R.DisplayName, U.UserName, U.Id AS UserId
    FROM 
        UserRole UR
    JOIN 
        Role R ON (R.IsDeleted IS NULL OR R.IsDeleted = 0) AND UR.RoleId = R.Id
    JOIN 
        [dbo].[User] AS U ON (U.IsDeleted IS NULL OR U.IsDeleted = 0) AND UR.UserId = U.Id
    WHERE 
        (UR.IsDeleted IS NULL OR UR.IsDeleted = 0) 
        AND ((@UserId IS NULL OR @UserId = '0') OR UR.UserId = @UserId OR U.OID = @UserId)
    ORDER BY 
        R.RoleName ASC;

    -- User Permissions Granted
    SELECT 
        P.Id AS PermissionId, PG.Id AS PermissionGrantedId, P.PermissionValue, 
        P.PermissionDisplayName, U.UserName, U.Id AS UserId, P.PermissionSetId, 
        ar.DisplayName AS PermissionSetName, p.ModuleId, a.DisplayName AS ModuleName
    FROM 
        PermissionGranted PG
    JOIN 
        Permission P ON P.Id = PG.PermissionId AND (P.IsDeleted IS NULL OR P.IsDeleted = 0)
    JOIN 
        [dbo].[User] AS U ON (U.IsDeleted IS NULL OR U.IsDeleted = 0) AND PG.UserId = U.Id
    INNER JOIN 
        AuthReferenceLookup AS a ON a.Id = P.ModuleId
    INNER JOIN 
        AuthReferenceLookup AS ar ON ar.Id = P.PermissionSetId
    WHERE 
        (PG.IsDeleted IS NULL OR PG.IsDeleted = 0) 
        AND ((@UserId IS NULL OR @UserId = '0') OR PG.UserId = @UserId OR U.OID = @UserId)
    ORDER BY 
        P.PermissionDisplayName ASC;

    -- User Permissions Denied
    SELECT 
        P.Id AS PermissionId, PD.Id AS PermissionDeniedId, P.PermissionValue, 
        P.PermissionDisplayName, U.UserName, U.Id AS UserId, P.PermissionSetId, 
        ar.DisplayName AS PermissionSetName, p.ModuleId, a.DisplayName AS ModuleName
    FROM 
        PermissionDenied PD
    JOIN 
        Permission P ON P.Id = PD.PermissionId AND (P.IsDeleted IS NULL OR P.IsDeleted = 0)
    JOIN 
        [dbo].[User] AS U ON (U.IsDeleted IS NULL OR U.IsDeleted = 0) AND PD.UserId = U.Id
    INNER JOIN 
        AuthReferenceLookup AS a ON a.Id = P.ModuleId
    INNER JOIN 
        AuthReferenceLookup AS ar ON ar.Id = P.PermissionSetId
    WHERE 
        (PD.IsDeleted IS NULL OR PD.IsDeleted = 0) 
        AND ((@UserId IS NULL OR @UserId = '0') OR PD.UserId = @UserId OR U.OID = @UserId)
    ORDER BY 
        P.PermissionDisplayName ASC;

    -- User UI Permission
    DECLARE @RoleIdList NVARCHAR(MAX);

    -- This query concatenates multiple role IDs into a comma-separated string
    SELECT 
        @RoleIdList = STRING_AGG(CAST(RoleId AS NVARCHAR(MAX)), ',')
    FROM 
        UserRole
    WHERE 
        (UserId = @UserId OR (@UserId IS NULL OR @UserId = '0'));

    -- If there are no roles for the given user, set the variable to an empty string
    IF @RoleIdList IS NULL
        SET @RoleIdList = '';

  -- User UI Permissions
SELECT DISTINCT
    a.UserId, a.UIPermissionId AS PermissionId, a.PermissionValue, 
    a.PermissionDisplayName, a.PermissionTypeId, a.PermissionTypeName, 
    a.PermissionParentId, a.PermissionParentName, a.ModuleId, a.ModuleName, 
    MAX(CASE WHEN a.CanCreate = 1 THEN 1 ELSE 0 END) AS CanCreate,
    MAX(CASE WHEN a.CanEdit = 1 THEN 1 ELSE 0 END) AS CanEdit,
    MAX(CASE WHEN a.CanDelete = 1 THEN 1 ELSE 0 END) AS CanDelete,
    MAX(CASE WHEN a.CanView = 1 THEN 1 ELSE 0 END) AS CanView,
    MAX(CASE WHEN a.IsUiPermissionDenied = 1 THEN 1 ELSE 0 END) AS IsUiPermissionDenied,
    MAX(CASE WHEN a.IsUiPermissionGranted = 1 THEN 1 ELSE 0 END) AS IsUiPermissionGranted
FROM
(
    -- UI PERMISSION DENIED
    SELECT 
        pd.UIPermissionId, pd.UserId, P.PermissionValue, 
        P.PermissionDisplayName, P.UiPermissionTypeId AS PermissionTypeId, 
        a.DisplayName AS PermissionTypeName, P.PermissionParentId, 
        UP.PermissionDisplayName AS PermissionParentName, P.ModuleId, 
        ar.DisplayName AS ModuleName, pd.CanCreate, pd.CanEdit, pd.CanDelete, 
        pd.CanView, 1 AS IsUiPermissionDenied, 0 AS IsUiPermissionGranted
    FROM 
        [dbo].[UIPermissionDenied] AS pd
    INNER JOIN 
        UIPermission P ON pd.UIPermissionId = P.Id AND P.IsActive = 1
    INNER JOIN 
        AuthReferenceLookup a ON a.Id = P.UiPermissionTypeId
    INNER JOIN 
        AuthReferenceLookup ar ON ar.Id = P.ModuleId
    LEFT JOIN 
        UIPermission UP ON UP.Id = P.PermissionParentId
    WHERE 
        (pd.UserId = @UserId OR (@UserId IS NULL OR @UserId = '0')) -- Apply filter

    UNION ALL

    -- UI PERMISSION GRANTED
    SELECT 
        pgr.UIPermissionId, pgr.UserId, P.PermissionValue, 
        P.PermissionDisplayName, P.UiPermissionTypeId AS PermissionTypeId, 
        a.DisplayName AS PermissionTypeName, P.ModuleId, 
        ar.DisplayName AS ModuleName, P.PermissionParentId, 
        up.PermissionDisplayName AS PermissionParentName, pgr.CanCreate, 
        pgr.CanEdit, pgr.CanDelete, pgr.CanView, 0 AS IsUiPermissionDenied, 
        1 AS IsUiPermissionGranted
    FROM 
        [dbo].[UIPermissionGranted] AS pgr
    INNER JOIN 
        UIPermission P ON pgr.UIPermissionId = P.Id AND P.IsActive = 1
    INNER JOIN 
        AuthReferenceLookup a ON a.Id = P.UiPermissionTypeId
    INNER JOIN 
        AuthReferenceLookup ar ON ar.Id = P.ModuleId
    LEFT JOIN 
        UiPermission up ON up.Id = P.PermissionParentId
    WHERE 
        (pgr.UserId = @UserId OR (@UserId IS NULL OR @UserId = '0')) -- Apply filter

    UNION ALL

    -- UI PERMISSIONS OF ROLE
    SELECT 
        RP.UIPermissionId, u.UserId, P.PermissionValue, 
        P.PermissionDisplayName, P.UiPermissionTypeId AS PermissionTypeId, 
        a.DisplayName AS PermissionTypeName, P.PermissionParentId, 
        UP.PermissionDisplayName AS PermissionParentName, P.ModuleId, 
        ar.DisplayName AS ModuleName, 
        MAX(CASE WHEN RP.CanCreate = 1 THEN 1 ELSE 0 END) AS CanCreate,
        MAX(CASE WHEN RP.CanEdit = 1 THEN 1 ELSE 0 END) AS CanEdit,
        MAX(CASE WHEN RP.CanDelete = 1 THEN 1 ELSE 0 END) AS CanDelete,
        MAX(CASE WHEN RP.CanView = 1 THEN 1 ELSE 0 END) AS CanView,
        0 AS IsUiPermissionDenied, 0 AS IsUiPermissionGranted                                     
    FROM 
        [dbo].[RoleUIPermission] AS RP
    INNER JOIN 
        UIPermission P ON RP.UIPermissionId = P.Id AND P.IsActive = 1
    INNER JOIN 
        AuthReferenceLookup a ON a.Id = P.UiPermissionTypeId
    INNER JOIN 
        AuthReferenceLookup ar ON ar.Id = P.ModuleId
    LEFT JOIN 
        UIPermission UP ON UP.Id = P.PermissionParentId
    INNER JOIN 
        [dbo].[UserRole] AS u ON u.RoleId = RP.RoleId
    WHERE 
        RP.RoleId IN (SELECT value FROM STRING_SPLIT(@RoleIdList, ',')) 
        AND (u.UserId = @UserId OR (@UserId IS NULL OR @UserId = '0')) -- Apply filter
    GROUP BY 
        u.UserId, RP.UIPermissionId, P.PermissionValue, 
        P.PermissionDisplayName, P.UiPermissionTypeId, 
        a.DisplayName, P.PermissionParentId, UP.PermissionDisplayName, 
        P.ModuleId, ar.DisplayName
) AS a
GROUP BY 
    a.UserId, a.UIPermissionId, a.PermissionValue, 
    a.PermissionDisplayName, a.PermissionTypeId, 
    a.PermissionTypeName, a.PermissionParentId, 
    a.PermissionParentName, a.ModuleId, a.ModuleName;

  End
GO
/****** Object:  StoredProcedure [dbo].[sp_UserProfile_Get_ByUserId]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_UserProfile_Get_ByUserId]   -- 6825                                   
(                                            
  @UserId varchar(100)                    
                             
)                                            
AS                                            
 BEGIN                     
                    
  SELECT [UserId], /*UserPic, -- important*/ DOB, Gender, BloodGroup, PersonalEmail, DateOfJoining, PassportNumber, FatherName,  
  MotherName, MaritalStatus, WeddingAnniversaryDate, SpouseName, SpouseDOB, HomeAddress1, HomeAddress2,  
  City, [State], HomeAddressCity, HomeAddressState, HomeAddressCountry, HomePhoneNumber, EmergencyContactNumber, EmergencyContactName, PrimarySkills, SecondarySkills, TertiarySkills, OtherSkills, [Branch],[LookUpCode],[LinkedInUrl], display_name FROM     
  [User] as U    
  inner join [DBO].[USERPROFILE] as Up    
  on up.UserId = u.Id    
  WHERE [UserId]= @UserId              
                    
end    
GO
/****** Object:  StoredProcedure [dbo].[sp_UserRole_Select]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create   PROCEDURE [dbo].[sp_UserRole_Select] --'0'                     
@RoleId varchar(100)                      
AS                      
BEGIN                      
Select r.Id,r.RoleName,r.RoleValue                  
 from Role as r                          
 where (r.IsDeleted Is NUll Or r.IsDeleted =0) and ((@RoleId Is Null Or @RoleId='0') OR r.Id=@RoleId)        
END  
GO
/****** Object:  StoredProcedure [dbo].[sp_Users_Select]    Script Date: 21-01-2026 14:47:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[sp_Users_Select]          
AS        
BEGIN        
  SELECT u.Id, u.UserName,r1.DisplayName as UserRoleName, u.AccessLevel,     
  u.IsDeleted ,        
  u.oid,u.BusinessUnit,u.mobile,u.email,u.Position,u.given_name,u.family_name,u.preferred_username        
         
  FROM [dbo].[User] u        
   LEFT JOIN UserRole r on r.UserId = u.Id     
   LEFT JOIN Role r1 on r.RoleId = u.Id    
   AND (u.IsDeleted=0 OR u.IsDeleted IS NULL)        
   ORDER BY  u.UserName        
END    
GO
