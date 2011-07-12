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
if ((select VersionNumber from AppSettings) <> '2011.06.28' and (select VersionNumber from AppSettings) <> '2011.07.10')
	RAISERROR (N'Database Update Script is not correct version for current database version',17,1)	

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------

GO

PRINT N'Altering [dbo].[Agent]...';
GO
if not Exists(select * from sys.columns where Name = N'NetMinOrMax' and Object_ID = Object_ID(N'Agent'))
	ALTER TABLE [dbo].[Agent]
		ADD [NetMinOrMax] VARCHAR (10) NULL;

GO
PRINT N'Altering [dbo].[Itinerary]...';
GO
if not Exists(select * from sys.columns where Name = N'NetMinOrMax' and Object_ID = Object_ID(N'Itinerary'))
	ALTER TABLE [dbo].[Itinerary]
		ADD [NetMinOrMax] VARCHAR (10) NULL;
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
	[NetMinOrMax]
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
	@NetMinOrMax
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
	@NetMinOrMax varchar(10)
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
	[NetMinOrMax] = @NetMinOrMax
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
	[NetMinOrMax]
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
	[NetMinOrMax]
FROM [dbo].[Agent]
ORDER BY 
	[AgentID] ASC
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
	[NetMinOrMax]
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
	@NetMinOrMax
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
	@NetMinOrMax varchar(10)
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
	[NetMinOrMax] = @NetMinOrMax
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
	[NetMinOrMax]
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
	[NetMinOrMax]
FROM [dbo].[Itinerary]
ORDER BY 
	[ItineraryID] ASC
GO


if not Exists(select * from sys.columns where Name = N'ExportID' and Object_ID = Object_ID(N'Service'))
	ALTER TABLE [dbo].[Service]
		ADD [ExportID] INT NULL;
GO
if not Exists(select * from sys.columns where Name = N'ExportID' and Object_ID = Object_ID(N'Rate'))
	ALTER TABLE [dbo].[Rate]
	    ADD [ExportID] INT NULL;
GO
if not Exists(select * from sys.columns where Name = N'ExportID' and Object_ID = Object_ID(N'Option'))
	ALTER TABLE [dbo].[Option]
		ADD [ExportID] INT NULL;
GO


if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[_AgentSet_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[_AgentSet_Sel_ByID]
GO

/* Select from multiple tables for dataset fill command */
CREATE PROCEDURE [dbo].[_AgentSet_Sel_ByID]
	@AgentID int
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
	[PaymentTermID] = (SELECT [PurchasePaymentTermID] FROM [Agent] WHERE [AgentID] = @AgentID)
OR
	[PaymentTermID] = (SELECT [SalePaymentTermID] FROM [Agent] WHERE [AgentID] = @AgentID)

-- Agent --
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
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[CurrencyCode],
	[NetMinOrMax]
FROM [dbo].[Agent]
WHERE
	[AgentID] = @AgentID

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
		SELECT [ContactID] FROM [dbo].[AgentContact] 
		WHERE [AgentID] = @AgentID )

-- AgentContact --
SELECT
	[AgentID],
	[ContactID],
	[Description],
	[IsDefaultContact],
	[IsDefaultBilling]
FROM [dbo].[AgentContact]
WHERE
	[AgentID] = @AgentID

-- AgentMargin --
SELECT
	[AgentID],
	[ServiceTypeID],
	[Margin]
FROM [dbo].[AgentMargin]
WHERE
	[AgentID] = @AgentID
GO


IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAdjustedItemGross]') AND type in (N'FN', N'IF',N'TF', N'FS', N'FT'))
	DROP FUNCTION [dbo].[GetAdjustedItemGross]
GO

create FUNCTION [dbo].[GetAdjustedItemGross] 
(	
	@itinMargin money,
	@stypeMargin money,
	@net money,
	@gross money,
	@comOrMup char(3),
	@minOrMax varchar(10)
)
RETURNS money
AS
BEGIN

	if (@net is null or @gross is null or @net = 0 or @gross = 0)
		return @gross
	
	declare @overrideMargin money
	set @overrideMargin = isnull(@itinMargin, @stypeMargin)
	
	-- nothing to do?
	if (@overrideMargin is null)
		return @gross -- no override

	-- get the current margin (com or mup)
	declare @currentMargin money
	if (@comOrMup = 'com')
		set @currentMargin = (@gross-@net)/@gross*100
	if (@comOrMup = 'mup')
		set @currentMargin = (@gross-@net)/@net*100		
	
	-- are we constraining the minimum or maximun margin?
	if (@minOrMax = 'min' and @currentMargin > @overrideMargin)
		set @overrideMargin = @currentMargin		
	if (@minOrMax = 'max' and @currentMargin < @overrideMargin)
		set @overrideMargin = @currentMargin
		
	-- recalculate the gross based on override margin
	if (@comOrMup = 'com')
		set @gross = @net*100/(100-@overrideMargin)
	if (@comOrMup = 'mup')
		set @gross = @net*(1+@overrideMargin/100)
	
	return @gross	

/*
SELECT 80*100/(100-cast(10 as money)) mup10, 80*100/(100-cast(25 as money)) mup25, 80*100/(100-cast(50 as money)) mup50, 
	   80*(1+cast(10 as money)/100) com10, 80*(1+(cast(25 as money)/100)) com25, 80*(1+(cast(50 as money)/100)) com50

select 
 case when (dbo.GetAdjustedItemGross(10,50,80,100,'mup','   ')     =  88.0000) then 'pass' else 'FAIL' end '1'
,case when (dbo.GetAdjustedItemGross(10,50,80,100,'mup','min')     = 100.0000) then 'pass' else 'FAIL' end '2' 
,case when (dbo.GetAdjustedItemGross(10,50,80,100,'mup','max')     =  88.0000) then 'pass' else 'FAIL' end '3'

,case when (dbo.GetAdjustedItemGross(10,50,80,100,'com','   ')     =  88.8888) then 'pass' else 'FAIL' end '4'
,case when (dbo.GetAdjustedItemGross(10,50,80,100,'com','min')     = 100.0000) then 'pass' else 'FAIL' end '5'
,case when (dbo.GetAdjustedItemGross(10,50,80,100,'com','max')     =  88.8888) then 'pass' else 'FAIL' end '6'

,case when (dbo.GetAdjustedItemGross(null,50,80,100,'mup','   ')   = 120.0000) then 'pass' else 'FAIL' end '7'
,case when (dbo.GetAdjustedItemGross(null,50,80,100,'mup','min')   = 120.0000) then 'pass' else 'FAIL' end '8'
,case when (dbo.GetAdjustedItemGross(null,50,80,100,'mup','max')   = 100.0000) then 'pass' else 'FAIL' end '9'

,case when (dbo.GetAdjustedItemGross(null,50,80,100,'com','   ')   = 160.0000) then 'pass' else 'FAIL' end '10'
,case when (dbo.GetAdjustedItemGross(null,50,80,100,'com','min')   = 160.0000) then 'pass' else 'FAIL' end '11'
,case when (dbo.GetAdjustedItemGross(null,50,80,100,'com','max')   = 100.0000) then 'pass' else 'FAIL' end '12'

,case when (dbo.GetAdjustedItemGross(null,null,80,100,'mup','   ') = 100.0000) then 'pass' else 'FAIL' end '13'
,case when (dbo.GetAdjustedItemGross(null,null,80,100,'mup','min') = 100.0000) then 'pass' else 'FAIL' end '14'
,case when (dbo.GetAdjustedItemGross(null,null,80,100,'mup','max') = 100.0000) then 'pass' else 'FAIL' end '15'

,case when (dbo.GetAdjustedItemGross(null,null,80,100,'com','   ') = 100.0000) then 'pass' else 'FAIL' end '16'
,case when (dbo.GetAdjustedItemGross(null,null,80,100,'com','min') = 100.0000) then 'pass' else 'FAIL' end '17'
,case when (dbo.GetAdjustedItemGross(null,null,80,100,'com','max') = 100.0000) then 'pass' else 'FAIL' end '18'
*/				
				
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseItemPricing]') AND type in (N'FN', N'IF',N'TF', N'FS', N'FT'))
	DROP FUNCTION [dbo].[PurchaseItemPricing]
GO

create FUNCTION [dbo].[PurchaseItemPricing]
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
				dbo.GetAdjustedItemGross(itin.NetMargin, stype.Margin, item.Net, item.Gross, itin.NetComOrMup, itin.NetMinOrMax) as GrossAdjusted,
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

exec __RefreshViews
GO

----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2011.07.12'
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
