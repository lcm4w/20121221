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
if ((select VersionNumber from AppSettings) <> '2010.11.06')
	RAISERROR (N'Update script version is invalid for this db version',17,1)	
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

----------------------------------------------------------------------------------------

PRINT N'Altering [dbo].[Option_Ins]...';


GO

ALTER PROCEDURE [dbo].[Option_Ins]
	@OptionName varchar(150),
	@OptionTypeID int,
	@RateID int,
	@Net money,
	@Gross money,
	@PricingOption char(2),
	@IsDefault bit,
	@IsRecordActive bit,
	@AddedOn datetime,
	@AddedBy int,
	@IsDeleted bit,
	@OptionID int OUTPUT
AS
INSERT [dbo].[Option]
(
	[OptionName],
	[OptionTypeID],
	[RateID],
	[Net],
	[Gross],
	[PricingOption],
	[IsDefault],
	[IsRecordActive],
	[AddedOn],
	[AddedBy],
	[IsDeleted]
)
VALUES
(
	@OptionName,
	@OptionTypeID,
	@RateID,
	@Net,
	@Gross,
	@PricingOption,
	@IsDefault,
	@IsRecordActive,
	@AddedOn,
	@AddedBy,
	@IsDeleted
)
SELECT @OptionID=SCOPE_IDENTITY()
GO
PRINT N'Altering [dbo].[Option_Sel_All]...';


GO

ALTER PROCEDURE [dbo].[Option_Sel_All]
AS
SET NOCOUNT ON
SELECT
	[OptionID],
	[OptionName],
	[OptionTypeID],
	[RateID],
	[Net],
	[Gross],
	[PricingOption],
	[IsDefault],
	[IsRecordActive],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted]
FROM [dbo].[Option]
ORDER BY 
	[OptionID] ASC
GO
PRINT N'Altering [dbo].[Option_Sel_ByID]...';


GO

ALTER PROCEDURE [dbo].[Option_Sel_ByID]
	@OptionID int
AS
SET NOCOUNT ON
SELECT
	[OptionID],
	[OptionName],
	[OptionTypeID],
	[RateID],
	[Net],
	[Gross],
	[PricingOption],
	[IsDefault],
	[IsRecordActive],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted]
FROM [dbo].[Option]
WHERE
	[OptionID] = @OptionID
GO
PRINT N'Altering [dbo].[Option_Sel_ByRateID]...';


GO

ALTER PROCEDURE [dbo].[Option_Sel_ByRateID]
	@RateID int
AS
SET NOCOUNT ON
SELECT
	[OptionID],
	[OptionName],
	[OptionTypeID],
	[RateID],
	[Net],
	[Gross],
	[PricingOption],
	[IsDefault],
	[IsRecordActive],
	[AddedOn],
	[AddedBy],
	[RowVersion],
	[IsDeleted]
FROM [dbo].[Option]
WHERE
	[RateID] = @RateID
GO
PRINT N'Altering [dbo].[Option_Upd]...';


GO

ALTER PROCEDURE [dbo].[Option_Upd]
	@OptionID int,
	@OptionName varchar(150),
	@OptionTypeID int,
	@RateID int,
	@Net money,
	@Gross money,
	@PricingOption char(2),
	@IsDefault bit,
	@IsRecordActive bit,
	@AddedOn datetime,
	@AddedBy int,
	@RowVersion timestamp,
	@IsDeleted bit
AS
UPDATE [dbo].[Option]
SET 
	[OptionName] = @OptionName,
	[OptionTypeID] = @OptionTypeID,
	[RateID] = @RateID,
	[Net] = @Net,
	[Gross] = @Gross,
	[PricingOption] = @PricingOption,
	[IsDefault] = @IsDefault,
	[IsRecordActive] = @IsRecordActive,
	[AddedOn] = @AddedOn,
	[AddedBy] = @AddedBy,
	[IsDeleted] = @IsDeleted
WHERE
	[OptionID] = @OptionID
	AND [RowVersion] = @RowVersion
GO

----------------------------------------------------------------------------------------
PRINT N'Updating [dbo].[AppSettings] version number'
GO
UPDATE [dbo].[AppSettings] SET [VersionNumber]='2011.01.11'
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
