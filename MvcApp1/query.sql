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

CREATE TABLE Students (
StudentId int IDENTITY(1,1) NOT NULL PRIMARY KEY,
StudentName varchar(255) NOT NULL,
Age int NOT NULL,
Email varchar(255) NULL
);

INSERT INTO Students (StudentName,Age,Email) VALUES ('John',18,'john@a.com');
INSERT INTO Students (StudentName,Age,Email) VALUES ('Steve',21,'steve@a.com');
INSERT INTO Students (StudentName,Age,Email) VALUES ('Bill',25,'bill@a.com');

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
IsDisabled bit NOT NULL DEFAULT 0,
CreatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
CreatedBy int NOT NULL DEFAULT 0,
UpdatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
UpdatedBy int NOT NULL DEFAULT 0,
Description varchar(1023) NULL
);

INSERT INTO Users (LoginName,DisplayName,Hash,Password,IsDisabled,Description) VALUES ('system','System',NULL,'system',0,'System');
INSERT INTO Users (LoginName,DisplayName,Hash,Password,IsDisabled,Description) VALUES ('admin','Administrator',NULL,'admin',0,'Administrator');
INSERT INTO Users (LoginName,DisplayName,Hash,Password,IsDisabled,Description) VALUES ('main','Maintenance Staff',NULL,'main',0,'Maintenance Staff');
INSERT INTO Users (LoginName,DisplayName,Hash,Password,IsDisabled,Description) VALUES ('oper','Operator',NULL,'oper',0,'Operator');

---- Roles
CREATE TABLE Roles (
RoleId int IDENTITY(1,1) NOT NULL PRIMARY KEY,
UniqueName varchar(255) NOT NULL UNIQUE,
DisplayName varchar(255) NOT NULL,
IsDisabled bit NOT NULL Default 0,
CreatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
CreatedBy int NOT NULL DEFAULT 0,
UpdatedDt datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
UpdatedBy int NOT NULL DEFAULT 0,
Description varchar(1023) NULL
);

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
--------------------------------------------------
--------------------------------------------------
--------------------------------------------------
--------------------------------------------------

