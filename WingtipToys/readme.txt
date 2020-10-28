WingtipToys
--------------------------------------------------
It is a sample program of an ASP.NET Web Forms application.
--------------------------------------------------
Getting Started with ASP.NET 4.5 Web Forms and Visual Studio 2017
https://docs.microsoft.com/en-us/aspnet/web-forms/overview/getting-started/getting-started-with-aspnet-45-web-forms/introduction-and-overview
ASP.NET WEB FORMS
https://www.tutorialandexample.com/asp-net-web-forms/
ASP.NET Web Forms Tutorial
https://asp.net-tutorials.com/
Creating A Simple Registration Form In ASP.NET
https://www.c-sharpcorner.com/blogs/creating-a-simple-registration-form-in-asp-net
--------------------------------------------------
IMPORTANT!!!
SQLite with C#.Net and Entity Framework
https://www.codeproject.com/Articles/1158937/SQLite-with-Csharp-Net-and-Entity-Framework
https://stackoverflow.com/questions/28382421/retrieving-data-using-linq
https://vijayt.com/post/using-sqlite-database-in-net-with-linq-to-sql-and-entity-framework-6/
--------------------------------------------------
--------------------------------------------------
CREATE TABLE Category (
CategoryID INTEGER PRIMARY KEY NOT NULL,
CategoryName varchar(100) NOT NULL,
Description varchar(1023) NULL
);

INSERT INTO Category (CategoryID,CategoryName) VALUES (1,'Cars');
INSERT INTO Category (CategoryID,CategoryName) VALUES (2,'Planes');
INSERT INTO Category (CategoryID,CategoryName) VALUES (3,'Trucks');
INSERT INTO Category (CategoryID,CategoryName) VALUES (4,'Boats');
INSERT INTO Category (CategoryID,CategoryName) VALUES (5,'Rockets');

CREATE TABLE Product (
ProductID INTEGER PRIMARY KEY NOT NULL,
ProductName varchar(100) NOT NULL,
Description varchar(10000) NOT NULL,
ImagePath varchar(100) NULL,
UnitPrice decimal NULL,
CategoryID INTEGER NULL
);

INSERT INTO Product (ProductID,ProductName,Description,ImagePath,UnitPrice,CategoryID) VALUES (1,'Convertible Car','This convertible car is fast! The engine is powered by a neutrino based battery (not included). Power it up and let it go!','carconvert.png',22.5,1);
INSERT INTO Product (ProductID,ProductName,Description,ImagePath,UnitPrice,CategoryID) VALUES (2,'Old-time Car','There's nothing old about this toy car, except it's looks. Compatible with other old toy cars.','carearly.png',15.95,1);
INSERT INTO Product (ProductID,ProductName,Description,ImagePath,UnitPrice,CategoryID) VALUES ();
INSERT INTO Product (ProductID,ProductName,Description,ImagePath,UnitPrice,CategoryID) VALUES ();
INSERT INTO Product (ProductID,ProductName,Description,ImagePath,UnitPrice,CategoryID) VALUES ();
INSERT INTO Product (ProductID,ProductName,Description,ImagePath,UnitPrice,CategoryID) VALUES ();
INSERT INTO Product (ProductID,ProductName,Description,ImagePath,UnitPrice,CategoryID) VALUES ();
INSERT INTO Product (ProductID,ProductName,Description,ImagePath,UnitPrice,CategoryID) VALUES ();
INSERT INTO Product (ProductID,ProductName,Description,ImagePath,UnitPrice,CategoryID) VALUES ();

CREATE TABLE CartItem (
ItemId varchar(100) NOT NULL,
CartId varchar(100) NULL,
Quantity INTEGER NOT NULL DEFAULT 0,
DateCreated datetime NOT NULL,
ProductId INTEGER NOT NULL
);

CREATE TABLE OrderHeader (
OrderHeaderId INTEGER PRIMARY KEY NOT NULL,
OrderDate datetime NOT NULL,
Username varchar(100) NULL,
FirstName varchar(160) NULL,
LastName varchar(160) NULL,
Address varchar(70) NULL,
City varchar(40) NULL,
State varchar(40) NULL,
PostalCode varchar(10) NULL,
Country varchar(40) NULL,
Phone varchar(24) NULL,
Email varchar(200) NULL,
Total decimal NOT NULL DEFAULT 0,
PaymentTransactionId varchar(200) NULL,
HasBeenShipped bit NOT NULL DEFAULT 0
);

CREATE TABLE OrderDetail (
OrderDetailId INTEGER PRIMARY KEY NOT NULL,
OrderHeaderId INTEGER NOT NULL,
Username varchar(100) NULL,
ProductId INTEGER NOT NULL,
Quantity INTEGER NOT NULL DEFAULT 0,
UnitPrice decimal NULL
);
