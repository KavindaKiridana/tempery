-- Step 1: Find the CHECK constraint name
SELECT name AS ConstraintName
FROM sys.check_constraints 
WHERE parent_object_id = OBJECT_ID('Document') 
AND definition LIKE '%Status%';

-- Step 2: Drop the CHECK constraint (replace [ConstraintName] with actual name from Step 1)
-- Example: ALTER TABLE Document DROP CONSTRAINT CK__Document__Status__123ABC45;

-- Step 3: Add default constraint to Status column
ALTER TABLE Document 
ADD CONSTRAINT DF_Document_Status DEFAULT 'pending' FOR Status;

-- Step 4: Optional - Update existing records to have default value if they have NULL
UPDATE Document 
SET Status = 'pending' 
WHERE Status IS NULL;

-- Step 5: Verify the changes
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Document' AND COLUMN_NAME = 'Status';