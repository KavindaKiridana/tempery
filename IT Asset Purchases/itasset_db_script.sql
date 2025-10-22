-- Create Database
-- CREATE DATABASE ITAssetRequest;
-- GO

-- Use the database
USE ITAssetRequest;
GO

-- Create Reason table
CREATE TABLE Reason (
    ReasonId INT IDENTITY(1,1) PRIMARY KEY,
    RName VARCHAR(255) NOT NULL,
    IsActive BIT NOT NULL DEFAULT(1)
);

-- Create Company table
CREATE TABLE Company (
    CompanyId INT IDENTITY(1,1) PRIMARY KEY,
    CName VARCHAR(255) NOT NULL,
    Flag VARCHAR(255) NOT NULL,
	IsActive BIT NOT NULL DEFAULT(1)
);

-- Create Department table
CREATE TABLE Department (
    DepartmentId INT PRIMARY KEY,
    DName VARCHAR(255) NOT NULL,
	IsActive BIT not null default(1)
);

-- Create Supplier table
CREATE TABLE Supplier (
    SupplierId INT IDENTITY(1,1) PRIMARY KEY,
    SName VARCHAR(255) NOT NULL,
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
    IsAuthorizer VARCHAR(50) CHECK (IsAuthorizer IN ('Unauthorizer', 'IT Manager', 'CEO', 'MD','Editor')),
    email VARCHAR(255) not null --need to add this ,give me sql query.need to add this part step by step method*
);

ALTER TABLE [Users] ADD email VARCHAR(255) NULL;
UPDATE Users set email='' where UserId=2 ;
ALTER TABLE [Users] ALTER COLUMN email VARCHAR(255) NOT NULL;

CREATE TABLE Tokens(
UsersId INT NOT NULL REFERENCES [Users](UsersId) UNIQUE,
Token VARCHAR(255) NOT NULL,
IsUsed BIT DEFAULT 0,
CreatedAt DATETIME DEFAULT GETDATE(),
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
    CategoryId INT NOT NULL,
    UsedByToWhom VARCHAR NOT NULL,
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
    LocationId VARCHAR(255) NOT NULL,
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
 FOREIGN KEY (LocationId) REFERENCES Location(LocationId),
    FOREIGN KEY (ConfirmedBy) REFERENCES [Users](UsersId),
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
    NeedToPrint BIT null,   -- use ITAssetRequest;ALTER TABLE RequestedItemPayments ADD NeedToPrint BIT NULL;
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

CREATE TABLE SerialNo (
    CompanyId INT NOT NULL,
    Year INT NOT NULL,
    DocNo INT NOT NULL DEFAULT 0,
    CONSTRAINT PK_SerialNo PRIMARY KEY (CompanyId, Year),
    CONSTRAINT FK_SerialNo_Company FOREIGN KEY (CompanyId) REFERENCES Company(CompanyId)
);


-- Create UserCompanyAccess table
CREATE TABLE UserCompanyAccess (
    UserCompanyAccessId INT IDENTITY(1,1) PRIMARY KEY,
    UsersId INT NOT NULL,
    CompanyId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT(1),
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UsersId) REFERENCES [Users](UsersId),
    FOREIGN KEY (CompanyId) REFERENCES Company(CompanyId),
    CONSTRAINT UQ_UserCompany UNIQUE (UsersId, CompanyId)
);

Create table Pages (
    PageId INT IDENTITY(1,1) PRIMARY KEY,
    Parent VARCHAR(50) not null,
    Child VARCHAR(50) not null,
    Path VARCHAR(50) not null,
    CONSTRAINT UQ_ParentChild UNIQUE (Parent, Child) 
);

Create table UserPagesAccess(
UsersId int not null,
PageId int not null,
FOREIGN KEY (UsersId) REFERENCES [Users](UsersId),
FOREIGN KEY (PageId) REFERENCES [Pages](PageId),
PRIMARY KEY(UsersId,PageId)
);

INSERT INTO Pages (Parent, Child, Path)
VALUES
--  Capex Form Page
('Capex Form', 'Capex Form', '~/About'),
--  View Capex Pages (all FinalView.aspx variations)
('View Capex', 'Pending Capex', '~/FinalView?status=pending'),
('View Capex', 'Given to Finance', '~/FinalView?status=finance'),
('View Capex', 'Payment Ready', '~/FinalView?status=payment'),
('View Capex', 'Completed Capex', '~/FinalView?status=completed'),
('View Capex', 'All Capex', '~/FinalView'),
('View Capex', 'Deleted Capex', '~/FinalView?status=deleted'),
--manage
('Manage System', 'Manage Password', '~/ManageAccount.aspx'), 
('Manage System', 'Manage Company', '~/ManageCompany.aspx'),
('Manage System', 'Manage Department', '~/ManageDepartment.aspx'),
('Manage System', 'Manage Reasons', '~/ManageReason.aspx'),
('Manage System', 'Manage Supplier', '~/ManageSupplier.aspx'),
('Manage System', 'Manage User', '~/ManageUser.aspx'),
('Manage System', 'Manage Template', '~/ManageTemplate.aspx'),
--logout
('Logout', 'Logout', '~/Logout.aspx');


INSERT INTO UserCompanyAccess (UsersId, CompanyId, IsActive) VALUES
-- Chamika (UserId 22)
(22, 20, 1), -- Renuka Agri Foods PLC
(22, 21, 1), -- Renuka Agri Organics Ltd
(22, 22, 1), -- Shaw Wallace Ceylon Ltd
(22, 23, 1), -- Richlife Dairies Limited
(22, 24, 1), -- Kandy Plantations Limited
(22, 25, 1), -- Galle Face Properties
(22, 26, 1), -- Coco Lanka (Pvt) Ltd
(22, 27, 1), -- Renuka Teas Ceylon (Pvt) Ltd
-- Kavinda (UserId 25)
(25, 20, 1), -- Renuka Agri Foods PLC
(25, 21, 1), -- Renuka Agri Organics Ltd
(25, 22, 1), -- Shaw Wallace Ceylon Ltd
(25, 23, 1), -- Richlife Dairies Limited
(25, 24, 1), -- Kandy Plantations Limited
(25, 25, 1), -- Galle Face Properties
(25, 26, 1), -- Coco Lanka (Pvt) Ltd
(25, 27, 1), -- Renuka Teas Ceylon (Pvt) Ltd
-- Lakshan (UserId 27)
(27, 20, 1), -- Renuka Agri Foods PLC
(27, 21, 1), -- Renuka Agri Organics Ltd
(27, 22, 1), -- Shaw Wallace Ceylon Ltd
(27, 23, 1), -- Richlife Dairies Limited
(27, 24, 1), -- Kandy Plantations Limited
(27, 25, 1), -- Galle Face Properties
(27, 26, 1), -- Coco Lanka (Pvt) Ltd
(27, 27, 1), -- Renuka Teas Ceylon (Pvt) Ltd
-- Thilina (UserId 17)
(17, 20, 1), -- Renuka Agri Foods PLC
(17, 21, 1), -- Renuka Agri Organics Ltd
(17, 22, 1), -- Shaw Wallace Ceylon Ltd
(17, 23, 1), -- Richlife Dairies Limited
(17, 24, 1), -- Kandy Plantations Limited
(17, 25, 1), -- Galle Face Properties
(17, 26, 1), -- Coco Lanka (Pvt) Ltd
(17, 27, 1), -- Renuka Teas Ceylon (Pvt) Ltd
-- Sahan (UserId 19)
(19, 20, 1), -- Renuka Agri Foods PLC (RAIT)
(19, 21, 1), -- Renuka Agri Organics Ltd (ROIT)
(19, 22, 1), -- Shaw Wallace Ceylon Ltd (SWIT)
-- Tharaka (UserId 18)
(18, 20, 1), -- Renuka Agri Foods PLC (RAIT)
(18, 21, 1), -- Renuka Agri Organics Ltd (ROIT)
(18, 22, 1), -- Shaw Wallace Ceylon Ltd (SWIT)
(18, 23, 1),  --Richlife
-- Sahan (UserId 19)
(29, 20, 1), -- Renuka Agri Foods PLC (RAIT)
(29, 21, 1), -- Renuka Agri Organics Ltd (ROIT)
(29, 22, 1), -- Shaw Wallace Ceylon Ltd (SWIT)
-- Chamila (UserId 31)
(31, 20, 1), -- Renuka Agri Foods PLC (RAIT)
(31, 21, 1), -- Renuka Agri Organics Ltd (ROIT)
(31, 24, 1), -- Kandy Plantations Limited (KPIT)
-- Lahiru (UserId 32)
(32, 20, 1), -- Renuka Agri Foods PLC (RAIT)
(32, 21, 1), -- Renuka Agri Organics Ltd (ROIT)
(32, 24, 1); -- Kandy Plantations Limited (KPIT)

