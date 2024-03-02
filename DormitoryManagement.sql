﻿CREATE DATABASE DormitoryManagement
use DormitoryManagement
CREATE TABLE AdminAccounts (
    AdminID INT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(50) UNIQUE NOT NULL,
    Password VARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    FullName VARCHAR(100) NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
	LoginAttempts INT DEFAULT 0,
    IsLocked INT DEFAULT 0
);


INSERT INTO AdminAccounts (Username, Password, Email, FullName)
VALUES ('admin', '123456', 'admin@example.com', 'Admin User');

CREATE TABLE Buildings (
    BuildingID INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL,
	Descrip NVARCHAR(255),
);

INSERT INTO Buildings (Name)
VALUES ('Tòa A'),
       ('Tòa B'),
       ('Tòa C');

CREATE TABLE Rooms (
    RoomID INT PRIMARY KEY IDENTITY,
    BuildingID INT FOREIGN KEY REFERENCES Buildings(BuildingID),
    Name NVARCHAR(100) NOT NULL,
    MaLoaiPhong INT FOREIGN KEY REFERENCES LoaiPhong(MaLoaiPhong), 
    MaxCapacity INT NOT NULL,
	Status NVARCHAR(100) Not Null,
	Occupancy INT DEFAULT 0, 
    Gender NVARCHAR(100) Not Null,
	Descript NVARCHAR(100),
);


INSERT INTO Rooms (BuildingID, Name, MaxCapacity, MaLoaiPhong, Status,Occupancy,Gender)
VALUES
    (1, '101', 2, '1',N'Còn trống' ,0,N'Nam'), -- Phòng 101 trong tòa nhà có ID là 1, có sức chứa 2 người, loại phòng Standard, hiện chưa có người ở
    (1, '102', 3, '2', N'Còn trống' ,1,N'Nữ'),   -- Phòng 102 trong tòa nhà có ID là 1, có sức chứa 3 người, loại phòng Deluxe, hiện có 1 người ở
    (2, '201', 2, '1',N'Còn trống' ,0,N'Nam'), -- Phòng 201 trong tòa nhà có ID là 2, có sức chứa 2 người, loại phòng Standard, hiện chưa có người ở
    (2, '202', 4, '2', N'Còn trống',2,N'Nữ');  

CREATE TABLE LoaiPhong (
    MaLoaiPhong INT PRIMARY KEY IDENTITY,
    TenLoaiPhong NVARCHAR(100) NOT NULL,
    GiaTien DECIMAL(18, 2) NOT NULL
);

INSERT INTO LoaiPhong (TenLoaiPhong, GiaTien)
VALUES 
    (N'Phòng Máy Lạnh', 100.00),
    (N'Phòng Thường', 150.00),
    (N'Phòng Vip', 200.00);


CREATE TABLE StudentAccounts (
    StudentID INT IDENTITY(1,1) PRIMARY KEY,
    UserName VARCHAR(50) UNIQUE NOT NULL,
	FullName VARCHAR(100) NOT NULL,
    Password VARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL, 
	PhoneNumber VARCHAR(100) NOT NULL, 
	Address VARCHAR(100), 
	Gender VARCHAR(100) NOT NULL, 
	ImagePath VARCHAR(100),
	RoomID INT FOREIGN KEY REFERENCES Rooms(RoomId),
	BuildingID INT FOREIGN KEY REFERENCES Buildings(BuildingID),
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
	LoginAttempts INT DEFAULT 0,
    IsLocked INT DEFAULT 0
);

INSERT INTO StudentAccounts (UserName, FullName, Password, Email, PhoneNumber, Gender)
VALUES 
('user1', 'User One', 'password1', 'user1@example.com', '1234567890', 'Male'),
('user2', 'User Two', 'password2', 'user2@example.com', '9876543210', 'Female'),
('user3', 'User Three', 'password3', 'user3@example.com', '4561237890', 'Male'),
('user4', 'User Four', 'password4', 'user4@example.com', '7894561230', 'Female'),
('user5', 'User Five', 'password5', 'user5@example.com', '3216549870', 'Male'),
('user6', 'User Six', 'password6', 'user6@example.com', '6543217890', 'Female'),
('user7', 'User Seven', 'password7', 'user7@example.com', '9871236540', 'Male'),
('user8', 'User Eight', 'password8', 'user8@example.com', '4567891230', 'Female'),
('user9', 'User Nine', 'password9', 'user9@example.com', '3219876540', 'Male'),
('user10', 'User Ten', 'password10', 'user10@example.com', '7893216540', 'Female');