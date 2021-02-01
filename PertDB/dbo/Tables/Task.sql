CREATE TABLE [dbo].[Task]
(
	[TaskId] INT IDENTITY NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Description] NVARCHAR(MAX) NULL, 
    [MinEstDuration] FLOAT NULL, 
    [MaxEstDuration] FLOAT NULL, 
    [MostLikelyEstDuration] FLOAT NULL, 
    [StartDate] DATETIME NOT NULL, 
    [EndDate] DATETIME NOT NULL, 
    [ModifiedDate] DATETIME NULL, 
    [StatusId] INT NOT NULL, 
    [UserId] INT NULL, 
    [ProjectId] INT NULL,
	CONSTRAINT FK_TaskUser FOREIGN KEY (UserId) REFERENCES [User](UserId),
	CONSTRAINT FK_TaskProject FOREIGN KEY (ProjectId) REFERENCES [Project](ProjectId),
	CONSTRAINT FK_TaskStatus FOREIGN KEY (StatusId) REFERENCES [Status](StatusId)
	
)