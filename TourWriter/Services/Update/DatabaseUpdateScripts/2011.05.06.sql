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
if ((select VersionNumber from AppSettings) <> '2011.04.14')
	RAISERROR (N'Database Update Script is not correct version for current database version',17,1)	

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------

GO
PRINT N'Altering [dbo].[ItineraryPax]...';


GO
if not Exists(select * from sys.columns where Name = N'GrossMarkup' and Object_ID = Object_ID(N'ItineraryPax'))
	ALTER TABLE [dbo].[ItineraryPax]
		ADD [GrossMarkup]   DECIMAL (7, 4) NULL;


GO			
if not Exists(select * from sys.columns where Name = N'GrossOverride' and Object_ID = Object_ID(N'ItineraryPax'))
	ALTER TABLE [dbo].[ItineraryPax]
		ADD [GrossOverride] MONEY NULL;


GO
PRINT N'Altering [dbo].[PurchaseItem]...';


GO
if Exists(select * from sys.columns where Name = N'GstUpdated' and Object_ID = Object_ID(N'PurchaseItem'))
	ALTER TABLE [dbo].[PurchaseItem] DROP COLUMN [GstUpdated];


GO
if not Exists(select * from sys.columns where Name = N'DiscountUnits' and Object_ID = Object_ID(N'PurchaseItem'))
	ALTER TABLE [dbo].[PurchaseItem]
		ADD [DiscountUnits] DECIMAL (12, 4) NULL;


GO
if not Exists(select * from sys.columns where Name = N'DiscountType' and Object_ID = Object_ID(N'PurchaseItem'))
	ALTER TABLE [dbo].[PurchaseItem]
		ADD [DiscountType]  VARCHAR (10) NULL;


GO
PRINT N'Altering [dbo].[_ItinerarySet_Sel_ByID]...';


GO
ALTER PROCEDURE [dbo].[_ItinerarySet_Sel_ByID]
@ItineraryID INT
AS
SET NOCOUNT ON

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
	[CurrencyCode]
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
	[StaffRooms],
	[GrossMarkup],
	[GrossOverride]
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
	i.[CurrencyRate],
	i.[Quantity],
	i.[NumberOfDays],
	i.[PaymentTermID],
	i.[RequestStatusID],
	i.[IsLockedAccounting],
	i.[AddedOn],
	i.[AddedBy],
	i.[RowVersion],
	i.[DiscountUnits],
	i.[DiscountType],
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
	[NetTaxTypeID],
	[GrossTaxTypeID]
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
PRINT N'Altering [dbo].[ItineraryPax_Ins]...';


GO

ALTER PROCEDURE [dbo].[ItineraryPax_Ins]
	@ItineraryPaxName varchar(50),
	@ItineraryID int,
	@MemberCount int,
	@MemberRooms decimal(8, 4),
	@StaffCount int,
	@StaffRooms decimal(8, 4),
	@GrossMarkup decimal(7, 4),
	@GrossOverride money,
	@ItineraryPaxID int OUTPUT
AS
INSERT [dbo].[ItineraryPax]
(
	[ItineraryPaxName],
	[ItineraryID],
	[MemberCount],
	[MemberRooms],
	[StaffCount],
	[StaffRooms],
	[GrossMarkup],
	[GrossOverride]
)
VALUES
(
	@ItineraryPaxName,
	@ItineraryID,
	@MemberCount,
	@MemberRooms,
	@StaffCount,
	@StaffRooms,
	@GrossMarkup,
	@GrossOverride
)
SELECT @ItineraryPaxID=SCOPE_IDENTITY()
GO
PRINT N'Altering [dbo].[ItineraryPax_Sel_All]...';


GO

ALTER PROCEDURE [dbo].[ItineraryPax_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[ItineraryPaxID],
	[ItineraryPaxName],
	[ItineraryID],
	[MemberCount],
	[MemberRooms],
	[StaffCount],
	[StaffRooms],
	[GrossMarkup],
	[GrossOverride]
FROM [dbo].[ItineraryPax]
ORDER BY 
	[ItineraryPaxID] ASC
GO
PRINT N'Altering [dbo].[ItineraryPax_Sel_ByID]...';


GO

ALTER PROCEDURE [dbo].[ItineraryPax_Sel_ByID]
	@ItineraryPaxID int
AS
SET NOCOUNT ON
SELECT
	[ItineraryPaxID],
	[ItineraryPaxName],
	[ItineraryID],
	[MemberCount],
	[MemberRooms],
	[StaffCount],
	[StaffRooms],
	[GrossMarkup],
	[GrossOverride]
FROM [dbo].[ItineraryPax]
WHERE
	[ItineraryPaxID] = @ItineraryPaxID
GO
PRINT N'Altering [dbo].[ItineraryPax_Sel_ByItineraryID]...';


GO

ALTER PROCEDURE [dbo].[ItineraryPax_Sel_ByItineraryID]
	@ItineraryID int
AS
SET NOCOUNT ON
SELECT
	[ItineraryPaxID],
	[ItineraryPaxName],
	[ItineraryID],
	[MemberCount],
	[MemberRooms],
	[StaffCount],
	[StaffRooms],
	[GrossMarkup],
	[GrossOverride]
FROM [dbo].[ItineraryPax]
WHERE
	[ItineraryID] = @ItineraryID
GO
PRINT N'Altering [dbo].[ItineraryPax_Upd]...';


GO

ALTER PROCEDURE [dbo].[ItineraryPax_Upd]
	@ItineraryPaxID int,
	@ItineraryPaxName varchar(50),
	@ItineraryID int,
	@MemberCount int,
	@MemberRooms decimal(8, 4),
	@StaffCount int,
	@StaffRooms decimal(8, 4),
	@GrossMarkup decimal(7, 4),
	@GrossOverride money
AS
UPDATE [dbo].[ItineraryPax]
SET 
	[ItineraryPaxName] = @ItineraryPaxName,
	[ItineraryID] = @ItineraryID,
	[MemberCount] = @MemberCount,
	[MemberRooms] = @MemberRooms,
	[StaffCount] = @StaffCount,
	[StaffRooms] = @StaffRooms,
	[GrossMarkup] = @GrossMarkup,
	[GrossOverride] = @GrossOverride
WHERE
	[ItineraryPaxID] = @ItineraryPaxID
GO
PRINT N'Altering [dbo].[PurchaseItem_Ins]...';


GO

ALTER PROCEDURE [dbo].[PurchaseItem_Ins]
	@PurchaseLineID int,
	@OptionID int,
	@PurchaseItemName varchar(255),
	@BookingReference varchar(50),
	@StartDate datetime,
	@StartTime datetime,
	@EndDate datetime,
	@EndTime datetime,
	@Net money,
	@Gross money,
	@CurrencyRate money,
	@Quantity decimal(12, 4),
	@NumberOfDays decimal(12, 4),
	@PaymentTermID int,
	@RequestStatusID int,
	@IsLockedAccounting bit,
	@AddedOn datetime,
	@AddedBy int,
	@DiscountUnits decimal(12, 4),
	@DiscountType varchar(10),
	@PurchaseItemID int OUTPUT
AS
INSERT [dbo].[PurchaseItem]
(
	[PurchaseLineID],
	[OptionID],
	[PurchaseItemName],
	[BookingReference],
	[StartDate],
	[StartTime],
	[EndDate],
	[EndTime],
	[Net],
	[Gross],
	[CurrencyRate],
	[Quantity],
	[NumberOfDays],
	[PaymentTermID],
	[RequestStatusID],
	[IsLockedAccounting],
	[AddedOn],
	[AddedBy],
	[DiscountUnits],
	[DiscountType]
)
VALUES
(
	@PurchaseLineID,
	@OptionID,
	@PurchaseItemName,
	@BookingReference,
	@StartDate,
	@StartTime,
	@EndDate,
	@EndTime,
	@Net,
	@Gross,
	@CurrencyRate,
	@Quantity,
	@NumberOfDays,
	@PaymentTermID,
	@RequestStatusID,
	@IsLockedAccounting,
	@AddedOn,
	@AddedBy,
	@DiscountUnits,
	@DiscountType
)
SELECT @PurchaseItemID=SCOPE_IDENTITY()
GO
PRINT N'Altering [dbo].[PurchaseItem_Sel_All]...';


GO

ALTER PROCEDURE [dbo].[PurchaseItem_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[PurchaseItemID],
	[PurchaseLineID],
	[OptionID],
	[PurchaseItemName],
	[BookingReference],
	[StartDate],
	[StartTime],
	[EndDate],
	[EndTime],
	[Net],
	[Gross],
	[CurrencyRate],
	[Quantity],
	[NumberOfDays],
	[PaymentTermID],
	[RequestStatusID],
	[IsLockedAccounting],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[DiscountUnits],
	[DiscountType]
FROM [dbo].[PurchaseItem]
ORDER BY 
	[PurchaseItemID] ASC
GO
PRINT N'Altering [dbo].[PurchaseItem_Sel_ByID]...';


GO

ALTER PROCEDURE [dbo].[PurchaseItem_Sel_ByID]
	@PurchaseItemID int
AS
SET NOCOUNT ON
SELECT
	[PurchaseItemID],
	[PurchaseLineID],
	[OptionID],
	[PurchaseItemName],
	[BookingReference],
	[StartDate],
	[StartTime],
	[EndDate],
	[EndTime],
	[Net],
	[Gross],
	[CurrencyRate],
	[Quantity],
	[NumberOfDays],
	[PaymentTermID],
	[RequestStatusID],
	[IsLockedAccounting],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[DiscountUnits],
	[DiscountType]
FROM [dbo].[PurchaseItem]
WHERE
	[PurchaseItemID] = @PurchaseItemID
GO
PRINT N'Altering [dbo].[PurchaseItem_Sel_ByOptionID]...';


GO

ALTER PROCEDURE [dbo].[PurchaseItem_Sel_ByOptionID]
	@OptionID int
AS
SET NOCOUNT ON
SELECT
	[PurchaseItemID],
	[PurchaseLineID],
	[OptionID],
	[PurchaseItemName],
	[BookingReference],
	[StartDate],
	[StartTime],
	[EndDate],
	[EndTime],
	[Net],
	[Gross],
	[CurrencyRate],
	[Quantity],
	[NumberOfDays],
	[PaymentTermID],
	[RequestStatusID],
	[IsLockedAccounting],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[DiscountUnits],
	[DiscountType]
FROM [dbo].[PurchaseItem]
WHERE
	[OptionID] = @OptionID
GO
PRINT N'Altering [dbo].[PurchaseItem_Sel_ByPurchaseLineID]...';


GO

ALTER PROCEDURE [dbo].[PurchaseItem_Sel_ByPurchaseLineID]
	@PurchaseLineID int
AS
SET NOCOUNT ON
SELECT
	[PurchaseItemID],
	[PurchaseLineID],
	[OptionID],
	[PurchaseItemName],
	[BookingReference],
	[StartDate],
	[StartTime],
	[EndDate],
	[EndTime],
	[Net],
	[Gross],
	[CurrencyRate],
	[Quantity],
	[NumberOfDays],
	[PaymentTermID],
	[RequestStatusID],
	[IsLockedAccounting],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[DiscountUnits],
	[DiscountType]
FROM [dbo].[PurchaseItem]
WHERE
	[PurchaseLineID] = @PurchaseLineID
GO
PRINT N'Altering [dbo].[PurchaseItem_Upd]...';


GO

ALTER PROCEDURE [dbo].[PurchaseItem_Upd]
	@PurchaseItemID int,
	@PurchaseLineID int,
	@OptionID int,
	@PurchaseItemName varchar(255),
	@BookingReference varchar(50),
	@StartDate datetime,
	@StartTime datetime,
	@EndDate datetime,
	@EndTime datetime,
	@Net money,
	@Gross money,
	@CurrencyRate money,
	@Quantity decimal(12, 4),
	@NumberOfDays decimal(12, 4),
	@PaymentTermID int,
	@RequestStatusID int,
	@IsLockedAccounting bit,
	@AddedOn datetime,
	@AddedBy int,
	@RowVersion timestamp,
	@DiscountUnits decimal(12, 4),
	@DiscountType varchar(10)
AS
UPDATE [dbo].[PurchaseItem]
SET 
	[PurchaseLineID] = @PurchaseLineID,
	[OptionID] = @OptionID,
	[PurchaseItemName] = @PurchaseItemName,
	[BookingReference] = @BookingReference,
	[StartDate] = @StartDate,
	[StartTime] = @StartTime,
	[EndDate] = @EndDate,
	[EndTime] = @EndTime,
	[Net] = @Net,
	[Gross] = @Gross,
	[CurrencyRate] = @CurrencyRate,
	[Quantity] = @Quantity,
	[NumberOfDays] = @NumberOfDays,
	[PaymentTermID] = @PaymentTermID,
	[RequestStatusID] = @RequestStatusID,
	[IsLockedAccounting] = @IsLockedAccounting,
	[AddedOn] = @AddedOn,
	[AddedBy] = @AddedBy,
	[DiscountUnits] = @DiscountUnits,
	[DiscountType] = @DiscountType
WHERE
	[PurchaseItemID] = @PurchaseItemID
	AND [RowVersion] = @RowVersion
GO
PRINT N'Altering [dbo].[PurchaseItemTaxType]...';


GO
ALTER FUNCTION [dbo].[PurchaseItemTaxType]
( )
RETURNS TABLE 
AS
RETURN 
(	
	select 
		PurchaseItemID,		
		case when ntaxSrv.TaxTypeID is not null then ntaxSrv.TaxTypeCode -- overridden in Service
			 when ntaxSup.TaxTypeID is not null then ntaxSup.TaxTypeCode -- or overridden in Supplier
			 when ntaxTyp.TaxTypeID is not null then ntaxTyp.TaxTypeCode -- or default, from ServiceType
		end as NetTaxCode,	
		case when ntaxSrv.TaxTypeID is not null then ntaxSrv.Amount
			 when ntaxSup.TaxTypeID is not null then ntaxSup.Amount 
			 when ntaxTyp.TaxTypeID is not null then ntaxTyp.Amount
		end as NetTaxPercent,
		case when gtaxSrv.TaxTypeID is not null then gtaxSrv.TaxTypeCode
			 when gtaxSup.TaxTypeID is not null then gtaxSup.TaxTypeCode
			 when gtaxTyp.TaxTypeID is not null then gtaxTyp.TaxTypeCode
		end as GrossTaxCode,	
		case when gtaxSrv.TaxTypeID is not null then gtaxSrv.Amount
			 when gtaxSup.TaxTypeID is not null then gtaxSup.Amount 
			 when gtaxTyp.TaxTypeID is not null then gtaxTyp.Amount
		end as GrossTaxPercent		
	from PurchaseItem item
	left join [Option] as opt on item.OptionID = opt.OptionID
	left join Rate as rate on opt.RateID = rate.RateID
	left join [Service] as serv on rate.ServiceID = serv.ServiceID
	left join [Supplier] as sup on serv.SupplierID = sup.SupplierID
	left join ServiceType as stype on serv.ServiceTypeID = stype.ServiceTypeID	
	
	left join TaxType as ntaxSrv on ntaxSrv.TaxTypeID = serv.NetTaxTypeID
	left join TaxType as ntaxSup on ntaxSup.TaxTypeID = sup.NetTaxTypeID
	left join TaxType as ntaxTyp on ntaxTyp.TaxTypeID = stype.NetTaxTypeID
	
	left join TaxType as gtaxSrv on gtaxSrv.TaxTypeID = serv.GrossTaxTypeID
	left join TaxType as gtaxSup on gtaxSup.TaxTypeID = sup.GrossTaxTypeID
	left join TaxType as gtaxTyp on gtaxTyp.TaxTypeID = stype.GrossTaxTypeID
)
GO
PRINT N'Altering [dbo].[PurchaseItemPricing]...';


GO
ALTER FUNCTION [dbo].[PurchaseItemPricing]
( )
RETURNS TABLE 
AS
RETURN 
    (	
	select 
		t.*,		
		case when NetBaseUnit = 0 then 0 else (GrossBaseUnit - NetBaseUnit)/NetBaseUnit*100 end as Markup,
		case when GrossBaseUnit = 0 then 0 else (GrossBaseUnit - NetBaseUnit)/GrossBaseUnit*100 end as Commission,
		NetTaxCode as NetTaxTypeCode,
		NetTaxPercent,
		NetBaseTotal - (NetBaseTotal*100/(100+NetTaxPercent)) as NetBaseTotalTaxAmount,		
		GrossTaxCode as GrossTaxTypeCode,
		GrossTaxPercent,		
		GrossFinalTotal - (GrossFinalTotal*100/(100+GrossTaxPercent)) as GrossFinalTotalTaxAmount
	from
	(
		select 
			ItineraryID, 
			PurchaseItemID,
			ServiceTypeID,
			UnitMultiplier,	
			CurrencyRate,			
			Net as NetBaseUnit,
			Net * UnitMultiplier as NetBaseTotal,
			Net * UnitMultiplier * CurrencyRate as NetFinalTotal,	
			GrossOrig as GrossBaseUnitPreAdjustment,	
			GrossAdjusted as GrossBaseUnit,
			GrossAdjusted * UnitMultiplier as GrossBaseTotal,
			GrossAdjusted * UnitMultiplier * CurrencyRate as GrossFinalTotal	
		from
		(
			select 		 
				itin.ItineraryID, 
				item.PurchaseItemID,
				serv.ServiceTypeID,
				item.Net,
				/* adjust gross with itinerary net level overrides */
				case when itin.NetComOrMup = 'com' -- adjust commission
					then 
					(
						isnull(item.Net*100/(100-itin.NetMargin), -- per itinerary override
						isnull(item.Net*100/(100-stype.Margin),   -- or per itinerary service type override
						item.Gross))                              -- or no override
					)
					else -- adjust markup
					(
						isnull(item.Net*(1+itin.NetMargin/100), 
						isnull(item.Net*(1+stype.Margin/100), 
						item.Gross))
					) 
					end as GrossAdjusted,
				item.Gross as GrossOrig,
				isnull(item.Quantity, 1) * isnull(item.NumberOfDays, 1) as UnitMultiplier,
				isnull(item.CurrencyRate, 1) as CurrencyRate
			from PurchaseItem item
			left outer join PurchaseLine as line on line.PurchaseLineID = item.PurchaseLineID
			left outer join Itinerary as itin on line.ItineraryID = itin.ItineraryID 
			left outer join [Option] as opt on item.OptionID = opt.OptionID
			left outer join Rate as rate on opt.RateID = rate.RateID
			left outer join [Service] AS serv on rate.ServiceID = serv.ServiceID 
			left outer join ItineraryMarginOverride as stype on itin.ItineraryID = stype.ItineraryID and serv.ServiceTypeID = stype.ServiceTypeID
		) x
	) t
	left outer join PurchaseItemTaxType() as tax on t.PurchaseItemID = tax.PurchaseItemID
)
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
		NetFinalTotal,
		GrossFinalTotal,
		case when NetFinalTotal = 0 then 0 else (GrossFinalTotal - NetFinalTotal)/NetFinalTotal*100 end as Markup,
		case when GrossFinalTotal = 0 then 0 else (GrossFinalTotal - NetFinalTotal)/GrossFinalTotal*100 end as Commission,	    
		GrossFinalTotal - NetFinalTotal as Yield,
		GrossFinalTotalTaxAmount
	from
	(
		select 
			i.ItineraryID,
			isnull(ItemCount,0) as ItemCount,
			NetFinalTotal, 
			/* adjust final gross with itinerary gross level overrides */
			case
				when (GrossOverride is not null) then GrossOverride									-- final $ override
				when (GrossMarkup is not null) then isnull(GrossFinalTotal,0) * (1+GrossMarkup/100)	-- *(already applied?) final % markup
				else GrossFinalTotal																-- sum of purchase items
			end as GrossFinalTotal,
			GrossFinalTotalTaxAmount
		from Itinerary i
		left outer join
		(
			select ItineraryID, 
				count(*) as ItemCount, sum(NetFinalTotal) as NetFinalTotal, sum(GrossFinalTotal) as GrossFinalTotal,
				sum(GrossFinalTotalTaxAmount) as GrossFinalTotalTaxAmount
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
PRINT N'Refreshing [dbo].[ItineraryDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryDetail';


GO
PRINT N'Refreshing [dbo].[ItinerarySaleAllocationDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItinerarySaleAllocationDetail';


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
PRINT N'Refreshing [dbo].[ItineraryPaymentDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryPaymentDetail';


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
		isnull(serv.CurrencyCode, (select top(1) CurrencyCode from Appsettings)) as CurrencyCode,
		isnull(itemCcy.DisplayFormat, sysCcy.DisplayFormat)as CurrencyFormat,  
		item.CurrencyRate,
		item.NumberOfDays,
		item.Quantity,
		pric.UnitMultiplier,
		pric.NetBaseUnit, -- as BaseNet,
		pric.NetBaseTotal, -- * UnitMultiplier as BaseTotalNet,
		pric.NetFinalTotal, --TotalNet as FinalTotalNet,
		pric.GrossBaseUnitPreAdjustment,
		pric.GrossBaseUnit, -- as BaseGross,
		pric.GrossBaseTotal, -- * UnitMultiplier as BaseTotalGross,
		pric.GrossFinalTotal, --TotalGross as FinalTotalGross,
		pric.Markup,
		pric.Commission,
		--pric.Yield,
	    pric.NetBaseTotalTaxAmount,
	    pric.GrossFinalTotalTaxAmount,
		NetBaseTotal - NetBaseTotalTaxAmount as NetBaseTotalExcl,
		GrossFinalTotal - GrossFinalTotalTaxAmount as GrossFinalTotalExcl,
		stype.ServiceTypeID,
		stype.ServiceTypeName,
		stype.BookingStartName,
		stype.BookingEndName,
		stype.NumberOfDaysName,
		pric.NetTaxTypeCode,
		pric.NetTaxPercent,
		stype.NetAccountingCategoryCode,
		pric.GrossTaxTypeCode,
		pric.GrossTaxPercent,
		stype.GrossAccountingCategoryCode,
		convert(varchar(3), PaymentTerm.DepositDuePeriod) + ' ' + depdue.PaymentDueName as DepositTerms,
		dbo.GetPaymentDate(item.StartDate, dbo.PaymentTerm.DepositDuePeriod, depdue.PaymentDueName) as DepositDueDate,
		PaymentTerm.DepositType,
		PaymentTerm.DepositAmount,
		convert(varchar(3), PaymentTerm.PaymentDuePeriod) + ' ' + baldue.PaymentDueName as BalanceTerms,
		dbo.GetPaymentDate(item.StartDate, PaymentTerm.PaymentDuePeriod, baldue.PaymentDueName) as BalanceDueDate
		
		----/**** only for backwards compatibility ****/
		--,pric.NetBaseUnit as Net
		--,pric.GrossBaseUnit as Gross
		--,pric.NetFinalTotal as TotalNet
		--,pric.GrossFinalTotal as TotalGross
		--,pric.GrossBaseUnit * pric.UnitMultiplier * pric.CurrencyRate as TotalGrossOrig
		--,NetBaseTotal - NetBaseTotalTaxAmount as TotalNetExcl
		----/******************************************/
		
	from PurchaseItem item
	join dbo.PurchaseItemPricing() pric on item.PurchaseItemID = pric.PurchaseItemID
	left outer join PurchaseLine as line on line.PurchaseLineID = item.PurchaseLineID
	left outer join ItineraryDetail as itin on line.ItineraryID = itin.ItineraryID
	left outer join Itinerary as itin2 on line.ItineraryID = itin2.ItineraryID
	left outer join RequestStatus req on req.RequestStatusID = item.RequestStatusID
	left outer join [Option] as opt on item.OptionID = opt.OptionID
	left outer join Rate as rate on opt.RateID = rate.RateID
	left outer join [Service] as serv on rate.ServiceID = serv.ServiceID
	left outer join SupplierDetail sup on serv.SupplierID = sup.SupplierID
	left outer join ServiceTypeDetail as stype on serv.ServiceTypeID = stype.ServiceTypeID
	left outer join PaymentDue as baldue
	right outer join PaymentTerm on baldue.PaymentDueID = PaymentTerm.PaymentDueID
	left outer join PaymentDue as depdue on PaymentTerm.DepositDueID = depdue.PaymentDueID on item.PaymentTermID = PaymentTerm.PaymentTermID
	left outer join Currency itemCcy on itemCcy.CurrencyCode = serv.CurrencyCode COLLATE DATABASE_DEFAULT
	left outer join Currency sysCcy on sysCcy.CurrencyCode = (select top(1) CurrencyCode from AppSettings) COLLATE DATABASE_DEFAULT
GO
PRINT N'Refreshing [dbo].[PurchaseItemPaymentsDetail]...';


GO
EXECUTE sp_refreshview N'dbo.PurchaseItemPaymentsDetail';


GO

----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2011.05.06'
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
