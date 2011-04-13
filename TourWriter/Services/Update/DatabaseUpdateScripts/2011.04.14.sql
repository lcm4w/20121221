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
if ((select VersionNumber from AppSettings) <> '2011.04.05' AND (select VersionNumber from AppSettings) <> '2011.04.12')
	RAISERROR (N'Database Update Script is not correct version for current database version',17,1)	

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------
GO
PRINT N'Altering [dbo].[ItineraryServiceTypePricing]...';


GO
ALTER FUNCTION [dbo].[ItineraryServiceTypePricing]
( )
RETURNS 
    @result TABLE (
        [ItineraryID]    INT   NULL,
        [ServiceTypeID]  INT   NULL,
        [ItemCount]      INT   NULL,
        [Net]            MONEY NULL,
        [Gross]          MONEY NULL,
        [Markup]         MONEY NULL,
        [Commission]     MONEY NULL,
        [Yield]          MONEY NULL)
AS
BEGIN

	-- all itinerary by all service types
	insert into @result
		select itin.ItineraryID, stype.ServiceTypeID, base.ItemCount, base.Net, base.Gross, null, null, null
		from Itinerary itin
		join (select ServiceTypeID, ServiceTypeName from ServiceType) stype on 1=1
		left outer join
		(
			select item.ItineraryID, item.ServicetypeID, count(*) as ItemCount , sum(item.NetFinalTotal) as Net, sum(item.GrossFinalTotal) as Gross
			from PurchaseItemPricing() item
			group by item.ItineraryID, item.ServiceTypeID
		) base on itin.ItineraryID = base.ItineraryID and stype.ServiceTypeID = base.ServiceTypeID

	-- add final override
	declare @deltaStype int
	set @deltaStype = (select top(1) ServiceTypeID from ServiceType order by IsAdditionalMarkupContainer desc)
	update @result 
	set Gross = 
		case
			when (itin.GrossOverride is not null) then isnull(base.Gross,0) + itin.GrossOverride - isnull(baseItinGross.Gross,0)
			when (itin.GrossMarkup   is not null) then isnull(base.Gross,0) + (isnull(baseItinGross.Gross,0) * (1+itin.GrossMarkup/100)) - isnull(baseItinGross.Gross,0)
			else base.Gross
		end
	from @result base 
	inner join Itinerary itin on  base.ItineraryID = itin.ItineraryID and base.ServiceTypeID = @deltaStype
	inner join
	(	select ItineraryID, sum(GrossFinalTotal) as Gross
		from PurchaseItemPricing()
		group by ItineraryID
	) baseItinGross
	on itin.ItineraryID = baseItinGross.ItineraryID

	update @result 
	set 
		Markup = (case when Net = 0 then 0 else (Gross - Net)/Net*100 end), 
		Commission = (case when Gross = 0 then 0 else (Gross - Net)/Gross*100 end),
		Yield = Gross - isnull(Net, 0)

RETURN 
END
GO
PRINT N'Altering [dbo].[ItineraryServiceTypeDetail]...';

GO

ALTER VIEW [dbo].[ItineraryServiceTypeDetail]
AS
select
	itin.*,
	stype.*, 
	price.ItemCount as PurchaseItemCount, 
	price.Net, 
	price.Gross,
	price.Markup,
	price.Commission,
	price.Yield,
	sales.Amount as Sales,
	isnull(price.Gross,0) - isnull(sales.Amount,0) as GrossMinusSales,
	price.Gross - (price.Gross*100/(100+stype.GrossTaxPercent)) as GrossTaxAmount
	
	--/**** only for backwards compatibility ****/
	--,0 as GrossOrig
	--,0 as MarkupOrig
	--,0 as CommissionOrig
	--,0 as ItineraryGrossOrig
	--/******************************************/
	
from ItineraryServiceTypePricing() price
inner join ItineraryDetail itin on price.ItineraryID = itin.ItineraryID
inner join ServiceTypeDetail stype on price.ServiceTypeID = stype.ServiceTypeID
left outer join 
(
	select ItineraryID, ServiceTypeID, sum(Amount) as Amount
	from ItinerarySale sale
	inner join ItinerarySaleAllocation alloc on sale.ItinerarySaleID = alloc.ItinerarySaleID
	group by ItineraryID, ServiceTypeID
) sales on itin.ItineraryID = sales.ItineraryID and stype.ServiceTypeID = sales.ServiceTypeID;



GO
PRINT N'Altering [dbo].[ItineraryDetail]...';


GO

ALTER VIEW [dbo].[ItineraryDetail]
AS
select
		itin.ItineraryID,
		itin.ItineraryName,
		isnull(itin.DisplayName, itin.ItineraryName) DisplayName,
		itin.CustomCode,
		itin.CountryID as OriginID,
		origin.CountryName as Origin,
		datediff(dd, itin.ArriveDate, itin.DepartDate) + 1 as ItineraryLength,
		itin.ArriveDate as ArriveDate, 
		case itin.ArriveDate when null then null else datename(weekday, itin.ArriveDate) end as ArriveDay,
		case itin.ArriveDate when null then null else datename(month,   itin.ArriveDate) end as ArriveMonth,
		case itin.ArriveDate when null then null else datename(year,    itin.ArriveDate) end as ArriveYear, 
		--case itin.ArriveDate when null then null else substring(convert(varchar(10), itin.ArriveDate, 120), 6, 2) end as ArriveMonth,
		arriveCity.CityName as ArriveCityName,
		ArriveFlight,
		itin.DepartDate,
		departCity.CityName as DepartCityName,
		DepartFlight,
		itin.AssignedTo as UserID,
		u.DisplayName as UserDisplayName,
		agent.AgentID,
		agent.AgentName,
		parentAgent.AgentName as ParentAgentName,
		agent.Phone as AgentPhone,
		agent.Email as AgentEmail,
		agent.VoucherFooter as AgentVoucherNote,
		stat.ItineraryStatusID,
		stat.ItineraryStatusName,
		ItinerarySourceName,
		usr.UserName as AssignedTo,
		depart.DepartmentID,
		depart.DepartmentName,
		branch.BranchID,
		branch.BranchName,
		isnull(itin.CurrencyCode, (select top(1) CurrencyCode from Appsettings)) as ItineraryCurrencyCode,
		isnull(itinCcy.DisplayFormat, sysCcy.DisplayFormat) as ItineraryCurrencyFormat,
		itin.NetComOrMup as NetOverrideComOrMup,
		pric.NetFinalTotal as ItineraryNetFinalTotal,
		pric.GrossFinalTotal as ItineraryGrossFinalTotal,
		pric.Markup as ItineraryMarkup,
		pric.Commission as ItineraryCommission,
		pric.Yield as ItineraryYield,
		pric.GrossFinalTotalTaxAmount as ItineraryGrossFinalTotalTaxAmount,
		payments.TotalPayments as ItineraryTotalPayments,
		pric.GrossFinalTotal - payments.TotalPayments as ItineraryTotalOutstanding,
		sales.TotalSales as ItineraryTotalSales,
		sales.TotalSalesTax as ItineraryTotalSalesTax,
		pric.ItemCount as ItineraryPurchaseItemCount,
		isnull(PaxOverride, pax.PaxCount) as PaxCount,
		convert(varchar(3), dbo.PaymentTerm.DepositDuePeriod) + ' ' + depdue.PaymentDueName as ItineraryDepositTerms,
		dbo.GetPaymentDate(itin.ArriveDate, dbo.PaymentTerm.DepositDuePeriod, depdue.PaymentDueName) as ItineraryDepositDueDate, 
		dbo.PaymentTerm.DepositType as ItineraryDepositType, 
		dbo.PaymentTerm.DepositAmount as ItineraryDepositAmount,
		convert(varchar(3), dbo.PaymentTerm.PaymentDuePeriod) + ' ' + baldue.PaymentDueName as ItineraryBalanceTerms,
		dbo.GetPaymentDate(itin.ArriveDate, dbo.PaymentTerm.PaymentDuePeriod, baldue.PaymentDueName) as ItineraryBalanceDueDate,
		--pric.GrossFinalTotal - dbo.PaymentTerm.DepositAmount as ItineraryBalanceAmount,
		itin.IsRecordActive,
		itin.AddedOn as ItineraryCreatedDate,
		ParentFolderID as MenuFolderID
		
		----/**** only for backwards compatibility ****/
		--,pric.NetFinalTotal as ItineraryNet
		--,pric.GrossFinalTotal as ItineraryGross
		----/******************************************/		
		
	from Itinerary itin
	join dbo.ItineraryPricing() pric on itin.ItineraryID = pric.ItineraryID
	left outer join Country origin on itin.CountryID = origin.CountryID
	left outer join City arriveCity on itin.ArriveCityID = arriveCity.CityID
	left outer join City departCity on itin.DepartCityID = departCity.CityID
	left outer join Agent agent on itin.AgentID = agent.AgentID
	left outer join Agent parentAgent on agent.ParentAgentID = parentAgent.AgentID
	left outer join dbo.PaymentDue as baldue 
	right outer join dbo.PaymentTerm on baldue.PaymentDueID = dbo.PaymentTerm.PaymentDueID 
	left outer join dbo.PaymentDue as depdue on dbo.PaymentTerm.DepositDueID = depdue.PaymentDueID on isnull(itin.PaymentTermID,agent.SalePaymentTermID) = dbo.PaymentTerm.PaymentTermID
	left outer join ItineraryStatus stat on itin.ItineraryStatusID = stat.ItineraryStatusID
	left outer join ItinerarySource source on itin.ItinerarySourceID = source.ItinerarySourceID
	left outer join [User] usr on itin.AssignedTo = usr.UserID
	left outer join Department depart on itin.DepartmentID = depart.DepartmentID
	left outer join Branch branch on itin.BranchID = branch.BranchID
	left outer join [User] u on u.UserID = itin.AssignedTo
	left outer join
	(
		select itin.ItineraryID, count(ItineraryMemberID) as PaxCount
		from ItineraryMember mem
		left outer join ItineraryGroup grp ON mem.ItineraryGroupID = grp.ItineraryGroupID
		left outer join Itinerary itin ON grp.ItineraryID = itin.ItineraryID
		group by itin.ItineraryID
	) pax on itin.ItineraryID = pax.ItineraryID
	left outer join 
	(
		select ItineraryID, sum(SaleAmount) as TotalSales, sum(TaxAmount) as TotalSalesTax
		from ItinerarySaleAllocationPricing() sale
		group by ItineraryID
	) sales on itin.ItineraryID = sales.ItineraryID
	left outer join 
	(
		select ItineraryID, sum(Amount) as TotalPayments
		from ItineraryPayment payment
		left outer join ItineraryMember mem on payment.ItineraryMemberID = mem.ItineraryMemberID
		left outer join ItineraryGroup grp on mem.ItineraryGroupID = grp.ItineraryGroupID
		group by ItineraryID
	) payments on itin.ItineraryID = payments.ItineraryID
	left join Currency itinCcy on itinCcy.CurrencyCode = itin.CurrencyCode COLLATE DATABASE_DEFAULT
	left join Currency sysCcy on sysCcy.CurrencyCode = (select top(1) CurrencyCode from AppSettings) COLLATE DATABASE_DEFAULT
	where 
		(itin.IsDeleted is null or itin.IsDeleted = 'false');
GO
PRINT N'Refreshing [dbo].[ItinerarySaleDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItinerarySaleDetail';


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
		NetBaseTotal - NetBaseTotalTaxAmount as NetBaseTotalExcl, -- TotalNetExcl,
		GrossFinalTotal - GrossFinalTotalTaxAmount as GrossFinalTotalExcl,
		stype.ServiceTypeID,
		stype.ServiceTypeName,
		stype.BookingStartName,
		stype.BookingEndName,
		stype.NumberOfDaysName,
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
PRINT N'Refreshing [dbo].[ItineraryServiceTypeDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryServiceTypeDetail';


GO

----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2011.04.14'
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
