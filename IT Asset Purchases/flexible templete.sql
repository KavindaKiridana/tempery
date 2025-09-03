
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