/*
TourWriter database update script, from version 2009.08.08 to 2009.08.28
*/
GO
if ('2009.08.28' <= (select VersionNumber from AppSettings)) return
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
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2009.08.28'
GO