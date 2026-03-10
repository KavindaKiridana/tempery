USE [ITAssetRequest];
GO
-- existing tables
CREATE TABLE Company (
    CompanyId int IDENTITY(1,1) PRIMARY KEY,
    CName varchar(255) NOT NULL,
    Flag varchar(255) NOT NULL,
    IsActive bit NOT NULL DEFAULT (1),
    AddedUser int NULL REFERENCES Users(UsersId),
	AddedTime datetime NULL
);
CREATE TABLE Location (
    LocationId int IDENTITY(1,1) PRIMARY KEY,
    LName varchar(255) NOT NULL,
    IsStockLocation bit NOT NULL DEFAULT (0),
    --when organization purchased any asset, initially those assets r assigned to this stock-locations not all locations
    IsActive bit NOT NULL DEFAULT (1)
);
 
CREATE TABLE Department (
    DepartmentId int IDENTITY(1,1) PRIMARY KEY,
    DName varchar(255) NOT NULL,
    IsActive bit NOT NULL DEFAULT (1),
    AddedUser int NULL REFERENCES Users(UsersId),
	AddedTime datetime NULL
);
CREATE TABLE Supplier (
    SupplierId int IDENTITY(1,1) PRIMARY KEY,
    SName varchar(255) NOT NULL,
    Currency varchar(50) NULL,
    IsActive bit NOT NULL DEFAULT (1),
    AddedUser int NULL REFERENCES Users(UsersId),
	AddedTime datetime NULL
);
CREATE TABLE Users (
    UsersId int IDENTITY(1,1) PRIMARY KEY,
    UserName varchar(255) NULL, 
    FullName varchar(255) NOT NULL,
    Password varchar(255) NULL, 
    email varchar(255) NULL,
    Phone varchar(20) NULL,
    Designation varchar(100) NULL,
    IsHeadOrNot bit NULL,
    IsActive bit NOT NULL DEFAULT (1),
    AddedUser int NULL REFERENCES Users(UsersId),
	AddedTime datetime NULL,
	isCapexUser bit NOT NULL DEFAULT (0),
	DepartmentId int NULL REFERENCES Department(DepartmentId)
);

---new tables which need to create
create table OS (
OsId int IDENTITY(1,1) PRIMARY KEY,
OS varchar(255) NOT NULL,
IsActive bit NOT NULL DEFAULT (1),
AddedUser int NULL REFERENCES Users(UsersId),
AddedTime datetime NULL
);
create table Software (
SoftwareId int IDENTITY(1,1) PRIMARY KEY,
SoftwareName varchar(255) NOT NULL,
IsActive bit NOT NULL DEFAULT (1),
AddedUser int NULL REFERENCES Users(UsersId),
AddedTime datetime NULL
);
create table Processor (
PId int IDENTITY(1,1) PRIMARY KEY,
Processor varchar(255) NOT NULL,
IsActive bit NOT NULL DEFAULT (1),
AddedUser int NULL REFERENCES Users(UsersId),
AddedTime datetime NULL
);
create table RAMSize (
RAMSId int IDENTITY(1,1) PRIMARY KEY,
Size varchar(255) NOT NULL,
IsActive bit NOT NULL DEFAULT (1),
AddedUser int NULL REFERENCES Users(UsersId),
AddedTime datetime NULL
);
create table RAMType (
RAMTId int IDENTITY(1,1) PRIMARY KEY,
Type varchar(255) NOT NULL,
IsActive bit NOT NULL DEFAULT (1),
AddedUser int NULL REFERENCES Users(UsersId),
AddedTime datetime NULL
);
create table HDD (
HDDId int IDENTITY(1,1) PRIMARY KEY,
HDD varchar(255) NOT NULL,
IsActive bit NOT NULL DEFAULT (1),
AddedUser int NULL REFERENCES Users(UsersId),
AddedTime datetime NULL
);
create table SSD (
SSDId int IDENTITY(1,1) PRIMARY KEY,
SSD varchar(255) NOT NULL,
IsActive bit NOT NULL DEFAULT (1),
AddedUser int NULL REFERENCES Users(UsersId),
AddedTime datetime NULL
);
create table Display (
DisplayId int IDENTITY(1,1) PRIMARY KEY,
Display varchar(255) NOT NULL,
IsActive bit NOT NULL DEFAULT (1),
AddedUser int NULL REFERENCES Users(UsersId),
AddedTime datetime NULL
);
create table Model (
ModelId int IDENTITY(1,1) PRIMARY KEY,
Model varchar(255) NOT NULL,
IsActive bit NOT NULL DEFAULT (1),
AddedUser int NULL REFERENCES Users(UsersId),
AddedTime datetime NULL
);

create table Type (
TypeId int IDENTITY(1,1) PRIMARY KEY,
Type varchar(255) NOT NULL,
Category varchar(255) NOT NULL,
IsActive bit NOT NULL DEFAULT (1),
AddedUser int NULL REFERENCES Users(UsersId),
AddedTime datetime NULL
);

-- Asset table definition
CREATE TABLE Asset (
    AssetId varchar(10) PRIMARY KEY, -- RGLxxxxx (changed from varcher to varchar)
    Type varchar(50) NOT NULL,
    SupplierId int NULL,
    EditedUser int NOT NULL,
    OsId int NULL,
    PId int NULL,
    RAMSId int NULL,
    RAMTId int NULL,
    HDDId int NULL,
    SSDId int NULL,
    DisplayId int NULL,
    Model int NULL REFERENCES Model(ModelId),
    DoP Date NULL,
    FinanceAssetCode varchar(100) NULL,
    Warranty int NULL,
    ManufactureSN varchar(255) NULL,
    Brandnew bit NULL,
    Cost decimal(18,2) NULL, 
    Name varchar(255) NULL,
    IPAddress varchar(45) NULL,
    Make varchar(100) NULL,
    WindowsKey varchar(100) NULL,
    Motherboard varchar(100) NULL,
    PowerSupply bit NULL,
    RAIDSupport bit NULL,
    Note varchar(MAX) NULL,
    AddedTime datetime NOT NULL DEFAULT (GETDATE()),
    FOREIGN KEY (SupplierId) REFERENCES Supplier(SupplierId),
    FOREIGN KEY (EditedUser) REFERENCES Users(UsersId),
    FOREIGN KEY (OsId) REFERENCES OS(OsId),
    FOREIGN KEY (PId) REFERENCES Processor(PId),
    FOREIGN KEY (RAMSId) REFERENCES RAMSize(RAMSId),
    FOREIGN KEY (RAMTId) REFERENCES RAMType(RAMTId),
    FOREIGN KEY (HDDId) REFERENCES HDD(HDDId),
    FOREIGN KEY (SSDId) REFERENCES SSD(SSDId),
    FOREIGN KEY (DisplayId) REFERENCES Display(DisplayId)
);

create table Stocks (
    AssetId varchar(10) NOT NULL,
    CompanyId int NOT NULL,
    LocationId int NOT NULL,    
    Quantity int NOT NULL,--only when added distroyed/losed this qty became 0 otherwise this this would be mostly 1
    FOREIGN KEY (AssetId) REFERENCES Asset(AssetId),
    FOREIGN KEY (CompanyId) REFERENCES Company(CompanyId),
    FOREIGN KEY (LocationId) REFERENCES Location(LocationId)
);

CREATE TABLE InstalledSoftwares (
    InstalledSoftware int IDENTITY(1,1) PRIMARY KEY,
    AssetId varchar(10) NOT NULL,
    SoftwareId int NOT NULL,
    IsActive bit NOT NULL DEFAULT (1),
	UNIQUE  (AssetId, SoftwareId),
    FOREIGN KEY (AssetId) REFERENCES Asset(AssetId),
    FOREIGN KEY (SoftwareId) REFERENCES Software(SoftwareId)
);

--a main item (laptop/desktop/server) would have one or more spare parts
CREATE TABLE AssetSpareParts (
    SparePartAssetId int IDENTITY(1,1) PRIMARY KEY,
    MainAssetId varchar(10) NOT NULL,
    SparePartId varchar(10) NOT NULL,
    IsActive bit NOT NULL DEFAULT (1),
    UNIQUE (MainAssetId,SparePartId),
    FOREIGN KEY (MainAssetId) REFERENCES Asset(AssetId),
    FOREIGN KEY (SparePartId) REFERENCES Asset(AssetId) 
);

create table AssetUsedBy (
AssetUserId int IDENTITY(1,1) PRIMARY KEY,
AssetId varchar(10) NOT NULL,
UsedBy int NOT NULL,
IsActive bit NOT NULL DEFAULT (1),
UNIQUE (AssetId, UsedBy),
FOREIGN KEY (AssetId) REFERENCES Asset(AssetId),
FOREIGN KEY (UsedBy) REFERENCES Users(UsersId)
);

create table Repairs (
AssetId varchar(10) NOT NULL UNIQUE,
RepairStatus bit NOT NULL DEFAULT (1),
FOREIGN KEY (AssetId) REFERENCES Asset(AssetId)
);

CREATE TABLE Transactions (
    TransactionId int IDENTITY(1,1) PRIMARY KEY,
    AssetId varchar(10) NOT NULL,
    EditedUser int NOT NULL,
    Type varchar(50) NOT NULL,--transaction type
    FromId int NULL, --this would be UserId,LocationId,SupplierId, LocationId
    ToId int NULL, --this would be UserId,LocationId,SupplierId, SupplierId
    RelatedAssetId varchar(10) NULL REFERENCES Asset(AssetId),
    Time datetime NOT NULL DEFAULT GETDATE(),
    Note varchar(MAX) NULL,
    RepairCost decimal(18,2) NULL,
    IsTempAssigned bit null, -- that indicates whether a temporary asset was given to a user or not,explain ur answer
    FOREIGN KEY (AssetId) REFERENCES Asset(AssetId),
    FOREIGN KEY (EditedUser) REFERENCES Users(UsersId)
);

CREATE TABLE Complains (
ComplainId int IDENTITY(1,1) PRIMARY KEY,
UserId int NOT NULL REFERENCES Users(UsersId),
Note varchar(MAX) NULL,
IsActive bit NOT NULL DEFAULT (1),
Time datetime NOT NULL DEFAULT GETDATE()
);

CREATE TABLE AssetComplains(
ComplainId int NOT NULL REFERENCES Complains(ComplainId),
AssetId varchar(10) NOT NULL REFERENCES Asset(AssetId)
);

CREATE TABLE ITObservation (
    ObservationId int IDENTITY(1,1) PRIMARY KEY,
    AssetId varchar(10) NOT NULL,
    ObservedBy int NOT NULL,
    ObservationNote varchar(MAX) NOT NULL,
    IsActive bit NOT NULL DEFAULT (1),
    ActionTaken bit NOT NULL DEFAULT (0),
    ObservationTime datetime NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (AssetId) REFERENCES Asset(AssetId),
    FOREIGN KEY (ObservedBy) REFERENCES Users(UsersId)
);



--user table modification
-- add isCapexUser
ALTER TABLE Users
ADD isCapexUser bit NOT NULL DEFAULT (0);
-- removing IsAuthorizer
ALTER TABLE Users
DROP CONSTRAINT CK_Users_IsAuthorizer;
ALTER TABLE Users
DROP COLUMN IsAuthorizer;
-- make nullable
ALTER TABLE Users
ALTER COLUMN UserName varchar(255) NULL;
ALTER TABLE Users
ALTER COLUMN Password varchar(255) NULL;
-- add departmentId
ALTER TABLE Users
ADD DepartmentId int NULL REFERENCES Department(DepartmentId);
-- add locationId
ALTER TABLE Users
ADD LocationId int NULL REFERENCES  Location(LocationId);