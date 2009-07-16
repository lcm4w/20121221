/*
TourWriter database update script, from version 2009.4.20 to 2009.5.5
*/
GO
if ('2009.5.5' <= (select VersionNumber from AppSettings)) return
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
PRINT N'Dropping dbo.FK_ServiceTime_Service...';


GO
ALTER TABLE [dbo].[ServiceTime] DROP CONSTRAINT [FK_ServiceTime_Service];


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
PRINT N'Dropping dbo.FK_ServiceConfig_Service...';


GO
ALTER TABLE [dbo].[ServiceConfig] DROP CONSTRAINT [FK_ServiceConfig_Service];


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
PRINT N'Dropping dbo.FK_ServiceConfig_ServiceConfigType...';


GO
ALTER TABLE [dbo].[ServiceConfig] DROP CONSTRAINT [FK_ServiceConfig_ServiceConfigType];


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
PRINT N'Dropping dbo.FK_Supplier_GradeExternal...';


GO
ALTER TABLE [dbo].[Supplier] DROP CONSTRAINT [FK_Supplier_GradeExternal];


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
PRINT N'Dropping dbo.FK_SupplierConfig_SupplierConfigType...';


GO
ALTER TABLE [dbo].[SupplierConfig] DROP CONSTRAINT [FK_SupplierConfig_SupplierConfigType];


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
        INSERT INTO [dbo].[tmp_ms_xx_Service] ([ServiceID], [ServiceName], [SupplierID], [ServiceTypeID], [Description], [Comments], [Warning], [MaxPax], [CheckinTime], [CheckoutTime], [CheckinMinutesEarly], [IsRecordActive], [CurrencyCode], [ChargeType], [PaymentTermID], [TaxTypeID], [AddedOn], [AddedBy], [IsDeleted])
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
PRINT N'Starting rebuilding table dbo.ServiceConfig...';


GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;


GO
BEGIN TRANSACTION;

CREATE TABLE [dbo].[tmp_ms_xx_ServiceConfig] (
    [ServiceID]           INT          NOT NULL,
    [ServiceConfigTypeID] INT          NOT NULL,
    [Note]                VARCHAR (50) NULL,
    [AddedOn]             DATETIME     NULL,
    [AddedBy]             INT          NULL,
    [RowVersion]          TIMESTAMP    NULL
);

ALTER TABLE [dbo].[tmp_ms_xx_ServiceConfig]
    ADD CONSTRAINT [tmp_ms_xx_clusteredindex_PK_ServiceConfig] PRIMARY KEY CLUSTERED ([ServiceID] ASC, [ServiceConfigTypeID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

IF EXISTS (SELECT TOP 1 1
           FROM   [dbo].[ServiceConfig])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_ServiceConfig] ([ServiceID], [ServiceConfigTypeID], [AddedOn], [AddedBy])
        SELECT   [ServiceID],
                 [ServiceConfigTypeID],
                 [AddedOn],
                 [AddedBy]
        FROM     [dbo].[ServiceConfig]
        ORDER BY [ServiceID] ASC, [ServiceConfigTypeID] ASC;
    END

DROP TABLE [dbo].[ServiceConfig];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_ServiceConfig]', N'ServiceConfig';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_clusteredindex_PK_ServiceConfig]', N'PK_ServiceConfig', N'OBJECT';

COMMIT TRANSACTION;


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
    [Description]         VARCHAR (2000) NULL,
    [Comments]            VARCHAR (2000) NULL,
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
        INSERT INTO [dbo].[tmp_ms_xx_Supplier] ([SupplierID], [SupplierName], [HostName], [StreetAddress], [PostAddress], [CityID], [RegionID], [StateID], [CountryID], [Phone], [MobilePhone], [FreePhone], [Fax], [Email], [Website], [GradeID], [GradeExternalID], [Description], [Comments], [CancellationPolicy], [BankDetails], [AccountingName], [TaxTypeID], [PaymentTermID], [DefaultMargin], [DefaultCheckinTime], [DefaultCheckoutTime], [ImportID], [ExportID], [BookingWebsite], [IsRecordActive], [ParentFolderID], [AddedOn], [AddedBy], [IsDeleted])
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
PRINT N'Starting rebuilding table dbo.SupplierConfig...';


GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;


GO
BEGIN TRANSACTION;

CREATE TABLE [dbo].[tmp_ms_xx_SupplierConfig] (
    [SupplierID]           INT          NOT NULL,
    [SupplierConfigTypeID] INT          NOT NULL,
    [Note]                 VARCHAR (50) NULL,
    [AddedOn]              DATETIME     NULL,
    [AddedBy]              INT          NULL,
    [RowVersion]           TIMESTAMP    NULL
);

ALTER TABLE [dbo].[tmp_ms_xx_SupplierConfig]
    ADD CONSTRAINT [tmp_ms_xx_clusteredindex_PK_SupplierConfig] PRIMARY KEY CLUSTERED ([SupplierID] ASC, [SupplierConfigTypeID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

IF EXISTS (SELECT TOP 1 1
           FROM   [dbo].[SupplierConfig])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_SupplierConfig] ([SupplierID], [SupplierConfigTypeID], [AddedOn], [AddedBy])
        SELECT   [SupplierID],
                 [SupplierConfigTypeID],
                 [AddedOn],
                 [AddedBy]
        FROM     [dbo].[SupplierConfig]
        ORDER BY [SupplierID] ASC, [SupplierConfigTypeID] ASC;
    END

DROP TABLE [dbo].[SupplierConfig];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_SupplierConfig]', N'SupplierConfig';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_clusteredindex_PK_SupplierConfig]', N'PK_SupplierConfig', N'OBJECT';

COMMIT TRANSACTION;


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
PRINT N'Creating dbo.FK_Service_Supplier...';


GO
ALTER TABLE [dbo].[Service]
    ADD CONSTRAINT [FK_Service_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[Supplier] ([SupplierID]) ON DELETE CASCADE ON UPDATE CASCADE;


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
PRINT N'Creating dbo.FK_Rate_Service...';


GO
ALTER TABLE [dbo].[Rate]
    ADD CONSTRAINT [FK_Rate_Service] FOREIGN KEY ([ServiceID]) REFERENCES [dbo].[Service] ([ServiceID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating dbo.FK_ServiceConfig_ServiceConfigType...';


GO
ALTER TABLE [dbo].[ServiceConfig]
    ADD CONSTRAINT [FK_ServiceConfig_ServiceConfigType] FOREIGN KEY ([ServiceConfigTypeID]) REFERENCES [dbo].[ServiceConfigType] ([ServiceConfigTypeID]) ON DELETE CASCADE ON UPDATE CASCADE;


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
PRINT N'Creating dbo.FK_Supplier_GradeExternal...';


GO
ALTER TABLE [dbo].[Supplier]
    ADD CONSTRAINT [FK_Supplier_GradeExternal] FOREIGN KEY ([GradeExternalID]) REFERENCES [dbo].[GradeExternal] ([GradeExternalID]) ON DELETE CASCADE ON UPDATE CASCADE;


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
PRINT N'Creating dbo.FK_SupplierConfig_SupplierConfigType...';


GO
ALTER TABLE [dbo].[SupplierConfig]
    ADD CONSTRAINT [FK_SupplierConfig_SupplierConfigType] FOREIGN KEY ([SupplierConfigTypeID]) REFERENCES [dbo].[SupplierConfigType] ([SupplierConfigTypeID]) ON DELETE CASCADE ON UPDATE CASCADE;


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
		case when Gross = 0 then 0 else (GrossOrig - Net)/GrossOrig*100 end as CommissionOrig
	
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
PRINT N'Refreshing dbo.ItinerarySaleAllocationDetail...';


GO
EXECUTE sp_refreshview N'dbo.ItinerarySaleAllocationDetail';


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
PRINT N'Refreshing dbo.ItineraryServiceTypeDetail...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryServiceTypeDetail';


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
PRINT N'Refreshing dbo.ItinerarySaleDetail...';


GO
EXECUTE sp_refreshview N'dbo.ItinerarySaleDetail';


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Refreshing dbo.SupplierDetail...';


GO
EXECUTE sp_refreshview N'dbo.SupplierDetail';


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
PRINT N'Refreshing dbo.SupplierRatesDetail...';


GO
EXECUTE sp_refreshview N'dbo.SupplierRatesDetail';


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
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2009.5.5'
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
