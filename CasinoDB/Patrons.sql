CREATE TABLE [dbo].[Patrons]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [Firstname] NVARCHAR(100) NULL, 
    [Lastname] NVARCHAR(100) NULL, 
    [Sex] CHAR(1) NOT NULL, 
    [Verified] BIT NOT NULL, 
    [Birthday] DATE NOT NULL
)
