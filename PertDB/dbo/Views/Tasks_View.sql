CREATE VIEW [Tasks_View]
	AS SELECT [t].[TaskId], [t].[Name] AS TaskName, [t].[Description], [t].[MinEstDuration], [t].[MaxEstDuration], 
	[t].[MostLikelyEstDuration], [t].[StartDate] AS TaskStartDate, [t].[EndDate], [t].[ModifiedDate], [t].[StatusId], 
	[t].[UserId], [t].[ProjectId] AS Id, [p].[ProjectId], [p].[Name], [p].[StartDate] AS ProjectStartDate, 
	[p].[WorkingHours], [p].[ProjectOwner] 
	FROM dbo.Task t
	left join dbo.Project p ON t.ProjectId = p.ProjectId
