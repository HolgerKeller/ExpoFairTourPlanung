use [easyjob]
GO
-- DROP PROCEDURE [expofair].[CustAddJobToTour]
-- GO
--This Procedure Adds a Job in the Job2Tour-Table to a Tour
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [expofair].[CustAddJobToTour] (
	@IdTour int,
	@IdTourJob int
	)
AS
BEGIN
update [expofair].[job2Tour] set IdTour = @IdTour where IdTourJob = @IdTourJob

-- Set inital Ranking
DECLARE @Ranking INT
select @Ranking =MAX(Ranking) from [expofair].[job2Tour] where IdTour = @IdTour
set @Ranking = @Ranking + 1
update [expofair].[job2Tour] set Ranking = @Ranking  where IdTourJob = @IdTourJob and IdTourJob = @IdTourJob

DECLARE @Weight FLOAT
select @Weight = sum(Weight) from [expofair].[job2Tour] where IdTour = @IdTour
update [expofair].Tour set Weight = @Weight where IdTour = @IdTour

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
update [expofair].[job2Tour] set IdTour = 0, Ranking = 0 where IdJob = @IdJob and IdTour = @IdTour
DECLARE @Weight FLOAT
select @Weight = sum(Weight) from [expofair].[job2Tour] where IdTour = @IdTour
update [expofair].Tour set Weight = @Weight where IdTour = @IdTour
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

CREATE OR ALTER PROCEDURE [expofair].[CreateSBTour] (
	@SbDate VARCHAR(20)
	)
AS
BEGIN
	
	DECLARE @IdTour int
	
	insert into [expofair].[Tour] (TourDate, TourName, IsSBTour) values ( convert(date, @SbDate),'SB-Tour', 1 )

	SELECT @IdTour = IDENT_CURRENT ('[expofair].[Tour]')


    DECLARE @Ranking INT

	DECLARE @IdTourJob INT
    
	SET @Ranking = 0
	
	DECLARE Cur1 CURSOR READ_ONLY FOR SELECT t.IdTourJob from [expofair].[job2Tour] t where cast(t.JobDate as Date) = convert(date, @SbDate) and t.Service = 'Selbstabholer'  order by t.JobStartTime

	Open Cur1 
    
	FETCH NEXT FROM Cur1 into @IdTourJob

	   WHILE  @@fetch_status = 0
       BEGIN

	   set @Ranking = @Ranking + 1

	   update [expofair].[job2Tour] set Ranking = @Ranking,IdTour = @IdTour where IdTourJob = @IdTourJob

	   FETCH NEXT FROM Cur1 into @IdTourJob
	   END
     
END
GO