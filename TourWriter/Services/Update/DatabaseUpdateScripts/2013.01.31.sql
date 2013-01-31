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
if ((select VersionNumber from AppSettings) <> '2012.11.18' and (select VersionNumber from AppSettings) <> '2013.01.10' and (select VersionNumber from AppSettings) <> '2013.01.22')
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


GO
PRINT N'Altering [dbo].[_AgentSet_Sel_ByID]...';


GO

/* Select from multiple tables for dataset fill command */
ALTER PROCEDURE [dbo].[_AgentSet_Sel_ByID]
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
	[IsRecordActive],
	[ParentFolderID],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[JobDescription],
	[PassportNumber],
	[PassportExpiry],
	[UpdatedOn],
	[Sex],
	[Preferences]
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
PRINT N'Altering [dbo].[_ItinerarySet_Sel_ByID]...';


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
	[AgentContactID],
	[UpdatedOn],
	[AllocationsItineraryID],
	[ItineraryTypeID],
	[Preferences]
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
	i.[SortDate],
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
	[IsRecordActive],
	[ParentFolderID],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[JobDescription],
	[PassportNumber],
	[PassportExpiry],
	[UpdatedOn],
	[Sex],
	[Preferences]
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

-- RoomType --
SELECT
	[RoomTypeID],
	[ItineraryID],
	[OptionTypeID],
	[RoomTypeName],
	[Quantity]
FROM [dbo].[RoomType]
where ItineraryID = @ItineraryID

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
	[Title],
	[RoomTypeID],
	[AgentID],
	[PriceOverride],
	[RoomName],
	[Preferences]
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

-- TASKS --
SELECT
	t.[TaskID],
	[TaskName],
	[DateDue],
	[DateCompleted],
	[Note]
FROM [dbo].[Task] t
inner join ItineraryTask i on i.TaskID = t.TaskID
where i.ItineraryID = @ItineraryID

-- ItineraryTask --
SELECT
	[ItineraryTaskID],
	[ItineraryID],
	[TaskID]
FROM [dbo].[ItineraryTask]
where ItineraryID = @ItineraryID

-- Allocations
SELECT [AllocationID]
      ,[ServiceID]
      ,[ItineraryID]
      ,[ValidFrom]
      ,[ValidTo]
      ,[Quantity]
      ,[Release]
  FROM [Allocation]
  where ItineraryID = @ItineraryID

SELECT [AllocationID]
      ,[AgentID]
      ,[Quantity]
      ,[Release]
  FROM [AllocationAgent]
  WHERE [AllocationID] IN ( 
    SELECT [AllocationID] FROM [dbo].[Allocation] a
    where ItineraryID = @ItineraryID )
    

-- GroupPrice
SELECT [GroupPriceID]
      ,[GroupPriceName]
      ,[ItineraryID]
      ,[ItineraryPaxID]
      ,[OptionTypeID]
      ,[Price]
      ,[Markup]
      ,[PriceOverride]
  FROM [GroupPrice]
  where ItineraryID = @ItineraryID
  
  -- Invoice
  SELECT
	[InvoiceID],
	[PurchaseLineID],
	[Filename],
	[OriginalFilename],
	[Amount],
	[Validated],
	[CreatedOn]
FROM [dbo].[Invoice]
WHERE [PurchaseLineID] IN ( 
		SELECT [PurchaseLineID] FROM [dbo].[PurchaseLine] 
		WHERE [ItineraryID] = @ItineraryID )
GO


PRINT N'Altering [dbo].[_SupplierSet_Sel_ByID]...';


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
	[IsDeleted],
	[UpdatedOn]
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
	[IsRecordActive],
	[ParentFolderID],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[JobDescription],
	[PassportNumber],
	[PassportExpiry],
	[UpdatedOn],
	[Sex],
	[Preferences]
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
	[ShowOnReport],
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

-- ServiceWarning
select * from ServiceWarning
where ServiceID in (select ServiceID from [Service] where SupplierID = @SupplierID)

-- Allocations
SELECT [AllocationID]
      ,[ServiceID]
      ,[ItineraryID]
      ,[ValidFrom]
      ,[ValidTo]
      ,[Quantity]
      ,[Release]
  FROM [Allocation]
  WHERE [ServiceID] IN ( 
    SELECT [ServiceID] FROM [dbo].[Service] 
	WHERE [SupplierID] = @SupplierID )

SELECT [AllocationID]
      ,[AgentID]
      ,[Quantity]
      ,[Release]
  FROM [AllocationAgent]
  WHERE [AllocationID] IN ( 
    SELECT [AllocationID] FROM [dbo].[Allocation] a
    LEFT JOIN [Service] s on s.ServiceID = a.ServiceID
	WHERE [SupplierID] = @SupplierID )

SELECT [AllocationID]
      ,[OptionTypeID]
      ,[AddedOn]
  FROM [AllocationOption]
  WHERE [AllocationID] IN ( 
    SELECT [AllocationID] FROM [dbo].[Allocation] a
    LEFT JOIN [Service] s on s.ServiceID = a.ServiceID
	WHERE [SupplierID] = @SupplierID )
GO



exec __RefreshViews;
GO

----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2013.01.31'
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