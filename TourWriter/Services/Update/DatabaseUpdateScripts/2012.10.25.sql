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
if ((select VersionNumber from AppSettings) <> '2012.10.10')
	RAISERROR (N'Database Update Script is not correct version for current database version',17,1)

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------

GO
PRINT N'Altering [dbo].[Contact]...';


GO
if not Exists(select * from sys.columns where Name = N'PassportNumber' and Object_ID = Object_ID(N'Contact'))
ALTER TABLE [dbo].[Contact]
    ADD [PassportNumber] VARCHAR (50) NULL,
        [PassportExpiry] DATETIME     NULL;


GO
PRINT N'Creating [dbo].[Allocation]...';


GO
if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Allocation]') and OBJECTPROPERTY(id, N'IsTable') = 1)
begin
CREATE TABLE [dbo].[Allocation] (
    [AllocationID] INT      IDENTITY (1, 1) NOT NULL,
    [ServiceID]    INT      NULL,
    [ItineraryID]  INT      NULL,
    [ValidFrom]    DATETIME NOT NULL,
    [ValidTo]      DATETIME NOT NULL,
    [Quantity]     INT      NOT NULL,
    [Release]      INT      NULL,
    CONSTRAINT [PK_Allocation] PRIMARY KEY CLUSTERED ([AllocationID] ASC)
);
ALTER TABLE [dbo].[Allocation] WITH NOCHECK
    ADD CONSTRAINT [FK_Allocation_Service] FOREIGN KEY ([ServiceID]) REFERENCES [dbo].[Service] ([ServiceID]) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE [dbo].[Allocation] WITH NOCHECK
    ADD CONSTRAINT [FK_Allocation_Itinerary] FOREIGN KEY ([ItineraryID]) REFERENCES [dbo].[Itinerary] ([ItineraryID]) ON DELETE CASCADE ON UPDATE CASCADE;
end

GO
PRINT N'Creating [dbo].[AllocationAgent]...';


GO
if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AllocationAgent]') and OBJECTPROPERTY(id, N'IsTable') = 1)
begin
CREATE TABLE [dbo].[AllocationAgent] (
    [AllocationID] INT NOT NULL,
    [AgentID]      INT NOT NULL,
    [Quantity]     INT NOT NULL,
    [Release]      INT NULL,
    CONSTRAINT [PK_AllocationAgent] PRIMARY KEY CLUSTERED ([AllocationID] ASC, [AgentID] ASC)
);
ALTER TABLE [dbo].[AllocationAgent] WITH NOCHECK
    ADD CONSTRAINT [FK_AllocationAgent_Allocation] FOREIGN KEY ([AllocationID]) REFERENCES [dbo].[Allocation] ([AllocationID]) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE [dbo].[AllocationAgent] WITH NOCHECK
    ADD CONSTRAINT [FK_AllocationAgent_Agent] FOREIGN KEY ([AgentID]) REFERENCES [dbo].[Agent] ([AgentID]) ON DELETE CASCADE ON UPDATE CASCADE;
end


GO
PRINT N'Creating [dbo].[AllocationOption]...';


GO
if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AllocationOption]') and OBJECTPROPERTY(id, N'IsTable') = 1)
begin
CREATE TABLE [dbo].[AllocationOption] (
    [AllocationID] INT      NOT NULL,
    [OptionTypeID] INT      NOT NULL,
    [AddedOn]      DATETIME NULL,
    CONSTRAINT [PK_AllocationFilter] PRIMARY KEY CLUSTERED ([AllocationID] ASC, [OptionTypeID] ASC)
);
ALTER TABLE [dbo].[AllocationOption] WITH NOCHECK
    ADD CONSTRAINT [FK_AllocationOption_Allocation] FOREIGN KEY ([AllocationID]) REFERENCES [dbo].[Allocation] ([AllocationID]) ON DELETE CASCADE ON UPDATE CASCADE;
ALTER TABLE [dbo].[AllocationOption] WITH NOCHECK
    ADD CONSTRAINT [FK_AllocationOption_OptionType] FOREIGN KEY ([OptionTypeID]) REFERENCES [dbo].[OptionType] ([OptionTypeID]) ON DELETE CASCADE ON UPDATE CASCADE;
end


GO


GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Allocation_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Allocation_Ins]
GO

CREATE PROCEDURE [dbo].[Allocation_Ins]
	@ServiceID int,
	@ItineraryID int,
	@ValidFrom datetime,
	@ValidTo datetime,
	@Quantity int,
	@Release int,
	@AllocationID int OUTPUT
AS
INSERT [dbo].[Allocation]
(
	[ServiceID],
	[ItineraryID],
	[ValidFrom],
	[ValidTo],
	[Quantity],
	[Release]
)
VALUES
(
	@ServiceID,
	@ItineraryID,
	@ValidFrom,
	@ValidTo,
	@Quantity,
	@Release
)
SELECT @AllocationID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Allocation_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Allocation_Upd]
GO

CREATE PROCEDURE [dbo].[Allocation_Upd]
	@AllocationID int,
	@ServiceID int,
	@ItineraryID int,
	@ValidFrom datetime,
	@ValidTo datetime,
	@Quantity int,
	@Release int
AS
UPDATE [dbo].[Allocation]
SET 
	[ServiceID] = @ServiceID,
	[ItineraryID] = @ItineraryID,
	[ValidFrom] = @ValidFrom,
	[ValidTo] = @ValidTo,
	[Quantity] = @Quantity,
	[Release] = @Release
WHERE
	[AllocationID] = @AllocationID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Allocation_Upd_ByServiceID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Allocation_Upd_ByServiceID]
GO

CREATE PROCEDURE [dbo].[Allocation_Upd_ByServiceID]
	@ServiceID int,
	@ServiceIDOld int
AS
UPDATE [dbo].[Allocation]
SET
	[ServiceID] = @ServiceID
WHERE
	[ServiceID] = @ServiceIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Allocation_Upd_ByItineraryID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Allocation_Upd_ByItineraryID]
GO

CREATE PROCEDURE [dbo].[Allocation_Upd_ByItineraryID]
	@ItineraryID int,
	@ItineraryIDOld int
AS
UPDATE [dbo].[Allocation]
SET
	[ItineraryID] = @ItineraryID
WHERE
	[ItineraryID] = @ItineraryIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Allocation_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Allocation_Del]
GO

CREATE PROCEDURE [dbo].[Allocation_Del]
	@AllocationID int
AS
DELETE FROM [dbo].[Allocation]
WHERE
	[AllocationID] = @AllocationID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Allocation_Del_ByServiceID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Allocation_Del_ByServiceID]
GO

CREATE PROCEDURE [dbo].[Allocation_Del_ByServiceID]
	@ServiceID int
AS
DELETE
FROM [dbo].[Allocation]
WHERE
	[ServiceID] = @ServiceID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Allocation_Del_ByItineraryID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Allocation_Del_ByItineraryID]
GO

CREATE PROCEDURE [dbo].[Allocation_Del_ByItineraryID]
	@ItineraryID int
AS
DELETE
FROM [dbo].[Allocation]
WHERE
	[ItineraryID] = @ItineraryID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Allocation_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Allocation_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[Allocation_Sel_ByID]
	@AllocationID int
AS
SET NOCOUNT ON
SELECT
	[AllocationID],
	[ServiceID],
	[ItineraryID],
	[ValidFrom],
	[ValidTo],
	[Quantity],
	[Release]
FROM [dbo].[Allocation]
WHERE
	[AllocationID] = @AllocationID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Allocation_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Allocation_Sel_All]
GO

CREATE PROCEDURE [dbo].[Allocation_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[AllocationID],
	[ServiceID],
	[ItineraryID],
	[ValidFrom],
	[ValidTo],
	[Quantity],
	[Release]
FROM [dbo].[Allocation]
ORDER BY 
	[AllocationID] ASC
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Allocation_Sel_ByServiceID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Allocation_Sel_ByServiceID]
GO

CREATE PROCEDURE [dbo].[Allocation_Sel_ByServiceID]
	@ServiceID int
AS
SET NOCOUNT ON
SELECT
	[AllocationID],
	[ServiceID],
	[ItineraryID],
	[ValidFrom],
	[ValidTo],
	[Quantity],
	[Release]
FROM [dbo].[Allocation]
WHERE
	[ServiceID] = @ServiceID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Allocation_Sel_ByItineraryID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Allocation_Sel_ByItineraryID]
GO

CREATE PROCEDURE [dbo].[Allocation_Sel_ByItineraryID]
	@ItineraryID int
AS
SET NOCOUNT ON
SELECT
	[AllocationID],
	[ServiceID],
	[ItineraryID],
	[ValidFrom],
	[ValidTo],
	[Quantity],
	[Release]
FROM [dbo].[Allocation]
WHERE
	[ItineraryID] = @ItineraryID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AllocationAgent_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AllocationAgent_Ins]
GO

CREATE PROCEDURE [dbo].[AllocationAgent_Ins]
	@AllocationID int,
	@AgentID int,
	@Quantity int,
	@Release int
AS
INSERT [dbo].[AllocationAgent]
(
	[AllocationID],
	[AgentID],
	[Quantity],
	[Release]
)
VALUES
(
	@AllocationID,
	@AgentID,
	@Quantity,
	@Release
)
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AllocationAgent_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AllocationAgent_Upd]
GO

CREATE PROCEDURE [dbo].[AllocationAgent_Upd]
	@AllocationID int,
	@AgentID int,
	@Quantity int,
	@Release int
AS
UPDATE [dbo].[AllocationAgent]
SET 
	[Quantity] = @Quantity,
	[Release] = @Release
WHERE
	[AllocationID] = @AllocationID
	AND [AgentID] = @AgentID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AllocationAgent_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AllocationAgent_Del]
GO

CREATE PROCEDURE [dbo].[AllocationAgent_Del]
	@AllocationID int,
	@AgentID int
AS
DELETE FROM [dbo].[AllocationAgent]
WHERE
	[AllocationID] = @AllocationID
	AND [AgentID] = @AgentID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AllocationAgent_Del_ByAllocationID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AllocationAgent_Del_ByAllocationID]
GO

CREATE PROCEDURE [dbo].[AllocationAgent_Del_ByAllocationID]
	@AllocationID int
AS
DELETE FROM [dbo].[AllocationAgent]
WHERE
	[AllocationID] = @AllocationID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AllocationAgent_Del_ByAgentID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AllocationAgent_Del_ByAgentID]
GO

CREATE PROCEDURE [dbo].[AllocationAgent_Del_ByAgentID]
	@AgentID int
AS
DELETE FROM [dbo].[AllocationAgent]
WHERE
	[AgentID] = @AgentID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AllocationAgent_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AllocationAgent_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[AllocationAgent_Sel_ByID]
	@AllocationID int,
	@AgentID int
AS
SET NOCOUNT ON
SELECT
	[AllocationID],
	[AgentID],
	[Quantity],
	[Release]
FROM [dbo].[AllocationAgent]
WHERE
	[AllocationID] = @AllocationID
	AND [AgentID] = @AgentID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AllocationAgent_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AllocationAgent_Sel_All]
GO

CREATE PROCEDURE [dbo].[AllocationAgent_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[AllocationID],
	[AgentID],
	[Quantity],
	[Release]
FROM [dbo].[AllocationAgent]
ORDER BY 
	[AllocationID] ASC
	, [AgentID] ASC
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AllocationAgent_Sel_ByAllocationID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AllocationAgent_Sel_ByAllocationID]
GO

CREATE PROCEDURE [dbo].[AllocationAgent_Sel_ByAllocationID]
	@AllocationID int
AS
SET NOCOUNT ON
SELECT
	[AllocationID],
	[AgentID],
	[Quantity],
	[Release]
FROM [dbo].[AllocationAgent]
WHERE
	[AllocationID] = @AllocationID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AllocationAgent_Sel_ByAgentID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AllocationAgent_Sel_ByAgentID]
GO

CREATE PROCEDURE [dbo].[AllocationAgent_Sel_ByAgentID]
	@AgentID int
AS
SET NOCOUNT ON
SELECT
	[AllocationID],
	[AgentID],
	[Quantity],
	[Release]
FROM [dbo].[AllocationAgent]
WHERE
	[AgentID] = @AgentID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AllocationOption_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AllocationOption_Ins]
GO

CREATE PROCEDURE [dbo].[AllocationOption_Ins]
	@AllocationID int,
	@OptionTypeID int,
	@AddedOn datetime
AS
INSERT [dbo].[AllocationOption]
(
	[AllocationID],
	[OptionTypeID],
	[AddedOn]
)
VALUES
(
	@AllocationID,
	@OptionTypeID,
	@AddedOn
)
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AllocationOption_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AllocationOption_Upd]
GO

CREATE PROCEDURE [dbo].[AllocationOption_Upd]
	@AllocationID int,
	@OptionTypeID int,
	@AddedOn datetime
AS
UPDATE [dbo].[AllocationOption]
SET 
	[AddedOn] = @AddedOn
WHERE
	[AllocationID] = @AllocationID
	AND [OptionTypeID] = @OptionTypeID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AllocationOption_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AllocationOption_Del]
GO

CREATE PROCEDURE [dbo].[AllocationOption_Del]
	@AllocationID int,
	@OptionTypeID int
AS
DELETE FROM [dbo].[AllocationOption]
WHERE
	[AllocationID] = @AllocationID
	AND [OptionTypeID] = @OptionTypeID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AllocationOption_Del_ByAllocationID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AllocationOption_Del_ByAllocationID]
GO

CREATE PROCEDURE [dbo].[AllocationOption_Del_ByAllocationID]
	@AllocationID int
AS
DELETE FROM [dbo].[AllocationOption]
WHERE
	[AllocationID] = @AllocationID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AllocationOption_Del_ByOptionTypeID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AllocationOption_Del_ByOptionTypeID]
GO

CREATE PROCEDURE [dbo].[AllocationOption_Del_ByOptionTypeID]
	@OptionTypeID int
AS
DELETE FROM [dbo].[AllocationOption]
WHERE
	[OptionTypeID] = @OptionTypeID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AllocationOption_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AllocationOption_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[AllocationOption_Sel_ByID]
	@AllocationID int,
	@OptionTypeID int
AS
SET NOCOUNT ON
SELECT
	[AllocationID],
	[OptionTypeID],
	[AddedOn]
FROM [dbo].[AllocationOption]
WHERE
	[AllocationID] = @AllocationID
	AND [OptionTypeID] = @OptionTypeID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AllocationOption_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AllocationOption_Sel_All]
GO

CREATE PROCEDURE [dbo].[AllocationOption_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[AllocationID],
	[OptionTypeID],
	[AddedOn]
FROM [dbo].[AllocationOption]
ORDER BY 
	[AllocationID] ASC
	, [OptionTypeID] ASC
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AllocationOption_Sel_ByAllocationID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AllocationOption_Sel_ByAllocationID]
GO

CREATE PROCEDURE [dbo].[AllocationOption_Sel_ByAllocationID]
	@AllocationID int
AS
SET NOCOUNT ON
SELECT
	[AllocationID],
	[OptionTypeID],
	[AddedOn]
FROM [dbo].[AllocationOption]
WHERE
	[AllocationID] = @AllocationID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[AllocationOption_Sel_ByOptionTypeID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[AllocationOption_Sel_ByOptionTypeID]
GO

CREATE PROCEDURE [dbo].[AllocationOption_Sel_ByOptionTypeID]
	@OptionTypeID int
AS
SET NOCOUNT ON
SELECT
	[AllocationID],
	[OptionTypeID],
	[AddedOn]
FROM [dbo].[AllocationOption]
WHERE
	[OptionTypeID] = @OptionTypeID
GO


---------------------------------------------------------------------------------------------------------------------

GO
PRINT N'Altering [dbo].[PurchaseItemPayments]...';


GO
ALTER FUNCTION [dbo].[PurchaseItemPayments]
( )
RETURNS 
    @result TABLE (
        [PurchaseItemID] INT          NULL,
        [PaymentDueDate] DATETIME     NULL,
        [PaymentDueName] VARCHAR (20) NULL,
        [PaymentAmount]  MONEY        NULL)
AS
BEGIN	

	declare @temp TABLE
	(	 
		PurchaseItemID int,
		PurchaseItemStartDate datetime,
		DepositTerms varchar(100),
		DepositType varchar(20),
		DepositAmount money,
		DepositDueDate datetime,
		BalanceDueDate datetime,
		TotalNet money
	) 
	insert into @temp 
	select detail.PurchaseItemID, PurchaseItemStartDate, DepositTerms, DepositType, DepositAmount, DepositDueDate, BalanceDueDate, detail.NetFinalTotal 
	from dbo.PurchaseItemDetail detail

	insert into @result
	select 
		PurchaseItemID, 
		DepositDueDate as PaymentDueDate,
		'Deposit' as PaymentDueName,
		case DepositType
			when 'c' then DepositAmount 
			when 'p' then TotalNet * DepositAmount / 100
		end as PaymentAmount
	from @temp where DepositTerms is not null
	UNION
	select 
		PurchaseItemID, 
		BalanceDueDate as PaymentDueDate,
		'Balance' as PaymentDueName, 
		case DepositType
			when 'c' then TotalNet - DepositAmount 
			when 'p' then TotalNet - (TotalNet * DepositAmount / 100)
		end as PaymentAmount
	from @temp where DepositTerms is not null
	UNION
	select 
		PurchaseItemID, 
		isnull(BalanceDueDate, PurchaseItemStartDate) as PaymentDueDate,
		'Full' as PaymentDueName,
		TotalNet as PaymentAmount
	from @temp where DepositTerms is null

RETURN 
END



---------------------------------------------------------------------------------------------------------------------



GO
PRINT N'Altering [dbo].[ContactDetail]...';


GO


ALTER VIEW [dbo].[ContactDetail]
AS
SELECT
    cont.ContactID,
    ContactName,
    Title,
    FirstName,
    LastName,
    JobDescription,
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
    FolderName,
    cont.ParentFolderID,
	(   SELECT STUFF(( 
			SELECT ', ' + c.ContactCategoryName
			FROM ContactCategory c
			INNER JOIN ContactContactCategory cc ON c.ContactCategoryID = cc.ContactCategoryID
			WHERE cc.ContactID = cont.ContactID
			FOR XML PATH (''), TYPE 
		).value('.', 'VARCHAR(MAX)'), 1, 2,'')
    ) AS Categories,
    (   SELECT STUFF(( 
			SELECT ', ' + a.AgentName
			FROM Contact c1
			LEFT JOIN AgentContact ac ON ac.ContactID = c1.ContactID
			LEFT JOIN Agent a ON a.AgentID = ac.AgentID
			WHERE c1.ContactID = cont.ContactID
			FOR XML PATH (''), TYPE 
		).value('.', 'VARCHAR(MAX)'), 1, 2,'')
    ) AS Agents,
    PassportNumber,
    PassportExpiry
FROM Contact AS cont
LEFT OUTER JOIN City AS city ON city.CityID = cont.CityID
LEFT OUTER JOIN Region AS region ON region.RegionID = cont.RegionID
LEFT OUTER JOIN [State] AS stat ON stat.StateID = cont.StateID
LEFT OUTER JOIN Country AS country ON country.CountryID = cont.CountryID
LEFT OUTER JOIN Folder AS fld ON fld.FolderId = cont.ParentFolderId
LEFT OUTER JOIN
(   SELECT ContactID, COUNT(ItineraryID) AS ItineraryCount
    FROM ItineraryMember mem
    LEFT OUTER JOIN ItineraryGroup grp ON mem.ItineraryGroupID = grp.ItineraryGroupID
    WHERE ContactID IS NOT NULL
    GROUP BY ContactID
) cnt ON cnt.ContactID = cont.ContactID
WHERE (IsDeleted IS NULL OR IsDeleted = 0);
GO
PRINT N'Refreshing [dbo].[ItineraryClientDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryClientDetail';


GO
PRINT N'Altering [dbo].[_AgentSet_Sel_All]...';


GO
/* Select from multiple tables for dataset fill command */
ALTER PROCEDURE [dbo].[_AgentSet_Sel_All]
AS
SET NOCOUNT ON

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
	[PaymentTermID] IN 
		(SELECT [PurchasePaymentTermID] FROM [Agent])
OR
	[PaymentTermID] IN 
		(SELECT [SalePaymentTermID] FROM [Agent])

-- Agent --
EXEC Agent_Sel_All

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
	[IsDeleted],
	[JobDescription],
	[PassportNumber],
	[PassportExpiry]
FROM [dbo].[Contact]
WHERE
	[ContactID] IN (
		SELECT [ContactID] FROM [dbo].[AgentContact]  )

-- AgentContact --
EXEC AgentContact_Sel_All

-- AgentMarkup --
EXEC AgentMargin_Sel_All
GO
PRINT N'Altering [dbo].[_AgentSet_Sel_ByID]...';


GO

/* Select from multiple tables for dataset fill command */
ALTER PROCEDURE [dbo].[_AgentSet_Sel_ByID]
	@AgentID int
AS
SET NOCOUNT ON

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
	[PaymentTermID] = (SELECT [PurchasePaymentTermID] FROM [Agent] WHERE [AgentID] = @AgentID)
OR
	[PaymentTermID] = (SELECT [SalePaymentTermID] FROM [Agent] WHERE [AgentID] = @AgentID)

-- Agent --
SELECT
	[AgentID],
	[AgentName],
	[ParentAgentID],
	[Address1],
	[Address2],
	[Address3],
	[Phone],
	[Fax],
	[Email],
	[TaxNumber],
	[InvoiceNumberMask],
	[PurchasePaymentTermID],
	[SalePaymentTermID],
	[LogoFile],
	[VoucherLogoFile],
	[NetComOrMup],
	[Comments],
	[AgentHeader],
	[RequestFooter],
	[ConfirmFooter],
	[RemitFooter],
	[ClientFooter],
	[VoucherFooter],
	[IsDefaultAgent],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[CurrencyCode],
	[NetMinOrMax]
FROM [dbo].[Agent]
WHERE
	[AgentID] = @AgentID

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
	[IsDeleted],
	[JobDescription],
	[PassportNumber],
	[PassportExpiry]
FROM [dbo].[Contact]
WHERE
	[ContactID] IN (
		SELECT [ContactID] FROM [dbo].[AgentContact] 
		WHERE [AgentID] = @AgentID )

-- AgentContact --
SELECT
	[AgentID],
	[ContactID],
	[Description],
	[IsDefaultContact],
	[IsDefaultBilling]
FROM [dbo].[AgentContact]
WHERE
	[AgentID] = @AgentID

-- AgentMargin --
SELECT
	[AgentID],
	[ServiceTypeID],
	[Margin]
FROM [dbo].[AgentMargin]
WHERE
	[AgentID] = @AgentID
GO
PRINT N'Altering [dbo].[_ContactSet_Copy_ByID]...';


GO
ALTER PROCEDURE [dbo].[_ContactSet_Copy_ByID]
@OrigContactID INT, @NewContactName VARCHAR (100), @AddedOn DATETIME, @AddedBy INT
AS
INSERT [dbo].[Contact]
(
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
	[IsRecordActive],
	[ParentFolderID],
	[AddedOn],
	[AddedBy],
	[JobDescription],
	[PassportNumber],
	[PassportExpiry]
)
SELECT
	@NewContactName,
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
	[IsRecordActive],
	[ParentFolderID],
	@AddedOn,
	@AddedBy,
	[JobDescription],
	[PassportNumber],
	[PassportExpiry]
FROM [dbo].[Contact]
WHERE
	[ContactID] = @OrigContactID

SELECT SCOPE_IDENTITY()
GO
PRINT N'Altering [dbo].[_ContactSet_Sel_ByID]...';


GO
/* Select from multiple tables for dataset fill command */
ALTER PROCEDURE [dbo].[_ContactSet_Sel_ByID]
	@ContactID int
AS
SET NOCOUNT ON

/* Select parent tables first */

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
	[IsRecordActive],
	[ParentFolderID],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[JobDescription],
	[PassportNumber],
	[PassportExpiry]
FROM [dbo].[Contact]
WHERE
	[ContactID] = @ContactID

SELECT
	[ContactID],
	[ContactCategoryID],
	[AddedBy]
FROM [dbo].[ContactContactCategory]
WHERE
	[ContactID] = @ContactID
GO
PRINT N'Altering [dbo].[_ItinerarySet_Sel_ByID]...';


GO

ALTER PROCEDURE [dbo].[_ItinerarySet_Sel_ByID]
@ItineraryID INT
AS
SET NOCOUNT ON

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
WHERE --*** SalePaymentTerms - do we need client or/else itinerary or/else agent? ***
	[PaymentTermID] = -- SalePaymentTerms from Itinerary
		(SELECT [PaymentTermID] FROM [Itinerary] WHERE [ItineraryID] = @ItineraryID)
OR
	[PaymentTermID] IN -- PurchasePaymentTerms from PurchaseItem
		(SELECT DISTINCT [PaymentTermID] FROM [PurchaseItem] WHERE
			[PurchaseLineID] IN ( 
				SELECT [PurchaseLineID] FROM [dbo].[PurchaseLine] 
					WHERE [ItineraryID] = @ItineraryID ))


-- *************************** Itinerary ****************************

-- Itinerary --
SELECT
	[ItineraryID],
	[ItineraryName],
	[DisplayName],
	[CustomCode],
	[ArriveDate],
	[ArriveCityID],
	[ArriveFlight],
	[ArriveNote],
	[DepartDate],
	[DepartCityID],
	[DepartFlight],
	[DepartNote],
	[NetComOrMup],
	[NetMargin],
	[GrossMarkup],
	[GrossOverride],
	[IsLockedGrossOverride],
	[PricingNote],
	[AgentID],
	[PaymentTermID],
	[ItineraryStatusID],
	[ItinerarySourceID],
	[CountryID],
	[AssignedTo],
	[DepartmentID],
	[BranchID],
	[PaxOverride],
	[Comments],
	[IsRecordActive],
	[IsReadOnly],
	[ParentFolderID],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[CurrencyCode],
	[NetMinOrMax],
	[AgentContactID]
FROM [dbo].[Itinerary]
WHERE
	[ItineraryID] = @ItineraryID

-- ItineraryPax --
SELECT
	[ItineraryPaxID],
	[ItineraryPaxName],
	[ItineraryID],
	[MemberCount],
	[MemberRooms],
	[StaffCount],
	[StaffRooms],
	[GrossMarkup],
	[GrossOverride]
FROM [dbo].[ItineraryPax]
WHERE
	[ItineraryID] = @ItineraryID
	
-- ItineraryMarginOverride --	
SELECT
	[ItineraryID],
	[ServiceTypeID],
	[Margin]
FROM [dbo].[ItineraryMarginOverride]
WHERE
	[ItineraryID] = @ItineraryID
	
-- PurchaseLine --
SELECT
	[PurchaseLineID],
	[ItineraryID],
	[SupplierID],
	[PurchaseLineName],
	[Comments],
	[NoteToSupplier],
	[NoteToVoucher],
	[NoteToClient],
	[Approved],
	[SupplierReference],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[PurchaseLine]
WHERE
	[ItineraryID] = @ItineraryID

-- PurchaseItem --
SELECT
	i.[PurchaseItemID],
	i.[PurchaseLineID],
	i.[OptionID],
	i.[PurchaseItemName],
	i.[BookingReference],
	i.[StartDate],
	i.[StartTime],
	i.[EndDate],
	i.[EndTime],
	i.[Net],
	i.[Gross],
	i.[CurrencyRate],
	i.[Quantity],
	i.[NumberOfDays],
	i.[PaymentTermID],
	i.[RequestStatusID],
	i.[IsLockedAccounting],
	i.[AddedOn],
	i.[AddedBy],
	i.[RowVersion],
	i.[DiscountUnits],
	i.[DiscountType],
	i.[IsInvoiced],
	i.[SortDate],
	-- add lookup columns
	r.[RateID],
	s.[ServiceID],
	o.[OptionTypeID],
	s.[ServiceTypeID],
	o.[OptionName],
	s.[ServiceName],
	stype.[ServiceTypeName],
	otype.[OptionTypeName],	
	s.[ChargeType],
	o.[IsDefault] as IsDefaultOptionType,
	s.[CurrencyCode]
FROM [dbo].[PurchaseItem] i 
INNER JOIN [Option] o ON o.OptionID = i.OptionID
INNER JOIN [Rate] r ON r.RateID = o.RateID
INNER JOIN [Service] s ON s.ServiceID = r.ServiceID
left outer join ServiceType as stype on s.ServiceTypeID = stype.ServiceTypeID
left outer join OptionType as otype on o.OptionTypeID = otype.OptionTypeID
WHERE [PurchaseLineID] IN ( 
		SELECT [PurchaseLineID] FROM [dbo].[PurchaseLine] 
		WHERE [ItineraryID] = @ItineraryID )	
ORDER BY [PurchaseLineID], [StartDate]

-- PurchaseItemNote --

SELECT
	[PurchaseItemNoteID],
	[PurchaseItemID],
	[FlagID],
	[Note]
FROM [dbo].[PurchaseItemNote]
WHERE [PurchaseItemID] IN (
	SELECT [PurchaseItemID] 
	FROM [dbo].[PurchaseItem] item 
	join PurchaseLine line on line.PurchaseLineID = item.PurchaseLineID
	WHERE [ItineraryID] = @ItineraryID )	
ORDER BY [FlagID], [PurchaseItemNoteID]

-- *********************** Members ********************************************

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
	[IsDeleted],
	[JobDescription],
	[PassportNumber],
	[PassportExpiry]
FROM [dbo].[Contact]
WHERE
	[ContactID] IN (
		SELECT c.[ContactID] 
		FROM   [dbo].[ItineraryMember] c, [dbo].[ItineraryGroup] b 
		WHERE  b.ItineraryID = @ItineraryID 
		AND    c.[ItineraryGroupID] = b.[ItineraryGroupID] )
			
-- ItineraryGroup --
SELECT
	[ItineraryGroupID],
	[ItineraryID],
	[ItineraryGroupName],
	[Comments],
	[NoteToClient],
	[NoteToSupplier],
	[CurrencyCode],
	[CurrencyRate],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[ItineraryGroup]
WHERE
	[ItineraryID] = @ItineraryID

-- ItineraryMember --
SELECT
	[ItineraryMemberID],
	[ItineraryGroupID],
	[ItineraryMemberName],
	[ContactID],
	[AgeGroupID],
	[Age],
	[ArriveDate],
	[ArriveCityID],
	[ArriveFlight],
	[DepartDate],
	[DepartCityID],
	[DepartFlight],
	[IsDefaultContact],
	[IsDefaultBilling],
	[Comments],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[Title],
	[RoomTypeID],
	[AgentID],
	[PriceOverride],
	[RoomName]
FROM [dbo].[ItineraryMember]
WHERE
	[ItineraryGroupID] IN ( 
		SELECT [ItineraryGroupID] FROM [dbo].[ItineraryGroup] 
		WHERE [ItineraryID] = @ItineraryID )

-- *********************** Messages *******************************************

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
		SELECT [MessageID] FROM [dbo].[ItineraryMessage]
		WHERE [ItineraryID] = @ItineraryID)

-- ItineraryMessage --
SELECT
	[ItineraryID],
	[MessageID],
	[AddedOn],
	[AddedBy]
FROM [dbo].[ItineraryMessage]
	WHERE [ItineraryID] = @ItineraryID

-- SupplierMessage (we don't want select data, just a placeholder for inserts) --
SELECT
	[SupplierID],
	[MessageID],
	[AddedOn],
	[AddedBy]
FROM [dbo].[SupplierMessage]
WHERE [SupplierID] = NULL AND[MessageID] = null

-- *********************** Lookup Tables **********************

-- Supplier Lookup --
SELECT DISTINCT  
	[SupplierID],
	[SupplierName],
	[HostName],
	[Phone],
	[MobilePhone],
	[FreePhone],
	[Fax],
	[Email],
	[GradeID],
	[GradeExternalID],
	[StreetAddress],
	[PostAddress],
	[CityID],
	[RegionID],
	[StateID],
	[CountryID],
	[NetTaxTypeID],
	[GrossTaxTypeID]
FROM [dbo].[Supplier] 
WHERE [SupplierID] IN (
	SELECT [SupplierID] 
	FROM [dbo].[PurchaseLine] 
	WHERE[ItineraryID] = @ItineraryID )

-- Option Lookup --
SELECT DISTINCT  
	op.[OptionID],
	op.[OptionName],
	sv.[ServiceName],
	sp.[SupplierID],
	sv.[ServiceTypeID],
	op.[OptionTypeID],	
	sv.[CheckinTime],	
	sv.[CheckoutTime],
	sv.[CheckinMinutesEarly],
	sv.[CurrencyCode],
	op.[Net],
	op.[Gross],
	op.[PricingOption],
	rt.[ValidFrom],
	rt.[ValidTo]
FROM [dbo].[Option] op 
INNER JOIN [dbo].[Rate] rt     ON op.[RateID]     = rt.[RateID] 
INNER JOIN [dbo].[Service] sv  ON rt.[ServiceID]   = sv.[ServiceID]
INNER JOIN [dbo].[Supplier] sp ON sv.[SupplierID] = sp.[SupplierID]
WHERE op.[OptionID] IN (
	SELECT [OptionID] 
	FROM [dbo].[PurchaseItem] i, [dbo].[PurchaseLine] l 
	WHERE i.[PurchaseLineID] = l.[PurchaseLineID]
	AND [ItineraryID] = @ItineraryID )

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
WHERE [SupplierID] IN (
	SELECT [SupplierID] 
	FROM [dbo].[PurchaseLine] 
	WHERE[ItineraryID] = @ItineraryID )

-- *********************** Publising *******************************************
SELECT
	[ItineraryPubFileID],
	[ItineraryPubFileName],
	[ItineraryID],
	[DayTemplateFile],
	[Layout],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[ItineraryPubFile]
WHERE
	[ItineraryID] = @ItineraryID


-- *********************** Payments *******************************************
SELECT
	[ItineraryPaymentID],
	[ItineraryMemberID],
	[Comments],
	[Amount],
	[PaymentDate],
	[PaymentTypeID],
	[ItinerarySaleID],
	[IsLockedAccounting],
	[RowVersion]
FROM [dbo].ItineraryPayment
WHERE
	[ItineraryMemberID] IN ( 
		SELECT m.[ItineraryMemberID] 
		FROM [dbo].[ItineraryMember] m, [dbo].[ItineraryGroup] g 
		WHERE g.[ItineraryID] = @ItineraryID
		AND m.[ItineraryGroupID] = g.[ItineraryGroupID] )
ORDER BY 
	[ItineraryPaymentID] ASC

-- *********************** Sales **********************************************
SELECT
	[ItinerarySaleID],
	[ItineraryID],
	[Comments],
	[SaleDate],
	[IsLockedAccounting]
FROM [dbo].[ItinerarySale]
WHERE [ItineraryID] = @ItineraryID

SELECT
	alloc.[ItinerarySaleID],
	alloc.[ServiceTypeID],
	[Amount]
FROM [dbo].[ItinerarySaleAllocation] alloc
LEFT OUTER JOIN [dbo].[ItinerarySale] sale ON alloc.ItinerarySaleID = sale.ItinerarySaleID
WHERE sale.[ItineraryID] = @ItineraryID

-- Discounts --

SELECT
	[DiscountID],
	[ServiceID],
	[UnitsUsed],
	[UnitsFree],
	[DiscountType]
FROM [dbo].[Discount]
WHERE
	[ServiceID] in 
	(	
		select distinct s.ServiceID
		from purchaseitem i
		inner join PurchaseLine line on i.PurchaseLineID = line.PurchaseLineID 
			and line.ItineraryID = @ItineraryID
		left outer join [Option] o on o.OptionID = i.OptionID
		left outer join [Rate] r on r.RateID = o.RateID
		left outer join [Service] s on s.ServiceID = r.ServiceID 
	)
	
-- GROUPS --		
SELECT
	o.[PurchaseItemID],
	o.[ItineraryPaxID],
	o.[MemberCount],
	o.[MemberRooms],
	o.[StaffCount],
	o.[StaffRooms]
FROM [dbo].[ItineraryPaxOverride] o
inner join PurchaseItem i on o.PurchaseItemID = i.PurchaseItemID
inner join PurchaseLine l on i.PurchaseLineID = l.PurchaseLineID
WHERE l.ItineraryID = @ItineraryID

-- ServiceWarnings
SELECT [ServiceWarningID]
		,[ServiceID]
		,[Description]
		,[ValidFrom]
		,[ValidTo]
	FROM [ServiceWarning]
WHERE
	[ServiceID] in 
	(	
		select distinct s.ServiceID
		from purchaseitem i
		inner join PurchaseLine line on i.PurchaseLineID = line.PurchaseLineID 
			and line.ItineraryID = @ItineraryID
		left outer join [Option] o on o.OptionID = i.OptionID
		left outer join [Rate] r on r.RateID = o.RateID
		left outer join [Service] s on s.ServiceID = r.ServiceID 
	)
AND ValidTo > getdate()

-- TASKS --
SELECT
	t.[TaskID],
	[TaskName],
	[DateDue],
	[DateCompleted],
	[Note]
FROM [dbo].[Task] t
inner join ItineraryTask i on i.TaskID = t.TaskID
where i.ItineraryID = @ItineraryID

-- ItineraryTask --
SELECT
	[ItineraryTaskID],
	[ItineraryID],
	[TaskID]
FROM [dbo].[ItineraryTask]
where ItineraryID = @ItineraryID

-- RoomType --
SELECT
	[RoomTypeID],
	[ItineraryID],
	[OptionTypeID],
	[RoomTypeName],
	[Quantity]
FROM [dbo].[RoomType]
where ItineraryID = @ItineraryID

-- Allocations
SELECT [AllocationID]
      ,[ServiceID]
      ,[ItineraryID]
      ,[ValidFrom]
      ,[ValidTo]
      ,[Quantity]
      ,[Release]
  FROM [TourWriterDev].[dbo].[Allocation]
  where ItineraryID = @ItineraryID

SELECT [AllocationID]
      ,[AgentID]
      ,[Quantity]
      ,[Release]
  FROM [TourWriterDev].[dbo].[AllocationAgent]
  WHERE [AllocationID] IN ( 
    SELECT [AllocationID] FROM [dbo].[Allocation] a
    where ItineraryID = @ItineraryID )
GO
PRINT N'Altering [dbo].[_SupplierSet_Sel_ByID]...';


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
	[IsDeleted],
	[JobDescription],
	[PassportNumber],
	[PassportExpiry]
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

-- Allocations
SELECT [AllocationID]
      ,[ServiceID]
      ,[ItineraryID]
      ,[ValidFrom]
      ,[ValidTo]
      ,[Quantity]
      ,[Release]
  FROM [TourWriterDev].[dbo].[Allocation]
  WHERE [ServiceID] IN ( 
    SELECT [ServiceID] FROM [dbo].[Service] 
	WHERE [SupplierID] = @SupplierID )

SELECT [AllocationID]
      ,[AgentID]
      ,[Quantity]
      ,[Release]
  FROM [TourWriterDev].[dbo].[AllocationAgent]
  WHERE [AllocationID] IN ( 
    SELECT [AllocationID] FROM [dbo].[Allocation] a
    LEFT JOIN [Service] s on s.ServiceID = a.ServiceID
	WHERE [SupplierID] = @SupplierID )

SELECT [AllocationID]
      ,[OptionTypeID]
      ,[AddedOn]
  FROM [TourWriterDev].[dbo].[AllocationOption]
  WHERE [AllocationID] IN ( 
    SELECT [AllocationID] FROM [dbo].[Allocation] a
    LEFT JOIN [Service] s on s.ServiceID = a.ServiceID
	WHERE [SupplierID] = @SupplierID )
GO
PRINT N'Altering [dbo].[_UserSet_Sel_All]...';


GO



/* Select from multiple tables for dataset fill command */
ALTER PROCEDURE [dbo].[_UserSet_Sel_All]
AS
SET NOCOUNT ON

-- Contact (dataset parent table first)-- 
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
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[JobDescription],
	[PassportNumber],
	[PassportExpiry]
FROM [dbo].[Contact]
WHERE [ContactID] IN (
	SELECT [ContactID] FROM [dbo].[User])

EXEC User_Sel_All
EXEC Role_Sel_All
EXEC UserRole_Sel_All
EXEC Permission_Sel_All
EXEC RolePermission_Sel_All
GO
PRINT N'Altering [dbo].[_UserSet_Sel_ByID]...';


GO




/* Select from multiple tables for dataset fill command */
ALTER PROCEDURE [dbo].[_UserSet_Sel_ByID]
	@UserID int
AS
SET NOCOUNT ON

-- Contact (dataset parent table first)-- 
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
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[JobDescription],
	[PassportNumber],
	[PassportExpiry]
FROM [dbo].[Contact]
WHERE [ContactID] = (
	SELECT [ContactID] FROM [dbo].[User] WHERE [UserID] = @UserID)

-- User -- 
SELECT
	[UserID],
	[UserName],
	[Password],
    [DisplayName],
	[Email],
	[ContactID],
	[IsRecordActive],
	[MustChangePassword],
	[IsExpertUser],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[User]
WHERE
	[UserID] = @UserID	
	
-- Role --	
SELECT
	[RoleID],
	[RoleName],
	[Description],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[Role]
WHERE [RoleID] IN ( 
	SELECT [RoleID] FROM [dbo].[UserRole] WHERE [UserID] = @UserID )
	
-- UserRole -- 	
SELECT
	[UserID],
	[RoleID],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[UserRole]
WHERE
	[UserID] = @UserID

-- Permission -- 
SELECT 
	[PermissionID],
	[PermissionName]
FROM [dbo].[Permission]
WHERE [PermissionID] IN ( 
	SELECT [PermissionID] FROM [dbo].[RolePermission] 
	WHERE [RoleID] IN ( 
		SELECT [RoleID] FROM [dbo].[UserRole] WHERE [UserID] = @UserID ))

-- RolePermission -- 
SELECT
	[RoleID],
	[PermissionID],
	[AddedOn],
	[AddedBy],
	[RowVersion]
FROM [dbo].[RolePermission]
WHERE
	[RoleID] IN ( 
		SELECT [RoleID] FROM [dbo].[UserRole] WHERE [UserID] = @UserID )
GO
PRINT N'Altering [dbo].[Contact_Ins]...';


GO

ALTER PROCEDURE [dbo].[Contact_Ins]
	@ContactName varchar(100),
	@Title varchar(50),
	@FirstName varchar(100),
	@LastName varchar(100),
	@StreetAddress varchar(500),
	@PostName varchar(100),
	@PostAddress varchar(500),
	@CityID int,
	@RegionID int,
	@StateID int,
	@CountryID int,
	@PostCode varchar(20),
	@WorkPhone varchar(50),
	@HomePhone varchar(50),
	@CellPhone varchar(50),
	@Fax varchar(50),
	@Email1 varchar(100),
	@Email2 varchar(100),
	@Website varchar(150),
	@BirthDate datetime,
	@Notes varchar(8000),
	@IsRecordActive bit,
	@ParentFolderID int,
	@AddedOn datetime,
	@AddedBy int,
	@IsDeleted bit,
	@JobDescription varchar(2000),
	@PassportNumber varchar(50),
	@PassportExpiry datetime,
	@ContactID int OUTPUT
AS
INSERT [dbo].[Contact]
(
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
	[IsRecordActive],
	[ParentFolderID],
	[AddedOn],
	[AddedBy],
	[IsDeleted],
	[JobDescription],
	[PassportNumber],
	[PassportExpiry]
)
VALUES
(
	ISNULL(@ContactName, ('')),
	@Title,
	@FirstName,
	@LastName,
	@StreetAddress,
	@PostName,
	@PostAddress,
	@CityID,
	@RegionID,
	@StateID,
	@CountryID,
	@PostCode,
	@WorkPhone,
	@HomePhone,
	@CellPhone,
	@Fax,
	@Email1,
	@Email2,
	@Website,
	@BirthDate,
	@Notes,
	@IsRecordActive,
	@ParentFolderID,
	@AddedOn,
	@AddedBy,
	@IsDeleted,
	@JobDescription,
	@PassportNumber,
	@PassportExpiry
)
SELECT @ContactID=SCOPE_IDENTITY()
GO
PRINT N'Altering [dbo].[Contact_Sel_All]...';


GO

ALTER PROCEDURE [dbo].[Contact_Sel_All]
AS
SET NOCOUNT ON
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
	[IsRecordActive],
	[ParentFolderID],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[JobDescription],
	[PassportNumber],
	[PassportExpiry]
FROM [dbo].[Contact]
ORDER BY 
	[ContactID] ASC
GO
PRINT N'Altering [dbo].[Contact_Sel_ByID]...';


GO

ALTER PROCEDURE [dbo].[Contact_Sel_ByID]
	@ContactID int
AS
SET NOCOUNT ON
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
	[IsRecordActive],
	[ParentFolderID],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted],
	[JobDescription],
	[PassportNumber],
	[PassportExpiry]
FROM [dbo].[Contact]
WHERE
	[ContactID] = @ContactID
GO
PRINT N'Altering [dbo].[Contact_Upd]...';


GO

ALTER PROCEDURE [dbo].[Contact_Upd]
	@ContactID int,
	@ContactName varchar(100),
	@Title varchar(50),
	@FirstName varchar(100),
	@LastName varchar(100),
	@StreetAddress varchar(500),
	@PostName varchar(100),
	@PostAddress varchar(500),
	@CityID int,
	@RegionID int,
	@StateID int,
	@CountryID int,
	@PostCode varchar(20),
	@WorkPhone varchar(50),
	@HomePhone varchar(50),
	@CellPhone varchar(50),
	@Fax varchar(50),
	@Email1 varchar(100),
	@Email2 varchar(100),
	@Website varchar(150),
	@BirthDate datetime,
	@Notes varchar(8000),
	@IsRecordActive bit,
	@ParentFolderID int,
	@AddedOn datetime,
	@AddedBy int,
	@RowVersion timestamp,
	@IsDeleted bit,
	@JobDescription varchar(2000),
	@PassportNumber varchar(50),
	@PassportExpiry datetime
AS
UPDATE [dbo].[Contact]
SET 
	[ContactName] = @ContactName,
	[Title] = @Title,
	[FirstName] = @FirstName,
	[LastName] = @LastName,
	[StreetAddress] = @StreetAddress,
	[PostName] = @PostName,
	[PostAddress] = @PostAddress,
	[CityID] = @CityID,
	[RegionID] = @RegionID,
	[StateID] = @StateID,
	[CountryID] = @CountryID,
	[PostCode] = @PostCode,
	[WorkPhone] = @WorkPhone,
	[HomePhone] = @HomePhone,
	[CellPhone] = @CellPhone,
	[Fax] = @Fax,
	[Email1] = @Email1,
	[Email2] = @Email2,
	[Website] = @Website,
	[BirthDate] = @BirthDate,
	[Notes] = @Notes,
	[IsRecordActive] = @IsRecordActive,
	[ParentFolderID] = @ParentFolderID,
	[AddedOn] = @AddedOn,
	[AddedBy] = @AddedBy,
	[IsDeleted] = @IsDeleted,
	[JobDescription] = @JobDescription,
	[PassportNumber] = @PassportNumber,
	[PassportExpiry] = @PassportExpiry
WHERE
	[ContactID] = @ContactID
	AND [RowVersion] = @RowVersion
GO
PRINT N'Refreshing [dbo].[_UserSet_Sel_ByUsernamePassword]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._UserSet_Sel_ByUsernamePassword';


---------------------------------------------------------------------------------------------------------------------

GO
PRINT N'Refreshing [dbo].[ContactDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ContactDetail';


GO
PRINT N'Refreshing [dbo].[ItineraryDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryDetail';


GO
PRINT N'Refreshing [dbo].[ItineraryClientDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryClientDetail';


GO
PRINT N'Refreshing [dbo].[ItineraryPaymentDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryPaymentDetail';


GO
PRINT N'Refreshing [dbo].[ItinerarySaleAllocationDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItinerarySaleAllocationDetail';


GO
PRINT N'Refreshing [dbo].[ItinerarySaleDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItinerarySaleDetail';


GO
PRINT N'Refreshing [dbo].[ItineraryServiceTypeDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.ItineraryServiceTypeDetail';


GO
PRINT N'Refreshing [dbo].[PurchaseItemDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.PurchaseItemDetail';


GO
PRINT N'Refreshing [dbo].[PurchaseItemPaymentsDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.PurchaseItemPaymentsDetail';


GO
PRINT N'Refreshing [dbo].[PurchaseItemPayments]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.PurchaseItemPayments';


GO
PRINT N'Refreshing [dbo].[__Repair_OrphanedMenuItems]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.__Repair_OrphanedMenuItems';


GO
PRINT N'Refreshing [dbo].[_AgentSet_Sel_All]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._AgentSet_Sel_All';


GO
PRINT N'Refreshing [dbo].[_AgentSet_Sel_ByID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._AgentSet_Sel_ByID';


GO
PRINT N'Refreshing [dbo].[_Contact_New]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._Contact_New';


GO
PRINT N'Refreshing [dbo].[_Contact_Rename]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._Contact_Rename';


GO
PRINT N'Refreshing [dbo].[_ContactSet_Copy_ByID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._ContactSet_Copy_ByID';


GO
PRINT N'Refreshing [dbo].[_ContactSet_Sel_ByID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._ContactSet_Sel_ByID';


GO
PRINT N'Refreshing [dbo].[_ItinerarySet_Sel_ByID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._ItinerarySet_Sel_ByID';


GO
PRINT N'Refreshing [dbo].[_SupplierSet_Sel_ByID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._SupplierSet_Sel_ByID';


GO
PRINT N'Refreshing [dbo].[_UserSet_Sel_All]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._UserSet_Sel_All';


GO
PRINT N'Refreshing [dbo].[_UserSet_Sel_ByID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._UserSet_Sel_ByID';


GO
PRINT N'Refreshing [dbo].[Contact_Del]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Contact_Del';


GO
PRINT N'Refreshing [dbo].[Contact_Ins]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Contact_Ins';


GO
PRINT N'Refreshing [dbo].[Contact_Sel_All]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Contact_Sel_All';


GO
PRINT N'Refreshing [dbo].[Contact_Sel_ByID]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Contact_Sel_ByID';


GO
PRINT N'Refreshing [dbo].[Contact_Upd]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.Contact_Upd';


GO
PRINT N'Refreshing [dbo].[_UserSet_Sel_ByUsernamePassword]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo._UserSet_Sel_ByUsernamePassword';


GO
PRINT N'Checking existing data against newly created constraints';



GO
ALTER TABLE [dbo].[Allocation] WITH CHECK CHECK CONSTRAINT [FK_Allocation_Service];

ALTER TABLE [dbo].[Allocation] WITH CHECK CHECK CONSTRAINT [FK_Allocation_Itinerary];

ALTER TABLE [dbo].[AllocationAgent] WITH CHECK CHECK CONSTRAINT [FK_AllocationAgent_Allocation];

ALTER TABLE [dbo].[AllocationAgent] WITH CHECK CHECK CONSTRAINT [FK_AllocationAgent_Agent];

ALTER TABLE [dbo].[AllocationOption] WITH CHECK CHECK CONSTRAINT [FK_AllocationOption_OptionType];


GO
PRINT N'Update complete.'
GO





exec __RefreshViews;
GO
----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2012.10.25'
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