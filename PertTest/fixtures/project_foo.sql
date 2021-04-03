Declare @username VarChar(3) = 'Joe';
INSERT INTO [dbo].[User] (UserName, [Password], Email, [Name])
VALUES (@username, 'Pass', 'Test@email.com', 'Joe Schmoe');

INSERT INTO [dbo].[Project] ([Name], [StartDate], [EndDate], [Description], [CreationDate], [Creator], [MostLikelyEstDuration])
VALUES (N'Foo', N'2021-02-06 00:00:00', N'2021-05-07 00:00:00', N'A test project', '2/5/2021', 'Joe', 30);

DECLARE @projectId int = (SELECT ProjectId FROM Project WHERE [Name] = 'Foo');
DECLARE @CreationDate DateTime, @Result BIT, @ResultId int, @ChildRowNum int;
-- Tasks
EXEC [dbo].CreateTask 'Foo', 'This is the first task', 1, 2, 3, '2/6/2021', '2/8/2021', @projectId, @username, @CreationDate, @Result, @ResultId, @ChildRowNum, 0, 0;

EXEC [dbo].CreateTask 'Bar', 'This is the second task', 2, 3, 5, '2/8/2021', '2/10/2021',  @projectId, @username, @CreationDate, @Result, @ResultId, @ChildRowNum, 0, 0;

EXEC [dbo].CreateTask 'Task3', 'This is the third task', 1, 2, 2, '2/9/2021', '2/11/2021', @projectId, @username, @CreationDate, @Result, @ResultId, @ChildRowNum, 0, 0;

EXEC [dbo].CreateTask 'Task4', 'This task has an earlier start date on purpose', 3, 5, 7, '2/6/2021', '2/8/2021', @projectId, @username, @CreationDate, @Result, @ResultId, @ChildRowNum, 0, 0;

EXEC [dbo].CreateTask 'Task5', 'This is the fifth task and is not complete', 2, 3, 10, '2/8/2021', Null,  @projectId, @username, @CreationDate, @Result, @ResultId, @ChildRowNum, 0, 0;
