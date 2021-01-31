/****** Object:  Table [dbo].[Dependency]    Script Date: 1/30/2021 3:45:09 PM ******/
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
/****** Object:  Table [dbo].[Project]    Script Date: 1/30/2021 3:45:09 PM ******/
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
/****** Object:  Table [dbo].[Task]    Script Date: 1/30/2021 3:45:09 PM ******/
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
/****** Object:  Table [dbo].[User]    Script Date: 1/30/2021 3:45:09 PM ******/
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
/****** Object:  Table [dbo].[UserProject]    Script Date: 1/30/2021 3:45:09 PM ******/
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
/****** Object:  Table [dbo].[UserTask]    Script Date: 1/30/2021 3:45:09 PM ******/
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
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_Project] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Project] ([ProjectId])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_Project]
GO
ALTER TABLE [dbo].[Task]  WITH CHECK ADD  CONSTRAINT [FK_Project] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Project] ([ProjectId])
GO
ALTER TABLE [dbo].[Task] CHECK CONSTRAINT [FK_Project]
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
ALTER TABLE [dbo].[Dependency]  WITH CHECK ADD  CONSTRAINT [CK_Dependency] CHECK  (([RootId]<>[DependentId]))
GO
ALTER TABLE [dbo].[Dependency] CHECK CONSTRAINT [CK_Dependency]
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
