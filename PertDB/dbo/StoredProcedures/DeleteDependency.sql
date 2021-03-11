CREATE PROCEDURE [dbo].[DeleteDependency]
	@taskId int
AS
BEGIN
	DELETE FROM [dbo].Dependency
	WHERE RootId = @taskId OR DependentId = @taskId
END
