/************************************************************
* Attempts to login using the UserId and Password
*	Returns bit (true if successful login)
* Created 2/4/2021 by Robert Nelson
************************************************************/
CREATE Procedure [dbo].[TryLogin]
(
	@UserId Varchar(50),
	@Password Varchar(50),
	@Success bit out
)
AS
IF EXISTS (SELECT 1 from [dbo].[User] where ([User].UserName=@UserId OR [User].Email=@UserId) 
	AND ([User].Password = @Password OR [User].Password = ''))
	SET @Success = 1
ELSE
	SET @Success = 0;
RETURN
GO
