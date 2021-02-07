USE [master]
GO
/****** Object:  Database [Pert]    Script Date: 2/7/2021 2:18:53 AM ******/
CREATE DATABASE [Pert]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Pert', SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Pert_log', SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [Pert] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Pert].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Pert] SET ANSI_NULL_DEFAULT ON 
GO
ALTER DATABASE [Pert] SET ANSI_NULLS ON 
GO
ALTER DATABASE [Pert] SET ANSI_PADDING ON 
GO
ALTER DATABASE [Pert] SET ANSI_WARNINGS ON 
GO
ALTER DATABASE [Pert] SET ARITHABORT ON 
GO
ALTER DATABASE [Pert] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Pert] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Pert] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Pert] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Pert] SET CURSOR_DEFAULT  LOCAL 
GO
ALTER DATABASE [Pert] SET CONCAT_NULL_YIELDS_NULL ON 
GO
ALTER DATABASE [Pert] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Pert] SET QUOTED_IDENTIFIER ON 
GO
ALTER DATABASE [Pert] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Pert] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Pert] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Pert] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Pert] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Pert] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Pert] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Pert] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Pert] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Pert] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Pert] SET  MULTI_USER 
GO
ALTER DATABASE [Pert] SET PAGE_VERIFY NONE  
GO
ALTER DATABASE [Pert] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Pert] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Pert] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [Pert] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Pert] SET CHANGE_TRACKING = ON (CHANGE_RETENTION = 2 DAYS,AUTO_CLEANUP = ON)
GO
ALTER DATABASE [Pert] SET QUERY_STORE = OFF
GO
USE [Pert]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
USE [Pert]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Project](
	[ProjectId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[Description] [varchar](max) NULL,
 CONSTRAINT [PK_Project] PRIMARY KEY CLUSTERED 
(
	[ProjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[Project] ENABLE CHANGE_TRACKING WITH(TRACK_COLUMNS_UPDATED = ON)
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Task](
	[TaskId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[Description] [varchar](max) NULL,
	[MinEstDuration] [int] NULL,
	[MaxEstDuration] [int] NULL,
	[MostLikelyEstDuration] [int] NOT NULL,
	[ProjectId] [int] NOT NULL,
 CONSTRAINT [PK_Task] PRIMARY KEY CLUSTERED 
(
	[TaskId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[Task] ENABLE CHANGE_TRACKING WITH(TRACK_COLUMNS_UPDATED = ON)
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [Tasks_View]
	AS SELECT [t].[TaskId], [t].[Name] AS TaskName, [t].[Description], [t].[MinEstDuration], [t].[MaxEstDuration], 
	[t].[MostLikelyEstDuration], [t].[StartDate] AS TaskStartDate, [t].[EndDate], 
	[t].[ProjectId] AS Id, [p].[ProjectId], [p].[Name], [p].[StartDate] AS ProjectStartDate
	FROM dbo.Task t
	left join dbo.Project p ON t.ProjectId = p.ProjectId
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[UserName] [varchar](50) NOT NULL,
	[Name] [varchar](50) NULL,
	[Email] [varchar](50) NULL,
	[Password] [varchar](50) NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[User] ENABLE CHANGE_TRACKING WITH(TRACK_COLUMNS_UPDATED = ON)
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserProject](
	[UserProjectId] [int] IDENTITY(1,1) NOT NULL,
	[ProjectId] [int] NOT NULL,
	[UserName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_UserProject] PRIMARY KEY CLUSTERED 
(
	[UserProjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[UserProject] ENABLE CHANGE_TRACKING WITH(TRACK_COLUMNS_UPDATED = ON)
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Dependency](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RootId] [int] NOT NULL,
	[DependentId] [int] NOT NULL,
 CONSTRAINT [PK_Dependency] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Dependency] ENABLE CHANGE_TRACKING WITH(TRACK_COLUMNS_UPDATED = ON)
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserTask](
	[UserTaskId] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](50) NOT NULL,
	[TaskId] [int] NOT NULL,
 CONSTRAINT [PK_UserTask] PRIMARY KEY CLUSTERED 
(
	[UserTaskId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[UserTask] ENABLE CHANGE_TRACKING WITH(TRACK_COLUMNS_UPDATED = ON)
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_Project] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Project] ([ProjectId])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_Project]
GO
ALTER TABLE [dbo].[UserProject]  WITH CHECK ADD  CONSTRAINT [FK_UserProject_Project] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Project] ([ProjectId])
GO
ALTER TABLE [dbo].[UserProject] CHECK CONSTRAINT [FK_UserProject_Project]
GO
ALTER TABLE [dbo].[UserProject]  WITH CHECK ADD  CONSTRAINT [FK_UserProject_User] FOREIGN KEY([UserName])
REFERENCES [dbo].[User] ([UserName])
GO
ALTER TABLE [dbo].[UserProject] CHECK CONSTRAINT [FK_UserProject_User]
GO
ALTER TABLE [dbo].[Task]  WITH CHECK ADD  CONSTRAINT [FK_Project] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Project] ([ProjectId])
GO
ALTER TABLE [dbo].[Task] CHECK CONSTRAINT [FK_Project]
GO
ALTER TABLE [dbo].[Dependency]  WITH CHECK ADD  CONSTRAINT [FK_Dependency_Dependent] FOREIGN KEY([DependentId])
REFERENCES [dbo].[Task] ([TaskId])
GO
ALTER TABLE [dbo].[Dependency] CHECK CONSTRAINT [FK_Dependency_Dependent]
GO
ALTER TABLE [dbo].[Dependency]  WITH CHECK ADD  CONSTRAINT [FK_Dependency_Root] FOREIGN KEY([RootId])
REFERENCES [dbo].[Task] ([TaskId])
GO
ALTER TABLE [dbo].[Dependency] CHECK CONSTRAINT [FK_Dependency_Root]
GO
ALTER TABLE [dbo].[UserTask]  WITH CHECK ADD  CONSTRAINT [FK_UserTask_Task] FOREIGN KEY([TaskId])
REFERENCES [dbo].[Task] ([TaskId])
GO
ALTER TABLE [dbo].[UserTask] CHECK CONSTRAINT [FK_UserTask_Task]
GO
ALTER TABLE [dbo].[UserTask]  WITH CHECK ADD  CONSTRAINT [FK_UserTask_User] FOREIGN KEY([UserName])
REFERENCES [dbo].[User] ([UserName])
GO
ALTER TABLE [dbo].[UserTask] CHECK CONSTRAINT [FK_UserTask_User]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [CK_Project_End_After_Start] CHECK  (([EndDate]>=[StartDate]))
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [CK_Project_End_After_Start]
GO
ALTER TABLE [dbo].[Task]  WITH CHECK ADD  CONSTRAINT [CK_Task_End_After_Start] CHECK  (([EndDate]>=[StartDate]))
GO
ALTER TABLE [dbo].[Task] CHECK CONSTRAINT [CK_Task_End_After_Start]
GO
ALTER TABLE [dbo].[Task]  WITH CHECK ADD  CONSTRAINT [CK_Task_Max] CHECK  (([MaxEstDuration]>=[MostLikelyEstDuration]))
GO
ALTER TABLE [dbo].[Task] CHECK CONSTRAINT [CK_Task_Max]
GO
ALTER TABLE [dbo].[Task]  WITH CHECK ADD  CONSTRAINT [CK_Task_Min] CHECK  (([MinEstDuration]<=[MostLikelyEstDuration]))
GO
ALTER TABLE [dbo].[Task] CHECK CONSTRAINT [CK_Task_Min]
GO
ALTER TABLE [dbo].[Dependency]  WITH CHECK ADD  CONSTRAINT [CK_Dependency] CHECK  (([RootId]<>[DependentId]))
GO
ALTER TABLE [dbo].[Dependency] CHECK CONSTRAINT [CK_Dependency]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Robert Nelson
-- Create date: 1/31/2021
-- Description:	Retrieves the current version from changes to Pert
-- =============================================

CREATE PROCEDURE [dbo].[TrackingVersion]
	@ret_value INT OUTPUT
AS
BEGIN
DECLARE @CHANGE_TRACKING_CURRENT_VERSION BIGINT;
EXEC Pert.sys.sp_executesql
  N'SELECT @ReturnValue = CHANGE_TRACKING_CURRENT_VERSION()',
  N'@ReturnValue BIGINT OUTPUT',
  @ReturnValue = @ret_value OUTPUT

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spReadTable]
	@tableName nvarchar(30)
AS
BEGIN
	declare @query nvarchar(500)
	set @query = 'SELECT * FROM dbo.' + @tableName + '_View'
	EXEC(@query)
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/************************************************************
* Attempts to login using the UserId and Password
*	Returns bit (true if successful login)
* Created 2/4/2021 by Robert Nelson
************************************************************/
CREATE Procedure [dbo].[TryLogin]
(
	@UserId Varchar(50),
	@Password Varchar(50),
	@Success bit out
)
AS
IF EXISTS (SELECT 1 from [dbo].[User] where ([User].UserName=@UserId OR [User].Email=@UserId) AND [User].Password = @Password)
	SET @Success = 1
ELSE
	SET @Success = 0;
RETURN
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*******************************************
*	Procedure: Register
*		Attempts to register a user, succeeds if the username is not in use or if the username has no password on it
*	result: True on success
*	Created 2/6/2021 by Robert Nelson
*/
CREATE PROCEDURE [dbo].[Register]
(
	@username varchar(50),
	@password varchar(50),
	@email varchar(50),
	@fullname varchar(50),
	@result BIT OUTPUT
)
AS
BEGIN
	DECLARE @found varchar(50);
	SET @result=0;
	/* Does the user exist in table? */
	IF (SELECT COUNT(*) FROM [User] WHERE [User].UserName=@username) > 0
		BEGIN
			/* Is the user registered? */
			IF (SELECT COUNT(*) FROM [User] WHERE [User].UserName=@username AND [User].Password!='' AND [User].Password IS NOT NULL) > 0
				RETURN;
			UPDATE [User] SET Password=@password, Email=@email, Name=@fullname WHERE [User].UserName=@username;
		END
	ELSE
		INSERT INTO dbo.[User] (UserName, Password, Email, Name) Values(@username, @password, @email, @fullname);
	SET @result=1;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ViewTasks]
AS
BEGIN
	SELECT *
	FROM dbo.Tasks_View
END
GO
