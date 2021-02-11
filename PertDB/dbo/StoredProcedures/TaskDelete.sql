/*******************************************
*	Procedure: Delete Task
*	Deletes a given task based on the ID provided.
*	Created 2/10/2021 by Tyler Kness-Miller
*/

CREATE PROCEDURE [dbo].TaskDelete
(
	@TaskID int
)
AS
BEGIN
	DELETE FROM [dbo].Task WHERE TaskId = @TaskID;
END
GO