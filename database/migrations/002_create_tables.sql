USE CustomerSupportDb;

DROP TABLE IF EXISTS Returns;
DROP TABLE IF EXISTS Shipments;
DROP TABLE IF EXISTS Payments;
DROP TABLE IF EXISTS OrderItems;
DROP TABLE IF EXISTS Orders;
DROP TABLE IF EXISTS Products;
DROP TABLE IF EXISTS Addresses;
DROP TABLE IF EXISTS Customers;

CREATE TABLE Customers (
    CustomerId INT PRIMARY KEY,
    CustomerCode VARCHAR(30) NOT NULL,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Phone VARCHAR(25),
    DateOfBirth DATE,
    Gender VARCHAR(20),
    CustomerType VARCHAR(30),
    LoyaltyPoints INT DEFAULT 0,
    AccountStatus VARCHAR(30),
    PreferredContactMethod VARCHAR(30),
    CreatedAt DATETIME,
    UpdatedAt DATETIME,
    IsActive BOOLEAN DEFAULT TRUE
);

CREATE TABLE Addresses (
    AddressId INT PRIMARY KEY,
    CustomerId INT NOT NULL,
    AddressType VARCHAR(30),
    Line1 VARCHAR(150),
    Line2 VARCHAR(150),
    City VARCHAR(80),
    State VARCHAR(80),
    PostalCode VARCHAR(20),
    Country VARCHAR(80),
    IsDefaultShipping BOOLEAN,
    IsDefaultBilling BOOLEAN,
    DeliveryInstructions VARCHAR(255),
    CreatedAt DATETIME,
    UpdatedAt DATETIME,
    IsActive BOOLEAN DEFAULT TRUE,
    FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId)
);

CREATE TABLE Products (
    ProductId INT PRIMARY KEY,
    ProductCode VARCHAR(30),
    ProductName VARCHAR(100),
    Category VARCHAR(50),
    Brand VARCHAR(50),
    UnitPrice DECIMAL(10,2),
    CostPrice DECIMAL(10,2),
    StockQuantity INT,
    ReorderLevel INT,
    ProductStatus VARCHAR(30),
    WeightKg DECIMAL(10,2),
    SupplierName VARCHAR(100),
    CreatedAt DATETIME,
    UpdatedAt DATETIME,
    IsActive BOOLEAN DEFAULT TRUE
);

CREATE TABLE Orders (
    OrderId INT PRIMARY KEY,
    CustomerId INT NOT NULL,
    ShippingAddressId INT,
    BillingAddressId INT,
    OrderNumber VARCHAR(50),
    OrderDate DATETIME,
    OrderStatus VARCHAR(50),
    PaymentStatus VARCHAR(50),
    ShippingStatus VARCHAR(50),
    SubTotal DECIMAL(10,2),
    TaxAmount DECIMAL(10,2),
    DiscountAmount DECIMAL(10,2),
    TotalAmount DECIMAL(10,2),
    CreatedAt DATETIME,
    UpdatedAt DATETIME,
    FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId),
    FOREIGN KEY (ShippingAddressId) REFERENCES Addresses(AddressId),
    FOREIGN KEY (BillingAddressId) REFERENCES Addresses(AddressId)
);

CREATE TABLE OrderItems (
    OrderItemId INT PRIMARY KEY,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    ProductNameSnapshot VARCHAR(100),
    Quantity INT,
    UnitPrice DECIMAL(10,2),
    DiscountAmount DECIMAL(10,2),
    TaxAmount DECIMAL(10,2),
    LineTotal DECIMAL(10,2),
    ItemStatus VARCHAR(50),
    IsGift BOOLEAN,
    GiftMessage VARCHAR(255),
    CreatedAt DATETIME,
    UpdatedAt DATETIME,
    IsActive BOOLEAN DEFAULT TRUE,
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);

CREATE TABLE Payments (
    PaymentId INT PRIMARY KEY,
    OrderId INT NOT NULL,
    PaymentReference VARCHAR(80),
    PaymentMethod VARCHAR(50),
    PaymentStatus VARCHAR(50),
    Amount DECIMAL(10,2),
    Currency VARCHAR(10),
    TransactionId VARCHAR(100),
    GatewayName VARCHAR(50),
    PaymentDate DATETIME,
    FailureReason VARCHAR(255),
    RefundStatus VARCHAR(50),
    CreatedAt DATETIME,
    UpdatedAt DATETIME,
    IsActive BOOLEAN DEFAULT TRUE,
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId)
);

CREATE TABLE Shipments (
    ShipmentId INT PRIMARY KEY,
    OrderId INT NOT NULL,
    ShipmentReference VARCHAR(80),
    CarrierName VARCHAR(50),
    TrackingNumber VARCHAR(100),
    ShipmentStatus VARCHAR(50),
    ShippingMethod VARCHAR(50),
    ShippedDate DATETIME,
    EstimatedDeliveryDate DATE,
    DeliveredDate DATETIME,
    DeliveryAttemptCount INT,
    LastLocation VARCHAR(100),
    CreatedAt DATETIME,
    UpdatedAt DATETIME,
    IsActive BOOLEAN DEFAULT TRUE,
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId)
);

CREATE TABLE Returns (
    ReturnId INT PRIMARY KEY,
    OrderId INT NOT NULL,
    ReturnReference VARCHAR(80),
    ReturnReason VARCHAR(255),
    ReturnStatus VARCHAR(50),
    RefundAmount DECIMAL(10,2),
    RefundStatus VARCHAR(50),
    RequestedDate DATETIME,
    ApprovedDate DATETIME,
    RefundedDate DATETIME,
    CustomerComments VARCHAR(255),
    InternalNotes VARCHAR(255),
    CreatedAt DATETIME,
    UpdatedAt DATETIME,
    IsActive BOOLEAN DEFAULT TRUE,
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId)
);