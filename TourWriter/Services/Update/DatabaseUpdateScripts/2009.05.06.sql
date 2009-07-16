/*
TourWriter database update script, from version 2009.4.20 to 2009.5.6
*/
GO
if ('2009.5.6' <= (select VersionNumber from AppSettings)) return
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

-----------------------------------------------------------------
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
PRINT N'Dropping dbo.DF_Supplier_IsRecordActive...';


GO
ALTER TABLE [dbo].[Supplier] DROP CONSTRAINT [DF_Supplier_IsRecordActive];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.DF_Supplier_ParentFolderID...';


GO
ALTER TABLE [dbo].[Supplier] DROP CONSTRAINT [DF_Supplier_ParentFolderID];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.FK_SupplierCreditcard_Supplier...';


GO
ALTER TABLE [dbo].[SupplierCreditCard] DROP CONSTRAINT [FK_SupplierCreditcard_Supplier];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.FK_SupplierContact_Supplier...';


GO
ALTER TABLE [dbo].[SupplierContact] DROP CONSTRAINT [FK_SupplierContact_Supplier];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.FK_SupplierConfig_Supplier...';


GO
ALTER TABLE [dbo].[SupplierConfig] DROP CONSTRAINT [FK_SupplierConfig_Supplier];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.FK_Supplier_Grade...';


GO
ALTER TABLE [dbo].[Supplier] DROP CONSTRAINT [FK_Supplier_Grade];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.FK_Supplier_GradeExternal...';


GO
ALTER TABLE [dbo].[Supplier] DROP CONSTRAINT [FK_Supplier_GradeExternal];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.FK_Service_Supplier...';


GO
ALTER TABLE [dbo].[Service] DROP CONSTRAINT [FK_Service_Supplier];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.FK_PurchaseLine_Supplier...';


GO
ALTER TABLE [dbo].[PurchaseLine] DROP CONSTRAINT [FK_PurchaseLine_Supplier];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.FK_SupplierText_Supplier...';


GO
ALTER TABLE [dbo].[SupplierText] DROP CONSTRAINT [FK_SupplierText_Supplier];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.FK_SupplierMessage_Supplier...';


GO
ALTER TABLE [dbo].[SupplierMessage] DROP CONSTRAINT [FK_SupplierMessage_Supplier];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.FK_SupplierNote_Supplier...';


GO
ALTER TABLE [dbo].[SupplierNote] DROP CONSTRAINT [FK_SupplierNote_Supplier];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Agent...';


GO
ALTER TABLE [dbo].[Agent] ALTER COLUMN [Comments] VARCHAR (2000) NULL;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Itinerary...';


GO
ALTER TABLE [dbo].[Itinerary] ALTER COLUMN [Comments] VARCHAR (4000) NULL;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.ServiceConfig...';


GO
ALTER TABLE [dbo].[ServiceConfig] ALTER COLUMN [Note] VARCHAR (255) NULL;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Starting rebuilding table dbo.Supplier...';


GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;


GO
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
    [Description]         VARCHAR (4000) NULL,
    [Comments]            VARCHAR (4000) NULL,
    [CancellationPolicy]  VARCHAR (2000) NULL,
    [BankDetails]         VARCHAR (255)  NULL,
    [AccountingName]      VARCHAR (150)  NULL,
    [TaxTypeID]           INT            NULL,
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
        INSERT INTO [dbo].[tmp_ms_xx_Supplier] ([SupplierID], [SupplierName], [HostName], [StreetAddress], [PostAddress], [CityID], [RegionID], [StateID], [CountryID], [Phone], [MobilePhone], [FreePhone], [Fax], [Email], [Website], [Latitude], [Longitude], [GradeID], [GradeExternalID], [Description], [Comments], [CancellationPolicy], [BankDetails], [AccountingName], [TaxTypeID], [PaymentTermID], [DefaultMargin], [DefaultCheckinTime], [DefaultCheckoutTime], [ImportID], [ExportID], [BookingWebsite], [IsRecordActive], [ParentFolderID], [AddedOn], [AddedBy], [IsDeleted])
        SELECT   [SupplierID],
                 [SupplierName],
                 [HostName],
                 [StreetAddress],
                 [PostAddress],
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
        FROM     [dbo].[Supplier]
        ORDER BY [SupplierID] ASC;
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Supplier] OFF;
    END

DROP TABLE [dbo].[Supplier];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_Supplier]', N'Supplier';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_clusteredindex_PK_Supplier]', N'PK_Supplier', N'OBJECT';

COMMIT TRANSACTION;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.SupplierConfig...';


GO
ALTER TABLE [dbo].[SupplierConfig] ALTER COLUMN [Note] VARCHAR (255) NULL;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.FK_SupplierCreditcard_Supplier...';


GO
ALTER TABLE [dbo].[SupplierCreditCard]
    ADD CONSTRAINT [FK_SupplierCreditcard_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[Supplier] ([SupplierID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.FK_SupplierContact_Supplier...';


GO
ALTER TABLE [dbo].[SupplierContact]
    ADD CONSTRAINT [FK_SupplierContact_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[Supplier] ([SupplierID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.FK_SupplierConfig_Supplier...';


GO
ALTER TABLE [dbo].[SupplierConfig]
    ADD CONSTRAINT [FK_SupplierConfig_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[Supplier] ([SupplierID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.FK_Supplier_Grade...';


GO
ALTER TABLE [dbo].[Supplier]
    ADD CONSTRAINT [FK_Supplier_Grade] FOREIGN KEY ([GradeID]) REFERENCES [dbo].[Grade] ([GradeID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.FK_Supplier_GradeExternal...';


GO
ALTER TABLE [dbo].[Supplier]
    ADD CONSTRAINT [FK_Supplier_GradeExternal] FOREIGN KEY ([GradeExternalID]) REFERENCES [dbo].[GradeExternal] ([GradeExternalID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.FK_Service_Supplier...';


GO
ALTER TABLE [dbo].[Service]
    ADD CONSTRAINT [FK_Service_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[Supplier] ([SupplierID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.FK_PurchaseLine_Supplier...';


GO
ALTER TABLE [dbo].[PurchaseLine]
    ADD CONSTRAINT [FK_PurchaseLine_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[Supplier] ([SupplierID]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.FK_SupplierText_Supplier...';


GO
ALTER TABLE [dbo].[SupplierText]
    ADD CONSTRAINT [FK_SupplierText_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[Supplier] ([SupplierID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.FK_SupplierMessage_Supplier...';


GO
ALTER TABLE [dbo].[SupplierMessage]
    ADD CONSTRAINT [FK_SupplierMessage_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[Supplier] ([SupplierID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.FK_SupplierNote_Supplier...';


GO
ALTER TABLE [dbo].[SupplierNote]
    ADD CONSTRAINT [FK_SupplierNote_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[Supplier] ([SupplierID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo._SupplierSet_Copy_ByID...';


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
	[TaxTypeID],
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
	[TaxTypeID],
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
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo._SupplierSet_Sel_ByID...';


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
	[TaxTypeID],
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


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Agent_Ins...';


GO
ALTER PROCEDURE [dbo].[Agent_Ins]
@AgentName VARCHAR (100), @ParentAgentID INT, @Address1 VARCHAR (255), @Address2 VARCHAR (255), @Address3 VARCHAR (255), @Phone VARCHAR (50), @Fax VARCHAR (50), @Email VARCHAR (255), @TaxNumber VARCHAR (50), @InvoiceNumberMask VARCHAR (50), @PurchasePaymentTermID INT, @SalePaymentTermID INT, @LogoFile VARCHAR (255), @VoucherLogoFile VARCHAR (255), @NetComOrMup CHAR (3), @Comments VARCHAR (2000), @AgentHeader TEXT, @RequestFooter TEXT, @ConfirmFooter TEXT, @RemitFooter TEXT, @ClientFooter TEXT, @VoucherFooter TEXT, @IsDefaultAgent BIT, @AddedOn DATETIME, @AddedBy INT, @AgentID INT OUTPUT
AS
INSERT [dbo].[Agent]
(
	[AgentName],
	[ParentAgentID],
	[Address1],
	[Address2],
	[Address3],
	[Phone],
	[Fax],
	[Email],
	[TaxNumber],
	[InvoiceNumberMask],
	[PurchasePaymentTermID],
	[SalePaymentTermID],
	[LogoFile],
	[VoucherLogoFile],
	[NetComOrMup],
	[Comments],
	[AgentHeader],
	[RequestFooter],
	[ConfirmFooter],
	[RemitFooter],
	[ClientFooter],
	[VoucherFooter],
	[IsDefaultAgent],
	[AddedOn],
	[AddedBy]
)
VALUES
(
	@AgentName,
	@ParentAgentID,
	@Address1,
	@Address2,
	@Address3,
	@Phone,
	@Fax,
	@Email,
	@TaxNumber,
	@InvoiceNumberMask,
	@PurchasePaymentTermID,
	@SalePaymentTermID,
	@LogoFile,
	@VoucherLogoFile,
	@NetComOrMup,
	@Comments,
	@AgentHeader,
	@RequestFooter,
	@ConfirmFooter,
	@RemitFooter,
	@ClientFooter,
	@VoucherFooter,
	@IsDefaultAgent,
	@AddedOn,
	@AddedBy
)
SELECT @AgentID=SCOPE_IDENTITY()


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Agent_Upd...';


GO
ALTER PROCEDURE [dbo].[Agent_Upd]
@AgentID INT, @AgentName VARCHAR (100), @ParentAgentID INT, @Address1 VARCHAR (255), @Address2 VARCHAR (255), @Address3 VARCHAR (255), @Phone VARCHAR (50), @Fax VARCHAR (50), @Email VARCHAR (255), @TaxNumber VARCHAR (50), @InvoiceNumberMask VARCHAR (50), @PurchasePaymentTermID INT, @SalePaymentTermID INT, @LogoFile VARCHAR (255), @VoucherLogoFile VARCHAR (255), @NetComOrMup CHAR (3), @Comments VARCHAR (2000), @AgentHeader TEXT, @RequestFooter TEXT, @ConfirmFooter TEXT, @RemitFooter TEXT, @ClientFooter TEXT, @VoucherFooter TEXT, @IsDefaultAgent BIT, @AddedOn DATETIME, @AddedBy INT, @RowVersion TIMESTAMP
AS
UPDATE [dbo].[Agent]
SET 
	[AgentName] = @AgentName,
	[ParentAgentID] = @ParentAgentID,
	[Address1] = @Address1,
	[Address2] = @Address2,
	[Address3] = @Address3,
	[Phone] = @Phone,
	[Fax] = @Fax,
	[Email] = @Email,
	[TaxNumber] = @TaxNumber,
	[InvoiceNumberMask] = @InvoiceNumberMask,
	[PurchasePaymentTermID] = @PurchasePaymentTermID,
	[SalePaymentTermID] = @SalePaymentTermID,
	[LogoFile] = @LogoFile,
	[VoucherLogoFile] = @VoucherLogoFile,
	[NetComOrMup] = @NetComOrMup,
	[Comments] = @Comments,
	[AgentHeader] = @AgentHeader,
	[RequestFooter] = @RequestFooter,
	[ConfirmFooter] = @ConfirmFooter,
	[RemitFooter] = @RemitFooter,
	[ClientFooter] = @ClientFooter,
	[VoucherFooter] = @VoucherFooter,
	[IsDefaultAgent] = @IsDefaultAgent,
	[AddedOn] = @AddedOn,
	[AddedBy] = @AddedBy
WHERE
	[AgentID] = @AgentID
	AND [RowVersion] = @RowVersion


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Itinerary_Ins...';


GO
ALTER PROCEDURE [dbo].[Itinerary_Ins]
@ItineraryName VARCHAR (255), @DisplayName VARCHAR (255), @CustomCode VARCHAR (50), @ArriveDate DATETIME, @ArriveCityID INT, @ArriveFlight VARCHAR (50), @ArriveNote VARCHAR (2000), @DepartDate DATETIME, @DepartCityID INT, @DepartFlight VARCHAR (50), @DepartNote VARCHAR (2000), @NetComOrMup CHAR (3), @NetMargin DECIMAL (7, 4), @GrossMarkup DECIMAL (7, 4), @GrossOverride MONEY, @IsLockedGrossOverride BIT, @PricingNote VARCHAR (500), @AgentID INT, @PaymentTermID INT, @ItineraryStatusID INT, @ItinerarySourceID INT, @CountryID INT, @AssignedTo INT, @DepartmentID INT, @BranchID INT, @PaxOverride INT, @Comments VARCHAR (4000), @IsRecordActive BIT, @IsReadOnly BIT, @ParentFolderID INT, @AddedOn DATETIME, @AddedBy INT, @IsDeleted BIT, @ItineraryID INT OUTPUT
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
	[IsDeleted]
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
	@IsDeleted
)
SELECT @ItineraryID=SCOPE_IDENTITY()


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Itinerary_Upd...';


GO
ALTER PROCEDURE [dbo].[Itinerary_Upd]
@ItineraryID INT, @ItineraryName VARCHAR (255), @DisplayName VARCHAR (255), @CustomCode VARCHAR (50), @ArriveDate DATETIME, @ArriveCityID INT, @ArriveFlight VARCHAR (50), @ArriveNote VARCHAR (2000), @DepartDate DATETIME, @DepartCityID INT, @DepartFlight VARCHAR (50), @DepartNote VARCHAR (2000), @NetComOrMup CHAR (3), @NetMargin DECIMAL (7, 4), @GrossMarkup DECIMAL (7, 4), @GrossOverride MONEY, @IsLockedGrossOverride BIT, @PricingNote VARCHAR (500), @AgentID INT, @PaymentTermID INT, @ItineraryStatusID INT, @ItinerarySourceID INT, @CountryID INT, @AssignedTo INT, @DepartmentID INT, @BranchID INT, @PaxOverride INT, @Comments VARCHAR (4000), @IsRecordActive BIT, @IsReadOnly BIT, @ParentFolderID INT, @AddedOn DATETIME, @AddedBy INT, @RowVersion TIMESTAMP, @IsDeleted BIT
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
	[IsDeleted] = @IsDeleted
WHERE
	[ItineraryID] = @ItineraryID
	AND [RowVersion] = @RowVersion


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Service_Ins...';


GO
ALTER PROCEDURE [dbo].[Service_Ins]
@ServiceName VARCHAR (150), @SupplierID INT, @ServiceTypeID INT, @Description VARCHAR (2000), @Comments VARCHAR (2000), @Warning VARCHAR (2000), @MaxPax INT, @CheckinTime DATETIME, @CheckoutTime DATETIME, @CheckinMinutesEarly INT, @IsRecordActive BIT, @CurrencyCode CHAR (3), @ChargeType VARCHAR (10), @PaymentTermID INT, @TaxTypeID INT, @Latitude FLOAT, @Longitude FLOAT, @AddedOn DATETIME, @AddedBy INT, @IsDeleted BIT, @ServiceID INT OUTPUT
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
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Service_Sel_All...';


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
	[TaxTypeID],
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
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Service_Sel_ByID...';


GO
ALTER PROCEDURE [dbo].[Service_Sel_ByID]
@ServiceID INT
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
	[TaxTypeID],
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
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Service_Sel_BySupplierID...';


GO
ALTER PROCEDURE [dbo].[Service_Sel_BySupplierID]
@SupplierID INT
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
	[TaxTypeID],
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
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Service_Upd...';


GO
ALTER PROCEDURE [dbo].[Service_Upd]
@ServiceID INT, @ServiceName VARCHAR (150), @SupplierID INT, @ServiceTypeID INT, @Description VARCHAR (2000), @Comments VARCHAR (2000), @Warning VARCHAR (2000), @MaxPax INT, @CheckinTime DATETIME, @CheckoutTime DATETIME, @CheckinMinutesEarly INT, @IsRecordActive BIT, @CurrencyCode CHAR (3), @ChargeType VARCHAR (10), @PaymentTermID INT, @TaxTypeID INT, @Latitude FLOAT, @Longitude FLOAT, @AddedOn DATETIME, @AddedBy INT, @RowVersion TIMESTAMP, @IsDeleted BIT
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
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.ServiceConfig_Ins...';


GO
ALTER PROCEDURE [dbo].[ServiceConfig_Ins]
@ServiceID INT, @ServiceConfigTypeID INT, @Note VARCHAR (255), @AddedOn DATETIME, @AddedBy INT
AS
INSERT [dbo].[ServiceConfig]
(
	[ServiceID],
	[ServiceConfigTypeID],
	[Note],
	[AddedOn],
	[AddedBy]
)
VALUES
(
	@ServiceID,
	@ServiceConfigTypeID,
	@Note,
	@AddedOn,
	@AddedBy
)


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.ServiceConfig_Sel_All...';


GO
ALTER PROCEDURE [dbo].[ServiceConfig_Sel_All]

AS
SET NOCOUNT ON
SELECT
	[ServiceID],
	[ServiceConfigTypeID],
	[Note],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[ServiceConfig]
ORDER BY 
	[ServiceID] ASC
	, [ServiceConfigTypeID] ASC


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.ServiceConfig_Sel_ByID...';


GO
ALTER PROCEDURE [dbo].[ServiceConfig_Sel_ByID]
@ServiceID INT, @ServiceConfigTypeID INT
AS
SET NOCOUNT ON
SELECT
	[ServiceID],
	[ServiceConfigTypeID],
	[Note],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[ServiceConfig]
WHERE
	[ServiceID] = @ServiceID
	AND [ServiceConfigTypeID] = @ServiceConfigTypeID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.ServiceConfig_Sel_ByServiceConfigTypeID...';


GO
ALTER PROCEDURE [dbo].[ServiceConfig_Sel_ByServiceConfigTypeID]
@ServiceConfigTypeID INT
AS
SET NOCOUNT ON
SELECT
	[ServiceID],
	[ServiceConfigTypeID],
	[Note],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[ServiceConfig]
WHERE
	[ServiceConfigTypeID] = @ServiceConfigTypeID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.ServiceConfig_Sel_ByServiceID...';


GO
ALTER PROCEDURE [dbo].[ServiceConfig_Sel_ByServiceID]
@ServiceID INT
AS
SET NOCOUNT ON
SELECT
	[ServiceID],
	[ServiceConfigTypeID],
	[Note],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[ServiceConfig]
WHERE
	[ServiceID] = @ServiceID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.ServiceConfig_Upd...';


GO
ALTER PROCEDURE [dbo].[ServiceConfig_Upd]
@ServiceID INT, @ServiceConfigTypeID INT, @Note VARCHAR (255), @AddedOn DATETIME, @AddedBy INT, @RowVersion TIMESTAMP
AS
UPDATE [dbo].[ServiceConfig]
SET 
	[Note] = @Note,
	[AddedOn] = @AddedOn,
	[AddedBy] = @AddedBy
WHERE
	[ServiceID] = @ServiceID
	AND [ServiceConfigTypeID] = @ServiceConfigTypeID
	AND [RowVersion] = @RowVersion


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Supplier_Ins...';


GO
ALTER PROCEDURE [dbo].[Supplier_Ins]
@SupplierName VARCHAR (150), @HostName VARCHAR (150), @StreetAddress VARCHAR (100), @PostAddress VARCHAR (500), @Postcode VARCHAR (20), @CityID INT, @RegionID INT, @StateID INT, @CountryID INT, @Phone VARCHAR (50), @MobilePhone VARCHAR (50), @FreePhone VARCHAR (50), @Fax VARCHAR (50), @Email VARCHAR (200), @Website VARCHAR (200), @Latitude FLOAT, @Longitude FLOAT, @GradeID INT, @GradeExternalID INT, @Description VARCHAR (4000), @Comments VARCHAR (4000), @CancellationPolicy VARCHAR (2000), @BankDetails VARCHAR (255), @AccountingName VARCHAR (150), @TaxTypeID INT, @PaymentTermID INT, @DefaultMargin REAL, @DefaultCheckinTime DATETIME, @DefaultCheckoutTime DATETIME, @ImportID VARCHAR (50), @ExportID VARCHAR (50), @BookingWebsite VARCHAR (255), @IsRecordActive BIT, @ParentFolderID INT, @AddedOn DATETIME, @AddedBy INT, @IsDeleted BIT, @SupplierID INT OUTPUT
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
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Supplier_Sel_All...';


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
	[RowVersion],
	[IsDeleted]
FROM [dbo].[Supplier]
ORDER BY 
	[SupplierID] ASC


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Supplier_Sel_ByGradeExternalID...';


GO
ALTER PROCEDURE [dbo].[Supplier_Sel_ByGradeExternalID]
@GradeExternalID INT
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
	[RowVersion],
	[IsDeleted]
FROM [dbo].[Supplier]
WHERE
	[GradeExternalID] = @GradeExternalID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Supplier_Sel_ByGradeID...';


GO
ALTER PROCEDURE [dbo].[Supplier_Sel_ByGradeID]
@GradeID INT
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
	[RowVersion],
	[IsDeleted]
FROM [dbo].[Supplier]
WHERE
	[GradeID] = @GradeID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Supplier_Sel_ByID...';


GO
ALTER PROCEDURE [dbo].[Supplier_Sel_ByID]
@SupplierID INT
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
	[RowVersion],
	[IsDeleted]
FROM [dbo].[Supplier]
WHERE
	[SupplierID] = @SupplierID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Supplier_Upd...';


GO
ALTER PROCEDURE [dbo].[Supplier_Upd]
@SupplierID INT, @SupplierName VARCHAR (150), @HostName VARCHAR (150), @StreetAddress VARCHAR (100), @PostAddress VARCHAR (500), @Postcode VARCHAR (20), @CityID INT, @RegionID INT, @StateID INT, @CountryID INT, @Phone VARCHAR (50), @MobilePhone VARCHAR (50), @FreePhone VARCHAR (50), @Fax VARCHAR (50), @Email VARCHAR (200), @Website VARCHAR (200), @Latitude FLOAT, @Longitude FLOAT, @GradeID INT, @GradeExternalID INT, @Description VARCHAR (4000), @Comments VARCHAR (4000), @CancellationPolicy VARCHAR (2000), @BankDetails VARCHAR (255), @AccountingName VARCHAR (150), @TaxTypeID INT, @PaymentTermID INT, @DefaultMargin REAL, @DefaultCheckinTime DATETIME, @DefaultCheckoutTime DATETIME, @ImportID VARCHAR (50), @ExportID VARCHAR (50), @BookingWebsite VARCHAR (255), @IsRecordActive BIT, @ParentFolderID INT, @AddedOn DATETIME, @AddedBy INT, @RowVersion TIMESTAMP, @IsDeleted BIT
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
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.SupplierConfig_Ins...';


GO
ALTER PROCEDURE [dbo].[SupplierConfig_Ins]
@SupplierID INT, @SupplierConfigTypeID INT, @Note VARCHAR (255), @AddedOn DATETIME, @AddedBy INT
AS
INSERT [dbo].[SupplierConfig]
(
	[SupplierID],
	[SupplierConfigTypeID],
	[Note],
	[AddedOn],
	[AddedBy]
)
VALUES
(
	@SupplierID,
	@SupplierConfigTypeID,
	@Note,
	@AddedOn,
	@AddedBy
)


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.SupplierConfig_Sel_All...';


GO
ALTER PROCEDURE [dbo].[SupplierConfig_Sel_All]

AS
SET NOCOUNT ON
SELECT
	[SupplierID],
	[SupplierConfigTypeID],
	[Note],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[SupplierConfig]
ORDER BY 
	[SupplierID] ASC
	, [SupplierConfigTypeID] ASC


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.SupplierConfig_Sel_ByID...';


GO
ALTER PROCEDURE [dbo].[SupplierConfig_Sel_ByID]
@SupplierID INT, @SupplierConfigTypeID INT
AS
SET NOCOUNT ON
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
	AND [SupplierConfigTypeID] = @SupplierConfigTypeID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.SupplierConfig_Sel_BySupplierConfigTypeID...';


GO
ALTER PROCEDURE [dbo].[SupplierConfig_Sel_BySupplierConfigTypeID]
@SupplierConfigTypeID INT
AS
SET NOCOUNT ON
SELECT
	[SupplierID],
	[SupplierConfigTypeID],
	[Note],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[SupplierConfig]
WHERE
	[SupplierConfigTypeID] = @SupplierConfigTypeID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.SupplierConfig_Sel_BySupplierID...';


GO
ALTER PROCEDURE [dbo].[SupplierConfig_Sel_BySupplierID]
@SupplierID INT
AS
SET NOCOUNT ON
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


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.SupplierConfig_Upd...';


GO
ALTER PROCEDURE [dbo].[SupplierConfig_Upd]
@SupplierID INT, @SupplierConfigTypeID INT, @Note VARCHAR (255), @AddedOn DATETIME, @AddedBy INT, @RowVersion TIMESTAMP
AS
UPDATE [dbo].[SupplierConfig]
SET 
	[Note] = @Note,
	[AddedOn] = @AddedOn,
	[AddedBy] = @AddedBy
WHERE
	[SupplierID] = @SupplierID
	AND [SupplierConfigTypeID] = @SupplierConfigTypeID
	AND [RowVersion] = @RowVersion


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.PurchaseItemPricing...';


GO
ALTER FUNCTION [dbo].[PurchaseItemPricing]
( )
RETURNS TABLE 
AS
RETURN 
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
	) t
)



GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.ItineraryPricing...';


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
		GrossOrig,
		case when Net = 0 then 0 else (GrossOrig - Net)/Net*100 end as MarkupOrig,
		case when GrossOrig = 0 then 0 else (GrossOrig - Net)/GrossOrig*100 end as CommissionOrig
	from
	(
		select i.ItineraryID, isnull(ItemCount,0) as ItemCount, Net, 
		case
			when (GrossOverride is not null) then GrossOverride
			when (GrossMarkup   is not null) then isnull(Gross,0) * (1+GrossMarkup/100)
			else Gross
		end as Gross,
		case
			when (GrossOverride is not null) then GrossOverride
			when (GrossMarkup   is not null) then isnull(GrossOrig,0) * (1+GrossMarkup/100)
			else GrossOrig
		end as GrossOrig
		from Itinerary i
		left outer join
		(
			select ItineraryID, count(*) as ItemCount, sum(TotalNet) as Net, sum(TotalGross) as Gross, sum(TotalGrossOrig) as GrossOrig
			from PurchaseItemPricing()
			group by ItineraryID
		) p on i.ItineraryID = p.ItineraryID
	) t
)



GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Refreshing dbo.ItineraryDetail...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryDetail';


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Refreshing dbo.ItineraryServiceTypeDetail...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryServiceTypeDetail';


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Refreshing dbo.ItinerarySaleDetail...';


GO
EXECUTE sp_refreshview N'dbo.ItinerarySaleDetail';


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Refreshing dbo.ItineraryClientDetail...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryClientDetail';


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Refreshing dbo.ItinerarySaleAllocationDetail...';


GO
EXECUTE sp_refreshview N'dbo.ItinerarySaleAllocationDetail';


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Refreshing dbo.ItineraryPaymentDetail...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryPaymentDetail';


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.SupplierDetail...';


GO
ALTER VIEW [dbo].[SupplierDetail]
AS
select 
	SupplierID,  
	SupplierName,
	[Description] as SupplierDescription,
	[Comments] as SupplierComments,
	HostName,
	StreetAddress,
	PostAddress,
	CityName,
	RegionName,
	StateName,
	CountryName,
	Postcode,
	Latitude,
	Longitude,
	Phone,
	MobilePhone,
	FreePhone,
	Fax,
	Email,
	Website,
	CancellationPolicy,
	isnull(GradeName, '') as Grade1,
	isnull(GradeExternalName, '') as Grade2,
	sup.ParentFolderID as SupplierParentFolderID
from Supplier as sup
left outer join City as city on city.CityID = sup.CityID 
left outer join Region as region on region.RegionID = sup.RegionID 
left outer join [State] as stat on stat.StateID = sup.StateID 
left outer join Country as country on country.CountryID = sup.CountryID 
left outer join Grade as grade on grade.GradeID = sup.GradeID 
left outer join GradeExternal as gradeEx on gradeEx.GradeExternalID = sup.GradeExternalID;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.SupplierRatesDetail...';


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
	opt.Gross
from [service] serv
left outer join ServiceTypeDetail stype on serv.ServiceTypeID = stype.ServiceTypeID
left outer join SupplierDetail sup on serv.SupplierID = sup.SupplierID
right outer join Rate rate on serv.ServiceID = rate.ServiceID
right outer join [Option] opt on rate.RateID = opt.RateID;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Refreshing dbo.PurchaseItemDetail...';


GO
EXECUTE sp_refreshview N'dbo.PurchaseItemDetail';


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Refreshing dbo.PurchaseItemPaymentsDetail...';


GO
EXECUTE sp_refreshview N'dbo.PurchaseItemPaymentsDetail';


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2009.5.6'
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
----------------------------------------------------------------------------------------
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
