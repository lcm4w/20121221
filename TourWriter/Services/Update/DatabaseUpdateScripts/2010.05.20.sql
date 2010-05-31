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
if ((select VersionNumber from AppSettings) <> '2010.05.04')
	RAISERROR (N'Update script version is invalid for this db version',17,1)	
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------
PRINT N'Dropping [dbo].[_User_RemoveLogin]...';


GO
DROP PROCEDURE [dbo].[_User_RemoveLogin];


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
PRINT N'Creating [dbo].[Content]...';


GO
CREATE TABLE [dbo].[Content] (
    [ContentId]     INT             IDENTITY (1, 1) NOT NULL,
    [SupplierId]    INT             NOT NULL,
    [ContentTypeId] INT             NULL,
    [ContentName]   NVARCHAR (50)   NOT NULL,
    [Heading]       NVARCHAR (250)  NULL,
    [Body]          NVARCHAR (4000) NULL,
    [ImagePath]     NVARCHAR (250)  NULL
);


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
PRINT N'Creating PK_Content...';


GO
ALTER TABLE [dbo].[Content]
    ADD CONSTRAINT [PK_Content] PRIMARY KEY CLUSTERED ([ContentId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


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
PRINT N'Creating [dbo].[ContentType]...';


GO
CREATE TABLE [dbo].[ContentType] (
    [ContentTypeId]   INT           IDENTITY (1, 1) NOT NULL,
    [ContentTypeName] NVARCHAR (50) NOT NULL
);


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
PRINT N'Creating PK_ContentType...';


GO
ALTER TABLE [dbo].[ContentType]
    ADD CONSTRAINT [PK_ContentType] PRIMARY KEY CLUSTERED ([ContentTypeId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);


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
PRINT N'Creating FK_Content_ContentType...';


GO
ALTER TABLE [dbo].[Content] WITH NOCHECK
    ADD CONSTRAINT [FK_Content_ContentType] FOREIGN KEY ([ContentTypeId]) REFERENCES [dbo].[ContentType] ([ContentTypeId]) ON DELETE NO ACTION ON UPDATE CASCADE;


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
PRINT N'Creating FK_Content_Supplier...';


GO
ALTER TABLE [dbo].[Content] WITH NOCHECK
    ADD CONSTRAINT [FK_Content_Supplier] FOREIGN KEY ([SupplierId]) REFERENCES [dbo].[Supplier] ([SupplierID]) ON DELETE CASCADE ON UPDATE CASCADE;


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
	[TaxTypeID],
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
	[TaxTypeID],
	[Latitude],
	[Longitude],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted]
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
	[IsDeleted]
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
	[IsDeleted]
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
	[RowVersion]
FROM [dbo].[SupplierNote]
WHERE
	[SupplierID] = @SupplierID
ORDER BY 
	SupplierNoteID

-- ServiceFoc --
SELECT
	[ServiceFocID],
	[ServiceID],
	[UnitsUsed],
	[UnitsFree]
FROM [dbo].[ServiceFoc]
WHERE
	[ServiceID] IN ( 
		SELECT [ServiceID] FROM [dbo].[Service] 
		WHERE [SupplierID] = @SupplierID )

-- Content
SELECT * FROM [Content] 
WHERE SupplierId = @SupplierID
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
PRINT N'Altering [dbo].[_ToolSet_Sel_All]...';


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
PRINT N'Altering [dbo].[Login_Ins]...';


GO

ALTER PROCEDURE [dbo].[Login_Ins]
	@LoginID uniqueidentifier,
	@UserID int,
	@LoginDate datetime,
	@LastActiveDate datetime,
	@ComputerName varchar(200),
	@Ended bit
AS
INSERT [dbo].[Login]
(
	[LoginID],
	[UserID],
	[LoginDate],
	[LastActiveDate],
	[ComputerName],
	[Ended]
)
VALUES
(
	@LoginID,
	@UserID,
	@LoginDate,
	@LastActiveDate,
	@ComputerName,
	@Ended
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
PRINT N'Altering [dbo].[Login_Sel_All]...';


GO

ALTER PROCEDURE [dbo].[Login_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[LoginID],
	[UserID],
	[LoginDate],
	[LastActiveDate],
	[ComputerName],
	[Ended]
FROM [dbo].[Login]
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
PRINT N'Creating [dbo].[Content_Del]...';


GO

CREATE PROCEDURE [dbo].[Content_Del]
	@ContentId int
AS
DELETE FROM [dbo].[Content]
WHERE
	[ContentId] = @ContentId
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
PRINT N'Creating [dbo].[Content_Del_ByContentTypeId]...';


GO

CREATE PROCEDURE [dbo].[Content_Del_ByContentTypeId]
	@ContentTypeId int
AS
DELETE
FROM [dbo].[Content]
WHERE
	[ContentTypeId] = @ContentTypeId
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
PRINT N'Creating [dbo].[Content_Del_BySupplierId]...';


GO

CREATE PROCEDURE [dbo].[Content_Del_BySupplierId]
	@SupplierId int
AS
DELETE
FROM [dbo].[Content]
WHERE
	[SupplierId] = @SupplierId
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
PRINT N'Creating [dbo].[Content_Ins]...';


GO

CREATE PROCEDURE [dbo].[Content_Ins]
	@SupplierId int,
	@ContentTypeId int,
	@ContentName nvarchar(50),
	@Heading nvarchar(250),
	@Body nvarchar(4000),
	@ImagePath nvarchar(250),
	@ContentId int OUTPUT
AS
INSERT [dbo].[Content]
(
	[SupplierId],
	[ContentTypeId],
	[ContentName],
	[Heading],
	[Body],
	[ImagePath]
)
VALUES
(
	@SupplierId,
	@ContentTypeId,
	@ContentName,
	@Heading,
	@Body,
	@ImagePath
)
SELECT @ContentId=SCOPE_IDENTITY()
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
PRINT N'Creating [dbo].[Content_Sel_All]...';


GO

CREATE PROCEDURE [dbo].[Content_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[ContentId],
	[SupplierId],
	[ContentTypeId],
	[ContentName],
	[Heading],
	[Body],
	[ImagePath]
FROM [dbo].[Content]
ORDER BY 
	[ContentId] ASC
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
PRINT N'Creating [dbo].[Content_Sel_ByContentTypeId]...';


GO

CREATE PROCEDURE [dbo].[Content_Sel_ByContentTypeId]
	@ContentTypeId int
AS
SET NOCOUNT ON
SELECT
	[ContentId],
	[SupplierId],
	[ContentTypeId],
	[ContentName],
	[Heading],
	[Body],
	[ImagePath]
FROM [dbo].[Content]
WHERE
	[ContentTypeId] = @ContentTypeId
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
PRINT N'Creating [dbo].[Content_Sel_ByID]...';


GO

CREATE PROCEDURE [dbo].[Content_Sel_ByID]
	@ContentId int
AS
SET NOCOUNT ON
SELECT
	[ContentId],
	[SupplierId],
	[ContentTypeId],
	[ContentName],
	[Heading],
	[Body],
	[ImagePath]
FROM [dbo].[Content]
WHERE
	[ContentId] = @ContentId
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
PRINT N'Creating [dbo].[Content_Sel_BySupplierId]...';


GO

CREATE PROCEDURE [dbo].[Content_Sel_BySupplierId]
	@SupplierId int
AS
SET NOCOUNT ON
SELECT
	[ContentId],
	[SupplierId],
	[ContentTypeId],
	[ContentName],
	[Heading],
	[Body],
	[ImagePath]
FROM [dbo].[Content]
WHERE
	[SupplierId] = @SupplierId
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
PRINT N'Creating [dbo].[Content_Upd]...';


GO

CREATE PROCEDURE [dbo].[Content_Upd]
	@ContentId int,
	@SupplierId int,
	@ContentTypeId int,
	@ContentName nvarchar(50),
	@Heading nvarchar(250),
	@Body nvarchar(4000),
	@ImagePath nvarchar(250)
AS
UPDATE [dbo].[Content]
SET 
	[SupplierId] = @SupplierId,
	[ContentTypeId] = @ContentTypeId,
	[ContentName] = @ContentName,
	[Heading] = @Heading,
	[Body] = @Body,
	[ImagePath] = @ImagePath
WHERE
	[ContentId] = @ContentId
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
PRINT N'Creating [dbo].[Content_Upd_ByContentTypeId]...';


GO

CREATE PROCEDURE [dbo].[Content_Upd_ByContentTypeId]
	@ContentTypeId int,
	@ContentTypeIdOld int
AS
UPDATE [dbo].[Content]
SET
	[ContentTypeId] = @ContentTypeId
WHERE
	[ContentTypeId] = @ContentTypeIdOld
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
PRINT N'Creating [dbo].[Content_Upd_BySupplierId]...';


GO

CREATE PROCEDURE [dbo].[Content_Upd_BySupplierId]
	@SupplierId int,
	@SupplierIdOld int
AS
UPDATE [dbo].[Content]
SET
	[SupplierId] = @SupplierId
WHERE
	[SupplierId] = @SupplierIdOld
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
PRINT N'Creating [dbo].[ContentType_Del]...';


GO

CREATE PROCEDURE [dbo].[ContentType_Del]
	@ContentTypeId int
AS
DELETE FROM [dbo].[ContentType]
WHERE
	[ContentTypeId] = @ContentTypeId
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
PRINT N'Creating [dbo].[ContentType_Ins]...';


GO

CREATE PROCEDURE [dbo].[ContentType_Ins]
	@ContentTypeName nvarchar(50),
	@ContentTypeId int OUTPUT
AS
INSERT [dbo].[ContentType]
(
	[ContentTypeName]
)
VALUES
(
	@ContentTypeName
)
SELECT @ContentTypeId=SCOPE_IDENTITY()
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
PRINT N'Creating [dbo].[ContentType_Sel_All]...';


GO

CREATE PROCEDURE [dbo].[ContentType_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[ContentTypeId],
	[ContentTypeName]
FROM [dbo].[ContentType]
ORDER BY 
	[ContentTypeId] ASC
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
PRINT N'Creating [dbo].[ContentType_Sel_ByID]...';


GO

CREATE PROCEDURE [dbo].[ContentType_Sel_ByID]
	@ContentTypeId int
AS
SET NOCOUNT ON
SELECT
	[ContentTypeId],
	[ContentTypeName]
FROM [dbo].[ContentType]
WHERE
	[ContentTypeId] = @ContentTypeId
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
PRINT N'Creating [dbo].[ContentType_Upd]...';


GO

CREATE PROCEDURE [dbo].[ContentType_Upd]
	@ContentTypeId int,
	@ContentTypeName nvarchar(50)
AS
UPDATE [dbo].[ContentType]
SET 
	[ContentTypeName] = @ContentTypeName
WHERE
	[ContentTypeId] = @ContentTypeId
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
PRINT N'Creating [dbo].[ItineraryPaxOverride_Sel_ByItineraryPaxID]...';


GO

CREATE PROCEDURE [dbo].[ItineraryPaxOverride_Sel_ByItineraryPaxID]
	@ItineraryPaxID int
AS
SET NOCOUNT ON
SELECT
	[PurchaseItemID],
	[ItineraryPaxID],
	[MemberCount],
	[MemberRooms],
	[StaffCount],
	[StaffRooms]
FROM [dbo].[ItineraryPaxOverride]
WHERE
	[ItineraryPaxID] = @ItineraryPaxID
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
PRINT N'Creating [dbo].[ItineraryPaxOverride_Sel_ByPurchaseItemID]...';


GO

CREATE PROCEDURE [dbo].[ItineraryPaxOverride_Sel_ByPurchaseItemID]
	@PurchaseItemID int
AS
SET NOCOUNT ON
SELECT
	[PurchaseItemID],
	[ItineraryPaxID],
	[MemberCount],
	[MemberRooms],
	[StaffCount],
	[StaffRooms]
FROM [dbo].[ItineraryPaxOverride]
WHERE
	[PurchaseItemID] = @PurchaseItemID
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
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2010.05.20'
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
