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
if ((select VersionNumber from AppSettings) <> '2011.05.28')
	RAISERROR (N'Database Update Script is not correct version for current database version',17,1)	

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------

GO
PRINT N'Updating [dbo].[Itinerary]'
GO

if Exists(select * from sys.columns where Name = N'OutputCurrency' and Object_ID = Object_ID(N'Itinerary'))
	ALTER TABLE [dbo].[Itinerary] 
		DROP COLUMN [OutputCurrency];


GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ServiceContent_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ServiceContent_Ins]
GO

CREATE PROCEDURE [dbo].[ServiceContent_Ins]
	@ServiceID int,
	@ContentID int,
	@ContentTypeID int,
	@ServiceContentID int OUTPUT
AS
INSERT [dbo].[ServiceContent]
(
	[ServiceID],
	[ContentID],
	[ContentTypeID]
)
VALUES
(
	@ServiceID,
	@ContentID,
	@ContentTypeID
)
SELECT @ServiceContentID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ServiceContent_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ServiceContent_Upd]
GO

CREATE PROCEDURE [dbo].[ServiceContent_Upd]
	@ServiceContentID int,
	@ServiceID int,
	@ContentID int,
	@ContentTypeID int
AS
UPDATE [dbo].[ServiceContent]
SET 
	[ServiceID] = @ServiceID,
	[ContentID] = @ContentID,
	[ContentTypeID] = @ContentTypeID
WHERE
	[ServiceContentID] = @ServiceContentID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ServiceContent_Upd_ByServiceID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ServiceContent_Upd_ByServiceID]
GO

CREATE PROCEDURE [dbo].[ServiceContent_Upd_ByServiceID]
	@ServiceID int,
	@ServiceIDOld int
AS
UPDATE [dbo].[ServiceContent]
SET
	[ServiceID] = @ServiceID
WHERE
	[ServiceID] = @ServiceIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ServiceContent_Upd_ByContentID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ServiceContent_Upd_ByContentID]
GO

CREATE PROCEDURE [dbo].[ServiceContent_Upd_ByContentID]
	@ContentID int,
	@ContentIDOld int
AS
UPDATE [dbo].[ServiceContent]
SET
	[ContentID] = @ContentID
WHERE
	[ContentID] = @ContentIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ServiceContent_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ServiceContent_Del]
GO

CREATE PROCEDURE [dbo].[ServiceContent_Del]
	@ServiceContentID int
AS
DELETE FROM [dbo].[ServiceContent]
WHERE
	[ServiceContentID] = @ServiceContentID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ServiceContent_Del_ByServiceID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ServiceContent_Del_ByServiceID]
GO

CREATE PROCEDURE [dbo].[ServiceContent_Del_ByServiceID]
	@ServiceID int
AS
DELETE
FROM [dbo].[ServiceContent]
WHERE
	[ServiceID] = @ServiceID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ServiceContent_Del_ByContentID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ServiceContent_Del_ByContentID]
GO

CREATE PROCEDURE [dbo].[ServiceContent_Del_ByContentID]
	@ContentID int
AS
DELETE
FROM [dbo].[ServiceContent]
WHERE
	[ContentID] = @ContentID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ServiceContent_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ServiceContent_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[ServiceContent_Sel_ByID]
	@ServiceContentID int
AS
SET NOCOUNT ON
SELECT
	[ServiceContentID],
	[ServiceID],
	[ContentID],
	[ContentTypeID]
FROM [dbo].[ServiceContent]
WHERE
	[ServiceContentID] = @ServiceContentID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ServiceContent_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ServiceContent_Sel_All]
GO

CREATE PROCEDURE [dbo].[ServiceContent_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[ServiceContentID],
	[ServiceID],
	[ContentID],
	[ContentTypeID]
FROM [dbo].[ServiceContent]
ORDER BY 
	[ServiceContentID] ASC
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ServiceContent_Sel_ByServiceID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ServiceContent_Sel_ByServiceID]
GO

CREATE PROCEDURE [dbo].[ServiceContent_Sel_ByServiceID]
	@ServiceID int
AS
SET NOCOUNT ON
SELECT
	[ServiceContentID],
	[ServiceID],
	[ContentID],
	[ContentTypeID]
FROM [dbo].[ServiceContent]
WHERE
	[ServiceID] = @ServiceID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ServiceContent_Sel_ByContentID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[ServiceContent_Sel_ByContentID]
GO

CREATE PROCEDURE [dbo].[ServiceContent_Sel_ByContentID]
	@ContentID int
AS
SET NOCOUNT ON
SELECT
	[ServiceContentID],
	[ServiceID],
	[ContentID],
	[ContentTypeID]
FROM [dbo].[ServiceContent]
WHERE
	[ContentID] = @ContentID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SupplierContent_Ins]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[SupplierContent_Ins]
GO

CREATE PROCEDURE [dbo].[SupplierContent_Ins]
	@SupplierID int,
	@ContentID int,
	@ContentTypeID int,
	@SupplierContentID int OUTPUT
AS
INSERT [dbo].[SupplierContent]
(
	[SupplierID],
	[ContentID],
	[ContentTypeID]
)
VALUES
(
	@SupplierID,
	@ContentID,
	@ContentTypeID
)
SELECT @SupplierContentID=SCOPE_IDENTITY()
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SupplierContent_Upd]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[SupplierContent_Upd]
GO

CREATE PROCEDURE [dbo].[SupplierContent_Upd]
	@SupplierContentID int,
	@SupplierID int,
	@ContentID int,
	@ContentTypeID int
AS
UPDATE [dbo].[SupplierContent]
SET 
	[SupplierID] = @SupplierID,
	[ContentID] = @ContentID,
	[ContentTypeID] = @ContentTypeID
WHERE
	[SupplierContentID] = @SupplierContentID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SupplierContent_Upd_BySupplierID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[SupplierContent_Upd_BySupplierID]
GO

CREATE PROCEDURE [dbo].[SupplierContent_Upd_BySupplierID]
	@SupplierID int,
	@SupplierIDOld int
AS
UPDATE [dbo].[SupplierContent]
SET
	[SupplierID] = @SupplierID
WHERE
	[SupplierID] = @SupplierIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SupplierContent_Upd_ByContentID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[SupplierContent_Upd_ByContentID]
GO

CREATE PROCEDURE [dbo].[SupplierContent_Upd_ByContentID]
	@ContentID int,
	@ContentIDOld int
AS
UPDATE [dbo].[SupplierContent]
SET
	[ContentID] = @ContentID
WHERE
	[ContentID] = @ContentIDOld
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SupplierContent_Del]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[SupplierContent_Del]
GO

CREATE PROCEDURE [dbo].[SupplierContent_Del]
	@SupplierContentID int
AS
DELETE FROM [dbo].[SupplierContent]
WHERE
	[SupplierContentID] = @SupplierContentID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SupplierContent_Del_BySupplierID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[SupplierContent_Del_BySupplierID]
GO

CREATE PROCEDURE [dbo].[SupplierContent_Del_BySupplierID]
	@SupplierID int
AS
DELETE
FROM [dbo].[SupplierContent]
WHERE
	[SupplierID] = @SupplierID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SupplierContent_Del_ByContentID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[SupplierContent_Del_ByContentID]
GO

CREATE PROCEDURE [dbo].[SupplierContent_Del_ByContentID]
	@ContentID int
AS
DELETE
FROM [dbo].[SupplierContent]
WHERE
	[ContentID] = @ContentID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SupplierContent_Sel_ByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[SupplierContent_Sel_ByID]
GO

CREATE PROCEDURE [dbo].[SupplierContent_Sel_ByID]
	@SupplierContentID int
AS
SET NOCOUNT ON
SELECT
	[SupplierContentID],
	[SupplierID],
	[ContentID],
	[ContentTypeID]
FROM [dbo].[SupplierContent]
WHERE
	[SupplierContentID] = @SupplierContentID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SupplierContent_Sel_All]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[SupplierContent_Sel_All]
GO

CREATE PROCEDURE [dbo].[SupplierContent_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[SupplierContentID],
	[SupplierID],
	[ContentID],
	[ContentTypeID]
FROM [dbo].[SupplierContent]
ORDER BY 
	[SupplierContentID] ASC
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SupplierContent_Sel_BySupplierID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[SupplierContent_Sel_BySupplierID]
GO

CREATE PROCEDURE [dbo].[SupplierContent_Sel_BySupplierID]
	@SupplierID int
AS
SET NOCOUNT ON
SELECT
	[SupplierContentID],
	[SupplierID],
	[ContentID],
	[ContentTypeID]
FROM [dbo].[SupplierContent]
WHERE
	[SupplierID] = @SupplierID
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SupplierContent_Sel_ByContentID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[SupplierContent_Sel_ByContentID]
GO

CREATE PROCEDURE [dbo].[SupplierContent_Sel_ByContentID]
	@ContentID int
AS
SET NOCOUNT ON
SELECT
	[SupplierContentID],
	[SupplierID],
	[ContentID],
	[ContentTypeID]
FROM [dbo].[SupplierContent]
WHERE
	[ContentID] = @ContentID
GO


----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2011.06.28'
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
