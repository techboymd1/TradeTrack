
CREATE DATABASE IF NOT EXISTS TradeTrack
CHARACTER SET utf8mb4
COLLATE utf8mb4_0900_ai_ci;

USE TradeTrack;

CREATE TABLE IF NOT EXISTS products (
    product_id BIGINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,

    sku VARCHAR(40) NOT NULL UNIQUE,
    name VARCHAR(150) NOT NULL,
    category VARCHAR(80),
    unit VARCHAR(20) NOT NULL DEFAULT 'unit',

    cost_price DECIMAL(12,2) NOT NULL,
    selling_price DECIMAL(12,2) NOT NULL,

    quantity_in_stock DECIMAL(12,3) NOT NULL DEFAULT 0,
    low_stock_threshold DECIMAL(12,3) NOT NULL DEFAULT 5,

    is_active BOOLEAN NOT NULL DEFAULT TRUE,

    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
        ON UPDATE CURRENT_TIMESTAMP,

    CONSTRAINT chk_cost_price
        CHECK (cost_price >= 0),

    CONSTRAINT chk_selling_price
        CHECK (selling_price >= 0),

    CONSTRAINT chk_stock
        CHECK (quantity_in_stock >= 0),

    CONSTRAINT chk_low_stock
        CHECK (low_stock_threshold >= 0)
);

SHOW TABLES;

DESCRIBE products;
