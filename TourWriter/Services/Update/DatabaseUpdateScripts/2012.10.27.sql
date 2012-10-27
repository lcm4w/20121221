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
if ((select VersionNumber from AppSettings) <> '2012.10.25')
	RAISERROR (N'Database Update Script is not correct version for current database version',17,1)

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------

GO
PRINT N'Altering [dbo].[Agent]...';


GO
if not Exists(select * from sys.columns where Name = N'AgentCode' and Object_ID = Object_ID(N'Agent'))
	ALTER TABLE [dbo].[Agent] 
		ADD [AgentCode] varchar(50);
GO


GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Agent_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Agent_Ins]
GO

CREATE PROCEDURE [dbo].[Agent_Ins]
	@AgentName varchar(100),
	@ParentAgentID int,
	@Address1 varchar(255),
	@Address2 varchar(255),
	@Address3 varchar(255),
	@Phone varchar(50),
	@Fax varchar(50),
	@Email varchar(255),
	@TaxNumber varchar(50),
	@InvoiceNumberMask varchar(50),
	@PurchasePaymentTermID int,
	@SalePaymentTermID int,
	@LogoFile varchar(255),
	@VoucherLogoFile varchar(255),
	@NetComOrMup char(3),
	@Comments varchar(2000),
	@AgentHeader text,
	@RequestFooter text,
	@ConfirmFooter text,
	@RemitFooter text,
	@ClientFooter text,
	@VoucherFooter text,
	@IsDefaultAgent bit,
	@DefaultCurrencyMargin money,
	@AddedOn datetime,
	@AddedBy int,
	@CurrencyCode char(3),
	@NetMinOrMax varchar(10),
	@AgentCode varchar(50),
	@AgentID int OUTPUT
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
	[DefaultCurrencyMargin],
	[AddedOn],
	[AddedBy],
	[CurrencyCode],
	[NetMinOrMax],
	[AgentCode]
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
	@DefaultCurrencyMargin,
	@AddedOn,
	@AddedBy,
	@CurrencyCode,
	@NetMinOrMax,
	@AgentCode
)
SELECT @AgentID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Agent_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Agent_Upd]
GO

CREATE PROCEDURE [dbo].[Agent_Upd]
	@AgentID int,
	@AgentName varchar(100),
	@ParentAgentID int,
	@Address1 varchar(255),
	@Address2 varchar(255),
	@Address3 varchar(255),
	@Phone varchar(50),
	@Fax varchar(50),
	@Email varchar(255),
	@TaxNumber varchar(50),
	@InvoiceNumberMask varchar(50),
	@PurchasePaymentTermID int,
	@SalePaymentTermID int,
	@LogoFile varchar(255),
	@VoucherLogoFile varchar(255),
	@NetComOrMup char(3),
	@Comments varchar(2000),
	@AgentHeader text,
	@RequestFooter text,
	@ConfirmFooter text,
	@RemitFooter text,
	@ClientFooter text,
	@VoucherFooter text,
	@IsDefaultAgent bit,
	@DefaultCurrencyMargin money,
	@AddedOn datetime,
	@AddedBy int,
	@RowVersion timestamp,
	@CurrencyCode char(3),
	@NetMinOrMax varchar(10),
	@AgentCode varchar(50)
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
	[DefaultCurrencyMargin] = @DefaultCurrencyMargin,
	[AddedOn] = @AddedOn,
	[AddedBy] = @AddedBy,
	[CurrencyCode] = @CurrencyCode,
	[NetMinOrMax] = @NetMinOrMax,
	[AgentCode] = @AgentCode
WHERE
	[AgentID] = @AgentID
	AND [RowVersion] = @RowVersion
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Agent_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Agent_Del]
GO

CREATE PROCEDURE [dbo].[Agent_Del]
	@AgentID int,
	@RowVersion timestamp
AS
DELETE FROM [dbo].[Agent]
WHERE
	[AgentID] = @AgentID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Agent_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Agent_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[Agent_Sel_ByID]
	@AgentID int
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
	[RowVersion],
	[CurrencyCode],
	[NetMinOrMax],
	[AgentCode]
FROM [dbo].[Agent]
WHERE
	[AgentID] = @AgentID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Agent_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Agent_Sel_All]
GO

CREATE PROCEDURE [dbo].[Agent_Sel_All]
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
	[RowVersion],
	[CurrencyCode],
	[NetMinOrMax],
	[AgentCode]
FROM [dbo].[Agent]
ORDER BY 
	[AgentID] ASC
GO



exec __RefreshViews;
GO
----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2012.10.27'
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