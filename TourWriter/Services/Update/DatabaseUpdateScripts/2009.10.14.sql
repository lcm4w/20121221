/*
TourWriter database update script, from version 2009.09.18 to 2009.10.14
*/
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
----------------------------------------------------------------------------------------
GO
PRINT N'Altering dbo.ItinerarySaleAllocationDetail...';
GO
ALTER VIEW [dbo].[ItinerarySaleAllocationDetail]
AS
select
	itin.*,
	sale.ItinerarySaleID,
	sale.Comments,
	sale.SaleDate,
	stype.ServiceTypeName,
	alloc.Amount as SaleAmount,
	case when itin.ItineraryMarkup = -100 then 0 else
		cast(alloc.Amount*100/(100+itin.ItineraryMarkup) as money) end as SaleNet,
	case when itin.ItineraryMarkup = -100 then 0 else
		cast(alloc.Amount - (alloc.Amount*100/(100+itin.ItineraryMarkup)) as money) end as SaleYield,		
	case when grossTax.Amount = -100 then 0 else
	    cast(alloc.Amount - (alloc.Amount*100/(100+grossTax.Amount)) as money) end as SaleTaxAmount,	
	grossTax.TaxTypeCode as SaleTaxTypeCode,
	grossAct.AccountingCategoryCode as AccountingCategoryCode,
	sale.IsLockedAccounting
from ItinerarySaleAllocation alloc
right outer join ItinerarySale sale on alloc.ItinerarySaleID = sale.ItinerarySaleID
left outer join dbo.ItineraryDetail itin on sale.ItineraryID = itin.ItineraryID
left outer join ServiceType stype on alloc.ServiceTypeID = stype.ServiceTypeID
left outer join AccountingCategory as grossAct on stype.GrossAccountingCategoryID = grossAct.AccountingCategoryID
left outer join TaxType as grossTax on stype.GrossTaxTypeID = grossTax.TaxTypeID;

GO
PRINT N'Altering dbo.ItinerarySaleDetail...';

GO
ALTER VIEW [dbo].[ItinerarySaleDetail]
AS
select
	itin.*,
	sale.ItinerarySaleID,
	sale.SaleDate,
	sale.Comments as SaleComments,
	sale.IsLockedAccounting as SaleIsLockedAccounting,
	cost.SaleAmount,
	cost.SaleNet,
	cost.SaleAmount-cost.SaleNet as SaleYield
from ItinerarySale sale
left outer join dbo.ItineraryDetail itin on sale.ItineraryID = itin.ItineraryID
left outer join
(
	select
		alloc.ItinerarySaleID, 
		sum(alloc.Amount) as SaleAmount,		
		sum(cast(
				case when itin.ItineraryMarkup = -100 then 0 else alloc.Amount*100/(100 + itin.ItineraryMarkup) end
			as money)) as SaleNet		
	from ItinerarySaleAllocation alloc
	left outer join ItinerarySale sale on alloc.ItinerarySaleID = sale.ItinerarySaleID
	left outer join dbo.ItineraryDetail itin on sale.ItineraryID = itin.ItineraryID
	group by alloc.ItinerarySaleID
) cost on cost.ItinerarySaleID = sale.ItinerarySaleID;

GO
PRINT N'Altering dbo.PurchaseItemDetail...';

GO
ALTER VIEW [dbo].[PurchaseItemDetail]
AS
select
	t.*,
	t.TotalNet - (t.TotalNet*100/(100+t.NetTaxPercent)) as NetTaxAmount,
	t.TotalGross - (t.TotalGross*100/(100+t.GrossTaxPercent)) as GrossTaxAmount
from
(
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
		serv.CurrencyCode,
		item.CurrencyRate,
		pric.Net,
		pric.Gross,
		item.NumberOfDays,
		item.Quantity,
		pric.UnitMultiplier,
		pric.TotalNet,
		pric.TotalGross,
		pric.Markup,
		pric.Commission,
		pric.TotalGross - pric.TotalNet as Yield,
	    (pric.TotalGross - pric.TotalNet)/(case pric.TotalGross when 0 then 1 else pric.TotalGross end)  as Margin,
		pric.TotalGrossOrig,
		pric.MarkupOrig,
		pric.CommissionOrig,
		stype.*,
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
) t;
GO
PRINT N'Refreshing dbo.PurchaseItemPaymentsDetail...';
GO
EXECUTE sp_refreshview N'dbo.PurchaseItemPaymentsDetail';
GO
----------------------------------------------------------------------------------------
PRINT N'Adding to [dbo].[TemplateCategory]...'
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO
SET IDENTITY_INSERT [dbo].[TemplateCategory] ON;
BEGIN TRANSACTION;
INSERT INTO [dbo].[TemplateCategory]([TemplateCategoryID], [TemplateCategoryName], [ParentTemplateCategoryID])
SELECT '5', 'Booking Email', NULL
COMMIT;
RAISERROR (N'[dbo].[TemplateCategory]: Insert Batch: 1.....Done!', 10, 1) WITH NOWAIT;
GO
SET IDENTITY_INSERT [dbo].[TemplateCategory] OFF;
GO

PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2009.10.14'
GO