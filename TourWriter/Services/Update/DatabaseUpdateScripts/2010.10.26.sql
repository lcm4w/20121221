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
if ((select VersionNumber from AppSettings) <> '2010.09.28' AND (select VersionNumber from AppSettings) <> '2010.10.06')
	RAISERROR (N'Update script version is invalid for this db version',17,1)	
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------
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
	left outer join PaymentDue as depdue on PaymentTerm.DepositDueID = depdue.PaymentDueID on item.PaymentTermID = PaymentTerm.PaymentTermID;
GO
PRINT N'Refreshing [dbo].[ItineraryServiceTypeDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryServiceTypeDetail';


GO
PRINT N'Refreshing [dbo].[ItinerarySaleDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItinerarySaleDetail';


GO
PRINT N'Refreshing [dbo].[ItineraryClientDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryClientDetail';


GO
PRINT N'Refreshing [dbo].[ItinerarySaleAllocationDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItinerarySaleAllocationDetail';


GO
PRINT N'Refreshing [dbo].[ItineraryPaymentDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryPaymentDetail';


GO
PRINT N'Refreshing [dbo].[PurchaseItemPaymentsDetail]...';


GO
EXECUTE sp_refreshview N'dbo.PurchaseItemPaymentsDetail';


GO
----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2010.10.26'
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
