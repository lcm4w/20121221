/*
TourWriter database update script, from version 2009.4.2 to 2009.4.20
*/
GO
if ('2009.4.20' <= (select VersionNumber from AppSettings)) return
GO


GO
PRINT N'Dropping dbo.FK_ItineraryPax_Itinerary...';


GO
ALTER TABLE [dbo].[ItineraryPax] DROP CONSTRAINT [FK_ItineraryPax_Itinerary];


GO
PRINT N'Creating dbo.FK_ItineraryPax_Itinerary...';


GO
ALTER TABLE [dbo].[ItineraryPax]
    ADD CONSTRAINT [FK_ItineraryPax_Itinerary] FOREIGN KEY ([ItineraryID]) REFERENCES [dbo].[Itinerary] ([ItineraryID]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Altering dbo.PurchaseItemPricing...';


GO
ALTER FUNCTION [dbo].[PurchaseItemPricing]
( )
RETURNS TABLE 
AS
RETURN 
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
		case when Gross = 0 then 0 else (GrossOrig - Net)/GrossOrig*100 end as CommissionOrig
	
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
	) t
)



GO
PRINT N'Altering dbo.ItineraryPricing...';


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
		GrossOrig,
		case when Net = 0 then 0 else (GrossOrig - Net)/Net*100 end as MarkupOrig,
		case when GrossOrig = 0 then 0 else (GrossOrig - Net)/GrossOrig*100 end as CommissionOrig
	from
	(
		select i.ItineraryID, isnull(ItemCount,0) as ItemCount, Net, 
		case
			when (GrossOverride is not null) then GrossOverride
			when (GrossMarkup   is not null) then isnull(Gross,0) * (1+GrossMarkup/100)
			else Gross
		end as Gross,
		case
			when (GrossOverride is not null) then GrossOverride
			when (GrossMarkup   is not null) then isnull(GrossOrig,0) * (1+GrossMarkup/100)
			else GrossOrig
		end as GrossOrig
		from Itinerary i
		left outer join
		(
			select ItineraryID, count(*) as ItemCount, sum(TotalNet) as Net, sum(TotalGross) as Gross, sum(TotalGrossOrig) as GrossOrig
			from PurchaseItemPricing()
			group by ItineraryID
		) p on i.ItineraryID = p.ItineraryID
	) t
)



GO
PRINT N'Altering dbo.ItineraryServiceTypePricing...';


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
        [GrossOrig]      MONEY NULL,
        [MarkupOrig]     MONEY NULL,
        [CommissionOrig] MONEY NULL)
AS
BEGIN

	-- all itinerary by all service types
	insert into @result
		select itin.ItineraryID, stype.ServiceTypeID, base.ItemCount, base.Net, base.Gross, null, null, base.GrossOrig, null, null
		from Itinerary itin
		join (select ServiceTypeID, ServiceTypeName from ServiceType) stype on 1=1
		left outer join
		(
			select item.ItineraryID, item.ServicetypeID, sum(item.TotalNet) as Net, sum(item.TotalGross) as Gross, sum(item.TotalGrossOrig) as GrossOrig, count(*) as ItemCount 
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
		end,
		GrossOrig = 
		case
			when (itin.GrossOverride is not null) then isnull(base.GrossOrig,0) + itin.GrossOverride - isnull(baseItinGross.GrossOrig,0)
			when (itin.GrossMarkup   is not null) then isnull(base.GrossOrig,0) + (isnull(baseItinGross.GrossOrig,0) * (1+itin.GrossMarkup/100)) - isnull(baseItinGross.GrossOrig,0)
			else base.GrossOrig
		end
	from @result base 
	inner join Itinerary itin on  base.ItineraryID = itin.ItineraryID and base.ServiceTypeID = @deltaStype
	inner join
	(	select ItineraryID, sum(TotalGross) as Gross, sum(TotalGrossOrig) as GrossOrig 
		from PurchaseItemPricing()
		group by ItineraryID
	) baseItinGross
	on itin.ItineraryID = baseItinGross.ItineraryID

	update @result 
	set 
		Markup = (case when Net = 0 then 0 else (Gross - Net)/Net*100 end), 
		Commission = (case when Gross = 0 then 0 else (Gross - Net)/Gross*100 end),
		MarkupOrig = (case when Net = 0 then 0 else (GrossOrig - Net)/Net*100 end), 
		CommissionOrig = (case when GrossOrig = 0 then 0 else (GrossOrig - Net)/GrossOrig*100 end)

RETURN 
END


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
	agent.VoucherFooter as AgentVoucherNote,
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
	pric.GrossOrig as ItineraryGrossOrig,
	pric.MarkupOrig as ItineraryMarkupOrig,
	pric.CommissionOrig as ItineraryCommissionOrig,
	payments.TotalPayments as ItineraryTotalPayments,
	pric.Gross - payments.TotalPayments as ItineraryTotalOutstanding,
	sales.TotalSales as ItineraryTotalSales,
	pric.ItemCount as ItineraryPurchaseItemCount,
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
	cast(alloc.Amount*100/(100+itin.ItineraryMarkup) as money) as SaleNet,
	cast(alloc.Amount - (alloc.Amount*100/(100+itin.ItineraryMarkup)) as money) as SaleYield,
	cast(alloc.Amount - (alloc.Amount*100/(100+grossTax.Amount)) as money) AS SaleTaxAmount,
	grossTax.TaxTypeCode AS SaleTaxTypeCode,
	grossAct.AccountingCategoryCode AS AccountingCategoryCode,
	sale.IsLockedAccounting
from ItinerarySaleAllocation alloc
right outer join ItinerarySale sale on alloc.ItinerarySaleID = sale.ItinerarySaleID
left outer join dbo.ItineraryDetail itin on sale.ItineraryID = itin.ItineraryID
left outer join ServiceType stype on alloc.ServiceTypeID = stype.ServiceTypeID
left outer join AccountingCategory as grossAct on stype.GrossAccountingCategoryID = grossAct.AccountingCategoryID
left outer join TaxType as grossTax on stype.GrossTaxTypeID = grossTax.TaxTypeID;


GO
PRINT N'Refreshing dbo.ItineraryPaymentDetail...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryPaymentDetail';


GO
PRINT N'Altering dbo.ItineraryServiceTypeDetail...';


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
	price.GrossOrig,
	price.MarkupOrig,
	price.CommissionOrig,
	sales.Amount as Sales,
	isnull(price.Gross,0) - isnull(sales.Amount,0) as GrossMinusSales,
	price.Gross - (price.Gross*100/(100+stype.GrossTaxPercent)) as GrossTaxAmount
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
PRINT N'Altering dbo.SupplierDetail...';


GO
ALTER VIEW [dbo].[SupplierDetail]
AS
select 
	SupplierID,  
	SupplierName,
	[Description] as SupplierDescription,
	[Comments] as SupplierComments,
	HostName,
	StreetAddress,
	PostAddress,
	CityName,
	RegionName,
	StateName,
	CountryName,
	Phone,
	MobilePhone,
	FreePhone,
	Fax,
	Email,
	Website,
	CancellationPolicy,
	isnull(GradeName, '') as Grade1,
	isnull(GradeExternalName, '') as Grade2,
	sup.ParentFolderID as SupplierParentFolderID
from Supplier as sup
left outer join City as city on city.CityID = sup.CityID 
left outer join Region as region on region.RegionID = sup.RegionID 
left outer join [State] as stat on stat.StateID = sup.StateID 
left outer join Country as country on country.CountryID = sup.CountryID 
left outer join Grade as grade on grade.GradeID = sup.GradeID 
left outer join GradeExternal as gradeEx on gradeEx.GradeExternalID = sup.GradeExternalID;


GO
PRINT N'Altering dbo.SupplierRatesDetail...';


GO
ALTER VIEW [dbo].[SupplierRatesDetail]
AS
select 
	sup.*,
	serv.ServiceName,
	serv.[Description] as ServiceDescription,
	serv.Comments as ServiceComments,
	serv.MaxPax,
	serv.CheckinTime,
	serv.CheckoutTime,
	serv.CheckinMinutesEarly,
	serv.IsRecordActive,
	serv.CurrencyCode as ServiceCurrencyCode,
	stype.*,
	rate.ValidFrom as RateValidFrom,
	rate.ValidTo as RateValidTo,
	opt.OptionName,
	opt.Net,
	opt.Gross
from [service] serv
left outer join ServiceTypeDetail stype on serv.ServiceTypeID = stype.ServiceTypeID
left outer join SupplierDetail sup on serv.SupplierID = sup.SupplierID
right outer join Rate rate on serv.ServiceID = rate.ServiceID
right outer join [Option] opt on rate.RateID = opt.RateID;


GO
PRINT N'Creating dbo.ContactDetail...';


GO
CREATE VIEW [dbo].[ContactDetail]
AS
select 
	cont.ContactID,  
	ContactName,
	Title,
	FirstName,
	LastName,
	StreetAddress,
	PostAddress,
	CityName,
	RegionName,
	StateName,
	CountryName,
	PostCode,
	WorkPhone,
	HomePhone,
	CellPhone,
	Fax,
	Email1,
	Email2,
	Website,
	BirthDate,
	Notes,
	ItineraryCount	
from Contact as cont
left outer join City as city on city.CityID = cont.CityID 
left outer join Region as region on region.RegionID = cont.RegionID 
left outer join [State] as stat on stat.StateID = cont.StateID 
left outer join Country as country on country.CountryID = cont.CountryID 
left outer join
(
	select ContactID, count(ItineraryID) as ItineraryCount
	from ItineraryMember mem
	left outer join ItineraryGroup grp on mem.ItineraryGroupID = grp.ItineraryGroupID
	where ContactID is not null 
	group by ContactID
) cnt on cnt.ContactID = cont.ContactID;


GO
PRINT N'Creating dbo.ItinerarySaleDetail...';


GO
CREATE VIEW [dbo].[ItinerarySaleDetail]
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
		sum(cast(alloc.Amount*100/(100 + itin.ItineraryMarkup) as money)) as SaleNet
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
PRINT N'Altering dbo.ItineraryClientDetail...';


GO
ALTER VIEW [dbo].[ItineraryClientDetail]
AS
select
	ItineraryMemberName,	
	cont.*,	
	AgeGroupName as AgeGroup,
	mem.Comments as MemberComments,
	grp.Comments as GroupComments,
	grp.NoteToClient as GroupNoteToClient,
	grp.NoteToSupplier as GroupNoteToSupplier,
	grp.CurrencyCode as GroupCurrencyCode,
	grp.CurrencyRate as GroupCurrencyRate,
	itin.ItineraryGross * grp.CurrencyRate as GroupItineraryPrice,	
	itin.*
from ItineraryMember mem
left outer join ItineraryGroup grp on mem.ItineraryGroupID = grp.ItineraryGroupID
left outer join ItineraryDetail itin on grp.ItineraryID = itin.ItineraryID
left outer join AgeGroup age on mem.AgeGroupID = age.AgeGroupID
left outer join ContactDetail cont on mem.ContactID = cont.ContactID;

GO
PRINT N'Refreshing dbo.PurchaseItemPaymentsDetail...';
GO
EXECUTE sp_refreshview N'dbo.PurchaseItemPaymentsDetail';
GO
-----------------------------------------------------------
PRINT N'Refreshing all views...'
GO
EXEC [dbo].[__RefreshViews]
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2009.4.20'
GO
