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
if ((select VersionNumber from AppSettings) <> '2012.07.19')
	RAISERROR (N'Database Update Script is not correct version for current database version',17,1)

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------
GO

if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Task]') and OBJECTPROPERTY(id, N'IsTable') = 1)
begin

	CREATE TABLE [dbo].[Task](
		[TaskID] [int] IDENTITY(1,1) NOT NULL,
		[TaskName] [varchar](50) NOT NULL,
		[DateDue] [datetime] NULL,
		[DateCompleted] [datetime] NULL,
		[Note] [varchar](500) NULL,
	 CONSTRAINT [PK_Task] PRIMARY KEY CLUSTERED 
	(
		[TaskID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

end
GO

if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryTask]') and OBJECTPROPERTY(id, N'IsTable') = 1)
begin

	CREATE TABLE [dbo].[ItineraryTask](
		[ItineraryTaskID] [int] IDENTITY(1,1) NOT NULL,
		[ItineraryID] [int] NOT NULL,
		[TaskID] [int] NOT NULL,
	 CONSTRAINT [PK_ItineraryTask] PRIMARY KEY CLUSTERED 
	(
		[ItineraryTaskID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
	 CONSTRAINT [IX_ItineraryTask] UNIQUE NONCLUSTERED 
	(
		[TaskID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[ItineraryTask]  WITH CHECK ADD  CONSTRAINT [FK_ItineraryTask_Itinerary] FOREIGN KEY([ItineraryID])
	REFERENCES [dbo].[Itinerary] ([ItineraryID])
	ON UPDATE CASCADE
	ON DELETE CASCADE

	ALTER TABLE [dbo].[ItineraryTask] CHECK CONSTRAINT [FK_ItineraryTask_Itinerary]

	ALTER TABLE [dbo].[ItineraryTask]  WITH CHECK ADD  CONSTRAINT [FK_ItineraryTask_Task] FOREIGN KEY([TaskID])
	REFERENCES [dbo].[Task] ([TaskID])
	ON UPDATE CASCADE
	ON DELETE CASCADE

	ALTER TABLE [dbo].[ItineraryTask] CHECK CONSTRAINT [FK_ItineraryTask_Task]

end
GO

if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DefaultTask]') and OBJECTPROPERTY(id, N'IsTable') = 1)
begin

	CREATE TABLE [dbo].[DefaultTask](
		[DefaultTaskID] [int] IDENTITY(1,1) NOT NULL,
		[TaskID] [int] NOT NULL,
		[Type] [varchar](10) NOT NULL,
	 CONSTRAINT [PK_DefaultTask] PRIMARY KEY CLUSTERED 
	(
		[DefaultTaskID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
	 CONSTRAINT [IX_DefaultTask] UNIQUE NONCLUSTERED 
	(
		[TaskID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

	SET ANSI_PADDING ON

	ALTER TABLE [dbo].[DefaultTask]  WITH CHECK ADD  CONSTRAINT [FK_DefaultTask_Task] FOREIGN KEY([TaskID])
	REFERENCES [dbo].[Task] ([TaskID])
	ON UPDATE CASCADE
	ON DELETE CASCADE

	ALTER TABLE [dbo].[DefaultTask] CHECK CONSTRAINT [FK_DefaultTask_Task]

end
GO





GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DefaultTask_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[DefaultTask_Ins]
GO

CREATE PROCEDURE [dbo].[DefaultTask_Ins]
	@TaskID int,
	@Type varchar(10),
	@DefaultTaskID int OUTPUT
AS
INSERT [dbo].[DefaultTask]
(
	[TaskID],
	[Type]
)
VALUES
(
	@TaskID,
	@Type
)
SELECT @DefaultTaskID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DefaultTask_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[DefaultTask_Upd]
GO

CREATE PROCEDURE [dbo].[DefaultTask_Upd]
	@DefaultTaskID int,
	@TaskID int,
	@Type varchar(10)
AS
UPDATE [dbo].[DefaultTask]
SET 
	[TaskID] = @TaskID,
	[Type] = @Type
WHERE
	[DefaultTaskID] = @DefaultTaskID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DefaultTask_Upd_ByTaskID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[DefaultTask_Upd_ByTaskID]
GO

CREATE PROCEDURE [dbo].[DefaultTask_Upd_ByTaskID]
	@TaskID int,
	@TaskIDOld int
AS
UPDATE [dbo].[DefaultTask]
SET
	[TaskID] = @TaskID
WHERE
	[TaskID] = @TaskIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DefaultTask_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[DefaultTask_Del]
GO

CREATE PROCEDURE [dbo].[DefaultTask_Del]
	@DefaultTaskID int
AS
DELETE FROM [dbo].[DefaultTask]
WHERE
	[DefaultTaskID] = @DefaultTaskID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DefaultTask_Del_ByTaskID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[DefaultTask_Del_ByTaskID]
GO

CREATE PROCEDURE [dbo].[DefaultTask_Del_ByTaskID]
	@TaskID int
AS
DELETE
FROM [dbo].[DefaultTask]
WHERE
	[TaskID] = @TaskID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DefaultTask_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[DefaultTask_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[DefaultTask_Sel_ByID]
	@DefaultTaskID int
AS
SET NOCOUNT ON
SELECT
	[DefaultTaskID],
	[TaskID],
	[Type]
FROM [dbo].[DefaultTask]
WHERE
	[DefaultTaskID] = @DefaultTaskID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DefaultTask_Sel_ByTaskID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[DefaultTask_Sel_ByTaskID]
GO

CREATE PROCEDURE [dbo].[DefaultTask_Sel_ByTaskID]
	@TaskID int
AS
SET NOCOUNT ON
SELECT
	[DefaultTaskID],
	[TaskID],
	[Type]
FROM [dbo].[DefaultTask]
WHERE
	[TaskID] = @TaskID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DefaultTask_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[DefaultTask_Sel_All]
GO

CREATE PROCEDURE [dbo].[DefaultTask_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[DefaultTaskID],
	[TaskID],
	[Type]
FROM [dbo].[DefaultTask]
ORDER BY 
	[DefaultTaskID] ASC
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DefaultTask_Sel_ByTaskID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[DefaultTask_Sel_ByTaskID]
GO

CREATE PROCEDURE [dbo].[DefaultTask_Sel_ByTaskID]
	@TaskID int
AS
SET NOCOUNT ON
SELECT
	[DefaultTaskID],
	[TaskID],
	[Type]
FROM [dbo].[DefaultTask]
WHERE
	[TaskID] = @TaskID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryTask_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryTask_Ins]
GO

CREATE PROCEDURE [dbo].[ItineraryTask_Ins]
	@ItineraryID int,
	@TaskID int,
	@ItineraryTaskID int OUTPUT
AS
INSERT [dbo].[ItineraryTask]
(
	[ItineraryID],
	[TaskID]
)
VALUES
(
	@ItineraryID,
	@TaskID
)
SELECT @ItineraryTaskID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryTask_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryTask_Upd]
GO

CREATE PROCEDURE [dbo].[ItineraryTask_Upd]
	@ItineraryTaskID int,
	@ItineraryID int,
	@TaskID int
AS
UPDATE [dbo].[ItineraryTask]
SET 
	[ItineraryID] = @ItineraryID,
	[TaskID] = @TaskID
WHERE
	[ItineraryTaskID] = @ItineraryTaskID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryTask_Upd_ByItineraryID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryTask_Upd_ByItineraryID]
GO

CREATE PROCEDURE [dbo].[ItineraryTask_Upd_ByItineraryID]
	@ItineraryID int,
	@ItineraryIDOld int
AS
UPDATE [dbo].[ItineraryTask]
SET
	[ItineraryID] = @ItineraryID
WHERE
	[ItineraryID] = @ItineraryIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryTask_Upd_ByTaskID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryTask_Upd_ByTaskID]
GO

CREATE PROCEDURE [dbo].[ItineraryTask_Upd_ByTaskID]
	@TaskID int,
	@TaskIDOld int
AS
UPDATE [dbo].[ItineraryTask]
SET
	[TaskID] = @TaskID
WHERE
	[TaskID] = @TaskIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryTask_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryTask_Del]
GO

CREATE PROCEDURE [dbo].[ItineraryTask_Del]
	@ItineraryTaskID int
AS
DELETE FROM [dbo].[ItineraryTask]
WHERE
	[ItineraryTaskID] = @ItineraryTaskID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryTask_Del_ByItineraryID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryTask_Del_ByItineraryID]
GO

CREATE PROCEDURE [dbo].[ItineraryTask_Del_ByItineraryID]
	@ItineraryID int
AS
DELETE
FROM [dbo].[ItineraryTask]
WHERE
	[ItineraryID] = @ItineraryID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryTask_Del_ByTaskID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryTask_Del_ByTaskID]
GO

CREATE PROCEDURE [dbo].[ItineraryTask_Del_ByTaskID]
	@TaskID int
AS
DELETE
FROM [dbo].[ItineraryTask]
WHERE
	[TaskID] = @TaskID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryTask_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryTask_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[ItineraryTask_Sel_ByID]
	@ItineraryTaskID int
AS
SET NOCOUNT ON
SELECT
	[ItineraryTaskID],
	[ItineraryID],
	[TaskID]
FROM [dbo].[ItineraryTask]
WHERE
	[ItineraryTaskID] = @ItineraryTaskID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryTask_Sel_ByTaskID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryTask_Sel_ByTaskID]
GO

CREATE PROCEDURE [dbo].[ItineraryTask_Sel_ByTaskID]
	@TaskID int
AS
SET NOCOUNT ON
SELECT
	[ItineraryTaskID],
	[ItineraryID],
	[TaskID]
FROM [dbo].[ItineraryTask]
WHERE
	[TaskID] = @TaskID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryTask_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryTask_Sel_All]
GO

CREATE PROCEDURE [dbo].[ItineraryTask_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[ItineraryTaskID],
	[ItineraryID],
	[TaskID]
FROM [dbo].[ItineraryTask]
ORDER BY 
	[ItineraryTaskID] ASC
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryTask_Sel_ByItineraryID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryTask_Sel_ByItineraryID]
GO

CREATE PROCEDURE [dbo].[ItineraryTask_Sel_ByItineraryID]
	@ItineraryID int
AS
SET NOCOUNT ON
SELECT
	[ItineraryTaskID],
	[ItineraryID],
	[TaskID]
FROM [dbo].[ItineraryTask]
WHERE
	[ItineraryID] = @ItineraryID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ItineraryTask_Sel_ByTaskID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ItineraryTask_Sel_ByTaskID]
GO

CREATE PROCEDURE [dbo].[ItineraryTask_Sel_ByTaskID]
	@TaskID int
AS
SET NOCOUNT ON
SELECT
	[ItineraryTaskID],
	[ItineraryID],
	[TaskID]
FROM [dbo].[ItineraryTask]
WHERE
	[TaskID] = @TaskID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Task_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Task_Ins]
GO

CREATE PROCEDURE [dbo].[Task_Ins]
	@TaskName varchar(50),
	@DateDue datetime,
	@DateCompleted datetime,
	@Note varchar(500),
	@TaskID int OUTPUT
AS
INSERT [dbo].[Task]
(
	[TaskName],
	[DateDue],
	[DateCompleted],
	[Note]
)
VALUES
(
	@TaskName,
	@DateDue,
	@DateCompleted,
	@Note
)
SELECT @TaskID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Task_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Task_Upd]
GO

CREATE PROCEDURE [dbo].[Task_Upd]
	@TaskID int,
	@TaskName varchar(50),
	@DateDue datetime,
	@DateCompleted datetime,
	@Note varchar(500)
AS
UPDATE [dbo].[Task]
SET 
	[TaskName] = @TaskName,
	[DateDue] = @DateDue,
	[DateCompleted] = @DateCompleted,
	[Note] = @Note
WHERE
	[TaskID] = @TaskID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Task_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Task_Del]
GO

CREATE PROCEDURE [dbo].[Task_Del]
	@TaskID int
AS
DELETE FROM [dbo].[Task]
WHERE
	[TaskID] = @TaskID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Task_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Task_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[Task_Sel_ByID]
	@TaskID int
AS
SET NOCOUNT ON
SELECT
	[TaskID],
	[TaskName],
	[DateDue],
	[DateCompleted],
	[Note]
FROM [dbo].[Task]
WHERE
	[TaskID] = @TaskID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Task_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Task_Sel_All]
GO

CREATE PROCEDURE [dbo].[Task_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[TaskID],
	[TaskName],
	[DateDue],
	[DateCompleted],
	[Note]
FROM [dbo].[Task]
ORDER BY 
	[TaskID] ASC
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
	[IsDeleted]
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
	[Title]
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
GO


ALTER PROCEDURE [dbo].[_ToolSet_Sel_All]

AS
SET NOCOUNT ON

EXEC ServiceType_Sel_All
EXEC Grade_Sel_All
EXEC GradeExternal_Sel_All
EXEC CreditCard_Sel_All
EXEC Country_Sel_All
EXEC State_Sel_All
EXEC Region_Sel_All
EXEC City_Sel_All
EXEC ServiceConfigType_Sel_All
EXEC AgeGroup_Sel_All
EXEC ItineraryStatus_Sel_All
EXEC RequestStatus_Sel_All
EXEC User_Sel_All
EXEC Branch_Sel_All
EXEC Department_Sel_All
EXEC ItinerarySource_Sel_All
EXEC AppSettings_Sel_All
EXEC Agent_Sel_All
EXEC SupplierConfigType_Sel_All
EXEC MenuType_Sel_All
EXEC PaymentType_Sel_All
EXEC Currency_Sel_All
EXEC TaxType_Sel_All
EXEC PaymentDue_Sel_All
EXEC AccountingCategory_Sel_All
EXEC ContactCategory_Sel_All
EXEC OptionType_Sel_All
EXEC Flag_Sel_All
EXEC TemplateCategory_Sel_All
EXEC Template_Sel_All
EXEC ContentType_Sel_All
select * from CurrencyRate where ValidTo > getdate() --EXEC CurrencyRate_Sel_All
select t.* from Task t inner join DefaultTask d on d.TaskID = t.TaskID -- all Tasks linked to DefaultTasks table
exec DefaultTask_Sel_All
GO




exec __RefreshViews;
GO
----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2012.08.16'
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