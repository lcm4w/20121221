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
if ((select VersionNumber from AppSettings) <> '2012.11.12')
	RAISERROR (N'Database Update Script is not correct version for current database version',17,1)

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------

GO
PRINT N'Altering [dbo].[Contact]...';

GO
if not Exists(select * from sys.columns where Name = N'UpdatedOn' and Object_ID = Object_ID(N'Contact'))
	ALTER TABLE [dbo].[Contact] 
		ADD [UpdatedOn] datetime;
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
	[UpdatedOn]
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
    UpdatedOn
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


ALTER PROCEDURE [dbo].[_Supplier_New]
@SupplierName VARCHAR (100), @ParentFolderID INT, @AddedOn DATETIME, @AddedBy INT
AS
SET NOCOUNT ON

INSERT INTO [dbo].[Supplier]
(
	[SupplierName],
	[IsRecordActive],
	[ParentFolderID],
	[AddedOn],
	[AddedBy],
	[UpdatedOn]
)
VALUES
(
	@SupplierName,
	1,
	@ParentFolderID,
	@AddedOn,
	@AddedBy,
	getdate()
)
SELECT SCOPE_IDENTITY()
GO


ALTER PROCEDURE [dbo].[_Itinerary_New]
	@ItineraryName varchar(100),
	@ArriveDate datetime,
	@ParentFolderID int,
	@AddedOn datetime,
	@AddedBy int
AS
SET NOCOUNT ON

DECLARE @DefaultAgentID int
SELECT @DefaultAgentID = (SELECT TOP 1 (AgentID) FROM Agent order by IsDefaultAgent desc)

INSERT INTO [dbo].[Itinerary]
(
	[ItineraryName],
	[ArriveDate],
	[NetComOrMup],
	[IsRecordActive],
	[ParentFolderID],
	[AgentID],
	[AddedOn],
	[AddedBy],
	[AssignedTo],
	[UpdatedOn]
)
SELECT
	@ItineraryName,
	@ArriveDate,
	ISNULL([NetComOrMup], 'mup'),
	1,
	@ParentFolderID,
	@DefaultAgentID,
	@AddedOn,
	@AddedBy,
	@AddedBy,
	getdate()
FROM Agent WHERE AgentID = @DefaultAgentID

DECLARE @ItineraryID int
SET @ItineraryID = SCOPE_IDENTITY()

-- Update itinerary custom code
if ((select top(1) CustomCodeFormat from AppSettings) is not null)
begin
	update Itinerary set
		CustomCode = (select replace((select top(1) CustomCodeFormat from AppSettings), '[!ItineraryID]', @ItineraryID)),
		ItineraryName = (select replace((select top(1) CustomCodeFormat from AppSettings), '[!ItineraryID]', @ItineraryID))
	where ItineraryID = @ItineraryID
end

-- Add the first itinerary group
INSERT [dbo].[ItineraryGroup]
(
	[ItineraryID],
	[ItineraryGroupName],
	[AddedOn],
	[AddedBy]
)
VALUES
(
	@ItineraryID,
	@ItineraryName,
	@AddedOn,
	@AddedBy
)

-- Add the default Agent margin overrides
INSERT [dbo].[ItineraryMarginOverride]
(
	[ServiceTypeID],
	[Margin],
	[ItineraryID]
)
SELECT 
	[ServiceTypeID], 
	[Margin], 
	@ItineraryID 
FROM AgentMargin WHERE AgentID = @DefaultAgentID

SELECT @ItineraryID
GO


ALTER PROCEDURE [dbo].[_Contact_New]
@ContactName VARCHAR (100), @ParentFolderID INT, @AddedOn DATETIME, @AddedBy INT
AS
SET NOCOUNT ON

INSERT INTO [dbo].[Contact]
(
	[ContactName],
	[IsRecordActive],
	[ParentFolderID],
	[AddedOn],
	[AddedBy],
	[UpdatedOn]
)
VALUES
(
	@ContactName,
	1,
	@ParentFolderID,
	@AddedOn,
	@AddedBy,
	getdate()
)
SELECT SCOPE_IDENTITY()
GO

-----------------------------------------------------------------------------------------------------------------------------------------


GO
if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GroupPrice]') and OBJECTPROPERTY(id, N'IsTable') = 1)
begin
	CREATE TABLE [GroupPrice](
		[GroupPriceID] [int] IDENTITY(1,1) NOT NULL,
		[GroupPriceName] [varchar](100) NOT NULL,
		[ItineraryID] [int] NOT NULL,
		[ItineraryPaxID] [int] NULL,
		[OptionTypeID] [int] NULL,
		[Price] [money] NULL,
		[Markup] [money] NULL,
		[PriceOverride] [money] NULL,
	 CONSTRAINT [PK_GroupPrice] PRIMARY KEY CLUSTERED 
	(
		[GroupPriceID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]


	ALTER TABLE [GroupPrice]  WITH CHECK ADD  CONSTRAINT [FK_GroupPrice_GroupPrice] FOREIGN KEY([GroupPriceID])
	REFERENCES [GroupPrice] ([GroupPriceID])

	ALTER TABLE [GroupPrice] CHECK CONSTRAINT [FK_GroupPrice_GroupPrice]

	ALTER TABLE [GroupPrice]  WITH CHECK ADD  CONSTRAINT [FK_GroupPrice_Itinerary] FOREIGN KEY([ItineraryID])
	REFERENCES [Itinerary] ([ItineraryID])

	ALTER TABLE [GroupPrice] CHECK CONSTRAINT [FK_GroupPrice_Itinerary]

	ALTER TABLE [GroupPrice]  WITH CHECK ADD  CONSTRAINT [FK_GroupPrice_ItineraryPax] FOREIGN KEY([ItineraryPaxID])
	REFERENCES [ItineraryPax] ([ItineraryPaxID])
	ON UPDATE CASCADE
	ON DELETE CASCADE

	ALTER TABLE [GroupPrice] CHECK CONSTRAINT [FK_GroupPrice_ItineraryPax]

	ALTER TABLE [GroupPrice]  WITH CHECK ADD  CONSTRAINT [FK_GroupPrice_OptionType] FOREIGN KEY([OptionTypeID])
	REFERENCES [OptionType] ([OptionTypeID])
	ON UPDATE CASCADE
	ON DELETE CASCADE

	ALTER TABLE [GroupPrice] CHECK CONSTRAINT [FK_GroupPrice_OptionType]

end
GO

----------------------------


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

-----------------------------------------------------------------------------------------------------------------------------------------


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
	[UpdatedOn]
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
	@UpdatedOn
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
	@UpdatedOn datetime
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
	[UpdatedOn] = @UpdatedOn
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
	[UpdatedOn]
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
	[UpdatedOn]
FROM [dbo].[Contact]
ORDER BY 
	[ContactID] ASC
GO


GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GroupPrice_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[GroupPrice_Ins]
GO

CREATE PROCEDURE [dbo].[GroupPrice_Ins]
	@GroupPriceName varchar(100),
	@ItineraryID int,
	@ItineraryPaxID int,
	@OptionTypeID int,
	@Price money,
	@Markup money,
	@PriceOverride money,
	@GroupPriceID int OUTPUT
AS
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
VALUES
(
	@GroupPriceName,
	@ItineraryID,
	@ItineraryPaxID,
	@OptionTypeID,
	@Price,
	@Markup,
	@PriceOverride
)
SELECT @GroupPriceID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GroupPrice_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[GroupPrice_Upd]
GO

CREATE PROCEDURE [dbo].[GroupPrice_Upd]
	@GroupPriceID int,
	@GroupPriceName varchar(100),
	@ItineraryID int,
	@ItineraryPaxID int,
	@OptionTypeID int,
	@Price money,
	@Markup money,
	@PriceOverride money
AS
UPDATE [dbo].[GroupPrice]
SET 
	[GroupPriceName] = @GroupPriceName,
	[ItineraryID] = @ItineraryID,
	[ItineraryPaxID] = @ItineraryPaxID,
	[OptionTypeID] = @OptionTypeID,
	[Price] = @Price,
	[Markup] = @Markup,
	[PriceOverride] = @PriceOverride
WHERE
	[GroupPriceID] = @GroupPriceID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GroupPrice_Upd_ByItineraryID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[GroupPrice_Upd_ByItineraryID]
GO

CREATE PROCEDURE [dbo].[GroupPrice_Upd_ByItineraryID]
	@ItineraryID int,
	@ItineraryIDOld int
AS
UPDATE [dbo].[GroupPrice]
SET
	[ItineraryID] = @ItineraryID
WHERE
	[ItineraryID] = @ItineraryIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GroupPrice_Upd_ByItineraryPaxID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[GroupPrice_Upd_ByItineraryPaxID]
GO

CREATE PROCEDURE [dbo].[GroupPrice_Upd_ByItineraryPaxID]
	@ItineraryPaxID int,
	@ItineraryPaxIDOld int
AS
UPDATE [dbo].[GroupPrice]
SET
	[ItineraryPaxID] = @ItineraryPaxID
WHERE
	[ItineraryPaxID] = @ItineraryPaxIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GroupPrice_Upd_ByOptionTypeID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[GroupPrice_Upd_ByOptionTypeID]
GO

CREATE PROCEDURE [dbo].[GroupPrice_Upd_ByOptionTypeID]
	@OptionTypeID int,
	@OptionTypeIDOld int
AS
UPDATE [dbo].[GroupPrice]
SET
	[OptionTypeID] = @OptionTypeID
WHERE
	[OptionTypeID] = @OptionTypeIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GroupPrice_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[GroupPrice_Del]
GO

CREATE PROCEDURE [dbo].[GroupPrice_Del]
	@GroupPriceID int
AS
DELETE FROM [dbo].[GroupPrice]
WHERE
	[GroupPriceID] = @GroupPriceID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GroupPrice_Del_ByItineraryID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[GroupPrice_Del_ByItineraryID]
GO

CREATE PROCEDURE [dbo].[GroupPrice_Del_ByItineraryID]
	@ItineraryID int
AS
DELETE
FROM [dbo].[GroupPrice]
WHERE
	[ItineraryID] = @ItineraryID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GroupPrice_Del_ByItineraryPaxID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[GroupPrice_Del_ByItineraryPaxID]
GO

CREATE PROCEDURE [dbo].[GroupPrice_Del_ByItineraryPaxID]
	@ItineraryPaxID int
AS
DELETE
FROM [dbo].[GroupPrice]
WHERE
	[ItineraryPaxID] = @ItineraryPaxID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GroupPrice_Del_ByOptionTypeID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[GroupPrice_Del_ByOptionTypeID]
GO

CREATE PROCEDURE [dbo].[GroupPrice_Del_ByOptionTypeID]
	@OptionTypeID int
AS
DELETE
FROM [dbo].[GroupPrice]
WHERE
	[OptionTypeID] = @OptionTypeID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GroupPrice_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[GroupPrice_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[GroupPrice_Sel_ByID]
	@GroupPriceID int
AS
SET NOCOUNT ON
SELECT
	[GroupPriceID],
	[GroupPriceName],
	[ItineraryID],
	[ItineraryPaxID],
	[OptionTypeID],
	[Price],
	[Markup],
	[PriceOverride]
FROM [dbo].[GroupPrice]
WHERE
	[GroupPriceID] = @GroupPriceID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GroupPrice_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[GroupPrice_Sel_All]
GO

CREATE PROCEDURE [dbo].[GroupPrice_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[GroupPriceID],
	[GroupPriceName],
	[ItineraryID],
	[ItineraryPaxID],
	[OptionTypeID],
	[Price],
	[Markup],
	[PriceOverride]
FROM [dbo].[GroupPrice]
ORDER BY 
	[GroupPriceID] ASC
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GroupPrice_Sel_ByItineraryID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[GroupPrice_Sel_ByItineraryID]
GO

CREATE PROCEDURE [dbo].[GroupPrice_Sel_ByItineraryID]
	@ItineraryID int
AS
SET NOCOUNT ON
SELECT
	[GroupPriceID],
	[GroupPriceName],
	[ItineraryID],
	[ItineraryPaxID],
	[OptionTypeID],
	[Price],
	[Markup],
	[PriceOverride]
FROM [dbo].[GroupPrice]
WHERE
	[ItineraryID] = @ItineraryID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GroupPrice_Sel_ByItineraryPaxID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[GroupPrice_Sel_ByItineraryPaxID]
GO

CREATE PROCEDURE [dbo].[GroupPrice_Sel_ByItineraryPaxID]
	@ItineraryPaxID int
AS
SET NOCOUNT ON
SELECT
	[GroupPriceID],
	[GroupPriceName],
	[ItineraryID],
	[ItineraryPaxID],
	[OptionTypeID],
	[Price],
	[Markup],
	[PriceOverride]
FROM [dbo].[GroupPrice]
WHERE
	[ItineraryPaxID] = @ItineraryPaxID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[GroupPrice_Sel_ByOptionTypeID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[GroupPrice_Sel_ByOptionTypeID]
GO

CREATE PROCEDURE [dbo].[GroupPrice_Sel_ByOptionTypeID]
	@OptionTypeID int
AS
SET NOCOUNT ON
SELECT
	[GroupPriceID],
	[GroupPriceName],
	[ItineraryID],
	[ItineraryPaxID],
	[OptionTypeID],
	[Price],
	[Markup],
	[PriceOverride]
FROM [dbo].[GroupPrice]
WHERE
	[OptionTypeID] = @OptionTypeID
GO




GO
PRINT N'Refreshing [dbo].[ItineraryDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryDetail';


GO
PRINT N'Refreshing [dbo].[ItineraryClientDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryClientDetail';


GO
PRINT N'Refreshing [dbo].[ItineraryPaymentDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryPaymentDetail';


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
PRINT N'Altering [dbo].[_Contact_New]...';


GO

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
PRINT N'Refreshing [dbo].[_Contact_Rename]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._Contact_Rename';


GO
PRINT N'Refreshing [dbo].[_ContactSet_Copy_ByID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._ContactSet_Copy_ByID';


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
PRINT N'Refreshing [dbo].[_UserSet_Sel_ByUsernamePassword]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._UserSet_Sel_ByUsernamePassword';


GO
PRINT N'Checking existing data against newly created constraints';



exec __RefreshViews;
GO

update itinerary set UpdatedOn = AddedOn where UpdatedOn is null;
update supplier set UpdatedOn = AddedOn where UpdatedOn is null;
update contact set UpdatedOn = AddedOn where UpdatedOn is null;

----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2012.11.13'
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