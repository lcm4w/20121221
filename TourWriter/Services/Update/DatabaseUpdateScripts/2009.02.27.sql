/*
TourWriter database update script, from version 2009.1.28 to 2009.2.27
*/
GO
PRINT N'Altering dbo.ItineraryDetail...';
GO
ALTER VIEW [dbo].[ItineraryDetail]
AS
select
	itin.ItineraryID,
	itin.ItineraryName,
	itin.DisplayName,
	origin.CountryName as Orgin,
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
	agent.AgentName,
	parentAgent.AgentName as ParentAgentName,
	agent.Phone as AgentPhone,
	agent.Email as AgentEmail,
	stat.ItineraryStatusID,
	stat.ItineraryStatusName,
	source.ItinerarySourceName,
	usr.UserName as AssignedTo,
	depart.DepartmentID,
	depart.DepartmentName,
	branch.BranchID,
	branch.BranchName,
	pric.Net as ItineraryNet,
	pric.Gross as ItineraryGross,
	pric.Markup as ItineraryMarkup,
	pric.Commission as ItineraryCommission,
	pric.Yield as ItineraryYield,
	payments.TotalPayments as ItineraryTotalPayments,
	pric.Gross - payments.TotalPayments as ItineraryTotalOutstanding,
	sales.TotalSales as ItineraryTotalSales,
	pric.ItemCount as PurchaseItemCount,
	isnull(PaxOverride, pax.PaxCount) as PaxCount,
	convert(varchar(3), dbo.PaymentTerm.DepositDuePeriod) + ' ' + depdue.PaymentDueName as ItineraryDepositTerms,
	dbo.GetPaymentDate(itin.ArriveDate, dbo.PaymentTerm.DepositDuePeriod, depdue.PaymentDueName) as ItineraryDepositDueDate, 
	dbo.PaymentTerm.DepositType as ItineraryDepositType, 
	dbo.PaymentTerm.DepositAmount as ItineraryDepositAmount,
	convert(varchar(3), dbo.PaymentTerm.PaymentDuePeriod) + ' ' + baldue.PaymentDueName as ItineraryBalanceTerms,
	dbo.GetPaymentDate(itin.ArriveDate, dbo.PaymentTerm.PaymentDuePeriod, baldue.PaymentDueName) as ItineraryBalanceDueDate,
	pric.Gross - dbo.PaymentTerm.DepositAmount as ItineraryBalanceAmount,
	itin.AddedOn as ItineraryCreatedDate,
	ParentFolderID as MenuFolderID	
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
	select ItineraryID, sum(Amount) as TotalSales
	from ItinerarySale sale
	inner join ItinerarySaleAllocation alloc on sale.ItinerarySaleID = alloc.ItinerarySaleID
	group by ItineraryID
) sales on itin.ItineraryID = sales.ItineraryID
left outer join 
(
	select ItineraryID, sum(Amount) as TotalPayments
	from ItineraryPayment payment
	left outer join ItineraryMember mem on payment.ItineraryMemberID = mem.ItineraryMemberID
	left outer join ItineraryGroup grp on mem.ItineraryGroupID = grp.ItineraryGroupID
	group by ItineraryID
) payments on itin.ItineraryID = payments.ItineraryID;
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
		case item.StartDate when null then null else datename(weekday, item.StartDate) end as PurchaseItemDay,
		case item.StartDate when null then null else datename(month,   item.StartDate) end as PurchaseItemMonth,
		case item.StartDate	when null then null else datename(year,    item.StartDate) end as PurchaseItemYear, 		
		--case item.StartDate when null then null else substring(convert(varchar(10), item.StartDate, 120), 6, 2) end as PurchaseItemMonthNumber,
		dateadd(day,item.NumberOfDays,item.StartDate) as PurchaseItemEndDate,
		sup.*,
		serv.ServiceName,
		opt.OptionName,
		rate.ValidFrom AS RateValidFrom,
		rate.ValidTo AS RateValidTo,
		serv.CurrencyCode,
		item.CurrencyRate,
		item.NumberOfDays,
		item.Quantity,
		pric.Net,
		pric.Gross,
		pric.TotalNet,
		pric.TotalGross,
		pric.Markup,
		pric.Commission,
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

PRINT N'Refreshing dbo.ItineraryClientDetail...';
GO
EXECUTE sp_refreshview N'dbo.ItineraryClientDetail';
GO
PRINT N'Refreshing dbo.ItinerarySaleAllocationDetail...';
GO
EXECUTE sp_refreshview N'dbo.ItinerarySaleAllocationDetail';
GO
PRINT N'Refreshing dbo.ItineraryPaymentDetail...';
GO
EXECUTE sp_refreshview N'dbo.ItineraryPaymentDetail';
GO
PRINT N'Refreshing dbo.PurchaseItemPaymentsDetail...';
GO
EXECUTE sp_refreshview N'dbo.PurchaseItemPaymentsDetail';
GO

PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2009.2.27'
GO
