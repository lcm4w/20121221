/*
TourWriter database update script, from version 2009.3.17 to 2009.4.2
*/
GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;
SET NUMERIC_ROUNDABORT OFF;
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
left outer join State as state on state.StateID = sup.StateID 
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
PRINT N'Refreshing dbo.PurchaseItemDetail...';
GO
EXECUTE sp_refreshview N'dbo.PurchaseItemDetail';
GO
PRINT N'Refreshing dbo.PurchaseItemPaymentsDetail...';
GO
EXECUTE sp_refreshview N'dbo.PurchaseItemPaymentsDetail';
GO
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2009.4.2'
GO
