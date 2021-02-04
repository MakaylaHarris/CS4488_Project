CREATE PROCEDURE [dbo].[spReadTable]
	@tableName nvarchar(30)
AS
BEGIN
	declare @query nvarchar(500)
	set @query = 'SELECT * FROM dbo.' + @tableName + '_View'
	EXEC(@query)
END
