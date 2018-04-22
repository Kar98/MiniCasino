
CREATE PROCEDURE [dbo].[AddStaff]
	@firstname nvarchar(100),
	@lastname nvarchar(100),
	@sex char(1),
	@birthday datetime,
	@deptid int

AS 

SET NOCOUNT ON;

INSERT INTO Staff(Firstname, Lastname, Sex, Birthday, DeptId)
VALUES (@firstname,@lastname,@sex, @birthday, @deptid);

RETURN scope_identity()

GO