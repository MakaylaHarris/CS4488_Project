/*******************************************
*	Procedure: Create Project
*	Attempts to create a new project.
*	Result: True on success
*	Created 2/8/2021 by Tyler Kness-Miller
*/

CREATE PROCEDURE  [dbo].[CreateProject]
(
	@ProjectName nvarchar(50),
	@StartDate datetime,
	@EndDate datetime,
	@Description nvarchar(MAX),
	@Result BIT OUTPUT
)
AS
BEGIN
	SET @Result = 0;
	/* Check and make sure Project Name is unique. If not, do not insert into table. */
	IF (SELECT COUNT(*) FROM [Project] WHERE [Project].Name = @ProjectName) = 0
		INSERT INTO dbo.Project (Name, StartDate, EndDate, Description) VALUES (@ProjectName, @StartDate, @EndDate, @Description)
	ELSE
		SET @Result=1;
END
GO