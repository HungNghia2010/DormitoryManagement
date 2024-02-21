﻿CREATE DATABASE DormitoryManagement

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
    RoomType NVARCHAR(100) NOT NULL, 
    MaxCapacity INT NOT NULL,
	Status NVARCHAR(100) Not Null,
	Occupancy INT DEFAULT 0, 
    Gender Nvarchar(50) Not Null,
	Descript Nvarchar(50),
);

INSERT INTO Rooms (BuildingID, Name, MaxCapacity, RoomType, Status,Occupancy,Gender)
VALUES
    (1, '101', 2, 'Standard',N'Còn trống' ,0,N'Nam'), -- Phòng 101 trong tòa nhà có ID là 1, có sức chứa 2 người, loại phòng Standard, hiện chưa có người ở
    (1, '102', 3, 'Deluxe', N'Còn trống' ,1,N'Nữ'),   -- Phòng 102 trong tòa nhà có ID là 1, có sức chứa 3 người, loại phòng Deluxe, hiện có 1 người ở
    (2, '201', 2, 'Standard',N'Còn trống' ,0,N'Nam'), -- Phòng 201 trong tòa nhà có ID là 2, có sức chứa 2 người, loại phòng Standard, hiện chưa có người ở
    (2, '202', 4, 'Suite', N'Còn trống',2,N'Nữ');  

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