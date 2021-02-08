CREATE TABLE [dbo].[UserProject](
	[ProjectId] [int] NOT NULL,
	[UserName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_UserProject] PRIMARY KEY CLUSTERED 
(
	ProjectId, UserName
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[UserProject] ENABLE CHANGE_TRACKING WITH(TRACK_COLUMNS_UPDATED = ON)
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
