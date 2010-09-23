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
if ((select VersionNumber from AppSettings) <> '2010.06.08')
	RAISERROR (N'Update script version is invalid for this db version',17,1)	
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------
GO

GO
PRINT N'Altering [dbo].[AppSettings]...';

GO

if not Exists(select * from sys.columns where Name = N'AccountingSystem' and Object_ID = Object_ID(N'AppSettings'))
	ALTER TABLE [dbo].[AppSettings]
		ADD [AccountingSystem] VARCHAR (50) NULL;

GO
PRINT N'Altering [dbo].[AppSettings_Ins]...';

GO

ALTER PROCEDURE [dbo].[AppSettings_Ins]
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
	[AccountingSystem]
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
	@AccountingSystem
)
SELECT @AppSettingsID=SCOPE_IDENTITY()
GO
PRINT N'Altering [dbo].[AppSettings_Sel_All]...';

GO

ALTER PROCEDURE [dbo].[AppSettings_Sel_All]
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
	[AccountingSystem]
FROM [dbo].[AppSettings]
ORDER BY 
	[AppSettingsID] ASC
GO
PRINT N'Altering [dbo].[AppSettings_Sel_ByID]...';

GO

ALTER PROCEDURE [dbo].[AppSettings_Sel_ByID]
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
	[AccountingSystem]
FROM [dbo].[AppSettings]
WHERE
	[AppSettingsID] = @AppSettingsID
GO
PRINT N'Altering [dbo].[AppSettings_Sel_ByInstallID]...';

GO

ALTER PROCEDURE [dbo].[AppSettings_Sel_ByInstallID]
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
	[AccountingSystem]
FROM [dbo].[AppSettings]
WHERE
	[InstallID] = @InstallID
GO
PRINT N'Altering [dbo].[AppSettings_Upd]...';

GO

ALTER PROCEDURE [dbo].[AppSettings_Upd]
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
	@AccountingSystem varchar(50)
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
	[AccountingSystem] = @AccountingSystem
WHERE
	[AppSettingsID] = @AppSettingsID
	AND [RowVersion] = @RowVersion
GO

GO
PRINT N'Altering [dbo].[PurchaseItemDetail]...';

GO

ALTER VIEW [dbo].[PurchaseItemDetail]
AS
select
		itin.*,
		line.PurchaseLineID,
		line.PurchaseLineName,
		item.PurchaseItemID,
		item.PurchaseItemName,
		item.RequestStatusID,
		req.RequestStatusName,
		item.BookingReference,
		item.StartDate as PurchaseItemStartDate,
		item.StartTime as PurchaseItemStartTime,
		case item.StartDate when null then null else datename(weekday, item.StartDate) end as PurchaseItemDay,
		case item.StartDate when null then null else datename(month,   item.StartDate) end as PurchaseItemMonth,
		case item.StartDate	when null then null else datename(year,    item.StartDate) end as PurchaseItemYear,
		--case item.StartDate when null then null else substring(convert(varchar(10), item.StartDate, 120), 6, 2) end as PurchaseItemMonthNumber,
		isnull(item.EndDate, dateadd(day,item.NumberOfDays,item.StartDate)) as PurchaseItemEndDate,
		item.EndTime as PurchaseItemEndTime,
		line.NoteToSupplier,
		line.NoteToVoucher,
		line.NoteToClient,
		sup.*,
		serv.ServiceName,
		serv.CheckinMinutesEarly,
		opt.OptionName,
		opt.PricingOption,
		rate.ValidFrom AS RateValidFrom,
		rate.ValidTo AS RateValidTo,
		opt.Net as OptionNet,
		opt.Gross as OptionGross,
		serv.CurrencyCode,
		item.CurrencyRate,
		pric.Net,
		pric.Gross,
		item.NumberOfDays,
		item.Quantity,
		pric.UnitMultiplier,
		pric.TotalNet,
		pric.TotalGross,
		pric.Markup,
		pric.Commission,
		pric.Yield,
	    pric.Margin,
	    pric.NetTaxAmount,
	    pric.GrossTaxAmount,
		TotalNetExcl = TotalNet - NetTaxAmount,
		TotalGrossExcl = TotalGross - GrossTaxAmount,
		pric.TotalGrossOrig,
		pric.MarkupOrig,
		pric.CommissionOrig,
		stype.*,
		convert(varchar(3), PaymentTerm.DepositDuePeriod) + ' ' + depdue.PaymentDueName as DepositTerms,
		dbo.GetPaymentDate(item.StartDate, dbo.PaymentTerm.DepositDuePeriod, depdue.PaymentDueName) as DepositDueDate,
		PaymentTerm.DepositType,
		PaymentTerm.DepositAmount,
		convert(varchar(3), PaymentTerm.PaymentDuePeriod) + ' ' + baldue.PaymentDueName as BalanceTerms,
		dbo.GetPaymentDate(item.StartDate, PaymentTerm.PaymentDuePeriod, baldue.PaymentDueName) as BalanceDueDate
	from PurchaseItem item
	join dbo.PurchaseItemPricing() pric on item.PurchaseItemID = pric.PurchaseItemID
	left outer join PurchaseLine as line on line.PurchaseLineID = item.PurchaseLineID
	left outer join ItineraryDetail as itin on line.ItineraryID = itin.ItineraryID
	left outer join Itinerary as itin2 on line.ItineraryID = itin2.ItineraryID
	left outer join RequestStatus req on req.RequestStatusID = item.RequestStatusID
	left outer join [Option] as opt on item.OptionID = opt.OptionID
	left outer join Rate as rate on opt.RateID = rate.RateID
	left outer join [Service] AS serv on rate.ServiceID = serv.ServiceID
	left outer join SupplierDetail sup on serv.SupplierID = sup.SupplierID
	left outer join ServiceTypeDetail as stype on serv.ServiceTypeID = stype.ServiceTypeID
	left outer join PaymentDue as baldue
	right outer join PaymentTerm on baldue.PaymentDueID = PaymentTerm.PaymentDueID
	left outer join PaymentDue as depdue on PaymentTerm.DepositDueID = depdue.PaymentDueID on item.PaymentTermID = PaymentTerm.PaymentTermID;
GO
EXECUTE sp_refreshview N'dbo.PurchaseItemPaymentsDetail';
GO

----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2010.09.21'
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
