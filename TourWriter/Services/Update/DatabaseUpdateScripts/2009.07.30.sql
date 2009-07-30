/*
TourWriter database update script, from version 2009.07.10 to 2009.07.30
*/
GO
if ('2009.07.30' <= (select VersionNumber from AppSettings)) return
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
PRINT N'Altering dbo.ServiceTypeDetail...';


GO
ALTER VIEW [dbo].[ServiceTypeDetail]
AS
select
	ServiceTypeID,
	ServiceTypeName,
	BookingStartName,
	BookingEndName,
	NumberOfDaysName,
	ntax.TaxTypeCode as NetTaxTypeCode,
	ntax.Amount as NetTaxPercent,
	ncat.AccountingCategoryCode NetAccountingCategoryCode,
	gtax.TaxTypeCode as GrossTaxTypeCode,
	gtax.Amount as GrossTaxPercent,
	gcat.AccountingCategoryCode GrossAccountingCategoryCode
from ServiceType stype
left outer join TaxType as ntax on stype.NetTaxTypeID = ntax.TaxTypeID
left outer join AccountingCategory ncat on stype.NetAccountingCategoryID = ncat.AccountingCategoryID
left outer join TaxType as gtax on stype.GrossTaxTypeID = gtax.TaxTypeID
left outer join AccountingCategory gcat on stype.GrossAccountingCategoryID = gcat.AccountingCategoryID;


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Refreshing dbo.SupplierRatesDetail...';


GO
EXECUTE sp_refreshview N'dbo.SupplierRatesDetail';


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Refreshing dbo.ItineraryServiceTypeDetail...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryServiceTypeDetail';


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
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
		dateadd(day,item.NumberOfDays,item.StartDate) as PurchaseItemEndDate,
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
		serv.CurrencyCode,
		item.CurrencyRate,
		item.NumberOfDays,
		item.Quantity,
		pric.Net,
		pric.Gross,
		pric.UnitMultiplier,
		pric.TotalNet,
		pric.TotalGross,
		pric.Markup,
		pric.Commission,
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
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Refreshing dbo.PurchaseItemPaymentsDetail...';


GO
EXECUTE sp_refreshview N'dbo.PurchaseItemPaymentsDetail';


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2009.07.30'
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
----------------------------------------------------------------------------------------
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

GO
