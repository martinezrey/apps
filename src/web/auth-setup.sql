use AppFramework
go

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'Auth')
BEGIN
    -- The schema must be run in its own batch!
    EXEC( 'CREATE SCHEMA Auth' );
END

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'Config')
BEGIN
    -- The schema must be run in its own batch!
    EXEC( 'CREATE SCHEMA Config' );
END

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'Logging')
BEGIN
    -- The schema must be run in its own batch!
    EXEC( 'CREATE SCHEMA Logging' );
END

if object_id('Auth.[OrgAppUserMetadata]', 'U') is not null
	drop table OrgAppUserMetadata
if object_id('Auth.[UserRefreshToken]', 'U') is not null
	drop table UserRefreshToken
if object_id('Auth.OrgGlobalUserRole', 'U') is not null
	drop table [auth].[OrgGlobalUserRole]
if object_id('Auth.OrgGlobalRole', 'U') is not null
	drop table [auth].[OrgGlobalRole]
if object_id('Auth.OrgUserRole', 'U') is not null
	drop table [auth].[OrgUserRole]
if object_id('Auth.OrgUser', 'U') is not null
	drop table [auth].[OrgUser]
if object_id('Auth.OrgRole', 'U') is not null
	drop table [auth].[OrgRole]
if object_id('Auth.OrgAppUserAuthIp', 'U') is not null
	drop table [auth].[OrgAppUserAuthIp]
if object_id('Auth.OrgAppUserRole', 'U') is not null
	drop table [auth].[OrgAppUserRole]
if object_id('Auth.OrgAppUser', 'U') is not null
	drop table [auth].[OrgAppUser]
if object_id('Auth.OrgAppRole', 'U') is not null
	drop table [auth].[OrgAppRole]
if object_id('Auth.OrgApp', 'U') is not null
	drop table [auth].[OrgApp]
if object_id('Auth.Org', 'U') is not null
	drop table [auth].[Org]
if object_id('Auth.User', 'U') is not null
	drop table [auth].[User]
if object_id('Auth.[Logs]', 'U') is not null
	drop table logs


create table [auth].[User]
(
	Id int not null identity(1,1)
	,UserName varchar(250) not null constraint uqc_User_UserName unique
	,Email varchar(250)
	,PhoneNumber varchar(250) null
	,PasswordHash varchar(max) not null
	,SecurityStamp varchar(max) not null
	,IsLockoutEnabled bit  not null
	,IsTwoFactorEnabled bit not null
	,AccessFailedCount int not null
	,LockoutEndDateUtc datetime null
	,Claims varchar(max) null
	,Logins varchar(max) null
	,FirstName varchar(250) null
	,LastName varchar(250) null
	,Suffix varchar(250) null
	,IsActive bit not null
	,IsDeleted bit not null
	,created_by varchar(50) not null
	,created_dt datetime not null
	,modified_by varchar(50) not null
	,modified_dt datetime not null

	constraint pk_User_Id primary key (Id asc)	
)

alter table [auth].[User] add constraint [df_User_IsLockoutEnabled] default (0) for [IsLockoutEnabled]
alter table [auth].[User] add constraint [df_User_IsTwoFactorEnabled] default (0) for [IsTwoFactorEnabled]
alter table [auth].[User] add constraint [df_User_AccessFailedCount] default (0) for [AccessFailedCount]
alter table [auth].[User] add constraint [df_User_IsActive] default (0) for [IsActive]
alter table [auth].[User] add constraint [df_User_IsDeleted] default (0) for [IsDeleted]
alter table [auth].[User] add constraint [df_User_created_by] default (user_name()) for [created_by]
alter table [auth].[User] add constraint [df_User_created_dt] default (getdate()) for [created_dt]
alter table [auth].[User] add constraint [df_User_modified_by] default (user_name()) for [modified_by]
alter table [auth].[User] add constraint [df_User_modified_dt] default (getdate()) for [modified_dt]

create table [auth].Org
(
	Id int not null identity(1,1)
	,OrgName varchar(250) not null constraint uqc_Org_OrgName unique
	,IsActive bit not null
	,IsDeleted bit not null
	,created_by varchar(50) not null
	,created_dt datetime not null
	,modified_by varchar(50) not null
	,modified_dt datetime not null

	constraint pk_Org_Id primary key (Id asc)
)
alter table [auth].[Org] add constraint [df_Org_IsActive] default (0) for [IsActive]
alter table [auth].[Org] add constraint [df_Org_IsDeleted] default (0) for [IsDeleted]
alter table [auth].[Org] add constraint [df_Org_created_by] default (user_name()) for [created_by]
alter table [auth].[Org] add constraint [df_Org_created_dt] default (getdate()) for [created_dt]
alter table [auth].[Org] add constraint [df_Org_modified_by] default (user_name()) for [modified_by]
alter table [auth].[Org] add constraint [df_Org_modified_dt] default (getdate()) for [modified_dt]


/*************************************
* Organization Roles and Users
*************************************/

create table [auth].OrgGlobalRole
(
	Id int not null identity(1,1)
	,OrgId int not null
		constraint fk_OrgGlobalRole_Org_Id references auth.Org(Id)
	,Name varchar(250) not null
	,IsActive bit not null
	,IsDeleted bit not null
	,created_by varchar(50) not null
	,created_dt datetime not null
	,modified_by varchar(50) not null
	,modified_dt datetime not null

	constraint pk_OrgGlobalRole_Id primary key (Id asc)
)

alter table [auth].[OrgGlobalRole] add constraint [df_OrgGlobalRole_IsActive] default (0) for [IsActive]
alter table [auth].[OrgGlobalRole] add constraint [df_OrgGlobalRole_IsDeleted] default (0) for [IsDeleted]
alter table [auth].[OrgGlobalRole] add constraint [df_OrgGlobalRole_created_by] default (user_name()) for [created_by]
alter table [auth].[OrgGlobalRole] add constraint [df_OrgGlobalRole_created_dt] default (getdate()) for [created_dt]
alter table [auth].[OrgGlobalRole] add constraint [df_OrgGlobalRole_modified_by] default (user_name()) for [modified_by]
alter table [auth].[OrgGlobalRole] add constraint [df_OrgGlobalRole_modified_dt] default (getdate()) for [modified_dt]

create table [auth].OrgRole
(
	Id int not null identity(1,1)
	,OrgId int not null
		constraint fk_OrgRole_Org_Id references auth.Org(Id)
	,Name varchar(250) not null
	,IsActive bit not null
	,IsDeleted bit not null
	,created_by varchar(50) not null
	,created_dt datetime not null
	,modified_by varchar(50) not null
	,modified_dt datetime not null

	constraint pk_OrgRole_Id primary key (Id asc)
)

alter table [auth].[OrgRole] add constraint [df_OrgRole_IsActive] default (0) for [IsActive]
alter table [auth].[OrgRole] add constraint [df_OrgRole_IsDeleted] default (0) for [IsDeleted]
alter table [auth].[OrgRole] add constraint [df_OrgRole_created_by] default (user_name()) for [created_by]
alter table [auth].[OrgRole] add constraint [df_OrgRole_created_dt] default (getdate()) for [created_dt]
alter table [auth].[OrgRole] add constraint [df_OrgRole_modified_by] default (user_name()) for [modified_by]
alter table [auth].[OrgRole] add constraint [df_OrgRole_modified_dt] default (getdate()) for [modified_dt]

create table [auth].OrgUser
(
	Id int not null identity(1,1)
	,OrgId int not null
		constraint fk_OrgUser_Org_Id references auth.Org(Id)
	,UserId int not null
		constraint fk_OrgUser_User_Id references auth.[User](Id)
	,IsActive bit not null
	,IsDeleted bit not null
	,created_by varchar(50) not null
	,created_dt datetime not null
	,modified_by varchar(50) not null
	,modified_dt datetime not null

	constraint pk_OrgUser_Id primary key (Id asc)
)

alter table [auth].[OrgUser] add constraint [df_OrgUser_IsActive] default (0) for [IsActive]
alter table [auth].[OrgUser] add constraint [df_OrgUser_IsDeleted] default (0) for [IsDeleted]
alter table [auth].[OrgUser] add constraint [df_OrgUser_created_by] default (user_name()) for [created_by]
alter table [auth].[OrgUser] add constraint [df_OrgUser_created_dt] default (getdate()) for [created_dt]
alter table [auth].[OrgUser] add constraint [df_OrgUser_modified_by] default (user_name()) for [modified_by]
alter table [auth].[OrgUser] add constraint [df_OrgUser_modified_dt] default (getdate()) for [modified_dt]

create table [auth].OrgGlobalUserRole
(
	Id int not null identity(1,1)
	,OrgUserId int not null
		constraint fk_OrgGlobalUserRole_OrgUser_Id references auth.OrgUser(Id)
	,OrgGlobalRoleId int not null
		constraint fk_OrgGlobalUserRole_OrgGlobalRole_Id references auth.OrgGlobalRole(Id)
	,IsActive bit not null
	,IsDeleted bit not null
	,created_by varchar(50) not null
	,created_dt datetime not null
	,modified_by varchar(50) not null
	,modified_dt datetime not null

	constraint pk_OrgGlobalUserRole_Id primary key (Id asc)
)

alter table [auth].[OrgGlobalUserRole] add constraint [df_OrgGlobalUserRole_IsActive] default (0) for [IsActive]
alter table [auth].[OrgGlobalUserRole] add constraint [df_OrgGlobalUserRole_IsDeleted] default (0) for [IsDeleted]
alter table [auth].[OrgGlobalUserRole] add constraint [df_OrgGlobalUserRole_created_by] default (user_name()) for [created_by]
alter table [auth].[OrgGlobalUserRole] add constraint [df_OrgGlobalUserRole_created_dt] default (getdate()) for [created_dt]
alter table [auth].[OrgGlobalUserRole] add constraint [df_OrgGlobalUserRole_modified_by] default (user_name()) for [modified_by]
alter table [auth].[OrgGlobalUserRole] add constraint [df_OrgGlobalUserRole_modified_dt] default (getdate()) for [modified_dt]

create table [auth].OrgUserRole
(
	Id int not null identity(1,1)
	,OrgUserId int not null
		constraint fk_OrgUserRole_OrgUser_Id references auth.OrgUser(Id)
	,OrgRoleId int not null
		constraint fk_OrgUserRole_OrgRole_Id references auth.OrgRole(Id)
	,IsActive bit not null
	,IsDeleted bit not null
	,created_by varchar(50) not null
	,created_dt datetime not null
	,modified_by varchar(50) not null
	,modified_dt datetime not null

	constraint pk_OrgUserRole_Id primary key (Id asc)
)

alter table [auth].[OrgUserRole] add constraint [df_OrgUserRole_IsActive] default (0) for [IsActive]
alter table [auth].[OrgUserRole] add constraint [df_OrgUserRole_IsDeleted] default (0) for [IsDeleted]
alter table [auth].[OrgUserRole] add constraint [df_OrgUserRole_created_by] default (user_name()) for [created_by]
alter table [auth].[OrgUserRole] add constraint [df_OrgUserRole_created_dt] default (getdate()) for [created_dt]
alter table [auth].[OrgUserRole] add constraint [df_OrgUserRole_modified_by] default (user_name()) for [modified_by]
alter table [auth].[OrgUserRole] add constraint [df_OrgUserRole_modified_dt] default (getdate()) for [modified_dt]

/*************************************
* Organization App specific tables
*************************************/
create table [auth].OrgApp
(
	Id int not null Identity(1,1)
	,OrgId int not null
		constraint fk_OrgApp_Org_Id references auth.Org(Id)
	,AppName varchar(250) not null
	,IsActive bit not null
	,IsDeleted bit not null
	,created_by varchar(50) not null
	,created_dt datetime not null
	,modified_by varchar(50) not null
	,modified_dt datetime not null

	constraint pk_OrgApp_Id primary key (Id asc)
)

alter table [auth].[OrgApp] add constraint [df_OrgApp_IsActive] default (0) for [IsActive]
alter table [auth].[OrgApp] add constraint [df_OrgApp_IsDeleted] default (0) for [IsDeleted]
alter table [auth].[OrgApp] add constraint [df_OrgApp_created_by] default (user_name()) for [created_by]
alter table [auth].[OrgApp] add constraint [df_OrgApp_created_dt] default (getdate()) for [created_dt]
alter table [auth].[OrgApp] add constraint [df_OrgApp_modified_by] default (user_name()) for [modified_by]
alter table [auth].[OrgApp] add constraint [df_OrgApp_modified_dt] default (getdate()) for [modified_dt]

create table [auth].OrgAppRole
(
	Id int not null identity(1,1)
	,OrgAppId int not null
		constraint fk_OrgAppRole_OrgApp_Id references auth.OrgApp(Id)
	,Name varchar(250) not null
	,IsActive bit not null
	,IsDeleted bit not null
	,created_by varchar(50) not null
	,created_dt datetime not null
	,modified_by varchar(50) not null
	,modified_dt datetime not null

	constraint pk_OrgAppRole_Id primary key (Id asc)
)

alter table [auth].[OrgAppRole] add constraint [df_OrgAppRole_IsActive] default (0) for [IsActive]
alter table [auth].[OrgAppRole] add constraint [df_OrgAppRole_IsDeleted] default (0) for [IsDeleted]
alter table [auth].[OrgAppRole] add constraint [df_OrgAppRole_created_by] default (user_name()) for [created_by]
alter table [auth].[OrgAppRole] add constraint [df_OrgAppRole_created_dt] default (getdate()) for [created_dt]
alter table [auth].[OrgAppRole] add constraint [df_OrgAppRole_modified_by] default (user_name()) for [modified_by]
alter table [auth].[OrgAppRole] add constraint [df_OrgAppRole_modified_dt] default (getdate()) for [modified_dt]

create table [auth].OrgAppUser
(
	Id int not null identity(1,1)
	,UserId int not null
		constraint fk_OrgAppUser_User_Id references auth.[User](id)
	,OrgAppId int not null 
		constraint fk_OrgAppUser_OrgApp_Id references auth.OrgApp(Id)
	,IsActive bit not null
	,IsDeleted bit not null
	,created_by varchar(50) not null
	,created_dt datetime not null
	,modified_by varchar(50) not null
	,modified_dt datetime not null

	constraint pk_OrgAppUser_Id primary key (Id asc)
)

alter table [auth].[OrgAppUser] add constraint [df_OrgAppUser_IsActive] default (0) for [IsActive]
alter table [auth].[OrgAppUser] add constraint [df_OrgAppUser_IsDeleted] default (0) for [IsDeleted]
alter table [auth].[OrgAppUser] add constraint [df_OrgAppUser_created_by] default (user_name()) for [created_by]
alter table [auth].[OrgAppUser] add constraint [df_OrgAppUser_created_dt] default (getdate()) for [created_dt]
alter table [auth].[OrgAppUser] add constraint [df_OrgAppUser_modified_by] default (user_name()) for [modified_by]
alter table [auth].[OrgAppUser] add constraint [df_OrgAppUser_modified_dt] default (getdate()) for [modified_dt]

create table [auth].OrgAppUserRole
(
	Id int not null identity(1,1)
	,OrgAppUserId int not null
		constraint fk_OrgAppUserRole_OrgAppUser_Id references auth.OrgAppUser(Id)
	,OrgAppRoleId int not null
		constraint fk_OrgAppUserRole_OrgAppRole_Id references auth.OrgAppRole(Id)
	,IsActive bit not null
	,IsDeleted bit not null
	,created_by varchar(50) not null
	,created_dt datetime not null
	,modified_by varchar(50) not null
	,modified_dt datetime not null

	constraint pk_OrgAppUserRole_Id primary key (Id asc)
)

alter table [auth].[OrgAppUserRole] add constraint [df_OrgAppUserRole_IsActive] default (0) for [IsActive]
alter table [auth].[OrgAppUserRole] add constraint [df_OrgAppUserRole_IsDeleted] default (0) for [IsDeleted]
alter table [auth].[OrgAppUserRole] add constraint [df_OrgAppUserRole_created_by] default (user_name()) for [created_by]
alter table [auth].[OrgAppUserRole] add constraint [df_OrgAppUserRole_created_dt] default (getdate()) for [created_dt]
alter table [auth].[OrgAppUserRole] add constraint [df_OrgAppUserRole_modified_by] default (user_name()) for [modified_by]
alter table [auth].[OrgAppUserRole] add constraint [df_OrgAppUserRole_modified_dt] default (getdate()) for [modified_dt]

create table [auth].OrgAppUserAuthIp
(
	Id int not null identity(1,1)
	,OrgAppUserId int not null
		constraint fk_OrgAppUserAuthIp_OrgAppUser_Id references auth.OrgAppUser(Id)
	,Ip varchar(15) not null
	,IsActive bit not null
	,IsDeleted bit not null
	,created_by varchar(50) not null
	,created_dt datetime not null
	,modified_by varchar(50) not null
	,modified_dt datetime not null

	constraint pk_OrgAppUserAuthIp_Id primary key (Id asc)
)

alter table [auth].[OrgAppUserAuthIp] add constraint [df_OrgAppUserAuthIp_IsActive] default (0) for [IsActive]
alter table [auth].[OrgAppUserAuthIp] add constraint [df_OrgAppUserAuthIp_IsDeleted] default (0) for [IsDeleted]
alter table [auth].[OrgAppUserAuthIp] add constraint [df_OrgAppUserAuthIp_created_by] default (user_name()) for [created_by]
alter table [auth].[OrgAppUserAuthIp] add constraint [df_OrgAppUserAuthIp_created_dt] default (getdate()) for [created_dt]
alter table [auth].[OrgAppUserAuthIp] add constraint [df_OrgAppUserAuthIp_modified_by] default (user_name()) for [modified_by]
alter table [auth].[OrgAppUserAuthIp] add constraint [df_OrgAppUserAuthIp_modified_dt] default (getdate()) for [modified_dt]


create table [auth].OrgAppUserMetadata
(
	Id int not null identity(1,1)
	,OrgAppUserId int not null
		constraint fk_OrgAppUserMetadata_OrgAppUser_Id references auth.OrgAppUser(Id)
	,Metadata xml not null
	,created_by varchar(50) not null
	,created_dt datetime not null
	,modified_by varchar(50) not null
	,modified_dt datetime not null

	constraint pk_OrgAppUserMetadata_Id primary key (Id asc)
)

alter table [auth].[OrgAppUserMetadata] add constraint [df_OrgAppUserMetadata_created_by] default (user_name()) for [created_by]
alter table [auth].[OrgAppUserMetadata] add constraint [df_OrgAppUserMetadata_created_dt] default (getdate()) for [created_dt]
alter table [auth].[OrgAppUserMetadata] add constraint [df_OrgAppUserMetadata_modified_by] default (user_name()) for [modified_by]
alter table [auth].[OrgAppUserMetadata] add constraint [df_OrgAppUserMetadata_modified_dt] default (getdate()) for [modified_dt]

create table [auth].UserRefreshToken
(
	Id uniqueidentifier not null
	,Username varchar(250) not null
	,AccessToken varchar(max) not null
	,ExpirationDateUtc datetimeoffset not null
	,IssuedDateUtc datetimeoffset not null
	,created_by varchar(50) not null
	,created_dt datetime not null
	,modified_by varchar(50) not null
	,modified_dt datetime not null

	constraint pk_UserRefreshToken_Id primary key (Id asc)
)

alter table [auth].[UserRefreshToken] add constraint [df_UserRefreshToken_created_by] default (user_name()) for [created_by]
alter table [auth].[UserRefreshToken] add constraint [df_UserRefreshToken_created_dt] default (getdate()) for [created_dt]
alter table [auth].[UserRefreshToken] add constraint [df_UserRefreshToken_modified_by] default (user_name()) for [modified_by]
alter table [auth].[UserRefreshToken] add constraint [df_UserRefreshToken_modified_dt] default (getdate()) for [modified_dt]