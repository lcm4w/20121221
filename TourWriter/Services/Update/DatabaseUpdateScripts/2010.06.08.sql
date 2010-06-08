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
if ((select VersionNumber from AppSettings) <> '2010.05.20')
	RAISERROR (N'Update script version is invalid for this db version',17,1)	
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------
GO
PRINT N'Altering [dbo].[Option]...';


GO
ALTER TABLE [dbo].[Option]
    ADD [GstUpdated] BIT NULL;


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Altering [dbo].[PurchaseItem]...';


GO
ALTER TABLE [dbo].[PurchaseItem]
    ADD [GstUpdated] BIT NULL;


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Altering [dbo].[_Login_AddOrUpdate]...';


GO
ALTER PROCEDURE [dbo].[_Login_AddOrUpdate]
	@LoginGuid UNIQUEIDENTIFIER, 
	@UserID INT, 
	@ComputerName VARCHAR (200),
	@SessionTimeout int
AS

declare @timestamp DateTime
set @timestamp = getdate()

-- update existing
UPDATE [Login]
SET LastActiveDate = @timestamp
where (Ended is null or Ended = 'false')
and ComputerName = @ComputerName
and cast(convert(char(10), LoginDate, 101) as smalldatetime) = cast(convert(char(10), @timestamp, 101) as smalldatetime) -- today

IF @@ROWCOUNT > 0
begin
	set @LoginGuid = (select top(1) LoginId from [Login] where ComputerName = @ComputerName and LastActiveDate = @timestamp)
end

else
-- insert new
begin
	SET @LoginGUID = NEWID()
	INSERT [dbo].[Login]
	(
		[LoginID],
		[UserID],
		[LoginDate],
		[LastActiveDate],
		[ComputerName]
	)
	VALUES
	(
		@LoginGuid,
		@UserID,
		@timestamp,
		@timestamp,
		@ComputerName
	)	
	-- house keeping (when inserting new) - remove older than 30 days
	DELETE Login 
	WHERE LastActiveDate < DATEADD(day, -30, GETDATE())	
END
	
if (@SessionTimeout > 0) set @SessionTimeout = -@SessionTimeout
select SessionId, SessionIndex, SessionCount
from (
	select 
		SessionId = LoginId,
		SessionIndex = row_number() over(order by LastActiveDate asc), 
		SessionCount = count(*) over()
	from [Login] 
	where LastActiveDate > dateadd(minute, @SessionTimeout, getdate())
	and (Ended is null or Ended = 'false')
) t where SessionId = @LoginGuid
GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Altering [dbo].[PurchaseItemTaxType]...';


GO
ALTER FUNCTION [dbo].[PurchaseItemTaxType]
( )
RETURNS TABLE 
AS
RETURN 
    (	
	select 
		PurchaseItemID,
		ntax.TaxTypeCode as NetTaxCode,
		ntax.Amount as NetTaxPercent,
		gtax.TaxTypeCode as GrossTaxCode,
		gtax.Amount as GrossTaxPercent
	from PurchaseItem item
	left join [Option] as opt on item.OptionID = opt.OptionID
	left join Rate as rate on opt.RateID = rate.RateID
	left join [Service] as serv on rate.ServiceID = serv.ServiceID
	left join [Supplier] as sup on serv.SupplierID = sup.SupplierID
	left join ServiceType as stype on serv.ServiceTypeID = stype.ServiceTypeID	
	left join TaxType as ntax on isnull(serv.TaxTypeID, isnull(sup.TaxTypeID, stype.NetTaxTypeID)) = ntax.TaxTypeID
	left join TaxType as gtax on stype.GrossTaxTypeID = gtax.TaxTypeID	
)
GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


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
		TotalGross - (TotalGross*100/(100+GrossTaxPercent)) as GrossTaxAmount	    	
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
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Altering [dbo].[ItineraryPricing]...';


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
		NetTax,
		GrossTax,
		GrossOrig,
		case when Net = 0 then 0 else (GrossOrig - Net)/Net*100 end as MarkupOrig,
		case when GrossOrig = 0 then 0 else (GrossOrig - Net)/GrossOrig*100 end as CommissionOrig
	from
	(
		select 
			i.ItineraryID,
			isnull(ItemCount,0) as ItemCount,
			Net, 
			case
				when (GrossOverride is not null) then GrossOverride
				when (GrossMarkup is not null) then isnull(Gross,0) * (1+GrossMarkup/100)
				else Gross
			end as Gross,
			case
				when (GrossOverride is not null) then GrossOverride
				when (GrossMarkup is not null) then isnull(GrossOrig,0) * (1+GrossMarkup/100)
				else GrossOrig
			end as GrossOrig,
			NetTax,
			GrossTax
		from Itinerary i
		left outer join
		(
			select ItineraryID, 
				count(*) as ItemCount, sum(TotalNet) as Net, sum(TotalGross) as Gross,
				sum(NetTaxAmount) as NetTax, sum(GrossTaxAmount) as GrossTax,
				sum(TotalGrossOrig) as GrossOrig
			from PurchaseItemPricing()
			group by ItineraryID
		) p on i.ItineraryID = p.ItineraryID
	) t
)
GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Altering [dbo].[ItinerarySaleAllocationPricing]...';


GO
ALTER FUNCTION [dbo].[ItinerarySaleAllocationPricing]
( )
RETURNS TABLE 
AS
RETURN 
    (	
	select
		itin.ItineraryId,
		sale.ItinerarySaleID,
		stype.ServiceTypeID,
		alloc.Amount as SaleAmount,
		case when itin.Markup = -100 then 0 else
			cast(alloc.Amount*100/(100+itin.Markup) as money) end as Net,
		case when itin.Markup = -100 then 0 else
			cast(alloc.Amount - (alloc.Amount*100/(100+itin.Markup)) as money) end as Yield,		
		case when grossTax.Amount = -100 then 0 else
			cast(alloc.Amount - (alloc.Amount*100/(100+grossTax.Amount)) as money) end as TaxAmount
	from ItinerarySaleAllocation alloc
	right outer join ItinerarySale sale on alloc.ItinerarySaleID = sale.ItinerarySaleID	
	left outer join dbo.ItineraryPricing() itin on sale.ItineraryID = itin.ItineraryID
	left outer join ServiceType stype on alloc.ServiceTypeID = stype.ServiceTypeID
	left outer join TaxType as grossTax on stype.GrossTaxTypeID = grossTax.TaxTypeID
)
GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Refreshing [dbo].[SupplierRatesDetail]...';


GO
EXECUTE sp_refreshview N'dbo.SupplierRatesDetail';


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Refreshing [dbo].[ItineraryDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryDetail';


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Refreshing [dbo].[ItinerarySaleAllocationDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItinerarySaleAllocationDetail';


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Refreshing [dbo].[ItineraryServiceTypeDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryServiceTypeDetail';


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Refreshing [dbo].[ItinerarySaleDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItinerarySaleDetail';


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Refreshing [dbo].[ItineraryClientDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryClientDetail';


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Refreshing [dbo].[ItineraryPaymentDetail]...';


GO
EXECUTE sp_refreshview N'dbo.ItineraryPaymentDetail';


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Refreshing [dbo].[PurchaseItemDetail]...';


GO
EXECUTE sp_refreshview N'dbo.PurchaseItemDetail';


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'Refreshing [dbo].[PurchaseItemPaymentsDetail]...';


GO
EXECUTE sp_refreshview N'dbo.PurchaseItemPaymentsDetail';


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2010.06.08'
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
