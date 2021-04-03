CREATE FUNCTION [dbo].[GetNextChildRow]
(
	@parentId int
)
RETURNS INT
AS
BEGIN
	DECLARE @NextRow int;
	SELECT @NextRow = MAX([ProjectRow]) FROM [Task] WHERE [Task].ProjectId = @parentId;
	IF @NextRow IS NULL SET @NextRow = 0 ELSE SET @NextRow = @NextRow + 1;
	RETURN @NextRow;
END
