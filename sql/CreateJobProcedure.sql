use [easyjob]
GO
-- DROP PROCEDURE [expofair].[CustJob_GetByDate]
-- GO
--This Procedure copies all Jobs from the easyjob-JobTable in a specific Timframe to the expofair.job2Tour Table 
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [expofair].[CustCopyJobsByDate] (
	@DateStart VARCHAR(20),
	@DateEnd VARCHAR(20)
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
Ranking
)
SELECT 
       t1.IdJob,
	   t1.IdJobState,
       t1.IdProject,
       t1.IdAddress_Delivery IdAdress,
       t1.Number,
       t1.Caption,
       t1.Comment,
       t1.DayTimeOut,
	   t3.Caption Service,
       t4.Caption Status,
	   t2.Company +  ';' + t2.Zip + ';' + t2.City +  ';' + t2.Street,
	   'Lieferung',
	   0
	FROM 
		[easyjob].[dbo].[Job] t1, [easyjob].[dbo].[Address] t2, [easyjob].[dbo].[JobService] t3, [easyjob].[dbo].[JobState] t4
 where  t1.IdJobState = 1 and cast(t1.DayTimeOut as Date) >= convert(date, @DateStart) and cast(t1.DayTimeOut as Date) <= convert(date , @DateEnd) and t1.IdAddress_Delivery = t2.IdAddress and t1.IdJobService = T3.IdJobService and t1.IdJobState = T4.IdJobState


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
Ranking
)
SELECT 
       t1.IdJob,
	   t1.IdJobState,
       t1.IdProject,
       t1.IdAddress_Delivery IdAdress,
       t1.Number,
       t1.Caption,
       t1.Comment,
       t1.DayTimeIn,
	   t3.Caption Service,
       t4.Caption Status,
	   t2.Company +  ';' + t2.Zip + ';' + t2.City +  ';' + t2.Street,
	   'Abholung',
	   0
	FROM 
		[easyjob].[dbo].[Job] t1, [easyjob].[dbo].[Address] t2, [easyjob].[dbo].[JobService] t3, [easyjob].[dbo].[JobState] t4
 where  t1.IdJobState = 1 and cast(t1.DayTimeIn as Date) >= convert(date, @DateStart) and cast(t1.DayTimeIn as Date) <= convert(date , @DateEnd) and t1.IdAddress_Delivery = t2.IdAddress and t1.IdJobService = T3.IdJobService and t1.IdJobState = T4.IdJobState 
 

 -- The Stock of the Job is concateneded into a string and add to the job

    DECLARE @STOCK VARCHAR(MAX)
	DECLARE @ID INT
	DECLARE @temp VARCHAR(100)

	DECLARE Cur1 CURSOR READ_ONLY FOR SELECT t.IdJob from [expofair].[job2Tour] t where t.Stock is NULL 

	Open Cur1 
    
	FETCH NEXT FROM Cur1 into @ID

	   WHILE  @@fetch_status = 0
       BEGIN

			  SET @STOCK = ''

			  DECLARE Cur2 CURSOR READ_ONLY FOR SELECT
			   convert(varchar(10),t1.[Factor]) + ' ' + t2.[Caption] Stock
			  FROM [easyjob].[dbo].[StockType2Job] t1, [easyjob].[dbo].[StockType] t2 where t1.Idjob = @ID and t1.IdStockType = t2.idstockType
	  
			  OPEN Cur2 

			  FETCH NEXT FROM Cur2 into @temp

			   WHILE  @@fetch_status = 0
			   BEGIN
      
				  SET @Stock = @Stock  +  @temp + CHAR(13)

				 FETCH NEXT FROM Cur2 into @temp
			  END
			Close cur2
			DEALLOCATE cur2 

			print @ID

			update [expofair].[job2Tour] set Stock = @STOCK where IdJob = @ID

	        FETCH NEXT FROM Cur1 into @ID
     END

	Close cur1
	DEALLOCATE cur1
END
GO

