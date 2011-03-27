
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
if ((select VersionNumber from AppSettings) <> '2011.02.28')
	RAISERROR (N'Database Update Script is not correct version for current database version',17,1)	

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------
GO
PRINT N'Renaming [Supplier].[TaxTypeID]...';

if not Exists(select * from sys.columns where Name = N'NetTaxTypeID' and Object_ID = Object_ID(N'[Supplier]'))
	exec sp_RENAME '[Supplier].[TaxTypeID]' , 'NetTaxTypeID', 'COLUMN';
GO
    
PRINT N'Renaming [Service].[TaxTypeID]...';
if not Exists(select * from sys.columns where Name = N'NetTaxTypeID' and Object_ID = Object_ID(N'[Service]'))
	exec sp_RENAME '[Service].[TaxTypeID]' , 'NetTaxTypeID', 'COLUMN'
GO



GO
PRINT N'Dropping DF_Service_ServiceName...';


GO
ALTER TABLE [dbo].[Service] DROP CONSTRAINT [DF_Service_ServiceName];


GO
PRINT N'Dropping DF_Service_SupplierID...';


GO
ALTER TABLE [dbo].[Service] DROP CONSTRAINT [DF_Service_SupplierID];


GO
PRINT N'Dropping DF_Service_ServiceTypeID...';


GO
ALTER TABLE [dbo].[Service] DROP CONSTRAINT [DF_Service_ServiceTypeID];


GO
PRINT N'Dropping DF_Service_Checkin...';


GO
ALTER TABLE [dbo].[Service] DROP CONSTRAINT [DF_Service_Checkin];


GO
PRINT N'Dropping DF_Service_Checkout...';


GO
ALTER TABLE [dbo].[Service] DROP CONSTRAINT [DF_Service_Checkout];


GO
PRINT N'Dropping DF_Service_CheckinMinutesEarly...';


GO
ALTER TABLE [dbo].[Service] DROP CONSTRAINT [DF_Service_CheckinMinutesEarly];


GO
PRINT N'Dropping DF_Service_IsRecordActive...';


GO
ALTER TABLE [dbo].[Service] DROP CONSTRAINT [DF_Service_IsRecordActive];


GO
PRINT N'Dropping DF_Service_DateAdded...';


GO
ALTER TABLE [dbo].[Service] DROP CONSTRAINT [DF_Service_DateAdded];


GO
PRINT N'Dropping DF_Service_AddedBy...';


GO
ALTER TABLE [dbo].[Service] DROP CONSTRAINT [DF_Service_AddedBy];


GO
PRINT N'Dropping DF_Service_Description...';


GO
ALTER TABLE [dbo].[Service] DROP CONSTRAINT [DF_Service_Description];


GO
PRINT N'Dropping DF_Supplier_ParentFolderID...';


GO
ALTER TABLE [dbo].[Supplier] DROP CONSTRAINT [DF_Supplier_ParentFolderID];


GO
PRINT N'Dropping DF_Supplier_IsRecordActive...';


GO
ALTER TABLE [dbo].[Supplier] DROP CONSTRAINT [DF_Supplier_IsRecordActive];


GO
PRINT N'Dropping FK_Service_Supplier...';


GO
ALTER TABLE [dbo].[Service] DROP CONSTRAINT [FK_Service_Supplier];


GO
PRINT N'Dropping FK_Rate_Service...';


GO
ALTER TABLE [dbo].[Rate] DROP CONSTRAINT [FK_Rate_Service];


GO
PRINT N'Dropping FK_ServiceConfig_Service...';


GO
ALTER TABLE [dbo].[ServiceConfig] DROP CONSTRAINT [FK_ServiceConfig_Service];


GO
PRINT N'Dropping FK_ServiceTime_Service...';


GO
ALTER TABLE [dbo].[ServiceTime] DROP CONSTRAINT [FK_ServiceTime_Service];


GO
PRINT N'Dropping FK_SupplierConfig_Supplier...';


GO
ALTER TABLE [dbo].[SupplierConfig] DROP CONSTRAINT [FK_SupplierConfig_Supplier];


GO
PRINT N'Dropping FK_SupplierContact_Supplier...';


GO
ALTER TABLE [dbo].[SupplierContact] DROP CONSTRAINT [FK_SupplierContact_Supplier];


GO
PRINT N'Dropping FK_SupplierCreditcard_Supplier...';


GO
ALTER TABLE [dbo].[SupplierCreditCard] DROP CONSTRAINT [FK_SupplierCreditcard_Supplier];


GO
PRINT N'Dropping FK_Supplier_Grade...';


GO
ALTER TABLE [dbo].[Supplier] DROP CONSTRAINT [FK_Supplier_Grade];


GO
PRINT N'Dropping FK_Supplier_GradeExternal...';


GO
ALTER TABLE [dbo].[Supplier] DROP CONSTRAINT [FK_Supplier_GradeExternal];


GO
PRINT N'Dropping FK_SupplierMessage_Supplier...';


GO
ALTER TABLE [dbo].[SupplierMessage] DROP CONSTRAINT [FK_SupplierMessage_Supplier];


GO
PRINT N'Dropping FK_SupplierText_Supplier...';


GO
ALTER TABLE [dbo].[SupplierText] DROP CONSTRAINT [FK_SupplierText_Supplier];


GO
PRINT N'Dropping FK_PurchaseLine_Supplier...';


GO
ALTER TABLE [dbo].[PurchaseLine] DROP CONSTRAINT [FK_PurchaseLine_Supplier];


GO
PRINT N'Dropping FK_Content_Supplier...';


GO
ALTER TABLE [dbo].[Content] DROP CONSTRAINT [FK_Content_Supplier];


GO
PRINT N'Dropping FK_SupplierNote_Supplier...';


GO
ALTER TABLE [dbo].[SupplierNote] DROP CONSTRAINT [FK_SupplierNote_Supplier];


GO
PRINT N'Starting rebuilding table [dbo].[Service]...';


GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

BEGIN TRANSACTION;

CREATE TABLE [dbo].[tmp_ms_xx_Service] (
    [ServiceID]           INT            IDENTITY (1, 1) NOT NULL,
    [ServiceName]         VARCHAR (150)  CONSTRAINT [DF_Service_ServiceName] DEFAULT ('') NOT NULL,
    [SupplierID]          INT            CONSTRAINT [DF_Service_SupplierID] DEFAULT ((0)) NULL,
    [ServiceTypeID]       INT            CONSTRAINT [DF_Service_ServiceTypeID] DEFAULT ((0)) NULL,
    [Description]         VARCHAR (8000) CONSTRAINT [DF_Service_Description] DEFAULT ('') NULL,
    [Comments]            VARCHAR (8000) NULL,
    [Warning]             VARCHAR (8000) NULL,
    [MaxPax]              INT            NULL,
    [CheckinTime]         DATETIME       CONSTRAINT [DF_Service_Checkin] DEFAULT ('12:00') NULL,
    [CheckoutTime]        DATETIME       CONSTRAINT [DF_Service_Checkout] DEFAULT ('12:00') NULL,
    [CheckinMinutesEarly] INT            CONSTRAINT [DF_Service_CheckinMinutesEarly] DEFAULT ((0)) NULL,
    [IsRecordActive]      BIT            CONSTRAINT [DF_Service_IsRecordActive] DEFAULT ((1)) NULL,
    [CurrencyCode]        CHAR (3)       NULL,
    [ChargeType]          VARCHAR (10)   NULL,
    [PaymentTermID]       INT            NULL,
    [NetTaxTypeID]        INT            NULL,
    [GrossTaxTypeID]      INT            NULL,
    [Latitude]            FLOAT          NULL,
    [Longitude]           FLOAT          NULL,
    [AddedOn]             DATETIME       CONSTRAINT [DF_Service_DateAdded] DEFAULT (getdate()) NULL,
    [AddedBy]             INT            CONSTRAINT [DF_Service_AddedBy] DEFAULT ((0)) NULL,
    [RowVersion]          TIMESTAMP      NULL,
    [IsDeleted]           BIT            NULL
);

ALTER TABLE [dbo].[tmp_ms_xx_Service]
    ADD CONSTRAINT [tmp_ms_xx_clusteredindex_PK_Service] PRIMARY KEY CLUSTERED ([ServiceID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

IF EXISTS (SELECT TOP 1 1
           FROM   [dbo].[Service])
    BEGIN
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Service] ON;
        INSERT INTO [dbo].[tmp_ms_xx_Service] ([ServiceID], [ServiceName], [SupplierID], [ServiceTypeID], [Description], [Comments], [Warning], [MaxPax], [CheckinTime], [CheckoutTime], [CheckinMinutesEarly], [IsRecordActive], [CurrencyCode], [ChargeType], [PaymentTermID], [NetTaxTypeID], [Latitude], [Longitude], [AddedOn], [AddedBy], [IsDeleted])
        SELECT   [ServiceID],
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
                 [Latitude],
                 [Longitude],
                 [AddedOn],
                 [AddedBy],
                 [IsDeleted]
        FROM     [dbo].[Service]
        ORDER BY [ServiceID] ASC;
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Service] OFF;
    END

DROP TABLE [dbo].[Service];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_Service]', N'Service';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_clusteredindex_PK_Service]', N'PK_Service', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Starting rebuilding table [dbo].[Supplier]...';


GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

BEGIN TRANSACTION;

CREATE TABLE [dbo].[tmp_ms_xx_Supplier] (
    [SupplierID]          INT            IDENTITY (1, 1) NOT NULL,
    [SupplierName]        VARCHAR (150)  NOT NULL,
    [HostName]            VARCHAR (150)  NULL,
    [StreetAddress]       VARCHAR (100)  NULL,
    [PostAddress]         VARCHAR (500)  NULL,
    [Postcode]            VARCHAR (20)   NULL,
    [CityID]              INT            NULL,
    [RegionID]            INT            NULL,
    [StateID]             INT            NULL,
    [CountryID]           INT            NULL,
    [Phone]               VARCHAR (50)   NULL,
    [MobilePhone]         VARCHAR (50)   NULL,
    [FreePhone]           VARCHAR (50)   NULL,
    [Fax]                 VARCHAR (50)   NULL,
    [Email]               VARCHAR (200)  NULL,
    [Website]             VARCHAR (200)  NULL,
    [Latitude]            FLOAT          NULL,
    [Longitude]           FLOAT          NULL,
    [GradeID]             INT            NULL,
    [GradeExternalID]     INT            NULL,
    [Description]         VARCHAR (8000) NULL,
    [Comments]            VARCHAR (8000) NULL,
    [CancellationPolicy]  VARCHAR (2000) NULL,
    [BankDetails]         VARCHAR (255)  NULL,
    [AccountingName]      VARCHAR (150)  NULL,
    [NetTaxTypeID]        INT            NULL,
    [GrossTaxTypeID]      INT            NULL,
    [PaymentTermID]       INT            NULL,
    [DefaultMargin]       REAL           NULL,
    [DefaultCheckinTime]  DATETIME       NULL,
    [DefaultCheckoutTime] DATETIME       NULL,
    [ImportID]            VARCHAR (50)   NULL,
    [ExportID]            VARCHAR (50)   NULL,
    [BookingWebsite]      VARCHAR (255)  NULL,
    [IsRecordActive]      BIT            CONSTRAINT [DF_Supplier_IsRecordActive] DEFAULT ((1)) NULL,
    [ParentFolderID]      INT            CONSTRAINT [DF_Supplier_ParentFolderID] DEFAULT ((0)) NULL,
    [AddedOn]             DATETIME       NULL,
    [AddedBy]             INT            NULL,
    [RowVersion]          TIMESTAMP      NULL,
    [IsDeleted]           BIT            NULL
);

ALTER TABLE [dbo].[tmp_ms_xx_Supplier]
    ADD CONSTRAINT [tmp_ms_xx_clusteredindex_PK_Supplier] PRIMARY KEY CLUSTERED ([SupplierID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

IF EXISTS (SELECT TOP 1 1
           FROM   [dbo].[Supplier])
    BEGIN
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Supplier] ON;
        INSERT INTO [dbo].[tmp_ms_xx_Supplier] ([SupplierID], [SupplierName], [HostName], [StreetAddress], [PostAddress], [Postcode], [CityID], [RegionID], [StateID], [CountryID], [Phone], [MobilePhone], [FreePhone], [Fax], [Email], [Website], [Latitude], [Longitude], [GradeID], [GradeExternalID], [Description], [Comments], [CancellationPolicy], [BankDetails], [AccountingName], [NetTaxTypeID], [PaymentTermID], [DefaultMargin], [DefaultCheckinTime], [DefaultCheckoutTime], [ImportID], [ExportID], [BookingWebsite], [IsRecordActive], [ParentFolderID], [AddedOn], [AddedBy], [IsDeleted])
        SELECT   [SupplierID],
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
        FROM     [dbo].[Supplier]
        ORDER BY [SupplierID] ASC;
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Supplier] OFF;
    END

DROP TABLE [dbo].[Supplier];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_Supplier]', N'Supplier';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_clusteredindex_PK_Supplier]', N'PK_Supplier', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating FK_Service_Supplier...';


GO
ALTER TABLE [dbo].[Service] WITH NOCHECK
    ADD CONSTRAINT [FK_Service_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[Supplier] ([SupplierID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Creating FK_Rate_Service...';


GO
ALTER TABLE [dbo].[Rate] WITH NOCHECK
    ADD CONSTRAINT [FK_Rate_Service] FOREIGN KEY ([ServiceID]) REFERENCES [dbo].[Service] ([ServiceID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Creating FK_ServiceConfig_Service...';


GO
ALTER TABLE [dbo].[ServiceConfig] WITH NOCHECK
    ADD CONSTRAINT [FK_ServiceConfig_Service] FOREIGN KEY ([ServiceID]) REFERENCES [dbo].[Service] ([ServiceID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Creating FK_ServiceTime_Service...';


GO
ALTER TABLE [dbo].[ServiceTime] WITH NOCHECK
    ADD CONSTRAINT [FK_ServiceTime_Service] FOREIGN KEY ([ServiceID]) REFERENCES [dbo].[Service] ([ServiceID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Creating FK_SupplierConfig_Supplier...';


GO
ALTER TABLE [dbo].[SupplierConfig] WITH NOCHECK
    ADD CONSTRAINT [FK_SupplierConfig_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[Supplier] ([SupplierID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Creating FK_SupplierContact_Supplier...';


GO
ALTER TABLE [dbo].[SupplierContact] WITH NOCHECK
    ADD CONSTRAINT [FK_SupplierContact_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[Supplier] ([SupplierID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Creating FK_SupplierCreditcard_Supplier...';


GO
ALTER TABLE [dbo].[SupplierCreditCard] WITH NOCHECK
    ADD CONSTRAINT [FK_SupplierCreditcard_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[Supplier] ([SupplierID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Creating FK_Supplier_Grade...';


GO
ALTER TABLE [dbo].[Supplier] WITH NOCHECK
    ADD CONSTRAINT [FK_Supplier_Grade] FOREIGN KEY ([GradeID]) REFERENCES [dbo].[Grade] ([GradeID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Creating FK_Supplier_GradeExternal...';


GO
ALTER TABLE [dbo].[Supplier] WITH NOCHECK
    ADD CONSTRAINT [FK_Supplier_GradeExternal] FOREIGN KEY ([GradeExternalID]) REFERENCES [dbo].[GradeExternal] ([GradeExternalID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Creating FK_SupplierMessage_Supplier...';


GO
ALTER TABLE [dbo].[SupplierMessage] WITH NOCHECK
    ADD CONSTRAINT [FK_SupplierMessage_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[Supplier] ([SupplierID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Creating FK_SupplierText_Supplier...';


GO
ALTER TABLE [dbo].[SupplierText] WITH NOCHECK
    ADD CONSTRAINT [FK_SupplierText_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[Supplier] ([SupplierID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Creating FK_PurchaseLine_Supplier...';


GO
ALTER TABLE [dbo].[PurchaseLine] WITH NOCHECK
    ADD CONSTRAINT [FK_PurchaseLine_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[Supplier] ([SupplierID]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_Content_Supplier...';


GO
ALTER TABLE [dbo].[Content] WITH NOCHECK
    ADD CONSTRAINT [FK_Content_Supplier] FOREIGN KEY ([SupplierId]) REFERENCES [dbo].[Supplier] ([SupplierID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Creating FK_SupplierNote_Supplier...';


GO
ALTER TABLE [dbo].[SupplierNote] WITH NOCHECK
    ADD CONSTRAINT [FK_SupplierNote_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[Supplier] ([SupplierID]) ON DELETE CASCADE ON UPDATE CASCADE;


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
	i.[CurrencyRate],
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
PRINT N'Altering [dbo].[_SupplierSet_Copy_ByID]...';


GO
ALTER PROCEDURE [dbo].[_SupplierSet_Copy_ByID]
@OrigSupplierID INT, @SupplierName VARCHAR (100), @AddedOn DATETIME, @AddedBy INT
AS
SET NOCOUNT ON

DECLARE @NewSupplierID int

-- Supplier ----------------------------------------------
INSERT INTO [dbo].[Supplier]
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
	[Description],
	[Comments],
	[CancellationPolicy],
	[BankDetails],
	[NetTaxTypeID],
	[GrossTaxTypeID],
	[PaymentTermID],
	[DefaultMargin],
	[DefaultCheckinTime],
	[DefaultCheckoutTime],
	[ImportID],
	[ExportID],
	[IsRecordActive],
	[ParentFolderID],
	[AddedOn],
	[AddedBy]
)
SELECT
	@SupplierName,
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
	[Description],
	[Comments],
	[CancellationPolicy],
	[BankDetails],
	[NetTaxTypeID],
	[GrossTaxTypeID],
	[PaymentTermID],
	[DefaultMargin],
	[DefaultCheckinTime],
	[DefaultCheckoutTime],
	[ImportID],
	[ExportID],
	[IsRecordActive],
	[ParentFolderID],
	@AddedOn,
	@AddedBy
FROM Supplier 
WHERE SupplierID = @OrigSupplierID

SELECT @NewSupplierID=SCOPE_IDENTITY()

-- SupplierCreditCard ---------------------------------------	
INSERT INTO [dbo].[SupplierCreditCard]
(
	[SupplierID],
	[CreditCardID],
	[AddedOn],
	[AddedBy]
)	
SELECT
	@NewSupplierID,
	[CreditCardID],
	@AddedOn,
	@AddedBy
FROM [dbo].[SupplierCreditCard]
WHERE [SupplierID] = @OrigSupplierID

-- SupplierConfig ------------------------------------------	
INSERT INTO [dbo].[SupplierConfig]
(
	[SupplierID],
	[SupplierConfigTypeID],
	[Note],
	[AddedOn],
	[AddedBy]
)	
SELECT
	@NewSupplierID,
	[SupplierConfigTypeID],
	[Note],
	@AddedOn,
	@AddedBy
FROM [dbo].[SupplierConfig]
WHERE [SupplierID] = @OrigSupplierID

-- SupplierText ------------------------------------------	

INSERT [dbo].[SupplierText]
(
	[SupplierTextName],
	[SupplierID],
	[FileName],
	[AddedOn],
	[AddedBy]
)	
SELECT
	[SupplierTextName],
	@NewSupplierID,
	[FileName],
	[AddedOn],
	[AddedBy]
FROM [dbo].[SupplierText]
WHERE [SupplierID] = @OrigSupplierID

--=== Loop services ======================================
DECLARE @NewServiceID int
DECLARE @OrigServiceID int
DECLARE ServiceCursor CURSOR FOR 
	SELECT ServiceID FROM Service WHERE SupplierID = @OrigSupplierID

OPEN ServiceCursor
FETCH NEXT FROM ServiceCursor INTO @OrigServiceID

WHILE @@FETCH_STATUS = 0
BEGIN -- ServiceCursor

	-- Service ------------------------------------------------
	INSERT INTO [dbo].[Service]
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
		[IsRecordActive],
		[CurrencyCode],
		[ChargeType],
		[AddedOn],
		[AddedBy]
	)
	SELECT
		[ServiceName],
		@NewSupplierID,
		[ServiceTypeID],
		[Description],
		[Comments],		
		[Warning],
		[MaxPax],
		[CheckinTime],
		[CheckoutTime],
		[IsRecordActive],
		[CurrencyCode],
		[ChargeType],
		@AddedOn,
		@AddedBy
	FROM [dbo].[Service] 
	WHERE [ServiceID] = @OrigServiceID

	SELECT @NewServiceID=SCOPE_IDENTITY()
	
	-- ServiceConfig ----------------------------------------------------
	INSERT INTO [dbo].[ServiceConfig]
	(
		[ServiceID],
		[ServiceConfigTypeID],
		[Note],
		[AddedOn],
		[AddedBy]
	)
	SELECT
		@NewServiceID,
		[ServiceConfigTypeID],
		[Note],
		@AddedOn,
		@AddedBy
	FROM [dbo].[ServiceConfig]
	WHERE [ServiceID] = @OrigServiceID

	--==Loop rates ======================================
	DECLARE @NewRateID int
	DECLARE @OrigRateID int
	DECLARE RateCursor CURSOR FOR 
		SELECT RateID FROM Rate WHERE ServiceID = @OrigServiceID
	
	OPEN RateCursor
	FETCH NEXT FROM RateCursor INTO @OrigRateID
	
	WHILE @@FETCH_STATUS = 0
	BEGIN -- RateCursor

	-- Rate ---------------------------------------------------------------

		INSERT INTO [dbo].[Rate]
		(
			[ServiceID],
			[ValidFrom],
			[ValidTo],
			[AddedOn],
			[AddedBy]
		)
		SELECT
			@NewServiceID,
			[ValidFrom],
			[ValidTo],
			@AddedOn,
			@AddedBy
		FROM [dbo].[Rate]	
		WHERE [RateID] = @OrigRateID

		SELECT @NewRateID=SCOPE_IDENTITY()

		-- Option ---------------------------------------------------------------

		INSERT INTO [dbo].[Option]
		(
			[OptionName],
			[OptionTypeID],
			[RateID],
			[Net],
			[Gross],
			[PricingOption],
			[IsDefault],
			[IsRecordActive],
			[AddedOn],
			[AddedBy]
		)
		SELECT
			[OptionName],
			[OptionTypeID],
			@NewRateID,
			[Net],
			[Gross],
			[PricingOption],
			[IsDefault],
			[IsRecordActive],
			@AddedOn,
			@AddedBy
		FROM [dbo].[Option]	
		WHERE [RateID] = @OrigRateID
		
		FETCH NEXT FROM RateCursor INTO @OrigRateID

	END -- RateCursor	
	CLOSE RateCursor
	DEALLOCATE RateCursor

	FETCH NEXT FROM ServiceCursor INTO @OrigServiceID

END -- ServiceCursor
CLOSE ServiceCursor
DEALLOCATE ServiceCursor

-- return new supplier id
SELECT @NewSupplierID
GO
PRINT N'Altering [dbo].[_SupplierSet_Sel_ByID]...';


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
	[IsDeleted]
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
	[IsDeleted]
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
	[IsDeleted]
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
	[IsDeleted]
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
	[IsDeleted]
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
	[RowVersion]
FROM [dbo].[SupplierNote]
WHERE
	[SupplierID] = @SupplierID
ORDER BY 
	SupplierNoteID

-- ServiceFoc --
SELECT
	[ServiceFocID],
	[ServiceID],
	[UnitsUsed],
	[UnitsFree]
FROM [dbo].[ServiceFoc]
WHERE
	[ServiceID] IN ( 
		SELECT [ServiceID] FROM [dbo].[Service] 
		WHERE [SupplierID] = @SupplierID )

-- Content
SELECT * FROM [Content] 
WHERE SupplierId = @SupplierID
GO
PRINT N'Altering [dbo].[Option_Sel_All]...';


GO

ALTER PROCEDURE [dbo].[Option_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[OptionID],
	[OptionName],
	[OptionTypeID],
	[RateID],
	[Net],
	[Gross],
	[PricingOption],
	[IsDefault],
	[IsRecordActive],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted]
FROM [dbo].[Option]
ORDER BY 
	[OptionID] ASC
GO
PRINT N'Altering [dbo].[Option_Sel_ByID]...';


GO

ALTER PROCEDURE [dbo].[Option_Sel_ByID]
	@OptionID int
AS
SET NOCOUNT ON
SELECT
	[OptionID],
	[OptionName],
	[OptionTypeID],
	[RateID],
	[Net],
	[Gross],
	[PricingOption],
	[IsDefault],
	[IsRecordActive],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted]
FROM [dbo].[Option]
WHERE
	[OptionID] = @OptionID
GO
PRINT N'Altering [dbo].[Option_Sel_ByRateID]...';


GO

ALTER PROCEDURE [dbo].[Option_Sel_ByRateID]
	@RateID int
AS
SET NOCOUNT ON
SELECT
	[OptionID],
	[OptionName],
	[OptionTypeID],
	[RateID],
	[Net],
	[Gross],
	[PricingOption],
	[IsDefault],
	[IsRecordActive],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted]
FROM [dbo].[Option]
WHERE
	[RateID] = @RateID
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
	[RowVersion]
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
	[RowVersion]
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
	[RowVersion]
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
	[RowVersion]
FROM [dbo].[PurchaseItem]
WHERE
	[PurchaseLineID] = @PurchaseLineID
GO
PRINT N'Altering [dbo].[Service_Ins]...';


GO

ALTER PROCEDURE [dbo].[Service_Ins]
	@ServiceName varchar(150),
	@SupplierID int,
	@ServiceTypeID int,
	@Description varchar(8000),
	@Comments varchar(8000),
	@Warning varchar(8000),
	@MaxPax int,
	@CheckinTime datetime,
	@CheckoutTime datetime,
	@CheckinMinutesEarly int,
	@IsRecordActive bit,
	@CurrencyCode char(3),
	@ChargeType varchar(10),
	@PaymentTermID int,
	@NetTaxTypeID int,
	@GrossTaxTypeID int,
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
	[NetTaxTypeID],
	[GrossTaxTypeID],
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
	@NetTaxTypeID,
	@GrossTaxTypeID,
	@Latitude,
	@Longitude,
	@AddedOn,
	@AddedBy,
	@IsDeleted
)
SELECT @ServiceID=SCOPE_IDENTITY()
GO
PRINT N'Altering [dbo].[Service_Sel_All]...';


GO

ALTER PROCEDURE [dbo].[Service_Sel_All]
AS
SET NOCOUNT ON
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
	[IsDeleted]
FROM [dbo].[Service]
ORDER BY 
	[ServiceID] ASC
GO
PRINT N'Altering [dbo].[Service_Sel_ByID]...';


GO

ALTER PROCEDURE [dbo].[Service_Sel_ByID]
	@ServiceID int
AS
SET NOCOUNT ON
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
	[IsDeleted]
FROM [dbo].[Service]
WHERE
	[ServiceID] = @ServiceID
GO
PRINT N'Altering [dbo].[Service_Sel_BySupplierID]...';


GO

ALTER PROCEDURE [dbo].[Service_Sel_BySupplierID]
	@SupplierID int
AS
SET NOCOUNT ON
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
	[IsDeleted]
FROM [dbo].[Service]
WHERE
	[SupplierID] = @SupplierID
GO
PRINT N'Altering [dbo].[Service_Upd]...';


GO

ALTER PROCEDURE [dbo].[Service_Upd]
	@ServiceID int,
	@ServiceName varchar(150),
	@SupplierID int,
	@ServiceTypeID int,
	@Description varchar(8000),
	@Comments varchar(8000),
	@Warning varchar(8000),
	@MaxPax int,
	@CheckinTime datetime,
	@CheckoutTime datetime,
	@CheckinMinutesEarly int,
	@IsRecordActive bit,
	@CurrencyCode char(3),
	@ChargeType varchar(10),
	@PaymentTermID int,
	@NetTaxTypeID int,
	@GrossTaxTypeID int,
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
	[NetTaxTypeID] = @NetTaxTypeID,
	[GrossTaxTypeID] = @GrossTaxTypeID,
	[Latitude] = @Latitude,
	[Longitude] = @Longitude,
	[AddedOn] = @AddedOn,
	[AddedBy] = @AddedBy,
	[IsDeleted] = @IsDeleted
WHERE
	[ServiceID] = @ServiceID
	AND [RowVersion] = @RowVersion
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
	@IsDeleted
)
SELECT @SupplierID=SCOPE_IDENTITY()
GO
PRINT N'Altering [dbo].[Supplier_Sel_All]...';


GO

ALTER PROCEDURE [dbo].[Supplier_Sel_All]
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
	[IsDeleted]
FROM [dbo].[Supplier]
ORDER BY 
	[SupplierID] ASC
GO
PRINT N'Altering [dbo].[Supplier_Sel_ByGradeExternalID]...';


GO

ALTER PROCEDURE [dbo].[Supplier_Sel_ByGradeExternalID]
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
	[IsDeleted]
FROM [dbo].[Supplier]
WHERE
	[GradeExternalID] = @GradeExternalID
GO
PRINT N'Altering [dbo].[Supplier_Sel_ByGradeID]...';


GO

ALTER PROCEDURE [dbo].[Supplier_Sel_ByGradeID]
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
	[IsDeleted]
FROM [dbo].[Supplier]
WHERE
	[GradeID] = @GradeID
GO
PRINT N'Altering [dbo].[Supplier_Sel_ByID]...';


GO

ALTER PROCEDURE [dbo].[Supplier_Sel_ByID]
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
	[IsDeleted]
FROM [dbo].[Supplier]
WHERE
	[SupplierID] = @SupplierID
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
	[IsDeleted] = @IsDeleted
WHERE
	[SupplierID] = @SupplierID
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
PRINT N'Altering [dbo].[ItineraryServiceTypePricing]...';


GO
ALTER FUNCTION [dbo].[ItineraryServiceTypePricing]
( )
RETURNS 
    @result TABLE (
        [ItineraryID]    INT   NULL,
        [ServiceTypeID]  INT   NULL,
        [ItemCount]      INT   NULL,
        [Net]            MONEY NULL,
        [Gross]          MONEY NULL,
        [Markup]         MONEY NULL,
        [Commission]     MONEY NULL,
        [Yield]          MONEY NULL)
AS
BEGIN

	-- all itinerary by all service types
	insert into @result
		select itin.ItineraryID, stype.ServiceTypeID, base.ItemCount, base.Net, base.Gross, null, null, null--, null, null
		from Itinerary itin
		join (select ServiceTypeID, ServiceTypeName from ServiceType) stype on 1=1
		left outer join
		(
			select item.ItineraryID, item.ServicetypeID, count(*) as ItemCount , sum(item.GrossFinalTotal) as Net, sum(item.GrossFinalTotal) as Gross
			from PurchaseItemPricing() item
			group by item.ItineraryID, item.ServiceTypeID
		) base on itin.ItineraryID = base.ItineraryID and stype.ServiceTypeID = base.ServiceTypeID

	-- add final override
	declare @deltaStype int
	set @deltaStype = (select top(1) ServiceTypeID from ServiceType order by IsAdditionalMarkupContainer desc)
	update @result 
	set Gross = 
		case
			when (itin.GrossOverride is not null) then isnull(base.Gross,0) + itin.GrossOverride - isnull(baseItinGross.Gross,0)
			when (itin.GrossMarkup   is not null) then isnull(base.Gross,0) + (isnull(baseItinGross.Gross,0) * (1+itin.GrossMarkup/100)) - isnull(baseItinGross.Gross,0)
			else base.Gross
		end
	from @result base 
	inner join Itinerary itin on  base.ItineraryID = itin.ItineraryID and base.ServiceTypeID = @deltaStype
	inner join
	(	select ItineraryID, sum(GrossFinalTotal) as Gross
		from PurchaseItemPricing()
		group by ItineraryID
	) baseItinGross
	on itin.ItineraryID = baseItinGross.ItineraryID

	update @result 
	set 
		Markup = (case when Net = 0 then 0 else (Gross - Net)/Net*100 end), 
		Commission = (case when Gross = 0 then 0 else (Gross - Net)/Gross*100 end),
		Yield = Gross - isnull(Net, 0)

RETURN 
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
		isnull(itin.CurrencyCode, (select top(1) CurrencyCode from Appsettings)) as ItineraryCurrencyCode,
		isnull(itinCcy.DisplayFormat, sysCcy.DisplayFormat) as ItineraryCurrencyFormat,
		itin.NetComOrMup as NetOverrideComOrMup,
		pric.NetFinalTotal as ItineraryNetFinalTotal,
		pric.GrossFinalTotal as ItineraryGrossFinalTotal,
		pric.Markup as ItineraryMarkup,
		pric.Commission as ItineraryCommission,
		pric.Yield as ItineraryYield,
		pric.GrossFinalTotalTaxAmount as ItineraryGrossFinalTotalTaxAmount,
		payments.TotalPayments as ItineraryTotalPayments,
		pric.GrossFinalTotal - payments.TotalPayments as ItineraryTotalOutstanding,
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
		ParentFolderID as MenuFolderID
		
		--/**** only for backwards compatibility ****/
		,pric.NetFinalTotal as ItineraryNet
		,pric.GrossFinalTotal as ItineraryGross
		--/******************************************/		
		
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
	) payments on itin.ItineraryID = payments.ItineraryID
	left join Currency itinCcy on itinCcy.CurrencyCode = itin.CurrencyCode COLLATE DATABASE_DEFAULT
	left join Currency sysCcy on sysCcy.CurrencyCode = (select top(1) CurrencyCode from AppSettings) COLLATE DATABASE_DEFAULT
	where 
		(itin.IsDeleted is null or itin.IsDeleted = 'false');
GO
PRINT N'Refreshing [dbo].[ItinerarySaleAllocationDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItinerarySaleAllocationDetail';


GO
PRINT N'Altering [dbo].[ItineraryServiceTypeDetail]...';


GO



ALTER VIEW [dbo].[ItineraryServiceTypeDetail]
AS
select
	itin.*,
	stype.*, 
	price.ItemCount as PurchaseItemCount, 
	price.Net, 
	price.Gross,
	price.Markup,
	price.Commission,
	price.Yield,
	sales.Amount as Sales,
	isnull(price.Gross,0) - isnull(sales.Amount,0) as GrossMinusSales,
	price.Gross - (price.Gross*100/(100+stype.GrossTaxPercent)) as GrossTaxAmount
	
	--/**** only for backwards compatibility ****/
	,0 as GrossOrig
	,0 as MarkupOrig
	,0 as CommissionOrig
	,0 as ItineraryGrossOrig
	--/******************************************/
	
from ItineraryServiceTypePricing() price
inner join ItineraryDetail itin on price.ItineraryID = itin.ItineraryID
inner join ServiceTypeDetail stype on price.ServiceTypeID = stype.ServiceTypeID
left outer join 
(
	select ItineraryID, ServiceTypeID, sum(Amount) as Amount
	from ItinerarySale sale
	inner join ItinerarySaleAllocation alloc on sale.ItinerarySaleID = alloc.ItinerarySaleID
	group by ItineraryID, ServiceTypeID
) sales on itin.ItineraryID = sales.ItineraryID and stype.ServiceTypeID = sales.ServiceTypeID;
GO
PRINT N'Refreshing [dbo].[ItinerarySaleDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItinerarySaleDetail';


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
	itin.*
from ItineraryMember mem
left outer join ItineraryGroup grp on mem.ItineraryGroupID = grp.ItineraryGroupID
left outer join ItineraryDetail itin on grp.ItineraryID = itin.ItineraryID
left outer join AgeGroup age on mem.AgeGroupID = age.AgeGroupID
left outer join ContactDetail cont on mem.ContactID = cont.ContactID
GO
PRINT N'Refreshing [dbo].[ItineraryPaymentDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryPaymentDetail';


GO
PRINT N'Refreshing [dbo].[SupplierDetail]...';


GO
EXECUTE sp_refreshview N'dbo.SupplierDetail';


GO
PRINT N'Refreshing [dbo].[SupplierRatesDetail]...';


GO
EXECUTE sp_refreshview N'dbo.SupplierRatesDetail';


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
		NetBaseTotal - NetBaseTotalTaxAmount as NetBaseTotalExcl, -- TotalNetExcl,
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
		
		--/**** only for backwards compatibility ****/
		,pric.NetBaseUnit as Net
		,pric.GrossBaseUnit as Gross
		,pric.NetFinalTotal as TotalNet
		,pric.GrossFinalTotal as TotalGross
		,pric.GrossBaseUnit * pric.UnitMultiplier * pric.CurrencyRate as TotalGrossOrig
		--/******************************************/
		
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
PRINT N'Altering [dbo].[PurchaseItemPaymentsDetail]...';


GO

ALTER VIEW [dbo].[PurchaseItemPaymentsDetail]
AS

select 
	item.*,
	t.PaymentDueName,  
	t.PaymentDueDate, 
	t.PaymentAmount, 
	NetBaseTotalTaxAmount * t.PaymentAmount / (case NetBaseTotal when 0 then 1 else NetBaseTotal end) as PaymentTaxAmount
from dbo.PurchaseItemDetail item 

--inner join dbo.PurchaseItemPayments() t on t.PurchaseItemID = item.PurchaseItemID

inner join 
(
	select 
		PurchaseItemID, 
		DepositDueDate as PaymentDueDate,
		'Deposit' as PaymentDueName,
		case DepositType
			when 'c' then DepositAmount 
			when 'p' then NetBaseTotal * DepositAmount / 100
		end as PaymentAmount
	from dbo.PurchaseItemDetail where DepositTerms is not null
	union all
	select 
		PurchaseItemID, 
		BalanceDueDate as PaymentDueDate,
		'Balance' as PaymentDueName, 
		case DepositType
			when 'c' then NetBaseTotal - DepositAmount 
			when 'p' then NetBaseTotal - (NetBaseTotal * DepositAmount / 100)
		end as PaymentAmount
	from dbo.PurchaseItemDetail where DepositTerms is not null
	union all
	select 
		PurchaseItemID, 
		isnull(BalanceDueDate, PurchaseItemStartDate) as PaymentDueDate,
		'Full' as PaymentDueName,
		NetBaseTotal as PaymentAmount
	from dbo.PurchaseItemDetail where DepositTerms is null
) t on t.PurchaseItemID = item.PurchaseItemID
GO
PRINT N'Checking existing data against newly created constraints';
GO
ALTER TABLE [dbo].[Service] WITH CHECK CHECK CONSTRAINT [FK_Service_Supplier];

ALTER TABLE [dbo].[Rate] WITH CHECK CHECK CONSTRAINT [FK_Rate_Service];

ALTER TABLE [dbo].[ServiceConfig] WITH CHECK CHECK CONSTRAINT [FK_ServiceConfig_Service];

ALTER TABLE [dbo].[ServiceTime] WITH CHECK CHECK CONSTRAINT [FK_ServiceTime_Service];

ALTER TABLE [dbo].[SupplierConfig] WITH CHECK CHECK CONSTRAINT [FK_SupplierConfig_Supplier];

ALTER TABLE [dbo].[SupplierContact] WITH CHECK CHECK CONSTRAINT [FK_SupplierContact_Supplier];

ALTER TABLE [dbo].[SupplierCreditCard] WITH CHECK CHECK CONSTRAINT [FK_SupplierCreditcard_Supplier];

ALTER TABLE [dbo].[Supplier] WITH CHECK CHECK CONSTRAINT [FK_Supplier_Grade];

ALTER TABLE [dbo].[Supplier] WITH CHECK CHECK CONSTRAINT [FK_Supplier_GradeExternal];

ALTER TABLE [dbo].[SupplierMessage] WITH CHECK CHECK CONSTRAINT [FK_SupplierMessage_Supplier];

ALTER TABLE [dbo].[SupplierText] WITH CHECK CHECK CONSTRAINT [FK_SupplierText_Supplier];

ALTER TABLE [dbo].[PurchaseLine] WITH CHECK CHECK CONSTRAINT [FK_PurchaseLine_Supplier];

ALTER TABLE [dbo].[Content] WITH CHECK CHECK CONSTRAINT [FK_Content_Supplier];

ALTER TABLE [dbo].[SupplierNote] WITH CHECK CHECK CONSTRAINT [FK_SupplierNote_Supplier];


GO
----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2011.03.27'
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
