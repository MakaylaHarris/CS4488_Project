USE PertChartDB;
-- written by James Kim
-- Insert Test Data for User
INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ("Merrill","Hammett","tempor.augue.ac@Donecsollicitudin.ca");
INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ("Nola","Raja","amet@iaculis.com");
INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ("Owen","Melyssa","feugiat.tellus.lorem@sodalesnisimagna.co.uk");
INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ("Teegan","Eden","dolor.elit.pellentesque@faucibusorciluctus.org");
INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ("Nora","Zephania","ante.dictum@fringillaeuismod.edu");
INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ("Karen","Rama","adipiscing.enim@diamloremauctor.net");
INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ("Hanna","Athena","facilisis.facilisis@massaVestibulumaccumsan.net");
INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ("Molly","Kelly","eu.ligula.Aenean@adipiscing.edu");
INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ("Lawrence","Phyllis","Quisque.varius@dictumauguemalesuada.org");
INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ("Zane","Mercedes","felis.ullamcorper@semutcursus.org");
INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ("Ursa","Noah","euismod@eratEtiam.com");
INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ("Bert","Bree","tempor.erat.neque@Donecporttitortellus.org");
INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ("Armando","Gil","lacus.Etiam@adipiscingMaurismolestie.org");
INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ("Maryam","Baker","fermentum.fermentum.arcu@tincidunt.net");
INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ("Ulysses","Julie","Suspendisse.eleifend@Phasellusornare.org");
INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ("Hiroko","Madeson","id.erat@nequeSedeget.org");
INSERT INTO [dbo].[User] (FirstName, LastName, EmailAddress) VALUES ("Castor","Porter","dis.parturient@vehiculaaliquetlibero.net");


-- Insert test data for Project
INSERT INTO [dbo].[Project] (Name, StartDate, WorkingHours) VALUES ("Window Application","2019-06-02",2);
INSERT INTO [dbo].[Project] (Name, StartDate, WorkingHours) VALUES ("Linux Application","2020-01-29",4);
INSERT INTO [dbo].[Project] (Name, StartDate, WorkingHours) VALUES ("Mac Application","2020-05-13",3);

-- Insert test data for Task
INSERT INTO [dbo].[Task] (Name, Description, MinEstDuration, MaxEstDuration, MostLikelyEstDuration, StartDate, EndDate, StatusId, UserId, ProjectId) VALUES ("Create button1","Create button 1 to the application",1,5,3,"2019-02-04","2019-02-05",1,2,1);
INSERT INTO [dbo].[Task] (Name, Description, MinEstDuration, MaxEstDuration, MostLikelyEstDuration, StartDate, EndDate, StatusId, UserId, ProjectId) VALUES ("Create button2","Create button 2 to the application",1,5,3,"2019-02-05","2019-02-07",1,3,1);
INSERT INTO [dbo].[Task] (Name, Description, MinEstDuration, MaxEstDuration, MostLikelyEstDuration, StartDate, EndDate, StatusId, UserId, ProjectId) VALUES ("Create button3","Create button 3 to the application",1,5,3,"2019-02-08","2019-02-09",2,4,1);

-- Insert test data for Status
INSERT INTO [dbo].[Status] (StatusId, Name) VALUES (1, "Not started");
INSERT INTO [dbo].[Status] (StatusId, Name) VALUES (2, "In Progress");
INSERT INTO [dbo].[Status] (StatusId, Name) VALUES (3, "Completed");

-- Insert test data for Dependency
INSERT INTO [dbo].[Status] (DepOnTaskId, TaskId) VALUES (1, 2);

