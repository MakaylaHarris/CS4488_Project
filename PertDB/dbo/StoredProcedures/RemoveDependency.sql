CREATE PROCEDURE [dbo].[RemoveDependency]
	@rootId int,
	@dependentId int
AS
BEGIN
	DELETE FROM [dbo].Dependency
	WHERE RootId = @rootId AND DependentId = @dependentId
END
