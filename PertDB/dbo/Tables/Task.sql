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
ALTER TABLE [dbo].[Task]  WITH CHECK ADD  CONSTRAINT [FK_Project] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[Project] ([ProjectId])
GO
ALTER TABLE [dbo].[Task] CHECK CONSTRAINT [FK_Project]
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
