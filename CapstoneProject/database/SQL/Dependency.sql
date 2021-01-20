CREATE TABLE [dbo].[Dependency]
(
	[DepOnTaskId] INT NOT NULL , 
    [TaskId] INT NOT NULL,
	CONSTRAINT FK_DependencyTaskId FOREIGN KEY(TaskId) REFERENCES Task(TaskId),
	CONSTRAINT FK_DependencyDepOnTaskId FOREIGN KEY (DepOnTaskId) REFERENCES Task(TaskId),
	CONSTRAINT PK_Dependency PRIMARY KEY(DepOnTaskId, TaskId)
)
