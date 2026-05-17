from faker import Faker
import random
from datetime import timedelta
from pathlib import Path

fake = Faker("en_US")

Faker.seed(12345)
random.seed(12345)

OUTPUT_FILE = Path("../migrations/004_seed_fake_data.sql")

CUSTOMERS_COUNT = 10000
ADDRESSES_COUNT = 15000
PRODUCTS_COUNT = 5000
ORDERS_COUNT = 20000
ORDER_ITEMS_COUNT = 30000
PAYMENTS_COUNT = 10000
SHIPMENTS_COUNT = 7000
RETURNS_COUNT = 3000

assert (
    CUSTOMERS_COUNT
    + ADDRESSES_COUNT
    + PRODUCTS_COUNT
    + ORDERS_COUNT
    + ORDER_ITEMS_COUNT
    + PAYMENTS_COUNT
    + SHIPMENTS_COUNT
    + RETURNS_COUNT
) == 100000


def clean(value):
    if value is None:
        return "NULL"

    if isinstance(value, bool):
        return "1" if value else "0"

    if isinstance(value, (int, float)):
        return str(value)

    value = str(value).replace("\\", "\\\\").replace("'", "''")
    return f"'{value}'"


def dt(value):
    if value is None:
        return "NULL"
    return clean(value.strftime("%Y-%m-%d %H:%M:%S"))


def d(value):
    if value is None:
        return "NULL"
    return clean(value.strftime("%Y-%m-%d"))


def write_insert(file, table, columns, rows):
    if not rows:
        return

    file.write(f"INSERT INTO {table} ({', '.join(columns)}) VALUES\n")

    values = []
    for row in rows:
        values.append("(" + ", ".join(row) + ")")

    file.write(",\n".join(values))
    file.write(";\n\n")


def chunks(items, size=500):
    for i in range(0, len(items), size):
        yield items[i:i + size]


OUTPUT_FILE.parent.mkdir(parents=True, exist_ok=True)

with open(OUTPUT_FILE, "w", encoding="utf-8") as f:
    f.write("USE CustomerSupportDb;\n\n")
    f.write("SET FOREIGN_KEY_CHECKS = 0;\n\n")

    f.write("DELETE FROM Returns;\n")
    f.write("DELETE FROM Shipments;\n")
    f.write("DELETE FROM Payments;\n")
    f.write("DELETE FROM OrderItems;\n")
    f.write("DELETE FROM Orders;\n")
    f.write("DELETE FROM Products;\n")
    f.write("DELETE FROM Addresses;\n")
    f.write("DELETE FROM Customers;\n\n")

    f.write("SET FOREIGN_KEY_CHECKS = 1;\n\n")

    # ---------------- CUSTOMERS ----------------
    customer_ids = list(range(1, CUSTOMERS_COUNT + 1))

    customer_rows = []
    for i in customer_ids:
        created_at = fake.date_time_between(start_date="-3y", end_date="-1y")
        updated_at = fake.date_time_between(start_date="-1y", end_date="now")

        customer_rows.append([
            clean(i),
            clean(f"CUST-{i:06}"),
            clean(fake.first_name()),
            clean(fake.last_name()),
            clean(f"user{i}@example.com"),
            clean(fake.phone_number()[:25]),
            d(fake.date_of_birth(minimum_age=18, maximum_age=75)),
            clean(random.choice(["Male", "Female", "Other"])),
            clean(random.choice(["Regular", "Premium", "Business"])),
            clean(random.randint(0, 10000)),
            clean(random.choice(["Active", "Inactive", "Suspended"])),
            clean(random.choice(["Email", "Phone", "SMS"])),
            dt(created_at),
            dt(updated_at),
            clean(random.choice([1, 1, 1, 0]))
        ])

    customer_columns = [
        "CustomerId", "CustomerCode", "FirstName", "LastName", "Email",
        "Phone", "DateOfBirth", "Gender", "CustomerType", "LoyaltyPoints",
        "AccountStatus", "PreferredContactMethod", "CreatedAt", "UpdatedAt", "IsActive"
    ]

    for batch in chunks(customer_rows):
        write_insert(f, "Customers", customer_columns, batch)

    # ---------------- ADDRESSES ----------------
    address_ids = list(range(1, ADDRESSES_COUNT + 1))

    address_rows = []
    customer_to_addresses = {}

    for i in address_ids:
        customer_id = random.choice(customer_ids)
        customer_to_addresses.setdefault(customer_id, []).append(i)

        created_at = fake.date_time_between(start_date="-3y", end_date="-1y")
        updated_at = fake.date_time_between(start_date="-1y", end_date="now")

        address_rows.append([
            clean(i),
            clean(customer_id),
            clean(random.choice(["Shipping", "Billing", "Home", "Office"])),
            clean(fake.street_address()),
            clean(fake.secondary_address()),
            clean(fake.city()),
            clean(fake.state()),
            clean(fake.postcode()),
            clean("USA"),
            clean(random.choice([0, 1])),
            clean(random.choice([0, 1])),
            clean(fake.sentence(nb_words=8)[:255]),
            dt(created_at),
            dt(updated_at),
            clean(1)
        ])

    address_columns = [
        "AddressId", "CustomerId", "AddressType", "Line1", "Line2",
        "City", "State", "PostalCode", "Country", "IsDefaultShipping",
        "IsDefaultBilling", "DeliveryInstructions", "CreatedAt", "UpdatedAt", "IsActive"
    ]

    for batch in chunks(address_rows):
        write_insert(f, "Addresses", address_columns, batch)

    # ---------------- PRODUCTS ----------------
    product_ids = list(range(1, PRODUCTS_COUNT + 1))
    products_cache = {}

    product_rows = []
    product_words = ["Laptop", "Mouse", "Keyboard", "Monitor", "Cable", "Dock", "Chair", "Desk", "Headset", "Adapter"]

    for i in product_ids:
        unit_price = round(random.uniform(10, 1200), 2)
        cost_price = round(unit_price * random.uniform(0.45, 0.8), 2)
        product_name = f"{random.choice(['Pro', 'Ultra', 'Smart', 'Eco', 'Prime'])} {random.choice(product_words)} {i}"

        products_cache[i] = {
            "name": product_name,
            "price": unit_price
        }

        created_at = fake.date_time_between(start_date="-3y", end_date="-1y")
        updated_at = fake.date_time_between(start_date="-1y", end_date="now")

        product_rows.append([
            clean(i),
            clean(f"PROD-{i:06}"),
            clean(product_name),
            clean(random.choice(["Electronics", "Office", "Accessories", "Home", "Computer"])),
            clean(random.choice(["Dell", "HP", "Logitech", "Apple", "Samsung", "Lenovo", "Generic"])),
            clean(unit_price),
            clean(cost_price),
            clean(random.randint(0, 500)),
            clean(random.randint(10, 50)),
            clean(random.choice(["Active", "Inactive", "Discontinued"])),
            clean(round(random.uniform(0.1, 15.0), 2)),
            clean(fake.company()[:100]),
            dt(created_at),
            dt(updated_at),
            clean(1)
        ])

    product_columns = [
        "ProductId", "ProductCode", "ProductName", "Category", "Brand",
        "UnitPrice", "CostPrice", "StockQuantity", "ReorderLevel", "ProductStatus",
        "WeightKg", "SupplierName", "CreatedAt", "UpdatedAt", "IsActive"
    ]

    for batch in chunks(product_rows):
        write_insert(f, "Products", product_columns, batch)

    # ---------------- ORDERS ----------------
    order_ids = list(range(1, ORDERS_COUNT + 1))
    orders_cache = {}

    order_rows = []

    for i in order_ids:
        customer_id = random.choice(customer_ids)

        possible_addresses = customer_to_addresses.get(customer_id)
        if possible_addresses:
            shipping_address_id = random.choice(possible_addresses)
            billing_address_id = random.choice(possible_addresses)
        else:
            shipping_address_id = random.choice(address_ids)
            billing_address_id = shipping_address_id

        subtotal = round(random.uniform(50, 2000), 2)
        tax = round(subtotal * 0.08, 2)
        discount = round(random.uniform(0, subtotal * 0.15), 2)
        total = round(subtotal + tax - discount, 2)

        order_date = fake.date_time_between(start_date="-1y", end_date="now")
        created_at = order_date
        updated_at = order_date + timedelta(days=random.randint(0, 30))

        order_status = random.choice(["Processing", "Confirmed", "Shipped", "Delivered", "Cancelled"])
        payment_status = random.choice(["Pending", "Paid", "Failed", "Refunded"])
        shipping_status = random.choice(["Not Shipped", "In Transit", "Delivered", "Delayed"])

        orders_cache[i] = {
            "customer_id": customer_id,
            "order_date": order_date,
            "total": total,
            "payment_status": payment_status,
            "shipping_status": shipping_status
        }

        order_rows.append([
            clean(i),
            clean(customer_id),
            clean(shipping_address_id),
            clean(billing_address_id),
            clean(f"ORD-{i:07}"),
            dt(order_date),
            clean(order_status),
            clean(payment_status),
            clean(shipping_status),
            clean(subtotal),
            clean(tax),
            clean(discount),
            clean(total),
            dt(created_at),
            dt(updated_at)
        ])

    order_columns = [
        "OrderId", "CustomerId", "ShippingAddressId", "BillingAddressId", "OrderNumber",
        "OrderDate", "OrderStatus", "PaymentStatus", "ShippingStatus", "SubTotal",
        "TaxAmount", "DiscountAmount", "TotalAmount", "CreatedAt", "UpdatedAt"
    ]

    for batch in chunks(order_rows):
        write_insert(f, "Orders", order_columns, batch)

    # ---------------- ORDER ITEMS ----------------
    order_item_rows = []

    for i in range(1, ORDER_ITEMS_COUNT + 1):
        order_id = random.choice(order_ids)
        product_id = random.choice(product_ids)
        product = products_cache[product_id]

        quantity = random.randint(1, 5)
        unit_price = product["price"]
        discount = round(random.uniform(0, unit_price * quantity * 0.1), 2)
        tax = round(unit_price * quantity * 0.08, 2)
        line_total = round((unit_price * quantity) + tax - discount, 2)

        order_date = orders_cache[order_id]["order_date"]

        order_item_rows.append([
            clean(i),
            clean(order_id),
            clean(product_id),
            clean(product["name"]),
            clean(quantity),
            clean(unit_price),
            clean(discount),
            clean(tax),
            clean(line_total),
            clean(random.choice(["Active", "Cancelled", "Returned"])),
            clean(random.choice([0, 1])),
            clean(fake.sentence(nb_words=6)[:255]),
            dt(order_date),
            dt(order_date + timedelta(days=random.randint(0, 5))),
            clean(1)
        ])

    order_item_columns = [
        "OrderItemId", "OrderId", "ProductId", "ProductNameSnapshot", "Quantity",
        "UnitPrice", "DiscountAmount", "TaxAmount", "LineTotal", "ItemStatus",
        "IsGift", "GiftMessage", "CreatedAt", "UpdatedAt", "IsActive"
    ]

    for batch in chunks(order_item_rows):
        write_insert(f, "OrderItems", order_item_columns, batch)

    # ---------------- PAYMENTS ----------------
    payment_rows = []
    paid_order_ids = order_ids[:PAYMENTS_COUNT]

    for i, order_id in enumerate(paid_order_ids, start=1):
        order = orders_cache[order_id]
        payment_status = order["payment_status"]

        payment_date = None
        if payment_status in ["Paid", "Refunded"]:
            payment_date = order["order_date"] + timedelta(minutes=random.randint(1, 120))

        amount = order["total"] if payment_status in ["Paid", "Refunded"] else 0

        payment_rows.append([
            clean(i),
            clean(order_id),
            clean(f"PAY-{i:07}"),
            clean(random.choice(["Credit Card", "Debit Card", "PayPal", "Gift Card"])),
            clean(payment_status),
            clean(amount),
            clean("USD"),
            clean(fake.uuid4()),
            clean(random.choice(["Stripe", "PayPal", "Adyen", "AuthorizeNet"])),
            dt(payment_date),
            clean(None if payment_status != "Failed" else random.choice(["Card declined", "Insufficient funds", "Gateway timeout"])),
            clean(random.choice(["None", "Pending", "Approved", "Completed"])),
            dt(order["order_date"]),
            dt(order["order_date"] + timedelta(days=random.randint(0, 10))),
            clean(1)
        ])

    payment_columns = [
        "PaymentId", "OrderId", "PaymentReference", "PaymentMethod", "PaymentStatus",
        "Amount", "Currency", "TransactionId", "GatewayName", "PaymentDate",
        "FailureReason", "RefundStatus", "CreatedAt", "UpdatedAt", "IsActive"
    ]

    for batch in chunks(payment_rows):
        write_insert(f, "Payments", payment_columns, batch)

    # ---------------- SHIPMENTS ----------------
    shipment_rows = []
    shipped_order_ids = order_ids[:SHIPMENTS_COUNT]

    for i, order_id in enumerate(shipped_order_ids, start=1):
        order = orders_cache[order_id]

        shipped_date = order["order_date"] + timedelta(days=random.randint(1, 3))
        estimated_date = shipped_date + timedelta(days=random.randint(2, 7))

        delivered_date = None
        if order["shipping_status"] == "Delivered":
            delivered_date = estimated_date + timedelta(hours=random.randint(1, 12))

        shipment_rows.append([
            clean(i),
            clean(order_id),
            clean(f"SHP-{i:07}"),
            clean(random.choice(["FedEx", "UPS", "USPS", "DHL"])),
            clean(f"TRK{random.randint(1000000000, 9999999999)}"),
            clean(order["shipping_status"]),
            clean(random.choice(["Standard", "Express", "Overnight"])),
            dt(shipped_date),
            d(estimated_date),
            dt(delivered_date),
            clean(random.randint(0, 3)),
            clean(fake.city()),
            dt(order["order_date"]),
            dt(order["order_date"] + timedelta(days=random.randint(1, 10))),
            clean(1)
        ])

    shipment_columns = [
        "ShipmentId", "OrderId", "ShipmentReference", "CarrierName", "TrackingNumber",
        "ShipmentStatus", "ShippingMethod", "ShippedDate", "EstimatedDeliveryDate", "DeliveredDate",
        "DeliveryAttemptCount", "LastLocation", "CreatedAt", "UpdatedAt", "IsActive"
    ]

    for batch in chunks(shipment_rows):
        write_insert(f, "Shipments", shipment_columns, batch)

    # ---------------- RETURNS ----------------
    return_rows = []
    returned_order_ids = order_ids[:RETURNS_COUNT]

    for i, order_id in enumerate(returned_order_ids, start=1):
        order = orders_cache[order_id]

        requested_date = order["order_date"] + timedelta(days=random.randint(5, 30))
        return_status = random.choice(["Requested", "Approved", "Rejected", "Completed"])
        refund_status = random.choice(["Pending", "Approved", "Completed", "Rejected"])

        approved_date = None
        refunded_date = None

        if return_status in ["Approved", "Completed"]:
            approved_date = requested_date + timedelta(days=random.randint(1, 4))

        if refund_status == "Completed":
            refunded_date = requested_date + timedelta(days=random.randint(5, 10))

        return_rows.append([
            clean(i),
            clean(order_id),
            clean(f"RET-{i:07}"),
            clean(random.choice(["Damaged item", "Wrong size", "Late delivery", "Changed mind", "Incorrect item"])),
            clean(return_status),
            clean(round(order["total"] * random.uniform(0.3, 1.0), 2)),
            clean(refund_status),
            dt(requested_date),
            dt(approved_date),
            dt(refunded_date),
            clean(fake.sentence(nb_words=8)[:255]),
            clean(fake.sentence(nb_words=8)[:255]),
            dt(requested_date),
            dt(requested_date + timedelta(days=random.randint(1, 5))),
            clean(1)
        ])

    return_columns = [
        "ReturnId", "OrderId", "ReturnReference", "ReturnReason", "ReturnStatus",
        "RefundAmount", "RefundStatus", "RequestedDate", "ApprovedDate", "RefundedDate",
        "CustomerComments", "InternalNotes", "CreatedAt", "UpdatedAt", "IsActive"
    ]

    for batch in chunks(return_rows):
        write_insert(f, "Returns", return_columns, batch)

    f.write("\n-- deterministic seed complete: 100000 total records\n")

print(f"Created {OUTPUT_FILE}")
print("Total records: 100000")