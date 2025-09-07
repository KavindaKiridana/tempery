-- Insert data into Reason table
INSERT INTO Reason (RName) 
VALUES ('Subscription Renewal');

-- Insert data into Company table
INSERT INTO Company (CName, Flag) 
VALUES 
    ('Renuka Agro Exports (Pvt) Ltd', 'RAE'),
    ('Renuka Agri Foods PLC', 'RAF');

-- Insert data into Department table
INSERT INTO Department (DName) 
VALUES 
    ('IT'),
    ('HR'),
    ('Finance');

-- Insert data into Supplier table
INSERT INTO Supplier (SName, Currency) 
VALUES 
    ('Supplier1', 'USD'),
    ('Supplier2', 'LKR');

-- Insert data into Users table
INSERT INTO [Users] (UserName, Password, IsActive, FullName, IsHeadOrNot, IsAuthorizer) 
VALUES 
    ('kaush', '123', 1, 'Mr.Kaushlya', 0, 'Unauthorizer'),
    ('sanjula', '123', 1, 'Mr.Sanjula Dayaweera', 0, 'IT Manager'),
    ('shamindra','123',1,'Mr.Shamindra Rajiyah',0,'MD'),
    ('asitha','123',1,'Mr.Asitha Peiris',1,'CEO');

Insert into Template (IsMDSign,CompanyId,ITManagerId,CEOId,MDId)
VALUES 
(1,1,2,4,3),
(0,1,2,4,NULL),
(1,2,2,4,3),
(0,2,2,4,NULL);

