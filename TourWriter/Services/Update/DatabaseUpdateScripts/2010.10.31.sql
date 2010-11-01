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
if ((select VersionNumber from AppSettings) <> '2010.10.26')
	RAISERROR (N'Update script version is invalid for this db version',17,1)	
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------

GO
PRINT N'Dropping FK_AgentContact_Agent...';


GO
ALTER TABLE [dbo].[AgentContact] DROP CONSTRAINT [FK_AgentContact_Agent];


GO
PRINT N'Starting rebuilding table [dbo].[Agent]...';


GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

BEGIN TRANSACTION;

CREATE TABLE [dbo].[tmp_ms_xx_Agent] (
    [AgentID]               INT            IDENTITY (1, 1) NOT NULL,
    [AgentName]             VARCHAR (100)  NOT NULL,
    [ParentAgentID]         INT            NULL,
    [Address1]              VARCHAR (255)  NULL,
    [Address2]              VARCHAR (255)  NULL,
    [Address3]              VARCHAR (255)  NULL,
    [Phone]                 VARCHAR (50)   NULL,
    [Fax]                   VARCHAR (50)   NULL,
    [Email]                 VARCHAR (255)  NULL,
    [TaxNumber]             VARCHAR (50)   NULL,
    [InvoiceNumberMask]     VARCHAR (50)   NULL,
    [PurchasePaymentTermID] INT            NULL,
    [SalePaymentTermID]     INT            NULL,
    [LogoFile]              VARCHAR (255)  NULL,
    [VoucherLogoFile]       VARCHAR (255)  NULL,
    [NetComOrMup]           CHAR (3)       NULL,
    [Comments]              VARCHAR (2000) NULL,
    [AgentHeader]           TEXT           NULL,
    [RequestFooter]         TEXT           NULL,
    [ConfirmFooter]         TEXT           NULL,
    [RemitFooter]           TEXT           NULL,
    [ClientFooter]          TEXT           NULL,
    [VoucherFooter]         TEXT           NULL,
    [IsDefaultAgent]        BIT            NULL,
    [DefaultCurrencyMargin] MONEY          NULL,
    [AddedOn]               DATETIME       NULL,
    [AddedBy]               INT            NULL,
    [RowVersion]            TIMESTAMP      NULL
);

ALTER TABLE [dbo].[tmp_ms_xx_Agent]
    ADD CONSTRAINT [tmp_ms_xx_clusteredindex_PK_AgentLabel] PRIMARY KEY CLUSTERED ([AgentID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

IF EXISTS (SELECT TOP 1 1
           FROM   [dbo].[Agent])
    BEGIN
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Agent] ON;
        INSERT INTO [dbo].[tmp_ms_xx_Agent] ([AgentID], [AgentName], [ParentAgentID], [Address1], [Address2], [Address3], [Phone], [Fax], [Email], [TaxNumber], [InvoiceNumberMask], [PurchasePaymentTermID], [SalePaymentTermID], [LogoFile], [VoucherLogoFile], [NetComOrMup], [Comments], [AgentHeader], [RequestFooter], [ConfirmFooter], [RemitFooter], [ClientFooter], [VoucherFooter], [IsDefaultAgent], [AddedOn], [AddedBy])
        SELECT   [AgentID],
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
        FROM     [dbo].[Agent]
        ORDER BY [AgentID] ASC;
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Agent] OFF;
    END

DROP TABLE [dbo].[Agent];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_Agent]', N'Agent';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_clusteredindex_PK_AgentLabel]', N'PK_AgentLabel', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating FK_AgentContact_Agent...';


GO
ALTER TABLE [dbo].[AgentContact] WITH NOCHECK
    ADD CONSTRAINT [FK_AgentContact_Agent] FOREIGN KEY ([AgentID]) REFERENCES [dbo].[Agent] ([AgentID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Altering Agent_Sel_All...';


GO
ALTER PROCEDURE [dbo].[Agent_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[AgentID],
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
	[DefaultCurrencyMargin],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[Agent]
ORDER BY 
	[AgentID] ASC


GO
PRINT N'Refreshing [dbo].[ItineraryDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryDetail';


GO
PRINT N'Refreshing [dbo].[ItineraryServiceTypeDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryServiceTypeDetail';


GO
PRINT N'Refreshing [dbo].[ItinerarySaleDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItinerarySaleDetail';


GO
PRINT N'Refreshing [dbo].[PurchaseItemDetail]...';


GO
EXECUTE sp_refreshview N'dbo.PurchaseItemDetail';


GO
PRINT N'Refreshing [dbo].[ItineraryClientDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryClientDetail';


GO
PRINT N'Altering [dbo].[ItinerarySaleAllocationDetail]...';


GO


ALTER VIEW [dbo].[ItinerarySaleAllocationDetail]
AS
select
	itin.*,
	sale.ItinerarySaleID,
	sale.Comments,
	sale.SaleDate,
	stype.ServiceTypeName,	
	alloc.SaleAmount,
	alloc.Net as SaleNet,
	alloc.SaleAmount as SaleGross,
	alloc.Yield as SaleYield,		
	alloc.TaxAmount as SaleTaxAmount,	
	SaleGrossExcl = SaleAmount - alloc.TaxAmount,    	
	grossTax.TaxTypeCode as SaleTaxTypeCode,
	grossAct.AccountingCategoryCode as AccountingCategoryCode,
	sale.IsLockedAccounting
from ItinerarySaleAllocationPricing() alloc
right outer join ItinerarySale sale on alloc.ItinerarySaleID = sale.ItinerarySaleID
left outer join dbo.ItineraryDetail itin on sale.ItineraryID = itin.ItineraryID
left outer join ServiceType stype on alloc.ServiceTypeID = stype.ServiceTypeID
left outer join AccountingCategory as grossAct on stype.GrossAccountingCategoryID = grossAct.AccountingCategoryID
left outer join TaxType as grossTax on stype.GrossTaxTypeID = grossTax.TaxTypeID;
GO
PRINT N'Refreshing [dbo].[ItineraryPaymentDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryPaymentDetail';


GO
PRINT N'Refreshing [dbo].[PurchaseItemPaymentsDetail]...';


GO
EXECUTE sp_refreshview N'dbo.PurchaseItemPaymentsDetail';


GO
PRINT N'Checking existing data against newly created constraints';


GO
ALTER TABLE [dbo].[AgentContact] WITH CHECK CHECK CONSTRAINT [FK_AgentContact_Agent];


GO

----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2010.10.31'
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
