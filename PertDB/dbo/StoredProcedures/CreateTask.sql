/*******************************************
*	Procedure: Create Task
*	Attempts to create a new task on an existing project, fails if there is no current active project with that project ID 
	or if the task name given is not unique. 
*	Result: True on success
*	Created 2/8/2021 by Kaden Marchetti
*/
CREATE PROCEDURE [dbo].[CreateTask]
(
	@TaskName nvarchar(50),
	@Description nvarchar(MAX),
	@MinEstDuration float,
	@MaxEstDuration float,
	@StartDate datetime,
	@EndDate datetime,
	@ModifiedDate datetime,
	@StatusID int,
	@UserId int,
	@ProjectID int,
	@Result BIT OUTPUT
	
)
AS
BEGIN

 SET @Result = 0;

 /* Check if there is an active project */
  IF (SELECT COUNT(*) FROM [Project] WHERE [Project].ProjectId = @ProjectID) = 0
	/* Project does not exist, return with result=0 */
	RETURN;

 /* Check if task has a unique name (Task already exists)*/
 IF (SELECT COUNT(*) FROM [Task] WHERE [Task].Name = @taskName) > 0
	/* Notify User and end procedure with result=0 */
	RETURN;

 /* Task is created referencing the current project (have project ID) */
 INSERT INTO dbo.[Task] (TaskName, Description, MinEstDuration, MaxEstDuration, StartDate, EndDate, ModifiedDate, StatusID,
 UserId, ProjectID) Values(@TaskName, @Description, @MinEstDuration, @MaxEstDuration, @StartDate, @EndDate, @ModifiedDate,
 @StatusID, @UserId, @ProjectID);


 /* Add Task to task list */

 SET @Result=1;

END
GO