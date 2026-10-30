
USE TradeTrack;

INSERT INTO products (
    sku,
    name,
    category,
    unit,
    cost_price,
    selling_price,
    quantity_in_stock,
    low_stock_threshold
)
SELECT
    'OIL-001',
    'Cooking Oil',
    'Groceries',
    'bottle',
    8000.00,
    10000.00,
    15,
    5
WHERE NOT EXISTS (
    SELECT 1
    FROM products
    WHERE sku = 'OIL-001'
);

SELECT * FROM products;
