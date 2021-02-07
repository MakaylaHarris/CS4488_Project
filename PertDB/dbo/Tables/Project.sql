CREATE TABLE [dbo].[Project]
(
	[ProjectId] INT  NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(50) NULL, 
    [StartDate] DATETIME NULL, 
    [WorkingHours] FLOAT NULL, 
    [ProjectOwner] INT NULL,
	CONSTRAINT FK_ProjectUser FOREIGN KEY (ProjectOwner) REFERENCES [User](UserId)

)