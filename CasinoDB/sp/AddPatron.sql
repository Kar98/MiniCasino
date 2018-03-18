
CREATE PROCEDURE [dbo].[AddPatron]
	@firstname nvarchar(100),
	@lastname nvarchar(100),
	@sex char(1),
	@verified bit,
	@birthday datetime

AS 

DECLARE @tmptable table(Id int, dollars money, Bitcoins float, Rorycoins float);
DECLARE @tmptable2 table(Firstname nvarchar(100), Lastname nvarchar(100), Sex char(1), Verified bit, Birthday datetime, WalletId int)

SET NOCOUNT ON;

INSERT INTO Wallet(Dollars,Bitcoins,Rorycoins)
OUTPUT INSERTED.Id, INSERTED.dollars, INSERTED.Bitcoins, INSERTED.Rorycoins INTO @tmptable
VALUES(0,null,null)

INSERT INTO Patrons (Firstname, Lastname, Sex, Verified, Birthday, WalletId)
OUTPUT INSERTED.Firstname, INSERTED.Lastname, INSERTED.Sex, INSERTED.Verified, INSERTED.Birthday, INSERTED.WalletId INTO @tmptable2
VALUES (@firstname,@lastname,@sex,@verified, @birthday,@tmptable.Id);

RETURN @tmptable2.Id

GO