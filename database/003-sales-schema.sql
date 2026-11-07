USE TradeTrack;

-- =====================================================
-- SALES
-- Stores the main information about each sale.
-- =====================================================

CREATE TABLE IF NOT EXISTS sales (
    sale_id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,

    invoice_number VARCHAR(40) NOT NULL,

    sale_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    subtotal DECIMAL(12,2) NOT NULL,

    discount_amount DECIMAL(12,2) NOT NULL DEFAULT 0.00,

    total_amount DECIMAL(12,2) NOT NULL,

    payment_method VARCHAR(30) NOT NULL,

    status VARCHAR(20) NOT NULL DEFAULT 'Completed',

    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    PRIMARY KEY (sale_id),

    UNIQUE KEY uq_sales_invoice_number (invoice_number),

    INDEX idx_sales_sale_date (sale_date),

    INDEX idx_sales_status (status),

    CONSTRAINT chk_sales_subtotal
        CHECK (subtotal >= 0),

    CONSTRAINT chk_sales_discount
        CHECK (discount_amount >= 0),

    CONSTRAINT chk_sales_total
        CHECK (total_amount >= 0),

    CONSTRAINT chk_sales_discount_limit
        CHECK (discount_amount <= subtotal)

) ENGINE=InnoDB;


-- =====================================================
-- SALE ITEMS
-- Stores individual products belonging to each sale.
-- =====================================================

CREATE TABLE IF NOT EXISTS sale_items (
    sale_item_id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,

    sale_id BIGINT UNSIGNED NOT NULL,

    product_id BIGINT UNSIGNED NOT NULL,

    product_name VARCHAR(150) NOT NULL,

    sku VARCHAR(40) NOT NULL,

    quantity DECIMAL(12,3) NOT NULL,

    unit_price DECIMAL(12,2) NOT NULL,

    unit_cost DECIMAL(12,2) NOT NULL,

    line_total DECIMAL(12,2) NOT NULL,

    PRIMARY KEY (sale_item_id),

    INDEX idx_sale_items_sale_id (sale_id),

    INDEX idx_sale_items_product_id (product_id),

    CONSTRAINT fk_sale_items_sales
        FOREIGN KEY (sale_id)
        REFERENCES sales(sale_id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_sale_items_products
        FOREIGN KEY (product_id)
        REFERENCES products(product_id)
        ON DELETE RESTRICT,

    CONSTRAINT chk_sale_items_quantity
        CHECK (quantity > 0),

    CONSTRAINT chk_sale_items_unit_price
        CHECK (unit_price >= 0),

    CONSTRAINT chk_sale_items_unit_cost
        CHECK (unit_cost >= 0),

    CONSTRAINT chk_sale_items_line_total
        CHECK (line_total >= 0)

) ENGINE=InnoDB;