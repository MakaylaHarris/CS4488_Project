USE [C:\USERS\ROBER\SOURCE\REPOS\PERT\WPF\DATABASE\PERT.MDF]
GO
/****** Object:  UserDefinedFunction [dbo].[TryLogin]    Script Date: 2/6/2021 8:00:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
