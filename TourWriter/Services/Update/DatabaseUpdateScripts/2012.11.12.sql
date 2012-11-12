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
if ((select VersionNumber from AppSettings) <> '2012.10.27')
	RAISERROR (N'Database Update Script is not correct version for current database version',17,1)

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------

GO
PRINT N'Altering [dbo].[Itinerary]...';


GO
if not Exists(select * from sys.columns where Name = N'UpdatedOn' and Object_ID = Object_ID(N'Itinerary'))
	ALTER TABLE [dbo].[Itinerary] 
		ADD [UpdatedOn] datetime;
		
GO
if not Exists(select * from sys.columns where Name = N'AllocationsItineraryID' and Object_ID = Object_ID(N'Itinerary'))
	ALTER TABLE [dbo].[Itinerary] 
		ADD [AllocationsItineraryID] int;
GO

GO
PRINT N'Altering [dbo].[Supplier]...';


GO
if not Exists(select * from sys.columns where Name = N'UpdatedOn' and Object_ID = Object_ID(N'Supplier'))
	ALTER TABLE [dbo].[Supplier] 
		ADD [UpdatedOn] datetime;
GO
-------------------------------------------------------------------------------------------------------------------------------------------------------
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
	[CurrencyCode],
	[NetMinOrMax],
	[AgentContactID],
	[UpdatedOn],
	[AllocationsItineraryID]
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
	i.[IsInvoiced],
	i.[SortDate],
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
	[IsDeleted],
	[JobDescription],
	[PassportNumber],
	[PassportExpiry]
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
	[RowVersion],
	[Title],
	[RoomTypeID],
	[AgentID],
	[PriceOverride],
	[RoomName]
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

-- Discounts --

SELECT
	[DiscountID],
	[ServiceID],
	[UnitsUsed],
	[UnitsFree],
	[DiscountType]
FROM [dbo].[Discount]
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

-- ServiceWarnings
SELECT [ServiceWarningID]
		,[ServiceID]
		,[Description]
		,[ValidFrom]
		,[ValidTo]
	FROM [ServiceWarning]
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
AND ValidTo > getdate()

-- TASKS --
SELECT
	t.[TaskID],
	[TaskName],
	[DateDue],
	[DateCompleted],
	[Note]
FROM [dbo].[Task] t
inner join ItineraryTask i on i.TaskID = t.TaskID
where i.ItineraryID = @ItineraryID

-- ItineraryTask --
SELECT
	[ItineraryTaskID],
	[ItineraryID],
	[TaskID]
FROM [dbo].[ItineraryTask]
where ItineraryID = @ItineraryID

-- RoomType --
SELECT
	[RoomTypeID],
	[ItineraryID],
	[OptionTypeID],
	[RoomTypeName],
	[Quantity]
FROM [dbo].[RoomType]
where ItineraryID = @ItineraryID

-- Allocations
SELECT [AllocationID]
      ,[ServiceID]
      ,[ItineraryID]
      ,[ValidFrom]
      ,[ValidTo]
      ,[Quantity]
      ,[Release]
  FROM [Allocation]
  where ItineraryID = @ItineraryID

SELECT [AllocationID]
      ,[AgentID]
      ,[Quantity]
      ,[Release]
  FROM [AllocationAgent]
  WHERE [AllocationID] IN ( 
    SELECT [AllocationID] FROM [dbo].[Allocation] a
    where ItineraryID = @ItineraryID )
GO


ALTER PROCEDURE [dbo].[_SupplierSet_Sel_ByID]
@SupplierID INT
AS
SET NOCOUNT ON

/* Select parent tables first */

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
WHERE
	[PaymentTermID] = -- PurchasePaymentTerms from Supplier
		(SELECT [PaymentTermID] FROM [Supplier] WHERE [SupplierID] = @SupplierID)
OR
	[PaymentTermID] IN -- PurchasePaymentTerms from Service
		(SELECT DISTINCT [PaymentTermID] FROM [Service] WHERE
			[ServiceID] IN ( 
				SELECT [ServiceID] FROM [dbo].[Service] 
					WHERE [SupplierID] = @SupplierID ))

-- Supplier --
SELECT
	[SupplierID],
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
	[NetTaxTypeID],
	[GrossTaxTypeID],
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
	[RowVersion],
	[IsDeleted],
	[UpdatedOn]
FROM [dbo].[Supplier]
WHERE
	[SupplierID] = @SupplierID
			
-- Service --
SELECT
	[ServiceID],
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
	[NetTaxTypeID],
	[GrossTaxTypeID],
	[Latitude],
	[Longitude],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[ImportID]
FROM [dbo].[Service]
WHERE
	[SupplierID] = @SupplierID
	
-- ServiceTime
SELECT
	[ServiceTimeID],
	[ServiceID],
	[StartTime],
	[EndTime],
	[Comment]
FROM [dbo].[ServiceTime]
WHERE 
	[ServiceID] IN ( 
		SELECT [ServiceID] FROM [dbo].[Service] 
		WHERE [SupplierID] = @SupplierID )

-- ServiceConfig --
SELECT
	[ServiceID],
	[ServiceConfigTypeID],
	[Note],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[ServiceConfig]
WHERE 
	[ServiceID] IN ( 
		SELECT [ServiceID] FROM [dbo].[Service] 
		WHERE [SupplierID] = @SupplierID )
	
-- Rate --	
SELECT
	[RateID],
	[ServiceID],
	[ValidFrom],
	[ValidTo],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[ImportID]
FROM [dbo].[Rate]	
WHERE 
	[ServiceID] IN ( 
		SELECT [ServiceID] FROM [dbo].[Service] 
		WHERE [SupplierID] = @SupplierID )
	
-- Option --
SELECT
	[OptionID],
	[OptionName],
	[OptionTypeID],
	[RateID],
	[Net],
	[Gross],
	[PricingOption],
	isnull([IsDefault], 0) as [IsDefault],
	[IsRecordActive],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[ImportID]
FROM [dbo].[Option]	
WHERE 
	[RateID] IN (		
		SELECT r.[RateID] 
		FROM   [dbo].[Rate] r, [dbo].[Service] s 
		WHERE  s.[SupplierID] = @SupplierID 
		AND    r.[ServiceID] = s.[ServiceID] )
			
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
	[IsDeleted],
	[JobDescription],
	[PassportNumber],
	[PassportExpiry]
FROM [dbo].[Contact]
WHERE
	[ContactID] IN (
		SELECT [ContactID] FROM [dbo].[SupplierContact] 
		WHERE [SupplierID] = @SupplierID )

-- SupplierContacts --			
SELECT
	[SupplierID],
	[ContactID],
	[Description],
	[IsDefaultContact],
	[IsDefaultBilling],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[SupplierContact]
WHERE
	[SupplierID] = @SupplierID
	
-- SupplierCreditCard --		
SELECT
	[SupplierID],
	[CreditCardID],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[SupplierCreditCard]
WHERE
	[SupplierID] = @SupplierID

-- SupplierConfig --		
SELECT
	[SupplierID],
	[SupplierConfigTypeID],
	[Note],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[SupplierConfig]
WHERE
	[SupplierID] = @SupplierID

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
		SELECT [MessageID] FROM [dbo].[SupplierMessage]
		WHERE [SupplierID] = @SupplierID)

-- SupplierMessage --
SELECT
	[SupplierID],
	[MessageID],
	[AddedOn],
	[AddedBy]
FROM [dbo].[SupplierMessage]
WHERE [SupplierID] = @SupplierID

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
WHERE
	[SupplierID] = @SupplierID

-- SupplierNote --
SELECT
	[SupplierNoteID],
	[SupplierID],
	[Note],
	[ShowOnReport],
	[RowVersion]
FROM [dbo].[SupplierNote]
WHERE
	[SupplierID] = @SupplierID
ORDER BY 
	SupplierNoteID

-- Discount --
SELECT
	[DiscountID],
	[ServiceID],
	[UnitsUsed],
	[UnitsFree],
	[DiscountType]
FROM [dbo].[Discount]
WHERE
	[ServiceID] IN ( 
		SELECT [ServiceID] FROM [dbo].[Service] 
		WHERE [SupplierID] = @SupplierID )
		
-- Content
SELECT c.* 
FROM [Content] c
inner join SupplierContent s on s.ContentID = c.ContentID
WHERE s.SupplierID = @SupplierID
UNION
SELECT c.* 
FROM [Content] c
inner join ServiceContent s on s.ContentID = c.ContentID
where ServiceID in (select ServiceID from [Service] where SupplierID = @SupplierID)

-- SupplierContent
select * from SupplierContent
where SupplierID = @SupplierID

-- ServiceContent
select * from ServiceContent
where ServiceID in (select ServiceID from [Service] where SupplierID = @SupplierID)

-- ServiceWarning
select * from ServiceWarning
where ServiceID in (select ServiceID from [Service] where SupplierID = @SupplierID)

-- Allocations
SELECT [AllocationID]
      ,[ServiceID]
      ,[ItineraryID]
      ,[ValidFrom]
      ,[ValidTo]
      ,[Quantity]
      ,[Release]
  FROM [Allocation]
  WHERE [ServiceID] IN ( 
    SELECT [ServiceID] FROM [dbo].[Service] 
	WHERE [SupplierID] = @SupplierID )

SELECT [AllocationID]
      ,[AgentID]
      ,[Quantity]
      ,[Release]
  FROM [AllocationAgent]
  WHERE [AllocationID] IN ( 
    SELECT [AllocationID] FROM [dbo].[Allocation] a
    LEFT JOIN [Service] s on s.ServiceID = a.ServiceID
	WHERE [SupplierID] = @SupplierID )

SELECT [AllocationID]
      ,[OptionTypeID]
      ,[AddedOn]
  FROM [AllocationOption]
  WHERE [AllocationID] IN ( 
    SELECT [AllocationID] FROM [dbo].[Allocation] a
    LEFT JOIN [Service] s on s.ServiceID = a.ServiceID
	WHERE [SupplierID] = @SupplierID )
GO



-------------------------------------------------------------------------------------------------------------------------------------------------------

GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Itinerary_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Itinerary_Ins]
GO

CREATE PROCEDURE [dbo].[Itinerary_Ins]
	@ItineraryName varchar(255),
	@DisplayName varchar(255),
	@CustomCode varchar(50),
	@ArriveDate datetime,
	@ArriveCityID int,
	@ArriveFlight varchar(50),
	@ArriveNote varchar(4000),
	@DepartDate datetime,
	@DepartCityID int,
	@DepartFlight varchar(50),
	@DepartNote varchar(4000),
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
	@Comments varchar(8000),
	@IsRecordActive bit,
	@IsReadOnly bit,
	@ParentFolderID int,
	@AddedOn datetime,
	@AddedBy int,
	@IsDeleted bit,
	@CurrencyCode char(3),
	@NetMinOrMax varchar(10),
	@AgentContactID int,
	@UpdatedOn datetime,
	@AllocationsItineraryID int,
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
	[CurrencyCode],
	[NetMinOrMax],
	[AgentContactID],
	[UpdatedOn],
	[AllocationsItineraryID]
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
	@CurrencyCode,
	@NetMinOrMax,
	@AgentContactID,
	@UpdatedOn,
	@AllocationsItineraryID
)
SELECT @ItineraryID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Itinerary_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Itinerary_Upd]
GO

CREATE PROCEDURE [dbo].[Itinerary_Upd]
	@ItineraryID int,
	@ItineraryName varchar(255),
	@DisplayName varchar(255),
	@CustomCode varchar(50),
	@ArriveDate datetime,
	@ArriveCityID int,
	@ArriveFlight varchar(50),
	@ArriveNote varchar(4000),
	@DepartDate datetime,
	@DepartCityID int,
	@DepartFlight varchar(50),
	@DepartNote varchar(4000),
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
	@Comments varchar(8000),
	@IsRecordActive bit,
	@IsReadOnly bit,
	@ParentFolderID int,
	@AddedOn datetime,
	@AddedBy int,
	@RowVersion timestamp,
	@IsDeleted bit,
	@CurrencyCode char(3),
	@NetMinOrMax varchar(10),
	@AgentContactID int,
	@UpdatedOn datetime,
	@AllocationsItineraryID int
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
	[CurrencyCode] = @CurrencyCode,
	[NetMinOrMax] = @NetMinOrMax,
	[AgentContactID] = @AgentContactID,
	[UpdatedOn] = @UpdatedOn,
	[AllocationsItineraryID] = @AllocationsItineraryID
WHERE
	[ItineraryID] = @ItineraryID
	AND [RowVersion] = @RowVersion
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Itinerary_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Itinerary_Del]
GO

CREATE PROCEDURE [dbo].[Itinerary_Del]
	@ItineraryID int,
	@RowVersion timestamp
AS
DELETE FROM [dbo].[Itinerary]
WHERE
	[ItineraryID] = @ItineraryID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Itinerary_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Itinerary_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[Itinerary_Sel_ByID]
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
	[CurrencyCode],
	[NetMinOrMax],
	[AgentContactID],
	[UpdatedOn],
	[AllocationsItineraryID]
FROM [dbo].[Itinerary]
WHERE
	[ItineraryID] = @ItineraryID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Itinerary_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Itinerary_Sel_All]
GO

CREATE PROCEDURE [dbo].[Itinerary_Sel_All]
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
	[CurrencyCode],
	[NetMinOrMax],
	[AgentContactID],
	[UpdatedOn],
	[AllocationsItineraryID]
FROM [dbo].[Itinerary]
ORDER BY 
	[ItineraryID] ASC
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Supplier_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Supplier_Ins]
GO

CREATE PROCEDURE [dbo].[Supplier_Ins]
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
	@Description varchar(8000),
	@Comments varchar(8000),
	@CancellationPolicy varchar(2000),
	@BankDetails varchar(255),
	@AccountingName varchar(150),
	@NetTaxTypeID int,
	@GrossTaxTypeID int,
	@PaymentTermID int,
	@DefaultMargin real,
	@DefaultCheckinTime datetime,
	@DefaultCheckoutTime datetime,
	@ImportID int,
	@ExportID varchar(50),
	@BookingWebsite varchar(255),
	@IsRecordActive bit,
	@ParentFolderID int,
	@AddedOn datetime,
	@AddedBy int,
	@IsDeleted bit,
	@UpdatedOn datetime,
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
	[NetTaxTypeID],
	[GrossTaxTypeID],
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
	[IsDeleted],
	[UpdatedOn]
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
	@NetTaxTypeID,
	@GrossTaxTypeID,
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
	@IsDeleted,
	@UpdatedOn
)
SELECT @SupplierID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Supplier_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Supplier_Upd]
GO

CREATE PROCEDURE [dbo].[Supplier_Upd]
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
	@Description varchar(8000),
	@Comments varchar(8000),
	@CancellationPolicy varchar(2000),
	@BankDetails varchar(255),
	@AccountingName varchar(150),
	@NetTaxTypeID int,
	@GrossTaxTypeID int,
	@PaymentTermID int,
	@DefaultMargin real,
	@DefaultCheckinTime datetime,
	@DefaultCheckoutTime datetime,
	@ImportID int,
	@ExportID varchar(50),
	@BookingWebsite varchar(255),
	@IsRecordActive bit,
	@ParentFolderID int,
	@AddedOn datetime,
	@AddedBy int,
	@RowVersion timestamp,
	@IsDeleted bit,
	@UpdatedOn datetime
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
	[NetTaxTypeID] = @NetTaxTypeID,
	[GrossTaxTypeID] = @GrossTaxTypeID,
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
	[IsDeleted] = @IsDeleted,
	[UpdatedOn] = @UpdatedOn
WHERE
	[SupplierID] = @SupplierID
	AND [RowVersion] = @RowVersion
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Supplier_Upd_ByGradeID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Supplier_Upd_ByGradeID]
GO

CREATE PROCEDURE [dbo].[Supplier_Upd_ByGradeID]
	@GradeID int,
	@GradeIDOld int
AS
UPDATE [dbo].[Supplier]
SET
	[GradeID] = @GradeID
WHERE
	[GradeID] = @GradeIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Supplier_Upd_ByGradeExternalID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Supplier_Upd_ByGradeExternalID]
GO

CREATE PROCEDURE [dbo].[Supplier_Upd_ByGradeExternalID]
	@GradeExternalID int,
	@GradeExternalIDOld int
AS
UPDATE [dbo].[Supplier]
SET
	[GradeExternalID] = @GradeExternalID
WHERE
	[GradeExternalID] = @GradeExternalIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Supplier_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Supplier_Del]
GO

CREATE PROCEDURE [dbo].[Supplier_Del]
	@SupplierID int,
	@RowVersion timestamp
AS
DELETE FROM [dbo].[Supplier]
WHERE
	[SupplierID] = @SupplierID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Supplier_Del_ByGradeID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Supplier_Del_ByGradeID]
GO

CREATE PROCEDURE [dbo].[Supplier_Del_ByGradeID]
	@GradeID int
AS
DELETE
FROM [dbo].[Supplier]
WHERE
	[GradeID] = @GradeID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Supplier_Del_ByGradeExternalID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Supplier_Del_ByGradeExternalID]
GO

CREATE PROCEDURE [dbo].[Supplier_Del_ByGradeExternalID]
	@GradeExternalID int
AS
DELETE
FROM [dbo].[Supplier]
WHERE
	[GradeExternalID] = @GradeExternalID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Supplier_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Supplier_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[Supplier_Sel_ByID]
	@SupplierID int
AS
SET NOCOUNT ON
SELECT
	[SupplierID],
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
	[NetTaxTypeID],
	[GrossTaxTypeID],
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
	[RowVersion],
	[IsDeleted],
	[UpdatedOn]
FROM [dbo].[Supplier]
WHERE
	[SupplierID] = @SupplierID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Supplier_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Supplier_Sel_All]
GO

CREATE PROCEDURE [dbo].[Supplier_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[SupplierID],
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
	[NetTaxTypeID],
	[GrossTaxTypeID],
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
	[RowVersion],
	[IsDeleted],
	[UpdatedOn]
FROM [dbo].[Supplier]
ORDER BY 
	[SupplierID] ASC
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Supplier_Sel_ByGradeID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Supplier_Sel_ByGradeID]
GO

CREATE PROCEDURE [dbo].[Supplier_Sel_ByGradeID]
	@GradeID int
AS
SET NOCOUNT ON
SELECT
	[SupplierID],
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
	[NetTaxTypeID],
	[GrossTaxTypeID],
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
	[RowVersion],
	[IsDeleted],
	[UpdatedOn]
FROM [dbo].[Supplier]
WHERE
	[GradeID] = @GradeID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Supplier_Sel_ByGradeExternalID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Supplier_Sel_ByGradeExternalID]
GO

CREATE PROCEDURE [dbo].[Supplier_Sel_ByGradeExternalID]
	@GradeExternalID int
AS
SET NOCOUNT ON
SELECT
	[SupplierID],
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
	[NetTaxTypeID],
	[GrossTaxTypeID],
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
	[RowVersion],
	[IsDeleted],
	[UpdatedOn]
FROM [dbo].[Supplier]
WHERE
	[GradeExternalID] = @GradeExternalID
GO




GO
PRINT N'Refreshing [dbo].[ItinerarySaleAllocationPricing]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItinerarySaleAllocationPricing';


GO
PRINT N'Refreshing [dbo].[PurchaseItemTaxType]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.PurchaseItemTaxType';


GO
PRINT N'Refreshing [dbo].[ItineraryDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryDetail';


GO
PRINT N'Refreshing [dbo].[ItineraryServiceTypeDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryServiceTypeDetail';


GO
PRINT N'Refreshing [dbo].[ItinerarySaleDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItinerarySaleDetail';


GO
PRINT N'Refreshing [dbo].[ItineraryClientDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryClientDetail';


GO
PRINT N'Refreshing [dbo].[ItinerarySaleAllocationDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItinerarySaleAllocationDetail';


GO
PRINT N'Refreshing [dbo].[ItineraryPaymentDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryPaymentDetail';


GO
PRINT N'Refreshing [dbo].[SupplierDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.SupplierDetail';


GO
PRINT N'Refreshing [dbo].[SupplierRatesDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.SupplierRatesDetail';


GO
PRINT N'Refreshing [dbo].[PurchaseItemDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.PurchaseItemDetail';


GO
PRINT N'Refreshing [dbo].[PurchaseItemPaymentsDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.PurchaseItemPaymentsDetail';


GO
PRINT N'Refreshing [dbo].[ItineraryServiceTypePricing]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryServiceTypePricing';


GO
PRINT N'Refreshing [dbo].[PurchaseItemPayments]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.PurchaseItemPayments';


GO
PRINT N'Refreshing [dbo].[_DataExtract_ItineraryFinancials]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._DataExtract_ItineraryFinancials';


GO
PRINT N'Refreshing [dbo].[__Repair_OrphanedMenuItems]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.__Repair_OrphanedMenuItems';


GO
PRINT N'Refreshing [dbo].[_Itinerary_New]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._Itinerary_New';


GO
PRINT N'Refreshing [dbo].[_Itinerary_Rename]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._Itinerary_Rename';


GO
PRINT N'Refreshing [dbo].[_ItinerarySet_Copy_ByID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._ItinerarySet_Copy_ByID';


GO
PRINT N'Refreshing [dbo].[_Report_WhoUsedSupplier]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._Report_WhoUsedSupplier';


GO
PRINT N'Refreshing [dbo].[Itinerary_Del]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Itinerary_Del';


GO
PRINT N'Refreshing [dbo].[Itinerary_Ins]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Itinerary_Ins';


GO
PRINT N'Refreshing [dbo].[Itinerary_Sel_All]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Itinerary_Sel_All';


GO
PRINT N'Refreshing [dbo].[Itinerary_Sel_ByID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Itinerary_Sel_ByID';


GO
PRINT N'Refreshing [dbo].[Itinerary_Upd]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Itinerary_Upd';


GO
PRINT N'Refreshing [dbo].[_Itinerary_ServiceSearch]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._Itinerary_ServiceSearch';


GO
PRINT N'Refreshing [dbo].[_Option_GetNewFromDate]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._Option_GetNewFromDate';


GO
PRINT N'Refreshing [dbo].[_Supplier_New]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._Supplier_New';


GO
PRINT N'Refreshing [dbo].[_Supplier_Rename]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._Supplier_Rename';


GO
PRINT N'Refreshing [dbo].[_SupplierSet_Copy_ByID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._SupplierSet_Copy_ByID';


GO
PRINT N'Refreshing [dbo].[Supplier_Del]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Supplier_Del';


GO
PRINT N'Refreshing [dbo].[Supplier_Del_ByGradeExternalID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Supplier_Del_ByGradeExternalID';


GO
PRINT N'Refreshing [dbo].[Supplier_Del_ByGradeID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Supplier_Del_ByGradeID';


GO
PRINT N'Refreshing [dbo].[Supplier_Ins]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Supplier_Ins';


GO
PRINT N'Refreshing [dbo].[Supplier_Sel_All]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Supplier_Sel_All';


GO
PRINT N'Refreshing [dbo].[Supplier_Sel_ByGradeExternalID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Supplier_Sel_ByGradeExternalID';


GO
PRINT N'Refreshing [dbo].[Supplier_Sel_ByGradeID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Supplier_Sel_ByGradeID';


GO
PRINT N'Refreshing [dbo].[Supplier_Sel_ByID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Supplier_Sel_ByID';


GO
PRINT N'Refreshing [dbo].[Supplier_Upd]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Supplier_Upd';


GO
PRINT N'Refreshing [dbo].[Supplier_Upd_ByGradeExternalID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Supplier_Upd_ByGradeExternalID';


GO
PRINT N'Refreshing [dbo].[Supplier_Upd_ByGradeID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Supplier_Upd_ByGradeID';


exec __RefreshViews;
GO
----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2012.11.12'
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