USE [MemberFeedback]
go
create schema mbr

GO
CREATE FUNCTION [dbo].[udf_get_StringInput] (@strInputList VARCHAR(2000))
/*
Author:       Khanh Ngo
Purpose:      Parse the string which to filter the report under these chosen such as GL Plans
			  or DPD Buckets that is entered by end users from MBL/Pipeline report. 
Created:      10/05/2010
Usuage:		  SELECT DISTINCT(DataInput) FROM udf_get_StringInput('03, 22, 24, 200') 
			  SELECT DISTINCT(DataInput) FROM udf_get_StringInput('0-29, 30+, 60+, 180+') 
			  
History:      Who             Date            Description
1)            KMN             10/05/2010      Initial Creation
2)			  KMN			  11/16/2010	  Change the parameter name to make the function works
											  generally for multiple string value input
3)			  WAP			  01/27/2011	  Changed DataInput from varchar 10 to 25.  Going to use this function for the usp_rpt_RE_CreateFileLabels too.
4)			  LJO			  10/3/2016		  Migrated proc to MemberFeedback 

*/

RETURNS @tblDataInput TABLE
	(
		DataInput VARCHAR(250)
	)

AS

BEGIN

	DECLARE @Delimeter char(1)			
	SET @Delimeter = ','		--delimiter between each modtype

	DECLARE @StartPos INT 
	DECLARE	@Length INT

	DECLARE @strInput VARCHAR(250)

	WHILE LEN(@strInputList) > 0
	  BEGIN
		SET @StartPos = CHARINDEX(@Delimeter, @strInputList)
		IF @StartPos < 0 SET @StartPos = 0
		SET @Length = LEN(@strInputList) - @StartPos - 1
		IF @Length < 0 SET @Length = 0
		IF @StartPos > 0
		  BEGIN
			SET @strInput = SUBSTRING(@strInputList, 1, @StartPos - 1)
			SET @strInputList = SUBSTRING(@strInputList, @StartPos + 1, LEN(@strInputList) - @StartPos)
		  END
		ELSE
		  BEGIN
			SET @strInput = @strInputList
			SET @strInputList = ''
		  END
	      
		INSERT INTO @tblDataInput(DataInput) VALUES(LTRIM(RTRIM(@strInput)))
	    
	  END 
  RETURN 
END
GO
/****** Object:  Table [dbo].[AuditLog]    Script Date: 11/28/2016 9:07:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AuditLog](
	[AuditLogID] [int] IDENTITY(1,1) NOT NULL,
	[SurrogateID] [int] NOT NULL,
	[SurrogateType] [varchar](40) NOT NULL,
	[OldValue] [varchar](max) NULL,
	[NewValue] [varchar](max) NULL,
	[Modified_by] [varchar](50) NOT NULL,
	[Modified_dt] [datetime] NOT NULL,
 CONSTRAINT [PK_AuditLog_AuditLogID] PRIMARY KEY CLUSTERED 
(
	[AuditLogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [mbr].[CNFG_Category]    Script Date: 11/28/2016 9:07:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [mbr].[CNFG_Category](
	[CategoryID] [smallint] IDENTITY(1,1) NOT NULL,
	[CategoryDescr] [varchar](75) NOT NULL,
	[RecordBeginDate] [date] NOT NULL,
	[RecordEndDate] [date] NULL,
	[boolIsActive] [bit] NOT NULL,
	[created_by] [varchar](50) NOT NULL CONSTRAINT [DF_CNFG_Category_created_by]  DEFAULT (suser_name()),
	[created_dt] [datetime] NOT NULL CONSTRAINT [DF_CNFG_Category_created_dt]  DEFAULT (getdate()),
	[modified_by] [varchar](50) NOT NULL CONSTRAINT [DF_CNFG_Category_modified_by]  DEFAULT (suser_name()),
	[modified_dt] [datetime] NOT NULL CONSTRAINT [DF_CNFG_Category_modified_dt]  DEFAULT (getdate()),
 CONSTRAINT [pk_CNFG_Category_CategoryID] PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [mbr].[CNFG_FeedbackType]    Script Date: 11/28/2016 9:07:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [mbr].[CNFG_FeedbackType](
	[FeedbackTypeID] [smallint] IDENTITY(1,1) NOT NULL,
	[FeedbackTypeDescr] [varchar](50) NOT NULL,
	[boolNeedPriv] [bit] NOT NULL,
	[RecordBeginDate] [date] NOT NULL,
	[RecordEndDate] [date] NULL,
	[boolIsActive] [bit] NOT NULL,
	[created_by] [varchar](50) NOT NULL CONSTRAINT [DF_CNFG_MemberFeedbackType_created_by]  DEFAULT (suser_name()),
	[created_dt] [datetime] NOT NULL CONSTRAINT [DF_CNFG_MemberFeedbackType_created_dt]  DEFAULT (getdate()),
	[modified_by] [varchar](50) NOT NULL CONSTRAINT [DF_CNFG_MemberFeedbackType_modified_by]  DEFAULT (suser_name()),
	[modified_dt] [datetime] NOT NULL CONSTRAINT [DF_CNFG_MemberFeedbackType_modified_dt]  DEFAULT (getdate()),
 CONSTRAINT [PK_MemberFeedbackType] PRIMARY KEY CLUSTERED 
(
	[FeedbackTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [mbr].[CNFG_FunctionalArea]    Script Date: 11/28/2016 9:07:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [mbr].[CNFG_FunctionalArea](
	[FunctionalID] [smallint] IDENTITY(1,1) NOT NULL,
	[FunctionalDescr] [varchar](150) NOT NULL CONSTRAINT [DF_CNFG_FunctionalArea_FunctionalDescr]  DEFAULT ('Undefined'),
	[RecordBeginDate] [date] NOT NULL,
	[RecordEndDate] [date] NULL,
	[boolIsActive] [bit] NOT NULL,
	[created_by] [varchar](50) NOT NULL CONSTRAINT [DF_CNFG_FunctionalArea_created_by]  DEFAULT (suser_name()),
	[created_dt] [datetime] NOT NULL CONSTRAINT [DF_CNFG_FunctionalArea_created_dt]  DEFAULT (getdate()),
	[modified_by] [varchar](50) NOT NULL CONSTRAINT [DF_CNFG_FunctionalArea_modified_by]  DEFAULT (suser_name()),
	[modified_dt] [datetime] NOT NULL CONSTRAINT [DF_CNFG_FunctionalArea_modified_dt]  DEFAULT (getdate()),
 CONSTRAINT [PK_CNFG_FunctionalArea] PRIMARY KEY CLUSTERED 
(
	[FunctionalID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 95) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [mbr].[CNFG_ReportDate]    Script Date: 11/28/2016 9:07:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [mbr].[CNFG_ReportDate](
	[ReportDateID] [int] NOT NULL,
	[ReportDate] [date] NOT NULL,
	[ReportDay] [tinyint] NOT NULL,
	[ReportMonth] [tinyint] NOT NULL,
	[ReportMonthName] [varchar](12) NOT NULL,
	[ReportQuarter] [tinyint] NOT NULL,
	[ReportYear] [int] NOT NULL,
	[ReportDOW] [smallint] NOT NULL,
	[ReportDOY] [smallint] NOT NULL,
	[MonthBeginDate] [date] NOT NULL,
	[MonthEndDate] [date] NOT NULL,
	[QuarterBeginDate] [date] NOT NULL,
	[QuarterEndDate] [date] NOT NULL,
	[QuarterName] [varchar](12) NOT NULL,
	[YearBeginDate] [date] NOT NULL,
	[YearEndDate] [date] NOT NULL,
	[boolIsMonthBegin] [bit] NOT NULL,
	[boolIsMonthEnd] [bit] NOT NULL,
	[boolIsClosed] [bit] NOT NULL CONSTRAINT [DF_CNFG_ReportDate_boolIsClosed]  DEFAULT ((0)),
	[ClosedReason] [varchar](50) NULL,
	[created_by] [varchar](75) NOT NULL CONSTRAINT [DF_CNFG_ReportDate_created_by]  DEFAULT (suser_name()),
	[created_dt] [datetime] NOT NULL CONSTRAINT [DF_CNFG_ReportDate_created_dt]  DEFAULT (getdate()),
	[modified_by] [varchar](75) NOT NULL CONSTRAINT [DF_CNFG_ReportDate_modified_by]  DEFAULT (suser_name()),
	[modified_dt] [datetime] NOT NULL,
 CONSTRAINT [PK_CNFG_ReportDate_ReportDateID] PRIMARY KEY NONCLUSTERED 
(
	[ReportDateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 95) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Index [UNQ_CNFG_ReportDate_ReportDate]    Script Date: 11/28/2016 9:07:56 AM ******/
CREATE CLUSTERED INDEX [UNQ_CNFG_ReportDate_ReportDate] ON [mbr].[CNFG_ReportDate]
(
	[ReportDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 95) ON [PRIMARY]
GO
/****** Object:  Table [mbr].[CNFG_Source]    Script Date: 11/28/2016 9:07:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [mbr].[CNFG_Source](
	[SourceID] [smallint] IDENTITY(1,1) NOT NULL,
	[SourceDescr] [varchar](50) NOT NULL,
	[RecordBeginDate] [date] NOT NULL,
	[RecordEndDate] [date] NULL,
	[boolIsActive] [bit] NOT NULL,
	[created_by] [varchar](50) NOT NULL CONSTRAINT [DF_CNFG_Source_created_by]  DEFAULT (suser_name()),
	[created_dt] [datetime] NOT NULL CONSTRAINT [DF_CNFG_Source_created_dt]  DEFAULT (getdate()),
	[modified_by] [varchar](50) NOT NULL CONSTRAINT [DF_CNFG_Source_modified_by]  DEFAULT (suser_name()),
	[modified_dt] [datetime] NOT NULL CONSTRAINT [DF_CNFG_Source_modified_dt]  DEFAULT (getdate()),
 CONSTRAINT [PK_CNFG_Source] PRIMARY KEY CLUSTERED 
(
	[SourceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 95) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [mbr].[CNFG_Status]    Script Date: 11/28/2016 9:07:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [mbr].[CNFG_Status](
	[StatusID] [smallint] IDENTITY(1,1) NOT NULL,
	[StatusDescr] [varchar](75) NOT NULL CONSTRAINT [DF_CNFG_Status_StatusDescription]  DEFAULT ('Not Reviewed'),
	[RecordBeginDate] [date] NOT NULL,
	[RecordEndDate] [date] NULL,
	[boolIsActive] [bit] NOT NULL,
	[created_by] [varchar](50) NOT NULL CONSTRAINT [DF_CNFG_Status_created_by]  DEFAULT (suser_name()),
	[created_dt] [datetime] NOT NULL CONSTRAINT [DF_CNFG_Status_created_dt]  DEFAULT (getdate()),
	[modified_by] [varchar](50) NOT NULL CONSTRAINT [DF_CNFG_Status_modified_by]  DEFAULT (suser_name()),
	[modified_dt] [datetime] NOT NULL CONSTRAINT [DF_CNFG_Status_modified_dt]  DEFAULT (getdate()),
 CONSTRAINT [PK_CNFG_Status] PRIMARY KEY CLUSTERED 
(
	[StatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 95) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [mbr].[CNFG_SystemUser]    Script Date: 11/28/2016 9:07:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [mbr].[CNFG_SystemUser](
	[SystemUserId] [int] IDENTITY(1,1) NOT NULL,
	[DomainLogin] [varchar](30) NOT NULL,
	[UserName] [varchar](100) NOT NULL,
	[EmailAddress] [varchar](150) NOT NULL,
	[boolIsActive] [bit] NOT NULL CONSTRAINT [DF_CNFG_SystemUser_boolIsActive]  DEFAULT ((0)),
	[boolIsAdmin] [bit] NOT NULL CONSTRAINT [DF_CNFG_SystemUser_boolIsAdmin]  DEFAULT ((0)),
	[boolIsAssignable] [bit] NOT NULL CONSTRAINT [DF_CNFG_SystemUser_boolIsManager]  DEFAULT ((1)),
	[created_by] [varchar](50) NOT NULL CONSTRAINT [DF_SysemUser_created_by]  DEFAULT (suser_name()),
	[created_dt] [datetime] NOT NULL CONSTRAINT [DF_SysemUser_created_dt]  DEFAULT (getdate()),
	[modified_by] [varchar](50) NOT NULL CONSTRAINT [DF_SysemUser_modified_by]  DEFAULT (suser_name()),
	[modified_dt] [datetime] NOT NULL CONSTRAINT [DF_SysemUser_modified_dt]  DEFAULT (getdate()),
 CONSTRAINT [PK_CNFG_SystemUser_SystemUserID] PRIMARY KEY CLUSTERED 
(
	[SystemUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [mbr].[MemberFeedback]    Script Date: 11/28/2016 9:07:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [mbr].[MemberFeedback](
	[MemberFeedbackID] [int] IDENTITY(1,1) NOT NULL,
	[MemberFeedbackCode] [varchar](15) NULL,
	[CategoryID] [smallint] NOT NULL,
	[FeedbackTypeID] [smallint] NOT NULL,
	[FeedbackSourceID] [smallint] NOT NULL,
	[StatusID] [smallint] NOT NULL,
	[FunctionalID] [smallint] NOT NULL,
	[Branch] [smallint] NOT NULL,
	[AccountNumber] [varchar](15) NULL,
	[MemberName] [varchar](40) NOT NULL,
	[Comment] [varchar](500) NOT NULL,
	[AssignedToSystemUserID] [int] NULL,
	[AssignedToCopy] [varchar](250) NULL,
	[AssignedDate] [date] NULL,
	[Resolution] [varchar](500) NULL,
	[Resolution_byID] [int] NULL,
	[boolDeleted] [bit] NOT NULL CONSTRAINT [DF_MemberFeedback_boolDeleted]  DEFAULT ((0)),
	[created_by] [varchar](50) NOT NULL CONSTRAINT [DF_MemberFeedback_created_by]  DEFAULT (suser_name()),
	[created_dt] [datetime] NOT NULL CONSTRAINT [DF_MemberFeedback_created_dt]  DEFAULT (getdate()),
	[modified_by] [varchar](50) NOT NULL CONSTRAINT [DF_MemberFeedback_modified_by]  DEFAULT (suser_name()),
	[modified_dt] [datetime] NOT NULL CONSTRAINT [DF_MemberFeedback_modified_dt]  DEFAULT (getdate()),
 CONSTRAINT [PK_MemberFeedback_MemberFeedbackID] PRIMARY KEY CLUSTERED 
(
	[MemberFeedbackID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [mbr].[MemberFeedback_History]    Script Date: 11/28/2016 9:07:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [mbr].[MemberFeedback_History](
	[MemberFeedbackHistoryID] [bigint] IDENTITY(1,1) NOT NULL,
	[HistoryEnum] [int] NOT NULL CONSTRAINT [DF_MemberFeedback_History_HistoryEnum]  DEFAULT ((0)),
	[MemberFeedbackID] [int] NOT NULL,
	[MemberFeedbackCode] [varchar](15) NULL,
	[CategoryID] [smallint] NOT NULL,
	[FeedbackTypeID] [smallint] NOT NULL,
	[FeedbackSourceID] [smallint] NOT NULL,
	[StatusID] [smallint] NOT NULL,
	[FunctionalID] [int] NOT NULL,
	[Branch] [smallint] NOT NULL,
	[AccountNumber] [varchar](15) NULL,
	[MemberName] [varchar](40) NOT NULL,
	[Comment] [varchar](500) NOT NULL,
	[AssignedToSystemUserID] [int] NULL,
	[AssignedToCopy] [varchar](250) NULL,
	[AssignedDate] [date] NULL,
	[Resolution] [varchar](500) NULL,
	[Resolution_byID] [int] NULL,
	[boolDeleted] [bit] NOT NULL,
	[created_by] [varchar](50) NOT NULL,
	[created_dt] [datetime] NOT NULL,
	[modified_by] [varchar](50) NOT NULL,
	[modified_dt] [datetime] NOT NULL,
	[inserted_by] [varchar](50) NOT NULL,
	[inserted_dt] [datetime] NOT NULL,
 CONSTRAINT [PK_MemberFeedback_History] PRIMARY KEY CLUSTERED 
(
	[MemberFeedbackHistoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [dbo].[vw_MemberFeedbackDetails]    Script Date: 11/28/2016 9:07:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vw_MemberFeedbackDetails]
AS

SELECT	 mf.MemberFeedbackID
		, mf.MemberFeedbackCode
		, mf.CategoryID
		, c.CategoryDescr
		, mf.FeedbackTypeID
		, fb.FeedbackTypeDescr
		, mf.FeedbackSourceID
		, so.SourceDescr
		, mf.StatusID
		, s.StatusDescr
		, mf.FunctionalID
		, fa.FunctionalDescr
		, mf.Branch
		, mf.AccountNumber
		, mf.MemberName
		, mf.Comment
		, mf.AssignedtoSystemUserID
		, mf.AssignedToCopy
		, mf.AssignedDate
		, mf.Resolution
		, mf.Resolution_byID
		, mf.created_dt
		, mf.created_by
		, mf.modified_by
		, mf.modified_dt
FROM	mbr.MemberFeedback mf
LEFT JOIN mbr.cnfg_category c
ON		mf.categoryid = c.categoryid
LEFT JOIN mbr.CNFG_feedbacktype fb
ON		mf.feedbacktypeid = fb.feedbacktypeid
left join mbr.cnfg_source so
ON		mf.feedbacksourceid = so.sourceid
LEFT JOIN mbr.cnfg_status s
ON		mf.statusid = s.statusid
LEFT JOIN mbr.CNFG_FunctionalArea fa
ON		mf.functionalid = fa.functionalid
LEFT JOIN mbr.cnfg_systemuser su
ON		mf.assignedtosystemuserid = su.systemuserid
left join mbr.cnfg_systemuser sures
ON		mf.resolution_byid = su.systemuserid


GO
ALTER TABLE [dbo].[AuditLog] ADD  CONSTRAINT [DF_AuditLog_modified_by]  DEFAULT (suser_name()) FOR [Modified_by]
GO
ALTER TABLE [dbo].[AuditLog] ADD  CONSTRAINT [DF_AuditLog_modified_dt]  DEFAULT (getdate()) FOR [Modified_dt]
GO
ALTER TABLE [mbr].[MemberFeedback]  WITH CHECK ADD  CONSTRAINT [fk_MemberFeedback_AssignedToSystemUserID] FOREIGN KEY([AssignedToSystemUserID])
REFERENCES [mbr].[CNFG_SystemUser] ([SystemUserId])
GO
ALTER TABLE [mbr].[MemberFeedback] CHECK CONSTRAINT [fk_MemberFeedback_AssignedToSystemUserID]
GO
ALTER TABLE [mbr].[MemberFeedback]  WITH CHECK ADD  CONSTRAINT [fk_MemberFeedback_CategoryID] FOREIGN KEY([CategoryID])
REFERENCES [mbr].[CNFG_Category] ([CategoryID])
GO
ALTER TABLE [mbr].[MemberFeedback] CHECK CONSTRAINT [fk_MemberFeedback_CategoryID]
GO
ALTER TABLE [mbr].[MemberFeedback]  WITH CHECK ADD  CONSTRAINT [fk_MemberFeedback_FeedbackSourceID] FOREIGN KEY([FeedbackSourceID])
REFERENCES [mbr].[CNFG_Source] ([SourceID])
GO
ALTER TABLE [mbr].[MemberFeedback] CHECK CONSTRAINT [fk_MemberFeedback_FeedbackSourceID]
GO
ALTER TABLE [mbr].[MemberFeedback]  WITH CHECK ADD  CONSTRAINT [fk_MemberFeedback_FeedbackTypeID] FOREIGN KEY([FeedbackTypeID])
REFERENCES [mbr].[CNFG_FeedbackType] ([FeedbackTypeID])
GO
ALTER TABLE [mbr].[MemberFeedback] CHECK CONSTRAINT [fk_MemberFeedback_FeedbackTypeID]
GO
ALTER TABLE [mbr].[MemberFeedback]  WITH NOCHECK ADD  CONSTRAINT [fk_MemberFeedback_FunctionalID] FOREIGN KEY([FunctionalID])
REFERENCES [mbr].[CNFG_FunctionalArea] ([FunctionalID])
GO
ALTER TABLE [mbr].[MemberFeedback] CHECK CONSTRAINT [fk_MemberFeedback_FunctionalID]
GO
ALTER TABLE [mbr].[MemberFeedback]  WITH CHECK ADD  CONSTRAINT [fk_MemberFeedback_ResolutionByID] FOREIGN KEY([Resolution_byID])
REFERENCES [mbr].[CNFG_SystemUser] ([SystemUserId])
GO
ALTER TABLE [mbr].[MemberFeedback] CHECK CONSTRAINT [fk_MemberFeedback_ResolutionByID]
GO
ALTER TABLE [mbr].[MemberFeedback]  WITH CHECK ADD  CONSTRAINT [fk_MemberFeedback_StatusID] FOREIGN KEY([StatusID])
REFERENCES [mbr].[CNFG_Status] ([StatusID])
GO
ALTER TABLE [mbr].[MemberFeedback] CHECK CONSTRAINT [fk_MemberFeedback_StatusID]
GO
/****** Object:  StoredProcedure [dbo].[usp_rpt_MemberFeedback_Detail]    Script Date: 11/28/2016 9:07:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_rpt_MemberFeedback_Detail]
/*
Author:       William Platt
Purpose:      Member Feedback detail report for Report Manager
Created:      06/13/2014
Usuage:		  EXEC dbo.usp_rpt_MemberFeedback_Detail '06/01/2014', '06/13/2014' , '1, 2, 3', 'Online', '1, 2, 3'
			  EXEC dbo.usp_rpt_MemberFeedback_Detail '06/01/2014', '06/13/2014' , '3', 'Online', '3'

History:      Who             Date            Description
1)            WAP             06/13/2014      Initial Creation
2)			  LJO			  10/03/2016	  Migrated to MemberFeedback and updated table references
3)			  LJO			  10/07/2016	  Added boolDeleted criteria
*/
	  @dteStart			DATE
	, @dteEnd			DATE
	, @strTypeID		VARCHAR(1000)
	, @strSource		VARCHAR(1000)
	, @strCategoryID	VARCHAR(1000)
AS

--Get detail info for report
SELECT
	  c.CategoryDescr
	, TypeDescr = ft.FeedbackTypeDescr
	, FeedbackSource = s.SourceDescr
	, f.AccountNumber
	, f.MemberName
	, f.Comment
	, f.created_by
	, f.created_dt
	, f.Branch
	, f.Resolution
FROM mbr.MemberFeedback f
INNER JOIN mbr.CNFG_Category c
	ON f.CategoryID = c.CategoryID
INNER JOIN mbr.CNFG_FeedbackType ft
	ON f.FeedbackTypeID = ft.FeedbackTypeID
JOIN mbr.CNFG_Source s
ON	f.FeedbackSourceID = s.SourceID
WHERE f.created_dt >= @dteStart
AND f.created_dt < DATEADD(dd, 1, @dteEnd)
AND f.FeedbackTypeID IN (SELECT * FROM dbo.udf_get_StringInput(@strTypeID))
AND s.SourceDescr IN (SELECT * FROM dbo.udf_get_StringInput(@strSource))
AND f.CategoryID IN (SELECT * FROM dbo.udf_get_StringInput(@strCategoryID))
AND f.boolDeleted = 'false'
GO
/****** Object:  StoredProcedure [dbo].[usp_rpt_MemberFeedback_Source_Summary]    Script Date: 11/28/2016 9:07:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_rpt_MemberFeedback_Source_Summary]
/*
Author:       Ben Tripp
Purpose:      Member Feedback Source summary report for Report Manager.  This procedure is used for the
				feedback source totals.
Created:      07/11/2014
Usage:		  EXEC dbo.usp_rpt_MemberFeedback_Source_Summary '05/01/2016', '05/31/2016'
			  

History:      Who             Date            Description
1)            BTT             07/11/2014      Initial Creation
2)			  LJO			  10/03/2016	  Migrated to MemberFeedback and updated table references
3)			  LJO			  10/07/2016	  Added boolDeleted criteria
*/
	  @dteStart			DATE
	, @dteEnd			DATE
AS

--Get summary info for report
--Put info into a CTE so we can get aggregate detail along with group/grand totals
;WITH FeedbackSourceSummary
AS
(
	SELECT	FeedbackSource = SourceDescr
			, FeedbackCount = COUNT(*)
	FROM	mbr.MemberFeedback f
	JOIN	mbr.CNFG_Source s
	ON		f.FeedbackSourceID = s.SourceID
	WHERE	f.created_dt >= @dteStart
	AND		f.created_dt < DATEADD(dd, 1, @dteEnd)
	AND		f.boolDeleted = 'false'
	GROUP BY s.SourceDescr
)

--Output data with grand totals and report group totals needed for reports % to total calcs
SELECT
	 f.FeedbackSource
	, f.FeedbackCount
	, TotalFeedbackCount = gt.FeedbackCount
FROM FeedbackSourceSummary f
CROSS JOIN
(
	SELECT
		FeedbackCount = SUM(FeedbackCount)
	FROM FeedbackSourceSummary
) gt
ORDER BY f.FeedbackSource


GO
/****** Object:  StoredProcedure [dbo].[usp_rpt_MemberFeedback_Summary]    Script Date: 11/28/2016 9:07:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_rpt_MemberFeedback_Summary]
/*
Author:       William Platt
Purpose:      Member Feedback summary report for Report Manager.  This procedure is used for the
				type/category and the category/type totals depending on what the grouping type switch is set to.
Created:      07/10/2014
Usuage:		  EXEC dbo.usp_rpt_MemberFeedback_Summary '05/01/2014', '05/31/2014', 1
			  EXEC dbo.usp_rpt_MemberFeedback_Summary '05/01/2014', '05/31/2014', 2

History:      Who             Date            Description
1)            WAP             07/10/2014      Initial Creation
2)			  LJO			  10/03/2016	  Migrated to MemberFeedback and updated references
3)			  LJO			  10/07/2016	  Added boolDeleted criteria

@intGroupingType
	1 = Type and then Category
	2 = Category and then Type
*/
	  @dteStart			DATE
	, @dteEnd			DATE
	, @intGroupingType	INT
AS

--Get summary info for report
--Put info into a CTE so we can get aggregate detail along with group/grand totals
;WITH FeedbackSummary
AS
(
	SELECT
		  ReportGroup1ID
		, ReportGroup1
		, ReportGroup2ID
		, ReportGroup2
		, FeedbackCount = COUNT(*)
	FROM
	(
		SELECT
			  ReportGroup1ID = CASE
								WHEN @intGroupingType = 1 THEN ft.FeedbackTypeID
								WHEN @intGroupingType = 2 THEN c.CategoryID
								END
			, ReportGroup1 = CASE
								WHEN @intGroupingType = 1 THEN ft.FeedbackTypeDescr
								WHEN @intGroupingType = 2 THEN c.CategoryDescr
								END
			, ReportGroup2ID = CASE
								WHEN @intGroupingType = 1 THEN c.CategoryID
								WHEN @intGroupingType = 2 THEN ft.FeedbackTypeID
								END
			, ReportGroup2 = CASE
								WHEN @intGroupingType = 1 THEN c.CategoryDescr
								WHEN @intGroupingType = 2 THEN ft.FeedbackTypeDescr
								END
			, f.MemberFeedbackID
		FROM mbr.MemberFeedback f
		INNER JOIN mbr.CNFG_Category c
			ON f.CategoryID = c.CategoryID
		INNER JOIN mbr.CNFG_FeedbackType ft
			ON f.FeedbackTypeID = ft.FeedbackTypeID
		WHERE f.created_dt >= @dteStart
		AND f.created_dt < DATEADD(dd, 1, @dteEnd)
		AND	f.boolDeleted = 'false'
	) t
	GROUP BY
		  ReportGroup1ID
		, ReportGroup1
		, ReportGroup2ID
		, ReportGroup2
) 

--Output data with grand totals and report group totals needed for reports % to total calcs
SELECT
	  f.ReportGroup1ID
	, f.ReportGroup1
	, f.ReportGroup2ID
	, f.ReportGroup2
	, f.FeedbackCount
	, TotalFeedbackCount = gt.FeedbackCount
	, ReportGroup1FeedbackCount = rg1.FeedbackCount
FROM FeedbackSummary f
CROSS JOIN
(
	SELECT
		FeedbackCount = SUM(FeedbackCount)
	FROM FeedbackSummary
) gt
INNER JOIN
(
	SELECT
		  ReportGroup1ID
		, FeedbackCount = SUM(FeedbackCount)
	FROM FeedbackSummary
	GROUP BY ReportGroup1ID
) rg1
	ON f.ReportGroup1ID = rg1.ReportGroup1ID
ORDER BY f.ReportGroup1, f.ReportGroup2


GO
/****** Object:  StoredProcedure [dbo].[usp_rpt_MemberFeedback_Trend]    Script Date: 11/28/2016 9:07:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_rpt_MemberFeedback_Trend]
/*
Author:       William Platt
Purpose:      Member Feedback trend report for Report Manager.  Based on the selected end date it will chart the prior 12 months of data
				for the selected type/source/category.  If end date selected is an EOM date, it will chart the prior 12 months starting
				at the selected EOM date
Created:      06/26/2014
Usage:		  EXEC dbo.usp_rpt_MemberFeedback_Trend '06/13/2014' , '1, 2, 3', 'Online', '1, 2, 3'
			  EXEC dbo.usp_rpt_MemberFeedback_Trend '06/30/2014' , '1, 2, 3', 'Online', '1, 2, 3'

History:      Who             Date            Description
1)            WAP             06/26/2014      Initial Creation
2)			  LJO			  10/03/2016	  Migrated to MemberFeedback and updated references
3)			  LJO			  10/07/2016	  Added boolDeleted criteria

*/
	  @dteEnd			DATE
	, @strTypeID		VARCHAR(1000)
	, @strSource		VARCHAR(1000)
	, @strCategoryID	VARCHAR(1000)
AS

DECLARE @dteStart DATE

--Get range of dates to chart
SELECT
	  @dteStart = DATEADD(mm, -12, DATEADD(dd, 1, dteEnd))
	, @dteEnd = dteEnd
FROM
(
	SELECT
			dteEnd = CASE
					WHEN ReportDate = MonthEndDate THEN MonthEndDate
					ELSE DATEADD(dd, -1, MonthBeginDate)
					END
	FROM	mbr.CNFG_ReportDate
	WHERE	ReportDate = @dteEnd
) t

--Get aggregate info by month for the report
SELECT
	  Enum = ROW_NUMBER() OVER(ORDER BY TrendYear, TrendMonth)
	, TrendMonth
	, TrendYear
	, TrendDescr = LEFT(d.ReportMonthName,3) + ' ' + CAST(d.ReportYear AS CHAR(4))
	, MonthBeginDate = d.MonthBeginDate
	, MonthEndDate = d.MonthEndDate
	, FeedbackCount
FROM
(
	SELECT
			  TrendMonth = MONTH(f.created_dt)
			, TrendMonthName = DATENAME(MONTH, f.created_dt)
			, TrendYear = YEAR(f.created_dt)
			, FeedbackCount = COUNT(*)
	FROM	mbr.MemberFeedback f
	JOIN	mbr.CNFG_Category c
	ON		f.CategoryID = c.CategoryID
	JOIN	mbr.CNFG_FeedbackType ft
	ON		f.FeedbackTypeID = ft.FeedbackTypeID
	JOIN	mbr.CNFG_Source s
	ON		f.FeedbackSourceID = s.SourceID
	WHERE	f.created_dt >= @dteStart
	AND		f.created_dt < DATEADD(dd, 1, @dteEnd)
	AND		f.FeedbackTypeID IN (SELECT * FROM dbo.udf_get_StringInput(@strTypeID))
	AND		s.SourceDescr IN (SELECT * FROM dbo.udf_get_StringInput(@strSource))
	AND		f.CategoryID IN (SELECT * FROM dbo.udf_get_StringInput(@strCategoryID))
	AND		f.boolDeleted = 'false'
	GROUP BY
		  MONTH(f.created_dt)
		, DATENAME(MONTH, f.created_dt)
		, YEAR(f.created_dt)
) f
JOIN	mbr.CNFG_ReportDate d
ON		f.TrendMonth = d.ReportMonth
AND		f.TrendYear = d.ReportYear
AND		d.ReportDate = d.MonthEndDate
ORDER BY Enum

GO
/****** Object:  Trigger [mbr].[trg_upd_MemberFeedbackCategory]    Script Date: 11/28/2016 9:07:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [mbr].[trg_upd_MemberFeedbackCategory] ON [mbr].[CNFG_Category]
FOR UPDATE
/*
Author:       William Platt
Purpose:      When record is updated make sure modified_dt and modified_by columns are modified
				This helps if an admin manually updates the table directly
Created:      03/05/2014

History:      Who             Date            Description
1)            WAP             03/05/2014      Initial Creation
2)			  LJO			  09/29/2016	  Modified to refer to renamed table, and changed user_name to suser_name
 
*/
AS

UPDATE c
SET
	  modified_dt = GETDATE()
	, modified_by = SUSER_NAME()
FROM mbr.CNFG_Category c INNER JOIN inserted i
	ON c.CategoryID = i.CategoryID

GO
/****** Object:  Trigger [mbr].[trg_ins_upd_GenerateMemberFeedbackCode]    Script Date: 11/28/2016 9:07:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [mbr].[trg_ins_upd_GenerateMemberFeedbackCode] ON [mbr].[MemberFeedback]
AFTER INSERT, UPDATE
/*
Author:		Leslie Owen
Purpose:	assign user friendly code for end users based on the recordid, and prevent updates
			  made that would cause duplicates
Created:	09/30/2016

History:	Who			Date		Description
1)			LJO			09/30/2016	Initial Description

*/
AS

IF EXISTS(
SELECT	*
FROM	inserted
WHERE	MemberFeedbackCode IS NULL
)
BEGIN
UPDATE	mf
SET		MemberFeedbackCode = cast(year(mf.created_dt) AS CHAR(4)) + '-' + REPLICATE('0', 6 - LEN(mf.Memberfeedbackid)) + CAST(mf.memberfeedbackid as VARCHAR)
		, Modified_dt = GETDATE()
		, Modified_by = OBJECT_NAME(@@PROCID)
FROM	inserted i
JOIN	mbr.MemberFeedback mf
ON		i.MemberFeedbackID = i.MemberFeedbackID
WHERE	mf.MemberFeedbackCode IS NULL	
END

-- nulls would be handled above
-- if the memberfeedbackid is not at the end of the memberfeedbackcode, impose the standardized code
-- we can still create a non-standard format, such as 0000-000___ as long as the memberfeedbackid is at the end
IF EXISTS(
SELECT	*
FROM	inserted 
WHERE	RIGHT(memberfeedbackcode, len(memberfeedbackid)) <> MemberFeedbackID
)
BEGIN
UPDATE	mf
SET		MemberFeedbackCode = cast(year(mf.created_dt) AS CHAR(4)) + '-' + REPLICATE('0', 6 - LEN(mf.Memberfeedbackid)) + CAST(mf.memberfeedbackid as VARCHAR)
		, Modified_dt = GETDATE()
		, Modified_by = OBJECT_NAME(@@PROCID)
FROM	inserted i
JOIN	mbr.MemberFeedback mf
ON		i.MemberFeedbackID = i.MemberFeedbackID
WHERE	RIGHT(RTRIM(LTRIM(i.memberfeedbackcode)), len(i.memberfeedbackid)) <> i.MemberFeedbackID

END

DECLARE @T_History AS TABLE
(
	  MemberFeedbackID	INT
	, HistoryEnum		INT
)

BEGIN TRANSACTION MemberHistory

IF (NOT UPDATE(modified_dt) OR NOT UPDATE(modified_by))
	BEGIN
		UPDATE  mf
		SET		modified_dt = GETDATE()
				, modified_by = SUSER_NAME()
		FROM	mbr.MemberFeedback mf
		JOIN	inserted i
		ON		mf.MemberFeedbackID = i.MemberFeedbackID
	END

	INSERT INTO @T_History
	SELECT	i.MemberFeedbackID
			, HistoryEnum = COALESCE(h.HistoryEnum,0)
	FROM	inserted i
	LEFT JOIN
	(
	SELECT	  MemberFeedbackID
			, HistoryEnum = MAX(HistoryEnum)
	FROM	mbr.MemberFeedback_History
	GROUP BY MemberFeedbackID
	)	h
	ON	i.MemberFeedbackID = h.MemberFeedbackID

	INSERT INTO mbr.MemberFeedback_History
	(
	  MemberFeedbackID
	, HistoryEnum
	, MemberFeedbackCode
	, CategoryID
	, FeedbackTypeID
	, FeedbackSourceID
	, StatusID
	, FunctionalID
	, Branch
	, AccountNumber
	, MemberName
	, Comment
	, AssignedToSystemUserID
	, AssignedToCopy
	, AssignedDate
	, Resolution
	, Resolution_byID
	, boolDeleted
	, created_by
	, created_dt
	, modified_by
	, modified_dt		
	, inserted_by
	, inserted_dt
	)

	SELECT	  d.MemberFeedbackID
			, HistoryEnum = h.HistoryEnum + 1
			, d.MemberFeedbackCode
			, d.CategoryID
			, d.FeedbackTypeID
			, d.FeedbackSourceID
			, d.StatusID
			, d.FunctionalID
			, d.Branch
			, d.AccountNumber
			, d.MemberName
			, d.Comment
			, d.AssignedToSystemUserID
			, d.AssignedToCopy
			, d.AssignedDate
			, d.Resolution
			, d.Resolution_byID
			, d.boolDeleted
			, d.created_by
			, d.created_dt
			, d.modified_by
			, d.modified_dt
			, inserted_by = CASE
							WHEN UPDATE(modified_by) THEN i.modified_by
							ELSE SUSER_NAME()
							END
			, inserted_dt = GETDATE()
	FROM	deleted d
	JOIN	@T_History h
	ON		d.MemberFeedbackID = h.MemberFeedbackID
	JOIN	inserted i
	ON		d.MemberFeedbackID = i.MemberFeedbackID

COMMIT TRANSACTION MemberHistory

GO
USE [master]
GO
ALTER DATABASE [MemberFeedback] SET  READ_WRITE 
GO
