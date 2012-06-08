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
if ((select VersionNumber from AppSettings) <> '2012.05.21')
	RAISERROR (N'Database Update Script is not correct version for current database version',17,1)

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------
GO

if not Exists(select * from sys.columns where Name = N'IsInvoiced' and Object_ID = Object_ID(N'PurchaseItem'))
	ALTER TABLE [dbo].[PurchaseItem]
		ADD [IsInvoiced] bit NULL;
GO

if not Exists(select * from sys.columns where Name = N'Title' and Object_ID = Object_ID(N'ItineraryMember'))
	ALTER TABLE [dbo].[ItineraryMember]
		ADD [Title] varchar(50) NULL;
GO

if not Exists(select * from Permission where PermissionName = 'Itinerary - set invoiced status')
	insert Permission (PermissionName) values ('Itinerary - set invoiced status')

GO


------------------------------------------

GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PurchaseItem_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[PurchaseItem_Ins]
GO

CREATE PROCEDURE [dbo].[PurchaseItem_Ins]
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
	@DiscountUnits decimal(12, 4),
	@DiscountType varchar(10),
	@IsInvoiced bit,
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
	[AddedBy],
	[DiscountUnits],
	[DiscountType],
	[IsInvoiced]
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
	@AddedBy,
	@DiscountUnits,
	@DiscountType,
	@IsInvoiced
)
SELECT @PurchaseItemID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PurchaseItem_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[PurchaseItem_Upd]
GO

CREATE PROCEDURE [dbo].[PurchaseItem_Upd]
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
	@RowVersion timestamp,
	@DiscountUnits decimal(12, 4),
	@DiscountType varchar(10),
	@IsInvoiced bit
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
	[AddedBy] = @AddedBy,
	[DiscountUnits] = @DiscountUnits,
	[DiscountType] = @DiscountType,
	[IsInvoiced] = @IsInvoiced
WHERE
	[PurchaseItemID] = @PurchaseItemID
	AND [RowVersion] = @RowVersion
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PurchaseItem_Upd_ByPurchaseLineID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[PurchaseItem_Upd_ByPurchaseLineID]
GO

CREATE PROCEDURE [dbo].[PurchaseItem_Upd_ByPurchaseLineID]
	@PurchaseLineID int,
	@PurchaseLineIDOld int
AS
UPDATE [dbo].[PurchaseItem]
SET
	[PurchaseLineID] = @PurchaseLineID
WHERE
	[PurchaseLineID] = @PurchaseLineIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PurchaseItem_Upd_ByOptionID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[PurchaseItem_Upd_ByOptionID]
GO

CREATE PROCEDURE [dbo].[PurchaseItem_Upd_ByOptionID]
	@OptionID int,
	@OptionIDOld int
AS
UPDATE [dbo].[PurchaseItem]
SET
	[OptionID] = @OptionID
WHERE
	[OptionID] = @OptionIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PurchaseItem_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[PurchaseItem_Del]
GO

CREATE PROCEDURE [dbo].[PurchaseItem_Del]
	@PurchaseItemID int,
	@RowVersion timestamp
AS
DELETE FROM [dbo].[PurchaseItem]
WHERE
	[PurchaseItemID] = @PurchaseItemID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PurchaseItem_Del_ByPurchaseLineID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[PurchaseItem_Del_ByPurchaseLineID]
GO

CREATE PROCEDURE [dbo].[PurchaseItem_Del_ByPurchaseLineID]
	@PurchaseLineID int
AS
DELETE
FROM [dbo].[PurchaseItem]
WHERE
	[PurchaseLineID] = @PurchaseLineID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PurchaseItem_Del_ByOptionID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[PurchaseItem_Del_ByOptionID]
GO

CREATE PROCEDURE [dbo].[PurchaseItem_Del_ByOptionID]
	@OptionID int
AS
DELETE
FROM [dbo].[PurchaseItem]
WHERE
	[OptionID] = @OptionID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PurchaseItem_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[PurchaseItem_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[PurchaseItem_Sel_ByID]
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
	[DiscountUnits],
	[DiscountType],
	[IsInvoiced]
FROM [dbo].[PurchaseItem]
WHERE
	[PurchaseItemID] = @PurchaseItemID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PurchaseItem_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[PurchaseItem_Sel_All]
GO

CREATE PROCEDURE [dbo].[PurchaseItem_Sel_All]
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
	[DiscountUnits],
	[DiscountType],
	[IsInvoiced]
FROM [dbo].[PurchaseItem]
ORDER BY 
	[PurchaseItemID] ASC
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PurchaseItem_Sel_ByPurchaseLineID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[PurchaseItem_Sel_ByPurchaseLineID]
GO

CREATE PROCEDURE [dbo].[PurchaseItem_Sel_ByPurchaseLineID]
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
	[DiscountUnits],
	[DiscountType],
	[IsInvoiced]
FROM [dbo].[PurchaseItem]
WHERE
	[PurchaseLineID] = @PurchaseLineID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[PurchaseItem_Sel_ByOptionID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[PurchaseItem_Sel_ByOptionID]
GO

CREATE PROCEDURE [dbo].[PurchaseItem_Sel_ByOptionID]
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
	[DiscountUnits],
	[DiscountType],
	[IsInvoiced]
FROM [dbo].[PurchaseItem]
WHERE
	[OptionID] = @OptionID
GO

------------------------------------------

GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryMember_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryMember_Ins]
GO

CREATE PROCEDURE [dbo].[ItineraryMember_Ins]
	@ItineraryGroupID int,
	@ItineraryMemberName varchar(100),
	@ContactID int,
	@AgeGroupID int,
	@Age int,
	@ArriveDate datetime,
	@ArriveCityID int,
	@ArriveFlight varchar(50),
	@DepartDate datetime,
	@DepartCityID int,
	@DepartFlight varchar(50),
	@IsDefaultContact bit,
	@IsDefaultBilling bit,
	@Comments varchar(500),
	@AddedOn datetime,
	@AddedBy int,
	@Title varchar(50),
	@ItineraryMemberID int OUTPUT
AS
INSERT [dbo].[ItineraryMember]
(
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
	[Title]
)
VALUES
(
	@ItineraryGroupID,
	@ItineraryMemberName,
	@ContactID,
	@AgeGroupID,
	@Age,
	@ArriveDate,
	@ArriveCityID,
	@ArriveFlight,
	@DepartDate,
	@DepartCityID,
	@DepartFlight,
	@IsDefaultContact,
	@IsDefaultBilling,
	@Comments,
	@AddedOn,
	@AddedBy,
	@Title
)
SELECT @ItineraryMemberID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryMember_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryMember_Upd]
GO

CREATE PROCEDURE [dbo].[ItineraryMember_Upd]
	@ItineraryMemberID int,
	@ItineraryGroupID int,
	@ItineraryMemberName varchar(100),
	@ContactID int,
	@AgeGroupID int,
	@Age int,
	@ArriveDate datetime,
	@ArriveCityID int,
	@ArriveFlight varchar(50),
	@DepartDate datetime,
	@DepartCityID int,
	@DepartFlight varchar(50),
	@IsDefaultContact bit,
	@IsDefaultBilling bit,
	@Comments varchar(500),
	@AddedOn datetime,
	@AddedBy int,
	@RowVersion timestamp,
	@Title varchar(50)
AS
UPDATE [dbo].[ItineraryMember]
SET 
	[ItineraryGroupID] = @ItineraryGroupID,
	[ItineraryMemberName] = @ItineraryMemberName,
	[ContactID] = @ContactID,
	[AgeGroupID] = @AgeGroupID,
	[Age] = @Age,
	[ArriveDate] = @ArriveDate,
	[ArriveCityID] = @ArriveCityID,
	[ArriveFlight] = @ArriveFlight,
	[DepartDate] = @DepartDate,
	[DepartCityID] = @DepartCityID,
	[DepartFlight] = @DepartFlight,
	[IsDefaultContact] = @IsDefaultContact,
	[IsDefaultBilling] = @IsDefaultBilling,
	[Comments] = @Comments,
	[AddedOn] = @AddedOn,
	[AddedBy] = @AddedBy,
	[Title] = @Title
WHERE
	[ItineraryMemberID] = @ItineraryMemberID
	AND [RowVersion] = @RowVersion
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryMember_Upd_ByItineraryGroupID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryMember_Upd_ByItineraryGroupID]
GO

CREATE PROCEDURE [dbo].[ItineraryMember_Upd_ByItineraryGroupID]
	@ItineraryGroupID int,
	@ItineraryGroupIDOld int
AS
UPDATE [dbo].[ItineraryMember]
SET
	[ItineraryGroupID] = @ItineraryGroupID
WHERE
	[ItineraryGroupID] = @ItineraryGroupIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryMember_Upd_ByContactID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryMember_Upd_ByContactID]
GO

CREATE PROCEDURE [dbo].[ItineraryMember_Upd_ByContactID]
	@ContactID int,
	@ContactIDOld int
AS
UPDATE [dbo].[ItineraryMember]
SET
	[ContactID] = @ContactID
WHERE
	[ContactID] = @ContactIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryMember_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryMember_Del]
GO

CREATE PROCEDURE [dbo].[ItineraryMember_Del]
	@ItineraryMemberID int,
	@RowVersion timestamp
AS
DELETE FROM [dbo].[ItineraryMember]
WHERE
	[ItineraryMemberID] = @ItineraryMemberID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryMember_Del_ByItineraryGroupID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryMember_Del_ByItineraryGroupID]
GO

CREATE PROCEDURE [dbo].[ItineraryMember_Del_ByItineraryGroupID]
	@ItineraryGroupID int
AS
DELETE
FROM [dbo].[ItineraryMember]
WHERE
	[ItineraryGroupID] = @ItineraryGroupID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryMember_Del_ByContactID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryMember_Del_ByContactID]
GO

CREATE PROCEDURE [dbo].[ItineraryMember_Del_ByContactID]
	@ContactID int
AS
DELETE
FROM [dbo].[ItineraryMember]
WHERE
	[ContactID] = @ContactID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryMember_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryMember_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[ItineraryMember_Sel_ByID]
	@ItineraryMemberID int
AS
SET NOCOUNT ON
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
	[RowVersion],
	[Title]
FROM [dbo].[ItineraryMember]
WHERE
	[ItineraryMemberID] = @ItineraryMemberID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryMember_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryMember_Sel_All]
GO

CREATE PROCEDURE [dbo].[ItineraryMember_Sel_All]
AS
SET NOCOUNT ON
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
	[RowVersion],
	[Title]
FROM [dbo].[ItineraryMember]
ORDER BY 
	[ItineraryMemberID] ASC
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryMember_Sel_ByItineraryGroupID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryMember_Sel_ByItineraryGroupID]
GO

CREATE PROCEDURE [dbo].[ItineraryMember_Sel_ByItineraryGroupID]
	@ItineraryGroupID int
AS
SET NOCOUNT ON
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
	[RowVersion],
	[Title]
FROM [dbo].[ItineraryMember]
WHERE
	[ItineraryGroupID] = @ItineraryGroupID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryMember_Sel_ByContactID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryMember_Sel_ByContactID]
GO


CREATE PROCEDURE [dbo].[ItineraryMember_Sel_ByContactID]
	@ContactID int
AS
SET NOCOUNT ON
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
	[RowVersion],
	[Title]
FROM [dbo].[ItineraryMember]
WHERE
	[ContactID] = @ContactID
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
	[CurrencyCode],
	[NetMinOrMax],
	[AgentContactID]
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
	[StaffRooms],
	[GrossMarkup],
	[GrossOverride]
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
	i.[DiscountUnits],
	i.[DiscountType],
	i.[IsInvoiced],
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
	[RowVersion],
	[Title]
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

-- Discounts --

SELECT
	[DiscountID],
	[ServiceID],
	[UnitsUsed],
	[UnitsFree],
	[DiscountType]
FROM [dbo].[Discount]
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

-- ServiceWarnings
SELECT [ServiceWarningID]
		,[ServiceID]
		,[Description]
		,[ValidFrom]
		,[ValidTo]
	FROM [ServiceWarning]
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
AND ValidTo > getdate()
GO

GO

ALTER VIEW [dbo].[ItineraryClientDetail]
AS
select
	ItineraryMemberName,
	mem.Title as ItineraryMemberTitle,	
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
		serv.ServiceID,
		serv.ServiceName,
		serv.CheckinMinutesEarly,
		rate.RateID,
		rate.ValidFrom AS RateValidFrom,
		rate.ValidTo AS RateValidTo,
		opt.OptionID,
		opt.OptionName,
		opt.PricingOption,
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
		NetBaseTotal - NetBaseTotalTaxAmount as NetBaseTotalExcl,
		GrossFinalTotal - GrossFinalTotalTaxAmount as GrossFinalTotalExcl,
		stype.ServiceTypeID,
		stype.ServiceTypeName,
		stype.BookingStartName,
		stype.BookingEndName,
		stype.NumberOfDaysName,
		item.IsLockedAccounting,
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
		dbo.GetPaymentDate(item.StartDate, PaymentTerm.PaymentDuePeriod, baldue.PaymentDueName) as BalanceDueDate,
		item.IsInvoiced
		
		----/**** only for backwards compatibility ****/
		--,pric.NetBaseUnit as Net
		--,pric.GrossBaseUnit as Gross
		--,pric.NetFinalTotal as TotalNet
		--,pric.GrossFinalTotal as TotalGross
		--,pric.GrossBaseUnit * pric.UnitMultiplier * pric.CurrencyRate as TotalGrossOrig
		--,NetBaseTotal - NetBaseTotalTaxAmount as TotalNetExcl
		----/******************************************/
		
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


--===========================

GO
PRINT N'Altering [dbo].[ServiceWarning]...';


GO
ALTER TABLE [dbo].[ServiceWarning] ALTER COLUMN [Description] VARCHAR (8000) NULL;


PRINT N'Altering [dbo].[ServiceWarning_Ins]...';


GO

ALTER PROCEDURE [dbo].[ServiceWarning_Ins]
	@ServiceID int,
	@Description varchar(8000),
	@ValidFrom datetime,
	@ValidTo datetime,
	@ServiceWarningID int OUTPUT
AS
INSERT [dbo].[ServiceWarning]
(
	[ServiceID],
	[Description],
	[ValidFrom],
	[ValidTo]
)
VALUES
(
	@ServiceID,
	@Description,
	@ValidFrom,
	@ValidTo
)
SELECT @ServiceWarningID=SCOPE_IDENTITY()
GO
PRINT N'Altering [dbo].[ServiceWarning_Upd]...';


GO

ALTER PROCEDURE [dbo].[ServiceWarning_Upd]
	@ServiceWarningID int,
	@ServiceID int,
	@Description varchar(8000),
	@ValidFrom datetime,
	@ValidTo datetime
AS
UPDATE [dbo].[ServiceWarning]
SET 
	[ServiceID] = @ServiceID,
	[Description] = @Description,
	[ValidFrom] = @ValidFrom,
	[ValidTo] = @ValidTo
WHERE
	[ServiceWarningID] = @ServiceWarningID
GO


exec __RefreshViews;
GO
----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2012.06.08'
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