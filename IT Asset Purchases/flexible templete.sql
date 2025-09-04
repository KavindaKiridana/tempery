
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