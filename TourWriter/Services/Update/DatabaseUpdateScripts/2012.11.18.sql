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
if ((select VersionNumber from AppSettings) <> '2012.11.13')
	RAISERROR (N'Database Update Script is not correct version for current database version',17,1)

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------

GO
if not Exists(select * from sys.columns where Name = N'Sex' and Object_ID = Object_ID(N'Contact'))
ALTER TABLE [dbo].[Contact]
    ADD [Sex] CHAR (1) NULL;
		
GO
if not Exists(select * from sys.columns where Name = N'Preferences' and Object_ID = Object_ID(N'Contact'))
ALTER TABLE [dbo].[Contact]
    ADD [Preferences] VARCHAR (8000) NULL;

GO
if not Exists(select * from sys.columns where Name = N'ItineraryTypeID' and Object_ID = Object_ID(N'Itinerary'))
ALTER TABLE [dbo].[Itinerary]
    ADD [ItineraryTypeID] INT NULL;

GO
if not Exists(select * from sys.columns where Name = N'Preferences' and Object_ID = Object_ID(N'Itinerary'))
ALTER TABLE [dbo].[Itinerary]
    ADD [Preferences] VARCHAR (8000) NULL;

GO
ALTER TABLE [dbo].[ItineraryMember] ALTER COLUMN [Comments] VARCHAR (8000) NULL;

GO
if not Exists(select * from sys.columns where Name = N'Preferences' and Object_ID = Object_ID(N'ItineraryMember'))
ALTER TABLE [dbo].[ItineraryMember]
    ADD [Preferences] VARCHAR (8000) NULL;

GO
if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Data]') and OBJECTPROPERTY(id, N'IsTable') = 1)
begin
CREATE TABLE [dbo].[Data] (
    [DataID]      INT           IDENTITY (1, 1) NOT NULL,
    [ItineraryID] INT           NOT NULL,
    [Json]        VARCHAR (MAX) NOT NULL,
    [CreatedOn]   DATETIME      NOT NULL,
    CONSTRAINT [PK_Data] PRIMARY KEY CLUSTERED ([DataID] ASC)
);
end

GO
if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryType]') and OBJECTPROPERTY(id, N'IsTable') = 1)
begin
CREATE TABLE [dbo].[ItineraryType] (
    [ItineraryTypeID]   INT           IDENTITY (1, 1) NOT NULL,
    [ItineraryTypeName] VARCHAR (100) NOT NULL,
    [IsDefault]         BIT           NULL,
    CONSTRAINT [PK_ItineraryType] PRIMARY KEY CLUSTERED ([ItineraryTypeID] ASC)
);
end

---================================================================================================================================
GO
ALTER VIEW [dbo].[ViewsColumnList]
AS
select
	table_name as TableName, 
	column_name as ColumnName,
	data_type as DataType, 
	character_maximum_length as MaxLength
from INFORMATION_SCHEMA.columns
WHERE TABLE_NAME in (select TABLE_NAME FROM INFORMATION_SCHEMA.TABLES where TABLE_TYPE='VIEW')
and table_name <> 'ViewsColumnList'
--order by table_name, column_name


GO
ALTER PROCEDURE [dbo].[_AgentSet_Sel_All]
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
WHERE
	[PaymentTermID] IN 
		(SELECT [PurchasePaymentTermID] FROM [Agent])
OR
	[PaymentTermID] IN 
		(SELECT [SalePaymentTermID] FROM [Agent])

-- Agent --
EXEC Agent_Sel_All

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
	[PassportExpiry],
	[UpdatedOn],
	[Sex],
	[Preferences]
FROM [dbo].[Contact]
WHERE
	[ContactID] IN (
		SELECT [ContactID] FROM [dbo].[AgentContact]  )

-- AgentContact --
EXEC AgentContact_Sel_All

-- AgentMarkup --
EXEC AgentMargin_Sel_All
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
	[PassportExpiry],
	[UpdatedOn],
	[Sex],
	[Preferences]
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
	[AllocationsItineraryID],
	[ItineraryTypeID],
	[Preferences]
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
	[PassportExpiry],
	[UpdatedOn],
	[Sex],
	[Preferences]
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
	[RoomName],
	[Preferences]
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
    

-- GroupPrice
SELECT [GroupPriceID]
      ,[GroupPriceName]
      ,[ItineraryID]
      ,[ItineraryPaxID]
      ,[OptionTypeID]
      ,[Price]
      ,[Markup]
      ,[PriceOverride]
  FROM [GroupPrice]
  where ItineraryID = @ItineraryID


GO
ALTER PROCEDURE [dbo].[_ContactSet_Sel_ByID]
	@ContactID int
AS
SET NOCOUNT ON

/* Select parent tables first */

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
	[IsRecordActive],
	[ParentFolderID],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[JobDescription],
	[PassportNumber],
	[PassportExpiry],
	[UpdatedOn],
	[Sex],
	[Preferences]
FROM [dbo].[Contact]
WHERE
	[ContactID] = @ContactID

SELECT
	[ContactID],
	[ContactCategoryID],
	[AddedBy]
FROM [dbo].[ContactContactCategory]
WHERE
	[ContactID] = @ContactID


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
exec AccountingCategory_Sel_All
exec ContactCategory_Sel_All
exec OptionType_Sel_All
exec Flag_Sel_All
exec TemplateCategory_Sel_All
exec Template_Sel_All
exec ContentType_Sel_All
select * from CurrencyRate where ValidTo > getdate() --EXEC CurrencyRate_Sel_All
select t.* from Task t inner join DefaultTask d on d.TaskID = t.TaskID -- all Tasks linked to DefaultTasks table
exec DefaultTask_Sel_All
exec ItineraryType_Sel_All
GO

PRINT N'Refreshing [dbo].[ItinerarySaleAllocationPricing]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItinerarySaleAllocationPricing';


GO
PRINT N'Altering [dbo].[ContactDetail]...';


GO

ALTER VIEW [dbo].[ContactDetail]
AS
SELECT
    cont.ContactID,
    ContactName,
    Title,
    FirstName,
    LastName,
    JobDescription,
    StreetAddress,
    PostAddress,
    CityName,
    RegionName,
    StateName,
    CountryName,
    PostCode,
    WorkPhone,
    HomePhone,
    CellPhone,
    Fax,
    Email1,
    Email2,
    Website,
    BirthDate,
    Notes,
    ItineraryCount,
    FolderName,
    cont.ParentFolderID,
	(   SELECT STUFF(( 
			SELECT ', ' + c.ContactCategoryName
			FROM ContactCategory c
			INNER JOIN ContactContactCategory cc ON c.ContactCategoryID = cc.ContactCategoryID
			WHERE cc.ContactID = cont.ContactID
			FOR XML PATH (''), TYPE 
		).value('.', 'VARCHAR(MAX)'), 1, 2,'')
    ) AS Categories,
    (   SELECT STUFF(( 
			SELECT ', ' + a.AgentName
			FROM Contact c1
			LEFT JOIN AgentContact ac ON ac.ContactID = c1.ContactID
			LEFT JOIN Agent a ON a.AgentID = ac.AgentID
			WHERE c1.ContactID = cont.ContactID
			FOR XML PATH (''), TYPE 
		).value('.', 'VARCHAR(MAX)'), 1, 2,'')
    ) AS Agents,
    PassportNumber,
    PassportExpiry,
    UpdatedOn,
    Sex,
    Preferences as ContactPreferences
FROM Contact AS cont
LEFT OUTER JOIN City AS city ON city.CityID = cont.CityID
LEFT OUTER JOIN Region AS region ON region.RegionID = cont.RegionID
LEFT OUTER JOIN [State] AS stat ON stat.StateID = cont.StateID
LEFT OUTER JOIN Country AS country ON country.CountryID = cont.CountryID
LEFT OUTER JOIN Folder AS fld ON fld.FolderId = cont.ParentFolderId
LEFT OUTER JOIN
(   SELECT ContactID, COUNT(ItineraryID) AS ItineraryCount
    FROM ItineraryMember mem
    LEFT OUTER JOIN ItineraryGroup grp ON mem.ItineraryGroupID = grp.ItineraryGroupID
    WHERE ContactID IS NOT NULL
    GROUP BY ContactID
) cnt ON cnt.ContactID = cont.ContactID
WHERE (IsDeleted IS NULL OR IsDeleted = 0);
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
		itin.AgentContactID,
		contact.FirstName as AgentContactFirstName,
		contact.LastName as AgentContactLastName,
		contact.ContactName as AgentContactDisplayName,
		contact.Email1 as AgentContactEmail,
		contact.PostAddress as AgentContactPostAddress,
		contact.StreetAddress as AgentContactStreetAddress,
		contactCity.CityName as AgentContactCityName,
		contactRegion.RegionName as AgentContactRegionName,
		contactState.StateName as AgentContactStateName,
		contactCountry.CountryName as AgentContactCountryName,
		contact.PostCode as AgentContactPostCode,
		contact.WorkPhone as AgentContactWorkPhone,
		contact.CellPhone as AgentContactCellPhone,
		contact.JobDescription as AgentContactJobDesription,				
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
		isnull(itin.CurrencyCode, (select top(1) CurrencyCode from Appsettings)) as ItineraryCurrencyCode,
		isnull(itinCcy.DisplayFormat, sysCcy.DisplayFormat) as ItineraryCurrencyFormat,
		itin.NetComOrMup as NetOverrideComOrMup,
		pric.NetFinalTotal as ItineraryNetFinalTotal,
		pric.GrossFinalTotal as ItineraryGrossFinalTotal,
		pric.Markup as ItineraryMarkup,
		pric.Commission as ItineraryCommission,
		pric.Yield as ItineraryYield,
		pric.GrossFinalTotalTaxAmount as ItineraryGrossFinalTotalTaxAmount,
		isnull(payments.TotalPayments, 0) as ItineraryTotalPayments,
		isnull(pric.GrossFinalTotal - payments.TotalPayments, isnull(pric.GrossFinalTotal,0)) as ItineraryTotalOutstanding,
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
		--pric.GrossFinalTotal - dbo.PaymentTerm.DepositAmount as ItineraryBalanceAmount,
		itin.IsRecordActive,
		itin.AddedOn as ItineraryCreatedDate,
		itin.ParentFolderID as MenuFolderID,
		itin.ItineraryTypeID,
		typ.ItineraryTypeName,
		itin.Preferences as ItineraryPreferences
		
		----/**** only for backwards compatibility ****/
		--,pric.NetFinalTotal as ItineraryNet
		--,pric.GrossFinalTotal as ItineraryGross
		----/******************************************/		
		
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
	left outer join Contact contact on contact.ContactID = itin.AgentContactID
	left outer join City contactCity on contact.CityID = contactCity.CityID
	left outer join Region contactRegion on contact.RegionID = contactRegion.RegionID
	left outer join [State] contactState on contact.StateID = contactState.StateID
	left outer join Country contactCountry on contact.CountryID = contactCountry.CountryID
	left outer join ItineraryType typ on typ.ItineraryTypeID = itin.ItineraryTypeID
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
	) payments on itin.ItineraryID = payments.ItineraryID
	left join Currency itinCcy on itinCcy.CurrencyCode = itin.CurrencyCode COLLATE DATABASE_DEFAULT
	left join Currency sysCcy on sysCcy.CurrencyCode = (select top(1) CurrencyCode from AppSettings) COLLATE DATABASE_DEFAULT
	where 
		(itin.IsDeleted is null or itin.IsDeleted = 'false');
GO
PRINT N'Altering [dbo].[ItineraryClientDetail]...';


GO



ALTER VIEW [dbo].[ItineraryClientDetail]
AS
select
	ItineraryMemberName,
	mem.Title as ItineraryMemberTitle,
	mem.RoomTypeID,
	room.RoomTypeName,
	mem.AgentID as MemberAgentID,
	agent.AgentName as MemberAgentName,
	mem.PriceOverride,
	mem.RoomName,
	mem.Preferences as MemberPreferences,
	cont.*,	
	AgeGroupName as AgeGroup,
	mem.Comments as MemberComments,
	grp.Comments as GroupComments,
	grp.NoteToClient as GroupNoteToClient,
	grp.NoteToSupplier as GroupNoteToSupplier,
	grp.CurrencyCode as GroupCurrencyCode,
	grp.CurrencyRate as GroupCurrencyRate,
	itin.*
from ItineraryMember mem
left outer join ItineraryGroup grp on mem.ItineraryGroupID = grp.ItineraryGroupID
left outer join ItineraryDetail itin on grp.ItineraryID = itin.ItineraryID
left outer join AgeGroup age on mem.AgeGroupID = age.AgeGroupID
left outer join ContactDetail cont on mem.ContactID = cont.ContactID
left outer join RoomType room on room.ItineraryID = itin.ItineraryID
left outer join Agent agent on agent.AgentID = mem.AgentID
GO
PRINT N'Refreshing [dbo].[ItineraryPaymentDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryPaymentDetail';

EXECUTE sp_refreshsqlmodule N'dbo.ViewsColumnList';

GO
PRINT N'Refreshing [dbo].[ItinerarySaleAllocationDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItinerarySaleAllocationDetail';


GO
PRINT N'Refreshing [dbo].[ItinerarySaleDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItinerarySaleDetail';


GO
PRINT N'Refreshing [dbo].[ItineraryServiceTypeDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryServiceTypeDetail';


GO
PRINT N'Refreshing [dbo].[PurchaseItemDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.PurchaseItemDetail';


GO
PRINT N'Refreshing [dbo].[PurchaseItemPaymentsDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.PurchaseItemPaymentsDetail';


GO
PRINT N'Altering [dbo].[_ItinerarySet_Copy_ByID]...';


GO
/** Copy Itinerary and related records **/

ALTER PROCEDURE [dbo].[_ItinerarySet_Copy_ByID]
    @OrigItineraryID int,
    @NewItineraryName varchar(100),
    @AddedBy int
AS
SET NOCOUNT ON

DECLARE @NewItineraryID int

-- Itinerary ----------------------------------------------
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
SELECT
    @NewItineraryName,
	DisplayName,
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
	IsLockedGrossOverride,
	PricingNote,
	AgentID,
	PaymentTermID,
	ItineraryStatusID,
	ItinerarySourceID,
	CountryID,
	AssignedTo,
	DepartmentID,
	BranchID,
	PaxOverride,
	Comments,
	IsRecordActive,
	IsReadOnly,
	ParentFolderID,
	getdate(), --AddedOn,
	@AddedBy,
	IsDeleted,
	CurrencyCode,
	NetMinOrMax,
	AgentContactID,
	getdate(), --UpdatedOn,
	AllocationsItineraryID
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
        getdate(), --AddedOn,
        @AddedBy
    FROM PurchaseLine WHERE PurchaseLineID = @OrigPurchaseLineID
    
    SELECT @NewPurchaseLineID=SCOPE_IDENTITY()

    -- PurchaseItem -----------------------------------------------------------------

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
		[DiscountType],
		[IsInvoiced],
		[SortDate]
    )
    SELECT
        @NewPurchaseLineID,
		OptionID,
		PurchaseItemName,
		BookingReference,
		StartDate,
		StartTime,
		EndDate,
		EndTime,
		Net,
		Gross,
		CurrencyRate,
		Quantity,
		NumberOfDays,
		PaymentTermID,
		RequestStatusID,
		IsLockedAccounting,
		getdate(), --AddedOn,
		@AddedBy,
		DiscountUnits,
		DiscountType,
		null, --IsInvoiced,
		SortDate
    FROM PurchaseItem WHERE PurchaseLineID = @OrigPurchaseLineID

    FETCH NEXT FROM PurchaseLineCursor INTO @OrigPurchaseLineID

END -- PurchaseLineCursor
CLOSE PurchaseLineCursor
DEALLOCATE PurchaseLineCursor

-- group prices
INSERT [dbo].[GroupPrice]
(
	[GroupPriceName],
	[ItineraryID],
	[ItineraryPaxID],
	[OptionTypeID],
	[Price],
	[Markup],
	[PriceOverride]
)
select
	GroupPriceName,
	ItineraryID,
	ItineraryPaxID,
	OptionTypeID,
	Price,
	Markup,
	PriceOverride
from GroupPrice where [ItineraryID] = @OrigItineraryID

-- return new itinerary id
SELECT @NewItineraryID
GO
PRINT N'Refreshing [dbo].[ItineraryServiceTypePricing]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryServiceTypePricing';


GO
PRINT N'Refreshing [dbo].[PurchaseItemPayments]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.PurchaseItemPayments';


GO
PRINT N'Refreshing [dbo].[__Repair_OrphanedMenuItems]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.__Repair_OrphanedMenuItems';


GO
PRINT N'Refreshing [dbo].[_AgentSet_Sel_All]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._AgentSet_Sel_All';


GO
PRINT N'Refreshing [dbo].[_AgentSet_Sel_ByID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._AgentSet_Sel_ByID';


GO
PRINT N'Refreshing [dbo].[_Contact_New]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._Contact_New';


GO
PRINT N'Refreshing [dbo].[_Contact_Rename]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._Contact_Rename';


GO
PRINT N'Refreshing [dbo].[_ContactSet_Copy_ByID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._ContactSet_Copy_ByID';


GO
PRINT N'Refreshing [dbo].[_ContactSet_Sel_ByID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._ContactSet_Sel_ByID';


GO
PRINT N'Refreshing [dbo].[_ItinerarySet_Sel_ByID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._ItinerarySet_Sel_ByID';


GO
PRINT N'Refreshing [dbo].[_SupplierSet_Sel_ByID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._SupplierSet_Sel_ByID';


GO
PRINT N'Refreshing [dbo].[_UserSet_Sel_All]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._UserSet_Sel_All';


GO
PRINT N'Refreshing [dbo].[_UserSet_Sel_ByID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._UserSet_Sel_ByID';


GO
PRINT N'Refreshing [dbo].[Contact_Del]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Contact_Del';


GO
PRINT N'Refreshing [dbo].[Contact_Ins]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Contact_Ins';


GO
PRINT N'Refreshing [dbo].[Contact_Sel_All]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Contact_Sel_All';


GO
PRINT N'Refreshing [dbo].[Contact_Sel_ByID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Contact_Sel_ByID';


GO
PRINT N'Refreshing [dbo].[Contact_Upd]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Contact_Upd';


GO
PRINT N'Refreshing [dbo].[_UserSet_Sel_ByUsernamePassword]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._UserSet_Sel_ByUsernamePassword';


GO
PRINT N'Refreshing [dbo].[_DataExtract_ItineraryFinancials]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._DataExtract_ItineraryFinancials';


GO
PRINT N'Refreshing [dbo].[_Itinerary_New]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._Itinerary_New';


GO
PRINT N'Refreshing [dbo].[_Itinerary_Rename]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._Itinerary_Rename';


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
PRINT N'Refreshing [dbo].[ItineraryMember_Del]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryMember_Del';


GO
PRINT N'Refreshing [dbo].[ItineraryMember_Del_ByContactID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryMember_Del_ByContactID';


GO
PRINT N'Refreshing [dbo].[ItineraryMember_Del_ByItineraryGroupID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryMember_Del_ByItineraryGroupID';


GO
PRINT N'Refreshing [dbo].[ItineraryMember_Ins]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryMember_Ins';


GO
PRINT N'Refreshing [dbo].[ItineraryMember_Sel_All]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryMember_Sel_All';


GO
PRINT N'Refreshing [dbo].[ItineraryMember_Sel_ByContactID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryMember_Sel_ByContactID';


GO
PRINT N'Refreshing [dbo].[ItineraryMember_Sel_ByID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryMember_Sel_ByID';


GO
PRINT N'Refreshing [dbo].[ItineraryMember_Sel_ByItineraryGroupID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryMember_Sel_ByItineraryGroupID';


GO
PRINT N'Refreshing [dbo].[ItineraryMember_Upd]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryMember_Upd';


GO
PRINT N'Refreshing [dbo].[ItineraryMember_Upd_ByContactID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryMember_Upd_ByContactID';


GO
PRINT N'Refreshing [dbo].[ItineraryMember_Upd_ByItineraryGroupID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryMember_Upd_ByItineraryGroupID';


GO
PRINT N'Update complete.'
GO

---================================================================================================================================

GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Contact_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Contact_Ins]
GO

CREATE PROCEDURE [dbo].[Contact_Ins]
	@ContactName varchar(100),
	@Title varchar(50),
	@FirstName varchar(100),
	@LastName varchar(100),
	@StreetAddress varchar(500),
	@PostName varchar(100),
	@PostAddress varchar(500),
	@CityID int,
	@RegionID int,
	@StateID int,
	@CountryID int,
	@PostCode varchar(20),
	@WorkPhone varchar(50),
	@HomePhone varchar(50),
	@CellPhone varchar(50),
	@Fax varchar(50),
	@Email1 varchar(100),
	@Email2 varchar(100),
	@Website varchar(150),
	@BirthDate datetime,
	@Notes varchar(8000),
	@IsRecordActive bit,
	@ParentFolderID int,
	@AddedOn datetime,
	@AddedBy int,
	@IsDeleted bit,
	@JobDescription varchar(2000),
	@PassportNumber varchar(50),
	@PassportExpiry datetime,
	@UpdatedOn datetime,
	@Sex char(1),
	@Preferences varchar(8000),
	@ContactID int OUTPUT
AS
INSERT [dbo].[Contact]
(
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
	[IsRecordActive],
	[ParentFolderID],
	[AddedOn],
	[AddedBy],
	[IsDeleted],
	[JobDescription],
	[PassportNumber],
	[PassportExpiry],
	[UpdatedOn],
	[Sex],
	[Preferences]
)
VALUES
(
	ISNULL(@ContactName, ('')),
	@Title,
	@FirstName,
	@LastName,
	@StreetAddress,
	@PostName,
	@PostAddress,
	@CityID,
	@RegionID,
	@StateID,
	@CountryID,
	@PostCode,
	@WorkPhone,
	@HomePhone,
	@CellPhone,
	@Fax,
	@Email1,
	@Email2,
	@Website,
	@BirthDate,
	@Notes,
	@IsRecordActive,
	@ParentFolderID,
	@AddedOn,
	@AddedBy,
	@IsDeleted,
	@JobDescription,
	@PassportNumber,
	@PassportExpiry,
	@UpdatedOn,
	@Sex,
	@Preferences
)
SELECT @ContactID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Contact_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Contact_Upd]
GO

CREATE PROCEDURE [dbo].[Contact_Upd]
	@ContactID int,
	@ContactName varchar(100),
	@Title varchar(50),
	@FirstName varchar(100),
	@LastName varchar(100),
	@StreetAddress varchar(500),
	@PostName varchar(100),
	@PostAddress varchar(500),
	@CityID int,
	@RegionID int,
	@StateID int,
	@CountryID int,
	@PostCode varchar(20),
	@WorkPhone varchar(50),
	@HomePhone varchar(50),
	@CellPhone varchar(50),
	@Fax varchar(50),
	@Email1 varchar(100),
	@Email2 varchar(100),
	@Website varchar(150),
	@BirthDate datetime,
	@Notes varchar(8000),
	@IsRecordActive bit,
	@ParentFolderID int,
	@AddedOn datetime,
	@AddedBy int,
	@RowVersion timestamp,
	@IsDeleted bit,
	@JobDescription varchar(2000),
	@PassportNumber varchar(50),
	@PassportExpiry datetime,
	@UpdatedOn datetime,
	@Sex char(1),
	@Preferences varchar(8000)
AS
UPDATE [dbo].[Contact]
SET 
	[ContactName] = @ContactName,
	[Title] = @Title,
	[FirstName] = @FirstName,
	[LastName] = @LastName,
	[StreetAddress] = @StreetAddress,
	[PostName] = @PostName,
	[PostAddress] = @PostAddress,
	[CityID] = @CityID,
	[RegionID] = @RegionID,
	[StateID] = @StateID,
	[CountryID] = @CountryID,
	[PostCode] = @PostCode,
	[WorkPhone] = @WorkPhone,
	[HomePhone] = @HomePhone,
	[CellPhone] = @CellPhone,
	[Fax] = @Fax,
	[Email1] = @Email1,
	[Email2] = @Email2,
	[Website] = @Website,
	[BirthDate] = @BirthDate,
	[Notes] = @Notes,
	[IsRecordActive] = @IsRecordActive,
	[ParentFolderID] = @ParentFolderID,
	[AddedOn] = @AddedOn,
	[AddedBy] = @AddedBy,
	[IsDeleted] = @IsDeleted,
	[JobDescription] = @JobDescription,
	[PassportNumber] = @PassportNumber,
	[PassportExpiry] = @PassportExpiry,
	[UpdatedOn] = @UpdatedOn,
	[Sex] = @Sex,
	[Preferences] = @Preferences
WHERE
	[ContactID] = @ContactID
	AND [RowVersion] = @RowVersion
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Contact_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Contact_Del]
GO

CREATE PROCEDURE [dbo].[Contact_Del]
	@ContactID int,
	@RowVersion timestamp
AS
DELETE FROM [dbo].[Contact]
WHERE
	[ContactID] = @ContactID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Contact_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Contact_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[Contact_Sel_ByID]
	@ContactID int
AS
SET NOCOUNT ON
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
	[IsRecordActive],
	[ParentFolderID],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[JobDescription],
	[PassportNumber],
	[PassportExpiry],
	[UpdatedOn],
	[Sex],
	[Preferences]
FROM [dbo].[Contact]
WHERE
	[ContactID] = @ContactID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Contact_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Contact_Sel_All]
GO

CREATE PROCEDURE [dbo].[Contact_Sel_All]
AS
SET NOCOUNT ON
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
	[IsRecordActive],
	[ParentFolderID],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[JobDescription],
	[PassportNumber],
	[PassportExpiry],
	[UpdatedOn],
	[Sex],
	[Preferences]
FROM [dbo].[Contact]
ORDER BY 
	[ContactID] ASC
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
	@ItineraryTypeID int,
	@Preferences varchar(8000),
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
	[AllocationsItineraryID],
	[ItineraryTypeID],
	[Preferences]
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
	@AllocationsItineraryID,
	@ItineraryTypeID,
	@Preferences
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
	@AllocationsItineraryID int,
	@ItineraryTypeID int,
	@Preferences varchar(8000)
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
	[AllocationsItineraryID] = @AllocationsItineraryID,
	[ItineraryTypeID] = @ItineraryTypeID,
	[Preferences] = @Preferences
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
	[AllocationsItineraryID],
	[ItineraryTypeID],
	[Preferences]
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
	[AllocationsItineraryID],
	[ItineraryTypeID],
	[Preferences]
FROM [dbo].[Itinerary]
ORDER BY 
	[ItineraryID] ASC
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryMember_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryMember_Ins]
GO

CREATE PROCEDURE [dbo].[ItineraryMember_Ins]
	@ItineraryGroupID int,
	@ItineraryMemberName varchar(100),
	@ContactID int,
	@AgeGroupID int,
	@Age int,
	@ArriveDate datetime,
	@ArriveCityID int,
	@ArriveFlight varchar(50),
	@DepartDate datetime,
	@DepartCityID int,
	@DepartFlight varchar(50),
	@IsDefaultContact bit,
	@IsDefaultBilling bit,
	@Comments varchar(8000),
	@AddedOn datetime,
	@AddedBy int,
	@Title varchar(50),
	@RoomTypeID int,
	@AgentID int,
	@PriceOverride decimal(18, 2),
	@RoomName varchar(50),
	@Preferences varchar(8000),
	@ItineraryMemberID int OUTPUT
AS
INSERT [dbo].[ItineraryMember]
(
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
	[Title],
	[RoomTypeID],
	[AgentID],
	[PriceOverride],
	[RoomName],
	[Preferences]
)
VALUES
(
	@ItineraryGroupID,
	@ItineraryMemberName,
	@ContactID,
	@AgeGroupID,
	@Age,
	@ArriveDate,
	@ArriveCityID,
	@ArriveFlight,
	@DepartDate,
	@DepartCityID,
	@DepartFlight,
	@IsDefaultContact,
	@IsDefaultBilling,
	@Comments,
	@AddedOn,
	@AddedBy,
	@Title,
	@RoomTypeID,
	@AgentID,
	@PriceOverride,
	@RoomName,
	@Preferences
)
SELECT @ItineraryMemberID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryMember_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryMember_Upd]
GO

CREATE PROCEDURE [dbo].[ItineraryMember_Upd]
	@ItineraryMemberID int,
	@ItineraryGroupID int,
	@ItineraryMemberName varchar(100),
	@ContactID int,
	@AgeGroupID int,
	@Age int,
	@ArriveDate datetime,
	@ArriveCityID int,
	@ArriveFlight varchar(50),
	@DepartDate datetime,
	@DepartCityID int,
	@DepartFlight varchar(50),
	@IsDefaultContact bit,
	@IsDefaultBilling bit,
	@Comments varchar(8000),
	@AddedOn datetime,
	@AddedBy int,
	@RowVersion timestamp,
	@Title varchar(50),
	@RoomTypeID int,
	@AgentID int,
	@PriceOverride decimal(18, 2),
	@RoomName varchar(50),
	@Preferences varchar(8000)
AS
UPDATE [dbo].[ItineraryMember]
SET 
	[ItineraryGroupID] = @ItineraryGroupID,
	[ItineraryMemberName] = @ItineraryMemberName,
	[ContactID] = @ContactID,
	[AgeGroupID] = @AgeGroupID,
	[Age] = @Age,
	[ArriveDate] = @ArriveDate,
	[ArriveCityID] = @ArriveCityID,
	[ArriveFlight] = @ArriveFlight,
	[DepartDate] = @DepartDate,
	[DepartCityID] = @DepartCityID,
	[DepartFlight] = @DepartFlight,
	[IsDefaultContact] = @IsDefaultContact,
	[IsDefaultBilling] = @IsDefaultBilling,
	[Comments] = @Comments,
	[AddedOn] = @AddedOn,
	[AddedBy] = @AddedBy,
	[Title] = @Title,
	[RoomTypeID] = @RoomTypeID,
	[AgentID] = @AgentID,
	[PriceOverride] = @PriceOverride,
	[RoomName] = @RoomName,
	[Preferences] = @Preferences
WHERE
	[ItineraryMemberID] = @ItineraryMemberID
	AND [RowVersion] = @RowVersion
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryMember_Upd_ByItineraryGroupID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryMember_Upd_ByItineraryGroupID]
GO

CREATE PROCEDURE [dbo].[ItineraryMember_Upd_ByItineraryGroupID]
	@ItineraryGroupID int,
	@ItineraryGroupIDOld int
AS
UPDATE [dbo].[ItineraryMember]
SET
	[ItineraryGroupID] = @ItineraryGroupID
WHERE
	[ItineraryGroupID] = @ItineraryGroupIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryMember_Upd_ByContactID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryMember_Upd_ByContactID]
GO

CREATE PROCEDURE [dbo].[ItineraryMember_Upd_ByContactID]
	@ContactID int,
	@ContactIDOld int
AS
UPDATE [dbo].[ItineraryMember]
SET
	[ContactID] = @ContactID
WHERE
	[ContactID] = @ContactIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryMember_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryMember_Del]
GO

CREATE PROCEDURE [dbo].[ItineraryMember_Del]
	@ItineraryMemberID int,
	@RowVersion timestamp
AS
DELETE FROM [dbo].[ItineraryMember]
WHERE
	[ItineraryMemberID] = @ItineraryMemberID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryMember_Del_ByItineraryGroupID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryMember_Del_ByItineraryGroupID]
GO

CREATE PROCEDURE [dbo].[ItineraryMember_Del_ByItineraryGroupID]
	@ItineraryGroupID int
AS
DELETE
FROM [dbo].[ItineraryMember]
WHERE
	[ItineraryGroupID] = @ItineraryGroupID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryMember_Del_ByContactID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryMember_Del_ByContactID]
GO

CREATE PROCEDURE [dbo].[ItineraryMember_Del_ByContactID]
	@ContactID int
AS
DELETE
FROM [dbo].[ItineraryMember]
WHERE
	[ContactID] = @ContactID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryMember_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryMember_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[ItineraryMember_Sel_ByID]
	@ItineraryMemberID int
AS
SET NOCOUNT ON
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
	[RoomName],
	[Preferences]
FROM [dbo].[ItineraryMember]
WHERE
	[ItineraryMemberID] = @ItineraryMemberID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryMember_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryMember_Sel_All]
GO

CREATE PROCEDURE [dbo].[ItineraryMember_Sel_All]
AS
SET NOCOUNT ON
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
	[RoomName],
	[Preferences]
FROM [dbo].[ItineraryMember]
ORDER BY 
	[ItineraryMemberID] ASC
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryMember_Sel_ByItineraryGroupID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryMember_Sel_ByItineraryGroupID]
GO

CREATE PROCEDURE [dbo].[ItineraryMember_Sel_ByItineraryGroupID]
	@ItineraryGroupID int
AS
SET NOCOUNT ON
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
	[RoomName],
	[Preferences]
FROM [dbo].[ItineraryMember]
WHERE
	[ItineraryGroupID] = @ItineraryGroupID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryMember_Sel_ByContactID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryMember_Sel_ByContactID]
GO

CREATE PROCEDURE [dbo].[ItineraryMember_Sel_ByContactID]
	@ContactID int
AS
SET NOCOUNT ON
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
	[RoomName],
	[Preferences]
FROM [dbo].[ItineraryMember]
WHERE
	[ContactID] = @ContactID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryType_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryType_Ins]
GO

CREATE PROCEDURE [dbo].[ItineraryType_Ins]
	@ItineraryTypeName varchar(100),
	@IsDefault bit,
	@ItineraryTypeID int OUTPUT
AS
INSERT [dbo].[ItineraryType]
(
	[ItineraryTypeName],
	[IsDefault]
)
VALUES
(
	@ItineraryTypeName,
	@IsDefault
)
SELECT @ItineraryTypeID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryType_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryType_Upd]
GO

CREATE PROCEDURE [dbo].[ItineraryType_Upd]
	@ItineraryTypeID int,
	@ItineraryTypeName varchar(100),
	@IsDefault bit
AS
UPDATE [dbo].[ItineraryType]
SET 
	[ItineraryTypeName] = @ItineraryTypeName,
	[IsDefault] = @IsDefault
WHERE
	[ItineraryTypeID] = @ItineraryTypeID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryType_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryType_Del]
GO

CREATE PROCEDURE [dbo].[ItineraryType_Del]
	@ItineraryTypeID int
AS
DELETE FROM [dbo].[ItineraryType]
WHERE
	[ItineraryTypeID] = @ItineraryTypeID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryType_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryType_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[ItineraryType_Sel_ByID]
	@ItineraryTypeID int
AS
SET NOCOUNT ON
SELECT
	[ItineraryTypeID],
	[ItineraryTypeName],
	[IsDefault]
FROM [dbo].[ItineraryType]
WHERE
	[ItineraryTypeID] = @ItineraryTypeID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryType_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryType_Sel_All]
GO

CREATE PROCEDURE [dbo].[ItineraryType_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[ItineraryTypeID],
	[ItineraryTypeName],
	[IsDefault]
FROM [dbo].[ItineraryType]
ORDER BY 
	[ItineraryTypeID] ASC
GO






exec __RefreshViews;
GO

----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2012.11.18'
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