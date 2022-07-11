---- SQL server

---- Naming Convensions:
---- Be consistent.
---- Use capital to separate words for fields.
---- Use plural names for tables.
---- Use singular names for fields.
---- Use "xxxId" as the primary key, where "xxx" is table name.
---- Use "xxxDt" for date time.
---- Use "xxxBy" for a person, role or group.

---- Reference:
---- https://www.w3schools.com/sql/sql_primarykey.asp
---- http://www.ntu.edu.sg/home/ehchua/programming/sql/relational_database_design.html
---- http://mark.random-article.com/musings/db-tables.html
---- https://medium.com/@fbnlsr/the-table-naming-dilemma-singular-vs-plural-dc260d90aaff
---- https://www.vertabelo.com/blog/technical-articles/naming-conventions-in-database-modeling

---- Why am I getting "Cannot Connect to Server - A network-related or instance-specific error"?
---- https://stackoverflow.com/questions/18060667/why-am-i-getting-cannot-connect-to-server-a-network-related-or-instance-speci
---- Open "SQL Server Configuration Manager"
---- Now Click on "SQL Server Network Configuration" and Click on "Protocols for Name"
---- Right Click on "TCP/IP" (make sure it is Enabled) Click on Properties
---- Now Select "IP Addresses" Tab -and- Go to the last entry "IP All"
---- Enter "TCP Port" 1433.
---- Now Restart "SQL Server .Name." using "services.msc" (winKey + r)

--------------------------------------------------
USE MvcApp1;
--USE AspNetCoreApp1;

CREATE TABLE Students (
StudentId int IDENTITY(1,1) NOT NULL PRIMARY KEY,
StudentName varchar(255) NOT NULL,
Age int NOT NULL,
Email varchar(255) NULL
);

INSERT INTO Students (StudentName,Age,Email) VALUES ('John',18,'john@a.com');
INSERT INTO Students (StudentName,Age,Email) VALUES ('Steve',21,'steve@a.com');
INSERT INTO Students (StudentName,Age,Email) VALUES ('Bill',25,'bill@a.com');

CREATE TABLE Departments (
DepartmentId int NOT NULL PRIMARY KEY,
Code varchar(8) NOT NULL,
DisplayName varchar(255) NOT NULL,
Description varchar(255) NULL
);

---- SQLite
/*
CREATE TABLE Categories (
DepartmentId INTEGER PRIMARY KEY NOT NULL,
Code varchar(8) NOT NULL UNIQUE,
DisplayName varchar(255) NOT NULL,
Description varchar(255) NULL
);
*/

INSERT INTO Departments (DepartmentId,Code,DisplayName,Description) VALUES (1,'ADM','Administrative','Administrative');
INSERT INTO Departments (DepartmentId,Code,DisplayName,Description) VALUES (2,'ACC','Accounting','Administrative');
INSERT INTO Departments (DepartmentId,Code,DisplayName,Description) VALUES (3,'IT','Internet Technology','Internet Technology');
INSERT INTO Departments (DepartmentId,Code,DisplayName,Description) VALUES (4,'LOG','Logistics','Logistics');
INSERT INTO Departments (DepartmentId,Code,DisplayName,Description) VALUES (5,'RET','Retail','Retail');

CREATE TABLE Categories (
CategoryId int NOT NULL PRIMARY KEY,
Code varchar(8) NOT NULL UNIQUE,
DepartmentId int NOT NULL,
DisplayName varchar(255) NOT NULL,
Description varchar(255) NULL
);

---- SQLite
/*
CREATE TABLE Categories (
CategoryId INTEGER PRIMARY KEY NOT NULL,
Code varchar(8) NOT NULL UNIQUE,
DepartmentId int NOT NULL,
DisplayName varchar(255) NOT NULL,
Description varchar(255) NULL
);
*/

INSERT INTO Categories (CategoryId,Code,DepartmentId,DisplayName,Description) VALUES (1,'ADM1',1,'Admin 1','Admin 1');
INSERT INTO Categories (CategoryId,Code,DepartmentId,DisplayName,Description) VALUES (2,'ADM2',1,'Admin 2','Admin 2');
INSERT INTO Categories (CategoryId,Code,DepartmentId,DisplayName,Description) VALUES (3,'ADM3',1,'Admin 3','Admin 3');
INSERT INTO Categories (CategoryId,Code,DepartmentId,DisplayName,Description) VALUES (4,'ACC1',2,'Account 1','Account 1');
INSERT INTO Categories (CategoryId,Code,DepartmentId,DisplayName,Description) VALUES (5,'ACC2',2,'Account 2','Account 2');
INSERT INTO Categories (CategoryId,Code,DepartmentId,DisplayName,Description) VALUES (6,'ACC3',2,'Account 3','Account 3');
INSERT INTO Categories (CategoryId,Code,DepartmentId,DisplayName,Description) VALUES (7,'IT1',3,'IT 1','IT 1');
INSERT INTO Categories (CategoryId,Code,DepartmentId,DisplayName,Description) VALUES (8,'IT2',3,'IT 2','IT 2');
INSERT INTO Categories (CategoryId,Code,DepartmentId,DisplayName,Description) VALUES (9,'IT3',3,'IT 3','IT 3');
INSERT INTO Categories (CategoryId,Code,DepartmentId,DisplayName,Description) VALUES (10,'LOG1',4,'Logistics 1','Logistics 1');
INSERT INTO Categories (CategoryId,Code,DepartmentId,DisplayName,Description) VALUES (11,'LOG2',4,'Logistics 2','Logistics 2');
INSERT INTO Categories (CategoryId,Code,DepartmentId,DisplayName,Description) VALUES (12,'LOG3',4,'Logistics 3','Logistics 3');
INSERT INTO Categories (CategoryId,Code,DepartmentId,DisplayName,Description) VALUES (13,'RET1',5,'Retail 1','Retail 1');
INSERT INTO Categories (CategoryId,Code,DepartmentId,DisplayName,Description) VALUES (14,'RET2',5,'Retail 2','Retail 2');
INSERT INTO Categories (CategoryId,Code,DepartmentId,DisplayName,Description) VALUES (15,'RET3',5,'Retail 3','Retail 3');

CREATE TABLE SubCategories (
SubCategoryId int NOT NULL PRIMARY KEY,
Code varchar(8) NOT NULL UNIQUE,
CategoryId int NOT NULL,
DisplayName varchar(255) NOT NULL,
Description varchar(255) NULL
);

---- SQLite
/*
CREATE TABLE SubCategories (
SubCategoryId INTEGER PRIMARY KEY NOT NULL,
Code varchar(8) NOT NULL UNIQUE,
CategoryId int NOT NULL,
DisplayName varchar(255) NOT NULL,
Description varchar(255) NULL
);
*/

INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (1,'ADM1A',1,'Admin 1A','Admin 1A');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (2,'ADM1B',1,'Admin 1B','Admin 1B');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (3,'ADM2A',2,'Admin 2A','Admin 2A');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (4,'ADM2B',2,'Admin 2B','Admin 2B');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (5,'ADM3A',3,'Admin 3A','Admin 3A');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (6,'ADM3B',3,'Admin 3B','Admin 3B');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (7,'ACC1A',4,'Account 1A','Account 1A');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (8,'ACC1B',4,'Account 1B','Account 1B');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (9,'ACC2A',5,'Account 2A','Account 2A');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (10,'ACC2B',5,'Account 2B','Account 2B');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (11,'ACC3A',6,'Account 3A','Account 3A');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (12,'ACC3B',6,'Account 3B','Account 3B');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (13,'IT1A',7,'IT 1A','IT 1A');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (14,'IT1B',7,'IT 1B','IT 1B');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (15,'IT2A',8,'IT 2A','IT 2A');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (16,'IT2B',8,'IT 2B','IT 2B');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (17,'IT3A',9,'IT 3A','IT 3A');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (18,'IT3B',9,'IT 3B','IT 3B');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (19,'LOG1A',10,'LOG 1A','LOG 1A');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (20,'LOG1B',10,'LOG 1B','LOG 1B');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (21,'LOG2A',11,'LOG 2A','LOG 2A');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (22,'LOG2B',11,'LOG 2B','LOG 2B');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (23,'LOG3A',12,'LOG 3A','LOG 3A');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (24,'LOG3B',12,'LOG 3B','LOG 3B');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (25,'RET1A',13,'RET 1A','RET 1A');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (26,'RET1B',13,'RET 1B','RET 1B');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (27,'RET2A',14,'RET 2A','RET 2A');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (28,'RET2B',14,'RET 2B','RET 2B');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (29,'RET3A',15,'RET 3A','RET 3A');
INSERT INTO SubCategories (SubCategoryId,Code,CategoryId,DisplayName,Description) VALUES (30,'RET3B',15,'RET 3B','RET 3B');

---- Users
---- UserId: Primary key
---- LoginName: Unique, only allow [A-Za-z0-9_-.] characters.
---- DisplayName: Only all ASCII characters.
---- https://www.w3schools.com/sql/sql_primarykey.asp
---- https://www.w3schools.com/sql/sql_autoincrement.asp
---- https://www.w3schools.com/sql/sql_unique.asp
CREATE TABLE Users (
UserId int IDENTITY(1,1) NOT NULL PRIMARY KEY,
LoginName varchar(255) NOT NULL UNIQUE,
DisplayName varchar(255) NOT NULL,
Hash varchar(255) NULL,
Password varchar(255) NULL,
DepartmentId int NULL,
CategoryId int NULL,
SubCategoryId int NULL,
Birthday datetime NULL,
RegistrationFee decimal(18,2) NULL,
IsDisabled bit NOT NULL DEFAULT 0,
CreatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
CreatedBy int NOT NULL DEFAULT 0,
UpdatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
UpdatedBy int NOT NULL DEFAULT 0,
Description varchar(1023) NULL
);

---- SQLite
/*
CREATE TABLE Users (
UserId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
LoginName varchar(255) NOT NULL UNIQUE,
DisplayName varchar(255) NOT NULL,
Hash varchar(255) NULL,
Password varchar(255) NULL,
DepartmentId int NULL,
CategoryId int NULL,
SubCategoryId int NULL,
Birthday datetime NULL,
RegistrationFee decimal(18,2) NULL,
IsDisabled bit NOT NULL DEFAULT 0,
CreatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
CreatedBy int NOT NULL DEFAULT 0,
UpdatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
UpdatedBy int NOT NULL DEFAULT 0,
Description varchar(1023) NULL
);
*/

INSERT INTO Users (LoginName,DisplayName,Hash,Password,DepartmentId,IsDisabled,Description) VALUES ('system','System',NULL,'system',3,0,'System');
INSERT INTO Users (LoginName,DisplayName,Hash,Password,DepartmentId,IsDisabled,Description) VALUES ('admin','Administrator',NULL,'admin',3,0,'Administrator');
INSERT INTO Users (LoginName,DisplayName,Hash,Password,DepartmentId,IsDisabled,Description) VALUES ('main','Maintenance Staff',NULL,'main',3,0,'Maintenance Staff');
INSERT INTO Users (LoginName,DisplayName,Hash,Password,DepartmentId,IsDisabled,Description) VALUES ('oper','Operator',NULL,'oper',3,0,'Operator');

---- Roles
CREATE TABLE Roles (
RoleId int IDENTITY(1,1) NOT NULL PRIMARY KEY,
UniqueName varchar(255) NOT NULL UNIQUE,
DisplayName varchar(255) NOT NULL,
IsDisabled bit NOT NULL Default 0,
UpdateVersion int NOT NULL DEFAULT 0,
CreatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
CreatedBy int NOT NULL DEFAULT 0,
UpdatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
UpdatedBy int NOT NULL DEFAULT 0,
Description varchar(1023) NULL
);

---- SQLite
/*
CREATE TABLE Roles (
RoleId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
UniqueName varchar(255) NOT NULL UNIQUE,
DisplayName varchar(255) NOT NULL,
IsDisabled bit NOT NULL Default 0,
CreatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
CreatedBy int NOT NULL DEFAULT 0,
UpdatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
UpdatedBy int NOT NULL DEFAULT 0,
Description varchar(1023) NULL
);
*/

INSERT INTO Roles (UniqueName,DisplayName,IsDisabled,Description) VALUES ('admin','Administration',0,'Administration Role');
INSERT INTO Roles (UniqueName,DisplayName,IsDisabled,Description) VALUES ('main','Maintenance',0,'Maintenance Role');
INSERT INTO Roles (UniqueName,DisplayName,IsDisabled,Description) VALUES ('oper','Operation',0,'Operation Role');

---- Mapping between roles and users
---- https://www.w3schools.com/sql/sql_primarykey.asp
CREATE TABLE MapRolesUsers (
UserId int NOT NULL FOREIGN KEY REFERENCES Users(UserId),
RoleId int NOT NULL FOREIGN KEY REFERENCES Roles(RoleId),
CreatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
CreatedBy int NOT NULL DEFAULT 0,
UpdatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
UpdatedBy int NOT NULL DEFAULT 0,
CONSTRAINT PK_MapRolesUsers PRIMARY KEY (UserId,RoleId)
);

---- SQLite
/*
CREATE TABLE MapRolesUsers (
UserId int NOT NULL,
RoleId int NOT NULL,
CreatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
CreatedBy int NOT NULL DEFAULT 0,
UpdatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
UpdatedBy int NOT NULL DEFAULT 0
);
*/

INSERT INTO MapRolesUsers (UserId,RoleId) SELECT UserId,(SELECT RoleId FROM Roles WHERE UniqueName='admin') FROM Users WHERE LoginName='admin';
INSERT INTO MapRolesUsers (UserId,RoleId) SELECT UserId,(SELECT RoleId FROM Roles WHERE UniqueName='main') FROM Users WHERE LoginName='main';
INSERT INTO MapRolesUsers (UserId,RoleId) SELECT UserId,(SELECT RoleId FROM Roles WHERE UniqueName='oper') FROM Users WHERE LoginName='oper';

---- Show the roles that assigned to a specific user.
SELECT u.UserId,u.LoginName,u.DisplayName,u.IsDisabled,r.RoleId,r.UniqueName,r.DisplayName,r.IsDisabled FROM Users u LEFT JOIN MapRolesUsers m ON u.UserId=m.UserId LEFT JOIN Roles r ON m.RoleId=r.RoleId WHERE u.UserId=2;
SELECT m.UserId,u.LoginName,u.DisplayName,u.IsDisabled,m.RoleId,r.UniqueName,r.DisplayName,r.IsDisabled FROM MapRolesUsers m INNER JOIN Users u ON u.UserId=m.UserId LEFT JOIN Roles r ON r.RoleId=m.RoleId WHERE m.UserId=2;

---- Show if the roles is selected for a specific user.
SELECT r.*, m.UserId, CASE WHEN m.UserId=2 THEN 1 ELSE 0 END AS IsSelected FROM Roles r LEFT JOIN MapRolesUsers m ON m.RoleId=r.RoleId;

---- App function levels
CREATE TABLE AppFuncLevels (
AppFuncLevelId int NOT NULL PRIMARY KEY,
UniqueName varchar(255) NOT NULL UNIQUE,
DisplayName varchar(255) NOT NULL,
Description varchar(1023) NULL
);

INSERT INTO AppFuncLevels (AppFuncLevelId,UniqueName,DisplayName,Description) VALUES (0,'hidden','Level 0','Level 0 menu item, which is hidden.');
INSERT INTO AppFuncLevels (AppFuncLevelId,UniqueName,DisplayName,Description) VALUES (1,'level1','Level 1','Level 1 menu item, which shows on the navigation bar and always visible.');
INSERT INTO AppFuncLevels (AppFuncLevelId,UniqueName,DisplayName,Description) VALUES (2,'level2','Level 2','Level 2 menu item, which is a dropdown item and only visible if selecting its parent level 1 menu item.');
INSERT INTO AppFuncLevels (AppFuncLevelId,UniqueName,DisplayName,Description) VALUES (3,'level3','Level 3','Level 3 menu item, which is a dropdown item and only visible if selecting its parent level 2 menu item.');

---- App functions
---- ParentId:
---- SequentialNum: determine the sequential order of current function.
CREATE TABLE AppFunctions (
AppFunctionId int IDENTITY(1,1) NOT NULL PRIMARY KEY,
UniqueName varchar(255) NOT NULL UNIQUE,
DisplayName varchar(255) NOT NULL,
ControllerName varchar(255) NULL,
ActionName varchar(255) NULL,
AppFuncLevelId int NOT NULL DEFAULT 0 FOREIGN KEY REFERENCES AppFuncLevels(AppFuncLevelId),
ParentId int NOT NULL DEFAULT 0,
SequentialNum int NOT NULL DEFAULT 1001,
IsDisabled bit NOT NULL DEFAULT 0,
IsNavItem bit NOT NULL DEFAULT 1,
CreatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
CreatedBy int NOT NULL DEFAULT 0,
UpdatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
UpdatedBy int NOT NULL DEFAULT 0,
Description varchar(1023) NULL
);

INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) VALUES ('home','Home','Home','Index',1,0,100,1,'Home page');
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) VALUES ('index2','Index2','Home','Index2',1,0,200,1,'Index 2');
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) VALUES ('about','About','Home','About',1,0,300,1,'About page');
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) VALUES ('contact','Contact','Home','Contact',1,0,400,1,'Contact page');
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) VALUES ('student','Student','Student','Index',1,0,500,1,'Student page');

/* <li>@Html.ActionLink("Home", "Index", "Home")</li>
<li>@Html.ActionLink("Index2", "Index2", "Home")</li>
<li>@Html.ActionLink("About", "About", "Home")</li>
<li>@Html.ActionLink("Contact", "Contact", "Home")</li>
<li>@Html.ActionLink("Student", "Index", "Student")</li>
 */
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) VALUES ('admin','Administration',NULL,NULL,1,0,600,1,'Administration menu');
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'user','User','User','Index',2,AppFunctionId,100,1,'User page' FROM AppFunctions WHERE UniqueName='admin';
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'role','Role','Role','Index',2,AppFunctionId,200,1,'Role page' FROM AppFunctions WHERE UniqueName='admin';
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'appfunc','App Function','AppFunction','Index',2,AppFunctionId,300,1,'Function page' FROM AppFunctions WHERE UniqueName='admin';

INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'user_details','View User Details','User','Index',3,AppFunctionId,100,0,'View User Details Function' FROM AppFunctions WHERE UniqueName='user';
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'user_create','Create User','User','Index',3,AppFunctionId,200,0,'Create User Function' FROM AppFunctions WHERE UniqueName='user';
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'user_edit','Edit User','User','Index',3,AppFunctionId,300,0,'Edit User Function' FROM AppFunctions WHERE UniqueName='user';
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'user_delete','Delete User','User','Index',3,AppFunctionId,400,0,'Delete User Function' FROM AppFunctions WHERE UniqueName='user';
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'user_assign_roles','Assign Roles to User','User','Index',3,AppFunctionId,500,0,'Assign Roles to User Function' FROM AppFunctions WHERE UniqueName='user';

INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'role_details','View Role Details','Role','Index',3,AppFunctionId,100,0,'View Role Details Function' FROM AppFunctions WHERE UniqueName='role';
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'role_create','Create Role','Role','Index',3,AppFunctionId,200,0,'Create Role Function' FROM AppFunctions WHERE UniqueName='role';
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'role_edit','Edit Role','Role','Index',3,AppFunctionId,300,0,'Edit Role Function' FROM AppFunctions WHERE UniqueName='role';
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'role_delete','Delete Role','Role','Index',3,AppFunctionId,400,0,'Delete Role Function' FROM AppFunctions WHERE UniqueName='role';
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'role_assign_users','Assign Users to Role','Role','Index',3,AppFunctionId,500,0,'Assign Users to Role Function' FROM AppFunctions WHERE UniqueName='role';
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'role_assign_func','Assign Functions to Role','Role','Index',3,AppFunctionId,600,0,'Assign Functions to Role Function' FROM AppFunctions WHERE UniqueName='role';

INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'appfunc_details','View Function Details','AppFunction','Index',3,AppFunctionId,100,0,'View Function Details Function' FROM AppFunctions WHERE UniqueName='appfunc';
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'appfunc_create','Create Function','AppFunction','Index',3,AppFunctionId,200,0,'Create Function Function' FROM AppFunctions WHERE UniqueName='appfunc';
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'appfunc_edit','Edit Function','AppFunction','Index',3,AppFunctionId,300,0,'Edit Function Function' FROM AppFunctions WHERE UniqueName='appfunc';
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'appfunc_delete','Delete Function','AppFunction','Index',3,AppFunctionId,400,0,'Delete Function Function' FROM AppFunctions WHERE UniqueName='appfunc';



---- testing
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'menu2','Menu 2',NULL,NULL,2,AppFunctionId,250,1,'Menu 2 page' FROM AppFunctions WHERE UniqueName='admin';
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'menu2a','Menu 2A',NULL,NULL,3,AppFunctionId,100,1,'Menu 2A page' FROM AppFunctions WHERE UniqueName='menu2';
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'menu2b','Menu 2B',NULL,NULL,3,AppFunctionId,200,1,'Menu 2B page' FROM AppFunctions WHERE UniqueName='menu2';
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'menu2c','Menu 2C',NULL,NULL,3,AppFunctionId,300,1,'Menu 2C page' FROM AppFunctions WHERE UniqueName='menu2';

INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) VALUES ('test1','Test 1',NULL,NULL,1,0,700,1,'Test 1 menu');
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'test1a','Test 1a',NULL,NULL,2,AppFunctionId,100,1,'Test 1a' FROM AppFunctions WHERE UniqueName='test1';
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'test1b','Test 1b',NULL,NULL,2,AppFunctionId,200,1,'Test 1B' FROM AppFunctions WHERE UniqueName='test1';
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'test1c','Test 1c',NULL,NULL,2,AppFunctionId,300,1,'Test 1c' FROM AppFunctions WHERE UniqueName='test1';
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'test1b1','Test 1b1',NULL,NULL,3,AppFunctionId,100,1,'Test 1B1 page' FROM AppFunctions WHERE UniqueName='test1b';
INSERT INTO AppFunctions (UniqueName,DisplayName,ControllerName,ActionName,AppFuncLevelId,ParentId,SequentialNum,IsNavItem,Description) SELECT 'test1b2','Test 1b2',NULL,NULL,3,AppFunctionId,200,1,'Test 1B2 page' FROM AppFunctions WHERE UniqueName='test1b';

--------------------------------------------------
SELECT AppFunctionId,UniqueName,DisplayName,ActionName,ControllerName FROM AppFunctions WHERE AppFuncLevelId=1 ORDER BY SequentialNum;
SELECT AppFunctionId,UniqueName,DisplayName,ActionName,ControllerName FROM AppFunctions WHERE AppFuncLevelId=2 AND ParentId=1 ORDER BY SequentialNum;
SELECT AppFunctionId,UniqueName,DisplayName,ActionName,ControllerName FROM AppFunctions WHERE AppFuncLevelId=2 AND ParentId=6 ORDER BY SequentialNum;
SELECT AppFunctionId,UniqueName,DisplayName,ActionName,ControllerName FROM AppFunctions WHERE AppFuncLevelId=3 AND ParentId=10 ORDER BY SequentialNum;

---- Show the tree of functions.
SELECT L1.AppFunctionId,L1.UniqueName,L1.AppFuncLevelId,L1.ParentId,
L2.AppFunctionId,L2.UniqueName,L2.AppFuncLevelId,L2.ParentId,
L3.AppFunctionId,L3.UniqueName,L3.AppFuncLevelId,L3.ParentId
--SELECT * 
FROM AppFunctions L1
LEFT JOIN AppFunctions L2 ON L1.AppFunctionId=L2.ParentId
LEFT JOIN AppFunctions L3 ON L2.AppFunctionId=L3.ParentId
WHERE L1.AppFunctionId=6;

---- Show the tree of functions in the upper level.
SELECT AppFunctionId,UniqueName,AppFuncLevelId,ParentId FROM AppFunctions WHERE AppFunctionId=6
UNION
SELECT AppFunctionId,UniqueName,AppFuncLevelId,ParentId FROM AppFunctions
WHERE ParentId IN (SELECT AppFunctionId FROM AppFunctions WHERE AppFunctionId=6)
--AND AppFuncLevelId - 1=(SELECT AppFuncLevelId FROM AppFunctions WHERE AppFunctionId=6)
UNION
SELECT AppFunctionId,UniqueName,AppFuncLevelId,ParentId FROM AppFunctions
WHERE ParentId IN (SELECT AppFunctionId FROM AppFunctions WHERE ParentId IN (SELECT AppFunctionId FROM AppFunctions WHERE AppFunctionId=6))
--AND AppFuncLevelId - 2=(SELECT AppFuncLevelId FROM AppFunctions WHERE AppFunctionId=6)
UNION
SELECT AppFunctionId,UniqueName,AppFuncLevelId,ParentId FROM AppFunctions
WHERE AppFunctionId IN (SELECT ParentId FROM AppFunctions WHERE AppFunctionId=6)
UNION
SELECT AppFunctionId,UniqueName,AppFuncLevelId,ParentId FROM AppFunctions
WHERE AppFunctionId IN (SELECT ParentId FROM AppFunctions WHERE AppFunctionId IN (SELECT ParentId FROM AppFunctions WHERE AppFunctionId=6))
;

---- Show the tree of functions in the lower level.
SELECT AppFunctionId,UniqueName,AppFuncLevelId,ParentId FROM AppFunctions WHERE AppFunctionId=12
UNION
SELECT AppFunctionId,UniqueName,AppFuncLevelId,ParentId FROM AppFunctions
WHERE ParentId IN (SELECT AppFunctionId FROM AppFunctions WHERE AppFunctionId=12)
UNION
SELECT AppFunctionId,UniqueName,AppFuncLevelId,ParentId FROM AppFunctions
WHERE ParentId IN (SELECT AppFunctionId FROM AppFunctions WHERE ParentId IN (SELECT AppFunctionId FROM AppFunctions WHERE AppFunctionId=12))
UNION
SELECT AppFunctionId,UniqueName,AppFuncLevelId,ParentId FROM AppFunctions
WHERE AppFunctionId IN (SELECT ParentId FROM AppFunctions WHERE AppFunctionId=12)
UNION
SELECT AppFunctionId,UniqueName,AppFuncLevelId,ParentId FROM AppFunctions
WHERE AppFunctionId IN (SELECT ParentId FROM AppFunctions WHERE AppFunctionId IN (SELECT ParentId FROM AppFunctions WHERE AppFunctionId=12))
;
--------------------------------------------------

---- Mapping between AppFunctions and Roles
CREATE TABLE MapAppFunctionsRoles (
RoleId int NOT NULL FOREIGN KEY REFERENCES Roles(RoleId),
AppFunctionId int NOT NULL FOREIGN KEY REFERENCES AppFunctions(AppFunctionId),
CreatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
CreatedBy int NOT NULL DEFAULT 0,
UpdatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
UpdatedBy int NOT NULL DEFAULT 0,
CONSTRAINT PK_MapAppFunctionsRoles PRIMARY KEY (RoleId,AppFunctionId)
);

INSERT INTO MapAppFunctionsRoles (RoleId,AppFunctionId) SELECT (SELECT RoleId FROM Roles WHERE UniqueName='admin'),AppFunctionId FROM AppFunctions;
INSERT INTO MapAppFunctionsRoles (RoleId,AppFunctionId)
SELECT (SELECT RoleId FROM Roles WHERE UniqueName='oper'),AppFunctionId FROM AppFunctions WHERE UniqueName='home'
UNION
SELECT (SELECT RoleId FROM Roles WHERE UniqueName='oper'),AppFunctionId FROM AppFunctions WHERE UniqueName='menu2b'
UNION
SELECT (SELECT RoleId FROM Roles WHERE UniqueName='oper'),AppFunctionId FROM AppFunctions
WHERE ParentId IN (SELECT AppFunctionId FROM AppFunctions WHERE UniqueName='menu2b')
UNION
SELECT (SELECT RoleId FROM Roles WHERE UniqueName='oper'),AppFunctionId FROM AppFunctions
WHERE ParentId IN (SELECT AppFunctionId FROM AppFunctions WHERE ParentId IN (SELECT AppFunctionId FROM AppFunctions WHERE UniqueName='menu2b'))
UNION
SELECT (SELECT RoleId FROM Roles WHERE UniqueName='oper'),AppFunctionId FROM AppFunctions
WHERE AppFunctionId IN (SELECT ParentId FROM AppFunctions WHERE AppFunctionId=12)
UNION
SELECT (SELECT RoleId FROM Roles WHERE UniqueName='oper'),AppFunctionId FROM AppFunctions
WHERE AppFunctionId IN (SELECT ParentId FROM AppFunctions WHERE AppFunctionId IN (SELECT ParentId FROM AppFunctions WHERE UniqueName='menu2b'))
;
INSERT INTO MapAppFunctionsRoles (RoleId,AppFunctionId) SELECT (SELECT RoleId FROM Roles WHERE UniqueName='main'),AppFunctionId FROM AppFunctions WHERE UniqueName='home';
INSERT INTO MapAppFunctionsRoles (RoleId,AppFunctionId) SELECT (SELECT RoleId FROM Roles WHERE UniqueName='main'),AppFunctionId FROM AppFunctions WHERE UniqueName='about';

---- Show all functions for a role.
SELECT AppFunctionId,UniqueName,AppFuncLevelId,ParentId FROM AppFunctions
WHERE AppFunctionId IN (SELECT AppFunctionId FROM MapAppFunctionsRoles WHERE RoleId=(SELECT RoleId FROM Roles WHERE UniqueName='admin'));

---- Show all funtions for a user.
SELECT AppFunctionId,UniqueName,AppFuncLevelId,ParentId FROM AppFunctions 
WHERE AppFunctionId IN (SELECT AppFunctionId FROM MapAppFunctionsRoles WHERE RoleId IN (SELECT RoleId FROM MapRolesUsers WHERE UserId=2));

---- Check if the current function is accessible for the current user.
SELECT 1 FROM MapAppFunctionsRoles fr 
WHERE AppFunctionId=(SELECT AppFunctionId FROM AppFunctions WHERE UniqueName='index2')
AND fr.RoleId IN (SELECT RoleId FROM MapRolesUsers WHERE UserId IN (SELECT UserId FROM Users WHERE LoginName='admin'));

SELECT 1 FROM MapAppFunctionsRoles fr 
INNER JOIN AppFunctions f ON f.AppFunctionId=fr.AppFunctionId 
INNER JOIN MapRolesUsers ru ON ru.RoleId=fr.RoleId 
WHERE f.UniqueName='index2' AND ru.UserId=2;

SELECT 1 FROM MapAppFunctionsRoles fr 
INNER JOIN AppFunctions f ON f.AppFunctionId=fr.AppFunctionId 
INNER JOIN MapRolesUsers ru ON ru.RoleId=fr.RoleId 
INNER JOIN Users u ON u.UserId=ru.UserId 
WHERE f.UniqueName='index2' AND u.LoginName='admin';
--------------------------------------------------
CREATE TABLE ProductTypes(
ProductTypeId int IDENTITY(1,1) NOT NULL PRIMARY KEY,
ProductTypeName varchar(255) NOT NULL,
IsEnabled bit NOT NULL DEFAULT 1,
CreatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
CreatedBy int NOT NULL DEFAULT 0,
UpdatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
UpdatedBy int NOT NULL DEFAULT 0
);
/*
---- SQLite
CREATE TABLE ProductTypes(
ProductTypeId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
ProductTypeName varchar(255) NOT NULL,
IsEnabled bit NOT NULL DEFAULT 1,
CreatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
CreatedBy int NOT NULL DEFAULT 0,
UpdatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
UpdatedBy int NOT NULL DEFAULT 0
);
*/

INSERT INTO ProductTypes (ProductTypeId,ProductTypeName) VALUES (1,'Clothes');
INSERT INTO ProductTypes (ProductTypeId,ProductTypeName) VALUES (2,'Pants');
INSERT INTO ProductTypes (ProductTypeId,ProductTypeName) VALUES (3,'Socks');
INSERT INTO ProductTypes (ProductTypeId,ProductTypeName) VALUES (4,'Footwear');
INSERT INTO ProductTypes (ProductTypeId,ProductTypeName) VALUES (5,'Glasses');

CREATE TABLE Products(
ProductId int IDENTITY(1,1) NOT NULL PRIMARY KEY,
Code varchar(255) NOT NULL,
Name varchar(255) NOT NULL,
Description varchar(255) NULL,
ProductTypeId int NOT NULL,
Price decimal(18,2) NULL,
Price2 decimal(18,2) NULL,
DiscountRate decimal(18,2) NULL,
Discount decimal(18,2) NULL,
IsEnabled bit NOT NULL DEFAULT 1,
Version int NOT NULL,
CreatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
CreatedBy int NOT NULL DEFAULT 0,
UpdatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
UpdatedBy int NOT NULL DEFAULT 0
);
/*
---- SQLite
CREATE TABLE Products(
ProductId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
Code varchar(255) NOT NULL,
Name varchar(255) NOT NULL,
Description varchar(255) NULL,
ProductTypeId int NOT NULL,
Price decimal(18,2) NULL,
Price2 decimal(18,2) NULL,
DiscountRate decimal(18,2) NULL,
Discount decimal(18,2) NULL,
IsEnabled bit NOT NULL DEFAULT 1,
Version int NOT NULL,
CreatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
CreatedBy int NOT NULL DEFAULT 0,
UpdatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
UpdatedBy int NOT NULL DEFAULT 0
);
*/

CREATE TABLE Vendors(
VendorId int IDENTITY(1,1) NOT NULL PRIMARY KEY,
Name varchar(255) NOT NULL,
Description varchar(255) NOT NULL,
IsDisabled bit NOT NULL DEFAULT 0,
Version int NOT NULL,
CreatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
CreatedBy int NOT NULL DEFAULT 0,
UpdatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
UpdatedBy int NOT NULL DEFAULT 0
);
/*
---- SQLite
CREATE TABLE Vendors(
VendorId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
Name varchar(255) NOT NULL,
Description varchar(255) NOT NULL,
IsDisabled bit NOT NULL DEFAULT 0,
Version int NOT NULL,
CreatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
CreatedBy int NOT NULL DEFAULT 0,
UpdatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
UpdatedBy int NOT NULL DEFAULT 0
);
*/
--------------------------------------------------
--------------------------------------------------
--------------------------------------------------
--------------------------------------------------

