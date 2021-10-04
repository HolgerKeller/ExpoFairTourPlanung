Use easyjob
GO
--create schema expofair
--delete from [expofair].[job2Tour] where 1=1
--delete from [expofair].[Tour] where 1=1
--delete from [expofair].[stock2job] where 1=1

GO
drop TABLE [expofair].[job2Tour];
GO
create Table [expofair].[job2Tour] (
IdTourJob INT NOT NULL IDENTITY(1,1),
IdTour INT NULL,
TourName NVARCHAR(200),
SplitCounter INT NOT NULL,
IdJob INT NOT NULL,
IdJobState INT NOT NULL,
IdProject INT NOT NULL,
IdAddress INT NULL,
Ranking INT NULL,
Number NVARCHAR(30) NULL,
Caption NVARCHAR(MAX) NULL,
Comment NVARCHAR(MAX) NULL,
HeadLine NVARCHAR(4000) NULL,
JobDate Date NOT NULL,
JobDateReturn Date NULL,
JobStartTime DATETIME NULL,
Service NVARCHAR(100) NOT NULL,
Status NVARCHAR(100) NOT NULL,
Address NVARCHAR(300) NULL,
Stock NVARCHAR(MAX) NULL,
Weight DECIMAL NULL,
Volume DECIMAL NULL,
Time NVARCHAR(200) NULL,
In_Out NVARCHAR(20) NOT NULL,
DeliveryTimeStart DATETIME NULL,
DeliveryTimeEnd DATETIME NULL,
PickupTimeStart DATETIME NULL,
PickupTimeEnd DATETIME NULL,
Contact NVARCHAR(100) NULL,
ContactPhone NVARCHAR(100) NULL,
ReadyTime NVARCHAR(200) NULL,
JobType NVARCHAR(50) NULL,
DeliveryType NVARCHAR(50) NULL,
PRIMARY KEY( IdTourJob),
CONSTRAINT AK_JOB_IN_OUT UNIQUE(IdJob, In_Out, SplitCounter)  WITH (IGNORE_DUP_KEY = ON)
);
GO
drop Table [expofair].[Tour];
GO
create Table [expofair].[Tour] (
IdTour INT NOT NULL IDENTITY(1,1),
TourName NVARCHAR(200) NULL,
Comment NVARCHAR(4000) NULL,
Footer NVARCHAR(4000) NULL,
TourDate DATE NOT NULL DEFAULT (GETDATE()),
VehicleNr NVARCHAR(20) NULL,
Driver  NVARCHAR(200) NULL,
Master NVARCHAR(200) NULL,
Weight DECIMAL NULL,
Volume DECIMAL NULL,
SecDriver NVARCHAR(200) NULL,
Team NVARCHAR(1000) NULL,
HeadLine NVARCHAR(300) NULL,
Hubwagen NVARCHAR(100) NULL,
Sackkarre NVARCHAR(100) NULL,
Sonstiges NVARCHAR(200) NULL,
CreatedBy VARCHAR(100) NULL,
IsSBTour BIT NULL,
TourNr INT NULL,
CreateTime DATETIME NOT NULL DEFAULT (GETDATE()),
PRIMARY KEY(IdTour)
);
GO
drop Table [expofair].[Vehicle];
GO
create Table [expofair].[Vehicle] (
IdVehicle INT NOT NULL IDENTITY(1,1),
VehicleNr NVARCHAR(20) NULL,
VehicleType NVARCHAR(100) NULL,
Comment NVARCHAR(500) NULL,
NetWeight decimal,
NetVolume decimal,
Status NVARCHAR(20) NULL,
Owner NVARCHAR(300) NULL,
IsActiv BIT NULL,
StartDate Date NULL,
EndDate  Date Null,
PRIMARY KEY(IdVehicle)
);
GO

drop Table [expofair].[Stuff];
GO
create Table [expofair].[Stuff] (
IdStuff INT NOT NULL IDENTITY(1,1),
EmployeeName1 NVARCHAR(100) NULL,
EmployeeName2 NVARCHAR(100) NULL,
EmployeeType NVARCHAR(100) NULL,
EmployeeNr NVARCHAR(100) NULL,
Comments NVARCHAR(500) NULL,
IsActiv BIT NULL,
Status NVARCHAR(20) NULL,
Employer NVARCHAR(300) NULL,
StartDate Date NULL,
EndDate  Date Null,
PRIMARY KEY(IdStuff)
);
GO
drop TABLE [expofair].[stock2job];
GO
create Table [expofair].[stock2job] (
IdStock INT NOT NULL IDENTITY(1,1),
IdTourJob INT NULL,
IdStockType INT  NULL,
Factor INT NULL,
Caption NVARCHAR(200) NULL,
Weight DECIMAL NULL,
Volume DECIMAL NULL,
Status NVARCHAR(10) NULL,
PRIMARY KEY( IdStock)
)