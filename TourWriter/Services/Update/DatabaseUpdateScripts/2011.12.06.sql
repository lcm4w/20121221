/* TourWriter database update script */
GO
SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING ON
GO
SET ANSI_WARNINGS ON
GO
SET CONCAT_NULL_YIELDS_NULL ON
GO
SET ARITHABORT ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
IF EXISTS (SELECT * FROM tempdb..sysobjects WHERE id=OBJECT_ID('tempdb..#tmpErrors')) DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL READ COMMITTED
GO
BEGIN TRANSACTION
GO
if ((select VersionNumber from AppSettings) <> '2011.09.26')
	RAISERROR (N'Database Update Script is not correct version for current database version',17,1)	

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------
GO
PRINT N'Creating [dbo].[CurrencyRate]...';
GO

if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME='CurrencyRate') begin 
	drop table [dbo].[CurrencyRate]
end

CREATE TABLE [dbo].[CurrencyRate] (
    [CurrencyRateID] INT      IDENTITY (1, 1) NOT NULL,
    [CodeFrom]       CHAR (3) NOT NULL,
    [CodeTo]         CHAR (3) NOT NULL,
    [Rate]           MONEY    NOT NULL,
    [ValidFrom]      DATETIME NOT NULL,
    [ValidTo]        DATETIME NOT NULL,
    [EnabledFrom]    DATETIME NULL,
    [EnabledTo]      DATETIME NULL
);


GO
PRINT N'Creating PK_CurrencyRate_CurrencyRateID...';


GO
ALTER TABLE [dbo].[CurrencyRate]
    ADD CONSTRAINT [PK_CurrencyRate_CurrencyRateID] PRIMARY KEY CLUSTERED ([CurrencyRateID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Altering [dbo].[_ToolSet_Sel_All]...';


GO
ALTER PROCEDURE [dbo].[_ToolSet_Sel_All]

AS
SET NOCOUNT ON

EXEC ServiceType_Sel_All
EXEC Grade_Sel_All
EXEC GradeExternal_Sel_All
EXEC CreditCard_Sel_All
EXEC Country_Sel_All
EXEC State_Sel_All
EXEC Region_Sel_All
EXEC City_Sel_All
EXEC ServiceConfigType_Sel_All
EXEC AgeGroup_Sel_All
EXEC ItineraryStatus_Sel_All
EXEC RequestStatus_Sel_All
EXEC User_Sel_All
EXEC Branch_Sel_All
EXEC Department_Sel_All
EXEC ItinerarySource_Sel_All
EXEC AppSettings_Sel_All
EXEC Agent_Sel_All
EXEC SupplierConfigType_Sel_All
EXEC MenuType_Sel_All
EXEC PaymentType_Sel_All
EXEC Currency_Sel_All
EXEC TaxType_Sel_All
EXEC PaymentDue_Sel_All
EXEC AccountingCategory_Sel_All
EXEC ContactCategory_Sel_All
EXEC OptionType_Sel_All
EXEC Flag_Sel_All
EXEC TemplateCategory_Sel_All
EXEC Template_Sel_All
EXEC ContentType_Sel_All
select * from CurrencyRate where ValidTo > getdate() --EXEC CurrencyRate_Sel_All
GO

GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CurrencyRate_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[CurrencyRate_Ins]
GO

CREATE PROCEDURE [dbo].[CurrencyRate_Ins]
	@CodeFrom char(3),
	@CodeTo char(3),
	@Rate money,
	@ValidFrom datetime,
	@ValidTo datetime,
	@EnabledFrom datetime,
	@EnabledTo datetime,
	@CurrencyRateID int OUTPUT
AS
INSERT [dbo].[CurrencyRate]
(
	[CodeFrom],
	[CodeTo],
	[Rate],
	[ValidFrom],
	[ValidTo],
	[EnabledFrom],
	[EnabledTo]
)
VALUES
(
	@CodeFrom,
	@CodeTo,
	@Rate,
	@ValidFrom,
	@ValidTo,
	@EnabledFrom,
	@EnabledTo
)
SELECT @CurrencyRateID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CurrencyRate_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[CurrencyRate_Upd]
GO

CREATE PROCEDURE [dbo].[CurrencyRate_Upd]
	@CurrencyRateID int,
	@CodeFrom char(3),
	@CodeTo char(3),
	@Rate money,
	@ValidFrom datetime,
	@ValidTo datetime,
	@EnabledFrom datetime,
	@EnabledTo datetime
AS
UPDATE [dbo].[CurrencyRate]
SET 
	[CodeFrom] = @CodeFrom,
	[CodeTo] = @CodeTo,
	[Rate] = @Rate,
	[ValidFrom] = @ValidFrom,
	[ValidTo] = @ValidTo,
	[EnabledFrom] = @EnabledFrom,
	[EnabledTo] = @EnabledTo
WHERE
	[CurrencyRateID] = @CurrencyRateID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CurrencyRate_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[CurrencyRate_Del]
GO

CREATE PROCEDURE [dbo].[CurrencyRate_Del]
	@CurrencyRateID int
AS
DELETE FROM [dbo].[CurrencyRate]
WHERE
	[CurrencyRateID] = @CurrencyRateID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CurrencyRate_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[CurrencyRate_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[CurrencyRate_Sel_ByID]
	@CurrencyRateID int
AS
SET NOCOUNT ON
SELECT
	[CurrencyRateID],
	[CodeFrom],
	[CodeTo],
	[Rate],
	[ValidFrom],
	[ValidTo],
	[EnabledFrom],
	[EnabledTo]
FROM [dbo].[CurrencyRate]
WHERE
	[CurrencyRateID] = @CurrencyRateID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CurrencyRate_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[CurrencyRate_Sel_All]
GO

CREATE PROCEDURE [dbo].[CurrencyRate_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[CurrencyRateID],
	[CodeFrom],
	[CodeTo],
	[Rate],
	[ValidFrom],
	[ValidTo],
	[EnabledFrom],
	[EnabledTo]
FROM [dbo].[CurrencyRate]
ORDER BY 
	[CurrencyRateID] ASC
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CurrencyRate_Del_ByCurrencyCodeFrom]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[CurrencyRate_Del_ByCurrencyCodeFrom]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CurrencyRate_Del_ByCurrencyCodeTo]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[CurrencyRate_Del_ByCurrencyCodeTo]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CurrencyRate_Sel_ByCurrencyCodeFrom]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[CurrencyRate_Sel_ByCurrencyCodeFrom]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CurrencyRate_Sel_ByCurrencyCodeTo]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[CurrencyRate_Sel_ByCurrencyCodeTo]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CurrencyRate_Upd_ByCurrencyCodeTo]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[CurrencyRate_Upd_ByCurrencyCodeTo]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[CurrencyRate_Upd_ByCurrencyCodeFrom]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[CurrencyRate_Upd_ByCurrencyCodeFrom]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[_ToolSet_Load_CurrencyRate]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[_ToolSet_Load_CurrencyRate]
GO










IF NOT EXISTS (select * from sys.columns where Name = N'CcyRateSource' and Object_ID = Object_ID(N'AppSettings'))
	ALTER TABLE [dbo].[AppSettings] ADD [CcyRateSource] VARCHAR(10) NULL;

IF NOT EXISTS (select * from sys.columns where Name = N'CcyDatePoint' and Object_ID = Object_ID(N'AppSettings'))
	ALTER TABLE [dbo].[AppSettings] ADD [CcyDatePoint] VARCHAR(10) NULL;


if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AppSettings_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AppSettings_Ins]
GO

CREATE PROCEDURE [dbo].[AppSettings_Ins]
	@InstallID uniqueidentifier,
	@InstallName varchar(255),
	@VersionNumber varchar(100),
	@CurrencyCode char(3),
	@SupportEmail varchar(255),
	@SupportPhone varchar(50),
	@SmtpServerName varchar(255),
	@SmtpServerPort int,
	@SmtpServerUsername varchar(255),
	@SmtpServerPassword varchar(255),
	@SmtpServerEnableSsl bit,
	@LastDbBackupDate datetime,
	@LastDbBackupFile varchar(500),
	@LastDbBackupName varchar(500),
	@ExternalFilesPath varchar(500),
	@CancelledRequestStatusID int,
	@AccountingSystem varchar(50),
	@CustomCodeFormat varchar(50),
	@LanguageCode varchar(10),
	@CcyRateSource varchar(20),
	@CcyDatePoint varchar(20),
	@AppSettingsID int OUTPUT
AS
INSERT [dbo].[AppSettings]
(
	[InstallID],
	[InstallName],
	[VersionNumber],
	[CurrencyCode],
	[SupportEmail],
	[SupportPhone],
	[SmtpServerName],
	[SmtpServerPort],
	[SmtpServerUsername],
	[SmtpServerPassword],
	[SmtpServerEnableSsl],
	[LastDbBackupDate],
	[LastDbBackupFile],
	[LastDbBackupName],
	[ExternalFilesPath],
	[CancelledRequestStatusID],
	[AccountingSystem],
	[CustomCodeFormat],
	[LanguageCode],
	[CcyRateSource],
	[CcyDatePoint]
)
VALUES
(
	ISNULL(@InstallID, (newid())),
	@InstallName,
	@VersionNumber,
	@CurrencyCode,
	@SupportEmail,
	@SupportPhone,
	@SmtpServerName,
	@SmtpServerPort,
	@SmtpServerUsername,
	@SmtpServerPassword,
	@SmtpServerEnableSsl,
	@LastDbBackupDate,
	@LastDbBackupFile,
	@LastDbBackupName,
	@ExternalFilesPath,
	@CancelledRequestStatusID,
	@AccountingSystem,
	@CustomCodeFormat,
	@LanguageCode,
	@CcyRateSource,
	@CcyDatePoint
)
SELECT @AppSettingsID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AppSettings_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AppSettings_Upd]
GO

CREATE PROCEDURE [dbo].[AppSettings_Upd]
	@AppSettingsID int,
	@InstallID uniqueidentifier,
	@InstallName varchar(255),
	@VersionNumber varchar(100),
	@CurrencyCode char(3),
	@SupportEmail varchar(255),
	@SupportPhone varchar(50),
	@SmtpServerName varchar(255),
	@SmtpServerPort int,
	@SmtpServerUsername varchar(255),
	@SmtpServerPassword varchar(255),
	@SmtpServerEnableSsl bit,
	@LastDbBackupDate datetime,
	@LastDbBackupFile varchar(500),
	@LastDbBackupName varchar(500),
	@ExternalFilesPath varchar(500),
	@CancelledRequestStatusID int,
	@RowVersion timestamp,
	@AccountingSystem varchar(50),
	@CustomCodeFormat varchar(50),
	@LanguageCode varchar(10),
	@CcyRateSource varchar(20),
	@CcyDatePoint varchar(20)
AS
UPDATE [dbo].[AppSettings]
SET 
	[InstallID] = @InstallID,
	[InstallName] = @InstallName,
	[VersionNumber] = @VersionNumber,
	[CurrencyCode] = @CurrencyCode,
	[SupportEmail] = @SupportEmail,
	[SupportPhone] = @SupportPhone,
	[SmtpServerName] = @SmtpServerName,
	[SmtpServerPort] = @SmtpServerPort,
	[SmtpServerUsername] = @SmtpServerUsername,
	[SmtpServerPassword] = @SmtpServerPassword,
	[SmtpServerEnableSsl] = @SmtpServerEnableSsl,
	[LastDbBackupDate] = @LastDbBackupDate,
	[LastDbBackupFile] = @LastDbBackupFile,
	[LastDbBackupName] = @LastDbBackupName,
	[ExternalFilesPath] = @ExternalFilesPath,
	[CancelledRequestStatusID] = @CancelledRequestStatusID,
	[AccountingSystem] = @AccountingSystem,
	[CustomCodeFormat] = @CustomCodeFormat,
	[LanguageCode] = @LanguageCode,
	[CcyRateSource] = @CcyRateSource,
	[CcyDatePoint] = @CcyDatePoint
WHERE
	[AppSettingsID] = @AppSettingsID
	AND [RowVersion] = @RowVersion
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AppSettings_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AppSettings_Del]
GO

CREATE PROCEDURE [dbo].[AppSettings_Del]
	@AppSettingsID int,
	@RowVersion timestamp
AS
DELETE FROM [dbo].[AppSettings]
WHERE
	[AppSettingsID] = @AppSettingsID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AppSettings_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AppSettings_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[AppSettings_Sel_ByID]
	@AppSettingsID int
AS
SET NOCOUNT ON
SELECT
	[AppSettingsID],
	[InstallID],
	[InstallName],
	[VersionNumber],
	[CurrencyCode],
	[SupportEmail],
	[SupportPhone],
	[SmtpServerName],
	[SmtpServerPort],
	[SmtpServerUsername],
	[SmtpServerPassword],
	[SmtpServerEnableSsl],
	[LastDbBackupDate],
	[LastDbBackupFile],
	[LastDbBackupName],
	[ExternalFilesPath],
	[CancelledRequestStatusID],
	[RowVersion],
	[AccountingSystem],
	[CustomCodeFormat],
	[LanguageCode],
	[CcyRateSource],
	[CcyDatePoint]
FROM [dbo].[AppSettings]
WHERE
	[AppSettingsID] = @AppSettingsID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AppSettings_Sel_ByInstallID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AppSettings_Sel_ByInstallID]
GO

CREATE PROCEDURE [dbo].[AppSettings_Sel_ByInstallID]
	@InstallID uniqueidentifier
AS
SET NOCOUNT ON
SELECT
	[AppSettingsID],
	[InstallID],
	[InstallName],
	[VersionNumber],
	[CurrencyCode],
	[SupportEmail],
	[SupportPhone],
	[SmtpServerName],
	[SmtpServerPort],
	[SmtpServerUsername],
	[SmtpServerPassword],
	[SmtpServerEnableSsl],
	[LastDbBackupDate],
	[LastDbBackupFile],
	[LastDbBackupName],
	[ExternalFilesPath],
	[CancelledRequestStatusID],
	[RowVersion],
	[AccountingSystem],
	[CustomCodeFormat],
	[LanguageCode],
	[CcyRateSource],
	[CcyDatePoint]
FROM [dbo].[AppSettings]
WHERE
	[InstallID] = @InstallID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AppSettings_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AppSettings_Sel_All]
GO

CREATE PROCEDURE [dbo].[AppSettings_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[AppSettingsID],
	[InstallID],
	[InstallName],
	[VersionNumber],
	[CurrencyCode],
	[SupportEmail],
	[SupportPhone],
	[SmtpServerName],
	[SmtpServerPort],
	[SmtpServerUsername],
	[SmtpServerPassword],
	[SmtpServerEnableSsl],
	[LastDbBackupDate],
	[LastDbBackupFile],
	[LastDbBackupName],
	[ExternalFilesPath],
	[CancelledRequestStatusID],
	[RowVersion],
	[AccountingSystem],
	[CustomCodeFormat],
	[LanguageCode],
	[CcyRateSource],
	[CcyDatePoint]
FROM [dbo].[AppSettings]
ORDER BY 
	[AppSettingsID] ASC
GO


if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[_Permission_AddToRoles]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[_Permission_AddToRoles]
GO

create PROCEDURE [dbo].[_Permission_AddToRoles] 
	@permissionName varchar(100)
AS

-- adds a permission to all roles, used when adding new permissions, to default enable for all users

DECLARE @roleId int
DECLARE @permissionId int
SET @permissionId = (select PermissionID from Permission where PermissionName = @permissionName)

DECLARE RoleIds CURSOR FOR SELECT RoleID FROM Role
OPEN RoleIds
FETCH NEXT FROM RoleIds INTO @roleId
WHILE @@FETCH_STATUS = 0
BEGIN
    if not exists (select RoleID from RolePermission where RoleID = @roleId and PermissionID = @permissionId)
		insert RolePermission (RoleID, PermissionID) values (@roleId, @permissionId)
    FETCH NEXT FROM RoleIds INTO @roleId
END

CLOSE RoleIds
DEALLOCATE RoleIds

GO


-- update Permissions
if not exists (select PermissionName from Permission where PermissionName = 'Reports - run Custom Reports')
	insert Permission values ('Reports - run Custom Reports');
exec [dbo].[_Permission_AddToRoles] 'Reports - run Custom Reports';
GO
if not exists (select PermissionName from Permission where PermissionName = 'Currency rates - edit')
	insert Permission values ('Currency rates - edit');
exec [dbo].[_Permission_AddToRoles] 'Currency rates - edit'
GO
if not exists (select PermissionName from Permission where PermissionName = 'Itinerary - refresh currency rates')
	insert Permission values ('Itinerary - refresh currency rates');
exec [dbo].[_Permission_AddToRoles] 'Itinerary - refresh currency rates';
GO


GO
exec __RefreshViews;
GO
----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2011.12.06'
GO
IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
PRINT N'The transacted portion of the database update succeeded.'
COMMIT TRANSACTION
END
ELSE PRINT N'The transacted portion of the database update failed.'
GO
DROP TABLE #tmpErrors
GO
