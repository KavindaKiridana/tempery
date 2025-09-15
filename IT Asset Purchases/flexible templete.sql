
CREATE TABLE FlexibleTemplate (
    FlexibleTemplateId int IDENTITY(1,1) PRIMARY KEY,
    CompanyId INT NOT NULL,
    IsActive bit not null DEFAULT(1),
    FOREIGN KEY (CompanyId) REFERENCES Company(CompanyId)
);

-- Related persons table
CREATE TABLE PersonPosition (
    PersonPositionId INT IDENTITY(1,1) PRIMARY KEY,
    FlexibleTemplateId int not null,
    PersonId int not null,
    Position VARCHAR(50) not null,
FOREIGN KEY (FlexibleTemplateId) REFERENCES FlexibleTemplate(FlexibleTemplateId),
FOREIGN KEY (PersonId) REFERENCES Users(UsersId),
CONSTRAINT UQ_PersonPosition UNIQUE (FlexibleTemplateId, PersonId) -- Composite unique constraint
);


ALTER TABLE RequestedItemPayments
ADD Currency VARCHAR(50);



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