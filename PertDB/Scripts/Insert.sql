
USE SmartPertDB;
GO

/*******************************************************************************
* Test data
* Added 2/6/2021 by Robert Nelson
********************************************************************************/
-- Users
INSERT INTO [dbo].[User] (UserName, [Password], Email, [Name])
VALUES ('TestUser', 'Pass', 'Test@email.com', 'Test Name');

INSERT INTO [dbo].[User] (UserName, [Password], Email, [Name])
VALUES ('TestUser2', 'Pass2', 'Test2@email.com', 'Test Name2');

INSERT INTO [dbo].[User] (UserName, [Password], Email, [Name])
VALUES ('TestUser3', 'Pass3', 'Test3@email.com', 'Test Name3');

-- Projects
INSERT INTO [dbo].[Project] ([Name], [StartDate], [EndDate], [Description], [CreationDate], [Creator], [MostLikelyEstDuration])
VALUES (N'Test', N'2021-02-06 00:00:00', N'2021-05-07 00:00:00', N'A test project', '2/5/2021', 'TestUser', 60);

DECLARE @projectId int = (SELECT ProjectId FROM Project WHERE [Name] = 'Test');
DECLARE @CreationDate DateTime, @Result BIT, @ResultId int, @ChildRowNum int;
-- Tasks
EXEC [dbo].CreateTask 'Task1', 'This is the first task', 1, 2, 3, '2/6/2021', '2/8/2021', @projectId, 'TestUser', @CreationDate, @Result, @ResultId, @ChildRowNum, 0, 0;

EXEC [dbo].CreateTask 'Task2', 'This is the second task', 2, 3, 5, '2/8/2021', '2/10/2021',  @projectId, 'TestUser', @CreationDate, @Result, @ResultId, @ChildRowNum, 0, 0;

EXEC [dbo].CreateTask 'Task3', 'This is the third task', 1, 2, 2, '2/9/2021', '2/11/2021', @projectId, 'TestUser', @CreationDate, @Result, @ResultId, @ChildRowNum, 0, 0;

EXEC [dbo].CreateTask 'Task4', 'This task has an earlier start date on purpose', 3, 5, 7, '2/6/2021', '2/8/2021', @projectId, 'TestUser', @CreationDate, @Result, @ResultId, @ChildRowNum, 0, 0;

EXEC [dbo].CreateTask 'Task5', 'This is the fifth task and is not complete', 2, 3, 10, '2/8/2021', Null,  @projectId, 'TestUser', @CreationDate, @Result, @ResultId, @ChildRowNum, 0, 0;

EXEC [dbo].CreateTask 'Task6', 'This is the sixth task and is not complete', 3, 5, 8, '2/20/2021', NULL,  @projectId, 'TestUser', @CreationDate, @Result, @ResultId, @ChildRowNum, 0, 0;


-- Dependencies
INSERT INTO [dbo].Dependency (RootId, DependentId) VALUES (dbo.GetTaskId('Task1'), dbo.GetTaskId('Task2'));
INSERT INTO [dbo].Dependency (RootId, DependentId) VALUES (dbo.GetTaskId('Task1'), dbo.GetTaskId('Task3'));
INSERT INTO [dbo].Dependency (RootId, DependentId) VALUES (dbo.GetTaskId('Task3'), dbo.GetTaskId('Task6'));
INSERT INTO [dbo].Dependency (RootId, DependentId) VALUES (dbo.GetTaskId('Task5'), dbo.GetTaskId('Task6'));

-- Add users to project
INSERT INTO [dbo].UserProject (UserName, ProjectId) VALUES ('TestUser', @projectId);
INSERT INTO [dbo].UserProject (UserName, ProjectId) VALUES ('TestUser2', @projectId);
INSERT INTO [dbo].UserProject (UserName, ProjectId) VALUES ('TestUser3', @projectId);

-- Add users to tasks
INSERT INTO [dbo].UserTask (UserName, TaskId) VALUES ('TestUser', dbo.GetTaskId('Task1'));
INSERT INTO [dbo].UserTask (UserName, TaskId) VALUES ('TestUser2', dbo.GetTaskId('Task1'));
INSERT INTO [dbo].UserTask (UserName, TaskId) VALUES ('TestUser', dbo.GetTaskId('Task3'));
INSERT INTO [dbo].UserTask (UserName, TaskId) VALUES ('TestUser3', dbo.GetTaskId('Task4'));
INSERT INTO [dbo].UserTask (UserName, TaskId) VALUES ('TestUser3', dbo.GetTaskId('Task5'));
INSERT INTO [dbo].UserTask (UserName, TaskId) VALUES ('TestUser2', dbo.GetTaskId('Task6'));
INSERT INTO [dbo].UserTask (UserName, TaskId) VALUES ('TestUser3', dbo.GetTaskId('Task1'));

-- Add SubTask
INSERT INTO [dbo].SubTask (SubTaskId, ParentTaskId) VALUES (dbo.GetTaskId('Task6'), dbo.GetTaskId('Task5'));

PRINT 'Finished Inserting Test Data';

