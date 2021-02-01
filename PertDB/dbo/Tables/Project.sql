CREATE TABLE [dbo].[Project]
(
	[ProjectId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NULL, 
    [StartDate] DATETIME NULL, 
    [WorkingHours] FLOAT NULL, 
    [ProjectOwner] INT NULL,
	CONSTRAINT FK_ProjectUser FOREIGN KEY (ProjectOwner) REFERENCES [User](UserId)

)