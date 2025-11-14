-- =============================================
-- ABC Retail App - Azure SQL Database Schema
-- Student: st10449316 (Mpumelelo Chonco)
-- Purpose: POE Part 3 - Azure SQL Database Implementation
-- =============================================

-- Create Database (Run this on master database)
-- CREATE DATABASE ABCRetailDB;
-- GO

USE ABCRetailDB;
GO

-- =============================================
-- Drop existing tables if they exist (for clean setup)
-- =============================================
IF OBJECT_ID('dbo.Orders', 'U') IS NOT NULL DROP TABLE dbo.Orders;
IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL DROP TABLE dbo.Products;
IF OBJECT_ID('dbo.Customers', 'U') IS NOT NULL DROP TABLE dbo.Customers;
GO

-- =============================================
-- Create Customers Table
-- =============================================
CREATE TABLE dbo.Customers (
    CustomerId INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Phone NVARCHAR(20) NULL,
    Address NVARCHAR(200) NOT NULL,
    DateCreated DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    LastModified DATETIME2 NULL,
    
    CONSTRAINT CK_Customers_Email CHECK (Email LIKE '%@%.%')
);
GO

-- Create index on Email for faster lookups
CREATE INDEX IX_Customers_Email ON dbo.Customers(Email);
CREATE INDEX IX_Customers_LastName ON dbo.Customers(LastName, FirstName);
GO

-- =============================================
-- Create Products Table
-- =============================================
CREATE TABLE dbo.Products (
    ProductId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    StockQuantity INT NOT NULL DEFAULT 0,
    ImageUrl NVARCHAR(500) NULL,
    DateCreated DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    LastModified DATETIME2 NULL,
    
    CONSTRAINT CK_Products_Price CHECK (Price >= 0),
    CONSTRAINT CK_Products_StockQuantity CHECK (StockQuantity >= 0)
);
GO

-- Create indexes for common queries
CREATE INDEX IX_Products_Category ON dbo.Products(Category);
CREATE INDEX IX_Products_Name ON dbo.Products(Name);
CREATE INDEX IX_Products_StockQuantity ON dbo.Products(StockQuantity);
GO

-- =============================================
-- Create Orders Table
-- =============================================
CREATE TABLE dbo.Orders (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    Notes NVARCHAR(1000) NULL,
    Status NVARCHAR(30) NOT NULL DEFAULT 'New',
    DateCreated DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    LastModified DATETIME2 NULL,
    
    CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerId) 
        REFERENCES dbo.Customers(CustomerId),
    CONSTRAINT FK_Orders_Products FOREIGN KEY (ProductId) 
        REFERENCES dbo.Products(ProductId),
    CONSTRAINT CK_Orders_Quantity CHECK (Quantity > 0),
    CONSTRAINT CK_Orders_UnitPrice CHECK (UnitPrice >= 0),
    CONSTRAINT CK_Orders_TotalAmount CHECK (TotalAmount >= 0),
    CONSTRAINT CK_Orders_Status CHECK (Status IN ('New', 'Processing', 'Completed', 'Cancelled'))
);
GO

-- Create indexes for foreign keys and common queries
CREATE INDEX IX_Orders_CustomerId ON dbo.Orders(CustomerId);
CREATE INDEX IX_Orders_ProductId ON dbo.Orders(ProductId);
CREATE INDEX IX_Orders_DateCreated ON dbo.Orders(DateCreated DESC);
CREATE INDEX IX_Orders_Status ON dbo.Orders(Status);
GO

-- =============================================
-- Insert Sample Data
-- =============================================

-- Insert Sample Customers
INSERT INTO dbo.Customers (FirstName, LastName, Email, Phone, Address, DateCreated)
VALUES 
    ('John', 'Doe', 'john.doe@example.com', '0821234567', '123 Main St, Johannesburg, 2000', GETUTCDATE()),
    ('Jane', 'Smith', 'jane.smith@example.com', '0827654321', '456 Oak Ave, Cape Town, 8001', GETUTCDATE()),
    ('Michael', 'Johnson', 'michael.j@example.com', '0823456789', '789 Pine Rd, Durban, 4001', GETUTCDATE()),
    ('Sarah', 'Williams', 'sarah.w@example.com', '0829876543', '321 Elm St, Pretoria, 0001', GETUTCDATE());
GO

-- Insert Sample Products
INSERT INTO dbo.Products (Name, Description, Price, Category, StockQuantity, DateCreated)
VALUES 
    ('Laptop', 'High-performance laptop for professionals', 15999.99, 'Electronics', 50, GETUTCDATE()),
    ('Office Chair', 'Ergonomic office chair with lumbar support', 2499.99, 'Furniture', 100, GETUTCDATE()),
    ('Wireless Mouse', 'Bluetooth wireless mouse with precision tracking', 299.99, 'Electronics', 200, GETUTCDATE()),
    ('Standing Desk', 'Adjustable height standing desk', 4999.99, 'Furniture', 30, GETUTCDATE()),
    ('Monitor 27"', '4K Ultra HD monitor with HDR support', 5499.99, 'Electronics', 75, GETUTCDATE()),
    ('Keyboard', 'Mechanical keyboard with RGB lighting', 1299.99, 'Electronics', 150, GETUTCDATE());
GO

-- Insert Sample Orders
INSERT INTO dbo.Orders (CustomerId, ProductId, Quantity, UnitPrice, TotalAmount, Status, DateCreated)
VALUES 
    (1, 1, 1, 15999.99, 15999.99, 'Completed', GETUTCDATE()),
    (1, 3, 2, 299.99, 599.98, 'Completed', GETUTCDATE()),
    (2, 2, 1, 2499.99, 2499.99, 'Processing', GETUTCDATE()),
    (3, 5, 1, 5499.99, 5499.99, 'New', GETUTCDATE()),
    (4, 4, 1, 4999.99, 4999.99, 'Completed', GETUTCDATE());
GO

-- =============================================
-- Create Views for Reporting
-- =============================================

-- View: Customer Order Summary
CREATE VIEW vw_CustomerOrderSummary AS
SELECT 
    c.CustomerId,
    c.FirstName + ' ' + c.LastName AS CustomerName,
    c.Email,
    COUNT(o.OrderId) AS TotalOrders,
    SUM(o.TotalAmount) AS TotalSpent,
    MAX(o.DateCreated) AS LastOrderDate
FROM dbo.Customers c
LEFT JOIN dbo.Orders o ON c.CustomerId = o.CustomerId
GROUP BY c.CustomerId, c.FirstName, c.LastName, c.Email;
GO

PRINT 'Database schema created successfully!';
GO

