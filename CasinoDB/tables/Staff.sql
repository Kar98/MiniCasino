CREATE TABLE [dbo].[Staff]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
	[Firstname] NVARCHAR(100) NULL, 
    [Lastname] NVARCHAR(100) NULL, 
    [Sex] CHAR(1) NOT NULL, 
    [Birthday] DATE NOT NULL, 
    [DeptId] INT NULL, 
    CONSTRAINT [fk_dept] FOREIGN KEY (DeptId) REFERENCES Department(Id)
)
