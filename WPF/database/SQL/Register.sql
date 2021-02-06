
/*******************************************
*	Procedure: Register
*		Attempts to register a user, succeeds if the username is not in use 
*		or if the username has no password on it.
*	@result: True on success
*	Created 2/6/2021 by Robert Nelson
*/
CREATE PROCEDURE [dbo].[Register]
(
	@username varchar(50),
	@password varchar(50),
	@email varchar(50),
	@fullname varchar(50),
	@result TINYINT OUTPUT
)
AS
BEGIN
	DECLARE @found varchar(50);
	SET @result=0;
	/* Does the user exist in table? */
	IF (SELECT COUNT(*) FROM [User] WHERE [User].UserName=@username) > 0
		BEGIN
			/* Is the user registered? */
			IF (SELECT COUNT(*) FROM [User] WHERE [User].UserName=@username AND [User].Password!='' AND [User].Password IS NOT NULL) > 0
				RETURN;
			UPDATE [User] SET Password=@password, Email=@email, Name=@fullname WHERE [User].UserName=@username;
		END
	ELSE
		INSERT INTO dbo.[User] (UserName, Password, Email, Name) Values(@username, @password, @email, @fullname);
	SET @result=1;
END
