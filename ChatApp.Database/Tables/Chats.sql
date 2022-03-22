﻿CREATE TABLE [dbo].[Chats]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY (1,1), 
    [MessageFrom] INT NOT NULL, 
    [MessageTo] INT NOT NULL, 
    [Type] NVARCHAR(50) NOT NULL, 
    [Content] TEXT NOT NULL, 
    [ReplyTo] INT NOT NULL DEFAULT 0 , 
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(), 
    [UpdatedAt] DATETIME2 NULL DEFAULT GETDATE(), 
    [DeletedAt] DATETIME2 NULL,
    [IsSeen] INT NULL DEFAULT 0, 
    CONSTRAINT [FK_Chats_MessageFrom_To_Profiles] FOREIGN KEY (MessageFrom) REFERENCES dbo.Profiles(Id),
    CONSTRAINT [FK_Chats_MessgeTo_To_Profiles] FOREIGN KEY (MessageTo) REFERENCES dbo.Profiles(Id),
)
