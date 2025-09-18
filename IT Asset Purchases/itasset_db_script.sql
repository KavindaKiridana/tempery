-- Create Database
-- CREATE DATABASE ITAssetRequest;
-- GO

-- Use the database
USE ITAssetRequest;
GO

-- Create Reason table
CREATE TABLE Reason (
    ReasonId INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    IsActive BIT NOT NULL DEFAULT(1)
);

-- Create Company table
CREATE TABLE Company (
    CompanyId INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Flag VARCHAR(255) NOT NULL,
	IsActive BIT NOT NULL DEFAULT(1)
);

-- Create Department table
CREATE TABLE Department (
    DepartmentId INT PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
	IsActive BIT not null default(1)
);

-- Create Supplier table
CREATE TABLE Supplier (
    SupplierId INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Currency VARCHAR(50),
    IsActive bit not null default(1)
);

-- Create User table
CREATE TABLE [Users] (
    UsersId INT IDENTITY(1,1) PRIMARY KEY,
    Password VARCHAR(255) NOT NULL,
    IsActive BIT NOT NULL,
    UserName VARCHAR(255) NOT NULL,
    FullName VARCHAR(255) NOT NULL,
    IsHeadOrNot BIT NOT NULL,
    IsAuthorizer VARCHAR(50) CHECK (IsAuthorizer IN ('Unauthorizer', 'IT Manager', 'CEO', 'MD','Editor'))
);

-- Create Document table
CREATE TABLE Document (
    DocumentId INT IDENTITY(1,1) PRIMARY KEY,
    ReasonId INT NOT NULL,
    CompanyId INT NOT NULL,
    DepartmentId INT NOT NULL,
    UsersId INT NOT NULL, -- edited user
    DepartmentHead INT NOT NULL, -- department Head user
    TemplateId int NOT NULL,
    CategoryId INT NOT NULL;
    ConfirmedBy int NOT NULL,
    SavedTime DATETIME DEFAULT GETDATE(),
    SerialNo VARCHAR(255),
    Status VARCHAR(50) ,
    ITDivisionComment TEXT NOT NULL,
    ITDivisionRecommendation TEXT,
    Remarks TEXT,
    Budgeted BIT,
    EIDDateOfPurchase DATETIME, -- EID means ExistingItemDetails
    EIDMake VARCHAR(255),
    EIDSerialNo VARCHAR(255),
    EIDWarranty VARCHAR(255),
    EIDModel VARCHAR(255),
    Quotation BIT,
    Configuration BIT,
    CostBeakdown BIT,
    Location VARCHAR(255) NOT NULL;
 SentToHO DATE NULL,
 ReceivedToHO DATE NULL,
 HandoverDate DATE NULL,
 ReceivedDate DATE NULL,
 DateGivenToFinance DATE NULL,
 PurchaseOrderNo VARCHAR(100) NULL,
 InvoiceNo VARCHAR(100) NULL,
 PaymentReady BIT NULL,
 IsSellerPaid BIT NULL,
 FinalRemarks VARCHAR(MAX) NULL,
    FOREIGN KEY (ConfirmedBy) REFERENCES [User](UsersId),
    FOREIGN KEY (ReasonId) REFERENCES Reason(ReasonId),
    FOREIGN KEY (CompanyId) REFERENCES Company(CompanyId),
    FOREIGN KEY (DepartmentId) REFERENCES Department(DepartmentId),
    FOREIGN KEY (UsersId) REFERENCES [Users](UsersId),
    FOREIGN key(TemplateId) REFERENCES [FlexibleTemplate](FlexibleTemplateId),
    FOREIGN KEY (DepartmentHead) REFERENCES [Users](UsersId),
    FOREIGN KEY (CategoryId) REFERENCES Category(CategoryId)
);

-- Create RequestedItemPayments table
CREATE TABLE RequestedItemPayments (
    RequestedItemPaymentsId INT IDENTITY(1,1) PRIMARY KEY,
    UnitPrice DECIMAL(18,2) NOT NULL,
    Qty INT NOT NULL,
    Description VARCHAR(500),
    DocumentID INT NOT NULL,
    SupplierId INT NOT NULL,
    Currency VARCHAR(50),
    FOREIGN KEY (DocumentID) REFERENCES Document(DocumentId),
    FOREIGN KEY (SupplierId) REFERENCES Supplier(SupplierId)
);


CREATE TABLE FlexibleTemplate (
    FlexibleTemplateId int IDENTITY(1,1) PRIMARY KEY,
    CompanyId INT NOT NULL,
    IsActive bit not null DEFAULT(1),
    FOREIGN KEY (CompanyId) REFERENCES Company(CompanyId)
);

CREATE TABLE PersonPosition (
    PersonPositionId INT IDENTITY(1,1) PRIMARY KEY,
    FlexibleTemplateId int not null,
    PersonId int not null,
    Position VARCHAR(50) not null,
FOREIGN KEY (FlexibleTemplateId) REFERENCES FlexibleTemplate(FlexibleTemplateId),
FOREIGN KEY (PersonId) REFERENCES Users(UsersId),
CONSTRAINT UQ_PersonPosition UNIQUE (FlexibleTemplateId, PersonId) -- Composite unique constraint
);

CREATE TABLE Category (
    CategoryId INT PRIMARY KEY IDENTITY(1,1),
    CategoryName VARCHAR(50) NOT NULL,
    Discription VARCHAR(200) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);






