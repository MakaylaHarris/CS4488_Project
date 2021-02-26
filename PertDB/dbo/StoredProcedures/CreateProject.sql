/*******************************************
*	Procedure: Create Project
*	Attempts to create a new project.
*	Result: True on success
*	Created 2/8/2021 by Tyler Kness-Miller
*	Edited 2/19/2021 by Robert Nelson to include Creator, CreationDate, and output id
*/

CREATE PROCEDURE  [dbo].[CreateProject]
(
	@ProjectName nvarchar(50),
	@StartDate datetime,
	@EndDate datetime,
	@Description nvarchar(MAX),
	@Creator varchar(50),				/* Added 2/19/2021 by Robert Nelson */
	@CreationDate datetime OUTPUT,		/* Added 2/19/2021 by Robert Nelson */
	@Result BIT OUTPUT,
	@ResultId int OUTPUT				/* Added 2/19/2021 by Robert Nelson */
)
AS
BEGIN
	SET @Result = 0;
	SET @ResultId = -1;
	SET @CreationDate = GETDATE();
	/* Check and make sure Project Name is unique. If not, do not insert into table. */
	IF (SELECT COUNT(*) FROM [Project] WHERE [Project].Name = @ProjectName) = 0
		BEGIN
			INSERT INTO dbo.Project (Name, StartDate, EndDate, Description, Creator, CreationDate) 
				VALUES (@ProjectName, @StartDate, @EndDate, @Description, @Creator, @CreationDate);
			SET @ResultId = SCOPE_IDENTITY();
			SET @Result=1;

		END
END
GO