Use easyjob
GO
drop TABLE [expofair].[job2Tour];
GO
create Table [expofair].[job2Tour] (
IdTourJob INT NOT NULL IDENTITY(1,1),
IdTour INT NULL,
IdJob INT NOT NULL,
IdJobState INT NOT NULL,
IdProject INT NOT NULL,
IdAddress INT NULL,
Ranking INT NULL,
Number NVARCHAR(30) NULL,
Caption NVARCHAR(MAX) NULL,
Comment NVARCHAR(MAX) NULL,
JobDate Date NOT NULL,
Service VARCHAR(100) NOT NULL,
Status VARCHAR(100) NOT NULL,
Address VARCHAR(300) NULL,
Stock NVARCHAR(MAX)   NULL,
TIME VARCHAR(300) NULL,
In_Out VARCHAR(20) NOT NULL,
PRIMARY KEY( IdTourJob),
CONSTRAINT AK_JOB_IN_OUT UNIQUE(IdJob, In_Out)  WITH (IGNORE_DUP_KEY = ON)
);
GO
drop Table [expofair].[Tour];
GO
create Table [expofair].[Tour] (
IdTour INT NOT NULL IDENTITY(1,1),
TourName VARCHAR(200) NOT NULL,
Comment NVARCHAR(MAX) NULL,
TourDate DATE NOT NULL DEFAULT (GETDATE()),
VehicleNr VARCHAR(20) NULL,
Driver  VARCHAR(200) NULL,
Master VARCHAR(200) NULL,
SecDriver VARCHAR(200) NULL,
Team NVARCHAR(1000) NULL,
HeadLine NVARCHAR(300) NULL,
Hubwagen NVARCHAR(100) NULL,
Sackkarre NVARCHAR(100) NULL,
Sonstiges NVARCHAR(200) NULL,
CreatedBy VARCHAR(100) NULL,
CreateTime DATETIME NOT NULL DEFAULT (GETDATE()),
PRIMARY KEY(IdTour)
);
GO
drop Table [expofair].[Vehicle];
GO
create Table [expofair].[Vehicle] (
IdVehicle INT NOT NULL IDENTITY(1,1),
VehicleNr VARCHAR(20) NULL,
VehicleType VARCHAR(20) NULL,
Comment VARCHAR(500) NULL,
MaxLoad INT NULL,
PRIMARY KEY(IdVehicle)
);
GO

insert into [expofair].[Vehicle] ( VehicleNr, VehicleType, MaxLoad) values ('B-HK1234', 'LKW 20T', 5000);
insert into [expofair].[Vehicle] ( VehicleNr, VehicleType, MaxLoad) values ('B-HK1212', 'LKW 20T', 5000);
insert into [expofair].[Vehicle] ( VehicleNr, VehicleType, MaxLoad) values ('B-HK4321', 'LKW 20T', 5000);

GO
drop Table [expofair].[Stuff];
GO
create Table [expofair].[Stuff] (
IdStuff INT NOT NULL IDENTITY(1,1),
EmployeeName1 VARCHAR(100) NULL,
EmployeeName2 VARCHAR(100) NULL,
EmployeeType VARCHAR(100) NULL,
EmployeeNr VARCHAR(100) NULL,
Comments VARCHAR(500) NULL,
PRIMARY KEY(IdStuff)
);
GO
insert into [expofair].[Stuff] (EmployeeName1, EmployeeType, EmployeeNr) values ( 'Marcus Söder', 'Fahrer', '123456' ); 
insert into [expofair].[Stuff] (EmployeeName1, EmployeeType, EmployeeNr) values ( 'Armin Laschet', 'Loader', '121212' ); 
insert into [expofair].[Stuff] (EmployeeName1, EmployeeType, EmployeeNr) values ( 'Olaf Scholz', 'Loader', '343434' );
GO

