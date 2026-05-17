USE CustomerSupportDb;

DROP PROCEDURE IF EXISTS sp_GetOrderSummary;
DROP PROCEDURE IF EXISTS sp_GetCustomerOrderHistory;
DROP PROCEDURE IF EXISTS sp_GetPaymentStatus;
DROP PROCEDURE IF EXISTS sp_GetShipmentStatus;
DROP PROCEDURE IF EXISTS sp_GetReturnStatus;
DROP PROCEDURE IF EXISTS sp_GetCustomerOverview;
DROP PROCEDURE IF EXISTS sp_GetPendingRefunds;
DROP PROCEDURE IF EXISTS sp_SearchProducts;

DELIMITER //

CREATE PROCEDURE sp_GetOrderSummary(IN p_OrderId INT)
BEGIN
    SELECT
        o.OrderId,
        o.OrderNumber,
        c.CustomerId,
        c.FirstName,
        c.LastName,
        c.Email,
        o.OrderDate,
        o.OrderStatus,
        o.PaymentStatus,
        o.ShippingStatus,
        o.TotalAmount,
        p.PaymentMethod,
        p.TransactionId,
        s.CarrierName,
        s.TrackingNumber,
        s.EstimatedDeliveryDate
    FROM Orders o
    JOIN Customers c ON o.CustomerId = c.CustomerId
    LEFT JOIN Payments p ON o.OrderId = p.OrderId
    LEFT JOIN Shipments s ON o.OrderId = s.OrderId
    WHERE o.OrderId = p_OrderId;
END //

CREATE PROCEDURE sp_GetCustomerOrderHistory(IN p_CustomerId INT)
BEGIN
    SELECT
        OrderId,
        OrderNumber,
        OrderDate,
        OrderStatus,
        PaymentStatus,
        ShippingStatus,
        TotalAmount
    FROM Orders
    WHERE CustomerId = p_CustomerId
    ORDER BY OrderDate DESC;
END //

CREATE PROCEDURE sp_GetPaymentStatus(IN p_OrderId INT)
BEGIN
    SELECT *
    FROM Payments
    WHERE OrderId = p_OrderId;
END //

CREATE PROCEDURE sp_GetShipmentStatus(IN p_OrderId INT)
BEGIN
    SELECT *
    FROM Shipments
    WHERE OrderId = p_OrderId;
END //

CREATE PROCEDURE sp_GetReturnStatus(IN p_OrderId INT)
BEGIN
    SELECT *
    FROM Returns
    WHERE OrderId = p_OrderId;
END //

CREATE PROCEDURE sp_GetCustomerOverview(IN p_CustomerId INT)
BEGIN
    SELECT
        c.CustomerId,
        c.CustomerCode,
        c.FirstName,
        c.LastName,
        c.Email,
        c.Phone,
        c.CustomerType,
        c.LoyaltyPoints,
        c.AccountStatus,
        COUNT(o.OrderId) AS TotalOrders,
        COALESCE(SUM(o.TotalAmount), 0) AS LifetimeSpend,
        MAX(o.OrderDate) AS LastOrderDate
    FROM Customers c
    LEFT JOIN Orders o ON c.CustomerId = o.CustomerId
    WHERE c.CustomerId = p_CustomerId
    GROUP BY
        c.CustomerId,
        c.CustomerCode,
        c.FirstName,
        c.LastName,
        c.Email,
        c.Phone,
        c.CustomerType,
        c.LoyaltyPoints,
        c.AccountStatus;
END //

CREATE PROCEDURE sp_GetPendingRefunds()
BEGIN
    SELECT
        r.ReturnId,
        r.OrderId,
        r.ReturnReference,
        r.RefundAmount,
        r.RefundStatus,
        r.RequestedDate,
        c.CustomerId,
        c.FirstName,
        c.LastName,
        c.Email
    FROM Returns r
    JOIN Orders o ON r.OrderId = o.OrderId
    JOIN Customers c ON o.CustomerId = c.CustomerId
    WHERE r.RefundStatus = 'Pending'
    ORDER BY r.RequestedDate ASC;
END //

CREATE PROCEDURE sp_SearchProducts(IN p_SearchText VARCHAR(100))
BEGIN
    SELECT *
    FROM Products
    WHERE ProductName LIKE CONCAT('%', p_SearchText, '%')
       OR Category LIKE CONCAT('%', p_SearchText, '%')
       OR Brand LIKE CONCAT('%', p_SearchText, '%');
END //

DELIMITER ;