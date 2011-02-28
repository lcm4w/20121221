
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
if ((select VersionNumber from AppSettings) <> '2011.02.17')
	RAISERROR (N'Database Update Script is not correct version for current database version',17,1)	

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------
GO
PRINT N'Dropping DF_Service_Description...';


GO
ALTER TABLE [dbo].[Service] DROP CONSTRAINT [DF_Service_Description];


GO
PRINT N'Altering [dbo].[Contact]...';


GO
ALTER TABLE [dbo].[Contact] ALTER COLUMN [Notes] VARCHAR (8000) NULL;


GO
PRINT N'Altering [dbo].[Content]...';


GO
ALTER TABLE [dbo].[Content] ALTER COLUMN [Body] NVARCHAR (MAX) NULL;


GO
PRINT N'Altering [dbo].[Itinerary]...';


GO
ALTER TABLE [dbo].[Itinerary] ALTER COLUMN [ArriveNote] VARCHAR (4000) NULL;

ALTER TABLE [dbo].[Itinerary] ALTER COLUMN [Comments] VARCHAR (8000) NULL;

ALTER TABLE [dbo].[Itinerary] ALTER COLUMN [DepartNote] VARCHAR (4000) NULL;


GO
PRINT N'Altering [dbo].[Service]...';


GO
ALTER TABLE [dbo].[Service] ALTER COLUMN [Comments] VARCHAR (8000) NULL;

ALTER TABLE [dbo].[Service] ALTER COLUMN [Description] VARCHAR (8000) NULL;

ALTER TABLE [dbo].[Service] ALTER COLUMN [Warning] VARCHAR (8000) NULL;


GO
PRINT N'Altering [dbo].[Supplier]...';


GO
ALTER TABLE [dbo].[Supplier] ALTER COLUMN [Comments] VARCHAR (8000) NULL;

ALTER TABLE [dbo].[Supplier] ALTER COLUMN [Description] VARCHAR (8000) NULL;


GO
PRINT N'Creating DF_Service_Description...';


GO
ALTER TABLE [dbo].[Service]
    ADD CONSTRAINT [DF_Service_Description] DEFAULT ('') FOR [Description];


GO
PRINT N'Altering [dbo].[Contact_Ins]...';


GO

ALTER PROCEDURE [dbo].[Contact_Ins]
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
	[IsDeleted]
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
	@IsDeleted
)
SELECT @ContactID=SCOPE_IDENTITY()
GO
PRINT N'Altering [dbo].[Contact_Upd]...';


GO

ALTER PROCEDURE [dbo].[Contact_Upd]
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
	@IsDeleted bit
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
	[IsDeleted] = @IsDeleted
WHERE
	[ContactID] = @ContactID
	AND [RowVersion] = @RowVersion
GO
PRINT N'Altering [dbo].[Content_Ins]...';


GO

ALTER PROCEDURE [dbo].[Content_Ins]
	@SupplierId int,
	@ContentTypeId int,
	@ContentName nvarchar(50),
	@Heading nvarchar(250),
	@Body nvarchar(MAX),
	@ImagePath nvarchar(250),
	@ContentId int OUTPUT
AS
INSERT [dbo].[Content]
(
	[SupplierId],
	[ContentTypeId],
	[ContentName],
	[Heading],
	[Body],
	[ImagePath]
)
VALUES
(
	@SupplierId,
	@ContentTypeId,
	@ContentName,
	@Heading,
	@Body,
	@ImagePath
)
SELECT @ContentId=SCOPE_IDENTITY()
GO
PRINT N'Altering [dbo].[Content_Upd]...';


GO

ALTER PROCEDURE [dbo].[Content_Upd]
	@ContentId int,
	@SupplierId int,
	@ContentTypeId int,
	@ContentName nvarchar(50),
	@Heading nvarchar(250),
	@Body nvarchar(MAX),
	@ImagePath nvarchar(250)
AS
UPDATE [dbo].[Content]
SET 
	[SupplierId] = @SupplierId,
	[ContentTypeId] = @ContentTypeId,
	[ContentName] = @ContentName,
	[Heading] = @Heading,
	[Body] = @Body,
	[ImagePath] = @ImagePath
WHERE
	[ContentId] = @ContentId
GO
PRINT N'Altering [dbo].[Itinerary_Ins]...';


GO

ALTER PROCEDURE [dbo].[Itinerary_Ins]
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
	[CurrencyCode]
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
	@CurrencyCode
)
SELECT @ItineraryID=SCOPE_IDENTITY()
GO
PRINT N'Altering [dbo].[Itinerary_Upd]...';


GO

ALTER PROCEDURE [dbo].[Itinerary_Upd]
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
	@CurrencyCode char(3)
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
	[CurrencyCode] = @CurrencyCode
WHERE
	[ItineraryID] = @ItineraryID
	AND [RowVersion] = @RowVersion
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
	[IsDeleted],
	[GstUpdated]
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
	[IsDeleted],
	[GstUpdated]
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
	[IsDeleted],
	[GstUpdated]
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
	[RowVersion],
	[GstUpdated]
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
	[RowVersion],
	[GstUpdated]
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
	[RowVersion],
	[GstUpdated]
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
	[RowVersion],
	[GstUpdated]
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
	@TaxTypeID int,
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
	@TaxTypeID int,
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
	@TaxTypeID int,
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
	@TaxTypeID int,
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
		case when ntaxSrv.TaxTypeID is not null then ntaxSrv.TaxTypeCode
			 when ntaxSup.TaxTypeID is not null then ntaxSup.TaxTypeCode 
			 when ntaxTyp.TaxTypeID is not null then ntaxTyp.TaxTypeCode
		end as NetTaxCode,	
		case when ntaxSrv.TaxTypeID is not null then ntaxSrv.Amount
			 when ntaxSup.TaxTypeID is not null then ntaxSup.Amount 
			 when ntaxTyp.TaxTypeID is not null then ntaxTyp.Amount
		end as NetTaxPercent,
		gtax.TaxTypeCode as GrossTaxCode,
		gtax.Amount as GrossTaxPercent
	from PurchaseItem item
	left join [Option] as opt on item.OptionID = opt.OptionID
	left join Rate as rate on opt.RateID = rate.RateID
	left join [Service] as serv on rate.ServiceID = serv.ServiceID
	left join [Supplier] as sup on serv.SupplierID = sup.SupplierID
	left join ServiceType as stype on serv.ServiceTypeID = stype.ServiceTypeID	
	left join TaxType as ntaxSrv on ntaxSrv.TaxTypeID = serv.TaxTypeID
	left join TaxType as ntaxSup on ntaxSup.TaxTypeID = sup.TaxTypeID
	left join TaxType as ntaxTyp on ntaxTyp.TaxTypeID = stype.NetTaxTypeID
	left join TaxType as gtax on stype.GrossTaxTypeID = gtax.TaxTypeID
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
		TotalGross - TotalNet as Yield,
	    (TotalGross - TotalNet)/(case TotalGross when 0 then 1 else TotalGross end)  as Margin,
		TotalNet - (TotalNet*100/(100+NetTaxPercent)) as NetTaxAmount,
		TotalGross - (TotalGross*100/(100+GrossTaxPercent)) as GrossTaxAmount,
		NetTaxCode as NetTaxTypeCode,
		NetTaxPercent,
		GrossTaxCode as GrossTaxTypeCode,
		GrossTaxPercent		    	
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
PRINT N'Refreshing [dbo].[ContactDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ContactDetail';


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
PRINT N'Refreshing [dbo].[ItinerarySaleAllocationDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItinerarySaleAllocationDetail';


GO
PRINT N'Refreshing [dbo].[ItineraryPaymentDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryPaymentDetail';


GO
PRINT N'Refreshing [dbo].[SupplierDetail]...';


GO
EXECUTE sp_refreshview N'dbo.SupplierDetail';


GO
PRINT N'Refreshing [dbo].[ItineraryClientDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryClientDetail';


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
		pric.Net,
		pric.Gross,
		item.NumberOfDays,
		item.Quantity,
		pric.UnitMultiplier,
		opt.Net * UnitMultiplier as BaseTotalNet,
		opt.Gross * UnitMultiplier as BaseTotalGross,
		pric.TotalNet,
		pric.TotalGross,
		pric.Markup,
		pric.Commission,
		pric.Yield,
	    pric.Margin,
	    pric.NetTaxAmount,
	    pric.GrossTaxAmount,
		TotalNetExcl = TotalNet - NetTaxAmount,
		TotalGrossExcl = TotalGross - GrossTaxAmount,
		pric.TotalGrossOrig,
		pric.MarkupOrig,
		pric.CommissionOrig,
		stype.ServiceTypeID,
		stype.ServiceTypeName,
		stype.BookingStartName,
		stype.BookingEndName,
		stype.NumberOfDaysName,
		pric.NetTaxTypeCode, --ntax.TaxTypeCode as NetTaxTypeCode,
		pric.NetTaxPercent, --ntax.Amount as NetTaxPercent,
		stype.NetAccountingCategoryCode,
		pric.GrossTaxTypeCode, --stype.GrossTaxTypeCode,
		pric.GrossTaxPercent, --stype.GrossTaxPercent,
		stype.GrossAccountingCategoryCode,
		convert(varchar(3), PaymentTerm.DepositDuePeriod) + ' ' + depdue.PaymentDueName as DepositTerms,
		dbo.GetPaymentDate(item.StartDate, dbo.PaymentTerm.DepositDuePeriod, depdue.PaymentDueName) as DepositDueDate,
		PaymentTerm.DepositType,
		PaymentTerm.DepositAmount,
		convert(varchar(3), PaymentTerm.PaymentDuePeriod) + ' ' + baldue.PaymentDueName as BalanceTerms,
		dbo.GetPaymentDate(item.StartDate, PaymentTerm.PaymentDuePeriod, baldue.PaymentDueName) as BalanceDueDate
	from PurchaseItem item
	join dbo.PurchaseItemPricing() pric on item.PurchaseItemID = pric.PurchaseItemID
	left outer join PurchaseLine as line on line.PurchaseLineID = item.PurchaseLineID
	left outer join ItineraryDetail as itin on line.ItineraryID = itin.ItineraryID
	left outer join Itinerary as itin2 on line.ItineraryID = itin2.ItineraryID
	left outer join RequestStatus req on req.RequestStatusID = item.RequestStatusID
	left outer join [Option] as opt on item.OptionID = opt.OptionID
	left outer join Rate as rate on opt.RateID = rate.RateID
	left outer join [Service] AS serv on rate.ServiceID = serv.ServiceID
	left outer join SupplierDetail sup on serv.SupplierID = sup.SupplierID
	left outer join ServiceTypeDetail as stype on serv.ServiceTypeID = stype.ServiceTypeID
	left outer join PaymentDue as baldue
	right outer join PaymentTerm on baldue.PaymentDueID = PaymentTerm.PaymentDueID
	left outer join PaymentDue as depdue on PaymentTerm.DepositDueID = depdue.PaymentDueID on item.PaymentTermID = PaymentTerm.PaymentTermID
	left outer join Currency itemCcy on itemCcy.CurrencyCode = serv.CurrencyCode COLLATE DATABASE_DEFAULT
	left outer join Currency sysCcy on sysCcy.CurrencyCode = (select top(1) CurrencyCode from AppSettings) COLLATE DATABASE_DEFAULT
GO
PRINT N'Refreshing [dbo].[PurchaseItemPaymentsDetail]...';


GO
EXECUTE sp_refreshview N'dbo.PurchaseItemPaymentsDetail';


GO
PRINT N'Refreshing [dbo].[SupplierRatesDetail]...';


GO
EXECUTE sp_refreshview N'dbo.SupplierRatesDetail';


GO
----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2011.02.28'
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
