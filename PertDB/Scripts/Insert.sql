
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

/* Old Insert stuff */

-- written by James Kim
-- Insert Test Data for User
/* INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ('Merrill','Hammett','tempor.augue.ac@Donecsollicitudin.ca');
GO

INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ('Nola','Raja','amet@iaculis.com');
GO

INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ('Owen','Melyssa','feugiat.tellus.lorem@sodalesnisimagna.co.uk');
GO

INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ('Teegan','Eden','dolor.elit.pellentesque@faucibusorciluctus.org');
GO

INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ('Nora','Zephania','ante.dictum@fringillaeuismod.edu');
GO

INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ('Karen','Rama','adipiscing.enim@diamloremauctor.net');
GO

INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ('Hanna','Athena','facilisis.facilisis@massaVestibulumaccumsan.net');
GO

INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ('Molly','Kelly','eu.ligula.Aenean@adipiscing.edu');
GO

INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ('Lawrence','Phyllis','Quisque.varius@dictumauguemalesuada.org');
GO

INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ('Zane','Mercedes','felis.ullamcorper@semutcursus.org');
GO

INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ('Ursa','Noah','euismod@eratEtiam.com');
GO

INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ('Bert','Bree','tempor.erat.neque@Donecporttitortellus.org');
GO

INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ('Armando','Gil','lacus.Etiam@adipiscingMaurismolestie.org');
GO

INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ('Maryam','Baker','fermentum.fermentum.arcu@tincidunt.net');
GO

INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ('Ulysses','Julie','Suspendisse.eleifend@Phasellusornare.org');
GO

INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ('Hiroko','Madeson','id.erat@nequeSedeget.org');
GO

INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ('Castor','Porter','dis.parturient@vehiculaaliquetlibero.net');
GO

-- Insert test data for Project
INSERT INTO [dbo].[Project] (Name, StartDate, WorkingHours) VALUES ('Window Application','2019-06-02',2);
GO

INSERT INTO [dbo].[Project] (Name, StartDate, WorkingHours) VALUES ('Linux Application','2020-01-29',4);
GO

INSERT INTO [dbo].[Project] (Name, StartDate, WorkingHours) VALUES ('Mac Application','2020-05-13',3);
GO
-- Insert test data for Status
INSERT INTO [dbo].[Status] (StatusId, Name) VALUES (1, 'Not started');
GO

INSERT INTO [dbo].[Status] (StatusId, Name) VALUES (2, 'In Progress');
GO

INSERT INTO [dbo].[Status] (StatusId, Name) VALUES (3, 'Completed');
GO

-- Insert test data for Task
INSERT INTO [dbo].[Task] (Name, Description, MinEstDuration, MaxEstDuration, MostLikelyEstDuration, StartDate, EndDate, StatusId, UserId, ProjectId) VALUES ('Create button1','Create button 1 to the application',1,5,3,'2019-02-04','2019-02-05',1,2,1);
GO

INSERT INTO [dbo].[Task] (Name, Description, MinEstDuration, MaxEstDuration, MostLikelyEstDuration, StartDate, EndDate, StatusId, UserId, ProjectId) VALUES ('Create button2','Create button 2 to the application',1,5,3,'2019-02-05','2019-02-07',1,3,1);
GO

INSERT INTO [dbo].[Task] (Name, Description, MinEstDuration, MaxEstDuration, MostLikelyEstDuration, StartDate, EndDate, StatusId, UserId, ProjectId) VALUES ('Create button3','Create button 3 to the application',1,5,3,'2019-02-08','2019-02-09',2,4,1);
GO
-- Insert test data for Dependency
INSERT INTO [dbo].[Dependency] (DepOnTaskId, TaskId) VALUES (1, 2);
GO
*/