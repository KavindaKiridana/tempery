-- Create Database
-- CREATE DATABASE ITAssetRequest;
-- GO

-- Use the database
USE ITAssetRequest;
GO

-- Create Reason table
CREATE TABLE Reason (
    ReasonId INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(255) NOT NULL
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

-- Create Template table
CREATE TABLE Template (
    TemplateId INT IDENTITY(1,1) PRIMARY KEY,
    Description VARCHAR(MAX) NULL,
    IsMDSign BIT NOT NULL,
    ITManagerId INT NOT NULL,
    CEOId INT NOT NULL,
    MDId INT NULL,
    FOREIGN KEY (ITManagerId) REFERENCES [User](UserId),
    FOREIGN KEY (CEOId) REFERENCES [User](UserId),
    FOREIGN KEY (MDId) REFERENCES [User](UserId)
);

-- Create Document table
CREATE TABLE Document (
    DocumentId INT IDENTITY(1,1) PRIMARY KEY,
    ReasonId INT NOT NULL,
    CompanyId INT NOT NULL,
    DepartmentId INT NOT NULL,
    UserId INT NOT NULL, -- edited user
    DepartmentHead INT NOT NULL, -- department Head user
    TemplateId int NOT NULL,
    SavedTime DATETIME DEFAULT GETDATE(),
    SerialNo VARCHAR(255),
    Status VARCHAR(50) CHECK (Status IN ('pending', 'approved', 'rejected')),
    ITDivisionComment TEXT,
    ITDivisionRecommendation TEXT,
    Remarks TEXT,
    TotalCost DECIMAL(18,2),
    Budgeted BIT,
    EIDDateOfPurchase DATETIME, -- EID means ExistingItemDetails
    EIDMake VARCHAR(255),
    EIDSerialNo VARCHAR(255),
    EIDWarranty VARCHAR(255),
    EIDModel VARCHAR(255),
    Quotation BIT,
    Configuration BIT,
    CostBeakdown BIT,
    ConfirmedBy int NOT NULL,
    FOREIGN KEY (ConfirmedBy) REFERENCES [User](UserId),
    FOREIGN KEY (ReasonId) REFERENCES Reason(ReasonId),
    FOREIGN KEY (CompanyId) REFERENCES Company(CompanyId),
    FOREIGN KEY (DepartmentId) REFERENCES Department(DepartmentId),
    FOREIGN KEY (UserId) REFERENCES [User](UserId),
    FOREIGN key(TemplateId) REFERENCES [FlexibleTemplate](FlexibleTemplateId),
    FOREIGN KEY (DepartmentHead) REFERENCES [User](UserId)
);

-- Create RequestedItemPayments table
CREATE TABLE RequestedItemPayments (
    RequestedItemPaymentsId INT IDENTITY(1,1) PRIMARY KEY,
    UnitPrice DECIMAL(18,2) NOT NULL,
    Qty INT NOT NULL,
    Description VARCHAR(500),
    DocumentID INT NOT NULL,
    SupplierId INT NOT NULL,
    FOREIGN KEY (DocumentID) REFERENCES Document(DocumentId),
    FOREIGN KEY (SupplierId) REFERENCES Supplier(SupplierId)
);

-- Create SignedBy table
CREATE TABLE SignedBy (
    DocumentID INT NOT NULL,
    UserId INT NOT NULL,
    DateTime DATETIME DEFAULT GETDATE(),
    Position VARCHAR(255),
    PRIMARY KEY (DocumentID, UserId),
    FOREIGN KEY (DocumentID) REFERENCES Document(DocumentId),
    FOREIGN KEY (UserId) REFERENCES [User](UserId)
);


CREATE TABLE FlexibleTemplate (
    FlexibleTemplateId int IDENTITY(1,1) PRIMARY KEY,
    IsActive bit not null DEFAULT(1),
)

-- Related persons table
CREATE TABLE PersonPosition (
    PersonPositionsId int IDENTITY(1,1) PRIMARY KEY,
    FlexibleTemplateId int FOREIGN KEY REFERENCES FlexibleTemplate(FlexibleTemplateId),
    Position nvarchar(100),
    PersonName nvarchar(100),
)



CREATE TABLE Category (
    CategoryId INT PRIMARY KEY IDENTITY(1,1),
    CategoryName VARCHAR(50) NOT NULL,
    Discription VARCHAR(200) NULL
);

INSERT INTO Category (CategoryName, Discription) VALUES
('Laptop/Desktop', 'Laptop & Desktop'),
('Printers/Photocopy', 'Photocopy, All kind of Printers'),
('UPS', 'UPS'),
('License', 'All kind of License'),
('AMC', 'AMC Items'),
('Rent', 'Rented Laptops or any other'),
('Monitor', 'Monitors'),
('Repair', 'Any Item which is sent to repair'),
('Others', 'Any items which is not listed above');

PRINT 'ITAssetRequest database and tables created successfully!';



