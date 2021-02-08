/********************************************************
*	DeleteUser Trigger
* Deletes the User Project and User Task associations.
* Created 2/7/2021 by Robert Nelson
*********************************************************/
CREATE TRIGGER dbo.[DeleteUser]
ON dbo.[User]
INSTEAD OF DELETE
AS
BEGIN
	-- User Projects
	DELETE dbo.UserProject FROM deleted D INNER JOIN [UserProject] T ON T.UserName = D.UserName;
	-- User Tasks
	DELETE dbo.UserTask FROM deleted D INNER JOIN [UserTask] T ON T.UserName = D.UserName;

		-- Then perform the actual delete
	DELETE dbo.[User] FROM deleted D INNER JOIN dbo.[User] T ON T.UserName = D.UserName;
END
