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
AddressTXT,
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
DeliveryType,
LastUpdate,
UserName,
UserEmail
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
	   IIF(t2.Company is NULL, '', t2.Company) + IIF(t2.Surname is NULL,'', CHAR(13)  + t2.Surname )  +   IIF(t2.Zip is NULL ,'', CHAR(13)  + t2.Zip) +  IIF(t2.City is NULL ,'', CHAR(13)  + t2.City)  + IIF(t2.Street is NULL ,'', CHAR(13)  + t2.Street)+ IIF(t5.Booth = '' ,'', CHAR(13) + 'Stand: '  + t5.Booth) + 
	   IIF(t5.Exhibitor = '' ,'', CHAR(13) + 'Aussteller: '  + t5.Exhibitor) + IIF(t2.Custom1 is NULL ,'', CHAR(13) + t2.Custom1) AdresseTXT,
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
	   'Lieferung',
	   CASE WHEN t1.UpdateTime IS NULL THEN t1.CreationTime ELSE t1.UpdateTime END,	
	   CASE WHEN t1.UpdateTime IS NULL THEN ((select FirstName + ' ' + SurName from [easyjob].[dbo].[User] where IdUser = t1.IdUserCreated)) ELSE
	   (select FirstName + ' ' + SurName from [easyjob].[dbo].[User] where IdUser = t1.IdUserUpdated ) END,
	   CASE WHEN t1.UpdateTime IS NULL THEN (select EMail from [dbo].[Address] s1,[dbo].[User] s2  where s1.IdAddress = s2.IdAddress and s2.IdUser = t1.IdUserCreated) ELSE
	   ( select EMail from [dbo].[Address] s1,[dbo].[User] s2  where s1.IdAddress = s2.IdAddress and s2.IdUser = t1.IdUserUpdated ) END
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
AddressTXT,
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
DeliveryType,
LastUpdate,
UserName,
UserEmail
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
	   IIF(t2.Company is NULL, '', t2.Company) + IIF(t2.Surname is NULL,'', CHAR(13)  + t2.Surname )  +   IIF(t2.Zip is NULL ,'', CHAR(13)  + t2.Zip) +  IIF(t2.City is NULL ,'', CHAR(13)  + t2.City)  + IIF(t2.Street is NULL ,'', CHAR(13)  + t2.Street)+ IIF(t5.Booth = '' ,'', CHAR(13) + 'Stand: '  + t5.Booth) + 
	   IIF(t5.Exhibitor = '' ,'', CHAR(13) + 'Aussteller: '  + t5.Exhibitor) + IIF(t2.Custom1 is NULL ,'', CHAR(13) + t2.Custom1) AdresseTXT,
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
	   'Abholung',
	   CASE WHEN t1.UpdateTime IS NULL THEN t1.CreationTime ELSE t1.UpdateTime END,
	   CASE WHEN t1.UpdateTime IS NULL THEN ((select FirstName + ' ' + SurName from [easyjob].[dbo].[User] where IdUser = t1.IdUserCreated)) ELSE
	   (select FirstName + ' ' + SurName from [easyjob].[dbo].[User] where IdUser = t1.IdUserUpdated ) END,
	   CASE WHEN t1.UpdateTime IS NULL THEN (select EMail from [dbo].[Address] s1,[dbo].[User] s2  where s1.IdAddress = s2.IdAddress and s2.IdUser = t1.IdUserCreated) ELSE
	   ( select EMail from [dbo].[Address] s1,[dbo].[User] s2  where s1.IdAddress = s2.IdAddress and s2.IdUser = t1.IdUserUpdated ) END
	FROM 
		[easyjob].[dbo].[Job] t1, [easyjob].[dbo].[Address] t2, [easyjob].[dbo].[JobService] t3, [easyjob].[dbo].[JobState] t4 , [easyjob].[dbo].[CusProjectInfo] t5
 where  t1.IdJobState in  (1,5) and cast(t1.DayTimeIn as Date) >= convert(date, @DateStart) and cast(t1.DayTimeIn as Date) <= convert(date , @DateEnd) and t1.IdAddress_Delivery = t2.IdAddress and t1.IdJobService = T3.IdJobService and t1.IdJobState = T4.IdJobState  and t5.IdJob = t1.IdJob
 

       update  [expofair].[job2Tour] set Time='' where Time='00:00-00:00'

       update  [expofair].[job2Tour] set Time = replace( Time, '-00:00','') where Time like '%00:00'  


	   update  [expofair].[job2Tour] set ReadyTime='' where ReadyTime='00:00' or ReadyTime='00:01'

	   update  [expofair].[job2Tour] set Status='Zulieferung Bestätigt' where IdJobState=5


	   update  [expofair].[job2Tour] set DeliveryType='Rückgabe' where DeliveryType='Abholung' and Service = 'Selbstabholer'

	   update  [expofair].[job2Tour] set DeliveryType='Ausgabe' where DeliveryType='Lieferung' and Service = 'Selbstabholer'

    DECLARE @STOCK VARCHAR(MAX)
	DECLARE @ID INT
	DECLARE @TOURJOB INT

	DECLARE Cur1 CURSOR READ_ONLY FOR SELECT t.IdTourJob, t.IdJob from [expofair].[job2Tour] t where t.Stock is NULL 

	Open Cur1 
    
	FETCH NEXT FROM Cur1 into @TOURJOB, @ID

	   WHILE  @@fetch_status = 0
       BEGIN

			update [easyjob].[expofair].[job2Tour] set Comment = (select substring(Comment,1,PATINDEX('%[_][_]%', Comment)-1) from [easyjob].[expofair].[job2Tour] where IdTourJob=@TOURJOB ) where IdTourJob=@TOURJOB and Comment like '%[_][_]%'
			update [easyjob].[expofair].[job2Tour] set Comment = (select substring(Comment,1,PATINDEX('%Storno%', Comment)-1) from [easyjob].[expofair].[job2Tour] where IdTourJob=@TOURJOB ) where IdTourJob=@TOURJOB and Comment like '%Storno%'
			update [easyjob].[expofair].[job2Tour] set Comment = (select substring(Comment,1,PATINDEX('%Cancelation%', Comment)-1) from [easyjob].[expofair].[job2Tour] where IdTourJob=@TOURJOB ) where IdTourJob=@TOURJOB and Comment like '%Cancelation%'

-----------------------------------------------------------------------------------------------------------
-- insert into Stock2Job

insert into [expofair].[stock2job] (
IdTourJob, 
IdJob,
IdStockType,
Factor, 
CustomNumber,
Caption,
Addition,
Weight
) 
	SELECT @TOURJOB, IdJob, idstocktype, factor, (SELECT CustomNumber FROM stocktype WHERE idstocktype = stocktype2job.idstocktype) CustomNumber,
	CASE WHEN CUSTOM2 IS NULL
	THEN
		CASE WHEN caption IS NULL 
			 THEN (SELECT caption FROM stocktype WHERE idstocktype = stocktype2job.idstocktype) 
		  ELSE caption 
		END 
	ELSE
	Custom2
	END,
	Custom1,
--	+ IIF(Custom1 is NULL, '' , CHAR(13) + Custom1),
	(SELECT weight FROM stocktype WHERE idstocktype = stocktype2job.idstocktype) AS Weight
  FROM stocktype2job 
  WHERE 
  idjob = @ID AND idstocktype2jobtype <> 4 AND idstocktype NOT IN (3530,4523) 
  AND (idstocktype NOT in (SELECT idstocktype FROM stocktype WHERE customnumber LIKE 'Text%') OR idstocktype IS NULL) ORDER BY sortorder


--SELECT @TOURJOB, idstocktype, factor, CASE WHEN caption IS NULL THEN (SELECT caption FROM stocktype WHERE idstocktype = stocktype2job.idstocktype) ELSE caption END AS Artikelbeschreibung, 
--                                       (SELECT weight FROM stocktype WHERE idstocktype = stocktype2job.idstocktype) AS Weight 
--                                       FROM stocktype2job 
--                                       WHERE 
--                                       idjob = @ID AND idstocktype2jobtype <> 4 AND idstocktype NOT IN (3530,4523) 
--                                       AND (idstocktype NOT in (SELECT idstocktype FROM stocktype WHERE customnumber LIKE 'Text%') OR idstocktype IS NULL) ORDER BY sortorde

update [expofair].[job2Tour]  set Weight = (select sum(Factor * Weight) from [expofair].[stock2job] where IdTourJob = @TOURJOB ) where IdTourJob =  @TOURJOB

------------------------------------------------------------------------------------------------------------------------------------------------
-- Update Stock

-- SELECT RIGHT('00000' + CAST(@Int AS VARCHAR(6)), 6)
-- 			  convert(varchar(10),Factor) + ' ' + Caption Stock
--			  RIGHT('000' + CAST(Factor AS VARCHAR(3)), 3) +  CHAR(9) + Replace(Caption, CHAR(13), CHAR(13) + CHAR(9)) Stock
--			  REPLACE(RIGHT('000' + CAST(Factor AS VARCHAR(3)), 3) +  CHAR(9) + Replace(Caption, CHAR(13), CHAR(13) + CHAR(9)), CHAR(10),' ') Stock from [expofair].[stock2job] 

			  SET @STOCK = ''
			  DECLARE @temp VARCHAR(1000)

			  DECLARE Cur2 CURSOR READ_ONLY FOR SELECT
			  REPLACE(RIGHT('000' + CAST(Factor AS VARCHAR(3)), 3) +  CHAR(9) + Replace(Caption + IIF( Addition Is NOT NULL, CHAR(13) + Addition, ''), CHAR(13), CHAR(13) + CHAR(9)), CHAR(10),' ') Stock from [expofair].[stock2job] 
			  where  IdTourJob = @TOURJOB

			  OPEN Cur2 

			  FETCH NEXT FROM Cur2 into @temp

			   WHILE  @@fetch_status = 0
			   BEGIN
      
				  SET @Stock = @Stock  +  @temp + CHAR(13)

				 FETCH NEXT FROM Cur2 into @temp
			  END
			Close cur2
			DEALLOCATE cur2 

	--	update [expofair].[job2Tour] set Stock = @STOCK where IdTourJob = @TOURJOB
		
		   update [expofair].[job2Tour] set Stock = LEFT(@STOCK, NULLIF(LEN(@STOCK)-1,-1)) where IdTourJob = @TOURJOB

-----------------------------------------------------------------------------------------------------------
	        FETCH NEXT FROM Cur1 into @TOURJOB, @ID
     END

	Close cur1
	DEALLOCATE cur1
END
GO

