CREATE PROCEDURE [dbo].[CreateDependency]
(
	@rootId int,
	@dependentId int
)
AS
BEGIN
INSERT INTO [dbo].Dependency (RootId, DependentId) VALUES(@rootId, @dependentId)
END
