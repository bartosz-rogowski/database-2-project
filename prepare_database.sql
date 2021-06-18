CREATE DATABASE Project_Hierarchy
GO
USE Project_Hierarchy
GO

CREATE TABLE Employees(
	Path hierarchyid NOT NULL UNIQUE,
	Name varchar(100) NOT NULL,
	Position varchar(100) NOT NULL,
	Start_Year int NOT NULL,
	Salary numeric(7, 2) NOT NULL
)
GO