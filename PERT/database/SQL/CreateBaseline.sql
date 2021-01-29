CREATE TABLE [dbo].[Project] (
    [ProjectId]    INT             IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (256)   NOT NULL,
    [StartDate]    DATETIME        NULL,
    [EndDate] DATETIME      NULL,
    [Description]  NVARCHAR (2000) NULL,
    PRIMARY KEY CLUSTERED ([ProjectId] ASC)
);

CREATE TABLE [dbo].[Task] (
    [TaskId]                INT            IDENTITY (1, 1) NOT NULL,
    [Name]                  NVARCHAR (256)  NOT NULL,
    [Description]           NVARCHAR (MAX) NULL,
    [MinEstDuration]        FLOAT (53)     NULL,
    [MaxEstDuration]        FLOAT (53)     NULL,
    [MostLikelyEstDuration] FLOAT (53)     NULL,
    [StartDate]             DATETIME       NULL,
    [EndDate]               DATETIME       NULL,
    [ProjectId]             INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([TaskId] ASC),
    CONSTRAINT [FK_TaskProject] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Project] ([ProjectId])
);

CREATE TABLE [dbo].[User] (
    [UserId]       INT           IDENTITY (1, 1) NOT NULL,
    [Name]    NVARCHAR (256) NOT NULL,
    [Email] NVARCHAR (256) NULL,
    [Password] NVARCHAR(50) NULL, 
    PRIMARY KEY CLUSTERED ([UserId] ASC)
);

