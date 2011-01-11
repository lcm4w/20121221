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
if ((select VersionNumber from AppSettings) <> '2010.11.06')
	RAISERROR (N'Update script version is invalid for this db version',17,1)	
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------

GO
PRINT N'Altering [dbo].[Agent]...';


GO
if not Exists(select * from sys.columns where Name = N'LanguageCode' and Object_ID = Object_ID(N'Agent'))
	ALTER TABLE [dbo].[Agent]
		ADD [LanguageCode] VARCHAR (20) NULL;


GO
PRINT N'Altering [dbo].[AppSettings]...';


GO
if not Exists(select * from sys.columns where Name = N'LanguageCode' and Object_ID = Object_ID(N'AppSettings'))
	ALTER TABLE [dbo].[AppSettings]
		ADD [LanguageCode] VARCHAR (20) NULL;


GO
PRINT N'Altering [dbo].[Itinerary]...';


GO
if not Exists(select * from sys.columns where Name = N'LanguageCode' and Object_ID = Object_ID(N'Itinerary'))
	ALTER TABLE [dbo].[Itinerary]
		ADD [LanguageCode] VARCHAR (20) NULL;


GO
PRINT N'Altering [dbo].[Service]...';


GO
if not Exists(select * from sys.columns where Name = N'LanguageCode' and Object_ID = Object_ID(N'Service'))
	ALTER TABLE [dbo].[Service]
		ADD [LanguageCode] VARCHAR (20) NULL;


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
	[LanguageCode]
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
PRINT N'Altering [dbo].[_ItinerarySet_Sel_ByID]...';


GO
ALTER PROCEDURE [dbo].[_ItinerarySet_Sel_ByID]
@ItineraryID INT
AS
SET NOCOUNT ON

DECLARE @DefaultCurrencyCode char(3) -- Used in PurchaseItem query
SET @DefaultCurrencyCode = (SELECT ISNULL(CurrencyCode,'NZD') FROM AppSettings)


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
	[BaseCurrency],
	[LanguageCode]
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
	[StaffRooms]
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
	-- TODO, alter below to leave just i.[CurrencyRate], removing back-compat currency code from old rates table   
	isnull(i.[CurrencyRate], dbo.GetConversionRate(s.CurrencyCode, i.StartDate)) as CurrencyRate,
	i.[Quantity],
	i.[NumberOfDays],
	i.[PaymentTermID],
	i.[RequestStatusID],
	i.[IsLockedAccounting],
	i.[AddedOn],
	i.[AddedBy],
	i.[RowVersion],
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
	s.[CurrencyCode],
	s.[LanguageCode]
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
	[RowVersion]
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
	[TaxTypeID]
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
	rt.[ValidTo],
	sv.[LanguageCode]
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

-- FOCs --

SELECT
	[ServiceFocID],
	[ServiceID],
	[UnitsUsed],
	[UnitsFree]
FROM [dbo].[ServiceFoc]
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
	[TaxTypeID],
	[Latitude],
	[Longitude],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[LanguageCode]
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
	[IsDeleted]
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
	[IsDeleted]
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

-- ServiceFoc --
SELECT
	[ServiceFocID],
	[ServiceID],
	[UnitsUsed],
	[UnitsFree]
FROM [dbo].[ServiceFoc]
WHERE
	[ServiceID] IN ( 
		SELECT [ServiceID] FROM [dbo].[Service] 
		WHERE [SupplierID] = @SupplierID )

-- Content
SELECT * FROM [Content] 
WHERE SupplierId = @SupplierID
GO
PRINT N'Altering [dbo].[Agent_Ins]...';


GO

ALTER PROCEDURE [dbo].[Agent_Ins]
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
	@LanguageCode varchar(20),
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
	[LanguageCode]
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
	@LanguageCode
)
SELECT @AgentID=SCOPE_IDENTITY()
GO
PRINT N'Altering [dbo].[Agent_Sel_All]...';


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
	[RowVersion],
	[LanguageCode]
FROM [dbo].[Agent]
ORDER BY 
	[AgentID] ASC
GO
PRINT N'Altering [dbo].[Agent_Sel_ByID]...';


GO

ALTER PROCEDURE [dbo].[Agent_Sel_ByID]
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
	[LanguageCode]
FROM [dbo].[Agent]
WHERE
	[AgentID] = @AgentID
GO
PRINT N'Altering [dbo].[Agent_Upd]...';


GO

ALTER PROCEDURE [dbo].[Agent_Upd]
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
	@LanguageCode varchar(20)
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
	[LanguageCode] = @LanguageCode
WHERE
	[AgentID] = @AgentID
	AND [RowVersion] = @RowVersion
GO
PRINT N'Altering [dbo].[AppSettings_Ins]...';


GO

ALTER PROCEDURE [dbo].[AppSettings_Ins]
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
	@LanguageCode varchar(20),
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
	[LanguageCode]
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
	@LanguageCode
)
SELECT @AppSettingsID=SCOPE_IDENTITY()
GO
PRINT N'Altering [dbo].[AppSettings_Sel_All]...';


GO

ALTER PROCEDURE [dbo].[AppSettings_Sel_All]
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
	[LanguageCode]
FROM [dbo].[AppSettings]
ORDER BY 
	[AppSettingsID] ASC
GO
PRINT N'Altering [dbo].[AppSettings_Sel_ByID]...';


GO

ALTER PROCEDURE [dbo].[AppSettings_Sel_ByID]
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
	[LanguageCode]
FROM [dbo].[AppSettings]
WHERE
	[AppSettingsID] = @AppSettingsID
GO
PRINT N'Altering [dbo].[AppSettings_Sel_ByInstallID]...';


GO

ALTER PROCEDURE [dbo].[AppSettings_Sel_ByInstallID]
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
	[LanguageCode]
FROM [dbo].[AppSettings]
WHERE
	[InstallID] = @InstallID
GO
PRINT N'Altering [dbo].[AppSettings_Upd]...';


GO

ALTER PROCEDURE [dbo].[AppSettings_Upd]
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
	@LanguageCode varchar(20)
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
	[LanguageCode] = @LanguageCode
WHERE
	[AppSettingsID] = @AppSettingsID
	AND [RowVersion] = @RowVersion
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
	@ArriveNote varchar(2000),
	@DepartDate datetime,
	@DepartCityID int,
	@DepartFlight varchar(50),
	@DepartNote varchar(2000),
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
	@Comments varchar(4000),
	@IsRecordActive bit,
	@IsReadOnly bit,
	@ParentFolderID int,
	@AddedOn datetime,
	@AddedBy int,
	@IsDeleted bit,
	@BaseCurrency varchar(5),
	@LanguageCode varchar(20),
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
	[BaseCurrency],
	[LanguageCode]
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
	@BaseCurrency,
	@LanguageCode
)
SELECT @ItineraryID=SCOPE_IDENTITY()
GO
PRINT N'Altering [dbo].[Itinerary_Sel_All]...';


GO

ALTER PROCEDURE [dbo].[Itinerary_Sel_All]
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
	[BaseCurrency],
	[LanguageCode]
FROM [dbo].[Itinerary]
ORDER BY 
	[ItineraryID] ASC
GO
PRINT N'Altering [dbo].[Itinerary_Sel_ByID]...';


GO

ALTER PROCEDURE [dbo].[Itinerary_Sel_ByID]
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
	[BaseCurrency],
	[LanguageCode]
FROM [dbo].[Itinerary]
WHERE
	[ItineraryID] = @ItineraryID
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
	@ArriveNote varchar(2000),
	@DepartDate datetime,
	@DepartCityID int,
	@DepartFlight varchar(50),
	@DepartNote varchar(2000),
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
	@Comments varchar(4000),
	@IsRecordActive bit,
	@IsReadOnly bit,
	@ParentFolderID int,
	@AddedOn datetime,
	@AddedBy int,
	@RowVersion timestamp,
	@IsDeleted bit,
	@BaseCurrency varchar(5),
	@LanguageCode varchar(20)
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
	[BaseCurrency] = @BaseCurrency,
	[LanguageCode] = @LanguageCode
WHERE
	[ItineraryID] = @ItineraryID
	AND [RowVersion] = @RowVersion
GO
PRINT N'Altering [dbo].[Option_Ins]...';


GO

ALTER PROCEDURE [dbo].[Option_Ins]
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
	[IsDeleted]
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
	@IsDeleted
)
SELECT @OptionID=SCOPE_IDENTITY()
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
	[IsDeleted]
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
	[IsDeleted]
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
	[IsDeleted]
FROM [dbo].[Option]
WHERE
	[RateID] = @RateID
GO
PRINT N'Altering [dbo].[Option_Upd]...';


GO

ALTER PROCEDURE [dbo].[Option_Upd]
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
	@IsDeleted bit
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
	[IsDeleted] = @IsDeleted
WHERE
	[OptionID] = @OptionID
	AND [RowVersion] = @RowVersion
GO
PRINT N'Altering [dbo].[Service_Ins]...';


GO

ALTER PROCEDURE [dbo].[Service_Ins]
	@ServiceName varchar(150),
	@SupplierID int,
	@ServiceTypeID int,
	@Description varchar(2000),
	@Comments varchar(2000),
	@Warning varchar(2000),
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
	@LanguageCode varchar(20),
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
	[IsDeleted],
	[LanguageCode]
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
	@IsDeleted,
	@LanguageCode
)
SELECT @ServiceID=SCOPE_IDENTITY()
GO
PRINT N'Altering [dbo].[Service_Sel_All]...';


GO

ALTER PROCEDURE [dbo].[Service_Sel_All]
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
	[TaxTypeID],
	[Latitude],
	[Longitude],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[LanguageCode]
FROM [dbo].[Service]
ORDER BY 
	[ServiceID] ASC
GO
PRINT N'Altering [dbo].[Service_Sel_ByID]...';


GO

ALTER PROCEDURE [dbo].[Service_Sel_ByID]
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
	[TaxTypeID],
	[Latitude],
	[Longitude],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[LanguageCode]
FROM [dbo].[Service]
WHERE
	[ServiceID] = @ServiceID
GO
PRINT N'Altering [dbo].[Service_Sel_BySupplierID]...';


GO

ALTER PROCEDURE [dbo].[Service_Sel_BySupplierID]
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
	[TaxTypeID],
	[Latitude],
	[Longitude],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[LanguageCode]
FROM [dbo].[Service]
WHERE
	[SupplierID] = @SupplierID
GO
PRINT N'Altering [dbo].[Service_Upd]...';


GO

ALTER PROCEDURE [dbo].[Service_Upd]
	@ServiceID int,
	@ServiceName varchar(150),
	@SupplierID int,
	@ServiceTypeID int,
	@Description varchar(2000),
	@Comments varchar(2000),
	@Warning varchar(2000),
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
	@IsDeleted bit,
	@LanguageCode varchar(20)
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
	[IsDeleted] = @IsDeleted,
	[LanguageCode] = @LanguageCode
WHERE
	[ServiceID] = @ServiceID
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
		ntax.TaxTypeCode as NetTaxCode,
		ntax.Amount as NetTaxPercent,
		gtax.TaxTypeCode as GrossTaxCode,
		gtax.Amount as GrossTaxPercent
	from PurchaseItem item
	left join [Option] as opt on item.OptionID = opt.OptionID
	left join Rate as rate on opt.RateID = rate.RateID
	left join [Service] as serv on rate.ServiceID = serv.ServiceID
	left join [Supplier] as sup on serv.SupplierID = sup.SupplierID
	left join ServiceType as stype on serv.ServiceTypeID = stype.ServiceTypeID	
	left join TaxType as ntax on isnull(serv.TaxTypeID, isnull(sup.TaxTypeID, stype.NetTaxTypeID)) = ntax.TaxTypeID
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

PRINT N'Altering [dbo].[_Option_GetNewFromDate]...';
GO

ALTER PROCEDURE [dbo].[_Option_GetNewFromDate] 
	@OptionID int,
	@NewDate  datetime
AS
SET NOCOUNT ON

SELECT TOP(1)  
	op.[OptionID],
	op.[OptionName],
	sv.[ServiceName],
	sp.[SupplierID],
	sv.[ServiceTypeID],	
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
	INNER JOIN [dbo].[Service] sv  ON rt.[ServiceID]  = sv.[ServiceID]
	INNER JOIN [dbo].[Supplier] sp ON sv.[SupplierID] = sp.[SupplierID]
WHERE op.RateID = rt.RateID
	AND (op.IsDeleted IS NULL OR op.IsDeleted = 0)
	AND op.IsRecordActive = 1
	AND op.OptionName = /* look for matching option name, i.e. same option */
	(
		SELECT OptionName FROM [Option]
		WHERE OptionID = @OptionID
	)
	AND op.RateID = /* new rate id to match the new date */
	(
		SELECT RateID FROM [Rate] 
		--WHERE ValidFrom <= @NewDate AND ValidTo >= @NewDate
		WHERE ValidFrom <= convert(char(8),@NewDate, 112) + ' 23:59' AND ValidTo >= convert(char(8),@NewDate, 112) + ' 00:00'
		AND (IsDeleted is null OR IsDeleted = 0)
		AND ServiceID = /* same service id as original option id */
		(
			SELECT ServiceID FROM [Rate] 
			WHERE RateID = /* original rate id */
			(
				SELECT RateID FROM [Option] 
				WHERE OptionID = @OptionID 
			)
		)
	)
GO

GO
PRINT N'Refreshing [dbo].[ItineraryDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryDetail';


GO
PRINT N'Refreshing [dbo].[PurchaseItemDetail]...';


GO
EXECUTE sp_refreshview N'dbo.PurchaseItemDetail';


GO
PRINT N'Refreshing [dbo].[ItinerarySaleAllocationDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItinerarySaleAllocationDetail';


GO
PRINT N'Refreshing [dbo].[ItineraryClientDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryClientDetail';


GO
PRINT N'Refreshing [dbo].[ItineraryServiceTypeDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryServiceTypeDetail';


GO
PRINT N'Refreshing [dbo].[ItineraryPaymentDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryPaymentDetail';


GO
PRINT N'Refreshing [dbo].[ItinerarySaleDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItinerarySaleDetail';


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
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2011.01.11'
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
