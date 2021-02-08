/********************************************************
*	DeleteTask Trigger
* Deletes associated Dependency records and user task records
* Created 2/7/2021 by Robert Nelson
*********************************************************/
CREATE TRIGGER [dbo].[DeleteTask]
ON [Task]
INSTEAD OF DELETE
AS
BEGIN
	-- Dependencies
	DELETE dbo.Dependency FROM deleted D INNER JOIN dbo.Dependency T ON T.DependentId = D.TaskId OR T.RootId = D.TaskId;
	-- User Tasks
	DELETE dbo.UserTask FROM deleted D INNER JOIN dbo.UserTask T ON T.TaskId = D.TaskId;
	
	-- Then perform the actual delete
	DELETE dbo.Task FROM deleted D INNER JOIN dbo.Task T ON T.TaskId = D.TaskId;
END
