/*
TourWriter database update script, from version 2009.08.02 to 2009.08.08
*/
GO
if ('2009.08.08' <= (select VersionNumber from AppSettings)) return
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
        [Yield]          MONEY NULL,
        [GrossOrig]      MONEY NULL,
        [MarkupOrig]     MONEY NULL,
        [CommissionOrig] MONEY NULL)
AS
BEGIN

	-- all itinerary by all service types
	insert into @result
		select itin.ItineraryID, stype.ServiceTypeID, base.ItemCount, base.Net, base.Gross, null, null, null, base.GrossOrig, null, null
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
		Yield = Gross - isnull(Net, 0),
		MarkupOrig = (case when Net = 0 then 0 else (GrossOrig - Net)/Net*100 end), 
		CommissionOrig = (case when GrossOrig = 0 then 0 else (GrossOrig - Net)/GrossOrig*100 end)

RETURN 
END


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
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
	price.Yield,
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
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2009.08.08'
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
