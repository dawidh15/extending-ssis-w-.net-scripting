-- Add new ID column to avoid the PK problem

-- Get the name of a constraint into a variable
DECLARE @CustomerPKName AS VARCHAR(100);

SET @CustomerPKName =  (SELECT CONSTRAINT_NAME
						  FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
						  WHERE TABLE_CONSTRAINTS.TABLE_NAME = 'Customer'
						  AND CONSTRAINT_TYPE = 'PRIMARY KEY');

EXEC('ALTER TABLE Customer DROP CONSTRAINT ' + @CustomerPKName);

GO

-- Add a new auto increment col and set it to PK
ALTER TABLE Customer ADD  ID INT IDENTITY(1,1);

ALTER TABLE CUSTOMER ADD CONSTRAINT PK_Customer PRIMARY KEY(ID);