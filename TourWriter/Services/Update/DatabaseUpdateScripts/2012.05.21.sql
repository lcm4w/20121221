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
if ((select VersionNumber from AppSettings) <> '2012.04.30')
	RAISERROR (N'Database Update Script is not correct version for current database version',17,1)

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------
GO

if not Exists(select * from sys.columns where Name = N'ShowOnReport' and Object_ID = Object_ID(N'SupplierNote'))
	ALTER TABLE [dbo].[SupplierNote]
		ADD [ShowOnReport] bit NULL;
GO

ALTER PROCEDURE [dbo].[_SupplierSet_Sel_ByID]
@SupplierID INT
AS
SET NOCOUNT ON

/* Select parent tables first */

-- PaymentTerm --
SELECT
	[PaymentTermID],
	[PaymentDueID],
	[PaymentDuePeriod],
	[DepositAmount],
	[DepositType],
	[DepositDueID],
	[DepositDuePeriod]
FROM [dbo].[PaymentTerm]
WHERE
	[PaymentTermID] = -- PurchasePaymentTerms from Supplier
		(SELECT [PaymentTermID] FROM [Supplier] WHERE [SupplierID] = @SupplierID)
OR
	[PaymentTermID] IN -- PurchasePaymentTerms from Service
		(SELECT DISTINCT [PaymentTermID] FROM [Service] WHERE
			[ServiceID] IN ( 
				SELECT [ServiceID] FROM [dbo].[Service] 
					WHERE [SupplierID] = @SupplierID ))

-- Supplier --
SELECT
	[SupplierID],
	[SupplierName],
	[HostName],
	[StreetAddress],
	[PostAddress],
	[Postcode],
	[CityID],
	[RegionID],
	[StateID],
	[CountryID],
	[Phone],
	[MobilePhone],
	[FreePhone],
	[Fax],
	[Email],
	[Website],
	[Latitude],
	[Longitude],
	[GradeID],
	[GradeExternalID],
	[Description],
	[Comments],
	[CancellationPolicy],
	[BankDetails],
	[AccountingName],
	[NetTaxTypeID],
	[GrossTaxTypeID],
	[PaymentTermID],
	[DefaultMargin],
	[DefaultCheckinTime],
	[DefaultCheckoutTime],
	[ImportID],
	[ExportID],
	[BookingWebsite],
	[IsRecordActive],
	[ParentFolderID],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted]
FROM [dbo].[Supplier]
WHERE
	[SupplierID] = @SupplierID
			
-- Service --
SELECT
	[ServiceID],
	[ServiceName],
	[SupplierID],
	[ServiceTypeID],
	[Description],
	[Comments],
	[Warning],
	[MaxPax],
	[CheckinTime],
	[CheckoutTime],
	[CheckinMinutesEarly],
	[IsRecordActive],
	[CurrencyCode],
	[ChargeType],
	[PaymentTermID],
	[NetTaxTypeID],
	[GrossTaxTypeID],
	[Latitude],
	[Longitude],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[ImportID]
FROM [dbo].[Service]
WHERE
	[SupplierID] = @SupplierID
	
-- ServiceTime
SELECT
	[ServiceTimeID],
	[ServiceID],
	[StartTime],
	[EndTime],
	[Comment]
FROM [dbo].[ServiceTime]
WHERE 
	[ServiceID] IN ( 
		SELECT [ServiceID] FROM [dbo].[Service] 
		WHERE [SupplierID] = @SupplierID )

-- ServiceConfig --
SELECT
	[ServiceID],
	[ServiceConfigTypeID],
	[Note],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[ServiceConfig]
WHERE 
	[ServiceID] IN ( 
		SELECT [ServiceID] FROM [dbo].[Service] 
		WHERE [SupplierID] = @SupplierID )
	
-- Rate --	
SELECT
	[RateID],
	[ServiceID],
	[ValidFrom],
	[ValidTo],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[ImportID]
FROM [dbo].[Rate]	
WHERE 
	[ServiceID] IN ( 
		SELECT [ServiceID] FROM [dbo].[Service] 
		WHERE [SupplierID] = @SupplierID )
	
-- Option --
SELECT
	[OptionID],
	[OptionName],
	[OptionTypeID],
	[RateID],
	[Net],
	[Gross],
	[PricingOption],
	isnull([IsDefault], 0) as [IsDefault],
	[IsRecordActive],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[ImportID]
FROM [dbo].[Option]	
WHERE 
	[RateID] IN (		
		SELECT r.[RateID] 
		FROM   [dbo].[Rate] r, [dbo].[Service] s 
		WHERE  s.[SupplierID] = @SupplierID 
		AND    r.[ServiceID] = s.[ServiceID] )
			
-- Contact --			
SELECT
	[ContactID],
	[ContactName],
	[Title],
	[FirstName],
	[LastName],
	[StreetAddress],
	[PostName],
	[PostAddress],
	[CityID],
	[RegionID],
	[StateID],
	[CountryID],
	[PostCode],
	[WorkPhone],
	[HomePhone],
	[CellPhone],
	[Fax],
	[Email1],
	[Email2],
	[Website],
	[BirthDate],
	[Notes],
	[ParentFolderID],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted]
FROM [dbo].[Contact]
WHERE
	[ContactID] IN (
		SELECT [ContactID] FROM [dbo].[SupplierContact] 
		WHERE [SupplierID] = @SupplierID )

-- SupplierContacts --			
SELECT
	[SupplierID],
	[ContactID],
	[Description],
	[IsDefaultContact],
	[IsDefaultBilling],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[SupplierContact]
WHERE
	[SupplierID] = @SupplierID
	
-- SupplierCreditCard --		
SELECT
	[SupplierID],
	[CreditCardID],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[SupplierCreditCard]
WHERE
	[SupplierID] = @SupplierID

-- SupplierConfig --		
SELECT
	[SupplierID],
	[SupplierConfigTypeID],
	[Note],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[SupplierConfig]
WHERE
	[SupplierID] = @SupplierID

-- Message --
SELECT
	[MessageID],
	[MessageName],
	[MessageType],
	[MessageTo],
	[MessageFrom],
	[MessageFile],
	[MessageDate],
	[Description],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[Message]
WHERE
	[MessageID] IN (
		SELECT [MessageID] FROM [dbo].[SupplierMessage]
		WHERE [SupplierID] = @SupplierID)

-- SupplierMessage --
SELECT
	[SupplierID],
	[MessageID],
	[AddedOn],
	[AddedBy]
FROM [dbo].[SupplierMessage]
WHERE [SupplierID] = @SupplierID

-- SupplierText --
SELECT
	[SupplierTextID],
	[SupplierTextName],
	[SupplierID],
	[FileName],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[SupplierText]
WHERE
	[SupplierID] = @SupplierID

-- SupplierNote --
SELECT
	[SupplierNoteID],
	[SupplierID],
	[Note],
	[ShowOnReport],
	[RowVersion]
FROM [dbo].[SupplierNote]
WHERE
	[SupplierID] = @SupplierID
ORDER BY 
	SupplierNoteID

-- Discount --
SELECT
	[DiscountID],
	[ServiceID],
	[UnitsUsed],
	[UnitsFree],
	[DiscountType]
FROM [dbo].[Discount]
WHERE
	[ServiceID] IN ( 
		SELECT [ServiceID] FROM [dbo].[Service] 
		WHERE [SupplierID] = @SupplierID )
		
-- Content
SELECT c.* 
FROM [Content] c
inner join SupplierContent s on s.ContentID = c.ContentID
WHERE s.SupplierID = @SupplierID
UNION
SELECT c.* 
FROM [Content] c
inner join ServiceContent s on s.ContentID = c.ContentID
where ServiceID in (select ServiceID from [Service] where SupplierID = @SupplierID)

-- SupplierContent
select * from SupplierContent
where SupplierID = @SupplierID

-- ServiceContent
select * from ServiceContent
where ServiceID in (select ServiceID from [Service] where SupplierID = @SupplierID)

-- ServiceWarning
select * from ServiceWarning
where ServiceID in (select ServiceID from [Service] where SupplierID = @SupplierID)
GO


if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SupplierNote_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[SupplierNote_Ins]
GO

CREATE PROCEDURE [dbo].[SupplierNote_Ins]
	@SupplierID int,
	@Note varchar(2000),
	@ShowOnReport bit,
	@SupplierNoteID int OUTPUT
AS
INSERT [dbo].[SupplierNote]
(
	[SupplierID],
	[Note],
	[ShowOnReport]
)
VALUES
(
	@SupplierID,
	@Note,
	@ShowOnReport
)
SELECT @SupplierNoteID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SupplierNote_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[SupplierNote_Upd]
GO

CREATE PROCEDURE [dbo].[SupplierNote_Upd]
	@SupplierNoteID int,
	@SupplierID int,
	@Note varchar(2000),
	@RowVersion timestamp,
	@ShowOnReport bit
AS
UPDATE [dbo].[SupplierNote]
SET 
	[SupplierID] = @SupplierID,
	[Note] = @Note,
	[ShowOnReport] = @ShowOnReport
WHERE
	[SupplierNoteID] = @SupplierNoteID
	AND [RowVersion] = @RowVersion
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SupplierNote_Upd_BySupplierID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[SupplierNote_Upd_BySupplierID]
GO

CREATE PROCEDURE [dbo].[SupplierNote_Upd_BySupplierID]
	@SupplierID int,
	@SupplierIDOld int
AS
UPDATE [dbo].[SupplierNote]
SET
	[SupplierID] = @SupplierID
WHERE
	[SupplierID] = @SupplierIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SupplierNote_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[SupplierNote_Del]
GO

CREATE PROCEDURE [dbo].[SupplierNote_Del]
	@SupplierNoteID int,
	@RowVersion timestamp
AS
DELETE FROM [dbo].[SupplierNote]
WHERE
	[SupplierNoteID] = @SupplierNoteID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SupplierNote_Del_BySupplierID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[SupplierNote_Del_BySupplierID]
GO

CREATE PROCEDURE [dbo].[SupplierNote_Del_BySupplierID]
	@SupplierID int
AS
DELETE
FROM [dbo].[SupplierNote]
WHERE
	[SupplierID] = @SupplierID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SupplierNote_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[SupplierNote_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[SupplierNote_Sel_ByID]
	@SupplierNoteID int
AS
SET NOCOUNT ON
SELECT
	[SupplierNoteID],
	[SupplierID],
	[Note],
	[RowVersion],
	[ShowOnReport]
FROM [dbo].[SupplierNote]
WHERE
	[SupplierNoteID] = @SupplierNoteID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SupplierNote_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[SupplierNote_Sel_All]
GO

CREATE PROCEDURE [dbo].[SupplierNote_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[SupplierNoteID],
	[SupplierID],
	[Note],
	[RowVersion],
	[ShowOnReport]
FROM [dbo].[SupplierNote]
ORDER BY 
	[SupplierNoteID] ASC
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SupplierNote_Sel_BySupplierID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[SupplierNote_Sel_BySupplierID]
GO

CREATE PROCEDURE [dbo].[SupplierNote_Sel_BySupplierID]
	@SupplierID int
AS
SET NOCOUNT ON
SELECT
	[SupplierNoteID],
	[SupplierID],
	[Note],
	[RowVersion],
	[ShowOnReport]
FROM [dbo].[SupplierNote]
WHERE
	[SupplierID] = @SupplierID
GO




ALTER FUNCTION [dbo].[GetAdjustedItemGross] 
(	
	@itinMargin money,
	@stypeMargin money,
	@net money,
	@gross money,
	@comOrMup char(3),
	@minOrMax varchar(10)
)
RETURNS money
AS
BEGIN

	-- null check (zero is ok for discounts
	if (@net is null or @gross is null)
		return @gross
	
	declare @overrideMargin money
	set @overrideMargin = isnull(@itinMargin, @stypeMargin)
	
	-- nothing to do?
	if (@overrideMargin is null)
		return @gross -- no override
		
	-- adjust gross and we're done...	
    if (@comOrMup = 'grs')
		-- WARNING: assume 'user friendly' postive number, so subtract because taking comm OFF gross
		return @gross * (1 - (@overrideMargin/100))


	-- zero check before division calcs
	if (@net = 0 or @gross = 0)
		return @gross
		
	-- get the current margin (com or mup)
	declare @currentMargin money
	if (@comOrMup = 'com')
		set @currentMargin = (@gross-@net)/@gross*100
	if (@comOrMup = 'mup')
		set @currentMargin = (@gross-@net)/@net*100		
	
	-- are we constraining the minimum or maximun margin?
	if (@minOrMax = 'min' and @currentMargin > @overrideMargin)
		set @overrideMargin = @currentMargin		
	if (@minOrMax = 'max' and @currentMargin < @overrideMargin)
		set @overrideMargin = @currentMargin
		
	-- recalculate the gross based on override margin
	if (@comOrMup = 'com')
		set @gross = @net*100/(100-@overrideMargin)
	if (@comOrMup = 'mup')
		set @gross = @net*(1+@overrideMargin/100)
	
	return @gross	

/*
SELECT 80*100/(100-cast(10 as money)) mup10, 80*100/(100-cast(25 as money)) mup25, 80*100/(100-cast(50 as money)) mup50, 
	   80*(1+cast(10 as money)/100) com10, 80*(1+(cast(25 as money)/100)) com25, 80*(1+(cast(50 as money)/100)) com50

select 
 case when (dbo.GetAdjustedItemGross(null,10,80,100,'grs',null)     =  90.0000) then 'pass' else 'FAIL' end '1'
,case when (dbo.GetAdjustedItemGross(null,10,80,100,'grs','  ')     =  90.0000) then 'pass' else 'FAIL' end '1'
,case when (dbo.GetAdjustedItemGross(10  ,20,80,100,'grs','  ')     =  90.0000) then 'pass' else 'FAIL' end '1'

select 
 case when (dbo.GetAdjustedItemGross(10,50,80,100,'mup','   ')     =  88.0000) then 'pass' else 'FAIL' end '1'
,case when (dbo.GetAdjustedItemGross(10,50,80,100,'mup','min')     = 100.0000) then 'pass' else 'FAIL' end '2' 
,case when (dbo.GetAdjustedItemGross(10,50,80,100,'mup','max')     =  88.0000) then 'pass' else 'FAIL' end '3'

,case when (dbo.GetAdjustedItemGross(10,50,80,100,'com','   ')     =  88.8888) then 'pass' else 'FAIL' end '4'
,case when (dbo.GetAdjustedItemGross(10,50,80,100,'com','min')     = 100.0000) then 'pass' else 'FAIL' end '5'
,case when (dbo.GetAdjustedItemGross(10,50,80,100,'com','max')     =  88.8888) then 'pass' else 'FAIL' end '6'

,case when (dbo.GetAdjustedItemGross(null,50,80,100,'mup','   ')   = 120.0000) then 'pass' else 'FAIL' end '7'
,case when (dbo.GetAdjustedItemGross(null,50,80,100,'mup','min')   = 120.0000) then 'pass' else 'FAIL' end '8'
,case when (dbo.GetAdjustedItemGross(null,50,80,100,'mup','max')   = 100.0000) then 'pass' else 'FAIL' end '9'

,case when (dbo.GetAdjustedItemGross(null,50,80,100,'com','   ')   = 160.0000) then 'pass' else 'FAIL' end '10'
,case when (dbo.GetAdjustedItemGross(null,50,80,100,'com','min')   = 160.0000) then 'pass' else 'FAIL' end '11'
,case when (dbo.GetAdjustedItemGross(null,50,80,100,'com','max')   = 100.0000) then 'pass' else 'FAIL' end '12'

,case when (dbo.GetAdjustedItemGross(null,null,80,100,'mup','   ') = 100.0000) then 'pass' else 'FAIL' end '13'
,case when (dbo.GetAdjustedItemGross(null,null,80,100,'mup','min') = 100.0000) then 'pass' else 'FAIL' end '14'
,case when (dbo.GetAdjustedItemGross(null,null,80,100,'mup','max') = 100.0000) then 'pass' else 'FAIL' end '15'

,case when (dbo.GetAdjustedItemGross(null,null,80,100,'com','   ') = 100.0000) then 'pass' else 'FAIL' end '16'
,case when (dbo.GetAdjustedItemGross(null,null,80,100,'com','min') = 100.0000) then 'pass' else 'FAIL' end '17'
,case when (dbo.GetAdjustedItemGross(null,null,80,100,'com','max') = 100.0000) then 'pass' else 'FAIL' end '18'
*/				
				
END
GO

exec __RefreshViews;
GO
----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2012.05.21'
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