SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE or ALTER PROCEDURE [expofair].[CustAddJobToTour]
      @JOBID int,
	  @TOURID int
AS
BEGIN 

      DECLARE @Address varchar(400)
	  DECLARE @Comment varchar(MAX)
	  DECLARE @Caption varchar(150)
	  DECLARE @Number varchar(100)

      DECLARE cur CURSOR FOR SELECT T1.Company + ':' + T1.Zip + ':' + T1.City + ':' + T1.Street, t2.Comment, T2.Caption, T2.Number from [easyjob].[dbo].[Address] t1, [easyjob].[dbo].Job t2 where t2.IdJob = @JOBID and t2.IdAddress_Delivery = t1.IdAddress
	  OPEN cur
	     
	  FETCH NEXT FROM cur INTO @Address, @Comment, @Caption, @Number
		 
      Close cur
	  DEALLOCATE cur 

	  print @Address

	  DECLARE @Stock varchar(8000)
	  DECLARE @temp varchar (100)
	   
	   SET @STOCK = ''

	   DECLARE Cur2 CURSOR READ_ONLY FOR SELECT
       convert(varchar(10),t1.[Loaded]) + ' ' + t2.[Caption] Stock
      FROM [easyjob].[dbo].[StockType2Job] t1, [easyjob].[dbo].[StockType] t2 where t1.Idjob = 102574 and t1.IdStockType = t2.idstockType
	  
	  OPEN Cur2 

	  FETCH NEXT FROM Cur2 into @temp

	   WHILE  @@fetch_status = 0
       BEGIN
      
          SET @Stock = @Stock + ';' +  @temp

         FETCH NEXT FROM Cur2 into @temp
      END

     Close cur2
	 DEALLOCATE cur2
END