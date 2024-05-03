create database contact_manager_db;
go

use contact_manager_db;
go

create table Categories (
  Id int identity(1, 1) primary key,
  Name varchar(100) not null,
  UserId int foreign key references Users(Id) not null,
);

create table Users (
  Email varchar(100) not null,
  Id int identity(1, 1) primary key,
  Username varchar(100) not null,
  constraint UNQ_Email unique (Email),
  constraint UNQ_Username unique (Username)
);

create table Contacts (
  CategoryId int foreign key references Categories(Id) null,
  DateOfBirth date null,
  Email varchar(100) null,
  FirstName varchar(100) null,
  Id int identity(1, 1) primary key,
  LastName varchar(100) null,
  Note varchar(200) null,
  PhoneNumber varchar(200) not null,
  UserId int foreign key references Users(Id) not null,
);

insert into Users ("Email", "Username") values 
	('peter@gmx.at', 'peter05');

insert into Categories ("Name", "UserId") values 
	--('All',	(select "Id" from "Users" where "Username" = 'peter05')), 
	('Friends', (select "Id" from "Users" where "Username" = 'peter05')), 
	('Family',	(select "Id" from "Users" where "Username" = 'peter05')), 
	('Work',	(select "Id" from "Users" where "Username" = 'peter05'));

insert into Contacts ("FirstName", "LastName", "PhoneNumber", "CategoryId", "UserId") values 
	('Max',		'Mustermann',	'+4912345678',	null, (select "Id" from "Users" where "Username" = 'peter05')),
	('Nadya',	'Musterfrau',	'+4311122233',	(select "Id" from "Categories" where "Name" = 'Family'), (select "Id" from "Users" where "Username" = 'peter05')),
	('Peter',	'Mustermann',	'+4355566688',	(select "Id" from "Categories" where "Name" = 'Family'), (select "Id" from "Users" where "Username" = 'peter05'));
