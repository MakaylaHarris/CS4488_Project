/************************************************************
* Attempts to login using the UserId and Password
*	Returns bit (true if successful login)
* Created 2/4/2021 by Robert Nelson
************************************************************/
CREATE FUNCTION [dbo].[TryLogin]
(
	@UserId Varchar(50),
	@Password Varchar(50)
)
RETURNS Bit
AS
BEGIN
	IF EXISTS (SELECT 1 from [dbo].[User] where ([User].UserName=@UserId OR [User].Email=@UserId) AND [User].Password = @Password)
		RETURN 1;
	RETURN 0;
END
GO
