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
if ((select VersionNumber from AppSettings) <> '2011.05.09')
	RAISERROR (N'Database Update Script is not correct version for current database version',17,1)	

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------
GO
PRINT N'Altering Content...';
GO
exec sp_RENAME '[Content].[ContentId]' , 'ContentID', 'COLUMN';
GO
exec sp_RENAME '[Content].[SupplierId]' , 'SupplierID', 'COLUMN';
GO
exec sp_RENAME '[Content].[ContentTypeId]' , 'ContentTypeID', 'COLUMN';
GO

PRINT N'Altering ContentType...';
GO
exec sp_RENAME '[ContentType].[ContentTypeId]' , 'ContentTypeID', 'COLUMN';

GO
PRINT N'Dropping FK_Content_Supplier...';
GO
ALTER TABLE [dbo].[Content] DROP CONSTRAINT [FK_Content_Supplier];


GO
PRINT N'Creating [dbo].[ServiceContent]...';


GO
CREATE TABLE [dbo].[ServiceContent] (
    [ServiceContentID] INT IDENTITY (1, 1) NOT NULL,
    [ServiceID]        INT NOT NULL,
    [ContentID]        INT NOT NULL,
	[ContentTypeID]    INT NULL
);


GO
PRINT N'Creating PK_ServiceContent...';


GO
ALTER TABLE [dbo].[ServiceContent]
    ADD CONSTRAINT [PK_ServiceContent] PRIMARY KEY CLUSTERED ([ServiceContentID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating [dbo].[SupplierContent]...';


GO
CREATE TABLE [dbo].[SupplierContent] (
    [SupplierContentID] INT IDENTITY (1, 1) NOT NULL,
    [SupplierID]        INT NOT NULL,
    [ContentID]         INT NOT NULL,
	[ContentTypeID]     INT NULL
);


GO
PRINT N'Creating PK_SupplierContent...';


GO
ALTER TABLE [dbo].[SupplierContent]
    ADD CONSTRAINT [PK_SupplierContent] PRIMARY KEY CLUSTERED ([SupplierContentID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


GO
PRINT N'Creating FK_ServiceContent_Content...';


GO
ALTER TABLE [dbo].[ServiceContent] WITH NOCHECK
    ADD CONSTRAINT [FK_ServiceContent_Content] FOREIGN KEY ([ContentID]) REFERENCES [dbo].[Content] ([ContentID]) ON DELETE CASCADE ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_ServiceContent_Service...';


GO
ALTER TABLE [dbo].[ServiceContent] WITH NOCHECK
    ADD CONSTRAINT [FK_ServiceContent_Service] FOREIGN KEY ([ServiceID]) REFERENCES [dbo].[Service] ([ServiceID]) ON DELETE CASCADE ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_SupplierContent_Content...';


GO
ALTER TABLE [dbo].[SupplierContent] WITH NOCHECK
    ADD CONSTRAINT [FK_SupplierContent_Content] FOREIGN KEY ([ContentID]) REFERENCES [dbo].[Content] ([ContentID]) ON DELETE CASCADE ON UPDATE NO ACTION;


GO
PRINT N'Creating FK_SupplierContent_Supplier...';


GO
ALTER TABLE [dbo].[SupplierContent] WITH NOCHECK
    ADD CONSTRAINT [FK_SupplierContent_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[Supplier] ([SupplierID]) ON DELETE CASCADE ON UPDATE NO ACTION;


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
	[IsDeleted]
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
GO
PRINT N'Creating [dbo].[ServiceContent_Del]...';


GO

CREATE PROCEDURE [dbo].[ServiceContent_Del]
	@ServiceContentID int
AS
DELETE FROM [dbo].[ServiceContent]
WHERE
	[ServiceContentID] = @ServiceContentID
GO
PRINT N'Creating [dbo].[ServiceContent_Del_ByContentID]...';


GO

CREATE PROCEDURE [dbo].[ServiceContent_Del_ByContentID]
	@ContentID int
AS
DELETE
FROM [dbo].[ServiceContent]
WHERE
	[ContentID] = @ContentID
GO
PRINT N'Creating [dbo].[ServiceContent_Del_ByServiceID]...';


GO

CREATE PROCEDURE [dbo].[ServiceContent_Del_ByServiceID]
	@ServiceID int
AS
DELETE
FROM [dbo].[ServiceContent]
WHERE
	[ServiceID] = @ServiceID
GO
PRINT N'Creating [dbo].[ServiceContent_Ins]...';


GO

CREATE PROCEDURE [dbo].[ServiceContent_Ins]
	@ServiceID int,
	@ContentID int,
	@ServiceContentID int OUTPUT
AS
INSERT [dbo].[ServiceContent]
(
	[ServiceID],
	[ContentID]
)
VALUES
(
	@ServiceID,
	@ContentID
)
SELECT @ServiceContentID=SCOPE_IDENTITY()
GO
PRINT N'Creating [dbo].[ServiceContent_Sel_All]...';


GO

CREATE PROCEDURE [dbo].[ServiceContent_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[ServiceContentID],
	[ServiceID],
	[ContentID]
FROM [dbo].[ServiceContent]
ORDER BY 
	[ServiceContentID] ASC
GO
PRINT N'Creating [dbo].[ServiceContent_Sel_ByContentID]...';


GO

CREATE PROCEDURE [dbo].[ServiceContent_Sel_ByContentID]
	@ContentID int
AS
SET NOCOUNT ON
SELECT
	[ServiceContentID],
	[ServiceID],
	[ContentID]
FROM [dbo].[ServiceContent]
WHERE
	[ContentID] = @ContentID
GO
PRINT N'Creating [dbo].[ServiceContent_Sel_ByID]...';


GO

CREATE PROCEDURE [dbo].[ServiceContent_Sel_ByID]
	@ServiceContentID int
AS
SET NOCOUNT ON
SELECT
	[ServiceContentID],
	[ServiceID],
	[ContentID]
FROM [dbo].[ServiceContent]
WHERE
	[ServiceContentID] = @ServiceContentID
GO
PRINT N'Creating [dbo].[ServiceContent_Sel_ByServiceID]...';


GO

CREATE PROCEDURE [dbo].[ServiceContent_Sel_ByServiceID]
	@ServiceID int
AS
SET NOCOUNT ON
SELECT
	[ServiceContentID],
	[ServiceID],
	[ContentID]
FROM [dbo].[ServiceContent]
WHERE
	[ServiceID] = @ServiceID
GO
PRINT N'Creating [dbo].[ServiceContent_Upd]...';


GO

CREATE PROCEDURE [dbo].[ServiceContent_Upd]
	@ServiceContentID int,
	@ServiceID int,
	@ContentID int
AS
UPDATE [dbo].[ServiceContent]
SET 
	[ServiceID] = @ServiceID,
	[ContentID] = @ContentID
WHERE
	[ServiceContentID] = @ServiceContentID
GO
PRINT N'Creating [dbo].[ServiceContent_Upd_ByContentID]...';


GO

CREATE PROCEDURE [dbo].[ServiceContent_Upd_ByContentID]
	@ContentID int,
	@ContentIDOld int
AS
UPDATE [dbo].[ServiceContent]
SET
	[ContentID] = @ContentID
WHERE
	[ContentID] = @ContentIDOld
GO
PRINT N'Creating [dbo].[ServiceContent_Upd_ByServiceID]...';


GO

CREATE PROCEDURE [dbo].[ServiceContent_Upd_ByServiceID]
	@ServiceID int,
	@ServiceIDOld int
AS
UPDATE [dbo].[ServiceContent]
SET
	[ServiceID] = @ServiceID
WHERE
	[ServiceID] = @ServiceIDOld
GO
PRINT N'Creating [dbo].[SupplierContent_Del]...';


GO

CREATE PROCEDURE [dbo].[SupplierContent_Del]
	@SupplierContentID int
AS
DELETE FROM [dbo].[SupplierContent]
WHERE
	[SupplierContentID] = @SupplierContentID
GO
PRINT N'Creating [dbo].[SupplierContent_Del_ByContentID]...';


GO

CREATE PROCEDURE [dbo].[SupplierContent_Del_ByContentID]
	@ContentID int
AS
DELETE
FROM [dbo].[SupplierContent]
WHERE
	[ContentID] = @ContentID
GO
PRINT N'Creating [dbo].[SupplierContent_Del_BySupplierID]...';


GO

CREATE PROCEDURE [dbo].[SupplierContent_Del_BySupplierID]
	@SupplierID int
AS
DELETE
FROM [dbo].[SupplierContent]
WHERE
	[SupplierID] = @SupplierID
GO
PRINT N'Creating [dbo].[SupplierContent_Ins]...';


GO

CREATE PROCEDURE [dbo].[SupplierContent_Ins]
	@SupplierID int,
	@ContentID int,
	@SupplierContentID int OUTPUT
AS
INSERT [dbo].[SupplierContent]
(
	[SupplierID],
	[ContentID]
)
VALUES
(
	@SupplierID,
	@ContentID
)
SELECT @SupplierContentID=SCOPE_IDENTITY()
GO
PRINT N'Creating [dbo].[SupplierContent_Sel_All]...';


GO

CREATE PROCEDURE [dbo].[SupplierContent_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[SupplierContentID],
	[SupplierID],
	[ContentID]
FROM [dbo].[SupplierContent]
ORDER BY 
	[SupplierContentID] ASC
GO
PRINT N'Creating [dbo].[SupplierContent_Sel_ByContentID]...';


GO

CREATE PROCEDURE [dbo].[SupplierContent_Sel_ByContentID]
	@ContentID int
AS
SET NOCOUNT ON
SELECT
	[SupplierContentID],
	[SupplierID],
	[ContentID]
FROM [dbo].[SupplierContent]
WHERE
	[ContentID] = @ContentID
GO
PRINT N'Creating [dbo].[SupplierContent_Sel_ByID]...';


GO

CREATE PROCEDURE [dbo].[SupplierContent_Sel_ByID]
	@SupplierContentID int
AS
SET NOCOUNT ON
SELECT
	[SupplierContentID],
	[SupplierID],
	[ContentID]
FROM [dbo].[SupplierContent]
WHERE
	[SupplierContentID] = @SupplierContentID
GO
PRINT N'Creating [dbo].[SupplierContent_Sel_BySupplierID]...';


GO

CREATE PROCEDURE [dbo].[SupplierContent_Sel_BySupplierID]
	@SupplierID int
AS
SET NOCOUNT ON
SELECT
	[SupplierContentID],
	[SupplierID],
	[ContentID]
FROM [dbo].[SupplierContent]
WHERE
	[SupplierID] = @SupplierID
GO
PRINT N'Creating [dbo].[SupplierContent_Upd]...';


GO

CREATE PROCEDURE [dbo].[SupplierContent_Upd]
	@SupplierContentID int,
	@SupplierID int,
	@ContentID int
AS
UPDATE [dbo].[SupplierContent]
SET 
	[SupplierID] = @SupplierID,
	[ContentID] = @ContentID
WHERE
	[SupplierContentID] = @SupplierContentID
GO
PRINT N'Creating [dbo].[SupplierContent_Upd_ByContentID]...';


GO

CREATE PROCEDURE [dbo].[SupplierContent_Upd_ByContentID]
	@ContentID int,
	@ContentIDOld int
AS
UPDATE [dbo].[SupplierContent]
SET
	[ContentID] = @ContentID
WHERE
	[ContentID] = @ContentIDOld
GO
PRINT N'Creating [dbo].[SupplierContent_Upd_BySupplierID]...';


GO

CREATE PROCEDURE [dbo].[SupplierContent_Upd_BySupplierID]
	@SupplierID int,
	@SupplierIDOld int
AS
UPDATE [dbo].[SupplierContent]
SET
	[SupplierID] = @SupplierID
WHERE
	[SupplierID] = @SupplierIDOld
GO
PRINT N'Checking existing data against newly created constraints';



GO
ALTER TABLE [dbo].[ServiceContent] WITH CHECK CHECK CONSTRAINT [FK_ServiceContent_Content];

ALTER TABLE [dbo].[ServiceContent] WITH CHECK CHECK CONSTRAINT [FK_ServiceContent_Service];

ALTER TABLE [dbo].[SupplierContent] WITH CHECK CHECK CONSTRAINT [FK_SupplierContent_Content];

ALTER TABLE [dbo].[SupplierContent] WITH CHECK CHECK CONSTRAINT [FK_SupplierContent_Supplier];


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
		dbo.GetPaymentDate(item.StartDate, PaymentTerm.PaymentDuePeriod, baldue.PaymentDueName) as BalanceDueDate
		
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
exec __RefreshViews
GO


------------------------------------------------------------------------------------------------

-- Populate SupplierContent table
insert into SupplierContent (SupplierID, ContentID, ContentTypeID)
	select SupplierID, ContentID, ContentTypeID from Content --where ContentID not in (select ContentID from SupplierContent)

GO

----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2011.05.28'
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
