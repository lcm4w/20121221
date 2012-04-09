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
if ((select VersionNumber from AppSettings) <> '2012.02.15')
	RAISERROR (N'Database Update Script is not correct version for current database version',17,1)

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------
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
	cont.ParentFolderID,
	(   SELECT CAST(ContactCategory.ContactCategoryName AS VARCHAR)+',' FROM ContactCategory
	    INNER JOIN ContactContactCategory ON ContactCategory.ContactCategoryID = ContactContactCategory.ContactCategoryID
	    WHERE ContactContactCategory.ContactID = cont.ContactID
	    FOR XML PATH ('')	
    ) AS Categories
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
exec __RefreshViews;
GO
----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2012.04.10'
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