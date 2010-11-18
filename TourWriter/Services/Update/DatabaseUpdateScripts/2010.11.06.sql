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
if ((select VersionNumber from AppSettings) <> '2010.10.31')
	RAISERROR (N'Update script version is invalid for this db version',17,1)	
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------
GO

PRINT N'Altering [dbo].[Itinerary]...';

GO

if not Exists(select * from sys.columns where Name = N'BaseCurrency' and Object_ID = Object_ID(N'Itinerary'))
ALTER TABLE [dbo].[Itinerary]
    ADD [BaseCurrency] CHAR (3) NULL;

GO

if not Exists(select * from sys.columns where Name = N'OutputCurrency' and Object_ID = Object_ID(N'Itinerary'))
ALTER TABLE [dbo].[Itinerary]
    ADD [OutputCurrency] CHAR (3) NULL;

GO

GO
PRINT N'Altering [dbo].[_ItinerarySet_Copy_ByID]...';


GO
/** Copy Itinerary and related records **/

ALTER PROCEDURE [dbo].[_ItinerarySet_Copy_ByID]
    @OrigItineraryID int,
    @NewItineraryName varchar(100),
    @AddedOn datetime,
    @AddedBy int
AS
SET NOCOUNT ON

DECLARE @NewItineraryID int

-- Itinerary ----------------------------------------------
INSERT [dbo].[Itinerary]
(
    [ItineraryName],
    [CustomCode],
    [ArriveDate],
    [ArriveCityID],
    [ArriveFlight],
    [ArriveNote],
    [DepartDate],
    [DepartCityID],
    [DepartFlight],
    [DepartNote],
    [NetComOrMup],
	[NetMargin],
	[GrossMarkup],
    [GrossOverride],
    [PricingNote],
    [AgentID],
    [ItineraryStatusID],
    [ItinerarySourceID],
    [CountryID],
    [AssignedTo],
    [DepartmentID],
    [BranchID],
	[PaxOverride],
    [Comments],
    [IsRecordActive],
    [ParentFolderID],
    [AddedOn],
    [AddedBy],
    [BaseCurrency],
	[OutputCurrency]
)
SELECT
    @NewItineraryName,
    CustomCode,
    ArriveDate,
    ArriveCityID,
    ArriveFlight,
    ArriveNote,
    DepartDate,
    DepartCityID,
    DepartFlight,
    DepartNote,
    NetComOrMup,
	NetMargin,
	GrossMarkup,
    GrossOverride,
    PricingNote,
    AgentID,
    ItineraryStatusID,
    ItinerarySourceID,
    CountryID,
    AssignedTo,
    DepartmentID,
    BranchID,
	PaxOverride,
    Comments,
    IsRecordActive,
    ParentFolderID,
    @AddedOn,
    @AddedBy,
    BaseCurrency,
	OutputCurrency
FROM Itinerary WHERE ItineraryID = @OrigItineraryID

SELECT @NewItineraryID=SCOPE_IDENTITY()

-- ItineraryMarginOverride ----------------------------------------------
INSERT [dbo].[ItineraryMarginOverride]
(
    [ItineraryID],
    [ServiceTypeID],
    [Margin]
)
SELECT
    @NewItineraryID,
    ServiceTypeID,
    Margin
FROM ItineraryMarginOverride WHERE ItineraryID = @OrigItineraryID

-- ItineraryPubFile ----------------------------------------------
INSERT [dbo].[ItineraryPubFile]
(
	[ItineraryPubFileName],
	[ItineraryID],
	[DayTemplateFile],
	[Layout],
	[AddedOn],
	[AddedBy]
)
SELECT
	[ItineraryPubFileName],
	@NewItineraryID,
	[DayTemplateFile],
	[Layout],
	[AddedOn],
	[AddedBy]
FROM [dbo].[ItineraryPubFile] WHERE [ItineraryID] = @OrigItineraryID

--=== Loop purchase lines ======================================
DECLARE @NewPurchaseLineID int
DECLARE @OrigPurchaseLineID int
DECLARE PurchaseLineCursor CURSOR FOR 
    SELECT PurchaseLineID FROM PurchaseLine WHERE ItineraryID = @OrigItineraryID

OPEN PurchaseLineCursor
FETCH NEXT FROM PurchaseLineCursor INTO @OrigPurchaseLineID

WHILE @@FETCH_STATUS = 0
BEGIN -- PurchaseLineCursor 
    
    INSERT [dbo].[PurchaseLine]
    (
        [ItineraryID],
        [SupplierID],
        [PurchaseLineName],
        [Comments],
        [NoteToSupplier],
        [NoteToClient],
        [Approved],
        [SupplierReference],
        [AddedOn],
        [AddedBy]
    )
    SELECT
        @NewItineraryID,
        SupplierID,
        PurchaseLineName,
        Comments,
        NoteToSupplier,
        NoteToClient,
        Approved,
        SupplierReference,
        @AddedOn,
        @AddedBy
    FROM PurchaseLine WHERE PurchaseLineID = @OrigPurchaseLineID
    
    SELECT @NewPurchaseLineID=SCOPE_IDENTITY()

    -- PurchaseItem -----------------------------------------------------------------

    INSERT [dbo].[PurchaseItem]
    (
        [PurchaseLineID],
        [OptionID],
        [BookingReference],
        [PurchaseItemName],
		[StartDate],
		[StartTime],
		[EndDate],
		[EndTime],
        [Net],
        [Gross],
        [Quantity],
        [NumberOfDays],
        [RequestStatusID],
        [AddedOn],
        [AddedBy]
    )
    SELECT
        @NewPurchaseLineID,
        OptionID,
        BookingReference,
        PurchaseItemName,
		StartDate,
		StartTime,
		EndDate,
		EndTime,
        Net,
        Gross,
        Quantity,
        NumberOfDays,
        RequestStatusID,
        @AddedOn,
        @AddedBy
    FROM PurchaseItem WHERE PurchaseLineID = @OrigPurchaseLineID

    FETCH NEXT FROM PurchaseLineCursor INTO @OrigPurchaseLineID

END -- PurchaseLineCursor
CLOSE PurchaseLineCursor
DEALLOCATE PurchaseLineCursor

-- return new itinerary id
SELECT @NewItineraryID
GO
PRINT N'Altering [dbo].[_ItinerarySet_Sel_ByID]...';


GO
ALTER PROCEDURE [dbo].[_ItinerarySet_Sel_ByID]
@ItineraryID INT
AS
SET NOCOUNT ON

DECLARE @DefaultCurrencyCode char(3) -- Used in PurchaseItem query
SET @DefaultCurrencyCode = (SELECT ISNULL(CurrencyCode,'NZD') FROM AppSettings)


-- PaymentTerm --
SELECT
	[PaymentTermID],
	[PaymentDueID],
	[PaymentDuePeriod],
	[DepositAmount],
	[DepositType],
	[DepositDueID],
	[DepositDuePeriod]
FROM [dbo].[PaymentTerm]
WHERE --*** SalePaymentTerms - do we need client or/else itinerary or/else agent? ***
	[PaymentTermID] = -- SalePaymentTerms from Itinerary
		(SELECT [PaymentTermID] FROM [Itinerary] WHERE [ItineraryID] = @ItineraryID)
OR
	[PaymentTermID] IN -- PurchasePaymentTerms from PurchaseItem
		(SELECT DISTINCT [PaymentTermID] FROM [PurchaseItem] WHERE
			[PurchaseLineID] IN ( 
				SELECT [PurchaseLineID] FROM [dbo].[PurchaseLine] 
					WHERE [ItineraryID] = @ItineraryID ))


-- *************************** Itinerary ****************************

-- Itinerary --
SELECT
	[ItineraryID],
	[ItineraryName],
	[DisplayName],
	[CustomCode],
	[ArriveDate],
	[ArriveCityID],
	[ArriveFlight],
	[ArriveNote],
	[DepartDate],
	[DepartCityID],
	[DepartFlight],
	[DepartNote],
	[NetComOrMup],
	[NetMargin],
	[GrossMarkup],
	[GrossOverride],
	[IsLockedGrossOverride],
	[PricingNote],
	[AgentID],
	[PaymentTermID],
	[ItineraryStatusID],
	[ItinerarySourceID],
	[CountryID],
	[AssignedTo],
	[DepartmentID],
	[BranchID],
	[PaxOverride],
	[Comments],
	[IsRecordActive],
	[IsReadOnly],
	[ParentFolderID],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[BaseCurrency],
	[OutputCurrency]
FROM [dbo].[Itinerary]
WHERE
	[ItineraryID] = @ItineraryID

-- ItineraryPax --
SELECT
	[ItineraryPaxID],
	[ItineraryPaxName],
	[ItineraryID],
	[MemberCount],
	[MemberRooms],
	[StaffCount],
	[StaffRooms]
FROM [dbo].[ItineraryPax]
WHERE
	[ItineraryID] = @ItineraryID
	
-- ItineraryMarginOverride --	
SELECT
	[ItineraryID],
	[ServiceTypeID],
	[Margin]
FROM [dbo].[ItineraryMarginOverride]
WHERE
	[ItineraryID] = @ItineraryID
	
-- PurchaseLine --
SELECT
	[PurchaseLineID],
	[ItineraryID],
	[SupplierID],
	[PurchaseLineName],
	[Comments],
	[NoteToSupplier],
	[NoteToVoucher],
	[NoteToClient],
	[Approved],
	[SupplierReference],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[PurchaseLine]
WHERE
	[ItineraryID] = @ItineraryID

-- PurchaseItem --
SELECT
	i.[PurchaseItemID],
	i.[PurchaseLineID],
	i.[OptionID],
	i.[PurchaseItemName],
	i.[BookingReference],
	i.[StartDate],
	i.[StartTime],
	i.[EndDate],
	i.[EndTime],
	i.[Net],
	i.[Gross],
	-- TODO, alter below to leave just i.[CurrencyRate], removing back-compat currency code from old rates table   
	isnull(i.[CurrencyRate], dbo.GetConversionRate(s.CurrencyCode, i.StartDate)) as CurrencyRate,
	i.[Quantity],
	i.[NumberOfDays],
	i.[PaymentTermID],
	i.[RequestStatusID],
	i.[IsLockedAccounting],
	i.[AddedOn],
	i.[AddedBy],
	i.[RowVersion],
	-- add lookup columns
	r.[RateID],
	s.[ServiceID],
	o.[OptionTypeID],
	s.[ServiceTypeID],
	o.[OptionName],
	s.[ServiceName],
	stype.[ServiceTypeName],
	otype.[OptionTypeName],	
	s.[ChargeType],
	o.[IsDefault] as IsDefaultOptionType,
	s.[CurrencyCode]
FROM [dbo].[PurchaseItem] i 
INNER JOIN [Option] o ON o.OptionID = i.OptionID
INNER JOIN [Rate] r ON r.RateID = o.RateID
INNER JOIN [Service] s ON s.ServiceID = r.ServiceID
left outer join ServiceType as stype on s.ServiceTypeID = stype.ServiceTypeID
left outer join OptionType as otype on o.OptionTypeID = otype.OptionTypeID
WHERE [PurchaseLineID] IN ( 
		SELECT [PurchaseLineID] FROM [dbo].[PurchaseLine] 
		WHERE [ItineraryID] = @ItineraryID )	
ORDER BY [PurchaseLineID], [StartDate]

-- PurchaseItemNote --

SELECT
	[PurchaseItemNoteID],
	[PurchaseItemID],
	[FlagID],
	[Note]
FROM [dbo].[PurchaseItemNote]
WHERE [PurchaseItemID] IN (
	SELECT [PurchaseItemID] 
	FROM [dbo].[PurchaseItem] item 
	join PurchaseLine line on line.PurchaseLineID = item.PurchaseLineID
	WHERE [ItineraryID] = @ItineraryID )	
ORDER BY [FlagID], [PurchaseItemNoteID]

-- *********************** Members ********************************************

-- Contact --		
SELECT
	[ContactID],
	[ContactName],
	[Title],
	[FirstName],
	[LastName],
	[StreetAddress],
	[PostName],
	[PostAddress],
	[CityID],
	[RegionID],
	[StateID],
	[CountryID],
	[PostCode],
	[WorkPhone],
	[HomePhone],
	[CellPhone],
	[Fax],
	[Email1],
	[Email2],
	[Website],
	[BirthDate],
	[Notes],
	[ParentFolderID],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted]
FROM [dbo].[Contact]
WHERE
	[ContactID] IN (
		SELECT c.[ContactID] 
		FROM   [dbo].[ItineraryMember] c, [dbo].[ItineraryGroup] b 
		WHERE  b.ItineraryID = @ItineraryID 
		AND    c.[ItineraryGroupID] = b.[ItineraryGroupID] )
			
-- ItineraryGroup --
SELECT
	[ItineraryGroupID],
	[ItineraryID],
	[ItineraryGroupName],
	[Comments],
	[NoteToClient],
	[NoteToSupplier],
	[CurrencyCode],
	[CurrencyRate],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[ItineraryGroup]
WHERE
	[ItineraryID] = @ItineraryID

-- ItineraryMember --
SELECT
	[ItineraryMemberID],
	[ItineraryGroupID],
	[ItineraryMemberName],
	[ContactID],
	[AgeGroupID],
	[Age],
	[ArriveDate],
	[ArriveCityID],
	[ArriveFlight],
	[DepartDate],
	[DepartCityID],
	[DepartFlight],
	[IsDefaultContact],
	[IsDefaultBilling],
	[Comments],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[ItineraryMember]
WHERE
	[ItineraryGroupID] IN ( 
		SELECT [ItineraryGroupID] FROM [dbo].[ItineraryGroup] 
		WHERE [ItineraryID] = @ItineraryID )

-- *********************** Messages *******************************************

-- Message --
SELECT
	[MessageID],
	[MessageName],
	[MessageType],
	[MessageTo],
	[MessageFrom],
	[MessageFile],
	[MessageDate],
	[Description],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[Message]
WHERE
	[MessageID] IN (
		SELECT [MessageID] FROM [dbo].[ItineraryMessage]
		WHERE [ItineraryID] = @ItineraryID)

-- ItineraryMessage --
SELECT
	[ItineraryID],
	[MessageID],
	[AddedOn],
	[AddedBy]
FROM [dbo].[ItineraryMessage]
	WHERE [ItineraryID] = @ItineraryID

-- SupplierMessage (we don't want select data, just a placeholder for inserts) --
SELECT
	[SupplierID],
	[MessageID],
	[AddedOn],
	[AddedBy]
FROM [dbo].[SupplierMessage]
WHERE [SupplierID] = NULL AND[MessageID] = null

-- *********************** Lookup Tables **********************

-- Supplier Lookup --
SELECT DISTINCT  
	[SupplierID],
	[SupplierName],
	[HostName],
	[Phone],
	[MobilePhone],
	[FreePhone],
	[Fax],
	[Email],
	[GradeID],
	[GradeExternalID],
	[StreetAddress],
	[PostAddress],
	[CityID],
	[RegionID],
	[StateID],
	[CountryID],
	[TaxTypeID]
FROM [dbo].[Supplier] 
WHERE [SupplierID] IN (
	SELECT [SupplierID] 
	FROM [dbo].[PurchaseLine] 
	WHERE[ItineraryID] = @ItineraryID )

-- Option Lookup --
SELECT DISTINCT  
	op.[OptionID],
	op.[OptionName],
	sv.[ServiceName],
	sp.[SupplierID],
	sv.[ServiceTypeID],
	op.[OptionTypeID],	
	sv.[CheckinTime],	
	sv.[CheckoutTime],
	sv.[CheckinMinutesEarly],
	sv.[CurrencyCode],
	op.[Net],
	op.[Gross],
	op.[PricingOption],
	rt.[ValidFrom],
	rt.[ValidTo]
FROM [dbo].[Option] op 
INNER JOIN [dbo].[Rate] rt     ON op.[RateID]     = rt.[RateID] 
INNER JOIN [dbo].[Service] sv  ON rt.[ServiceID]   = sv.[ServiceID]
INNER JOIN [dbo].[Supplier] sp ON sv.[SupplierID] = sp.[SupplierID]
WHERE op.[OptionID] IN (
	SELECT [OptionID] 
	FROM [dbo].[PurchaseItem] i, [dbo].[PurchaseLine] l 
	WHERE i.[PurchaseLineID] = l.[PurchaseLineID]
	AND [ItineraryID] = @ItineraryID )

-- SupplierText --
SELECT
	[SupplierTextID],
	[SupplierTextName],
	[SupplierID],
	[FileName],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[SupplierText]
WHERE [SupplierID] IN (
	SELECT [SupplierID] 
	FROM [dbo].[PurchaseLine] 
	WHERE[ItineraryID] = @ItineraryID )

-- *********************** Publising *******************************************
SELECT
	[ItineraryPubFileID],
	[ItineraryPubFileName],
	[ItineraryID],
	[DayTemplateFile],
	[Layout],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[ItineraryPubFile]
WHERE
	[ItineraryID] = @ItineraryID


-- *********************** Payments *******************************************
SELECT
	[ItineraryPaymentID],
	[ItineraryMemberID],
	[Comments],
	[Amount],
	[PaymentDate],
	[PaymentTypeID],
	[ItinerarySaleID],
	[IsLockedAccounting],
	[RowVersion]
FROM [dbo].ItineraryPayment
WHERE
	[ItineraryMemberID] IN ( 
		SELECT m.[ItineraryMemberID] 
		FROM [dbo].[ItineraryMember] m, [dbo].[ItineraryGroup] g 
		WHERE g.[ItineraryID] = @ItineraryID
		AND m.[ItineraryGroupID] = g.[ItineraryGroupID] )
ORDER BY 
	[ItineraryPaymentID] ASC

-- *********************** Sales **********************************************
SELECT
	[ItinerarySaleID],
	[ItineraryID],
	[Comments],
	[SaleDate],
	[IsLockedAccounting]
FROM [dbo].[ItinerarySale]
WHERE [ItineraryID] = @ItineraryID

SELECT
	alloc.[ItinerarySaleID],
	alloc.[ServiceTypeID],
	[Amount]
FROM [dbo].[ItinerarySaleAllocation] alloc
LEFT OUTER JOIN [dbo].[ItinerarySale] sale ON alloc.ItinerarySaleID = sale.ItinerarySaleID
WHERE sale.[ItineraryID] = @ItineraryID

-- FOCs --

SELECT
	[ServiceFocID],
	[ServiceID],
	[UnitsUsed],
	[UnitsFree]
FROM [dbo].[ServiceFoc]
WHERE
	[ServiceID] in 
	(	
		select distinct s.ServiceID
		from purchaseitem i
		inner join PurchaseLine line on i.PurchaseLineID = line.PurchaseLineID 
			and line.ItineraryID = @ItineraryID
		left outer join [Option] o on o.OptionID = i.OptionID
		left outer join [Rate] r on r.RateID = o.RateID
		left outer join [Service] s on s.ServiceID = r.ServiceID 
	)
	
-- GROUPS --		
SELECT
	o.[PurchaseItemID],
	o.[ItineraryPaxID],
	o.[MemberCount],
	o.[MemberRooms],
	o.[StaffCount],
	o.[StaffRooms]
FROM [dbo].[ItineraryPaxOverride] o
inner join PurchaseItem i on o.PurchaseItemID = i.PurchaseItemID
inner join PurchaseLine l on i.PurchaseLineID = l.PurchaseLineID
WHERE l.ItineraryID = @ItineraryID
GO
PRINT N'Altering [dbo].[Itinerary_Ins]...';


GO

ALTER PROCEDURE [dbo].[Itinerary_Ins]
	@ItineraryName varchar(255),
	@DisplayName varchar(255),
	@CustomCode varchar(50),
	@ArriveDate datetime,
	@ArriveCityID int,
	@ArriveFlight varchar(50),
	@ArriveNote varchar(2000),
	@DepartDate datetime,
	@DepartCityID int,
	@DepartFlight varchar(50),
	@DepartNote varchar(2000),
	@NetComOrMup char(3),
	@NetMargin decimal(7, 4),
	@GrossMarkup decimal(7, 4),
	@GrossOverride money,
	@IsLockedGrossOverride bit,
	@PricingNote varchar(500),
	@AgentID int,
	@PaymentTermID int,
	@ItineraryStatusID int,
	@ItinerarySourceID int,
	@CountryID int,
	@AssignedTo int,
	@DepartmentID int,
	@BranchID int,
	@PaxOverride int,
	@Comments varchar(4000),
	@IsRecordActive bit,
	@IsReadOnly bit,
	@ParentFolderID int,
	@AddedOn datetime,
	@AddedBy int,
	@IsDeleted bit,
	@BaseCurrency char(3),
	@OutputCurrency char(3),
	@ItineraryID int OUTPUT
AS
INSERT [dbo].[Itinerary]
(
	[ItineraryName],
	[DisplayName],
	[CustomCode],
	[ArriveDate],
	[ArriveCityID],
	[ArriveFlight],
	[ArriveNote],
	[DepartDate],
	[DepartCityID],
	[DepartFlight],
	[DepartNote],
	[NetComOrMup],
	[NetMargin],
	[GrossMarkup],
	[GrossOverride],
	[IsLockedGrossOverride],
	[PricingNote],
	[AgentID],
	[PaymentTermID],
	[ItineraryStatusID],
	[ItinerarySourceID],
	[CountryID],
	[AssignedTo],
	[DepartmentID],
	[BranchID],
	[PaxOverride],
	[Comments],
	[IsRecordActive],
	[IsReadOnly],
	[ParentFolderID],
	[AddedOn],
	[AddedBy],
	[IsDeleted],
	[BaseCurrency],
	[OutputCurrency]
)
VALUES
(
	ISNULL(@ItineraryName, ('')),
	@DisplayName,
	@CustomCode,
	@ArriveDate,
	@ArriveCityID,
	@ArriveFlight,
	@ArriveNote,
	@DepartDate,
	@DepartCityID,
	@DepartFlight,
	@DepartNote,
	@NetComOrMup,
	@NetMargin,
	@GrossMarkup,
	@GrossOverride,
	@IsLockedGrossOverride,
	@PricingNote,
	@AgentID,
	@PaymentTermID,
	@ItineraryStatusID,
	@ItinerarySourceID,
	@CountryID,
	@AssignedTo,
	@DepartmentID,
	@BranchID,
	@PaxOverride,
	@Comments,
	@IsRecordActive,
	@IsReadOnly,
	@ParentFolderID,
	@AddedOn,
	@AddedBy,
	@IsDeleted,
	@BaseCurrency,
	@OutputCurrency
)
SELECT @ItineraryID=SCOPE_IDENTITY()
GO
PRINT N'Altering [dbo].[Itinerary_Sel_All]...';


GO

ALTER PROCEDURE [dbo].[Itinerary_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[ItineraryID],
	[ItineraryName],
	[DisplayName],
	[CustomCode],
	[ArriveDate],
	[ArriveCityID],
	[ArriveFlight],
	[ArriveNote],
	[DepartDate],
	[DepartCityID],
	[DepartFlight],
	[DepartNote],
	[NetComOrMup],
	[NetMargin],
	[GrossMarkup],
	[GrossOverride],
	[IsLockedGrossOverride],
	[PricingNote],
	[AgentID],
	[PaymentTermID],
	[ItineraryStatusID],
	[ItinerarySourceID],
	[CountryID],
	[AssignedTo],
	[DepartmentID],
	[BranchID],
	[PaxOverride],
	[Comments],
	[IsRecordActive],
	[IsReadOnly],
	[ParentFolderID],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[BaseCurrency],
	[OutputCurrency]
FROM [dbo].[Itinerary]
ORDER BY 
	[ItineraryID] ASC
GO
PRINT N'Altering [dbo].[Itinerary_Sel_ByID]...';


GO

ALTER PROCEDURE [dbo].[Itinerary_Sel_ByID]
	@ItineraryID int
AS
SET NOCOUNT ON
SELECT
	[ItineraryID],
	[ItineraryName],
	[DisplayName],
	[CustomCode],
	[ArriveDate],
	[ArriveCityID],
	[ArriveFlight],
	[ArriveNote],
	[DepartDate],
	[DepartCityID],
	[DepartFlight],
	[DepartNote],
	[NetComOrMup],
	[NetMargin],
	[GrossMarkup],
	[GrossOverride],
	[IsLockedGrossOverride],
	[PricingNote],
	[AgentID],
	[PaymentTermID],
	[ItineraryStatusID],
	[ItinerarySourceID],
	[CountryID],
	[AssignedTo],
	[DepartmentID],
	[BranchID],
	[PaxOverride],
	[Comments],
	[IsRecordActive],
	[IsReadOnly],
	[ParentFolderID],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[BaseCurrency],
	[OutputCurrency]
FROM [dbo].[Itinerary]
WHERE
	[ItineraryID] = @ItineraryID
GO
PRINT N'Altering [dbo].[Itinerary_Upd]...';


GO

ALTER PROCEDURE [dbo].[Itinerary_Upd]
	@ItineraryID int,
	@ItineraryName varchar(255),
	@DisplayName varchar(255),
	@CustomCode varchar(50),
	@ArriveDate datetime,
	@ArriveCityID int,
	@ArriveFlight varchar(50),
	@ArriveNote varchar(2000),
	@DepartDate datetime,
	@DepartCityID int,
	@DepartFlight varchar(50),
	@DepartNote varchar(2000),
	@NetComOrMup char(3),
	@NetMargin decimal(7, 4),
	@GrossMarkup decimal(7, 4),
	@GrossOverride money,
	@IsLockedGrossOverride bit,
	@PricingNote varchar(500),
	@AgentID int,
	@PaymentTermID int,
	@ItineraryStatusID int,
	@ItinerarySourceID int,
	@CountryID int,
	@AssignedTo int,
	@DepartmentID int,
	@BranchID int,
	@PaxOverride int,
	@Comments varchar(4000),
	@IsRecordActive bit,
	@IsReadOnly bit,
	@ParentFolderID int,
	@AddedOn datetime,
	@AddedBy int,
	@RowVersion timestamp,
	@IsDeleted bit,
	@BaseCurrency char(3),
	@OutputCurrency char(3)
AS
UPDATE [dbo].[Itinerary]
SET 
	[ItineraryName] = @ItineraryName,
	[DisplayName] = @DisplayName,
	[CustomCode] = @CustomCode,
	[ArriveDate] = @ArriveDate,
	[ArriveCityID] = @ArriveCityID,
	[ArriveFlight] = @ArriveFlight,
	[ArriveNote] = @ArriveNote,
	[DepartDate] = @DepartDate,
	[DepartCityID] = @DepartCityID,
	[DepartFlight] = @DepartFlight,
	[DepartNote] = @DepartNote,
	[NetComOrMup] = @NetComOrMup,
	[NetMargin] = @NetMargin,
	[GrossMarkup] = @GrossMarkup,
	[GrossOverride] = @GrossOverride,
	[IsLockedGrossOverride] = @IsLockedGrossOverride,
	[PricingNote] = @PricingNote,
	[AgentID] = @AgentID,
	[PaymentTermID] = @PaymentTermID,
	[ItineraryStatusID] = @ItineraryStatusID,
	[ItinerarySourceID] = @ItinerarySourceID,
	[CountryID] = @CountryID,
	[AssignedTo] = @AssignedTo,
	[DepartmentID] = @DepartmentID,
	[BranchID] = @BranchID,
	[PaxOverride] = @PaxOverride,
	[Comments] = @Comments,
	[IsRecordActive] = @IsRecordActive,
	[IsReadOnly] = @IsReadOnly,
	[ParentFolderID] = @ParentFolderID,
	[AddedOn] = @AddedOn,
	[AddedBy] = @AddedBy,
	[IsDeleted] = @IsDeleted,
	[BaseCurrency] = @BaseCurrency,
	[OutputCurrency] = @OutputCurrency
WHERE
	[ItineraryID] = @ItineraryID
	AND [RowVersion] = @RowVersion
GO

PRINT N'Altering [dbo].[ItineraryPricing]...';


GO
ALTER FUNCTION [dbo].[ItineraryPricing]
( )
RETURNS TABLE 
AS
RETURN 
    (	
	select 
		ItineraryID,
		ItemCount,
		Net,
		Gross,
		case when Net = 0 then 0 else (Gross - Net)/Net*100 end as Markup,
		case when Gross = 0 then 0 else (Gross - Net)/Gross*100 end as Commission,
		Gross - Net as Yield,
		NetTax,
		GrossTax,
		GrossOrig,
		case when Net = 0 then 0 else (GrossOrig - Net)/Net*100 end as MarkupOrig,
		case when GrossOrig = 0 then 0 else (GrossOrig - Net)/GrossOrig*100 end as CommissionOrig
	from
	(
		select 
			i.ItineraryID,
			isnull(ItemCount,0) as ItemCount,
			Net, 
			case
				when (GrossOverride is not null) then GrossOverride
				when (GrossMarkup is not null) then isnull(Gross,0) * (1+GrossMarkup/100)
				else Gross
			end as Gross,
			case
				when (GrossOverride is not null) then GrossOverride
				when (GrossMarkup is not null) then isnull(GrossOrig,0) * (1+GrossMarkup/100)
				else GrossOrig
			end as GrossOrig,
			NetTax,
			GrossTax
		from Itinerary i
		left outer join
		(
			select ItineraryID, 
				count(*) as ItemCount, sum(TotalNet) as Net, sum(TotalGross) as Gross,
				sum(NetTaxAmount) as NetTax, sum(GrossTaxAmount) as GrossTax,
				sum(TotalGrossOrig) as GrossOrig
			from PurchaseItemPricing()
			group by ItineraryID
		) p on i.ItineraryID = p.ItineraryID
	) t
)
GO
PRINT N'Altering [dbo].[ItinerarySaleAllocationPricing]...';


GO
ALTER FUNCTION [dbo].[ItinerarySaleAllocationPricing]
( )
RETURNS TABLE 
AS
RETURN 
    (	
	select
		itin.ItineraryId,
		sale.ItinerarySaleID,
		stype.ServiceTypeID,
		alloc.Amount as SaleAmount,
		case when itin.Markup = -100 then 0 else
			cast(alloc.Amount*100/(100+itin.Markup) as money) end as Net,
		case when itin.Markup = -100 then 0 else
			cast(alloc.Amount - (alloc.Amount*100/(100+itin.Markup)) as money) end as Yield,		
		case when grossTax.Amount = -100 then 0 else
			cast(alloc.Amount - (alloc.Amount*100/(100+grossTax.Amount)) as money) end as TaxAmount
	from ItinerarySaleAllocation alloc
	right outer join ItinerarySale sale on alloc.ItinerarySaleID = sale.ItinerarySaleID	
	left outer join dbo.ItineraryPricing() itin on sale.ItineraryID = itin.ItineraryID
	left outer join ServiceType stype on alloc.ServiceTypeID = stype.ServiceTypeID
	left outer join TaxType as grossTax on stype.GrossTaxTypeID = grossTax.TaxTypeID
)
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
		itin.BaseCurrency as ItineraryBaseCurrency,
		itin.OutputCurrency as ItineraryOutputCurrency,
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
PRINT N'Refreshing [dbo].[PurchaseItemDetail]...';


GO
EXECUTE sp_refreshview N'dbo.PurchaseItemDetail';


GO
PRINT N'Refreshing [dbo].[ItineraryServiceTypeDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryServiceTypeDetail';


GO
PRINT N'Refreshing [dbo].[ItinerarySaleDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItinerarySaleDetail';


GO
PRINT N'Refreshing [dbo].[ItineraryClientDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryClientDetail';


GO
PRINT N'Refreshing [dbo].[ItinerarySaleAllocationDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItinerarySaleAllocationDetail';


GO
PRINT N'Refreshing [dbo].[ItineraryPaymentDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryPaymentDetail';


GO
PRINT N'Refreshing [dbo].[PurchaseItemPaymentsDetail]...';


GO
EXECUTE sp_refreshview N'dbo.PurchaseItemPaymentsDetail';


GO





----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2010.11.06'
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
