/*
TourWriter database update script, from version 2009.05.18 to 2009.05.29
*/
GO
if ('2009.05.29' <= (select VersionNumber from AppSettings)) return
GO

----------------------------------------------------------------------------------------
PRINT N'Altering dbo.ContactDetail...';

GO
ALTER VIEW [dbo].[ContactDetail]
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
	ItineraryCount,
	FolderName	
from Contact as cont
left outer join City as city on city.CityID = cont.CityID 
left outer join Region as region on region.RegionID = cont.RegionID 
left outer join [State] as stat on stat.StateID = cont.StateID 
left outer join Country as country on country.CountryID = cont.CountryID  
left outer join Folder as fld on fld.FolderId = cont.ParentFolderId
left outer join
(
	select ContactID, count(ItineraryID) as ItineraryCount
	from ItineraryMember mem
	left outer join ItineraryGroup grp on mem.ItineraryGroupID = grp.ItineraryGroupID
	where ContactID is not null 
	group by ContactID
) cnt on cnt.ContactID = cont.ContactID;
GO

PRINT N'Refreshing dbo.ItineraryClientDetail...';
GO
EXECUTE sp_refreshview N'dbo.ItineraryClientDetail';
GO

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
		line.NoteToSupplier,
		line.NoteToVoucher,
		line.NoteToClient,
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
PRINT N'Refreshing dbo.PurchaseItemPaymentsDetail...';
GO
EXECUTE sp_refreshview N'dbo.PurchaseItemPaymentsDetail';
----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2009.05.29'
GO