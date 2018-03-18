CREATE TABLE [dbo].[Wallet]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [Dollars] MONEY NULL, 
    [Bitcoins] FLOAT NULL, 
    [Rorycoins] FLOAT NULL
)
