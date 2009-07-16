/*
TourWriter database update script, from version 2009.5.6 to 2009.05.18
*/
GO
if ('2009.05.18' <= (select VersionNumber from AppSettings)) return
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
	) t
)



GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
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
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Refreshing dbo.ItineraryDetail...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryDetail';


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Refreshing dbo.ItinerarySaleAllocationDetail...';


GO
EXECUTE sp_refreshview N'dbo.ItinerarySaleAllocationDetail';


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Refreshing dbo.ItineraryClientDetail...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryClientDetail';


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
PRINT N'Refreshing dbo.ItineraryPaymentDetail...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryPaymentDetail';


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Refreshing dbo.ItinerarySaleDetail...';


GO
EXECUTE sp_refreshview N'dbo.ItinerarySaleDetail';


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Refreshing dbo.PurchaseItemDetail...';


GO
EXECUTE sp_refreshview N'dbo.PurchaseItemDetail';


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
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2009.05.18'
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
