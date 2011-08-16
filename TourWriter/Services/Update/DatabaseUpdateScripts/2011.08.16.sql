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
if ((select VersionNumber from AppSettings) <> '2011.07.10' and (select VersionNumber from AppSettings) <> '2011.07.12')
	RAISERROR (N'Database Update Script is not correct version for current database version',17,1)	

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------

GO

if not Exists(select * from sys.columns where Name = N'JobDescription' and Object_ID = Object_ID(N'Contact'))
	ALTER TABLE [dbo].[Contact]
		ADD [JobDescription] VARCHAR (2000) NULL;
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Contact_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Contact_Ins]
GO

CREATE PROCEDURE [dbo].[Contact_Ins]
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
	[JobDescription]
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
	@JobDescription
)
SELECT @ContactID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Contact_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Contact_Upd]
GO

CREATE PROCEDURE [dbo].[Contact_Upd]
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
	@JobDescription varchar(2000)
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
	[JobDescription] = @JobDescription
WHERE
	[ContactID] = @ContactID
	AND [RowVersion] = @RowVersion
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Contact_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Contact_Del]
GO

CREATE PROCEDURE [dbo].[Contact_Del]
	@ContactID int,
	@RowVersion timestamp
AS
DELETE FROM [dbo].[Contact]
WHERE
	[ContactID] = @ContactID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Contact_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Contact_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[Contact_Sel_ByID]
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
	[JobDescription]
FROM [dbo].[Contact]
WHERE
	[ContactID] = @ContactID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Contact_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[Contact_Sel_All]
GO

CREATE PROCEDURE [dbo].[Contact_Sel_All]
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
	[JobDescription]
FROM [dbo].[Contact]
ORDER BY 
	[ContactID] ASC
GO



ALTER VIEW [dbo].[ContactDetail]
AS
select 
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
	cont.ParentFolderID
from Contact as cont
left outer join City as city on city.CityID = cont.CityID 
left outer join Region as region on region.RegionID = cont.RegionID 
left outer join [State] as stat on stat.StateID = cont.StateID 
left outer join Country as country on country.CountryID = cont.CountryID  
left outer join Folder as fld on fld.FolderId = cont.ParentFolderId
left outer join
(
	select ContactID, count(ItineraryID) as ItineraryCount
	from ItineraryMember mem
	left outer join ItineraryGroup grp on mem.ItineraryGroupID = grp.ItineraryGroupID
	where ContactID is not null 
	group by ContactID
) cnt on cnt.ContactID = cont.ContactID;

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
	[JobDescription]
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
	[JobDescription]	
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
	[JobDescription]
FROM [dbo].[Contact]
WHERE
	[ContactID] = @OrigContactID

SELECT SCOPE_IDENTITY()

GO

exec __RefreshViews
GO

----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2011.08.16'
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
