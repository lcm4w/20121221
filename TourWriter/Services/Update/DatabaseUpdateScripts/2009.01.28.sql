/*
TourWriter database update script, from version 2008.12.01 to 2009.1.28
*/
GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;
SET NUMERIC_ROUNDABORT OFF;
GO

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
PRINT N'Dropping dbo.DF_ServiceOption_AddedOn...';


GO
ALTER TABLE [dbo].[Option] DROP CONSTRAINT [DF_ServiceOption_AddedOn];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.DF_ServiceOption_AddedBy...';


GO
ALTER TABLE [dbo].[Option] DROP CONSTRAINT [DF_ServiceOption_AddedBy];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.DF_Option_IsRecordActive...';


GO
ALTER TABLE [dbo].[Option] DROP CONSTRAINT [DF_Option_IsRecordActive];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.DF_Service_ServiceName...';


GO
ALTER TABLE [dbo].[Service] DROP CONSTRAINT [DF_Service_ServiceName];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.DF_Service_SupplierID...';


GO
ALTER TABLE [dbo].[Service] DROP CONSTRAINT [DF_Service_SupplierID];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.DF_Service_ServiceTypeID...';


GO
ALTER TABLE [dbo].[Service] DROP CONSTRAINT [DF_Service_ServiceTypeID];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.DF_Service_Description...';


GO
ALTER TABLE [dbo].[Service] DROP CONSTRAINT [DF_Service_Description];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.DF_Service_Checkin...';


GO
ALTER TABLE [dbo].[Service] DROP CONSTRAINT [DF_Service_Checkin];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.DF_Service_Checkout...';


GO
ALTER TABLE [dbo].[Service] DROP CONSTRAINT [DF_Service_Checkout];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.DF_Service_CheckinMinutesEarly...';


GO
ALTER TABLE [dbo].[Service] DROP CONSTRAINT [DF_Service_CheckinMinutesEarly];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.DF_Service_IsRecordActive...';


GO
ALTER TABLE [dbo].[Service] DROP CONSTRAINT [DF_Service_IsRecordActive];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.DF_Service_DateAdded...';


GO
ALTER TABLE [dbo].[Service] DROP CONSTRAINT [DF_Service_DateAdded];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.DF_Service_AddedBy...';


GO
ALTER TABLE [dbo].[Service] DROP CONSTRAINT [DF_Service_AddedBy];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.DF_ServiceType_ServiceTypeName...';


GO
ALTER TABLE [dbo].[ServiceType] DROP CONSTRAINT [DF_ServiceType_ServiceTypeName];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.FK_PurchaseItem_Option...';


GO
ALTER TABLE [dbo].[PurchaseItem] DROP CONSTRAINT [FK_PurchaseItem_Option];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.FK_Option_Rate...';


GO
ALTER TABLE [dbo].[Option] DROP CONSTRAINT [FK_Option_Rate];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.FK_ServiceTime_Service...';


GO
ALTER TABLE [dbo].[ServiceTime] DROP CONSTRAINT [FK_ServiceTime_Service];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.FK_ServiceConfig_Service...';


GO
ALTER TABLE [dbo].[ServiceConfig] DROP CONSTRAINT [FK_ServiceConfig_Service];


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
PRINT N'Dropping dbo.FK_Rate_Service...';


GO
ALTER TABLE [dbo].[Rate] DROP CONSTRAINT [FK_Rate_Service];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.FK_ItinerarySaleAllocation_ServiceType...';


GO
ALTER TABLE [dbo].[ItinerarySaleAllocation] DROP CONSTRAINT [FK_ItinerarySaleAllocation_ServiceType];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.GroupPaxRange...';


GO
DROP TABLE [dbo].[GroupPaxRange];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.GroupPaxRange_Del...';


GO
DROP PROCEDURE [dbo].[GroupPaxRange_Del];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.GroupPaxRange_Ins...';


GO
DROP PROCEDURE [dbo].[GroupPaxRange_Ins];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.GroupPaxRange_Sel_All...';


GO
DROP PROCEDURE [dbo].[GroupPaxRange_Sel_All];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.GroupPaxRange_Sel_ByID...';


GO
DROP PROCEDURE [dbo].[GroupPaxRange_Sel_ByID];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping dbo.GroupPaxRange_Upd...';


GO
DROP PROCEDURE [dbo].[GroupPaxRange_Upd];


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Starting rebuilding table dbo.Option...';


GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;


GO
BEGIN TRANSACTION;

CREATE TABLE [dbo].[tmp_ms_xx_Option] (
    [OptionID]       INT           IDENTITY (1, 1) NOT NULL,
    [OptionName]     VARCHAR (150) NOT NULL,
    [OptionTypeID]   INT           NULL,
    [RateID]         INT           NULL,
    [Net]            MONEY         NULL,
    [Gross]          MONEY         NULL,
    [PricingOption]  CHAR (2)      NULL,
    [IsDefault]      BIT           NULL,
    [IsRecordActive] BIT           CONSTRAINT [DF_Option_IsRecordActive] DEFAULT ((1)) NULL,
    [AddedOn]        DATETIME      CONSTRAINT [DF_ServiceOption_AddedOn] DEFAULT (getdate()) NULL,
    [AddedBy]        INT           CONSTRAINT [DF_ServiceOption_AddedBy] DEFAULT ((0)) NULL,
    [RowVersion]     TIMESTAMP     NULL,
    [IsDeleted]      BIT           NULL
);

ALTER TABLE [dbo].[tmp_ms_xx_Option]
    ADD CONSTRAINT [tmp_ms_xx_clusteredindex_PK_ServiceOption] PRIMARY KEY CLUSTERED ([OptionID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

IF EXISTS (SELECT TOP 1 1
           FROM   [dbo].[Option])
    BEGIN
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Option] ON;
        INSERT INTO [dbo].[tmp_ms_xx_Option] ([OptionID], [OptionName], [RateID], [Net], [Gross], [PricingOption], [IsRecordActive], [AddedOn], [AddedBy], [IsDeleted])
        SELECT   [OptionID],
                 [OptionName],
                 [RateID],
                 [Net],
                 [Gross],
                 [PricingOption],
                 [IsRecordActive],
                 [AddedOn],
                 [AddedBy],
                 [IsDeleted]
        FROM     [dbo].[Option]
        ORDER BY [OptionID] ASC;
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Option] OFF;
    END

DROP TABLE [dbo].[Option];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_Option]', N'Option';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_clusteredindex_PK_ServiceOption]', N'PK_ServiceOption', N'OBJECT';

COMMIT TRANSACTION;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Starting rebuilding table dbo.Service...';


GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;


GO
BEGIN TRANSACTION;

CREATE TABLE [dbo].[tmp_ms_xx_Service] (
    [ServiceID]           INT            IDENTITY (1, 1) NOT NULL,
    [ServiceName]         VARCHAR (150)  CONSTRAINT [DF_Service_ServiceName] DEFAULT ('') NOT NULL,
    [SupplierID]          INT            CONSTRAINT [DF_Service_SupplierID] DEFAULT ((0)) NULL,
    [ServiceTypeID]       INT            CONSTRAINT [DF_Service_ServiceTypeID] DEFAULT ((0)) NULL,
    [Description]         VARCHAR (2000) CONSTRAINT [DF_Service_Description] DEFAULT ('') NULL,
    [Comments]            VARCHAR (2000) NULL,
    [Warning]             VARCHAR (2000) NULL,
    [MaxPax]              INT            NULL,
    [CheckinTime]         DATETIME       CONSTRAINT [DF_Service_Checkin] DEFAULT ('12:00') NULL,
    [CheckoutTime]        DATETIME       CONSTRAINT [DF_Service_Checkout] DEFAULT ('12:00') NULL,
    [CheckinMinutesEarly] INT            CONSTRAINT [DF_Service_CheckinMinutesEarly] DEFAULT ((0)) NULL,
    [IsRecordActive]      BIT            CONSTRAINT [DF_Service_IsRecordActive] DEFAULT ((1)) NULL,
    [CurrencyCode]        CHAR (3)       NULL,
    [ChargeType]          VARCHAR (10)   NULL,
    [PaymentTermID]       INT            NULL,
    [TaxTypeID]           INT            NULL,
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
        INSERT INTO [dbo].[tmp_ms_xx_Service] ([ServiceID], [ServiceName], [SupplierID], [ServiceTypeID], [Description], [Comments], [Warning], [MaxPax], [CheckinTime], [CheckoutTime], [CheckinMinutesEarly], [IsRecordActive], [CurrencyCode], [PaymentTermID], [TaxTypeID], [AddedOn], [AddedBy], [IsDeleted])
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
                 [PaymentTermID],
                 [TaxTypeID],
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


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Starting rebuilding table dbo.ServiceType...';


GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;


GO
BEGIN TRANSACTION;

CREATE TABLE [dbo].[tmp_ms_xx_ServiceType] (
    [ServiceTypeID]               INT           IDENTITY (1, 1) NOT NULL,
    [ServiceTypeName]             VARCHAR (100) CONSTRAINT [DF_ServiceType_ServiceTypeName] DEFAULT ('') NOT NULL,
    [BookingStartName]            VARCHAR (50)  NULL,
    [BookingEndName]              VARCHAR (50)  NULL,
    [NumberOfDaysName]            VARCHAR (50)  NULL,
    [NetAccountingCategoryID]     INT           NULL,
    [NetTaxTypeID]                INT           NULL,
    [GrossAccountingCategoryID]   INT           NULL,
    [GrossTaxTypeID]              INT           NULL,
    [IsDeleted]                   BIT           NULL,
    [IsAdditionalMarkupContainer] BIT           NULL
);

ALTER TABLE [dbo].[tmp_ms_xx_ServiceType]
    ADD CONSTRAINT [tmp_ms_xx_clusteredindex_PK_ServiceType] PRIMARY KEY CLUSTERED ([ServiceTypeID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

IF EXISTS (SELECT TOP 1 1
           FROM   [dbo].[ServiceType])
    BEGIN
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_ServiceType] ON;
        INSERT INTO [dbo].[tmp_ms_xx_ServiceType] ([ServiceTypeID], [ServiceTypeName], [BookingStartName], [BookingEndName], [NetAccountingCategoryID], [NetTaxTypeID], [GrossAccountingCategoryID], [GrossTaxTypeID], [IsDeleted], [IsAdditionalMarkupContainer])
        SELECT   [ServiceTypeID],
                 [ServiceTypeName],
                 [BookingStartName],
                 [BookingEndName],
                 [NetAccountingCategoryID],
                 [NetTaxTypeID],
                 [GrossAccountingCategoryID],
                 [GrossTaxTypeID],
                 [IsDeleted],
                 [IsAdditionalMarkupContainer]
        FROM     [dbo].[ServiceType]
        ORDER BY [ServiceTypeID] ASC;
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_ServiceType] OFF;
    END

DROP TABLE [dbo].[ServiceType];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_ServiceType]', N'ServiceType';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_clusteredindex_PK_ServiceType]', N'PK_ServiceType', N'OBJECT';

COMMIT TRANSACTION;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Template...';


GO
ALTER TABLE [dbo].[Template] ALTER COLUMN [FilePath] VARCHAR (255) NOT NULL;

ALTER TABLE [dbo].[Template]
    ADD [ParentTemplateCategoryID] INT NULL;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.Flag...';


GO
CREATE TABLE [dbo].[Flag] (
    [FlagID]    INT   IDENTITY (1, 1) NOT NULL,
    [FlagImage] IMAGE NOT NULL
);


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.ItineraryPax...';


GO
CREATE TABLE [dbo].[ItineraryPax] (
    [ItineraryPaxID]   INT          IDENTITY (1, 1) NOT NULL,
    [ItineraryPaxName] VARCHAR (50) NOT NULL,
    [ItineraryID]      INT          NOT NULL,
    [MemberCount]      INT          NULL,
    [MemberRooms]      INT          NULL,
    [StaffCount]       INT          NULL,
    [StaffRooms]       INT          NULL
);


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.OptionType...';


GO
CREATE TABLE [dbo].[OptionType] (
    [OptionTypeID]   INT          IDENTITY (1, 1) NOT NULL,
    [OptionTypeName] VARCHAR (10) NOT NULL,
    [Divisor]        INT          NULL
);


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.PurchaseItemNote...';


GO
CREATE TABLE [dbo].[PurchaseItemNote] (
    [PurchaseItemNoteID] INT           IDENTITY (1, 1) NOT NULL,
    [PurchaseItemID]     INT           NOT NULL,
    [FlagID]             INT           NOT NULL,
    [Note]               VARCHAR (255) NOT NULL
);


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.ServiceChargeType...';


GO
CREATE TABLE [dbo].[ServiceChargeType] (
    [ServiceChargeTypeID]   INT          IDENTITY (1, 1) NOT NULL,
    [ServiceChargeTypeName] VARCHAR (50) NOT NULL
);


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.ServiceFoc...';


GO
CREATE TABLE [dbo].[ServiceFoc] (
    [ServiceFocID] INT IDENTITY (1, 1) NOT NULL,
    [ServiceID]    INT NOT NULL,
    [UnitsUsed]    INT NOT NULL,
    [UnitsFree]    INT NOT NULL
);


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.TemplateCategory...';


GO
CREATE TABLE [dbo].[TemplateCategory] (
    [TemplateCategoryID]       INT           IDENTITY (100, 1) NOT NULL,
    [TemplateCategoryName]     VARCHAR (100) NOT NULL,
    [ParentTemplateCategoryID] INT           NULL
);


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.PK_Flag...';


GO
ALTER TABLE [dbo].[Flag]
    ADD CONSTRAINT [PK_Flag] PRIMARY KEY CLUSTERED ([FlagID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.PK_ItineraryPax...';


GO
ALTER TABLE [dbo].[ItineraryPax]
    ADD CONSTRAINT [PK_ItineraryPax] PRIMARY KEY CLUSTERED ([ItineraryPaxID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.PK_OptionType...';


GO
ALTER TABLE [dbo].[OptionType]
    ADD CONSTRAINT [PK_OptionType] PRIMARY KEY CLUSTERED ([OptionTypeID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.PK_PurchaseItemMessage...';


GO
ALTER TABLE [dbo].[PurchaseItemNote]
    ADD CONSTRAINT [PK_PurchaseItemMessage] PRIMARY KEY CLUSTERED ([PurchaseItemNoteID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.PK_ServiceChargeType...';


GO
ALTER TABLE [dbo].[ServiceChargeType]
    ADD CONSTRAINT [PK_ServiceChargeType] PRIMARY KEY CLUSTERED ([ServiceChargeTypeID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.PK_Foc...';


GO
ALTER TABLE [dbo].[ServiceFoc]
    ADD CONSTRAINT [PK_Foc] PRIMARY KEY CLUSTERED ([ServiceFocID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.PK_TemplateCategory...';


GO
ALTER TABLE [dbo].[TemplateCategory]
    ADD CONSTRAINT [PK_TemplateCategory] PRIMARY KEY CLUSTERED ([TemplateCategoryID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.FK_PurchaseItem_Option...';


GO
ALTER TABLE [dbo].[PurchaseItem]
    ADD CONSTRAINT [FK_PurchaseItem_Option] FOREIGN KEY ([OptionID]) REFERENCES [dbo].[Option] ([OptionID]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.FK_Option_Rate...';


GO
ALTER TABLE [dbo].[Option]
    ADD CONSTRAINT [FK_Option_Rate] FOREIGN KEY ([RateID]) REFERENCES [dbo].[Rate] ([RateID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.FK_ServiceTime_Service...';


GO
ALTER TABLE [dbo].[ServiceTime]
    ADD CONSTRAINT [FK_ServiceTime_Service] FOREIGN KEY ([ServiceID]) REFERENCES [dbo].[Service] ([ServiceID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.FK_ServiceConfig_Service...';


GO
ALTER TABLE [dbo].[ServiceConfig]
    ADD CONSTRAINT [FK_ServiceConfig_Service] FOREIGN KEY ([ServiceID]) REFERENCES [dbo].[Service] ([ServiceID]) ON DELETE CASCADE ON UPDATE CASCADE;


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
PRINT N'Creating dbo.FK_Rate_Service...';


GO
ALTER TABLE [dbo].[Rate]
    ADD CONSTRAINT [FK_Rate_Service] FOREIGN KEY ([ServiceID]) REFERENCES [dbo].[Service] ([ServiceID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.FK_ItinerarySaleAllocation_ServiceType...';


GO
ALTER TABLE [dbo].[ItinerarySaleAllocation]
    ADD CONSTRAINT [FK_ItinerarySaleAllocation_ServiceType] FOREIGN KEY ([ServiceTypeID]) REFERENCES [dbo].[ServiceType] ([ServiceTypeID]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.FK_ItineraryPax_Itinerary...';


GO
ALTER TABLE [dbo].[ItineraryPax]
    ADD CONSTRAINT [FK_ItineraryPax_Itinerary] FOREIGN KEY ([ItineraryID]) REFERENCES [dbo].[Itinerary] ([ItineraryID]) ON DELETE NO ACTION ON UPDATE NO ACTION;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.__Repair_Login...';


GO
ALTER PROCEDURE [dbo].[__Repair_Login]

AS
EXEC sp_change_users_login 'Update_One', 'twuser', 'twuser'


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.__Repair_OrphanedMenuItems...';


GO
ALTER PROCEDURE [dbo].[__Repair_OrphanedMenuItems]

AS
UPDATE Folder
SET ParentFolderID = 0
WHERE FolderID = ParentFolderID

UPDATE Folder
SET ParentFolderID = 0
WHERE ParentFolderID != 0 
AND ParentFolderID NOT IN (SELECT FolderID FROM Folder)

UPDATE Itinerary
SET ParentFolderID = 0
WHERE ParentFolderID != 0 
AND ParentFolderID NOT IN (SELECT FolderID FROM Folder)

UPDATE Supplier
SET ParentFolderID = 0
WHERE ParentFolderID != 0 
AND ParentFolderID NOT IN (SELECT FolderID FROM Folder)

UPDATE Contact
SET ParentFolderID = 0
WHERE ParentFolderID != 0 
AND ParentFolderID NOT IN (SELECT FolderID FROM Folder)


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo._Contact_New...';


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
	[AddedBy]
)
VALUES
(
	@ContactName,
	1,
	@ParentFolderID,
	@AddedOn,
	@AddedBy
)

SELECT SCOPE_IDENTITY()


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo._Contact_Rename...';


GO
ALTER PROCEDURE [dbo].[_Contact_Rename]
@ContactID INT, @ContactName VARCHAR (100)
AS
SET NOCOUNT ON

UPDATE Contact
SET ContactName = @ContactName
WHERE ContactID = @ContactID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo._ContactSet_Copy_ByID...';


GO
ALTER PROCEDURE [dbo].[_ContactSet_Copy_ByID]
@OrigContactID INT, @NewContactName VARCHAR (100), @AddedOn DATETIME, @AddedBy INT
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
	[AddedBy]
)
SELECT
	@NewContactName,
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
	@AddedOn,
	@AddedBy
FROM [dbo].[Contact]
WHERE
	[ContactID] = @OrigContactID

SELECT SCOPE_IDENTITY()


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo._Folder_New...';


GO
ALTER PROCEDURE [dbo].[_Folder_New]
@FolderName VARCHAR (200), @ParentFolderID INT, @MenuTypeID INT, @AddedOn DATETIME, @AddedBy INT
AS
SET NOCOUNT ON

INSERT [dbo].[Folder]
(
	[FolderName],
	[ParentFolderID],
	[MenuTypeID],
	[IsRecordActive],
	[AddedOn],
	[AddedBy]
)
VALUES
(
	@FolderName,
	@ParentFolderID,
	@MenuTypeID,
	1,
	@AddedOn,
	@AddedBy
)
SELECT SCOPE_IDENTITY()


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo._Folder_Rename...';


GO
ALTER PROCEDURE [dbo].[_Folder_Rename]
@FolderID INT, @FolderName VARCHAR (200)
AS
SET NOCOUNT ON

UPDATE Folder
SET FolderName = @FolderName
WHERE FolderID = @FolderID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo._Itinerary_Rename...';


GO
ALTER PROCEDURE [dbo].[_Itinerary_Rename]
@ItineraryID INT, @ItineraryName VARCHAR (100)
AS
SET NOCOUNT ON

UPDATE Itinerary
SET ItineraryName = @ItineraryName
WHERE ItineraryID = @ItineraryID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo._Itinerary_ServiceSearch...';


GO
ALTER PROCEDURE [dbo].[_Itinerary_ServiceSearch]
@CityID INT=NULL, @RegionID INT=NULL, @GradeID INT=NULL, @ServiceTypeID INT=NULL
AS
SET NOCOUNT ON	

SELECT 
	sp.[SupplierID],  
	sr.[ServiceID], 
	sp.[SupplierName],
	sr.[ServiceName],
	sr.[ServiceTypeID], 
	sp.[CityID], 
	sp.[GradeID]
FROM 
	[dbo].[Supplier] sp,
	[dbo].[Service] sr
WHERE 
	sr.[SupplierID] = sp.[SupplierID] 
-- add search params, allowing for both null field values and null params --	
AND ISNULL(sp.[CityID],   '')            = ISNULL(ISNULL(@CityID, sp.CityID),   '') 
AND ISNULL(sp.[RegionID],'')         = ISNULL(ISNULL(@RegionID, sp.RegionID),   '') 
AND ISNULL(sp.[GradeID],  '')         = ISNULL(ISNULL(@GradeID, sp.GradeID),      '')
AND ISNULL(sr.[ServiceTypeID],'') = ISNULL(ISNULL(@ServiceTypeID, sr.ServiceTypeID),'')


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo._ItinerarySet_Sel_ByID...';


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


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo._License_GetLatest...';


GO
ALTER PROCEDURE [dbo].[_License_GetLatest]

AS
DECLARE @LicenseID int
	
	SET @LicenseID = (SELECT MAX(LicenseID) FROM License)

	EXEC License_Sel_ByID @LicenseID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo._Menu_GetChildListAndParentTree...';


GO
ALTER PROCEDURE [dbo].[_Menu_GetChildListAndParentTree]
@ParentFolderID INT, @TableName VARCHAR (200)
AS
SET NOCOUNT ON

-- temporary tables
CREATE TABLE #ChildCollection  
(	[ID] 		 int, 
	[FolderName] 	 varchar (200), 
	[ParentFolderID] int,
	[IsRecordActive] bit,
	[Type] 		 varchar (200)
)
CREATE TABLE #ResultCollection  
(	[ID] 		 int, 
	[FolderName] 	 varchar (200), 
	[ParentFolderID] int,
	[IsRecordActive] bit,
	[Type] 		 varchar (200)
)

DECLARE @FolderID int
WHILE @ParentFolderID IS NOT NULL
BEGIN
	-- Get the child collection
	IF @ParentFolderID > 0 BEGIN	

		INSERT #ChildCollection 
			EXEC _Menu_GetChildList @ParentFolderID,  @TableName

		-- move to next higher level
		SET @FolderID = (SELECT TOP 1 [ParentFolderID] FROM #ChildCollection)
		SET @ParentFolderID = 
		(
			SELECT ParentFolderID FROM Folder
			WHERE FolderID = @FolderID
		)
	END	
	-- Get the single top level folder
	ELSE BEGIN

		INSERT #ChildCollection			
			SELECT  FolderID       	AS [ID],
				FolderName     	AS [Name],
				ParentFolderID 	AS [ParentFolderID],
				IsRecordActive 	AS [IsActive],
				'Folder'     	AS [Type]
			FROM Folder 
			WHERE FolderID = @FolderID

		SET @ParentFolderID = NULL
	END

	-- shuffle results so order is top to bottom
	INSERT #ChildCollection SELECT * FROM #ResultCollection
	DELETE FROM #ResultCollection
	INSERT #ResultCollection SELECT * FROM #ChildCollection	
	DELETE FROM #ChildCollection
END

-- return results
SELECT * FROM #ResultCollection

-- cleanup
DROP TABLE #ChildCollection
DROP TABLE #ResultCollection


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo._Menu_SetParentFolderID...';


GO
ALTER PROCEDURE [dbo].[_Menu_SetParentFolderID]
@ItemID INT, @ParentFolderID INT, @TableName VARCHAR (100)
AS
DECLARE @cmd varchar(1000)

SET @cmd = 
	'UPDATE ' + @TableName +
	' SET ParentFolderID = ' + STR(@ParentFolderID) +
	' WHERE ' + @TableName + 'ID = ' + STR(@ItemID)

EXEC(@cmd)


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo._Report_ClientLocations...';


GO
ALTER PROCEDURE [dbo].[_Report_ClientLocations]
@StartDate DATETIME, @EndDate DATETIME, @ItineraryStatusList VARCHAR (200), @RequestStatusList VARCHAR (200)
AS
SET NOCOUNT ON

-- Handle empty lists so IN clause won't throw error
IF @ItineraryStatusList  = ''  SELECT @ItineraryStatusList  = ''''''
IF @RequestStatusList = ''  SELECT @RequestStatusList = ''''''

DECLARE @sql varchar(2000)
SET @sql = '
	SELECT item.StartDate, ISNULL(city.CityName,''''), itin.ItineraryName, sup.SupplierName,
	ISNULL(status.RequestStatusName, '''') AS RequestStatus
	FROM PurchaseItem item 
		LEFT OUTER JOIN PurchaseLine line 
			ON line.PurchaseLineID = item.PurchaseLineID 
		LEFT OUTER JOIN Itinerary itin 
			ON itin.ItineraryID = line.ItineraryID  
		LEFT OUTER JOIN Supplier sup 
			ON sup.SupplierID = line.SupplierID   
		LEFT OUTER JOIN City city 
			ON city.CityID = sup.CityID
		LEFT OUTER JOIN RequestStatus status
			ON item.RequestStatusID = status.RequestStatusID

	WHERE item.StartDate >= ''' + CONVERT(char(8), @StartDate, 112) + '''
	AND item.StartDate < ''' + CONVERT(char(8), @EndDate+1, 112) + '''
	AND ( itin.ItineraryStatusID IN (' + @ItineraryStatusList  + ')'

IF(@ItineraryStatusList LIKE '%NULL%')
SET @sql = @sql + '
		OR itin.ItineraryStatusID IS NULL'

SET @sql = @sql + ' )
	AND ( item.RequestStatusID IN (' + @RequestStatusList + ')'

IF(@RequestStatusList LIKE '%NULL%')
SET @sql = @sql + '
		OR item.RequestStatusID IS NULL'

SET @sql = @sql + ' )
	ORDER BY item.StartDate, city.CityName'

--PRINT @sql
EXEC(@sql)


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo._Report_Myob...';


GO
ALTER PROCEDURE [dbo].[_Report_Myob]
@StartDate DATETIME, @EndDate DATETIME
AS
SET NOCOUNT ON

/*
declare	@StartDate datetime
declare	@EndDate  datetime
declare	@ItineraryStatusList  varchar(200)
declare	@RequestStatusList varchar(200)
set @StartDate='20050201'
set @EndDate='20050228'
set @ItineraryStatusList='1,2,3,4,5,6'
set @RequestStatusList='1,2,3,4,5,6'
*/

-- Handle empty lists so IN clause won't throw error
-- @ItineraryStatusList  = ''  SELECT @ItineraryStatusList  = ''''''
-- @RequestStatusList = ''  SELECT @RequestStatusList = ''''''


DECLARE @sql varchar(2000)
SET @sql = '
	SELECT
		sup.SupplierName, 
		'''' AS '' '', 
		line.PurchaseLineID,
		item.StartDate AS ItemDate, 
		'''' AS '' '',  
		'''' AS '' '',  
		'''' AS '' '',  
		'''' AS '' '', 
		ISNULL(item.PurchaseItemName, ''''),
		item.ServiceTypeID, 
		item.Net  * ISNULL(NumberOfDays, 1) * ISNULL(Quantity, 1) AS TotalNet
	FROM PurchaseItem item 
		LEFT OUTER JOIN PurchaseLine line 
			ON line.PurchaseLineID = item.PurchaseLineID 
		LEFT OUTER JOIN Itinerary itin 
			ON itin.ItineraryID = line.ItineraryID 
		LEFT OUTER JOIN Supplier sup 
			ON sup.SupplierID = line.SupplierID   
		LEFT OUTER JOIN City city 
			ON city.CityID = sup.CityID 
		LEFT OUTER JOIN RequestStatus status
			ON item.RequestStatusID = status.RequestStatusID	
	WHERE item.StartDate >= ''' + CONVERT(char(8), @StartDate, 112) + '''
	AND item.StartDate < ''' + CONVERT(char(8), @EndDate+1, 112) + '''

ORDER BY sup.SupplierName, item.StartDate'/*

	AND ( itin.ItineraryStatusID IN (' + @ItineraryStatusList  + ')'

IF(@ItineraryStatusList LIKE '%NULL%')
SET @sql = @sql + '
		OR itin.ItineraryStatusID IS NULL'

SET @sql = @sql + ' )
	AND ( item.RequestStatusID IN (' + @RequestStatusList + ')'

IF(@RequestStatusList LIKE '%NULL%')
SET @sql = @sql + '
		OR item.RequestStatusID IS NULL'

SET @sql = @sql + ' )
	ORDER BY sup.SupplierName, item.StartDate'
*/
--PRINT @sql
EXEC(@sql)


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo._Report_SuppilerPurchases...';


GO
ALTER PROCEDURE [dbo].[_Report_SuppilerPurchases]
@StartDate DATETIME, @EndDate DATETIME, @ItineraryStatusList VARCHAR (200), @RequestStatusList VARCHAR (200)
AS
SET NOCOUNT ON


-- Handle empty lists so IN clause won't throw error
IF @ItineraryStatusList  = ''  SELECT @ItineraryStatusList  = ''''''
IF @RequestStatusList = ''  SELECT @RequestStatusList = ''''''


DECLARE @sql varchar(2000)
SET @sql = '
	SELECT
		sup.SupplierName, 
		item.StartDate, 
		itin.ItineraryName, 
		ISNULL(item.PurchaseItemName, ''''), 
		ISNULL(status.RequestStatusName, '''') AS RequestStatus,
		item.Gross * ISNULL(NumberOfDays, 1) * ISNULL(Quantity, 1) AS TotalGross,
		item.Net   * ISNULL(NumberOfDays, 1) * ISNULL(Quantity, 1) AS TotalNet
	FROM PurchaseItem item 
		LEFT OUTER JOIN PurchaseLine line 
			ON line.PurchaseLineID = item.PurchaseLineID 
		LEFT OUTER JOIN Itinerary itin 
			ON itin.ItineraryID = line.ItineraryID 
		LEFT OUTER JOIN Supplier sup 
			ON sup.SupplierID = line.SupplierID   
		LEFT OUTER JOIN City city 
			ON city.CityID = sup.CityID 
		LEFT OUTER JOIN RequestStatus status
			ON item.RequestStatusID = status.RequestStatusID
	
	WHERE item.StartDate >= ''' + CONVERT(char(8), @StartDate, 112) + '''
	AND item.StartDate < ''' + CONVERT(char(8), @EndDate+1, 112) + '''
	AND ( itin.ItineraryStatusID IN (' + @ItineraryStatusList  + ')'

IF(@ItineraryStatusList LIKE '%NULL%')
SET @sql = @sql + '
		OR itin.ItineraryStatusID IS NULL'

SET @sql = @sql + ' )
	AND ( item.RequestStatusID IN (' + @RequestStatusList + ')'

IF(@RequestStatusList LIKE '%NULL%')
SET @sql = @sql + '
		OR item.RequestStatusID IS NULL'

SET @sql = @sql + ' )
	ORDER BY sup.SupplierName, item.StartDate'

--PRINT @sql
EXEC(@sql)


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo._Report_WhoUsedSupplier...';


GO
ALTER PROCEDURE [dbo].[_Report_WhoUsedSupplier]
@SupplierID INT, @StartDate DATETIME, @EndDate DATETIME
AS
SET NOCOUNT ON

SELECT srv.ServiceName, item.StartDate, itin.ItineraryName, line.PurchaseLineName, 
    ISNULL( item.PurchaseItemName,'')

FROM PurchaseItem item 
    LEFT OUTER JOIN PurchaseLine line 
        ON line.PurchaseLineID = item.PurchaseLineID 
    LEFT OUTER JOIN Itinerary itin 
        ON itin.ItineraryID = line.ItineraryID     
    LEFT OUTER JOIN [Option] opt 
        ON item.OptionID = opt.OptionID 
    LEFT OUTER JOIN Rate rate 
        ON rate.RateID = opt.RateID
    LEFT OUTER JOIN Service srv 
        ON srv.ServiceID = rate.ServiceID 
WHERE 
(
    line.SupplierID = @SupplierID
    OR item.OptionID IN
    (
        SELECT OptionID 
        FROM [Option] opt  
            LEFT OUTER JOIN Rate rate
                ON rate.RateID = opt.RateID
            LEFT OUTER JOIN Service srv 
                ON srv.ServiceID = rate.ServiceID
        WHERE SupplierID = @SupplierID
    )
)
AND item.StartDate >= CAST(CONVERT(char(8), @StartDate, 112) AS datetime)
AND item.StartDate <   CAST(CONVERT(char(8), @EndDate+1,  112) AS datetime)
ORDER BY srv.ServiceName, item.StartDate DESC


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo._Supplier_New...';


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
	[AddedBy]
)
VALUES
(
	@SupplierName,
	1,
	@ParentFolderID,
	@AddedOn,
	@AddedBy
)

SELECT SCOPE_IDENTITY()


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo._Supplier_Rename...';


GO
ALTER PROCEDURE [dbo].[_Supplier_Rename]
@SupplierID INT, @SupplierName VARCHAR (100)
AS
SET NOCOUNT ON

UPDATE Supplier
SET SupplierName = @SupplierName
WHERE SupplierID = @SupplierID


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
	[AddedOn],
	[AddedBy]
)	
SELECT
	@NewSupplierID,
	[SupplierConfigTypeID],
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
		[AddedOn],
		[AddedBy]
	)
	SELECT
		@NewServiceID,
		[ServiceConfigTypeID],
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
PRINT N'Altering dbo._ToolSet_Sel_All...';


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
EXEC AccountingCategory_Sel_All
EXEC ContactCategory_Sel_All
EXEC OptionType_Sel_All
EXEC Flag_Sel_All
EXEC TemplateCategory_Sel_All
EXEC Template_Sel_All


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo._User_CheckLogin...';


GO
ALTER PROCEDURE [dbo].[_User_CheckLogin]
@LoginGuid UNIQUEIDENTIFIER, @UserID INT, @ComputerName VARCHAR (200), @MaxUsers INT, @NewGuid UNIQUEIDENTIFIER OUTPUT
AS
DECLARE @LogoutMinutes int
SET @LogoutMinutes = -20 --(MUST BE A NEGATIVE NUMBER)


-- Existing login ---------------------------------------------

UPDATE Login
	SET LastActiveDate = GETDATE()
	WHERE LoginID = @LoginGuid

-- New login ------------------------------------------------

IF @@ROWCOUNT = 0  -- no row updated
BEGIN
	-- Count current logins
	DECLARE @UserCount int
	SET @UserCount = (SELECT COUNT(*) FROM Login)
	
	-- Compare count to maximum allowed logins
	IF @UserCount >= @MaxUsers
	BEGIN	
		-- Delete one if older then @LogoutMinutes
		DELETE Login 
		WHERE LastActiveDate = (SELECT MIN(LastActiveDate) FROM Login)
		AND LastActiveDate < DATEADD(minute, @LogoutMinutes, GETDATE())
		
		-- Recheck user count
		SET @UserCount = (SELECT COUNT(*) FROM Login)
	END
	
	-- Login this user if there is room
	IF @UserCount < @MaxUsers
	BEGIN
		
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
			GETDATE(),
			GETDATE(),
			@ComputerName
		)
	END
	
	ELSE -- no logins available
		SET @LoginGuid = NULL
END

-- return login id
SELECT @LoginGuid


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo._User_RemoveLogin...';


GO
ALTER PROCEDURE [dbo].[_User_RemoveLogin]
@LoginGuid UNIQUEIDENTIFIER
AS
DELETE Login WHERE LoginID = @LoginGuid


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo._User_UpdateLogin...';


GO
ALTER PROCEDURE [dbo].[_User_UpdateLogin]
@LoginGuid UNIQUEIDENTIFIER
AS
UPDATE Login 
	SET LastActiveDate = GETDATE()
	WHERE LoginID = @LoginGuid


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo._UserSet_Sel_ByUsernamePassword...';


GO
ALTER PROCEDURE [dbo].[_UserSet_Sel_ByUsernamePassword]
@Username VARCHAR (255), @Password VARCHAR (255)
AS
DECLARE @UserID int

SET @UserID = (
SELECT [UserID] FROM [User] 
	WHERE [Username] COLLATE Latin1_General_CS_AS = @Username
	AND [Password]   COLLATE Latin1_General_CS_AS = @Password
)
IF(@UserID != '')
	EXEC dbo._UserSet_Sel_ByID @UserID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Option_Ins...';


GO
ALTER PROCEDURE [dbo].[Option_Ins]
@OptionName VARCHAR (150), @OptionTypeID INT, @RateID INT, @Net MONEY, @Gross MONEY, @PricingOption CHAR (2), @IsDefault BIT, @IsRecordActive BIT, @AddedOn DATETIME, @AddedBy INT, @IsDeleted BIT, @OptionID INT OUTPUT
AS
INSERT [dbo].[Option]
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
	[AddedBy],
	[IsDeleted]
)
VALUES
(
	@OptionName,
	@OptionTypeID,
	@RateID,
	@Net,
	@Gross,
	@PricingOption,
	@IsDefault,
	@IsRecordActive,
	@AddedOn,
	@AddedBy,
	@IsDeleted
)
SELECT @OptionID=SCOPE_IDENTITY()


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Option_Sel_All...';


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
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Option_Sel_ByID...';


GO
ALTER PROCEDURE [dbo].[Option_Sel_ByID]
@OptionID INT
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
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Option_Sel_ByRateID...';


GO
ALTER PROCEDURE [dbo].[Option_Sel_ByRateID]
@RateID INT
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
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Option_Upd...';


GO
ALTER PROCEDURE [dbo].[Option_Upd]
@OptionID INT, @OptionName VARCHAR (150), @OptionTypeID INT, @RateID INT, @Net MONEY, @Gross MONEY, @PricingOption CHAR (2), @IsDefault BIT, @IsRecordActive BIT, @AddedOn DATETIME, @AddedBy INT, @RowVersion TIMESTAMP, @IsDeleted BIT
AS
UPDATE [dbo].[Option]
SET 
	[OptionName] = @OptionName,
	[OptionTypeID] = @OptionTypeID,
	[RateID] = @RateID,
	[Net] = @Net,
	[Gross] = @Gross,
	[PricingOption] = @PricingOption,
	[IsDefault] = @IsDefault,
	[IsRecordActive] = @IsRecordActive,
	[AddedOn] = @AddedOn,
	[AddedBy] = @AddedBy,
	[IsDeleted] = @IsDeleted
WHERE
	[OptionID] = @OptionID
	AND [RowVersion] = @RowVersion


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Service_Ins...';


GO
ALTER PROCEDURE [dbo].[Service_Ins]
@ServiceName VARCHAR (150), @SupplierID INT, @ServiceTypeID INT, @Description VARCHAR (2000), @Comments VARCHAR (2000), @Warning VARCHAR (2000), @MaxPax INT, @CheckinTime DATETIME, @CheckoutTime DATETIME, @CheckinMinutesEarly INT, @IsRecordActive BIT, @CurrencyCode CHAR (3), @ChargeType VARCHAR (10), @PaymentTermID INT, @TaxTypeID INT, @AddedOn DATETIME, @AddedBy INT, @IsDeleted BIT, @ServiceID INT OUTPUT
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
@ServiceID INT, @ServiceName VARCHAR (150), @SupplierID INT, @ServiceTypeID INT, @Description VARCHAR (2000), @Comments VARCHAR (2000), @Warning VARCHAR (2000), @MaxPax INT, @CheckinTime DATETIME, @CheckoutTime DATETIME, @CheckinMinutesEarly INT, @IsRecordActive BIT, @CurrencyCode CHAR (3), @ChargeType VARCHAR (10), @PaymentTermID INT, @TaxTypeID INT, @AddedOn DATETIME, @AddedBy INT, @RowVersion TIMESTAMP, @IsDeleted BIT
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
PRINT N'Altering dbo.ServiceType_Ins...';


GO
ALTER PROCEDURE [dbo].[ServiceType_Ins]
@ServiceTypeName VARCHAR (100), @BookingStartName VARCHAR (50), @BookingEndName VARCHAR (50), @NumberOfDaysName VARCHAR (50), @NetAccountingCategoryID INT, @NetTaxTypeID INT, @GrossAccountingCategoryID INT, @GrossTaxTypeID INT, @IsDeleted BIT, @IsAdditionalMarkupContainer BIT, @ServiceTypeID INT OUTPUT
AS
INSERT [dbo].[ServiceType]
(
	[ServiceTypeName],
	[BookingStartName],
	[BookingEndName],
	[NumberOfDaysName],
	[NetAccountingCategoryID],
	[NetTaxTypeID],
	[GrossAccountingCategoryID],
	[GrossTaxTypeID],
	[IsDeleted],
	[IsAdditionalMarkupContainer]
)
VALUES
(
	ISNULL(@ServiceTypeName, ('')),
	@BookingStartName,
	@BookingEndName,
	@NumberOfDaysName,
	@NetAccountingCategoryID,
	@NetTaxTypeID,
	@GrossAccountingCategoryID,
	@GrossTaxTypeID,
	@IsDeleted,
	@IsAdditionalMarkupContainer
)
SELECT @ServiceTypeID=SCOPE_IDENTITY()


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.ServiceType_Sel_All...';


GO
ALTER PROCEDURE [dbo].[ServiceType_Sel_All]

AS
SET NOCOUNT ON
SELECT
	[ServiceTypeID],
	[ServiceTypeName],
	[BookingStartName],
	[BookingEndName],
	[NumberOfDaysName],
	[NetAccountingCategoryID],
	[NetTaxTypeID],
	[GrossAccountingCategoryID],
	[GrossTaxTypeID],
	[IsDeleted],
	[IsAdditionalMarkupContainer]
FROM [dbo].[ServiceType]
ORDER BY 
	[ServiceTypeID] ASC


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.ServiceType_Sel_ByID...';


GO
ALTER PROCEDURE [dbo].[ServiceType_Sel_ByID]
@ServiceTypeID INT
AS
SET NOCOUNT ON
SELECT
	[ServiceTypeID],
	[ServiceTypeName],
	[BookingStartName],
	[BookingEndName],
	[NumberOfDaysName],
	[NetAccountingCategoryID],
	[NetTaxTypeID],
	[GrossAccountingCategoryID],
	[GrossTaxTypeID],
	[IsDeleted],
	[IsAdditionalMarkupContainer]
FROM [dbo].[ServiceType]
WHERE
	[ServiceTypeID] = @ServiceTypeID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.ServiceType_Upd...';


GO
ALTER PROCEDURE [dbo].[ServiceType_Upd]
@ServiceTypeID INT, @ServiceTypeName VARCHAR (100), @BookingStartName VARCHAR (50), @BookingEndName VARCHAR (50), @NumberOfDaysName VARCHAR (50), @NetAccountingCategoryID INT, @NetTaxTypeID INT, @GrossAccountingCategoryID INT, @GrossTaxTypeID INT, @IsDeleted BIT, @IsAdditionalMarkupContainer BIT
AS
UPDATE [dbo].[ServiceType]
SET 
	[ServiceTypeName] = @ServiceTypeName,
	[BookingStartName] = @BookingStartName,
	[BookingEndName] = @BookingEndName,
	[NumberOfDaysName] = @NumberOfDaysName,
	[NetAccountingCategoryID] = @NetAccountingCategoryID,
	[NetTaxTypeID] = @NetTaxTypeID,
	[GrossAccountingCategoryID] = @GrossAccountingCategoryID,
	[GrossTaxTypeID] = @GrossTaxTypeID,
	[IsDeleted] = @IsDeleted,
	[IsAdditionalMarkupContainer] = @IsAdditionalMarkupContainer
WHERE
	[ServiceTypeID] = @ServiceTypeID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Template_Ins...';


GO
ALTER PROCEDURE [dbo].[Template_Ins]
@TemplateName VARCHAR (100), @FilePath VARCHAR (255), @ParentTemplateCategoryID INT, @TemplateID INT OUTPUT
AS
INSERT [dbo].[Template]
(
	[TemplateName],
	[FilePath],
	[ParentTemplateCategoryID]
)
VALUES
(
	@TemplateName,
	@FilePath,
	@ParentTemplateCategoryID
)
SELECT @TemplateID=SCOPE_IDENTITY()


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Template_Sel_All...';


GO
ALTER PROCEDURE [dbo].[Template_Sel_All]

AS
SET NOCOUNT ON
SELECT
	[TemplateID],
	[TemplateName],
	[FilePath],
	[ParentTemplateCategoryID]
FROM [dbo].[Template]
ORDER BY 
	[TemplateID] ASC


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Template_Sel_ByID...';


GO
ALTER PROCEDURE [dbo].[Template_Sel_ByID]
@TemplateID INT
AS
SET NOCOUNT ON
SELECT
	[TemplateID],
	[TemplateName],
	[FilePath],
	[ParentTemplateCategoryID]
FROM [dbo].[Template]
WHERE
	[TemplateID] = @TemplateID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.Template_Upd...';


GO
ALTER PROCEDURE [dbo].[Template_Upd]
@TemplateID INT, @TemplateName VARCHAR (100), @FilePath VARCHAR (255), @ParentTemplateCategoryID INT
AS
UPDATE [dbo].[Template]
SET 
	[TemplateName] = @TemplateName,
	[FilePath] = @FilePath,
	[ParentTemplateCategoryID] = @ParentTemplateCategoryID
WHERE
	[TemplateID] = @TemplateID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.Flag_Del...';


GO
CREATE PROCEDURE [dbo].[Flag_Del]
@FlagID INT
AS
DELETE FROM [dbo].[Flag]
WHERE
	[FlagID] = @FlagID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.Flag_Ins...';


GO
CREATE PROCEDURE [dbo].[Flag_Ins]
@FlagImage IMAGE, @FlagID INT OUTPUT
AS
INSERT [dbo].[Flag]
(
	[FlagImage]
)
VALUES
(
	@FlagImage
)
SELECT @FlagID=SCOPE_IDENTITY()


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.Flag_Sel_All...';


GO
CREATE PROCEDURE [dbo].[Flag_Sel_All]

AS
SET NOCOUNT ON
SELECT
	[FlagID],
	[FlagImage]
FROM [dbo].[Flag]
ORDER BY 
	[FlagID] ASC


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.Flag_Sel_ByID...';


GO
CREATE PROCEDURE [dbo].[Flag_Sel_ByID]
@FlagID INT
AS
SET NOCOUNT ON
SELECT
	[FlagID],
	[FlagImage]
FROM [dbo].[Flag]
WHERE
	[FlagID] = @FlagID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.Flag_Upd...';


GO
CREATE PROCEDURE [dbo].[Flag_Upd]
@FlagID INT, @FlagImage IMAGE
AS
UPDATE [dbo].[Flag]
SET 
	[FlagImage] = @FlagImage
WHERE
	[FlagID] = @FlagID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.ItineraryPax_Del...';


GO
CREATE PROCEDURE [dbo].[ItineraryPax_Del]
@ItineraryPaxID INT
AS
DELETE FROM [dbo].[ItineraryPax]
WHERE
	[ItineraryPaxID] = @ItineraryPaxID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.ItineraryPax_Del_ByItineraryID...';


GO
CREATE PROCEDURE [dbo].[ItineraryPax_Del_ByItineraryID]
@ItineraryID INT
AS
DELETE
FROM [dbo].[ItineraryPax]
WHERE
	[ItineraryID] = @ItineraryID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.ItineraryPax_Ins...';


GO
CREATE PROCEDURE [dbo].[ItineraryPax_Ins]
@ItineraryPaxName VARCHAR (50), @ItineraryID INT, @MemberCount INT, @MemberRooms INT, @StaffCount INT, @StaffRooms INT, @ItineraryPaxID INT OUTPUT
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
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.ItineraryPax_Sel_All...';


GO
CREATE PROCEDURE [dbo].[ItineraryPax_Sel_All]

AS
SET NOCOUNT ON
SELECT
	[ItineraryPaxID],
	[ItineraryPaxName],
	[ItineraryID],
	[MemberCount],
	[MemberRooms],
	[StaffCount],
	[StaffRooms]
FROM [dbo].[ItineraryPax]
ORDER BY 
	[ItineraryPaxID] ASC


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.ItineraryPax_Sel_ByID...';


GO
CREATE PROCEDURE [dbo].[ItineraryPax_Sel_ByID]
@ItineraryPaxID INT
AS
SET NOCOUNT ON
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
	[ItineraryPaxID] = @ItineraryPaxID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.ItineraryPax_Sel_ByItineraryID...';


GO
CREATE PROCEDURE [dbo].[ItineraryPax_Sel_ByItineraryID]
@ItineraryID INT
AS
SET NOCOUNT ON
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


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.ItineraryPax_Upd...';


GO
CREATE PROCEDURE [dbo].[ItineraryPax_Upd]
@ItineraryPaxID INT, @ItineraryPaxName VARCHAR (50), @ItineraryID INT, @MemberCount INT, @MemberRooms INT, @StaffCount INT, @StaffRooms INT
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
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.ItineraryPax_Upd_ByItineraryID...';


GO
CREATE PROCEDURE [dbo].[ItineraryPax_Upd_ByItineraryID]
@ItineraryID INT, @ItineraryIDOld INT
AS
UPDATE [dbo].[ItineraryPax]
SET
	[ItineraryID] = @ItineraryID
WHERE
	[ItineraryID] = @ItineraryIDOld


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.OptionType_Del...';


GO
CREATE PROCEDURE [dbo].[OptionType_Del]
@OptionTypeID INT
AS
DELETE FROM [dbo].[OptionType]
WHERE
	[OptionTypeID] = @OptionTypeID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.OptionType_Ins...';


GO
CREATE PROCEDURE [dbo].[OptionType_Ins]
@OptionTypeName VARCHAR (10), @Divisor INT, @OptionTypeID INT OUTPUT
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
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.OptionType_Sel_All...';


GO
CREATE PROCEDURE [dbo].[OptionType_Sel_All]

AS
SET NOCOUNT ON
SELECT
	[OptionTypeID],
	[OptionTypeName],
	[Divisor]
FROM [dbo].[OptionType]
ORDER BY 
	[OptionTypeID] ASC


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.OptionType_Sel_ByID...';


GO
CREATE PROCEDURE [dbo].[OptionType_Sel_ByID]
@OptionTypeID INT
AS
SET NOCOUNT ON
SELECT
	[OptionTypeID],
	[OptionTypeName],
	[Divisor]
FROM [dbo].[OptionType]
WHERE
	[OptionTypeID] = @OptionTypeID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.OptionType_Upd...';


GO
CREATE PROCEDURE [dbo].[OptionType_Upd]
@OptionTypeID INT, @OptionTypeName VARCHAR (10), @Divisor INT
AS
UPDATE [dbo].[OptionType]
SET 
	[OptionTypeName] = @OptionTypeName,
	[Divisor] = @Divisor
WHERE
	[OptionTypeID] = @OptionTypeID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.PurchaseItemNote_Del...';


GO
CREATE PROCEDURE [dbo].[PurchaseItemNote_Del]
@PurchaseItemNoteID INT
AS
DELETE FROM [dbo].[PurchaseItemNote]
WHERE
	[PurchaseItemNoteID] = @PurchaseItemNoteID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.PurchaseItemNote_Ins...';


GO
CREATE PROCEDURE [dbo].[PurchaseItemNote_Ins]
@PurchaseItemID INT, @FlagID INT, @Note VARCHAR (255), @PurchaseItemNoteID INT OUTPUT
AS
INSERT [dbo].[PurchaseItemNote]
(
	[PurchaseItemID],
	[FlagID],
	[Note]
)
VALUES
(
	@PurchaseItemID,
	@FlagID,
	@Note
)
SELECT @PurchaseItemNoteID=SCOPE_IDENTITY()


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.PurchaseItemNote_Sel_All...';


GO
CREATE PROCEDURE [dbo].[PurchaseItemNote_Sel_All]

AS
SET NOCOUNT ON
SELECT
	[PurchaseItemNoteID],
	[PurchaseItemID],
	[FlagID],
	[Note]
FROM [dbo].[PurchaseItemNote]
ORDER BY 
	[PurchaseItemNoteID] ASC


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.PurchaseItemNote_Sel_ByID...';


GO
CREATE PROCEDURE [dbo].[PurchaseItemNote_Sel_ByID]
@PurchaseItemNoteID INT
AS
SET NOCOUNT ON
SELECT
	[PurchaseItemNoteID],
	[PurchaseItemID],
	[FlagID],
	[Note]
FROM [dbo].[PurchaseItemNote]
WHERE
	[PurchaseItemNoteID] = @PurchaseItemNoteID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.PurchaseItemNote_Upd...';


GO
CREATE PROCEDURE [dbo].[PurchaseItemNote_Upd]
@PurchaseItemNoteID INT, @PurchaseItemID INT, @FlagID INT, @Note VARCHAR (255)
AS
UPDATE [dbo].[PurchaseItemNote]
SET 
	[PurchaseItemID] = @PurchaseItemID,
	[FlagID] = @FlagID,
	[Note] = @Note
WHERE
	[PurchaseItemNoteID] = @PurchaseItemNoteID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.ServiceChargeType_Del...';


GO
CREATE PROCEDURE [dbo].[ServiceChargeType_Del]
@ServiceChargeTypeID INT
AS
DELETE FROM [dbo].[ServiceChargeType]
WHERE
	[ServiceChargeTypeID] = @ServiceChargeTypeID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.ServiceChargeType_Ins...';


GO
CREATE PROCEDURE [dbo].[ServiceChargeType_Ins]
@ServiceChargeTypeName VARCHAR (50), @ServiceChargeTypeID INT OUTPUT
AS
INSERT [dbo].[ServiceChargeType]
(
	[ServiceChargeTypeName]
)
VALUES
(
	@ServiceChargeTypeName
)
SELECT @ServiceChargeTypeID=SCOPE_IDENTITY()


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.ServiceChargeType_Sel_All...';


GO
CREATE PROCEDURE [dbo].[ServiceChargeType_Sel_All]

AS
SET NOCOUNT ON
SELECT
	[ServiceChargeTypeID],
	[ServiceChargeTypeName]
FROM [dbo].[ServiceChargeType]
ORDER BY 
	[ServiceChargeTypeID] ASC


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.ServiceChargeType_Sel_ByID...';


GO
CREATE PROCEDURE [dbo].[ServiceChargeType_Sel_ByID]
@ServiceChargeTypeID INT
AS
SET NOCOUNT ON
SELECT
	[ServiceChargeTypeID],
	[ServiceChargeTypeName]
FROM [dbo].[ServiceChargeType]
WHERE
	[ServiceChargeTypeID] = @ServiceChargeTypeID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.ServiceChargeType_Upd...';


GO
CREATE PROCEDURE [dbo].[ServiceChargeType_Upd]
@ServiceChargeTypeID INT, @ServiceChargeTypeName VARCHAR (50)
AS
UPDATE [dbo].[ServiceChargeType]
SET 
	[ServiceChargeTypeName] = @ServiceChargeTypeName
WHERE
	[ServiceChargeTypeID] = @ServiceChargeTypeID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.ServiceFoc_Del...';


GO
CREATE PROCEDURE [dbo].[ServiceFoc_Del]
@ServiceFocID INT
AS
DELETE FROM [dbo].[ServiceFoc]
WHERE
	[ServiceFocID] = @ServiceFocID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.ServiceFoc_Ins...';


GO
CREATE PROCEDURE [dbo].[ServiceFoc_Ins]
@ServiceID INT, @UnitsUsed INT, @UnitsFree INT, @ServiceFocID INT OUTPUT
AS
INSERT [dbo].[ServiceFoc]
(
	[ServiceID],
	[UnitsUsed],
	[UnitsFree]
)
VALUES
(
	@ServiceID,
	@UnitsUsed,
	@UnitsFree
)
SELECT @ServiceFocID=SCOPE_IDENTITY()


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.ServiceFoc_Sel_All...';


GO
CREATE PROCEDURE [dbo].[ServiceFoc_Sel_All]

AS
SET NOCOUNT ON
SELECT
	[ServiceFocID],
	[ServiceID],
	[UnitsUsed],
	[UnitsFree]
FROM [dbo].[ServiceFoc]
ORDER BY 
	[ServiceFocID] ASC


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.ServiceFoc_Sel_ByID...';


GO
CREATE PROCEDURE [dbo].[ServiceFoc_Sel_ByID]
@ServiceFocID INT
AS
SET NOCOUNT ON
SELECT
	[ServiceFocID],
	[ServiceID],
	[UnitsUsed],
	[UnitsFree]
FROM [dbo].[ServiceFoc]
WHERE
	[ServiceFocID] = @ServiceFocID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.ServiceFoc_Upd...';


GO
CREATE PROCEDURE [dbo].[ServiceFoc_Upd]
@ServiceFocID INT, @ServiceID INT, @UnitsUsed INT, @UnitsFree INT
AS
UPDATE [dbo].[ServiceFoc]
SET 
	[ServiceID] = @ServiceID,
	[UnitsUsed] = @UnitsUsed,
	[UnitsFree] = @UnitsFree
WHERE
	[ServiceFocID] = @ServiceFocID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.TemplateCategory_Del...';


GO
CREATE PROCEDURE [dbo].[TemplateCategory_Del]
@TemplateCategoryID INT
AS
DELETE FROM [dbo].[TemplateCategory]
WHERE
	[TemplateCategoryID] = @TemplateCategoryID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.TemplateCategory_Ins...';


GO
CREATE PROCEDURE [dbo].[TemplateCategory_Ins]
@TemplateCategoryName VARCHAR (100), @ParentTemplateCategoryID INT, @TemplateCategoryID INT OUTPUT
AS
INSERT [dbo].[TemplateCategory]
(
	[TemplateCategoryName],
	[ParentTemplateCategoryID]
)
VALUES
(
	@TemplateCategoryName,
	@ParentTemplateCategoryID
)
SELECT @TemplateCategoryID=SCOPE_IDENTITY()


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.TemplateCategory_Sel_All...';


GO
CREATE PROCEDURE [dbo].[TemplateCategory_Sel_All]

AS
SET NOCOUNT ON
SELECT
	[TemplateCategoryID],
	[TemplateCategoryName],
	[ParentTemplateCategoryID]
FROM [dbo].[TemplateCategory]
ORDER BY 
	[TemplateCategoryID] ASC


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.TemplateCategory_Sel_ByID...';


GO
CREATE PROCEDURE [dbo].[TemplateCategory_Sel_ByID]
@TemplateCategoryID INT
AS
SET NOCOUNT ON
SELECT
	[TemplateCategoryID],
	[TemplateCategoryName],
	[ParentTemplateCategoryID]
FROM [dbo].[TemplateCategory]
WHERE
	[TemplateCategoryID] = @TemplateCategoryID


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.TemplateCategory_Upd...';


GO
CREATE PROCEDURE [dbo].[TemplateCategory_Upd]
@TemplateCategoryID INT, @TemplateCategoryName VARCHAR (100), @ParentTemplateCategoryID INT
AS
UPDATE [dbo].[TemplateCategory]
SET 
	[TemplateCategoryName] = @TemplateCategoryName,
	[ParentTemplateCategoryID] = @ParentTemplateCategoryID
WHERE
	[TemplateCategoryID] = @TemplateCategoryID


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
		Net * UnitMultiplier * CurrencyRate  as TotalNet,
		Gross * UnitMultiplier * CurrencyRate as TotalGross,
		case when Net = 0 then 0 else (Gross - Net)/Net*100 end as Markup,
		case when Gross = 0 then 0 else (Gross - Net)/Gross*100 end as Commission
	
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
		Gross - Net as Yield
	from
	(
		select i.ItineraryID, isnull(ItemCount,0) as ItemCount, Net, 
		case
			when (GrossOverride is not null) then GrossOverride
			when (GrossMarkup   is not null) then isnull(Gross,0) * (1+GrossMarkup/100)
			else Gross
		end as Gross
		from Itinerary i
		left outer join
		(
			select ItineraryID, count(*) as ItemCount, sum(TotalNet) as Net, sum(TotalGross) as Gross
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
PRINT N'Altering dbo.PurchaseItemPayments...';


GO
ALTER FUNCTION [dbo].[PurchaseItemPayments]
( )
RETURNS 
    @result TABLE (
        [PurchaseItemID] INT          NULL,
        [PaymentDueDate] DATETIME     NULL,
        [PaymentDueName] VARCHAR (20) NULL,
        [PaymentAmount]  MONEY        NULL)
AS
BEGIN	

	declare @temp TABLE
	(	 
		PurchaseItemID int,
		PurchaseItemStartDate datetime,
		DepositTerms varchar(100),
		DepositType varchar(20),
		DepositAmount money,
		DepositDueDate datetime,
		BalanceDueDate datetime,
		TotalNet money
	) 
	insert into @temp 
	select detail.PurchaseItemID, PurchaseItemStartDate, DepositTerms, DepositType, DepositAmount, DepositDueDate, BalanceDueDate, detail.TotalNet 
	from dbo.PurchaseItemDetail detail

	insert into @result
	select 
		PurchaseItemID, 
		DepositDueDate as PaymentDueDate,
		'Deposit' as PaymentDueName,
		case DepositType
			when 'c' then DepositAmount 
			when 'p' then TotalNet * DepositAmount / 100
		end as PaymentAmount
	from @temp where DepositTerms is not null
	UNION
	select 
		PurchaseItemID, 
		BalanceDueDate as PaymentDueDate,
		'Balance' as PaymentDueName, 
		case DepositType
			when 'c' then TotalNet - DepositAmount 
			when 'p' then TotalNet - (TotalNet * DepositAmount / 100)
		end as PaymentAmount
	from @temp where DepositTerms is not null
	UNION
	select 
		PurchaseItemID, 
		isnull(BalanceDueDate, PurchaseItemStartDate) as PaymentDueDate,
		'Full' as PaymentDueName,
		TotalNet as PaymentAmount
	from @temp where DepositTerms is null

RETURN 
END


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
PRINT N'Refreshing dbo.ServiceTypeDetail...';


GO
EXECUTE sp_refreshview N'dbo.ServiceTypeDetail';


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering dbo.ItineraryServiceTypeDetail...';


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
	sales.Amount as Sales,
	isnull(price.Gross,0) - isnull(sales.Amount,0) as GrossMinusSales,
	price.Gross - (price.Gross*100/(100+stype.GrossTaxPercent)) as GrossTaxAmount
from ItineraryServiceTypePricing() price
inner join Itinerary itin on price.ItineraryID = itin.ItineraryID
inner join ServiceTypeDetail stype on price.ServiceTypeID = stype.ServiceTypeID
left outer join 
(
	select ItineraryID, ServiceTypeID, sum(Amount) as Amount
	from ItinerarySale sale
	inner join ItinerarySaleAllocation alloc on sale.ItinerarySaleID = alloc.ItinerarySaleID
	group by ItineraryID, ServiceTypeID
) sales on itin.ItineraryID = sales.ItineraryID and stype.ServiceTypeID = sales.ServiceTypeID;


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
	HostName,
	StreetAddress,
	PostAddress,
	CityName,
	RegionName,
	StateName,
	CountryName,
	Phone,
	MobilePhone,
	FreePhone,
	Fax,
	Email,
	Website,
	CancellationPolicy,
	[Description] as SupplierDescription,
	[Comments] as SupplierComments,
	isnull(GradeName, '') as Grade1,
	isnull(GradeExternalName, '') as Grade2,
	sup.ParentFolderID as SupplierParentFolderID
from Supplier as sup
left outer join City as city on city.CityID = sup.CityID 
left outer join Region as region on region.RegionID = sup.RegionID 
left outer join State as state on state.StateID = sup.StateID 
left outer join Country as country on country.CountryID = sup.CountryID 
left outer join Grade as grade on grade.GradeID = sup.GradeID 
left outer join GradeExternal as gradeEx on gradeEx.GradeExternalID = sup.GradeExternalID;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Refreshing dbo.SupplierRatesDetail...';


GO
EXECUTE sp_refreshview N'dbo.SupplierRatesDetail';


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
------------------------------------------------------------------------------------------

PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2009.1.28'
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

------------------------------------------------------------------------------------------
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

--========================================================================================

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO
SET IDENTITY_INSERT [dbo].[Flag] ON;
BEGIN TRANSACTION;
INSERT INTO [dbo].[Flag]([FlagID], [FlagImage])
SELECT '1', 0x89504E470D0A1A0A0000000D49484452000000100000001008060000001FF3FF61000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000206348524D00007A26000080840000FA00000080E8000075300000EA6000003A98000017709CBA513C0000023B49444154384FA5936D4893511886ED8B2885A01F168296A066594AE94A25FDD128A8100CAD84CC2408C34022088A322A9B650C06692ACEA94C0D9BC3DC4C2DB4986899D632C352EA8726455F4216AC31B7DEABF3BEC65823FBE3819B03E7705FE73E0FCFB3000898D79201FF52B13A25A42929CED217BFEE5DAF2A72A23931A6FDDAB104C3D5AC5084DA84F6288FCF05F81C1628CD64EF431A7A8A6435E349DDCC68522C6EE737BAAE6C55200AA03865DB4247F4D21647D4921947C4621CE142618BF0E84BE1CD287CFA08437618ECC776B78EC9EE621A4EC5CA80A30A409B1C1FE88C5CD68B510F8DB55027F6EE7BF0EC097475C0C8F0EC79F54D2A0A54BCEFD1A13B122E03421480216953946B4DD04B05202273281D4A2E418330F5F7C1C023A837307DFAA4CB7C56C5DB3B05B25978676B17605145A4BBA3564D6234404E26E41E84EB97C1D4082F44F4AE4E91AA8AD1B2D2699B7607E39D857F03EC71AB3552B9EE3B9A42C8CB86E339A0D540AB0926C6A1E701DC6F67B8B292D7C603F4DFD8250346BC09BE6E08B28A173C4AF48CDD50741E5A6E439F0DCFADDA5F3F0E67FE7465A5B96D79390AA0F542B20C28F2029CD1810392BE0C9AEA858C481D565C6A15536B83A4E7FBF7DADDE5BAAC0FF9B9355B428319A9C9A02A3F4606A47A01CD89EBF5AF62831D5311CBA52F4263912B5DA684E8C7BEFD210CEAEA131B9504BE05FC6F23F9014A2C17B7CF0B30365891362F80627EA8DDE96D616F0DE69A05BF2F2800F3B9441970C6F76ECE61F20394FF9942795FE17BF71B16BB89F90508F2440000000049454E44AE426082 UNION ALL
SELECT '2', 0x89504E470D0A1A0A0000000D49484452000000100000001008060000001FF3FF61000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000206348524D00007A26000080840000FA00000080E8000075300000EA6000003A98000017709CBA513C0000024449444154384FA5D35F4853711407F035D0EA217A0C8A1E8C28B07F0F6594890F4941512F155224444205493ED483456D29336B24B96E7F4633D39C3DDCAB686E6C16E958BAB5B9364A3625F3CFA69B54EEFFDCDDEE92F9EDDE9B8D355A2FBB70B80F3FCEE7777E8773560010E4F471C0BFA29BBC5DA27D7522686CDF921C6ADD9C542A2ABEDFBBB0A7E5EEE98D6043CDC651FEF26C80ED89103F867680F1EB41BB9BE1E9CB87FE591E16E341BCABDFCB233C4051923C0D551130132B619109F09110C0FA4880F0F82D24A2562C324EC4C33AD0010D06B56D98ED6F40C7D59D1C50F907D8D0DF7924B630750751E77D44A71B10FF4621E11F4064BE154CC48080BB167ED735C8AB8BE079DF84A673051CB09E0748AAFED4879EE29F1CE03514F2111ABDC8637440C5869A05C4708D1C43D78D224CF45473C96CEEEFDE092852A418516D4D46A7A5F01A77C167DACD029740CF108885B82ADAE073D760CA2159D2371E84B34FF437D04D5E7104C7C54B217B257CE6FD6C1C4078AC0AF4AC1C89D828A23E12116F0B3E9BA4186B2F878938CC018E54051AB23CB430B95CFEE02684ECE7F964665E0DFF4C1DBE0E1762D2BA0E9F5E16F0C06B713107485280AEB38C894C8840BB64ECBB1F2036A7C4DCDBD5B0C985B068CF3036BBE6B1728032F4D6E5C3F1E2241497B77140690A50759CF5188935B03C146258268499588537CDA5F1F4F96013CA9E576DE72B486FE07F07290390F6D696E4047CB1C88FE704F0C9BAC643A9114EF520DB2E643C8107BA6EEEE380EBE9675997290378BABC85DC7F6DFAD92FA3CFA46CF59788D50000000049454E44AE426082 UNION ALL
SELECT '3', 0x89504E470D0A1A0A0000000D49484452000000100000001008060000001FF3FF61000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000206348524D00007A26000080840000FA00000080E8000075300000EA6000003A98000017709CBA513C0000024349444154384FA5D36D4853511807702B237283A00F16821F825EB18C5E5611D587A2A0425092F2435404BD60219904494DAAEDCEA4C520AD856B265654DBCCB69C93F265A1656FAB94E95626E6CA6AB9D08AEBB86EDD7FCFB9C658A3F565171E2EDCCBFF779EE770CE040049093D0CF857AD3DA8495B7CEC96759EBA7D604E49DBFB8C22B3FDECBEE5C6D2BC7450DDA3DA222D1E0F9069FD628E630CCFFD222C7D229698C3C8ACF020141CC603F50A09918095FB3513A770FC9DC96A7E2C99E3915CCA6352198FF2CE303CC3C0E751C015003AFC80B3BE1ABE260DAE1FCD64C05E095876402B9BAA0DB6193CC0D53780C10B347E009E50A0C107747E1BFF7EB11BD01728F0F1A10EBADDB3189026018B0E1BE7CAF542170372EE03D954A75D147A0BB47F011E5119093DE21C112CC50AF4D615B03065C7F72E6976A1357B4675C867A455729B801D2DC019026EBCA3D68700077553497879AB67C4A95D8F7E87F26F6066B18BD37589DF952F809D14DED50A70AF00531FD0FF13681E04EC348AEE662D7A6AB6A3E3C22606B8231DC85543365A21CC5ADFDC089C24E836859D9F688C9EF0AFDCFA1FA3597621546B334BC0DD92D50C504500D9B9E0D30AB7886BBD400DCD6D1B10A13009909F0F885B2FBF74E95E87F2F6D40D562D4D4F85BB6A1B2AF33318B02E022C28341B5295DD7C8A2620A6705FC5E92AAF30BFC8F438FA7C5060C395430BA50EA237F0BF07290628B39E5A9310E07DA6CF4A0890C22DDA8D91231CD983787721660409B09C58C580E3D1FFE25EA618E0D29F5BC8DED3A2FFFD06CBE1A19E7D9917550000000049454E44AE426082 UNION ALL
SELECT '4', 0x89504E470D0A1A0A0000000D49484452000000100000001008060000001FF3FF61000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000206348524D00007A26000080840000FA00000080E8000075300000EA6000003A98000017709CBA513C0000024549444154384FA5D35D4C5261180770EBB68BEEBA68F3A29B6EFA98A5942BD6854BCF2C6895E5726DB9A5992373AE59A0A46345C0057EE4085092F890486109C2209414714A56462EA7360B289C446AADADCDADAC7FE7BC35462CBAE16CFF9D8BF7797EE739EFDE770380AC8C1E06F8578AAF55B28E379EF874AAA5789DA3E6AC53B2D255D9853CAFF44C36E838E81C211F4F079CD4B1D0E03E077F7C04AEC587104D5F468982C2F7B5CF1812EF230801F65749365252E91CC79407CEFD3DE092E440E9BF8189E5313C5B9DC448DC036FCC0D9F5387F75E097AAEEC6680F304C8BD28DF74545EF6C5177F4C173D228563CBA378B232015BF401863F0CA227DC09ED1B3354B52C2C8EB6A1AD7C1B036C25C0AE9AEEEDF468DF7A237A088295241DF36218432AB8976C7406600CABA10CB6C2DAC0C2427F2DD34CF7FEDEBBAC63F5B2BAB39D453F2CEF0C680C56E17AB09A00A6B00683311799421FBA03C364377CF20284DD4D7F036CA1EE5EEF2B1D340BED687AC943F3740D14AF253047B408AC8CC3B3E42493B45B5A306B2845A0A38801661213E488F5CF6DD13EF05F54A07EAA1CADB322F447CDF0C49CB0448CB83A5E8DBA00FFA7D5612580ADF90003DC4C000715FC8FA690067D7431137BD402E1140F1C070B1566DE9A7ACED525F03AFC7BB3B760465B822EDE0E063894000A6E95BD2D34B2410DE482B2D331E5E3B084FB35F97CD00DA7EF5EDA492648DEC0FF1EA414E0B65DC4CE08987FAAE2660490E6617961E20827F620DD5D48F905025885F90C20485E4B7B995200E59F5BC8BC3727AFFD0203D88B80084FE06D0000000049454E44AE426082 UNION ALL
SELECT '5', 0x89504E470D0A1A0A0000000D49484452000000100000001008060000001FF3FF61000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000206348524D00007A26000080840000FA00000080E8000075300000EA6000003A98000017709CBA513C0000024449444154384FA5D36D4853511807707B237283A00F16821F82CAC2327A5945541F8A020B4149CA0F951544A0B422A2D25A356DF3A5C1A0598276B79A2E6A1BE566666FDA62CB99DBAC6CB659913A26924999701BD7ADFBEFDC638C355A5F76E1E15CEEE1F99DE73E9C671A8094A41E01F857280F6D4EBF53B2D2E2389539643FBD78D0742CABB5EAC85AA6B23003245A48ECA4872702BE9489F8497D3EF8A00BBCD78C8866157C17B3110E7DC7D3CBEB284201E5FEF5D3D99AD9F7D8EA59936CD54CB00A12153310E9D400A33E606204087A8021276C0F6E22F04C89A693D902709802AAA235A250E51C3B5C0D8047077493F5C32320D005F43F0446DE4E7DEFBA863AA904C1176AA88B160A403A0598A32B967072712F059AF281C63CA05D0EB849D2A0839CFC92BC3318379EE0CCA5127CBC2F159249EE54EF522CC717E58515F303703180A100B8BD17E828075E1B8061527A7F1BA9AA1EBE66CDB84DB515036DB2BF014FD90205EF50FFC0131960DC07980E00CF15C03B23F06D00F8D40EF85BE1B63278AFDF03E7D51D02E08D56F05529B6921322B4746D0EF0F83CD07B17F86C43C4ADFB35A12BF8C9E972C36F0CA51468BEB051002AA240A846F48A77D6023D8D24F4E0FBACE06A25189389F99E2BBB3C61BBBA70F8D641EDEA8C3478B5BB515F9C25005BA28049BAACA14F96C68E95A7F2A3F254DE7F661E672C5EDA197B3F48C2B61B25CB6905B10DFCEF458A03AA2D97362505F8BBEB7293026872876A7BF40A477B906816E27E8102E6731B04E06CEC5EC2618A03AEFF9942619D1BBBF71B34CE9DA13C92E8550000000049454E44AE426082 UNION ALL
SELECT '6', 0x89504E470D0A1A0A0000000D49484452000000100000001008060000001FF3FF61000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000206348524D00007A26000080840000FA00000080E8000075300000EA6000003A98000017709CBA513C0000023649444154384FA5935D48537118C6ADDB2EBA97BAE82602A3AB8C88B2E8AC2E844557B128302A092A8484A4B5923ED43E50040927B6C5CA8DDA58AEB92F6AE63CA93307966DB65A5BB39D52CB9AE65A0DE7CE797ACF59CC1CAD9B1D78F8C339E7F9FD9FF7E57D57002829EA1101FF52CB694369C7118F5577F075EC696D82D79FE116AE576FD65E53AC05C946AA942E2F043032F3C2A092C7EC1B01332F81E173C0836A3F16537370376C912012E0E609C34AD7AE74B773673AEDA848C346B2EE48E39D59402206A4E2408203BE47817EBB0E5C6F13F4B59B44C05109D072CCB8CABE3B33306107265CC07B3A3FFB807810987E4EC60830E5053E7A00754D393EB1AD68AD5A27024A25C0AD2AE77A8B8CF78B00EF05604805047559D8D700F08D343944EFF43CCCCA72842D35A299BCD9DE956814DEFD5639B8A883EAAC275DA69FEF023137301BA21423041800C2BD49F4373304BEB81C707F5FB83164C2FCB816F035909A08D0958D9C9C06BE8C6653BC70C510D09EC770DB5E11309E4BD023FBD113B521234667EB808006E0FA8099312AC321F0AC2AF58BADCB2C7AAECCE1557B171ED56F1301577300279319893C043E3C213DA6B88380FB380F932C2958CF464643465EC136C6EF94ADD9007F87129D27CB4440450ED079F8D96D939CFB696192423793100C95930BEA432CF57D69C0C8C0684E6D44F0DE8165F5FF7790F20037AC97B6170578EB53CB8B0248E6BEE63DB911CEF5A0D02EE4952001CCAAAD2280B662A93F0597290FD0FE670BC573F5DFDF7E03EDD3B934B8CC102F0000000049454E44AE426082 UNION ALL
SELECT '7', 0x89504E470D0A1A0A0000000D49484452000000100000001008060000001FF3FF61000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000000206348524D00007A26000080840000FA00000080E8000075300000EA6000003A98000017709CBA513C0000022E49444154384FA5935D48936114C71FC33B91E8032F142F84F02235283422A20B33C14031FAC0080A826E0ABCE82A8B5C64A5A2A5EDC2B98F5424AA0B996ED38239B7996B66E1852523B6A07D0869CE5CD36DBAD5FBF73C8F32E768DDEC85C381E7E1FF3BFF73DEE7640060697D1CF0AF787658966BCC53EB5CD96F3CCEAC41B725473DD272BDF479735D3E280C146744F1548020B349A8F200F610F07A1928FE86C0DE4F88459631FAF0A8800880FCA06C57948D69D798291A616388300BC2CC0A3CF9097C8900BE183045792204EB701FBCA6C77871EB10075C1300E581A62C89BD9F40F712A0F90574538C04011B5536509E26313F97FBA1A82FC3DC78073AAE167040AE0068F39E162273E6B300D478816A8AC60512916D2B41C629547477E5C7FA4043195C83F55C4CDACDD9B1C96C4D2DF638BC505195B33EE0FC1C209B07FA03643D0C0C930BC512DCCD8E80B5BD1CDFDFDEDB09F031ED23B42DFEC61DAA7A89009709F080FA7F4500571430AE00FA20865A7570F45FC4A4BC920366E30E426C544F15FE08EB9534F50602BD24B1691550FAFFA2C21346853BF6E1825E00861A8F7340531C800CDB143AFD401FF5DC4B6D68C9728993FE845DF294BE9B46CB62DD5A95B3E7487E0E667BCE4175A388034EC60196FD4AF50233845699590A329334CF8CEBE67D1A7BE2FB20C129CDCD62E1207180FF7D48498056DDFD136901BE7E5454A705106273FBE9F813DE1E628A654A6A410006EE1EE380DB897729972909D0B5B5853CEF4EBCDB000659ABE7110345C30000000049454E44AE426082
COMMIT;
RAISERROR (N'[dbo].[Flag]: Insert Batch: 1.....Done!', 10, 1) WITH NOWAIT;
GO
SET IDENTITY_INSERT [dbo].[Flag] OFF;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO
SET IDENTITY_INSERT [dbo].[TemplateCategory] ON;
BEGIN TRANSACTION;
INSERT INTO [dbo].[TemplateCategory]([TemplateCategoryID], [TemplateCategoryName], [ParentTemplateCategoryID])
SELECT '1', 'General', NULL UNION ALL
SELECT '2', 'Itinerary', NULL UNION ALL
SELECT '3', 'Supplier', NULL UNION ALL
SELECT '4', 'Accounting', NULL
COMMIT;
RAISERROR (N'[dbo].[TemplateCategory]: Insert Batch: 1.....Done!', 10, 1) WITH NOWAIT;
GO
SET IDENTITY_INSERT [dbo].[TemplateCategory] OFF;
GO
update [dbo].[Template] set ParentTemplateCategoryID = 4
GO
ALTER TABLE [dbo].[Template] ALTER COLUMN [ParentTemplateCategoryID] INT NOT NULL;
GO

INSERT INTO [dbo].[Template]([TemplateName], [FilePath], [ParentTemplateCategoryID]) values ('Client locations', '\Templates\Reports\General\ClientLocations.rdlc', '1')
INSERT INTO [dbo].[Template]([TemplateName], [FilePath], [ParentTemplateCategoryID]) values ('Itinerary yield', '\Templates\Reports\General\ItineraryYield.rdlc', '1')
INSERT INTO [dbo].[Template]([TemplateName], [FilePath], [ParentTemplateCategoryID]) values ('Supplier purchases', '\Templates\Reports\General\SupplierPurchases.rdlc', '1')
INSERT INTO [dbo].[Template]([TemplateName], [FilePath], [ParentTemplateCategoryID]) values ('Supplier rates', '\Templates\Reports\General\SupplierRates.rdlc', '1')
INSERT INTO [dbo].[Template]([TemplateName], [FilePath], [ParentTemplateCategoryID]) values ('Client pricing', '\Templates\Reports\Itinerary\ClientPricing.rdlc', '2')
INSERT INTO [dbo].[Template]([TemplateName], [FilePath], [ParentTemplateCategoryID]) values ('Client pricing detailed', '\Templates\Reports\Itinerary\ClientPricingDetailed.rdlc', '2')
INSERT INTO [dbo].[Template]([TemplateName], [FilePath], [ParentTemplateCategoryID]) values ('Contact list', '\Templates\Reports\Itinerary\ContactList.rdlc', '2')
INSERT INTO [dbo].[Template]([TemplateName], [FilePath], [ParentTemplateCategoryID]) values ('Pricing detailed', '\Templates\Reports\Itinerary\PricingDetailed.rdlc', '2')
INSERT INTO [dbo].[Template]([TemplateName], [FilePath], [ParentTemplateCategoryID]) values ('Pricing summary', '\Templates\Reports\Itinerary\PricingSummary.rdlc', '2')
INSERT INTO [dbo].[Template]([TemplateName], [FilePath], [ParentTemplateCategoryID]) values ('Supplier remittance', '\Templates\Reports\Itinerary\SupplierRemittance.rdlc', '2')
INSERT INTO [dbo].[Template]([TemplateName], [FilePath], [ParentTemplateCategoryID]) values ('Voucher', '\Templates\Reports\Itinerary\Voucher.rdlc', '2')
INSERT INTO [dbo].[Template]([TemplateName], [FilePath], [ParentTemplateCategoryID]) values ('Who used', '\Templates\Reports\Supplier\WhoUsed.rdlc', '3')
GO