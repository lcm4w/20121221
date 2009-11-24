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
if ((select VersionNumber from AppSettings) <> '2009.10.19')
	RAISERROR (N'Update script version is invalid for this db version',17,1)	
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
----------------------------------------------------------------------------------------

PRINT N'Dropping DF_LineItem_AddedOn...';

GO
ALTER TABLE [dbo].[PurchaseItem] DROP CONSTRAINT [DF_LineItem_AddedOn];

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
PRINT N'Dropping FK_PurchaseItem_PurchaseLine...';

GO
ALTER TABLE [dbo].[PurchaseItem] DROP CONSTRAINT [FK_PurchaseItem_PurchaseLine];

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
PRINT N'Dropping FK_PurchaseItem_Option...';

GO
ALTER TABLE [dbo].[PurchaseItem] DROP CONSTRAINT [FK_PurchaseItem_Option];

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
PRINT N'Altering [dbo].[ItineraryPax]...';

GO
ALTER TABLE [dbo].[ItineraryPax] ALTER COLUMN [MemberRooms] DECIMAL (8, 4) NULL;
ALTER TABLE [dbo].[ItineraryPax] ALTER COLUMN [StaffRooms] DECIMAL (8, 4) NULL;

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
PRINT N'Altering [dbo].[OptionType]...';

GO
ALTER TABLE [dbo].[OptionType] ALTER COLUMN [OptionTypeName] VARCHAR (100) NOT NULL;

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
PRINT N'Starting rebuilding table [dbo].[PurchaseItem]...';

GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
SET XACT_ABORT ON;

GO
BEGIN TRANSACTION;
CREATE TABLE [dbo].[tmp_ms_xx_PurchaseItem] (
    [PurchaseItemID]     INT             IDENTITY (1, 1) NOT NULL,
    [PurchaseLineID]     INT             NOT NULL,
    [OptionID]           INT             NOT NULL,
    [PurchaseItemName]   VARCHAR (255)   NULL,
    [BookingReference]   VARCHAR (50)    NULL,
    [StartDate]          DATETIME        NULL,
    [StartTime]          DATETIME        NULL,
    [EndDate]            DATETIME        NULL,
    [EndTime]            DATETIME        NULL,
    [Net]                MONEY           NULL,
    [Gross]              MONEY           NULL,
    [CurrencyRate]       MONEY           NULL,
    [Quantity]           DECIMAL (12, 4) NULL,
    [NumberOfDays]       DECIMAL (12, 4) NULL,
    [PaymentTermID]      INT             NULL,
    [RequestStatusID]    INT             NULL,
    [IsLockedAccounting] BIT             NULL,
    [AddedOn]            DATETIME        CONSTRAINT [DF_LineItem_AddedOn] DEFAULT (getdate()) NULL,
    [AddedBy]            INT             NULL,
    [RowVersion]         TIMESTAMP       NULL
);
ALTER TABLE [dbo].[tmp_ms_xx_PurchaseItem]
    ADD CONSTRAINT [tmp_ms_xx_clusteredindex_PK_LineItem] PRIMARY KEY CLUSTERED ([PurchaseItemID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);
IF EXISTS (SELECT TOP 1 1
           FROM   [dbo].[PurchaseItem])
    BEGIN
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_PurchaseItem] ON;
        INSERT INTO [dbo].[tmp_ms_xx_PurchaseItem] ([PurchaseItemID], [PurchaseLineID], [OptionID], [PurchaseItemName], [BookingReference], [StartDate], [StartTime], [EndDate], [EndTime], [Net], [Gross], [CurrencyRate], [Quantity], [NumberOfDays], [PaymentTermID], [RequestStatusID], [IsLockedAccounting], [AddedOn], [AddedBy])
        SELECT   [PurchaseItemID],
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
                 CAST ([Quantity] AS DECIMAL (12, 4)),
                 CAST ([NumberOfDays] AS DECIMAL (12, 4)),
                 [PaymentTermID],
                 [RequestStatusID],
                 [IsLockedAccounting],
                 [AddedOn],
                 [AddedBy]
        FROM     [dbo].[PurchaseItem]
        ORDER BY [PurchaseItemID] ASC;
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_PurchaseItem] OFF;
    END
DROP TABLE [dbo].[PurchaseItem];
EXECUTE sp_rename N'[dbo].[tmp_ms_xx_PurchaseItem]', N'PurchaseItem';
EXECUTE sp_rename N'[dbo].[tmp_ms_xx_clusteredindex_PK_LineItem]', N'PK_LineItem', N'OBJECT';
COMMIT TRANSACTION;

GO
SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

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
PRINT N'Creating [dbo].[PurchaseItem].[IX_PurchaseItem_PurchaseLineID]...';

GO
CREATE NONCLUSTERED INDEX [IX_PurchaseItem_PurchaseLineID]
    ON [dbo].[PurchaseItem]([PurchaseLineID] ASC, [Net] ASC, [CurrencyRate] ASC, [Quantity] ASC, [NumberOfDays] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0);

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
PRINT N'Creating [dbo].[ItineraryPaxOverride]...';

GO
CREATE TABLE [dbo].[ItineraryPaxOverride] (
    [PurchaseItemID] INT            NOT NULL,
    [ItineraryPaxID] INT            NOT NULL,
    [MemberCount]    INT            NULL,
    [MemberRooms]    DECIMAL (8, 4) NULL,
    [StaffCount]     INT            NULL,
    [StaffRooms]     DECIMAL (8, 4) NULL
);

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
PRINT N'Creating PK_ItineraryPaxOverride...';

GO
ALTER TABLE [dbo].[ItineraryPaxOverride]
    ADD CONSTRAINT [PK_ItineraryPaxOverride] PRIMARY KEY CLUSTERED ([PurchaseItemID] ASC, [ItineraryPaxID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

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
PRINT N'Creating FK_PurchaseItem_PurchaseLine...';

GO
ALTER TABLE [dbo].[PurchaseItem] WITH NOCHECK
    ADD CONSTRAINT [FK_PurchaseItem_PurchaseLine] FOREIGN KEY ([PurchaseLineID]) REFERENCES [dbo].[PurchaseLine] ([PurchaseLineID]) ON DELETE CASCADE ON UPDATE CASCADE;

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
PRINT N'Creating FK_PurchaseItem_Option...';

GO
ALTER TABLE [dbo].[PurchaseItem] WITH NOCHECK
    ADD CONSTRAINT [FK_PurchaseItem_Option] FOREIGN KEY ([OptionID]) REFERENCES [dbo].[Option] ([OptionID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

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
PRINT N'Creating FK_ItineraryPaxOverride_PurchaseItem...';

GO
ALTER TABLE [dbo].[ItineraryPaxOverride] WITH NOCHECK
    ADD CONSTRAINT [FK_ItineraryPaxOverride_PurchaseItem] FOREIGN KEY ([PurchaseItemID]) REFERENCES [dbo].[PurchaseItem] ([PurchaseItemID]) ON DELETE CASCADE ON UPDATE CASCADE;

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
PRINT N'Creating FK_ItineraryPaxOverride_ItineraryPax...';

GO
ALTER TABLE [dbo].[ItineraryPaxOverride] WITH NOCHECK
    ADD CONSTRAINT [FK_ItineraryPaxOverride_ItineraryPax] FOREIGN KEY ([ItineraryPaxID]) REFERENCES [dbo].[ItineraryPax] ([ItineraryPaxID]) ON DELETE NO ACTION ON UPDATE NO ACTION;

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
	[IsDeleted]
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
PRINT N'Altering [dbo].[ItineraryPax_Ins]...';

GO
ALTER PROCEDURE [dbo].[ItineraryPax_Ins]
	@ItineraryPaxName varchar(50),
	@ItineraryID int,
	@MemberCount int,
	@MemberRooms decimal(8, 4),
	@StaffCount int,
	@StaffRooms decimal(8, 4),
	@ItineraryPaxID int OUTPUT
AS
INSERT [dbo].[ItineraryPax]
(
	[ItineraryPaxName],
	[ItineraryID],
	[MemberCount],
	[MemberRooms],
	[StaffCount],
	[StaffRooms]
)
VALUES
(
	@ItineraryPaxName,
	@ItineraryID,
	@MemberCount,
	@MemberRooms,
	@StaffCount,
	@StaffRooms
)
SELECT @ItineraryPaxID=SCOPE_IDENTITY()
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
PRINT N'Altering [dbo].[ItineraryPax_Upd]...';

GO
ALTER PROCEDURE [dbo].[ItineraryPax_Upd]
	@ItineraryPaxID int,
	@ItineraryPaxName varchar(50),
	@ItineraryID int,
	@MemberCount int,
	@MemberRooms decimal(8, 4),
	@StaffCount int,
	@StaffRooms decimal(8, 4)
AS
UPDATE [dbo].[ItineraryPax]
SET 
	[ItineraryPaxName] = @ItineraryPaxName,
	[ItineraryID] = @ItineraryID,
	[MemberCount] = @MemberCount,
	[MemberRooms] = @MemberRooms,
	[StaffCount] = @StaffCount,
	[StaffRooms] = @StaffRooms
WHERE
	[ItineraryPaxID] = @ItineraryPaxID
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
PRINT N'Altering [dbo].[OptionType_Ins]...';

GO
ALTER PROCEDURE [dbo].[OptionType_Ins]
	@OptionTypeName varchar(100),
	@Divisor int,
	@OptionTypeID int OUTPUT
AS
INSERT [dbo].[OptionType]
(
	[OptionTypeName],
	[Divisor]
)
VALUES
(
	@OptionTypeName,
	@Divisor
)
SELECT @OptionTypeID=SCOPE_IDENTITY()
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
PRINT N'Altering [dbo].[OptionType_Upd]...';

GO
ALTER PROCEDURE [dbo].[OptionType_Upd]
	@OptionTypeID int,
	@OptionTypeName varchar(100),
	@Divisor int
AS
UPDATE [dbo].[OptionType]
SET 
	[OptionTypeName] = @OptionTypeName,
	[Divisor] = @Divisor
WHERE
	[OptionTypeID] = @OptionTypeID
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
	[AddedBy]
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
	@AddedBy
)
SELECT @PurchaseItemID=SCOPE_IDENTITY()
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
	@RowVersion timestamp
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
	[AddedBy] = @AddedBy
WHERE
	[PurchaseItemID] = @PurchaseItemID
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
PRINT N'Creating [dbo].[ItineraryPaxOverride_Del]...';

GO
CREATE PROCEDURE [dbo].[ItineraryPaxOverride_Del]
	@PurchaseItemID int,
	@ItineraryPaxID int
AS
DELETE FROM [dbo].[ItineraryPaxOverride]
WHERE
	[PurchaseItemID] = @PurchaseItemID
	AND [ItineraryPaxID] = @ItineraryPaxID
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
PRINT N'Creating [dbo].[ItineraryPaxOverride_Del_ByItineraryPaxID]...';

GO
CREATE PROCEDURE [dbo].[ItineraryPaxOverride_Del_ByItineraryPaxID]
	@ItineraryPaxID int
AS
DELETE FROM [dbo].[ItineraryPaxOverride]
WHERE
	[ItineraryPaxID] = @ItineraryPaxID
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
PRINT N'Creating [dbo].[ItineraryPaxOverride_Del_ByPurchaseItemID]...';

GO
CREATE PROCEDURE [dbo].[ItineraryPaxOverride_Del_ByPurchaseItemID]
	@PurchaseItemID int
AS
DELETE FROM [dbo].[ItineraryPaxOverride]
WHERE
	[PurchaseItemID] = @PurchaseItemID
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
PRINT N'Creating [dbo].[ItineraryPaxOverride_Ins]...';

GO
CREATE PROCEDURE [dbo].[ItineraryPaxOverride_Ins]
	@PurchaseItemID int,
	@ItineraryPaxID int,
	@MemberCount int,
	@MemberRooms decimal(8, 4),
	@StaffCount int,
	@StaffRooms decimal(8, 4)
AS
INSERT [dbo].[ItineraryPaxOverride]
(
	[PurchaseItemID],
	[ItineraryPaxID],
	[MemberCount],
	[MemberRooms],
	[StaffCount],
	[StaffRooms]
)
VALUES
(
	@PurchaseItemID,
	@ItineraryPaxID,
	@MemberCount,
	@MemberRooms,
	@StaffCount,
	@StaffRooms
)
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
PRINT N'Creating [dbo].[ItineraryPaxOverride_Sel_All]...';

GO
CREATE PROCEDURE [dbo].[ItineraryPaxOverride_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[PurchaseItemID],
	[ItineraryPaxID],
	[MemberCount],
	[MemberRooms],
	[StaffCount],
	[StaffRooms]
FROM [dbo].[ItineraryPaxOverride]
ORDER BY 
	[PurchaseItemID] ASC
	, [ItineraryPaxID] ASC
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
PRINT N'Creating [dbo].[ItineraryPaxOverride_Sel_ByID]...';

GO
CREATE PROCEDURE [dbo].[ItineraryPaxOverride_Sel_ByID]
	@PurchaseItemID int,
	@ItineraryPaxID int
AS
SET NOCOUNT ON
SELECT
	[PurchaseItemID],
	[ItineraryPaxID],
	[MemberCount],
	[MemberRooms],
	[StaffCount],
	[StaffRooms]
FROM [dbo].[ItineraryPaxOverride]
WHERE
	[PurchaseItemID] = @PurchaseItemID
	AND [ItineraryPaxID] = @ItineraryPaxID
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
PRINT N'Creating [dbo].[ItineraryPaxOverride_Upd]...';

GO
CREATE PROCEDURE [dbo].[ItineraryPaxOverride_Upd]
	@PurchaseItemID int,
	@ItineraryPaxID int,
	@MemberCount int,
	@MemberRooms decimal(8, 4),
	@StaffCount int,
	@StaffRooms decimal(8, 4)
AS
UPDATE [dbo].[ItineraryPaxOverride]
SET 
	[MemberCount] = @MemberCount,
	[MemberRooms] = @MemberRooms,
	[StaffCount] = @StaffCount,
	[StaffRooms] = @StaffRooms
WHERE
	[PurchaseItemID] = @PurchaseItemID
	AND [ItineraryPaxID] = @ItineraryPaxID
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
PRINT N'Altering [dbo].[GetPaymentDate]...';

GO

ALTER FUNCTION [dbo].[GetPaymentDate] 
(
	@date datetime,
	@period int,
	@terms varchar(100)
)
RETURNS datetime
AS
BEGIN
	DECLARE @Result datetime

	SELECT @Result = CASE @terms
	
		WHEN 'days before' 
			THEN DATEADD(dd, -@period, @date)

		WHEN 'days after' 
			THEN DATEADD(dd, @period, @date)

		WHEN 'day of the month' 
			THEN CASE 
				WHEN @PERIOD > day(dateadd(mm,datediff(mm,-1,@date),-1))
					THEN dateadd(ms,-3,DATEADD(mm, DATEDIFF(m,0,getdate()  )+1, 0))
					ELSE @Date + @period - day(@date)
				END

		WHEN 'day of the month following'
			THEN DATEADD(mm, 1, DATEADD(dd, (@period - day(@date)), @date))

		ELSE NULL
	
	END

	RETURN @Result

END

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
		ntax.TaxTypeCode as NetTaxCode,
		ntax.Amount as NetTaxPercent,
		gtax.TaxTypeCode as GrossTaxCode,
		gtax.Amount as GrossTaxPercent
	from PurchaseItem item
	left join [Option] as opt on item.OptionID = opt.OptionID
	left join Rate as rate on opt.RateID = rate.RateID
	left join [Service] as serv on rate.ServiceID = serv.ServiceID
	left join [Supplier] as sup on serv.SupplierID = sup.SupplierID
	left join ServiceType as stype on serv.ServiceTypeID = stype.ServiceTypeID	
	left join TaxType as ntax on isnull(serv.TaxTypeID, isnull(sup.TaxTypeID, stype.NetTaxTypeID)) = ntax.TaxTypeID
	left join TaxType as gtax on stype.GrossTaxTypeID = gtax.TaxTypeID	
)

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
		TotalGross - TotalNet as Yield,
	    (TotalGross - TotalNet)/(case TotalGross when 0 then 1 else TotalGross end)  as Margin,
		TotalNet - (TotalNet*100/(100+NetTaxPercent)) as NetTaxAmount,
		TotalGross - (TotalGross*100/(100+GrossTaxPercent)) as GrossTaxAmount	    	
	from
	(
		select 
			ItineraryID, 
			PurchaseItemID,
			ServiceTypeID,
			Net * CurrencyRate as Net,
			Gross * CurrencyRate as Gross,
			UnitMultiplier,	
			Net * UnitMultiplier * CurrencyRate  as TotalNet,
			Gross * UnitMultiplier * CurrencyRate as TotalGross,
			case when Net = 0 then 0 else (Gross - Net)/Net*100 end as Markup,
			case when Gross = 0 then 0 else (Gross - Net)/Gross*100 end as Commission,
			GrossOrig * UnitMultiplier * CurrencyRate as TotalGrossOrig,
			case when Net = 0 then 0 else (GrossOrig - Net)/Net*100 end as MarkupOrig,
			case when GrossOrig = 0 then 0 else (GrossOrig - Net)/GrossOrig*100 end as CommissionOrig	
		from
		(
			select 		 
				itin.ItineraryID, 
				item.PurchaseItemID,
				serv.ServiceTypeID,
				item.Net,
				case when itin.NetComOrMup = 'com' 
					then 
					(
						isnull(item.Net*100/(100-itin.NetMargin),
						isnull(item.Net*100/(100-stype.Margin), 
						item.Gross))
					)
					else
					(
						isnull(item.Net*(1+itin.NetMargin/100), 
						isnull(item.Net*(1+stype.Margin/100), 
						item.Gross))
					) 
					end as Gross,
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
PRINT N'Refreshing [dbo].[ItineraryDetail]...';

GO
EXECUTE sp_refreshview N'dbo.ItineraryDetail';

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
PRINT N'Refreshing [dbo].[ItineraryClientDetail]...';

GO
EXECUTE sp_refreshview N'dbo.ItineraryClientDetail';

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
IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2009.11.24'
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
GO
PRINT N'Checking existing data aginst newly created constraints';
GO
ALTER TABLE [dbo].[PurchaseItem] WITH CHECK CHECK CONSTRAINT [FK_PurchaseItem_PurchaseLine];
ALTER TABLE [dbo].[PurchaseItem] WITH CHECK CHECK CONSTRAINT [FK_PurchaseItem_Option];
ALTER TABLE [dbo].[ItineraryPaxOverride] WITH CHECK CHECK CONSTRAINT [FK_ItineraryPaxOverride_PurchaseItem];
ALTER TABLE [dbo].[ItineraryPaxOverride] WITH CHECK CHECK CONSTRAINT [FK_ItineraryPaxOverride_ItineraryPax];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO
SET IDENTITY_INSERT [dbo].[OptionType] ON;
BEGIN TRANSACTION;
INSERT INTO [dbo].[OptionType]([OptionTypeID], [OptionTypeName], [Divisor])
SELECT 1, N'Single', 1 UNION ALL
SELECT 2, N'Double', 2 UNION ALL
SELECT 3, N'Triple', 3 UNION ALL
SELECT 4, N'Quad', 4 UNION ALL
SELECT 5, N'Adult', NULL UNION ALL
SELECT 6, N'Child', NULL UNION ALL
SELECT 7, N'Infant', NULL
COMMIT;
RAISERROR (N'[dbo].[OptionType]: Insert Batch: 1.....Done!', 10, 1) WITH NOWAIT;
GO
SET IDENTITY_INSERT [dbo].[OptionType] OFF;
GO