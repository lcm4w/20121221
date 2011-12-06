
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
if ((select VersionNumber from AppSettings) <> '2011.02.10')
	RAISERROR (N'Update script version is invalid for this db version',17,1)	

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------

GO
PRINT N'Altering [dbo].[Currency]...';

GO
if Exists(select * from sys.columns where Name = N'Symbol' and Object_ID = Object_ID(N'Currency'))
ALTER TABLE [dbo].[Currency] DROP COLUMN [Symbol];

GO
if not Exists(select * from sys.columns where Name = N'DisplayFormat' and Object_ID = Object_ID(N'Currency'))
ALTER TABLE [dbo].[Currency]
    ADD [DisplayFormat] NVARCHAR (50) NULL;
		
GO
if not Exists(select * from sys.columns where Name = N'Enabled' and Object_ID = Object_ID(N'Currency'))
ALTER TABLE [dbo].[Currency]
    ADD [Enabled] BIT NULL;	
GO

GO
alter table Currency
	alter column CurrencyName nvarchar (255)
GO

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
		pric.Net as ItineraryNet,
		pric.Gross as ItineraryGross,
		pric.Markup as ItineraryMarkup,
		pric.Commission as ItineraryCommission,
		pric.Yield as ItineraryYield,
		pric.NetTax as ItineraryNetTax,
		pric.GrossTax as ItineraryGrossTax,
		pric.GrossOrig as ItineraryGrossOrig,
		pric.MarkupOrig as ItineraryMarkupOrig,
		pric.CommissionOrig as ItineraryCommissionOrig,
		payments.TotalPayments as ItineraryTotalPayments,
		pric.Gross - payments.TotalPayments as ItineraryTotalOutstanding,
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
		pric.Gross - dbo.PaymentTerm.DepositAmount as ItineraryBalanceAmount,
		itin.IsRecordActive,
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
	left outer join PaymentDue as depdue on PaymentTerm.DepositDueID = depdue.PaymentDueID on item.PaymentTermID = PaymentTerm.PaymentTermID
	left outer join Currency itemCcy on itemCcy.CurrencyCode = serv.CurrencyCode COLLATE DATABASE_DEFAULT
	left outer join Currency sysCcy on sysCcy.CurrencyCode = (select top(1) CurrencyCode from AppSettings) COLLATE DATABASE_DEFAULT
GO

GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Currency_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Currency_Ins]
GO

CREATE PROCEDURE [dbo].[Currency_Ins]
	@CurrencyCode char(3),
	@CurrencyName nvarchar(255),
	@DisplayFormat nvarchar(50),
	@Enabled bit
AS
INSERT [dbo].[Currency]
(
	[CurrencyCode],
	[CurrencyName],
	[DisplayFormat],
	[Enabled]
)
VALUES
(
	@CurrencyCode,
	@CurrencyName,
	@DisplayFormat,
	@Enabled
)
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Currency_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Currency_Upd]
GO

CREATE PROCEDURE [dbo].[Currency_Upd]
	@CurrencyCode char(3),
	@CurrencyName nvarchar(255),
	@DisplayFormat nvarchar(50),
	@Enabled bit
AS
UPDATE [dbo].[Currency]
SET 
	[CurrencyName] = @CurrencyName,
	[DisplayFormat] = @DisplayFormat,
	[Enabled] = @Enabled
WHERE
	[CurrencyCode] = @CurrencyCode
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Currency_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Currency_Del]
GO

CREATE PROCEDURE [dbo].[Currency_Del]
	@CurrencyCode char(3)
AS
DELETE FROM [dbo].[Currency]
WHERE
	[CurrencyCode] = @CurrencyCode
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Currency_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Currency_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[Currency_Sel_ByID]
	@CurrencyCode char(3)
AS
SET NOCOUNT ON
SELECT
	[CurrencyCode],
	[CurrencyName],
	[DisplayFormat],
	[Enabled]
FROM [dbo].[Currency]
WHERE
	[CurrencyCode] = @CurrencyCode
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Currency_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Currency_Sel_All]
GO

CREATE PROCEDURE [dbo].[Currency_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[CurrencyCode],
	[CurrencyName],
	[DisplayFormat],
	[Enabled]
FROM [dbo].[Currency]
ORDER BY 
	[CurrencyCode] ASC
GO


GO
PRINT N'Refreshing all views...';
GO

exec __RefreshViews


GO
PRINT N'Add currency data...';
GO

if ((select count(currencycode) from currency) < 100)
begin

	if exists (select currencycode from Currency where CurrencyCode = 'AED') 
		 update Currency set CurrencyName = N'UAE Dirham', DisplayFormat = N'''DH''#,##0.00', Enabled = 'true' where CurrencyCode = 'AED'
	else insert into Currency values ('AED', N'UAE Dirham', N'''DH''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'AFN') 
		 update Currency set CurrencyName = N'Afghani', DisplayFormat = N'''AFN''#,##0.00', Enabled = 'true' where CurrencyCode = 'AFN'
	else insert into Currency values ('AFN', N'Afghani', N'''AFN''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'ALL') 
		 update Currency set CurrencyName = N'Lek', DisplayFormat = N'''Lek''#,##0.00', Enabled = 'true' where CurrencyCode = 'ALL'
	else insert into Currency values ('ALL', N'Lek', N'''Lek''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'AMD') 
		 update Currency set CurrencyName = N'Armenian Dram', DisplayFormat = N'#,##0.00''dram''', Enabled = 'true' where CurrencyCode = 'AMD'
	else insert into Currency values ('AMD', N'Armenian Dram', N'#,##0.00''dram''', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'ANG') 
		 update Currency set CurrencyName = N'Netherlands Antillian Guikder', DisplayFormat = N'''NAƒ''#,##0.00', Enabled = 'true' where CurrencyCode = 'ANG'
	else insert into Currency values ('ANG', N'Netherlands Antillian Guikder', N'''NAƒ''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'AOA') 
		 update Currency set CurrencyName = N'Kwanza', DisplayFormat = N'''Kz''#,##0.00', Enabled = 'true' where CurrencyCode = 'AOA'
	else insert into Currency values ('AOA', N'Kwanza', N'''Kz''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'ARS') 
		 update Currency set CurrencyName = N'Argentine Peso', DisplayFormat = N'''AR$''#,##0.00', Enabled = 'true' where CurrencyCode = 'ARS'
	else insert into Currency values ('ARS', N'Argentine Peso', N'''AR$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'AUD') 
		 update Currency set CurrencyName = N'Australian Dollar', DisplayFormat = N'''AU$''#,##0.00', Enabled = 'true' where CurrencyCode = 'AUD'
	else insert into Currency values ('AUD', N'Australian Dollar', N'''AU$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'AWG') 
		 update Currency set CurrencyName = N'Aruban Guilder', DisplayFormat = N'''Afl.''#,##0.00', Enabled = 'true' where CurrencyCode = 'AWG'
	else insert into Currency values ('AWG', N'Aruban Guilder', N'''Afl.''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'AZN') 
		 update Currency set CurrencyName = N'Azerbaijanian Manat', DisplayFormat = N'''man''#,##0.00', Enabled = 'true' where CurrencyCode = 'AZN'
	else insert into Currency values ('AZN', N'Azerbaijanian Manat', N'''man''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'BAM') 
		 update Currency set CurrencyName = N'Convertible Marks', DisplayFormat = N'''KM''#,##0.00', Enabled = 'true' where CurrencyCode = 'BAM'
	else insert into Currency values ('BAM', N'Convertible Marks', N'''KM''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'BBD') 
		 update Currency set CurrencyName = N'Barbados Dollar', DisplayFormat = N'''Bds$''#,##0.00', Enabled = 'true' where CurrencyCode = 'BBD'
	else insert into Currency values ('BBD', N'Barbados Dollar', N'''Bds$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'BDT') 
		 update Currency set CurrencyName = N'Taka', DisplayFormat = N'''Tk''#,##0.00', Enabled = 'true' where CurrencyCode = 'BDT'
	else insert into Currency values ('BDT', N'Taka', N'''Tk''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'BGN') 
		 update Currency set CurrencyName = N'Bulgarian Lev', DisplayFormat = N'#,##0.00''лв''', Enabled = 'true' where CurrencyCode = 'BGN'
	else insert into Currency values ('BGN', N'Bulgarian Lev', N'#,##0.00''лв''', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'BHD') 
		 update Currency set CurrencyName = N'Bahraini Dinar', DisplayFormat = N'''BD''#,##0.000', Enabled = 'true' where CurrencyCode = 'BHD'
	else insert into Currency values ('BHD', N'Bahraini Dinar', N'''BD''#,##0.000', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'BIF') 
		 update Currency set CurrencyName = N'Burundi Franc', DisplayFormat = N'''FBu''#,##0', Enabled = 'true' where CurrencyCode = 'BIF'
	else insert into Currency values ('BIF', N'Burundi Franc', N'''FBu''#,##0', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'BMD') 
		 update Currency set CurrencyName = N'Bermudian Dollar', DisplayFormat = N'''BD$''#,##0.00', Enabled = 'true' where CurrencyCode = 'BMD'
	else insert into Currency values ('BMD', N'Bermudian Dollar', N'''BD$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'BND') 
		 update Currency set CurrencyName = N'Brunei Dollar', DisplayFormat = N'''B$''#,##0.00', Enabled = 'true' where CurrencyCode = 'BND'
	else insert into Currency values ('BND', N'Brunei Dollar', N'''B$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'BOB') 
		 update Currency set CurrencyName = N'Boliviano', DisplayFormat = N'''B$''#,##0.00', Enabled = 'true' where CurrencyCode = 'BOB'
	else insert into Currency values ('BOB', N'Boliviano', N'''B$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'BRL') 
		 update Currency set CurrencyName = N'Brazilian Real', DisplayFormat = N'''R$''#,##0.00', Enabled = 'true' where CurrencyCode = 'BRL'
	else insert into Currency values ('BRL', N'Brazilian Real', N'''R$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'BSD') 
		 update Currency set CurrencyName = N'Bahamian Dollar', DisplayFormat = N'''B$''#,##0.00', Enabled = 'true' where CurrencyCode = 'BSD'
	else insert into Currency values ('BSD', N'Bahamian Dollar', N'''B$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'BTN') 
		 update Currency set CurrencyName = N'Ngultrum', DisplayFormat = N'''Nu.''#,##0.00', Enabled = 'true' where CurrencyCode = 'BTN'
	else insert into Currency values ('BTN', N'Ngultrum', N'''Nu.''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'BWP') 
		 update Currency set CurrencyName = N'Pula', DisplayFormat = N'''pula''#,##0.00', Enabled = 'true' where CurrencyCode = 'BWP'
	else insert into Currency values ('BWP', N'Pula', N'''pula''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'BYR') 
		 update Currency set CurrencyName = N'Belarussian Ruble', DisplayFormat = N'''Br''#,##0', Enabled = 'true' where CurrencyCode = 'BYR'
	else insert into Currency values ('BYR', N'Belarussian Ruble', N'''Br''#,##0', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'BZD') 
		 update Currency set CurrencyName = N'Belize Dollar', DisplayFormat = N'''BZ$''#,##0.00', Enabled = 'true' where CurrencyCode = 'BZD'
	else insert into Currency values ('BZD', N'Belize Dollar', N'''BZ$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'CAD') 
		 update Currency set CurrencyName = N'Canadian Dollar', DisplayFormat = N'''C$''#,##0.00', Enabled = 'true' where CurrencyCode = 'CAD'
	else insert into Currency values ('CAD', N'Canadian Dollar', N'''C$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'CDF') 
		 update Currency set CurrencyName = N'Franc Congolais', DisplayFormat = N'''CDF''#,##0.00', Enabled = 'true' where CurrencyCode = 'CDF'
	else insert into Currency values ('CDF', N'Franc Congolais', N'''CDF''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'CHF') 
		 update Currency set CurrencyName = N'Swiss Franc', DisplayFormat = N'''CHF''#,##0.00', Enabled = 'true' where CurrencyCode = 'CHF'
	else insert into Currency values ('CHF', N'Swiss Franc', N'''CHF''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'CLP') 
		 update Currency set CurrencyName = N'Chilean Peso', DisplayFormat = N'''CL$''#,##0', Enabled = 'true' where CurrencyCode = 'CLP'
	else insert into Currency values ('CLP', N'Chilean Peso', N'''CL$''#,##0', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'CNY') 
		 update Currency set CurrencyName = N'Yuan Renminbi', DisplayFormat = N'''RMB¥''#,##0.00', Enabled = 'true' where CurrencyCode = 'CNY'
	else insert into Currency values ('CNY', N'Yuan Renminbi', N'''RMB¥''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'COP') 
		 update Currency set CurrencyName = N'Colombian Peso', DisplayFormat = N'''COL$''#,##0.00', Enabled = 'true' where CurrencyCode = 'COP'
	else insert into Currency values ('COP', N'Colombian Peso', N'''COL$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'CRC') 
		 update Currency set CurrencyName = N'Costa Rican Colon', DisplayFormat = N'''CR₡''#,##0.00', Enabled = 'true' where CurrencyCode = 'CRC'
	else insert into Currency values ('CRC', N'Costa Rican Colon', N'''CR₡''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'CUP') 
		 update Currency set CurrencyName = N'Cuban Peso', DisplayFormat = N'''$MN''#,##0.00', Enabled = 'true' where CurrencyCode = 'CUP'
	else insert into Currency values ('CUP', N'Cuban Peso', N'''$MN''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'CVE') 
		 update Currency set CurrencyName = N'Cape Verde Escudo', DisplayFormat = N'''Esc''#,##0.00', Enabled = 'true' where CurrencyCode = 'CVE'
	else insert into Currency values ('CVE', N'Cape Verde Escudo', N'''Esc''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'CZK') 
		 update Currency set CurrencyName = N'Czech Koruna', DisplayFormat = N'#,##0.00''Kč''', Enabled = 'true' where CurrencyCode = 'CZK'
	else insert into Currency values ('CZK', N'Czech Koruna', N'#,##0.00''Kč''', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'DJF') 
		 update Currency set CurrencyName = N'Djibouti Franc', DisplayFormat = N'''Fdj''#,##0', Enabled = 'true' where CurrencyCode = 'DJF'
	else insert into Currency values ('DJF', N'Djibouti Franc', N'''Fdj''#,##0', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'DKK') 
		 update Currency set CurrencyName = N'Danish Krone', DisplayFormat = N'#,##0.00''kr''', Enabled = 'true' where CurrencyCode = 'DKK'
	else insert into Currency values ('DKK', N'Danish Krone', N'#,##0.00''kr''', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'DOP') 
		 update Currency set CurrencyName = N'Dominican Peso', DisplayFormat = N'''RD$''#,##0.00', Enabled = 'true' where CurrencyCode = 'DOP'
	else insert into Currency values ('DOP', N'Dominican Peso', N'''RD$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'DZD') 
		 update Currency set CurrencyName = N'Algerian Dinar', DisplayFormat = N'''DA''#,##0.00', Enabled = 'true' where CurrencyCode = 'DZD'
	else insert into Currency values ('DZD', N'Algerian Dinar', N'''DA''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'EEK') 
		 update Currency set CurrencyName = N'Kroon', DisplayFormat = N'#,##0.00''EEK''', Enabled = 'true' where CurrencyCode = 'EEK'
	else insert into Currency values ('EEK', N'Kroon', N'#,##0.00''EEK''', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'EGP') 
		 update Currency set CurrencyName = N'Egyptian Pound', DisplayFormat = N'''LE''#,##0.00', Enabled = 'true' where CurrencyCode = 'EGP'
	else insert into Currency values ('EGP', N'Egyptian Pound', N'''LE''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'ERN') 
		 update Currency set CurrencyName = N'Nakfa', DisplayFormat = N'''Nfk''#,##0.00', Enabled = 'true' where CurrencyCode = 'ERN'
	else insert into Currency values ('ERN', N'Nakfa', N'''Nfk''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'ETB') 
		 update Currency set CurrencyName = N'Ethiopian Birr', DisplayFormat = N'''Br''#,##0.00', Enabled = 'true' where CurrencyCode = 'ETB'
	else insert into Currency values ('ETB', N'Ethiopian Birr', N'''Br''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'EUR') 
		 update Currency set CurrencyName = N'Euro', DisplayFormat = N'#,##0.00''€''', Enabled = 'true' where CurrencyCode = 'EUR'
	else insert into Currency values ('EUR', N'Euro', N'#,##0.00''€''', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'FJD') 
		 update Currency set CurrencyName = N'Fiji Dollar', DisplayFormat = N'''FJ$''#,##0.00', Enabled = 'true' where CurrencyCode = 'FJD'
	else insert into Currency values ('FJD', N'Fiji Dollar', N'''FJ$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'FKP') 
		 update Currency set CurrencyName = N'Falkland Islands Pound', DisplayFormat = N'''FK£''#,##0.00', Enabled = 'true' where CurrencyCode = 'FKP'
	else insert into Currency values ('FKP', N'Falkland Islands Pound', N'''FK£''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'GBP') 
		 update Currency set CurrencyName = N'Pound Sterling', DisplayFormat = N'''GB£''#,##0.00', Enabled = 'true' where CurrencyCode = 'GBP'
	else insert into Currency values ('GBP', N'Pound Sterling', N'''GB£''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'GEL') 
		 update Currency set CurrencyName = N'Lari', DisplayFormat = N'''GEL''#,##0.00', Enabled = 'true' where CurrencyCode = 'GEL'
	else insert into Currency values ('GEL', N'Lari', N'''GEL''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'GHS') 
		 update Currency set CurrencyName = N'Cedi', DisplayFormat = N'''GHS¢''#,##0.00', Enabled = 'true' where CurrencyCode = 'GHS'
	else insert into Currency values ('GHS', N'Cedi', N'''GHS¢''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'GIP') 
		 update Currency set CurrencyName = N'Gibraltar Pound', DisplayFormat = N'''GI£''#,##0.00', Enabled = 'true' where CurrencyCode = 'GIP'
	else insert into Currency values ('GIP', N'Gibraltar Pound', N'''GI£''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'GMD') 
		 update Currency set CurrencyName = N'Dalasi', DisplayFormat = N'''GMD''#,##0.00', Enabled = 'true' where CurrencyCode = 'GMD'
	else insert into Currency values ('GMD', N'Dalasi', N'''GMD''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'GNF') 
		 update Currency set CurrencyName = N'Guinea Franc', DisplayFormat = N'''FG''#,##0', Enabled = 'true' where CurrencyCode = 'GNF'
	else insert into Currency values ('GNF', N'Guinea Franc', N'''FG''#,##0', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'GTQ') 
		 update Currency set CurrencyName = N'Quetzal', DisplayFormat = N'''GTQ''#,##0.00', Enabled = 'true' where CurrencyCode = 'GTQ'
	else insert into Currency values ('GTQ', N'Quetzal', N'''GTQ''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'GYD') 
		 update Currency set CurrencyName = N'Guyana Dollar', DisplayFormat = N'''GY$''#,##0.00', Enabled = 'true' where CurrencyCode = 'GYD'
	else insert into Currency values ('GYD', N'Guyana Dollar', N'''GY$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'HKD') 
		 update Currency set CurrencyName = N'Hong Kong Dollar', DisplayFormat = N'''HK$''#,##0.00', Enabled = 'true' where CurrencyCode = 'HKD'
	else insert into Currency values ('HKD', N'Hong Kong Dollar', N'''HK$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'HNL') 
		 update Currency set CurrencyName = N'Lempira', DisplayFormat = N'''HNL''#,##0.00', Enabled = 'true' where CurrencyCode = 'HNL'
	else insert into Currency values ('HNL', N'Lempira', N'''HNL''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'HRK') 
		 update Currency set CurrencyName = N'Croatian Kuna', DisplayFormat = N'''kn''#,##0.00', Enabled = 'true' where CurrencyCode = 'HRK'
	else insert into Currency values ('HRK', N'Croatian Kuna', N'''kn''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'HTG') 
		 update Currency set CurrencyName = N'Gourde', DisplayFormat = N'''HTG''#,##0.00', Enabled = 'true' where CurrencyCode = 'HTG'
	else insert into Currency values ('HTG', N'Gourde', N'''HTG''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'HUF') 
		 update Currency set CurrencyName = N'Forint', DisplayFormat = N'#,##0.00''Ft''', Enabled = 'true' where CurrencyCode = 'HUF'
	else insert into Currency values ('HUF', N'Forint', N'#,##0.00''Ft''', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'IDR') 
		 update Currency set CurrencyName = N'Rupiah', DisplayFormat = N'''Rp''#,##0.00', Enabled = 'true' where CurrencyCode = 'IDR'
	else insert into Currency values ('IDR', N'Rupiah', N'''Rp''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'ILS') 
		 update Currency set CurrencyName = N'New Israeli Sheqel', DisplayFormat = N'#,##0.00''IL₪''', Enabled = 'true' where CurrencyCode = 'ILS'
	else insert into Currency values ('ILS', N'New Israeli Sheqel', N'#,##0.00''IL₪''', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'INR') 
		 update Currency set CurrencyName = N'Indian Rupee', DisplayFormat = N'''Rs''#,##0.00', Enabled = 'true' where CurrencyCode = 'INR'
	else insert into Currency values ('INR', N'Indian Rupee', N'''Rs''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'IQD') 
		 update Currency set CurrencyName = N'Iraqi Dinar', DisplayFormat = N'''IQD''#,##0.000', Enabled = 'true' where CurrencyCode = 'IQD'
	else insert into Currency values ('IQD', N'Iraqi Dinar', N'''IQD''#,##0.000', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'IRR') 
		 update Currency set CurrencyName = N'Iranian Rial', DisplayFormat = N'''IRR''#,##0.00', Enabled = 'true' where CurrencyCode = 'IRR'
	else insert into Currency values ('IRR', N'Iranian Rial', N'''IRR''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'ISK') 
		 update Currency set CurrencyName = N'Iceland Krona', DisplayFormat = N'#,##0.00''kr''', Enabled = 'true' where CurrencyCode = 'ISK'
	else insert into Currency values ('ISK', N'Iceland Krona', N'#,##0.00''kr''', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'JMD') 
		 update Currency set CurrencyName = N'Jamaican Dollar', DisplayFormat = N'''JA$''#,##0.00', Enabled = 'true' where CurrencyCode = 'JMD'
	else insert into Currency values ('JMD', N'Jamaican Dollar', N'''JA$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'JOD') 
		 update Currency set CurrencyName = N'Jordanian Dinar', DisplayFormat = N'''JOD''#,##0.000', Enabled = 'true' where CurrencyCode = 'JOD'
	else insert into Currency values ('JOD', N'Jordanian Dinar', N'''JOD''#,##0.000', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'JPY') 
		 update Currency set CurrencyName = N'Yen', DisplayFormat = N'''JP¥''#,##0', Enabled = 'true' where CurrencyCode = 'JPY'
	else insert into Currency values ('JPY', N'Yen', N'''JP¥''#,##0', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'KES') 
		 update Currency set CurrencyName = N'Kenyan Shilling', DisplayFormat = N'''KSh''#,##0.00', Enabled = 'true' where CurrencyCode = 'KES'
	else insert into Currency values ('KES', N'Kenyan Shilling', N'''KSh''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'KGS') 
		 update Currency set CurrencyName = N'Som', DisplayFormat = N'''som''#,##0.00', Enabled = 'true' where CurrencyCode = 'KGS'
	else insert into Currency values ('KGS', N'Som', N'''som''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'KHR') 
		 update Currency set CurrencyName = N'Riel', DisplayFormat = N'#,##0.00''KHR''', Enabled = 'true' where CurrencyCode = 'KHR'
	else insert into Currency values ('KHR', N'Riel', N'#,##0.00''KHR''', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'KMF') 
		 update Currency set CurrencyName = N'Comoro Franc', DisplayFormat = N'''KMF''#,##0', Enabled = 'true' where CurrencyCode = 'KMF'
	else insert into Currency values ('KMF', N'Comoro Franc', N'''KMF''#,##0', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'KPW') 
		 update Currency set CurrencyName = N'North Korean Won', DisplayFormat = N'''KPW''#,##0.00', Enabled = 'true' where CurrencyCode = 'KPW'
	else insert into Currency values ('KPW', N'North Korean Won', N'''KPW''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'KRW') 
		 update Currency set CurrencyName = N'Won', DisplayFormat = N'''KR₩''#,##0', Enabled = 'true' where CurrencyCode = 'KRW'
	else insert into Currency values ('KRW', N'Won', N'''KR₩''#,##0', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'KWD') 
		 update Currency set CurrencyName = N'Kuwaiti Dinar', DisplayFormat = N'''KWD''#,##0.000', Enabled = 'true' where CurrencyCode = 'KWD'
	else insert into Currency values ('KWD', N'Kuwaiti Dinar', N'''KWD''#,##0.000', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'KYD') 
		 update Currency set CurrencyName = N'Cayman Islands Dollar', DisplayFormat = N'''CI$''#,##0.00', Enabled = 'true' where CurrencyCode = 'KYD'
	else insert into Currency values ('KYD', N'Cayman Islands Dollar', N'''CI$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'KZT') 
		 update Currency set CurrencyName = N'Tenge', DisplayFormat = N'#,##0.00''KZT''', Enabled = 'true' where CurrencyCode = 'KZT'
	else insert into Currency values ('KZT', N'Tenge', N'#,##0.00''KZT''', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'LAK') 
		 update Currency set CurrencyName = N'Kip', DisplayFormat = N'''LA₭''#,##0.00', Enabled = 'true' where CurrencyCode = 'LAK'
	else insert into Currency values ('LAK', N'Kip', N'''LA₭''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'LBP') 
		 update Currency set CurrencyName = N'Lebanese Pound', DisplayFormat = N'''LBP''#,##0.00', Enabled = 'true' where CurrencyCode = 'LBP'
	else insert into Currency values ('LBP', N'Lebanese Pound', N'''LBP''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'LKR') 
		 update Currency set CurrencyName = N'Sri Lanka Rupee', DisplayFormat = N'''SLRs''#,##0.00', Enabled = 'true' where CurrencyCode = 'LKR'
	else insert into Currency values ('LKR', N'Sri Lanka Rupee', N'''SLRs''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'LRD') 
		 update Currency set CurrencyName = N'Liberian Dollar', DisplayFormat = N'''L$''#,##0.00', Enabled = 'true' where CurrencyCode = 'LRD'
	else insert into Currency values ('LRD', N'Liberian Dollar', N'''L$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'LSL') 
		 update Currency set CurrencyName = N'Loti', DisplayFormat = N'''LSL''#,##0.00', Enabled = 'true' where CurrencyCode = 'LSL'
	else insert into Currency values ('LSL', N'Loti', N'''LSL''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'LTL') 
		 update Currency set CurrencyName = N'Lithuanian Litas', DisplayFormat = N'#,##0.00''Lt''', Enabled = 'true' where CurrencyCode = 'LTL'
	else insert into Currency values ('LTL', N'Lithuanian Litas', N'#,##0.00''Lt''', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'LVL') 
		 update Currency set CurrencyName = N'Latvian Lats', DisplayFormat = N'#,##0.00''Ls''', Enabled = 'true' where CurrencyCode = 'LVL'
	else insert into Currency values ('LVL', N'Latvian Lats', N'#,##0.00''Ls''', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'LYD') 
		 update Currency set CurrencyName = N'Libyan Dinar', DisplayFormat = N'''LD''#,##0.000', Enabled = 'true' where CurrencyCode = 'LYD'
	else insert into Currency values ('LYD', N'Libyan Dinar', N'''LD''#,##0.000', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'MAD') 
		 update Currency set CurrencyName = N'Moroccan Dirham', DisplayFormat = N'''MAD''#,##0.00', Enabled = 'true' where CurrencyCode = 'MAD'
	else insert into Currency values ('MAD', N'Moroccan Dirham', N'''MAD''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'MDL') 
		 update Currency set CurrencyName = N'Moldovan Leu', DisplayFormat = N'''MDL''#,##0.00', Enabled = 'true' where CurrencyCode = 'MDL'
	else insert into Currency values ('MDL', N'Moldovan Leu', N'''MDL''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'MGA') 
		 update Currency set CurrencyName = N'Malagasy Ariary', DisplayFormat = N'''MGA''#,##0.0', Enabled = 'true' where CurrencyCode = 'MGA'
	else insert into Currency values ('MGA', N'Malagasy Ariary', N'''MGA''#,##0.0', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'MKD') 
		 update Currency set CurrencyName = N'Denar', DisplayFormat = N'''MKD''#,##0.00', Enabled = 'true' where CurrencyCode = 'MKD'
	else insert into Currency values ('MKD', N'Denar', N'''MKD''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'MMK') 
		 update Currency set CurrencyName = N'Kyat', DisplayFormat = N'''MMK''#,##0.00', Enabled = 'true' where CurrencyCode = 'MMK'
	else insert into Currency values ('MMK', N'Kyat', N'''MMK''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'MNT') 
		 update Currency set CurrencyName = N'Tugrik', DisplayFormat = N'''MN₮''#,##0.00', Enabled = 'true' where CurrencyCode = 'MNT'
	else insert into Currency values ('MNT', N'Tugrik', N'''MN₮''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'MOP') 
		 update Currency set CurrencyName = N'Pataca', DisplayFormat = N'''MOP$''#,##0.00', Enabled = 'true' where CurrencyCode = 'MOP'
	else insert into Currency values ('MOP', N'Pataca', N'''MOP$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'MRO') 
		 update Currency set CurrencyName = N'Ouguiya', DisplayFormat = N'''UM''#,##0.0', Enabled = 'true' where CurrencyCode = 'MRO'
	else insert into Currency values ('MRO', N'Ouguiya', N'''UM''#,##0.0', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'MUR') 
		 update Currency set CurrencyName = N'Mauritius Rupee', DisplayFormat = N'''MURs''#,##0.00', Enabled = 'true' where CurrencyCode = 'MUR'
	else insert into Currency values ('MUR', N'Mauritius Rupee', N'''MURs''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'MVR') 
		 update Currency set CurrencyName = N'Rufiyaa', DisplayFormat = N'''MRF''#,##0.00', Enabled = 'true' where CurrencyCode = 'MVR'
	else insert into Currency values ('MVR', N'Rufiyaa', N'''MRF''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'MWK') 
		 update Currency set CurrencyName = N'Kwacha', DisplayFormat = N'''MK''#,##0.00', Enabled = 'true' where CurrencyCode = 'MWK'
	else insert into Currency values ('MWK', N'Kwacha', N'''MK''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'MXN') 
		 update Currency set CurrencyName = N'Mexican Peso', DisplayFormat = N'''Mex$''#,##0.00', Enabled = 'true' where CurrencyCode = 'MXN'
	else insert into Currency values ('MXN', N'Mexican Peso', N'''Mex$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'MYR') 
		 update Currency set CurrencyName = N'Malaysian Ringgit', DisplayFormat = N'''RM''#,##0.00', Enabled = 'true' where CurrencyCode = 'MYR'
	else insert into Currency values ('MYR', N'Malaysian Ringgit', N'''RM''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'MZN') 
		 update Currency set CurrencyName = N'Merical', DisplayFormat = N'''MTn''#,##0.00', Enabled = 'true' where CurrencyCode = 'MZN'
	else insert into Currency values ('MZN', N'Merical', N'''MTn''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'NAD') 
		 update Currency set CurrencyName = N'Namibian Dollar', DisplayFormat = N'''N$''#,##0.00', Enabled = 'true' where CurrencyCode = 'NAD'
	else insert into Currency values ('NAD', N'Namibian Dollar', N'''N$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'NGN') 
		 update Currency set CurrencyName = N'Naira', DisplayFormat = N'''NG₦''#,##0.00', Enabled = 'true' where CurrencyCode = 'NGN'
	else insert into Currency values ('NGN', N'Naira', N'''NG₦''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'NIO') 
		 update Currency set CurrencyName = N'Cordoba Oro', DisplayFormat = N'''C$''#,##0.00', Enabled = 'true' where CurrencyCode = 'NIO'
	else insert into Currency values ('NIO', N'Cordoba Oro', N'''C$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'NOK') 
		 update Currency set CurrencyName = N'Norwegian Krone', DisplayFormat = N'#,##0.00''NOkr''', Enabled = 'true' where CurrencyCode = 'NOK'
	else insert into Currency values ('NOK', N'Norwegian Krone', N'#,##0.00''NOkr''', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'NPR') 
		 update Currency set CurrencyName = N'Nepalese Rupee', DisplayFormat = N'''NPRs''#,##0.00', Enabled = 'true' where CurrencyCode = 'NPR'
	else insert into Currency values ('NPR', N'Nepalese Rupee', N'''NPRs''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'NZD') 
		 update Currency set CurrencyName = N'New Zealand Dollar', DisplayFormat = N'''NZ$''#,##0.00', Enabled = 'true' where CurrencyCode = 'NZD'
	else insert into Currency values ('NZD', N'New Zealand Dollar', N'''NZ$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'OMR') 
		 update Currency set CurrencyName = N'Rial Omani', DisplayFormat = N'''OMR''#,##0.000', Enabled = 'true' where CurrencyCode = 'OMR'
	else insert into Currency values ('OMR', N'Rial Omani', N'''OMR''#,##0.000', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'PAB') 
		 update Currency set CurrencyName = N'Balboa', DisplayFormat = N'''B/.''#,##0.00', Enabled = 'true' where CurrencyCode = 'PAB'
	else insert into Currency values ('PAB', N'Balboa', N'''B/.''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'PEN') 
		 update Currency set CurrencyName = N'Nuevo Sol', DisplayFormat = N'''S/.''#,##0.00', Enabled = 'true' where CurrencyCode = 'PEN'
	else insert into Currency values ('PEN', N'Nuevo Sol', N'''S/.''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'PGK') 
		 update Currency set CurrencyName = N'Kina', DisplayFormat = N'''PGK''#,##0.00', Enabled = 'true' where CurrencyCode = 'PGK'
	else insert into Currency values ('PGK', N'Kina', N'''PGK''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'PHP') 
		 update Currency set CurrencyName = N'Philippine Peso', DisplayFormat = N'''PHP''#,##0.00', Enabled = 'true' where CurrencyCode = 'PHP'
	else insert into Currency values ('PHP', N'Philippine Peso', N'''PHP''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'PKR') 
		 update Currency set CurrencyName = N'Pakistan Rupee', DisplayFormat = N'''PKRs.''#,##0.00', Enabled = 'true' where CurrencyCode = 'PKR'
	else insert into Currency values ('PKR', N'Pakistan Rupee', N'''PKRs.''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'PLN') 
		 update Currency set CurrencyName = N'Zloty', DisplayFormat = N'#,##0.00''zł''', Enabled = 'true' where CurrencyCode = 'PLN'
	else insert into Currency values ('PLN', N'Zloty', N'#,##0.00''zł''', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'PYG') 
		 update Currency set CurrencyName = N'Guarani', DisplayFormat = N'''PYG''#,##0', Enabled = 'true' where CurrencyCode = 'PYG'
	else insert into Currency values ('PYG', N'Guarani', N'''PYG''#,##0', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'QAR') 
		 update Currency set CurrencyName = N'Qatari Rial', DisplayFormat = N'''QR''#,##0.00', Enabled = 'true' where CurrencyCode = 'QAR'
	else insert into Currency values ('QAR', N'Qatari Rial', N'''QR''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'RON') 
		 update Currency set CurrencyName = N'Leu', DisplayFormat = N'''RON''#,##0.00', Enabled = 'true' where CurrencyCode = 'RON'
	else insert into Currency values ('RON', N'Leu', N'''RON''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'RSD') 
		 update Currency set CurrencyName = N'Serbian Dinar', DisplayFormat = N'''RSD''#,##0.00', Enabled = 'true' where CurrencyCode = 'RSD'
	else insert into Currency values ('RSD', N'Serbian Dinar', N'''RSD''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'RUB') 
		 update Currency set CurrencyName = N'Russian Ruble', DisplayFormat = N'#,##0.00''руб''', Enabled = 'true' where CurrencyCode = 'RUB'
	else insert into Currency values ('RUB', N'Russian Ruble', N'#,##0.00''руб''', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'RWF') 
		 update Currency set CurrencyName = N'Rwanda Franc', DisplayFormat = N'''RF''#,##0', Enabled = 'true' where CurrencyCode = 'RWF'
	else insert into Currency values ('RWF', N'Rwanda Franc', N'''RF''#,##0', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'SAR') 
		 update Currency set CurrencyName = N'Saudi Riyal', DisplayFormat = N'''SR''#,##0.00', Enabled = 'true' where CurrencyCode = 'SAR'
	else insert into Currency values ('SAR', N'Saudi Riyal', N'''SR''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'SBD') 
		 update Currency set CurrencyName = N'Solomon Islands Dollar', DisplayFormat = N'''SI$''#,##0.00', Enabled = 'true' where CurrencyCode = 'SBD'
	else insert into Currency values ('SBD', N'Solomon Islands Dollar', N'''SI$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'SCR') 
		 update Currency set CurrencyName = N'Seychelles Rupee', DisplayFormat = N'''SCR''#,##0.00', Enabled = 'true' where CurrencyCode = 'SCR'
	else insert into Currency values ('SCR', N'Seychelles Rupee', N'''SCR''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'SDG') 
		 update Currency set CurrencyName = N'Sudanese Pound', DisplayFormat = N'''SDG''#,##0.00', Enabled = 'true' where CurrencyCode = 'SDG'
	else insert into Currency values ('SDG', N'Sudanese Pound', N'''SDG''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'SEK') 
		 update Currency set CurrencyName = N'Swedish Krona', DisplayFormat = N'#,##0.00''kr''', Enabled = 'true' where CurrencyCode = 'SEK'
	else insert into Currency values ('SEK', N'Swedish Krona', N'#,##0.00''kr''', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'SGD') 
		 update Currency set CurrencyName = N'Singapore Dollar', DisplayFormat = N'''S$''#,##0.00', Enabled = 'true' where CurrencyCode = 'SGD'
	else insert into Currency values ('SGD', N'Singapore Dollar', N'''S$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'SHP') 
		 update Currency set CurrencyName = N'Saint Helena Pound', DisplayFormat = N'''SH£''#,##0.00', Enabled = 'true' where CurrencyCode = 'SHP'
	else insert into Currency values ('SHP', N'Saint Helena Pound', N'''SH£''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'SKK') 
		 update Currency set CurrencyName = N'Slovak Koruna', DisplayFormat = N'#,##0.00''Sk''', Enabled = 'true' where CurrencyCode = 'SKK'
	else insert into Currency values ('SKK', N'Slovak Koruna', N'#,##0.00''Sk''', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'SLL') 
		 update Currency set CurrencyName = N'Leone', DisplayFormat = N'''Le''#,##0.00', Enabled = 'true' where CurrencyCode = 'SLL'
	else insert into Currency values ('SLL', N'Leone', N'''Le''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'SOS') 
		 update Currency set CurrencyName = N'Somali Shilling', DisplayFormat = N'''So. Sh.''#,##0.00', Enabled = 'true' where CurrencyCode = 'SOS'
	else insert into Currency values ('SOS', N'Somali Shilling', N'''So. Sh.''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'SRD') 
		 update Currency set CurrencyName = N'Surinam Dollar', DisplayFormat = N'''SR$''#,##0.00', Enabled = 'true' where CurrencyCode = 'SRD'
	else insert into Currency values ('SRD', N'Surinam Dollar', N'''SR$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'STD') 
		 update Currency set CurrencyName = N'Dobra', DisplayFormat = N'''Db''#,##0.00', Enabled = 'true' where CurrencyCode = 'STD'
	else insert into Currency values ('STD', N'Dobra', N'''Db''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'SYP') 
		 update Currency set CurrencyName = N'Syrian Pound', DisplayFormat = N'''SYP''#,##0.00', Enabled = 'true' where CurrencyCode = 'SYP'
	else insert into Currency values ('SYP', N'Syrian Pound', N'''SYP''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'SZL') 
		 update Currency set CurrencyName = N'Lilangeni', DisplayFormat = N'''SZL''#,##0.00', Enabled = 'true' where CurrencyCode = 'SZL'
	else insert into Currency values ('SZL', N'Lilangeni', N'''SZL''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'THB') 
		 update Currency set CurrencyName = N'Baht', DisplayFormat = N'''THB''#,##0.00', Enabled = 'true' where CurrencyCode = 'THB'
	else insert into Currency values ('THB', N'Baht', N'''THB''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'TJS') 
		 update Currency set CurrencyName = N'Somoni', DisplayFormat = N'''TJS''#,##0.00', Enabled = 'true' where CurrencyCode = 'TJS'
	else insert into Currency values ('TJS', N'Somoni', N'''TJS''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'TMM') 
		 update Currency set CurrencyName = N'Manat', DisplayFormat = N'''TMM''#,##0.00', Enabled = 'true' where CurrencyCode = 'TMM'
	else insert into Currency values ('TMM', N'Manat', N'''TMM''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'TND') 
		 update Currency set CurrencyName = N'Tunisian Dinar', DisplayFormat = N'''DT''#,##0.000', Enabled = 'true' where CurrencyCode = 'TND'
	else insert into Currency values ('TND', N'Tunisian Dinar', N'''DT''#,##0.000', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'TOP') 
		 update Currency set CurrencyName = N'Tongan Pa''anga', DisplayFormat = N'''T$''#,##0.00', Enabled = 'true' where CurrencyCode = 'TOP'
	else insert into Currency values ('TOP', N'Tongan Pa''anga', N'''T$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'TRY') 
		 update Currency set CurrencyName = N'new Turkish Lira', DisplayFormat = N'''YTL''#,##0.00', Enabled = 'true' where CurrencyCode = 'TRY'
	else insert into Currency values ('TRY', N'new Turkish Lira', N'''YTL''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'TTD') 
		 update Currency set CurrencyName = N'Trinidad and Tobago Dollar', DisplayFormat = N'''TT$''#,##0.00', Enabled = 'true' where CurrencyCode = 'TTD'
	else insert into Currency values ('TTD', N'Trinidad and Tobago Dollar', N'''TT$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'TWD') 
		 update Currency set CurrencyName = N'New Taiwan Dollar', DisplayFormat = N'''NT$''#,##0.00', Enabled = 'true' where CurrencyCode = 'TWD'
	else insert into Currency values ('TWD', N'New Taiwan Dollar', N'''NT$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'TZS') 
		 update Currency set CurrencyName = N'Tanzanian Shilling', DisplayFormat = N'#,##0.00''TZS''', Enabled = 'true' where CurrencyCode = 'TZS'
	else insert into Currency values ('TZS', N'Tanzanian Shilling', N'#,##0.00''TZS''', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'UAH') 
		 update Currency set CurrencyName = N'Hryvnia', DisplayFormat = N'#,##0.00''грн''', Enabled = 'true' where CurrencyCode = 'UAH'
	else insert into Currency values ('UAH', N'Hryvnia', N'#,##0.00''грн''', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'UGX') 
		 update Currency set CurrencyName = N'Uganda Shilling', DisplayFormat = N'''USh''#,##0.00', Enabled = 'true' where CurrencyCode = 'UGX'
	else insert into Currency values ('UGX', N'Uganda Shilling', N'''USh''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'USD') 
		 update Currency set CurrencyName = N'US Dollar', DisplayFormat = N'''US$''#,##0.00', Enabled = 'true' where CurrencyCode = 'USD'
	else insert into Currency values ('USD', N'US Dollar', N'''US$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'UYU') 
		 update Currency set CurrencyName = N'Peso Uruguayo', DisplayFormat = N'''UY$''#,##0.00', Enabled = 'true' where CurrencyCode = 'UYU'
	else insert into Currency values ('UYU', N'Peso Uruguayo', N'''UY$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'UZS') 
		 update Currency set CurrencyName = N'Uzbekistan Sum', DisplayFormat = N'''UZS''#,##0.00', Enabled = 'true' where CurrencyCode = 'UZS'
	else insert into Currency values ('UZS', N'Uzbekistan Sum', N'''UZS''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'VEF') 
		 update Currency set CurrencyName = N'Bolivar Fuerte', DisplayFormat = N'''Bs.F''#,##0.00', Enabled = 'true' where CurrencyCode = 'VEF'
	else insert into Currency values ('VEF', N'Bolivar Fuerte', N'''Bs.F''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'VND') 
		 update Currency set CurrencyName = N'Dong', DisplayFormat = N'#,##0.00''VN₫''', Enabled = 'true' where CurrencyCode = 'VND'
	else insert into Currency values ('VND', N'Dong', N'#,##0.00''VN₫''', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'VUV') 
		 update Currency set CurrencyName = N'Vatu', DisplayFormat = N'''Vt''#,##0', Enabled = 'true' where CurrencyCode = 'VUV'
	else insert into Currency values ('VUV', N'Vatu', N'''Vt''#,##0', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'WST') 
		 update Currency set CurrencyName = N'Tala', DisplayFormat = N'''WS$''#,##0.00', Enabled = 'true' where CurrencyCode = 'WST'
	else insert into Currency values ('WST', N'Tala', N'''WS$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'XAF') 
		 update Currency set CurrencyName = N'CFA Franc', DisplayFormat = N'''FCFA''#,##0', Enabled = 'true' where CurrencyCode = 'XAF'
	else insert into Currency values ('XAF', N'CFA Franc', N'''FCFA''#,##0', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'XCD') 
		 update Currency set CurrencyName = N'East Caribbean Dollar', DisplayFormat = N'''EC$''#,##0.00', Enabled = 'true' where CurrencyCode = 'XCD'
	else insert into Currency values ('XCD', N'East Caribbean Dollar', N'''EC$''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'XOF') 
		 update Currency set CurrencyName = N'CFA Franc BCEAO †', DisplayFormat = N'''CFA''#,##0', Enabled = 'true' where CurrencyCode = 'XOF'
	else insert into Currency values ('XOF', N'CFA Franc BCEAO †', N'''CFA''#,##0', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'XPF') 
		 update Currency set CurrencyName = N'CFP Franc', DisplayFormat = N'''XPF''#,##0', Enabled = 'true' where CurrencyCode = 'XPF'
	else insert into Currency values ('XPF', N'CFP Franc', N'''XPF''#,##0', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'YER') 
		 update Currency set CurrencyName = N'Yemeni Rial', DisplayFormat = N'''YER''#,##0.00', Enabled = 'true' where CurrencyCode = 'YER'
	else insert into Currency values ('YER', N'Yemeni Rial', N'''YER''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'ZAR') 
		 update Currency set CurrencyName = N'Rand', DisplayFormat = N'''ZAR''#,##0.00', Enabled = 'true' where CurrencyCode = 'ZAR'
	else insert into Currency values ('ZAR', N'Rand', N'''ZAR''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'ZMK') 
		 update Currency set CurrencyName = N'Kwacha', DisplayFormat = N'''ZK''#,##0.00', Enabled = 'true' where CurrencyCode = 'ZMK'
	else insert into Currency values ('ZMK', N'Kwacha', N'''ZK''#,##0.00', 'false')
	if exists (select currencycode from Currency where CurrencyCode = 'ZWL') 
		 update Currency set CurrencyName = N'Zimbabwe Dollar', DisplayFormat = N'''ZW$''#,##0.00', Enabled = 'true' where CurrencyCode = 'ZWL'
	else insert into Currency values ('ZWL', N'Zimbabwe Dollar', N'''ZW$''#,##0.00', 'false')

end
GO

----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2011.02.14'
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
