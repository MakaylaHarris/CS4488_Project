USE [C:\USERS\ROBER\SOURCE\REPOS\PERT\WPF\DATABASE\PERT.MDF]
GO
/****** Object:  Table [dbo].[Dependency]    Script Date: 2/6/2021 8:00:27 PM ******/
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
ALTER TABLE [dbo].[Dependency]  WITH CHECK ADD  CONSTRAINT [CK_Dependency] CHECK  (([RootId]<>[DependentId]))
GO
ALTER TABLE [dbo].[Dependency] CHECK CONSTRAINT [CK_Dependency]
GO
