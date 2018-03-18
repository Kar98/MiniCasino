
CREATE PROCEDURE [dbo].[AddPatron]
	@firstname nvarchar(100),
	@lastname nvarchar(100),
	@sex char(1),
	@verified bit,
	@birthday datetime

AS 
drop table #tmptable
drop table #tmptable2
Create TABLE #tmptable (Id int);
Create TABLE #tmptable2 (Firstname nvarchar(100), Lastname nvarchar(100), Sex char(1), Verified bit, Birthday datetime, WalletId int)

SET NOCOUNT ON;

INSERT INTO Wallet(Dollars,Bitcoins,Rorycoins)
OUTPUT INSERTED.Id INTO #tmptable
VALUES(0,null,null)

declare @walletid int;
declare @returnid int;
Select @walletid = Id FROM #tmptable;

INSERT INTO Patrons (Firstname, Lastname, Sex, Verified, Birthday, WalletId)
OUTPUT INSERTED.Id, INSERTED.Firstname, INSERTED.Lastname, INSERTED.Sex, INSERTED.Verified, INSERTED.Birthday, INSERTED.WalletId INTO #tmptable2
VALUES (@firstname,@lastname,@sex,@verified, @birthday, @walletid);

Select @returnid = Id FROM #tmptable2;

drop table #tmptable
drop table #tmptable2

RETURN @returnid

GO