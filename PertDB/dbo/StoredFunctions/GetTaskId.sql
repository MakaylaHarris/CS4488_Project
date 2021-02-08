Create Function [dbo].GetTaskId(@name Varchar(50)) 
RETURNS INT AS
BEGIN
	DECLARE @ret int; 
	SET @ret = (SELECT [TaskId] FROM Task WHERE [Name] = @name);
	return @ret;
END
