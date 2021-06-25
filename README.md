# Project for Databases 2

The main goal was to prepare API (*application programming inteface*) allowing to creating and managing hierarchic structure (in this case) for company's organisational structure.

Type `hierarchyid` available in MS SQL Server (*2008*) was used in this project.

Employees data (such as name, position etc.) are elements of mentioned hierarchic structure.

|     name     |              type             |                      meaning                      |
|:-----------:|:------------------------------:|:-------------------------------------------------:|
|    `Path`    | `hierarchyid NOT NULL UNIQUE` |                     acts as ID                    |
|    `Name`    |    `varchar(100) NOT NULL`    |                 first & last name                 |
|  `Position`  |    `varchar(100) NOT NULL`    |            position/role in the company           |
| `Start_Year` |         `int NOT NULL`        | year when employee started working in the company |
|   `Salary`   |    `numeric(7, 2) NOT NULL`   |                   salary (in $)                   |
>Table: Structure of *Employee* table.

API also have some basic unit tests written with *Visual Studio 2008 IDE*. 

## Screenshots

- Functionalities are available from console:

![console_app](/screenshots/konsola.png)
>---

- Displaying mock data:

![mock_data](/screenshots/mock-data.png)
>---

- Passed tests:

![tests](/screenshots/testy.png)
>---

***More information are included in documentation pdf file (in polish).***
