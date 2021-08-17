use [easyjob]
GO
-- DROP PROCEDURE [expofair].[CustAddJobToTour]
-- GO
--This Procedure Adds a Job in the Job2Tour-Table to a Tour
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [expofair].[CustAddJobToTour] (
	@IdTour int,
	@IdJob int
	)
AS
BEGIN
update [expofair].[job2Tour] set IdTour = @IdTour where IdJob = @IdJob

-- Set inital Ranking
DECLARE @Ranking INT
select @Ranking =MAX(Ranking) from [expofair].[job2Tour] where IdTour = @IdTour
set @Ranking = @Ranking + 1
update [expofair].[job2Tour] set Ranking = @Ranking  where IdJob = @IdJob and IdTour = @IdTour
END
-- DROP PROCEDURE [expofair].[CustAddJobToTour]
-- GO
--This Procedure Adds a Job in the Job2Tour-Table to a Tour
SET QUOTED_IDENTIFIER ON
GO
--
--
--This Procedure Increases a Job Ranking  in the Job2Tour-Table to a Tour
CREATE OR ALTER PROCEDURE [expofair].[CustIncreaseJobRanking] (
	@IdTour int,
	@IdJob int
	)
AS
BEGIN

-- Select Max Ranking
DECLARE @MaxRanking INT
select @MaxRanking =MAX(Ranking) from [expofair].[job2Tour] where IdTour = @IdTour

DECLARE @Ranking INT
select @Ranking = Ranking  from [expofair].[job2Tour] where IdJob = @IdJob and IdTour = @IdTour


IF @Ranking = @MaxRanking
	RETURN
ELSE
	update [expofair].[job2Tour] set Ranking = @Ranking where IdTour = @IdTour and Ranking = @Ranking + 1
	update [expofair].[job2Tour] set Ranking = @Ranking + 1 where IdTour = @IdTour and IdJob = @IdJob
RETURN
END
GO
--
--
--This Procedure Decreases a Job Ranking  in the Job2Tour-Table to a Tour
CREATE OR ALTER PROCEDURE [expofair].[CustDecreaseJobRanking] (
	@IdTour int,
	@IdJob int
	)
AS
BEGIN

-- Select Min Ranking
DECLARE @MinRanking INT
select @MinRanking = Min(Ranking) from [expofair].[job2Tour] where IdTour = @IdTour

DECLARE @Ranking INT
select @Ranking = Ranking  from [expofair].[job2Tour] where IdJob = @IdJob and IdTour = @IdTour

IF @Ranking < 2
	RETURN
ELSE
	update [expofair].[job2Tour] set Ranking = @Ranking where IdTour = @IdTour and Ranking = @Ranking - 1
	update [expofair].[job2Tour] set Ranking = @Ranking - 1 where IdTour = @IdTour and IdJob = @IdJob
RETURN
END
GO
--
-- Delete Job from Tour
CREATE OR ALTER PROCEDURE [expofair].[CustDelJobFromTour] (
	@IdTour int,
	@IdJob int
	)
AS
BEGIN
update [expofair].[job2Tour] set IdTour = 0, Ranking = 0 where IdJob = @IdJob
END
GO

CREATE OR ALTER PROCEDURE [expofair].[CustDeleteTour] (
	@IdTour int
	)
AS
BEGIN
   Update  [expofair].[job2Tour] set IdTour = 0, Ranking = 0 where IdTour = @IdTour
   Delete from [expofair].[Tour] where IdTour = @IdTour
END
GO