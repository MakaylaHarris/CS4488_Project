CREATE TABLE [dbo].[Project](
	[ProjectId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[MinEstDuration] [int] NULL,
	[MaxEstDuration] [int] NULL,
	[MostLikelyEstDuration] [int] NOT NULL,
	[Description] [varchar](max) NULL,
	[Creator] VARCHAR(50) NULL, 
    [CreationDate] DATETIME NOT NULL, 
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
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [CK_Project_End_After_Start] CHECK  (([EndDate]>=[StartDate]))
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [CK_Project_End_After_Start]
GO
ALTER TABLE [dbo].[Project] WITH CHECK ADD CONSTRAINT [FK_Project_Creator] FOREIGN KEY ([Creator])
REFERENCES [dbo].[User] ([UserName])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_Creator]
GO
ALTER TABLE [dbo].[Task]  WITH CHECK ADD  CONSTRAINT [CK_Project_Max] CHECK  (([MaxEstDuration]>=[MostLikelyEstDuration]))
GO
ALTER TABLE [dbo].[Task] CHECK CONSTRAINT [CK_Project_Max]
GO
ALTER TABLE [dbo].[Task]  WITH CHECK ADD  CONSTRAINT [CK_Project_Min] CHECK  (([MinEstDuration]<=[MostLikelyEstDuration] AND [MinEstDuration]>=0))
GO
ALTER TABLE [dbo].[Task] CHECK CONSTRAINT [CK_Project_Min]
