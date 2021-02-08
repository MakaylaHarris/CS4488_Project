/*****************************************
*	DeleteProject trigger
*	Created 2/7/2021 by Robert Nelson
******************************************/
CREATE TRIGGER dbo.[DeleteProject]
ON dbo.Project
INSTEAD OF DELETE
AS
BEGIN
	-- Delete User Project refs
	DELETE dbo.UserProject FROM deleted D INNER JOIN dbo.UserProject T ON T.ProjectId = D.ProjectId;
	-- Delete Tasks
	DELETE dbo.Task FROM deleted D INNER JOIN dbo.Task T ON t.ProjectId = D.ProjectId;
	-- Then perform the actual delete
	DELETE dbo.Project FROM deleted D INNER JOIN dbo.Project T ON T.ProjectId = D.ProjectId;
END
