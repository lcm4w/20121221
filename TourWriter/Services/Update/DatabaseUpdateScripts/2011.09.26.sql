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
if ((select VersionNumber from AppSettings) <> '2011.08.16')
	RAISERROR (N'Database Update Script is not correct version for current database version',17,1)	

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------
GO
PRINT N'Altering [dbo].[Supplier]...';
GO
IF NOT EXISTS (select * from sys.columns where Name = N'ImportID' and Object_ID = Object_ID(N'Supplier'))
	ALTER TABLE [dbo].[Supplier] ADD [ImportID] INT NULL;
UPDATE [dbo].[Supplier] SET [ImportID] = NULL;
ALTER TABLE [dbo].[Supplier] ALTER COLUMN [ImportID] INT NULL;
GO
PRINT N'Altering [dbo].[Service]...';
GO
IF EXISTS (select * from sys.columns where Name = N'ExportID' and Object_ID = Object_ID(N'Service'))
	EXEC sp_RENAME '[Service].[ExportID]' , 'ImportID', 'COLUMN';	
IF NOT EXISTS (select * from sys.columns where Name = N'ImportID' and Object_ID = Object_ID(N'Service'))
	ALTER TABLE [dbo].[Service] ADD [ImportID] INT NULL;
ALTER TABLE [dbo].[Service] ALTER COLUMN [ImportID] INT NULL;
GO
PRINT N'Altering [dbo].[Rate]...';
GO
IF EXISTS (select * from sys.columns where Name = N'ExportID' and Object_ID = Object_ID(N'Rate'))
	EXEC sp_RENAME '[Rate].[ExportID]' , 'ImportID', 'COLUMN';	
IF NOT EXISTS (select * from sys.columns where Name = N'ImportID' and Object_ID = Object_ID(N'Rate'))
	ALTER TABLE [dbo].[Rate] ADD [ImportID] INT NULL;
ALTER TABLE [dbo].[Rate] ALTER COLUMN [ImportID] INT NULL;
GO
PRINT N'Altering [dbo].[Option]...';
GO
IF EXISTS (select * from sys.columns where Name = N'ExportID' and Object_ID = Object_ID(N'Option'))
	EXEC sp_RENAME '[Option].[ExportID]' , 'ImportID', 'COLUMN';	
IF NOT EXISTS (select * from sys.columns where Name = N'ImportID' and Object_ID = Object_ID(N'Option'))
	ALTER TABLE [dbo].[Option] ADD [ImportID] INT NULL;
ALTER TABLE [dbo].[Option] ALTER COLUMN [ImportID] INT NULL;
GO


GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Option_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Option_Ins]
GO

CREATE PROCEDURE [dbo].[Option_Ins]
	@OptionName varchar(150),
	@OptionTypeID int,
	@RateID int,
	@Net money,
	@Gross money,
	@PricingOption char(2),
	@IsDefault bit,
	@IsRecordActive bit,
	@AddedOn datetime,
	@AddedBy int,
	@IsDeleted bit,
	@ImportID int,
	@OptionID int OUTPUT
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
	[IsDeleted],
	[ImportID]
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
	@IsDeleted,
	@ImportID
)
SELECT @OptionID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Option_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Option_Upd]
GO

CREATE PROCEDURE [dbo].[Option_Upd]
	@OptionID int,
	@OptionName varchar(150),
	@OptionTypeID int,
	@RateID int,
	@Net money,
	@Gross money,
	@PricingOption char(2),
	@IsDefault bit,
	@IsRecordActive bit,
	@AddedOn datetime,
	@AddedBy int,
	@RowVersion timestamp,
	@IsDeleted bit,
	@ImportID int
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
	[IsDeleted] = @IsDeleted,
	[ImportID] = @ImportID
WHERE
	[OptionID] = @OptionID
	AND [RowVersion] = @RowVersion
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Option_Upd_ByRateID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Option_Upd_ByRateID]
GO

CREATE PROCEDURE [dbo].[Option_Upd_ByRateID]
	@RateID int,
	@RateIDOld int
AS
UPDATE [dbo].[Option]
SET
	[RateID] = @RateID
WHERE
	[RateID] = @RateIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Option_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Option_Del]
GO

CREATE PROCEDURE [dbo].[Option_Del]
	@OptionID int,
	@RowVersion timestamp
AS
DELETE FROM [dbo].[Option]
WHERE
	[OptionID] = @OptionID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Option_Del_ByRateID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Option_Del_ByRateID]
GO

CREATE PROCEDURE [dbo].[Option_Del_ByRateID]
	@RateID int
AS
DELETE
FROM [dbo].[Option]
WHERE
	[RateID] = @RateID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Option_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Option_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[Option_Sel_ByID]
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
	[IsDeleted],
	[ImportID]
FROM [dbo].[Option]
WHERE
	[OptionID] = @OptionID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Option_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Option_Sel_All]
GO

CREATE PROCEDURE [dbo].[Option_Sel_All]
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
	[IsDeleted],
	[ImportID]
FROM [dbo].[Option]
ORDER BY 
	[OptionID] ASC
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Option_Sel_ByRateID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Option_Sel_ByRateID]
GO

CREATE PROCEDURE [dbo].[Option_Sel_ByRateID]
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
	[IsDeleted],
	[ImportID]
FROM [dbo].[Option]
WHERE
	[RateID] = @RateID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Rate_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Rate_Ins]
GO

CREATE PROCEDURE [dbo].[Rate_Ins]
	@ServiceID int,
	@ValidFrom datetime,
	@ValidTo datetime,
	@AddedOn datetime,
	@AddedBy int,
	@IsDeleted bit,
	@ImportID int,
	@RateID int OUTPUT
AS
INSERT [dbo].[Rate]
(
	[ServiceID],
	[ValidFrom],
	[ValidTo],
	[AddedOn],
	[AddedBy],
	[IsDeleted],
	[ImportID]
)
VALUES
(
	@ServiceID,
	ISNULL(@ValidFrom, (getdate())),
	ISNULL(@ValidTo, (getdate())),
	@AddedOn,
	@AddedBy,
	@IsDeleted,
	@ImportID
)
SELECT @RateID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Rate_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Rate_Upd]
GO

CREATE PROCEDURE [dbo].[Rate_Upd]
	@RateID int,
	@ServiceID int,
	@ValidFrom datetime,
	@ValidTo datetime,
	@AddedOn datetime,
	@AddedBy int,
	@RowVersion timestamp,
	@IsDeleted bit,
	@ImportID int
AS
UPDATE [dbo].[Rate]
SET 
	[ServiceID] = @ServiceID,
	[ValidFrom] = @ValidFrom,
	[ValidTo] = @ValidTo,
	[AddedOn] = @AddedOn,
	[AddedBy] = @AddedBy,
	[IsDeleted] = @IsDeleted,
	[ImportID] = @ImportID
WHERE
	[RateID] = @RateID
	AND [RowVersion] = @RowVersion
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Rate_Upd_ByServiceID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Rate_Upd_ByServiceID]
GO

CREATE PROCEDURE [dbo].[Rate_Upd_ByServiceID]
	@ServiceID int,
	@ServiceIDOld int
AS
UPDATE [dbo].[Rate]
SET
	[ServiceID] = @ServiceID
WHERE
	[ServiceID] = @ServiceIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Rate_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Rate_Del]
GO

CREATE PROCEDURE [dbo].[Rate_Del]
	@RateID int,
	@RowVersion timestamp
AS
DELETE FROM [dbo].[Rate]
WHERE
	[RateID] = @RateID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Rate_Del_ByServiceID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Rate_Del_ByServiceID]
GO

CREATE PROCEDURE [dbo].[Rate_Del_ByServiceID]
	@ServiceID int
AS
DELETE
FROM [dbo].[Rate]
WHERE
	[ServiceID] = @ServiceID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Rate_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Rate_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[Rate_Sel_ByID]
	@RateID int
AS
SET NOCOUNT ON
SELECT
	[RateID],
	[ServiceID],
	[ValidFrom],
	[ValidTo],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[ImportID]
FROM [dbo].[Rate]
WHERE
	[RateID] = @RateID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Rate_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Rate_Sel_All]
GO

CREATE PROCEDURE [dbo].[Rate_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[RateID],
	[ServiceID],
	[ValidFrom],
	[ValidTo],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[ImportID]
FROM [dbo].[Rate]
ORDER BY 
	[RateID] ASC
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Rate_Sel_ByServiceID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Rate_Sel_ByServiceID]
GO

CREATE PROCEDURE [dbo].[Rate_Sel_ByServiceID]
	@ServiceID int
AS
SET NOCOUNT ON
SELECT
	[RateID],
	[ServiceID],
	[ValidFrom],
	[ValidTo],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[ImportID]
FROM [dbo].[Rate]
WHERE
	[ServiceID] = @ServiceID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Service_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Service_Ins]
GO

CREATE PROCEDURE [dbo].[Service_Ins]
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
	@ImportID int,
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
	[IsDeleted],
	[ImportID]
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
	@IsDeleted,
	@ImportID
)
SELECT @ServiceID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Service_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Service_Upd]
GO

CREATE PROCEDURE [dbo].[Service_Upd]
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
	@IsDeleted bit,
	@ImportID int
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
	[IsDeleted] = @IsDeleted,
	[ImportID] = @ImportID
WHERE
	[ServiceID] = @ServiceID
	AND [RowVersion] = @RowVersion
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Service_Upd_BySupplierID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Service_Upd_BySupplierID]
GO

CREATE PROCEDURE [dbo].[Service_Upd_BySupplierID]
	@SupplierID int,
	@SupplierIDOld int
AS
UPDATE [dbo].[Service]
SET
	[SupplierID] = @SupplierID
WHERE
	[SupplierID] = @SupplierIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Service_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Service_Del]
GO

CREATE PROCEDURE [dbo].[Service_Del]
	@ServiceID int,
	@RowVersion timestamp
AS
DELETE FROM [dbo].[Service]
WHERE
	[ServiceID] = @ServiceID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Service_Del_BySupplierID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Service_Del_BySupplierID]
GO

CREATE PROCEDURE [dbo].[Service_Del_BySupplierID]
	@SupplierID int
AS
DELETE
FROM [dbo].[Service]
WHERE
	[SupplierID] = @SupplierID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Service_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Service_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[Service_Sel_ByID]
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
	[IsDeleted],
	[ImportID]
FROM [dbo].[Service]
WHERE
	[ServiceID] = @ServiceID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Service_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Service_Sel_All]
GO

CREATE PROCEDURE [dbo].[Service_Sel_All]
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
	[IsDeleted],
	[ImportID]
FROM [dbo].[Service]
ORDER BY 
	[ServiceID] ASC
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Service_Sel_BySupplierID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Service_Sel_BySupplierID]
GO

CREATE PROCEDURE [dbo].[Service_Sel_BySupplierID]
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
	[IsDeleted],
	[ImportID]
FROM [dbo].[Service]
WHERE
	[SupplierID] = @SupplierID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Supplier_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Supplier_Ins]
GO

CREATE PROCEDURE [dbo].[Supplier_Ins]
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
	@ImportID int,
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

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Supplier_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Supplier_Upd]
GO

CREATE PROCEDURE [dbo].[Supplier_Upd]
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
	@ImportID int,
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

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Supplier_Upd_ByGradeID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Supplier_Upd_ByGradeID]
GO

CREATE PROCEDURE [dbo].[Supplier_Upd_ByGradeID]
	@GradeID int,
	@GradeIDOld int
AS
UPDATE [dbo].[Supplier]
SET
	[GradeID] = @GradeID
WHERE
	[GradeID] = @GradeIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Supplier_Upd_ByGradeExternalID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Supplier_Upd_ByGradeExternalID]
GO

CREATE PROCEDURE [dbo].[Supplier_Upd_ByGradeExternalID]
	@GradeExternalID int,
	@GradeExternalIDOld int
AS
UPDATE [dbo].[Supplier]
SET
	[GradeExternalID] = @GradeExternalID
WHERE
	[GradeExternalID] = @GradeExternalIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Supplier_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Supplier_Del]
GO

CREATE PROCEDURE [dbo].[Supplier_Del]
	@SupplierID int,
	@RowVersion timestamp
AS
DELETE FROM [dbo].[Supplier]
WHERE
	[SupplierID] = @SupplierID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Supplier_Del_ByGradeID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Supplier_Del_ByGradeID]
GO

CREATE PROCEDURE [dbo].[Supplier_Del_ByGradeID]
	@GradeID int
AS
DELETE
FROM [dbo].[Supplier]
WHERE
	[GradeID] = @GradeID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Supplier_Del_ByGradeExternalID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Supplier_Del_ByGradeExternalID]
GO

CREATE PROCEDURE [dbo].[Supplier_Del_ByGradeExternalID]
	@GradeExternalID int
AS
DELETE
FROM [dbo].[Supplier]
WHERE
	[GradeExternalID] = @GradeExternalID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Supplier_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Supplier_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[Supplier_Sel_ByID]
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

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Supplier_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Supplier_Sel_All]
GO

CREATE PROCEDURE [dbo].[Supplier_Sel_All]
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

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Supplier_Sel_ByGradeID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Supplier_Sel_ByGradeID]
GO

CREATE PROCEDURE [dbo].[Supplier_Sel_ByGradeID]
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

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Supplier_Sel_ByGradeExternalID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Supplier_Sel_ByGradeExternalID]
GO

CREATE PROCEDURE [dbo].[Supplier_Sel_ByGradeExternalID]
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



if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[_SupplierSet_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[_SupplierSet_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[_SupplierSet_Sel_ByID]
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
	[IsDeleted],
	[ImportID]
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
	[IsDeleted],
	[ImportID]
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
	[IsDeleted],
	[ImportID]
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

-- Discount --
SELECT
	[DiscountID],
	[ServiceID],
	[UnitsUsed],
	[UnitsFree],
	[DiscountType]
FROM [dbo].[Discount]
WHERE
	[ServiceID] IN ( 
		SELECT [ServiceID] FROM [dbo].[Service] 
		WHERE [SupplierID] = @SupplierID )
		
-- Content
SELECT c.* 
FROM [Content] c
inner join SupplierContent s on s.ContentID = c.ContentID
WHERE s.SupplierID = @SupplierID
UNION
SELECT c.* 
FROM [Content] c
inner join ServiceContent s on s.ContentID = c.ContentID
where ServiceID in (select ServiceID from [Service] where SupplierID = @SupplierID)

-- SupplierContent
select * from SupplierContent
where SupplierID = @SupplierID

-- ServiceContent
select * from ServiceContent
where ServiceID in (select ServiceID from [Service] where SupplierID = @SupplierID)

----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2011.09.26'
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
