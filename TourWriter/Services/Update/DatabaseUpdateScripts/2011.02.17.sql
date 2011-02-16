
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
if ((select VersionNumber from AppSettings) <> '2011.02.14')
	RAISERROR (N'Update script version is invalid for this db version',17,1)	

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------

GO
PRINT N'Altering [dbo].[SupplierRatesDetail]...';
GO

ALTER VIEW [dbo].[SupplierRatesDetail]
AS

select 
	sup.*,
	serv.ServiceName,
	serv.[Description] as ServiceDescription,
	serv.Comments as ServiceComments,
	isnull(serv.Latitude, sup.Latitude) as ServiceLatitude,
	isnull(serv.Longitude, sup.Longitude) as ServiceLongitude,
	serv.MaxPax,
	serv.CheckinTime,
	serv.CheckoutTime,
	serv.CheckinMinutesEarly,
	serv.IsRecordActive,
	serv.CurrencyCode as ServiceCurrencyCode,
	c.DisplayFormat as ServiceCurrencyFormat,
	stype.*,
	rate.ValidFrom as RateValidFrom,
	rate.ValidTo as RateValidTo,
	opt.OptionName,
	opt.Net,
	opt.Gross,	
	case when (opt.Gross != 0) then (opt.Gross - opt.Net)/opt.Gross*100 else null end as Commission,
	case when (opt.Net != 0 ) then (opt.Gross - opt.Net)/opt.Net*100 else null end as Markup,
	opt.PricingOption
from [service] serv
left outer join ServiceTypeDetail stype on serv.ServiceTypeID = stype.ServiceTypeID
left outer join SupplierDetail sup on serv.SupplierID = sup.SupplierID
right outer join Rate rate on serv.ServiceID = rate.ServiceID
right outer join [Option] opt on rate.RateID = opt.RateID
left join Currency c on serv.CurrencyCode = c.CurrencyCode COLLATE DATABASE_DEFAULT
GO

GO
PRINT N'Refreshing all views...';
GO
exec __RefreshViews
GO

----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2011.02.17'
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
