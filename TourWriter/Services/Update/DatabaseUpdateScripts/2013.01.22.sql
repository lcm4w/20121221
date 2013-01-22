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
if ((select VersionNumber from AppSettings) <> '2012.11.18' and (select VersionNumber from AppSettings) <> '2013.01.10')
	RAISERROR (N'Database Update Script is not correct version for current database version',17,1)

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------
GO
ALTER PROCEDURE  [dbo].[_ItinerarySet_Copy_ByID]
(
	@OrigItineraryID int,
    @NewItineraryName varchar(100),
    @ParentFolderID int,
    @ClientDataOption Bit,
    @TaskDataOption Bit,
    @AddedOn datetime,
    @AddedBy int
)
AS
SET NOCOUNT ON

DECLARE @NewItineraryID int

-- Itinerary ----------------------------------------------
PRINT 'Processing Itinerary...'
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
	[NetMinOrMax],
	[AgentContactID]
)
SELECT
    @NewItineraryName,
	DisplayName,
	CustomCode,
	ArriveDate,
	ArriveCityID,
	ArriveFlight,
	ArriveNote,
	DepartDate,
	DepartCityID,
	DepartFlight,
	DepartNote,
	NetComOrMup,
	NetMargin,
	GrossMarkup,
	GrossOverride,
	IsLockedGrossOverride,
	PricingNote,
	AgentID,
	PaymentTermID,
	ItineraryStatusID,
	ItinerarySourceID,
	CountryID,
	AssignedTo,
	DepartmentID,
	BranchID,
	PaxOverride,
	Comments,
	IsRecordActive,
	IsReadOnly,
	@ParentFolderID,
	@AddedOn,
	@AddedBy,
	IsDeleted,
	CurrencyCode,
	NetMinOrMax,
	AgentContactID
FROM Itinerary WHERE ItineraryID = @OrigItineraryID

SELECT @NewItineraryID=SCOPE_IDENTITY()

-- ItineraryMarginOverride ----------------------------------------------
PRINT 'Processing ItineraryMarginOverride...'
INSERT [dbo].[ItineraryMarginOverride]
(
    [ItineraryID],
    [ServiceTypeID],
    [Margin]
)
SELECT
    @NewItineraryID,
    ServiceTypeID,
    Margin
FROM ItineraryMarginOverride WHERE ItineraryID = @OrigItineraryID

-- ItineraryPubFile ----------------------------------------------
PRINT 'Processing ItineraryPubFile...'
INSERT [dbo].[ItineraryPubFile]
(
	[ItineraryPubFileName],
	[ItineraryID],
	[DayTemplateFile],
	[Layout],
	[AddedOn],
	[AddedBy]
)
SELECT
	ItineraryPubFileName,
	@NewItineraryID,
	DayTemplateFile,
	Layout,
	@AddedOn,
	@AddedBy
FROM [dbo].[ItineraryPubFile] WHERE [ItineraryID] = @OrigItineraryID


--=== Loop purchase lines ======================================
DECLARE @NewPurchaseLineID int
DECLARE @OrigPurchaseLineID int
DECLARE @OrigPurchaseItemID int
DECLARE @NewPurchaseItemID int
DECLARE @PurchaseItemTable Table(OldPurchaseItemID int, NewPurchaseItemID int)
DECLARE PurchaseLineCursor CURSOR FOR 
    SELECT PurchaseLineID FROM PurchaseLine WHERE ItineraryID = @OrigItineraryID

OPEN PurchaseLineCursor
FETCH NEXT FROM PurchaseLineCursor INTO @OrigPurchaseLineID

PRINT 'Processing PurchaseLineCursor...'
WHILE @@FETCH_STATUS = 0
BEGIN -- PurchaseLineCursor 
    
    INSERT [dbo].[PurchaseLine]
    (
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
		[AddedBy]
    )
    SELECT
        @NewItineraryID,
		SupplierID,
		PurchaseLineName,
		Comments,
		NoteToSupplier,
		NoteToVoucher,
		NoteToClient,
		Approved,
		SupplierReference,
		@AddedOn,
		@AddedBy
    FROM PurchaseLine WHERE PurchaseLineID = @OrigPurchaseLineID
    
    SELECT @NewPurchaseLineID=SCOPE_IDENTITY()

    -- PurchaseItem -----------------------------------------------------------------
    PRINT 'Processing PurchaseItemCursor...'
	DECLARE PurchaseItemCursor CURSOR FOR 
		SELECT PurchaseItemID FROM PurchaseItem WHERE PurchaseLineID = @OrigPurchaseLineID

	OPEN PurchaseItemCursor
	FETCH NEXT FROM PurchaseItemCursor INTO @OrigPurchaseItemID

	WHILE @@FETCH_STATUS = 0
	BEGIN -- PurchaseItemCursor 

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
			[IsInvoiced],
			[SortDate]
	    )
		SELECT
			@NewPurchaseLineID,
			OptionID,
			PurchaseItemName,
			BookingReference,
			StartDate,
			StartTime,
			EndDate,
			EndTime,
			Net,
			Gross,
			CurrencyRate,
			Quantity,
			NumberOfDays,
			PaymentTermID,
			RequestStatusID,
			IsLockedAccounting,
			@AddedOn,
			@AddedBy,
			DiscountUnits,
			DiscountType,
			IsInvoiced,
			SortDate
		FROM PurchaseItem WHERE PurchaseLineID = @OrigPurchaseLineID and PurchaseItemID = @OrigPurchaseItemID

		SELECT @NewPurchaseItemID=SCOPE_IDENTITY()

		INSERT INTO @PurchaseItemTable (OldPurchaseItemID, NewPurchaseItemID) Values (@OrigPurchaseItemID, @NewPurchaseItemID)

		-- Purchase Item Note --------------------------------------------------
		PRINT 'Processing PurchaseItemNote...'
		INSERT [dbo].PurchaseItemNote
		(
			[PurchaseItemID],
			[FlagID],
			[Note]
		)	
		SELECT
			PurchaseItemID,
			FlagID,
			Note
		FROM PurchaseItemNote WHERE PurchaseItemID = @OrigPurchaseItemID
		
		FETCH NEXT FROM PurchaseItemCursor INTO @OrigPurchaseItemID
	END --PurchaseItemCursor
	CLOSE PurchaseItemCursor
	DEALLOCATE PurchaseItemCursor

    FETCH NEXT FROM PurchaseLineCursor INTO @OrigPurchaseLineID

END -- PurchaseLineCursor
CLOSE PurchaseLineCursor
DEALLOCATE PurchaseLineCursor


--=== Loop Itinerary Group ======================================
IF @ClientDataOption = 1 
BEGIN
	DECLARE @NewItineraryGroupID int
	DECLARE @OrigItineraryGroupID int
	DECLARE @NewItineraryMemberID int
	DECLARE @OrigItineraryMemberID int
	DECLARE GroupCursor CURSOR FOR 
		SELECT ItineraryGroupID FROM ItineraryGroup WHERE ItineraryID = @OrigItineraryID

	PRINT 'Processing GroupCursor...'
	OPEN GroupCursor
	FETCH NEXT FROM GroupCursor INTO @OrigItineraryGroupID

	WHILE @@FETCH_STATUS = 0
	BEGIN -- GroupCursor 
	    
		INSERT [dbo].[ItineraryGroup] 
		(
			[ItineraryID],
			[ItineraryGroupName],
			[Comments],
			[NoteToClient],
			[NoteToSupplier],
			[CurrencyCode],
			[CurrencyRate],
			[AddedOn],
			[AddedBy]
		)
		SELECT
			@NewItineraryID,
			ItineraryGroupName,
			Comments,
			NoteToClient,
			NoteToSupplier,
			CurrencyCode,
			CurrencyRate,
			@AddedOn,
			@AddedBy
		FROM ItineraryGroup WHERE ItineraryID = @OrigItineraryID 
	    
		SELECT @NewItineraryGroupID=SCOPE_IDENTITY()

		-- ItineraryMember -----------------------------------------------------------------
		PRINT 'Processing ItineraryMember...'
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
			[Title],
			[RoomTypeID],
			[AgentID],
			[PriceOverride],
			[RoomName]
		)
		SELECT
			@NewItineraryGroupID,
			ItineraryMemberName,
			ContactID,
			AgeGroupID,
			Age,
			ArriveDate,
			ArriveCityID,
			ArriveFlight,
			DepartDate,
			DepartCityID,
			DepartFlight,
			IsDefaultContact,
			IsDefaultBilling,
			Comments,
			@AddedOn,
			@AddedBy,
			Title,
			RoomTypeID,
			AgentID,
			PriceOverride,
			RoomName
		FROM ItineraryMember WHERE ItineraryGroupID = @OrigItineraryGroupID

		SELECT @NewItineraryMemberID=SCOPE_IDENTITY()

		-- Itinerary Payments -----------------------------------------------------------------
		PRINT 'Processing ItineraryPayment...'
		DECLARE MemberCursor CURSOR FOR 
			SELECT ItineraryMemberID FROM ItineraryMember WHERE ItineraryGroupID = @OrigItineraryGroupID

		OPEN MemberCursor
		FETCH NEXT FROM MemberCursor INTO @OrigItineraryMemberID

		WHILE @@FETCH_STATUS = 0
		BEGIN -- MemberCursor 
			INSERT [dbo].[ItineraryPayment]
			(
				[ItineraryMemberID],
				[Comments],
				[Amount],
				[PaymentDate],
				[PaymentTypeID],
				[ItinerarySaleID],
				[IsLockedAccounting]
			)
			SELECT
				@NewItineraryMemberID,
				Comments,
				Amount,
				PaymentDate,
				PaymentTypeID,
				ItinerarySaleID,
				IsLockedAccounting
			FROM ItineraryPayment WHERE ItineraryMemberID = @OrigItineraryMemberID

			FETCH NEXT FROM MemberCursor INTO @OrigItineraryMemberID
		END -- MemberCursor
		CLOSE MemberCursor
		DEALLOCATE MemberCursor
	    
		FETCH NEXT FROM GroupCursor INTO @OrigItineraryGroupID

	END -- GroupCursor
	CLOSE GroupCursor
	DEALLOCATE GroupCursor
END

	
--=== Itinerary Pax ======================================
DECLARE @NewItineraryPaxID int
DECLARE @OrigItineraryPaxID int

DECLARE PaxCursor CURSOR FOR 
	SELECT ItineraryPaxID FROM ItineraryPax WHERE ItineraryID = @OrigItineraryID

PRINT 'Processing ItineraryPax...'
OPEN PaxCursor
FETCH NEXT FROM PaxCursor INTO @OrigItineraryPaxID

WHILE @@FETCH_STATUS = 0
BEGIN -- PaxCursor 
	    
	INSERT [dbo].[ItineraryPax]  
	(
		[ItineraryPaxName],
		[ItineraryID],
		[MemberCount],
		[MemberRooms],
		[StaffCount],
		[StaffRooms],
		[GrossMarkup],
		[GrossOverride]
	)
	SELECT
		ItineraryPaxName,
		@NewItineraryID,
		MemberCount,
		MemberRooms,
		StaffCount,
		StaffRooms,
		GrossMarkup,
		GrossOverride
	FROM ItineraryPax WHERE ItineraryID = @OrigItineraryID and ItineraryPaxID = @OrigItineraryPaxID

	SELECT @NewItineraryPaxID=SCOPE_IDENTITY()

	DECLARE PaxOverrideCursor CURSOR FOR 
		SELECT PurchaseItemID FROM ItineraryPaxOverride WHERE ItineraryPaxID = @OrigItineraryPaxID

	PRINT 'Processing ItineraryPaxOverride...'
	OPEN PaxOverrideCursor
	FETCH NEXT FROM PaxOverrideCursor INTO @OrigPurchaseItemID

	WHILE @@FETCH_STATUS = 0
	BEGIN -- PaxOverrideCursor 

		-- Itinerary Pax Override -----------------------------------------------------------------
		
		SELECT @NewPurchaseItemID = NewPurchaseItemID FROM @PurchaseItemTable WHERE OldPurchaseItemID = @OrigPurchaseItemID

		INSERT [dbo].[ItineraryPaxOverride] 
		(
			[PurchaseItemID],
			[ItineraryPaxID],
			[MemberCount],
			[MemberRooms],
			[StaffCount],
			[StaffRooms]
		)
		SELECT
			@NewPurchaseItemID,
			@NewItineraryPaxID,
			MemberCount,
			MemberRooms,
			StaffCount,
			StaffRooms
		FROM ItineraryPaxOverride  WHERE ItineraryPaxID = @OrigItineraryPaxID and PurchaseItemID = @OrigPurchaseItemID

		FETCH NEXT FROM PaxOverrideCursor INTO @OrigPurchaseItemID

	END -- PaxOverrideCursor
	CLOSE PaxOverrideCursor
	DEALLOCATE PaxOverrideCursor

	FETCH NEXT FROM PaxCursor INTO @OrigItineraryPaxID

END -- PaxCursor
CLOSE PaxCursor
DEALLOCATE PaxCursor


--=== Loop Itinerary Sale ======================================
DECLARE @NewItinerarySaleID int
DECLARE @OrigItinerarySaleID int
DECLARE SaleCursor CURSOR FOR 
    SELECT ItinerarySaleID FROM ItinerarySale WHERE ItineraryID = @OrigItineraryID

PRINT 'Processing ItinerarySale...'
OPEN SaleCursor
FETCH NEXT FROM SaleCursor INTO @OrigItinerarySaleID

WHILE @@FETCH_STATUS = 0
BEGIN -- SaleCursor 
    
    INSERT [dbo].[ItinerarySale]  
    (
		[ItineraryID],
		[Comments],
		[SaleDate],
		[IsLockedAccounting]
    )
    SELECT
        @NewItineraryID,
		Comments,
		SaleDate,
		IsLockedAccounting
    FROM ItinerarySale WHERE ItineraryID = @OrigItineraryID 
    
    SELECT @NewItinerarySaleID=SCOPE_IDENTITY()

    -- Itinerary Sale Allocation -----------------------------------------------------------------
	PRINT 'Processing ItinerarySaleAllocation...'
    INSERT [dbo].[ItinerarySaleAllocation] 
    (
		[ItinerarySaleID],
		[ServiceTypeID],
		[Amount]
	)
    SELECT
        @NewItinerarySaleID,
		ServiceTypeID,
		Amount
    FROM ItinerarySaleAllocation WHERE ItinerarySaleID = @OrigItinerarySaleID 

	FETCH NEXT FROM SaleCursor INTO @OrigItinerarySaleID

END -- SaleCursor
CLOSE SaleCursor
DEALLOCATE SaleCursor


--=== Loop Message ======================================
DECLARE @NewMessageID int
DECLARE @OrigMessageID int
DECLARE MessageCursor CURSOR FOR 
    SELECT MessageID FROM ItineraryMessage WHERE ItineraryID = @OrigItineraryID

PRINT 'Processing Message...'
OPEN MessageCursor
FETCH NEXT FROM MessageCursor INTO @OrigMessageID

WHILE @@FETCH_STATUS = 0
BEGIN -- MessageCursor 

    INSERT [dbo].[Message] 
    (
		[MessageName],
		[MessageType],
		[MessageTo],
		[MessageFrom],
		[MessageFile],
		[MessageDate],
		[Description],
		[AddedOn],
		[AddedBy]
	)
    SELECT
		MessageName,
		MessageType,
		MessageTo,
		MessageFrom,
		MessageFile,
		MessageDate,
		Description,
		AddedOn,
		AddedBy
    FROM Message WHERE MessageID = @OrigMessageID

    SELECT @NewMessageID = SCOPE_IDENTITY()

    -- Itinerary Message table -----------------------------------------------------------------
	PRINT 'Processing ItineraryMessage...'
	INSERT [dbo].[ItineraryMessage]
	(
		[ItineraryID],
		[MessageID],
		[AddedOn],
		[AddedBy]
	)
	SELECT
		@NewItineraryID,
		@NewMessageID,
		@AddedOn,
		@AddedBy
	FROM [dbo].[ItineraryMessage] WHERE [MessageID] = @OrigMessageID
    
	FETCH NEXT FROM MessageCursor INTO @OrigMessageID

END -- MessageCursor
CLOSE MessageCursor
DEALLOCATE MessageCursor


--=== Loop Task ======================================
IF @TaskDataOption = 1
BEGIN
	DECLARE @NewTaskID int
	DECLARE @OrigTaskID int
	DECLARE TaskCursor CURSOR FOR 
		SELECT TaskID FROM ItineraryTask WHERE ItineraryID = @OrigItineraryID

	PRINT 'Processing Task...'
	OPEN TaskCursor
	FETCH NEXT FROM TaskCursor INTO @OrigTaskID

	WHILE @@FETCH_STATUS = 0
	BEGIN -- TaskCursor 

		INSERT [dbo].[Task] 
		(
			[TaskName],
			[DateDue],
			[DateCompleted],
			[Note]
		)
		SELECT
			TaskName,
			DateDue,
			DateCompleted,
			Note
		FROM Task WHERE TaskID = @OrigTaskID

		SELECT @NewTaskID = SCOPE_IDENTITY()

		-- Itinerary Task table -----------------------------------------------------------------
		PRINT 'Processing ItineraryTask...'
		INSERT [dbo].[ItineraryTask]
		(
			[ItineraryID],
			[TaskID]
		)
		SELECT
			@NewItineraryID,
			@NewTaskID
		FROM [dbo].[ItineraryTask] WHERE [TaskID] = @OrigTaskID 
	    
		FETCH NEXT FROM TaskCursor INTO @OrigTaskID

	END -- TaskCursor
	CLOSE TaskCursor
	DEALLOCATE TaskCursor
END

-- Room Type ----------------------------------------------

PRINT 'Processing Room Type...'
INSERT [dbo].[RoomType]
(
	[ItineraryID],
	[OptionTypeID],
	[RoomTypeName],
	[Quantity]
)
SELECT
	@NewItineraryID,
	OptionTypeID,
	RoomTypeName,
	Quantity
FROM RoomType WHERE ItineraryID = @OrigItineraryID

-- done --------------------------------------------------
-- return new itinerary id
SELECT @NewItineraryID
GO
-- =======================================================================================================================================

GO
if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Invoice]') and OBJECTPROPERTY(id, N'IsTable') = 1)
begin
CREATE TABLE [dbo].[Invoice] (
    [InvoiceID]        INT           IDENTITY (1, 1) NOT NULL,
    [PurchaseLineID]   INT           NOT NULL,
    [Filename]         VARCHAR (150) NOT NULL,
    [OriginalFilename] VARCHAR (150) NULL,
    [Amount]           MONEY         NULL,
    [Validated]        BIT           NULL,
    [CreatedOn]        DATETIME      NOT NULL,
    CONSTRAINT [PK_Invoice] PRIMARY KEY CLUSTERED ([InvoiceID] ASC),
    CONSTRAINT [FK_Invoice_PurchaseLine] FOREIGN KEY ([PurchaseLineID]) REFERENCES [dbo].[PurchaseLine] ([PurchaseLineID]) ON DELETE CASCADE ON UPDATE CASCADE
);
end

GO

if not Exists(select * from sys.columns where Name = N'ClientCode' and Object_ID = Object_ID(N'AppSettings'))
ALTER TABLE [dbo].[AppSettings]
    ADD [ClientCode] VARCHAR (20) NULL;
		
GO

SET NOCOUNT ON
GO
USE [TourWriterDev]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AppSettings_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AppSettings_Ins]
GO

CREATE PROCEDURE [dbo].[AppSettings_Ins]
	@InstallID uniqueidentifier,
	@InstallName varchar(255),
	@VersionNumber varchar(100),
	@CurrencyCode char(3),
	@SupportEmail varchar(255),
	@SupportPhone varchar(50),
	@SmtpServerName varchar(255),
	@SmtpServerPort int,
	@SmtpServerUsername varchar(255),
	@SmtpServerPassword varchar(255),
	@SmtpServerEnableSsl bit,
	@LastDbBackupDate datetime,
	@LastDbBackupFile varchar(500),
	@LastDbBackupName varchar(500),
	@ExternalFilesPath varchar(500),
	@CancelledRequestStatusID int,
	@AccountingSystem varchar(50),
	@CustomCodeFormat varchar(50),
	@LanguageCode varchar(10),
	@CcyRateSource varchar(10),
	@CcyDatePoint varchar(10),
	@ClientCode varchar(20),
	@AppSettingsID int OUTPUT
AS
INSERT [dbo].[AppSettings]
(
	[InstallID],
	[InstallName],
	[VersionNumber],
	[CurrencyCode],
	[SupportEmail],
	[SupportPhone],
	[SmtpServerName],
	[SmtpServerPort],
	[SmtpServerUsername],
	[SmtpServerPassword],
	[SmtpServerEnableSsl],
	[LastDbBackupDate],
	[LastDbBackupFile],
	[LastDbBackupName],
	[ExternalFilesPath],
	[CancelledRequestStatusID],
	[AccountingSystem],
	[CustomCodeFormat],
	[LanguageCode],
	[CcyRateSource],
	[CcyDatePoint],
	[ClientCode]
)
VALUES
(
	ISNULL(@InstallID, (newid())),
	@InstallName,
	@VersionNumber,
	@CurrencyCode,
	@SupportEmail,
	@SupportPhone,
	@SmtpServerName,
	@SmtpServerPort,
	@SmtpServerUsername,
	@SmtpServerPassword,
	@SmtpServerEnableSsl,
	@LastDbBackupDate,
	@LastDbBackupFile,
	@LastDbBackupName,
	@ExternalFilesPath,
	@CancelledRequestStatusID,
	@AccountingSystem,
	@CustomCodeFormat,
	@LanguageCode,
	@CcyRateSource,
	@CcyDatePoint,
	@ClientCode
)
SELECT @AppSettingsID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AppSettings_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AppSettings_Upd]
GO

CREATE PROCEDURE [dbo].[AppSettings_Upd]
	@AppSettingsID int,
	@InstallID uniqueidentifier,
	@InstallName varchar(255),
	@VersionNumber varchar(100),
	@CurrencyCode char(3),
	@SupportEmail varchar(255),
	@SupportPhone varchar(50),
	@SmtpServerName varchar(255),
	@SmtpServerPort int,
	@SmtpServerUsername varchar(255),
	@SmtpServerPassword varchar(255),
	@SmtpServerEnableSsl bit,
	@LastDbBackupDate datetime,
	@LastDbBackupFile varchar(500),
	@LastDbBackupName varchar(500),
	@ExternalFilesPath varchar(500),
	@CancelledRequestStatusID int,
	@RowVersion timestamp,
	@AccountingSystem varchar(50),
	@CustomCodeFormat varchar(50),
	@LanguageCode varchar(10),
	@CcyRateSource varchar(10),
	@CcyDatePoint varchar(10),
	@ClientCode varchar(20)
AS
UPDATE [dbo].[AppSettings]
SET 
	[InstallID] = @InstallID,
	[InstallName] = @InstallName,
	[VersionNumber] = @VersionNumber,
	[CurrencyCode] = @CurrencyCode,
	[SupportEmail] = @SupportEmail,
	[SupportPhone] = @SupportPhone,
	[SmtpServerName] = @SmtpServerName,
	[SmtpServerPort] = @SmtpServerPort,
	[SmtpServerUsername] = @SmtpServerUsername,
	[SmtpServerPassword] = @SmtpServerPassword,
	[SmtpServerEnableSsl] = @SmtpServerEnableSsl,
	[LastDbBackupDate] = @LastDbBackupDate,
	[LastDbBackupFile] = @LastDbBackupFile,
	[LastDbBackupName] = @LastDbBackupName,
	[ExternalFilesPath] = @ExternalFilesPath,
	[CancelledRequestStatusID] = @CancelledRequestStatusID,
	[AccountingSystem] = @AccountingSystem,
	[CustomCodeFormat] = @CustomCodeFormat,
	[LanguageCode] = @LanguageCode,
	[CcyRateSource] = @CcyRateSource,
	[CcyDatePoint] = @CcyDatePoint,
	[ClientCode] = @ClientCode
WHERE
	[AppSettingsID] = @AppSettingsID
	AND [RowVersion] = @RowVersion
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AppSettings_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AppSettings_Del]
GO

CREATE PROCEDURE [dbo].[AppSettings_Del]
	@AppSettingsID int,
	@RowVersion timestamp
AS
DELETE FROM [dbo].[AppSettings]
WHERE
	[AppSettingsID] = @AppSettingsID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AppSettings_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AppSettings_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[AppSettings_Sel_ByID]
	@AppSettingsID int
AS
SET NOCOUNT ON
SELECT
	[AppSettingsID],
	[InstallID],
	[InstallName],
	[VersionNumber],
	[CurrencyCode],
	[SupportEmail],
	[SupportPhone],
	[SmtpServerName],
	[SmtpServerPort],
	[SmtpServerUsername],
	[SmtpServerPassword],
	[SmtpServerEnableSsl],
	[LastDbBackupDate],
	[LastDbBackupFile],
	[LastDbBackupName],
	[ExternalFilesPath],
	[CancelledRequestStatusID],
	[RowVersion],
	[AccountingSystem],
	[CustomCodeFormat],
	[LanguageCode],
	[CcyRateSource],
	[CcyDatePoint],
	[ClientCode]
FROM [dbo].[AppSettings]
WHERE
	[AppSettingsID] = @AppSettingsID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AppSettings_Sel_ByInstallID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AppSettings_Sel_ByInstallID]
GO

CREATE PROCEDURE [dbo].[AppSettings_Sel_ByInstallID]
	@InstallID uniqueidentifier
AS
SET NOCOUNT ON
SELECT
	[AppSettingsID],
	[InstallID],
	[InstallName],
	[VersionNumber],
	[CurrencyCode],
	[SupportEmail],
	[SupportPhone],
	[SmtpServerName],
	[SmtpServerPort],
	[SmtpServerUsername],
	[SmtpServerPassword],
	[SmtpServerEnableSsl],
	[LastDbBackupDate],
	[LastDbBackupFile],
	[LastDbBackupName],
	[ExternalFilesPath],
	[CancelledRequestStatusID],
	[RowVersion],
	[AccountingSystem],
	[CustomCodeFormat],
	[LanguageCode],
	[CcyRateSource],
	[CcyDatePoint],
	[ClientCode]
FROM [dbo].[AppSettings]
WHERE
	[InstallID] = @InstallID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AppSettings_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AppSettings_Sel_All]
GO

CREATE PROCEDURE [dbo].[AppSettings_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[AppSettingsID],
	[InstallID],
	[InstallName],
	[VersionNumber],
	[CurrencyCode],
	[SupportEmail],
	[SupportPhone],
	[SmtpServerName],
	[SmtpServerPort],
	[SmtpServerUsername],
	[SmtpServerPassword],
	[SmtpServerEnableSsl],
	[LastDbBackupDate],
	[LastDbBackupFile],
	[LastDbBackupName],
	[ExternalFilesPath],
	[CancelledRequestStatusID],
	[RowVersion],
	[AccountingSystem],
	[CustomCodeFormat],
	[LanguageCode],
	[CcyRateSource],
	[CcyDatePoint],
	[ClientCode]
FROM [dbo].[AppSettings]
ORDER BY 
	[AppSettingsID] ASC
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Invoice_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Invoice_Ins]
GO

CREATE PROCEDURE [dbo].[Invoice_Ins]
	@PurchaseLineID int,
	@Filename varchar(150),
	@OriginalFilename varchar(150),
	@Amount money,
	@Validated bit,
	@CreatedOn datetime,
	@InvoiceID int OUTPUT
AS
INSERT [dbo].[Invoice]
(
	[PurchaseLineID],
	[Filename],
	[OriginalFilename],
	[Amount],
	[Validated],
	[CreatedOn]
)
VALUES
(
	@PurchaseLineID,
	@Filename,
	@OriginalFilename,
	@Amount,
	@Validated,
	@CreatedOn
)
SELECT @InvoiceID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Invoice_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Invoice_Upd]
GO

CREATE PROCEDURE [dbo].[Invoice_Upd]
	@InvoiceID int,
	@PurchaseLineID int,
	@Filename varchar(150),
	@OriginalFilename varchar(150),
	@Amount money,
	@Validated bit,
	@CreatedOn datetime
AS
UPDATE [dbo].[Invoice]
SET 
	[PurchaseLineID] = @PurchaseLineID,
	[Filename] = @Filename,
	[OriginalFilename] = @OriginalFilename,
	[Amount] = @Amount,
	[Validated] = @Validated,
	[CreatedOn] = @CreatedOn
WHERE
	[InvoiceID] = @InvoiceID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Invoice_Upd_ByPurchaseLineID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Invoice_Upd_ByPurchaseLineID]
GO

CREATE PROCEDURE [dbo].[Invoice_Upd_ByPurchaseLineID]
	@PurchaseLineID int,
	@PurchaseLineIDOld int
AS
UPDATE [dbo].[Invoice]
SET
	[PurchaseLineID] = @PurchaseLineID
WHERE
	[PurchaseLineID] = @PurchaseLineIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Invoice_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Invoice_Del]
GO

CREATE PROCEDURE [dbo].[Invoice_Del]
	@InvoiceID int
AS
DELETE FROM [dbo].[Invoice]
WHERE
	[InvoiceID] = @InvoiceID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Invoice_Del_ByPurchaseLineID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Invoice_Del_ByPurchaseLineID]
GO

CREATE PROCEDURE [dbo].[Invoice_Del_ByPurchaseLineID]
	@PurchaseLineID int
AS
DELETE
FROM [dbo].[Invoice]
WHERE
	[PurchaseLineID] = @PurchaseLineID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Invoice_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Invoice_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[Invoice_Sel_ByID]
	@InvoiceID int
AS
SET NOCOUNT ON
SELECT
	[InvoiceID],
	[PurchaseLineID],
	[Filename],
	[OriginalFilename],
	[Amount],
	[Validated],
	[CreatedOn]
FROM [dbo].[Invoice]
WHERE
	[InvoiceID] = @InvoiceID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Invoice_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Invoice_Sel_All]
GO

CREATE PROCEDURE [dbo].[Invoice_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[InvoiceID],
	[PurchaseLineID],
	[Filename],
	[OriginalFilename],
	[Amount],
	[Validated],
	[CreatedOn]
FROM [dbo].[Invoice]
ORDER BY 
	[InvoiceID] ASC
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Invoice_Sel_ByPurchaseLineID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Invoice_Sel_ByPurchaseLineID]
GO

CREATE PROCEDURE [dbo].[Invoice_Sel_ByPurchaseLineID]
	@PurchaseLineID int
AS
SET NOCOUNT ON
SELECT
	[InvoiceID],
	[PurchaseLineID],
	[Filename],
	[OriginalFilename],
	[Amount],
	[Validated],
	[CreatedOn]
FROM [dbo].[Invoice]
WHERE
	[PurchaseLineID] = @PurchaseLineID
GO

--==============================================================================================================================================

ALTER VIEW [dbo].[PurchaseItemPaymentsDetail]
AS

select 
	item.*,
	t.PaymentDueName,  
	t.PaymentDueDate, 
	t.PaymentAmount, 
	NetBaseTotalTaxAmount * t.PaymentAmount / (case NetBaseTotal when 0 then 1 else NetBaseTotal end) as PaymentTaxAmount,
	sale.Comments, sale.SaleDate
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
left outer join dbo.ItinerarySale sale ON item.ItineraryID = sale.ItineraryID
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
exec AccountingCategory_Sel_All
exec ContactCategory_Sel_All
exec OptionType_Sel_All
exec Flag_Sel_All
exec TemplateCategory_Sel_All
exec Template_Sel_All
exec ContentType_Sel_All
select * from CurrencyRate where ValidTo >= DATEADD(year,-1,GetDate()) -- EXEC CurrencyRate_Sel_All
select t.* from Task t inner join DefaultTask d on d.TaskID = t.TaskID -- all Tasks linked to DefaultTasks table
exec DefaultTask_Sel_All
exec ItineraryType_Sel_All
GO

GO
alter FUNCTION [dbo].[ItineraryPricing]( )
RETURNS TABLE 
AS
RETURN 
(	
	select 
		ItineraryID,
		ItemCount,
		NetFinalTotal,
		GrossFinalTotal,
		case when NetFinalTotal = 0 then 0 else round((GrossFinalTotal - NetFinalTotal)/NetFinalTotal*100,2) end as Markup,
		case when GrossFinalTotal = 0 then 0 else round((GrossFinalTotal - NetFinalTotal)/GrossFinalTotal*100,2) end as Commission,	    
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
				when (GrossOverride is not null) then GrossOverride -- final $ override
				when (GrossMarkup is not null) then round(isnull(GrossFinalTotal,0) * (1+GrossMarkup/100),2) -- *(already applied?) final % markup
				else GrossFinalTotal																		-- sum of purchase items
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

GO
alter FUNCTION [dbo].[PurchaseItemPricing]( )
RETURNS TABLE 
AS
RETURN 
    (	
	select 
		t.*,		
		case when NetBaseUnit = 0 then 0 else round((GrossBaseUnit - NetBaseUnit)/NetBaseUnit*100,2) end as Markup,
		case when GrossBaseUnit = 0 then 0 else round((GrossBaseUnit - NetBaseUnit)/GrossBaseUnit*100,2) end as Commission,
		NetTaxCode as NetTaxTypeCode,
		NetTaxPercent,
		round(NetBaseTotal - (NetBaseTotal*100/(100+NetTaxPercent)),2) as NetBaseTotalTaxAmount,		
		GrossTaxCode as GrossTaxTypeCode,
		GrossTaxPercent,		
		round(GrossFinalTotal - (GrossFinalTotal*100/(100+GrossTaxPercent)),2) as GrossFinalTotalTaxAmount
	from
	(
		select 
			ItineraryID, 
			PurchaseItemID,
			ServiceTypeID,
			UnitMultiplier,	
			CurrencyRate,			
			Net as NetBaseUnit,
			round(Net * UnitMultiplier,2) as NetBaseTotal,
			round(Net * UnitMultiplier * CurrencyRate,2) as NetFinalTotal,
			GrossOrig as GrossBaseUnitPreAdjustment,	
			round(GrossAdjusted,2) as GrossBaseUnit,
			round(GrossAdjusted * UnitMultiplier,2) as GrossBaseTotal,
			round(GrossAdjusted * UnitMultiplier * CurrencyRate,2) as GrossFinalTotal	
		from
		(
			select 		 
				itin.ItineraryID, 
				item.PurchaseItemID,
				serv.ServiceTypeID,
				item.Net,
				round(dbo.GetAdjustedItemGross(itin.NetMargin, stype.Margin, item.Net, item.Gross, itin.NetComOrMup, itin.NetMinOrMax),2) as GrossAdjusted,
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


exec __RefreshViews;
GO

----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2013.01.22'
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