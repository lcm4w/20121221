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
if ((select VersionNumber from AppSettings) <> '2012.11.18')
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

-- return new itinerary id
SELECT @NewItineraryID
GO

exec __RefreshViews;
GO

----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2013.01.10'
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