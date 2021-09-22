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
update [expofair].[job2Tour] set TourName=(select TourName from [expofair].Tour where IdTour = @IdTour) where IdTourJob = @IdTourJob

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
	@IdTourJob int

	)
AS
BEGIN

-- Select Max Ranking
DECLARE @MaxRanking INT
select @MaxRanking =MAX(Ranking) from [expofair].[job2Tour] where IdTour = @IdTour

DECLARE @Ranking INT
select @Ranking = Ranking  from [expofair].[job2Tour] where IdTourJob = @IdTourJob and IdTour = @IdTour


IF @Ranking = @MaxRanking
	RETURN
ELSE
	update [expofair].[job2Tour] set Ranking = @Ranking where IdTour = @IdTour and Ranking = @Ranking + 1
	update [expofair].[job2Tour] set Ranking = @Ranking + 1 where IdTour = @IdTour and IdTourJob = @IdTourJob
RETURN
END
GO
--
--
--This Procedure Decreases a Job Ranking  in the Job2Tour-Table to a Tour
CREATE OR ALTER PROCEDURE [expofair].[CustDecreaseJobRanking] (
	@IdTour int,
	@IdTourJob int
	)
AS
BEGIN

-- Select Min Ranking
DECLARE @MinRanking INT
select @MinRanking = Min(Ranking) from [expofair].[job2Tour] where IdTour = @IdTour

DECLARE @Ranking INT
select @Ranking = Ranking  from [expofair].[job2Tour] where IdTourJob = @IdTourJob and IdTour = @IdTour

IF @Ranking < 2
	RETURN
ELSE
	update [expofair].[job2Tour] set Ranking = @Ranking where IdTour = @IdTour and Ranking = @Ranking - 1
	update [expofair].[job2Tour] set Ranking = @Ranking - 1 where IdTour = @IdTour and IdTourJob = @IdTourJob
RETURN
END
GO
--
-- Delete Job from Tour
CREATE OR ALTER PROCEDURE [expofair].[CustDelJobFromTour] (
	@IdTour int,
	@IdTourJob int
	)
AS
BEGIN
update [expofair].[job2Tour] set IdTour = 0, Ranking = 0 where IdTourJob = @IdTourJob and IdTour = @IdTour
DECLARE @Weight FLOAT
select @Weight = sum(Weight) from [expofair].[job2Tour] where IdTour = @IdTour
update [expofair].Tour set Weight = @Weight where IdTour = @IdTour
END
GO

--#######################################

CREATE OR ALTER PROCEDURE [expofair].[CustDeleteTour] (
	@IdTour int
	)
AS
BEGIN
   Update  [expofair].[job2Tour] set IdTour = 0, Ranking = 0, TourName = NULL where IdTour = @IdTour
   Delete from [expofair].[Tour] where IdTour = @IdTour
END
GO

--########################################

CREATE OR ALTER PROCEDURE [expofair].[CreateSBTour] (
	@SbDate VARCHAR(20)
	)
AS
BEGIN
	
	DECLARE @IdTour int
	
	insert into [expofair].[Tour] (TourDate, TourName, IsSBTour) values ( convert(date, @SbDate),'Tour SB', 1 )

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

	   update [expofair].[job2Tour] set Ranking = @Ranking,IdTour = @IdTour, TourName = 'Tour SB' where IdTourJob = @IdTourJob

	   FETCH NEXT FROM Cur1 into @IdTourJob
	   END
     
END
GO
CREATE OR ALTER PROCEDURE [expofair].[CustCloneJob] (
	@IdTourJob int
	)
AS
BEGIN

INSERT INTO [expofair].[job2Tour] (
IdJob,
IdJobState,
IdProject,
IdAddress,
Number,
Caption,
Comment,
JobDate,
Service,
Status,
Address,
In_Out,
Ranking,
JobDateReturn,
DeliveryTimeStart,
DeliveryTimeEnd,
PickupTimeStart,
PickupTimeEnd,
Contact,
ContactPhone,
JobStartTime,
Time,
ReadyTime,
SplitCounter,
JobType,
Stock
)
SELECT 
	IdJob,
	IdJobState,
	IdProject,
	IdAddress,
	(select Number from [expofair].[job2Tour] where IdTourJob = @IdTourJob) + ' (Dup)',
	(select Caption from [expofair].[job2Tour] where IdTourJob = @IdTourJob) + ' (Dup)',
	Comment,
	JobDate,
	Service,
	Status,
	Address,
	In_Out,
	Ranking,
	JobDateReturn,
	DeliveryTimeStart,
	DeliveryTimeEnd,
	PickupTimeStart,
	PickupTimeEnd,
	Contact,
	ContactPhone,
	JobStartTime,
	Time,
	ReadyTime,
	(select SplitCounter from [expofair].[job2Tour] where IdTourJob = @IdTourJob) + 1,
	JobType,
	Stock
	FROM 
		[expofair].[job2Tour] where IdTourJob = @IdTourJob
END
GO