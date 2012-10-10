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
if ((select VersionNumber from AppSettings) <> '2012.08.16' and (select VersionNumber from AppSettings) <> '2012.10.05')
	RAISERROR (N'Database Update Script is not correct version for current database version',17,1)

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------

GO
PRINT N'alter Supplier-Grade constraint on-delete to set null, not cascade';

GO
ALTER TABLE [dbo].[Supplier] DROP CONSTRAINT [FK_Supplier_Grade];
GO
ALTER TABLE [dbo].[Supplier] WITH NOCHECK
  ADD CONSTRAINT [FK_Supplier_Grade] FOREIGN KEY ([GradeID]) REFERENCES [dbo].[Grade] ([GradeID]) ON DELETE SET NULL ON UPDATE CASCADE;
GO
ALTER TABLE [dbo].[Supplier] DROP CONSTRAINT [FK_Supplier_GradeExternal];
GO
ALTER TABLE [dbo].[Supplier] WITH NOCHECK
  ADD CONSTRAINT [FK_Supplier_GradeExternal] FOREIGN KEY ([GradeExternalID]) REFERENCES [dbo].[GradeExternal] ([GradeExternalID]) ON DELETE SET NULL ON UPDATE CASCADE;
GO

GO
PRINT N'Add new column SortDate...';

GO

if not Exists(select * from sys.columns where Name = N'SortDate' and Object_ID = Object_ID(N'PurchaseItem'))
	ALTER TABLE [dbo].[PurchaseItem]
		ADD [SortDate] DATETIME NULL;
GO


GO
PRINT N'Altering [dbo].[ItineraryMember]...';

if not Exists(select * from sys.columns where Name = N'RoomTypeID' and Object_ID = Object_ID(N'ItineraryMember'))
	ALTER TABLE [dbo].[ItineraryMember]
		ADD [RoomTypeID] INT NULL;
GO
if not Exists(select * from sys.columns where Name = N'AgentID' and Object_ID = Object_ID(N'ItineraryMember'))
	ALTER TABLE [dbo].[ItineraryMember]
		ADD [AgentID] INT NULL;
GO
if not Exists(select * from sys.columns where Name = N'PriceOverride' and Object_ID = Object_ID(N'ItineraryMember'))
	ALTER TABLE [dbo].[ItineraryMember]
		ADD [PriceOverride] DECIMAL (18, 2) NULL;
GO
if not Exists(select * from sys.columns where Name = N'RoomName' and Object_ID = Object_ID(N'ItineraryMember'))
	ALTER TABLE [dbo].[ItineraryMember]
		ADD [RoomName] VARCHAR (50) NULL;
GO

GO

PRINT N'Creating [dbo].[RoomType]...';


GO


if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RoomType]') and OBJECTPROPERTY(id, N'IsTable') = 1)
begin

	CREATE TABLE [dbo].[RoomType] (
		[RoomTypeID]   INT           IDENTITY (1, 1) NOT NULL,
		[ItineraryID]  INT           NOT NULL,
		[OptionTypeID] INT           NULL,
		[RoomTypeName] VARCHAR (100) NOT NULL,
		[Quantity]     INT           NULL,
		CONSTRAINT [PK_RoomType] PRIMARY KEY CLUSTERED ([RoomTypeID] ASC)
	);

	ALTER TABLE [dbo].[RoomType] WITH NOCHECK
		ADD CONSTRAINT [FK_RoomType_Itinerary] FOREIGN KEY ([ItineraryID]) REFERENCES [dbo].[Itinerary] ([ItineraryID]) ON DELETE CASCADE ON UPDATE CASCADE;

	ALTER TABLE [dbo].[RoomType] WITH NOCHECK
		ADD CONSTRAINT [FK_RoomType_OptionType] FOREIGN KEY ([OptionTypeID]) REFERENCES [dbo].[OptionType] ([OptionTypeID]) ON DELETE SET NULL ON UPDATE CASCADE;

end
GO



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
	[CurrencyCode],
	[NetMinOrMax],
	[AgentContactID]
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

GO




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
	@Comments varchar(500),
	@AddedOn datetime,
	@AddedBy int,
	@Title varchar(50),
	@RoomTypeID int,
	@AgentID int,
	@PriceOverride decimal(18, 2),
	@RoomName varchar(50),
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
	[RoomName]
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
	@RoomName
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
	@Comments varchar(500),
	@AddedOn datetime,
	@AddedBy int,
	@RowVersion timestamp,
	@Title varchar(50),
	@RoomTypeID int,
	@AgentID int,
	@PriceOverride decimal(18, 2),
	@RoomName varchar(50)
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
	[RoomName] = @RoomName
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
	[RoomName]
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
	[RoomName]
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
	[RoomName]
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
	[RoomName]
FROM [dbo].[ItineraryMember]
WHERE
	[ContactID] = @ContactID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PurchaseItem_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[PurchaseItem_Ins]
GO

CREATE PROCEDURE [dbo].[PurchaseItem_Ins]
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
	@IsInvoiced bit,
	@SortDate datetime,
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
	[DiscountType],
	[IsInvoiced],
	[SortDate]
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
	@DiscountType,
	@IsInvoiced,
	@SortDate
)
SELECT @PurchaseItemID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PurchaseItem_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[PurchaseItem_Upd]
GO

CREATE PROCEDURE [dbo].[PurchaseItem_Upd]
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
	@DiscountType varchar(10),
	@IsInvoiced bit,
	@SortDate datetime
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
	[DiscountType] = @DiscountType,
	[IsInvoiced] = @IsInvoiced,
	[SortDate] = @SortDate
WHERE
	[PurchaseItemID] = @PurchaseItemID
	AND [RowVersion] = @RowVersion
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PurchaseItem_Upd_ByPurchaseLineID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[PurchaseItem_Upd_ByPurchaseLineID]
GO

CREATE PROCEDURE [dbo].[PurchaseItem_Upd_ByPurchaseLineID]
	@PurchaseLineID int,
	@PurchaseLineIDOld int
AS
UPDATE [dbo].[PurchaseItem]
SET
	[PurchaseLineID] = @PurchaseLineID
WHERE
	[PurchaseLineID] = @PurchaseLineIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PurchaseItem_Upd_ByOptionID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[PurchaseItem_Upd_ByOptionID]
GO

CREATE PROCEDURE [dbo].[PurchaseItem_Upd_ByOptionID]
	@OptionID int,
	@OptionIDOld int
AS
UPDATE [dbo].[PurchaseItem]
SET
	[OptionID] = @OptionID
WHERE
	[OptionID] = @OptionIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PurchaseItem_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[PurchaseItem_Del]
GO

CREATE PROCEDURE [dbo].[PurchaseItem_Del]
	@PurchaseItemID int,
	@RowVersion timestamp
AS
DELETE FROM [dbo].[PurchaseItem]
WHERE
	[PurchaseItemID] = @PurchaseItemID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PurchaseItem_Del_ByPurchaseLineID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[PurchaseItem_Del_ByPurchaseLineID]
GO

CREATE PROCEDURE [dbo].[PurchaseItem_Del_ByPurchaseLineID]
	@PurchaseLineID int
AS
DELETE
FROM [dbo].[PurchaseItem]
WHERE
	[PurchaseLineID] = @PurchaseLineID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PurchaseItem_Del_ByOptionID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[PurchaseItem_Del_ByOptionID]
GO

CREATE PROCEDURE [dbo].[PurchaseItem_Del_ByOptionID]
	@OptionID int
AS
DELETE
FROM [dbo].[PurchaseItem]
WHERE
	[OptionID] = @OptionID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PurchaseItem_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[PurchaseItem_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[PurchaseItem_Sel_ByID]
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
	[DiscountType],
	[IsInvoiced],
	[SortDate]
FROM [dbo].[PurchaseItem]
WHERE
	[PurchaseItemID] = @PurchaseItemID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PurchaseItem_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[PurchaseItem_Sel_All]
GO

CREATE PROCEDURE [dbo].[PurchaseItem_Sel_All]
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
	[DiscountType],
	[IsInvoiced],
	[SortDate]
FROM [dbo].[PurchaseItem]
ORDER BY 
	[PurchaseItemID] ASC
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PurchaseItem_Sel_ByPurchaseLineID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[PurchaseItem_Sel_ByPurchaseLineID]
GO

CREATE PROCEDURE [dbo].[PurchaseItem_Sel_ByPurchaseLineID]
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
	[DiscountType],
	[IsInvoiced],
	[SortDate]
FROM [dbo].[PurchaseItem]
WHERE
	[PurchaseLineID] = @PurchaseLineID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PurchaseItem_Sel_ByOptionID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[PurchaseItem_Sel_ByOptionID]
GO

CREATE PROCEDURE [dbo].[PurchaseItem_Sel_ByOptionID]
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
	[DiscountType],
	[IsInvoiced],
	[SortDate]
FROM [dbo].[PurchaseItem]
WHERE
	[OptionID] = @OptionID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RoomType_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[RoomType_Ins]
GO

CREATE PROCEDURE [dbo].[RoomType_Ins]
	@ItineraryID int,
	@OptionTypeID int,
	@RoomTypeName varchar(100),
	@Quantity int,
	@RoomTypeID int OUTPUT
AS
INSERT [dbo].[RoomType]
(
	[ItineraryID],
	[OptionTypeID],
	[RoomTypeName],
	[Quantity]
)
VALUES
(
	@ItineraryID,
	@OptionTypeID,
	@RoomTypeName,
	@Quantity
)
SELECT @RoomTypeID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RoomType_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[RoomType_Upd]
GO

CREATE PROCEDURE [dbo].[RoomType_Upd]
	@RoomTypeID int,
	@ItineraryID int,
	@OptionTypeID int,
	@RoomTypeName varchar(100),
	@Quantity int
AS
UPDATE [dbo].[RoomType]
SET 
	[ItineraryID] = @ItineraryID,
	[OptionTypeID] = @OptionTypeID,
	[RoomTypeName] = @RoomTypeName,
	[Quantity] = @Quantity
WHERE
	[RoomTypeID] = @RoomTypeID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RoomType_Upd_ByItineraryID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[RoomType_Upd_ByItineraryID]
GO

CREATE PROCEDURE [dbo].[RoomType_Upd_ByItineraryID]
	@ItineraryID int,
	@ItineraryIDOld int
AS
UPDATE [dbo].[RoomType]
SET
	[ItineraryID] = @ItineraryID
WHERE
	[ItineraryID] = @ItineraryIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RoomType_Upd_ByOptionTypeID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[RoomType_Upd_ByOptionTypeID]
GO

CREATE PROCEDURE [dbo].[RoomType_Upd_ByOptionTypeID]
	@OptionTypeID int,
	@OptionTypeIDOld int
AS
UPDATE [dbo].[RoomType]
SET
	[OptionTypeID] = @OptionTypeID
WHERE
	[OptionTypeID] = @OptionTypeIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RoomType_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[RoomType_Del]
GO

CREATE PROCEDURE [dbo].[RoomType_Del]
	@RoomTypeID int
AS
DELETE FROM [dbo].[RoomType]
WHERE
	[RoomTypeID] = @RoomTypeID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RoomType_Del_ByItineraryID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[RoomType_Del_ByItineraryID]
GO

CREATE PROCEDURE [dbo].[RoomType_Del_ByItineraryID]
	@ItineraryID int
AS
DELETE
FROM [dbo].[RoomType]
WHERE
	[ItineraryID] = @ItineraryID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RoomType_Del_ByOptionTypeID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[RoomType_Del_ByOptionTypeID]
GO

CREATE PROCEDURE [dbo].[RoomType_Del_ByOptionTypeID]
	@OptionTypeID int
AS
DELETE
FROM [dbo].[RoomType]
WHERE
	[OptionTypeID] = @OptionTypeID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RoomType_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[RoomType_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[RoomType_Sel_ByID]
	@RoomTypeID int
AS
SET NOCOUNT ON
SELECT
	[RoomTypeID],
	[ItineraryID],
	[OptionTypeID],
	[RoomTypeName],
	[Quantity]
FROM [dbo].[RoomType]
WHERE
	[RoomTypeID] = @RoomTypeID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RoomType_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[RoomType_Sel_All]
GO

CREATE PROCEDURE [dbo].[RoomType_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[RoomTypeID],
	[ItineraryID],
	[OptionTypeID],
	[RoomTypeName],
	[Quantity]
FROM [dbo].[RoomType]
ORDER BY 
	[RoomTypeID] ASC
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RoomType_Sel_ByItineraryID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[RoomType_Sel_ByItineraryID]
GO

CREATE PROCEDURE [dbo].[RoomType_Sel_ByItineraryID]
	@ItineraryID int
AS
SET NOCOUNT ON
SELECT
	[RoomTypeID],
	[ItineraryID],
	[OptionTypeID],
	[RoomTypeName],
	[Quantity]
FROM [dbo].[RoomType]
WHERE
	[ItineraryID] = @ItineraryID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RoomType_Sel_ByOptionTypeID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[RoomType_Sel_ByOptionTypeID]
GO

CREATE PROCEDURE [dbo].[RoomType_Sel_ByOptionTypeID]
	@OptionTypeID int
AS
SET NOCOUNT ON
SELECT
	[RoomTypeID],
	[ItineraryID],
	[OptionTypeID],
	[RoomTypeName],
	[Quantity]
FROM [dbo].[RoomType]
WHERE
	[OptionTypeID] = @OptionTypeID
GO

GO

PRINT N'Refreshing [dbo].[ItineraryServiceTypePricing]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryServiceTypePricing';


GO
PRINT N'Refreshing [dbo].[PurchaseItemPayments]...';


GO
--EXECUTE sp_refreshsqlmodule N'dbo.PurchaseItemPayments';


GO
PRINT N'Refreshing [dbo].[_DataExtract_ItineraryFinancials]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._DataExtract_ItineraryFinancials';


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
PRINT N'Refreshing [dbo].[ItineraryMember_Upd_ByContactID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryMember_Upd_ByContactID';


GO
PRINT N'Refreshing [dbo].[ItineraryMember_Upd_ByItineraryGroupID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryMember_Upd_ByItineraryGroupID';


GO
PRINT N'Refreshing [dbo].[_ItinerarySet_Copy_ByID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._ItinerarySet_Copy_ByID';


GO
PRINT N'Refreshing [dbo].[_Report_WhoUsedSupplier]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._Report_WhoUsedSupplier';


GO
PRINT N'Refreshing [dbo].[PurchaseItem_Del]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.PurchaseItem_Del';


GO
PRINT N'Refreshing [dbo].[PurchaseItem_Del_ByOptionID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.PurchaseItem_Del_ByOptionID';


GO
PRINT N'Refreshing [dbo].[PurchaseItem_Del_ByPurchaseLineID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.PurchaseItem_Del_ByPurchaseLineID';


GO
PRINT N'Refreshing [dbo].[PurchaseItem_Upd_ByOptionID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.PurchaseItem_Upd_ByOptionID';


GO
PRINT N'Refreshing [dbo].[PurchaseItem_Upd_ByPurchaseLineID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.PurchaseItem_Upd_ByPurchaseLineID';



exec __RefreshViews;
GO
----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2012.10.10'
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