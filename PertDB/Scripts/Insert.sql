
USE Pert;
GO

-- written by James Kim
-- Insert Test Data for User
INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ('Merrill','Hammett','tempor.augue.ac@Donecsollicitudin.ca');
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
