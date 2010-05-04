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
if ((select VersionNumber from AppSettings) <> '2009.11.24')
	RAISERROR (N'Update script version is invalid for this db version',17,1)	
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------

GO
PRINT N'Dropping [dbo].[_User_CheckLogin]...';


GO
DROP PROCEDURE [dbo].[_User_CheckLogin];


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Dropping [dbo].[_User_RemoveLogin]...';


GO
DROP PROCEDURE [dbo].[_User_RemoveLogin];


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Dropping [dbo].[_User_UpdateLogin]...';


GO
DROP PROCEDURE [dbo].[_User_UpdateLogin];


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Altering [dbo].[Login]...';


GO
ALTER TABLE [dbo].[Login]
    ADD [Ended] BIT NULL;


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Altering [dbo].[Service_Ins]...';


GO

ALTER PROCEDURE [dbo].[Service_Ins]
	@ServiceName varchar(150),
	@SupplierID int,
	@ServiceTypeID int,
	@Description varchar(2000),
	@Comments varchar(2000),
	@Warning varchar(2000),
	@MaxPax int,
	@CheckinTime datetime,
	@CheckoutTime datetime,
	@CheckinMinutesEarly int,
	@IsRecordActive bit,
	@CurrencyCode char(3),
	@ChargeType varchar(10),
	@PaymentTermID int,
	@TaxTypeID int,
	@Latitude float(53),
	@Longitude float(53),
	@AddedOn datetime,
	@AddedBy int,
	@IsDeleted bit,
	@ServiceID int OUTPUT
AS
INSERT [dbo].[Service]
(
	[ServiceName],
	[SupplierID],
	[ServiceTypeID],
	[Description],
	[Comments],
	[Warning],
	[MaxPax],
	[CheckinTime],
	[CheckoutTime],
	[CheckinMinutesEarly],
	[IsRecordActive],
	[CurrencyCode],
	[ChargeType],
	[PaymentTermID],
	[TaxTypeID],
	[Latitude],
	[Longitude],
	[AddedOn],
	[AddedBy],
	[IsDeleted]
)
VALUES
(
	ISNULL(@ServiceName, ('')),
	@SupplierID,
	@ServiceTypeID,
	@Description,
	@Comments,
	@Warning,
	@MaxPax,
	@CheckinTime,
	@CheckoutTime,
	@CheckinMinutesEarly,
	@IsRecordActive,
	@CurrencyCode,
	@ChargeType,
	@PaymentTermID,
	@TaxTypeID,
	@Latitude,
	@Longitude,
	@AddedOn,
	@AddedBy,
	@IsDeleted
)
SELECT @ServiceID=SCOPE_IDENTITY()
GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Altering [dbo].[Service_Upd]...';


GO

ALTER PROCEDURE [dbo].[Service_Upd]
	@ServiceID int,
	@ServiceName varchar(150),
	@SupplierID int,
	@ServiceTypeID int,
	@Description varchar(2000),
	@Comments varchar(2000),
	@Warning varchar(2000),
	@MaxPax int,
	@CheckinTime datetime,
	@CheckoutTime datetime,
	@CheckinMinutesEarly int,
	@IsRecordActive bit,
	@CurrencyCode char(3),
	@ChargeType varchar(10),
	@PaymentTermID int,
	@TaxTypeID int,
	@Latitude float(53),
	@Longitude float(53),
	@AddedOn datetime,
	@AddedBy int,
	@RowVersion timestamp,
	@IsDeleted bit
AS
UPDATE [dbo].[Service]
SET 
	[ServiceName] = @ServiceName,
	[SupplierID] = @SupplierID,
	[ServiceTypeID] = @ServiceTypeID,
	[Description] = @Description,
	[Comments] = @Comments,
	[Warning] = @Warning,
	[MaxPax] = @MaxPax,
	[CheckinTime] = @CheckinTime,
	[CheckoutTime] = @CheckoutTime,
	[CheckinMinutesEarly] = @CheckinMinutesEarly,
	[IsRecordActive] = @IsRecordActive,
	[CurrencyCode] = @CurrencyCode,
	[ChargeType] = @ChargeType,
	[PaymentTermID] = @PaymentTermID,
	[TaxTypeID] = @TaxTypeID,
	[Latitude] = @Latitude,
	[Longitude] = @Longitude,
	[AddedOn] = @AddedOn,
	[AddedBy] = @AddedBy,
	[IsDeleted] = @IsDeleted
WHERE
	[ServiceID] = @ServiceID
	AND [RowVersion] = @RowVersion
GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Altering [dbo].[Supplier_Ins]...';


GO

ALTER PROCEDURE [dbo].[Supplier_Ins]
	@SupplierName varchar(150),
	@HostName varchar(150),
	@StreetAddress varchar(100),
	@PostAddress varchar(500),
	@Postcode varchar(20),
	@CityID int,
	@RegionID int,
	@StateID int,
	@CountryID int,
	@Phone varchar(50),
	@MobilePhone varchar(50),
	@FreePhone varchar(50),
	@Fax varchar(50),
	@Email varchar(200),
	@Website varchar(200),
	@Latitude float(53),
	@Longitude float(53),
	@GradeID int,
	@GradeExternalID int,
	@Description varchar(4000),
	@Comments varchar(4000),
	@CancellationPolicy varchar(2000),
	@BankDetails varchar(255),
	@AccountingName varchar(150),
	@TaxTypeID int,
	@PaymentTermID int,
	@DefaultMargin real,
	@DefaultCheckinTime datetime,
	@DefaultCheckoutTime datetime,
	@ImportID varchar(50),
	@ExportID varchar(50),
	@BookingWebsite varchar(255),
	@IsRecordActive bit,
	@ParentFolderID int,
	@AddedOn datetime,
	@AddedBy int,
	@IsDeleted bit,
	@SupplierID int OUTPUT
AS
INSERT [dbo].[Supplier]
(
	[SupplierName],
	[HostName],
	[StreetAddress],
	[PostAddress],
	[Postcode],
	[CityID],
	[RegionID],
	[StateID],
	[CountryID],
	[Phone],
	[MobilePhone],
	[FreePhone],
	[Fax],
	[Email],
	[Website],
	[Latitude],
	[Longitude],
	[GradeID],
	[GradeExternalID],
	[Description],
	[Comments],
	[CancellationPolicy],
	[BankDetails],
	[AccountingName],
	[TaxTypeID],
	[PaymentTermID],
	[DefaultMargin],
	[DefaultCheckinTime],
	[DefaultCheckoutTime],
	[ImportID],
	[ExportID],
	[BookingWebsite],
	[IsRecordActive],
	[ParentFolderID],
	[AddedOn],
	[AddedBy],
	[IsDeleted]
)
VALUES
(
	@SupplierName,
	@HostName,
	@StreetAddress,
	@PostAddress,
	@Postcode,
	@CityID,
	@RegionID,
	@StateID,
	@CountryID,
	@Phone,
	@MobilePhone,
	@FreePhone,
	@Fax,
	@Email,
	@Website,
	@Latitude,
	@Longitude,
	@GradeID,
	@GradeExternalID,
	@Description,
	@Comments,
	@CancellationPolicy,
	@BankDetails,
	@AccountingName,
	@TaxTypeID,
	@PaymentTermID,
	@DefaultMargin,
	@DefaultCheckinTime,
	@DefaultCheckoutTime,
	@ImportID,
	@ExportID,
	@BookingWebsite,
	@IsRecordActive,
	@ParentFolderID,
	@AddedOn,
	@AddedBy,
	@IsDeleted
)
SELECT @SupplierID=SCOPE_IDENTITY()
GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Altering [dbo].[Supplier_Upd]...';


GO

ALTER PROCEDURE [dbo].[Supplier_Upd]
	@SupplierID int,
	@SupplierName varchar(150),
	@HostName varchar(150),
	@StreetAddress varchar(100),
	@PostAddress varchar(500),
	@Postcode varchar(20),
	@CityID int,
	@RegionID int,
	@StateID int,
	@CountryID int,
	@Phone varchar(50),
	@MobilePhone varchar(50),
	@FreePhone varchar(50),
	@Fax varchar(50),
	@Email varchar(200),
	@Website varchar(200),
	@Latitude float(53),
	@Longitude float(53),
	@GradeID int,
	@GradeExternalID int,
	@Description varchar(4000),
	@Comments varchar(4000),
	@CancellationPolicy varchar(2000),
	@BankDetails varchar(255),
	@AccountingName varchar(150),
	@TaxTypeID int,
	@PaymentTermID int,
	@DefaultMargin real,
	@DefaultCheckinTime datetime,
	@DefaultCheckoutTime datetime,
	@ImportID varchar(50),
	@ExportID varchar(50),
	@BookingWebsite varchar(255),
	@IsRecordActive bit,
	@ParentFolderID int,
	@AddedOn datetime,
	@AddedBy int,
	@RowVersion timestamp,
	@IsDeleted bit
AS
UPDATE [dbo].[Supplier]
SET 
	[SupplierName] = @SupplierName,
	[HostName] = @HostName,
	[StreetAddress] = @StreetAddress,
	[PostAddress] = @PostAddress,
	[Postcode] = @Postcode,
	[CityID] = @CityID,
	[RegionID] = @RegionID,
	[StateID] = @StateID,
	[CountryID] = @CountryID,
	[Phone] = @Phone,
	[MobilePhone] = @MobilePhone,
	[FreePhone] = @FreePhone,
	[Fax] = @Fax,
	[Email] = @Email,
	[Website] = @Website,
	[Latitude] = @Latitude,
	[Longitude] = @Longitude,
	[GradeID] = @GradeID,
	[GradeExternalID] = @GradeExternalID,
	[Description] = @Description,
	[Comments] = @Comments,
	[CancellationPolicy] = @CancellationPolicy,
	[BankDetails] = @BankDetails,
	[AccountingName] = @AccountingName,
	[TaxTypeID] = @TaxTypeID,
	[PaymentTermID] = @PaymentTermID,
	[DefaultMargin] = @DefaultMargin,
	[DefaultCheckinTime] = @DefaultCheckinTime,
	[DefaultCheckoutTime] = @DefaultCheckoutTime,
	[ImportID] = @ImportID,
	[ExportID] = @ExportID,
	[BookingWebsite] = @BookingWebsite,
	[IsRecordActive] = @IsRecordActive,
	[ParentFolderID] = @ParentFolderID,
	[AddedOn] = @AddedOn,
	[AddedBy] = @AddedBy,
	[IsDeleted] = @IsDeleted
WHERE
	[SupplierID] = @SupplierID
	AND [RowVersion] = @RowVersion
GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Creating [dbo].[_Login_AddOrUpdate]...';


GO
CREATE PROCEDURE [dbo].[_Login_AddOrUpdate]
	@LoginGuid UNIQUEIDENTIFIER, 
	@UserID INT, 
	@ComputerName VARCHAR (200),
	@SessionTimeout int
AS

declare @timestamp DateTime = getdate()

-- update existing
UPDATE [Login]
SET LastActiveDate = @timestamp
where (Ended is null or Ended = 'false')
and ComputerName = @ComputerName
and cast(convert(char(10), LoginDate, 101) as smalldatetime) = cast(convert(char(10), @timestamp, 101) as smalldatetime) -- today

IF @@ROWCOUNT > 0
begin
	set @LoginGuid = (select top(1) LoginId from [Login] where ComputerName = @ComputerName and LastActiveDate = @timestamp)
end
else
begin
	SET @LoginGUID = NEWID()
	INSERT [dbo].[Login]
	(
		[LoginID],
		[UserID],
		[LoginDate],
		[LastActiveDate],
		[ComputerName]
	)
	VALUES
	(
		@LoginGuid,
		@UserID,
		@timestamp,
		@timestamp,
		@ComputerName
	)
	
	-- house keeping (when inserting new) - remove older than 30 days
	DELETE Login 
	WHERE LastActiveDate < DATEADD(day, -30, GETDATE())	
END
	
if (@SessionTimeout > 0) set @SessionTimeout = -@SessionTimeout
select SessionId, SessionIndex, SessionCount
from (
	select 
		SessionId = LoginId,
		SessionIndex = row_number() over(order by LastActiveDate asc), 
		SessionCount = count(*) over()
	from [Login] 
	where LastActiveDate > dateadd(minute, @SessionTimeout, getdate())
	and (Ended is null or Ended = 'false')
) t where SessionId = @LoginGuid
GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Altering [dbo].[ItineraryDetail]...';


GO

ALTER VIEW [dbo].[ItineraryDetail]
AS
select
		itin.ItineraryID,
		itin.ItineraryName,
		isnull(itin.DisplayName, itin.ItineraryName) DisplayName,
		itin.CustomCode,
		itin.CountryID as OriginID,
		origin.CountryName as Origin,
		datediff(dd, itin.ArriveDate, itin.DepartDate) + 1 as ItineraryLength,
		itin.ArriveDate as ArriveDate, 
		case itin.ArriveDate when null then null else datename(weekday, itin.ArriveDate) end as ArriveDay,
		case itin.ArriveDate when null then null else datename(month,   itin.ArriveDate) end as ArriveMonth,
		case itin.ArriveDate when null then null else datename(year,    itin.ArriveDate) end as ArriveYear, 
		--case itin.ArriveDate when null then null else substring(convert(varchar(10), itin.ArriveDate, 120), 6, 2) end as ArriveMonth,
		arriveCity.CityName as ArriveCityName,
		ArriveFlight,
		itin.DepartDate,
		departCity.CityName as DepartCityName,
		DepartFlight,
		itin.AssignedTo as UserID,
		u.DisplayName as UserDisplayName,
		agent.AgentID,
		agent.AgentName,
		parentAgent.AgentName as ParentAgentName,
		agent.Phone as AgentPhone,
		agent.Email as AgentEmail,
		agent.VoucherFooter as AgentVoucherNote,
		stat.ItineraryStatusID,
		stat.ItineraryStatusName,
		ItinerarySourceName,
		usr.UserName as AssignedTo,
		depart.DepartmentID,
		depart.DepartmentName,
		branch.BranchID,
		branch.BranchName,
		itin.NetComOrMup as NetOverrideComOrMup,
		pric.Net as ItineraryNet,
		pric.Gross as ItineraryGross,
		pric.Markup as ItineraryMarkup,
		pric.Commission as ItineraryCommission,
		pric.Yield as ItineraryYield,
		pric.NetTax as ItineraryNetTax,
		pric.GrossTax as ItineraryGrossTax,
		pric.GrossOrig as ItineraryGrossOrig,
		pric.MarkupOrig as ItineraryMarkupOrig,
		pric.CommissionOrig as ItineraryCommissionOrig,
		payments.TotalPayments as ItineraryTotalPayments,
		pric.Gross - payments.TotalPayments as ItineraryTotalOutstanding,
		sales.TotalSales as ItineraryTotalSales,
		sales.TotalSalesTax as ItineraryTotalSalesTax,
		pric.ItemCount as ItineraryPurchaseItemCount,
		isnull(PaxOverride, pax.PaxCount) as PaxCount,
		convert(varchar(3), dbo.PaymentTerm.DepositDuePeriod) + ' ' + depdue.PaymentDueName as ItineraryDepositTerms,
		dbo.GetPaymentDate(itin.ArriveDate, dbo.PaymentTerm.DepositDuePeriod, depdue.PaymentDueName) as ItineraryDepositDueDate, 
		dbo.PaymentTerm.DepositType as ItineraryDepositType, 
		dbo.PaymentTerm.DepositAmount as ItineraryDepositAmount,
		convert(varchar(3), dbo.PaymentTerm.PaymentDuePeriod) + ' ' + baldue.PaymentDueName as ItineraryBalanceTerms,
		dbo.GetPaymentDate(itin.ArriveDate, dbo.PaymentTerm.PaymentDuePeriod, baldue.PaymentDueName) as ItineraryBalanceDueDate,
		pric.Gross - dbo.PaymentTerm.DepositAmount as ItineraryBalanceAmount,
		itin.AddedOn as ItineraryCreatedDate,
		ParentFolderID as MenuFolderID	
	from Itinerary itin
	join dbo.ItineraryPricing() pric on itin.ItineraryID = pric.ItineraryID
	left outer join Country origin on itin.CountryID = origin.CountryID
	left outer join City arriveCity on itin.ArriveCityID = arriveCity.CityID
	left outer join City departCity on itin.DepartCityID = departCity.CityID
	left outer join Agent agent on itin.AgentID = agent.AgentID
	left outer join Agent parentAgent on agent.ParentAgentID = parentAgent.AgentID
	left outer join dbo.PaymentDue as baldue 
	right outer join dbo.PaymentTerm on baldue.PaymentDueID = dbo.PaymentTerm.PaymentDueID 
	left outer join dbo.PaymentDue as depdue on dbo.PaymentTerm.DepositDueID = depdue.PaymentDueID on isnull(itin.PaymentTermID,agent.SalePaymentTermID) = dbo.PaymentTerm.PaymentTermID
	left outer join ItineraryStatus stat on itin.ItineraryStatusID = stat.ItineraryStatusID
	left outer join ItinerarySource source on itin.ItinerarySourceID = source.ItinerarySourceID
	left outer join [User] usr on itin.AssignedTo = usr.UserID
	left outer join Department depart on itin.DepartmentID = depart.DepartmentID
	left outer join Branch branch on itin.BranchID = branch.BranchID
	left outer join [User] u on u.UserID = itin.AssignedTo
	left outer join
	(
		select itin.ItineraryID, count(ItineraryMemberID) as PaxCount
		from ItineraryMember mem
		left outer join ItineraryGroup grp ON mem.ItineraryGroupID = grp.ItineraryGroupID
		left outer join Itinerary itin ON grp.ItineraryID = itin.ItineraryID
		group by itin.ItineraryID
	) pax on itin.ItineraryID = pax.ItineraryID
	left outer join 
	(
		select ItineraryID, sum(SaleAmount) as TotalSales, sum(TaxAmount) as TotalSalesTax
		from ItinerarySaleAllocationPricing() sale
		group by ItineraryID
	) sales on itin.ItineraryID = sales.ItineraryID
	left outer join 
	(
		select ItineraryID, sum(Amount) as TotalPayments
		from ItineraryPayment payment
		left outer join ItineraryMember mem on payment.ItineraryMemberID = mem.ItineraryMemberID
		left outer join ItineraryGroup grp on mem.ItineraryGroupID = grp.ItineraryGroupID
		group by ItineraryID
	) payments on itin.ItineraryID = payments.ItineraryID;
GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Refreshing [dbo].[ItineraryServiceTypeDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryServiceTypeDetail';


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Refreshing [dbo].[ItinerarySaleDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItinerarySaleDetail';


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Refreshing [dbo].[PurchaseItemDetail]...';


GO
EXECUTE sp_refreshview N'dbo.PurchaseItemDetail';


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Refreshing [dbo].[ItinerarySaleAllocationDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItinerarySaleAllocationDetail';


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Refreshing [dbo].[ItineraryPaymentDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryPaymentDetail';


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Refreshing [dbo].[PurchaseItemPaymentsDetail]...';


GO
EXECUTE sp_refreshview N'dbo.PurchaseItemPaymentsDetail';


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Altering [dbo].[SupplierRatesDetail]...';


GO





ALTER VIEW [dbo].[SupplierRatesDetail]
AS

select 
	sup.*,
	serv.ServiceName,
	serv.[Description] as ServiceDescription,
	serv.Comments as ServiceComments,
	isnull(serv.Latitude, sup.Latitude) as ServiceLatitude,
	isnull(serv.Longitude, sup.Longitude) as ServiceLongitude,
	serv.MaxPax,
	serv.CheckinTime,
	serv.CheckoutTime,
	serv.CheckinMinutesEarly,
	serv.IsRecordActive,
	serv.CurrencyCode as ServiceCurrencyCode,
	stype.*,
	rate.ValidFrom as RateValidFrom,
	rate.ValidTo as RateValidTo,
	opt.OptionName,
	opt.Net,
	opt.Gross,	
	case when (opt.Gross != 0) then (opt.Gross - opt.Net)/opt.Gross*100 else null end as Commission,
	case when (opt.Net != 0 ) then (opt.Gross - opt.Net)/opt.Net*100 else null end as Markup,
	opt.PricingOption
from [service] serv
left outer join ServiceTypeDetail stype on serv.ServiceTypeID = stype.ServiceTypeID
left outer join SupplierDetail sup on serv.SupplierID = sup.SupplierID
right outer join Rate rate on serv.ServiceID = rate.ServiceID
right outer join [Option] opt on rate.RateID = opt.RateID
GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Altering [dbo].[ItineraryClientDetail]...';


GO



ALTER VIEW [dbo].[ItineraryClientDetail]
AS

select
	ItineraryMemberName,	
	cont.*,	
	AgeGroupName as AgeGroup,
	mem.Comments as MemberComments,
	grp.Comments as GroupComments,
	grp.NoteToClient as GroupNoteToClient,
	grp.NoteToSupplier as GroupNoteToSupplier,
	grp.CurrencyCode as GroupCurrencyCode,
	grp.CurrencyRate as GroupCurrencyRate,
	itin.ItineraryGross * grp.CurrencyRate as GroupItineraryPrice,	
	itin.*
from ItineraryMember mem
left outer join ItineraryGroup grp on mem.ItineraryGroupID = grp.ItineraryGroupID
left outer join ItineraryDetail itin on grp.ItineraryID = itin.ItineraryID
left outer join AgeGroup age on mem.AgeGroupID = age.AgeGroupID
left outer join ContactDetail cont on mem.ContactID = cont.ContactID
GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END

GO

----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2010.05.04'
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
--------------------
