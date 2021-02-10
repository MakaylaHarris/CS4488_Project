/*******************************************
*	Procedure: Delete Project
*	Deletes a given project based on the ID provided. 
*	Created 2/10/2021 by Tyler Kness-Miller
*/

CREATE PROCEDURE [dbo].ProjectDelete
(
	@ProjectID int
)
AS
BEGIN
	DELETE FROM [dbo].Project WHERE ProjectId = @ProjectID;
END
GO