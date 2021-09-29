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
DeliveryType
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
	   	   ISNULL(t2.Company,'') +  ';' + ISNULL(t2.Surname,'')  + ';' +  ISNULL(t2.Zip,'') + ';' + ISNULL(t2.City,'') +  ';' + ISNULL(t2.Street,'') Adresse,
	   'OUT',
	   0,
	   t1.DayTimeIn,
	   t5.DeliveryTimeStart,
       t5.DeliveryTimeEnd,
       t5.PickupTimeStart,
       t5.PickupTimeEnd,
       t5.Contact,
       t5.ContactPhone,
	   t5.DeliveryTimeStart,
	   convert(varchar(5), t5.DeliveryTimeStart,108) + '-' + convert(varchar(5), t5.DeliveryTimeEnd,108),
	   convert(varchar(5), t5.SetupEnd,108),
	   1,
	   (select x2.Caption FROM [easyjob].[dbo].[Project] x1, [easyjob].[dbo].[ProjectType] x2 where t1.IdProject = x1.IdProject and x2.IdProjectType = x1.IdProjectType),
	   'Lieferung'
 	FROM 
		[easyjob].[dbo].[Job] t1, [easyjob].[dbo].[Address] t2, [easyjob].[dbo].[JobService] t3, [easyjob].[dbo].[JobState] t4, [easyjob].[dbo].[CusProjectInfo] t5
 where  t1.IdJobState in (1,5) and cast(t1.DayTimeOut as Date) >= convert(date, @DateStart) and cast(t1.DayTimeOut as Date) <= convert(date , @DateEnd) and t1.IdAddress_Delivery = t2.IdAddress and t1.IdJobService = T3.IdJobService and t1.IdJobState = T4.IdJobState and t5.IdJob = t1.IdJob

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
DeliveryType
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
	   ISNULL(t2.Company,'') +  ';' + ISNULL(t2.Surname,'')  + ';' +  ISNULL(t2.Zip,'') + ';' + ISNULL(t2.City,'') +  ';' + ISNULL(t2.Street,'') Adresse,
	   'IN',
	   0,
	   t5.DeliveryTimeStart,
       t5.DeliveryTimeEnd,
       t5.PickupTimeStart,
       t5.PickupTimeEnd,
       t5.Contact,
       t5.ContactPhone,
	   t5.PickupTimeStart,
	   convert(varchar(5), t5.PickupTimeStart,108) + '-' + convert(varchar(5), t5.PickupTimeEnd,108),
	   convert(varchar(5), t5.BreakdownEnd,108),
	   1,
	   (select x2.Caption FROM [easyjob].[dbo].[Project] x1, [easyjob].[dbo].[ProjectType] x2 where t1.IdProject = x1.IdProject and x2.IdProjectType = x1.IdProjectType),
	   'Abholung'
	FROM 
		[easyjob].[dbo].[Job] t1, [easyjob].[dbo].[Address] t2, [easyjob].[dbo].[JobService] t3, [easyjob].[dbo].[JobState] t4 , [easyjob].[dbo].[CusProjectInfo] t5
 where  t1.IdJobState in  (1,5) and cast(t1.DayTimeIn as Date) >= convert(date, @DateStart) and cast(t1.DayTimeIn as Date) <= convert(date , @DateEnd) and t1.IdAddress_Delivery = t2.IdAddress and t1.IdJobService = T3.IdJobService and t1.IdJobState = T4.IdJobState  and t5.IdJob = t1.IdJob
 

       update  [expofair].[job2Tour] set Time='' where Time='00:00-00:00'

       update  [expofair].[job2Tour] set Time = replace( Time, '-00:00','') where Time like '%00:00'  


	   update  [expofair].[job2Tour] set ReadyTime='' where ReadyTime='00:00'

	   update  [expofair].[job2Tour] set Status='Zulieferung Bestätigt' where IdJobState=5


	   update  [expofair].[job2Tour] set DeliveryType='Rückgabe' where DeliveryType='Abholung' and Service = 'Selbstabholer'

	   update  [expofair].[job2Tour] set DeliveryType='Ausgabe' where DeliveryType='Lieferung' and Service = 'Selbstabholer'




 -- The Stock of the Job is concateneded into a string and add to the job

-- SELECT factor, CASE WHEN caption IS NULL THEN (SELECT caption FROM stocktype WHERE idstocktype = stocktype2job.idstocktype) ELSE caption END AS Artikelbeschreibung, 
--                                       (SELECT weight FROM stocktype WHERE idstocktype = stocktype2job.idstocktype) AS Gewicht 
--                                       FROM stocktype2job 
--                                       WHERE 
--                                       idjob = 102958 AND idstocktype2jobtype <> 4 
--                                       AND (idstocktype NOT in (SELECT idstocktype FROM stocktype WHERE customnumber LIKE 'Text%') OR idstocktype IS NULL)
--                                       ORDER BY sortorder


    DECLARE @STOCK VARCHAR(MAX)
	DECLARE @ID INT
	DECLARE @TOURJOB INT
	DECLARE @temp VARCHAR(100)

	DECLARE Cur1 CURSOR READ_ONLY FOR SELECT t.IdTourJob, t.IdJob from [expofair].[job2Tour] t where t.Stock is NULL 

	Open Cur1 
    
	FETCH NEXT FROM Cur1 into @TOURJOB, @ID

	   WHILE  @@fetch_status = 0
       BEGIN

			  SET @STOCK = ''

			  DECLARE Cur2 CURSOR READ_ONLY FOR SELECT
			   convert(varchar(10),t1.[Factor]) + ' ' + t2.[Caption] Stock
			  FROM [easyjob].[dbo].[StockType2Job] t1, [easyjob].[dbo].[StockType] t2 where t1.Idjob = @ID and t1.IdStockType = t2.idstockType
			  AND t1.IdStockType2JobType <> 4 
			  AND t1.IdStockType <>  4523
              AND (t1.IdStockType NOT in (SELECT idstocktype FROM [easyjob].[dbo].[StockType] WHERE customnumber LIKE 'Text%') OR t1.IdStockType IS NULL)
              ORDER BY t1.sortorder
	  
			  OPEN Cur2 

			  FETCH NEXT FROM Cur2 into @temp

			   WHILE  @@fetch_status = 0
			   BEGIN
      
				  SET @Stock = @Stock  +  @temp + CHAR(13)

				 FETCH NEXT FROM Cur2 into @temp
			  END
			Close cur2
			DEALLOCATE cur2 

			update [expofair].[job2Tour] set Stock = @STOCK where IdJob = @ID

			update [easyjob].[expofair].[job2Tour] set Comment = (select substring(Comment,1,PATINDEX('%[_][_]%', Comment)-1) from [easyjob].[expofair].[job2Tour] where IdTourJob=@TOURJOB ) where IdTourJob=@TOURJOB and Comment like '%[_][_]%'
			update [easyjob].[expofair].[job2Tour] set Comment = (select substring(Comment,1,PATINDEX('%Storno%', Comment)-1) from [easyjob].[expofair].[job2Tour] where IdTourJob=@TOURJOB ) where IdTourJob=@TOURJOB and Comment like '%Storno%'
			update [easyjob].[expofair].[job2Tour] set Comment = (select substring(Comment,1,PATINDEX('%Cancelation%', Comment)-1) from [easyjob].[expofair].[job2Tour] where IdTourJob=@TOURJOB ) where IdTourJob=@TOURJOB and Comment like '%Cancelation%'



	        FETCH NEXT FROM Cur1 into @TOURJOB, @ID
     END

	Close cur1
	DEALLOCATE cur1
END
GO

