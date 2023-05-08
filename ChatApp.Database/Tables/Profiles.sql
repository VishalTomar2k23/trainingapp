﻿CREATE TABLE [dbo].[Profiles]
(
    [Id] INT PRIMARY KEY IDENTITY (1, 1) NOT NULL, 
    [FirstName] NVARCHAR(1000) NOT NULL, 
    [LastName] NVARCHAR(1000) NULL, 
    [UserName] NVARCHAR(1000) NOT NULL, 
    [Email ] NVARCHAR(1000) NOT NULL, 
    [Password] NVARCHAR(MAX) NOT NULL, 
    [ProfileType] INT NOT NULL,
    [CreatedAt] DATETIME2 NULL, 
    [CreatedBy] INT NULL, 
    [LastUpdatedAt] DATETIME2 NULL, 
    [LastUpdatedBy] INT NULL, 
    [ImagePath] NVARCHAR(1000) NULL, 
    [Designation] INT NOT NULL ,
    [Status] INT NOT NULL,
    CONSTRAINT [FK_Profiles_Designation_To_Designation] FOREIGN KEY (Designation) REFERENCES dbo.Designation(Id), 
    CONSTRAINT [FK_Profiles_Status_To_UserStatus] FOREIGN KEY (Status) REFERENCES dbo.UserStatus(Id), 
)