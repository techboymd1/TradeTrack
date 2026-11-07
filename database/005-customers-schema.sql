USE tradetrack;

CREATE TABLE IF NOT EXISTS customers (
    customer_id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    full_name VARCHAR(150) NOT NULL,
    phone VARCHAR(30) NULL,
    email VARCHAR(150) NULL,
    address VARCHAR(300) NULL,
    notes VARCHAR(500) NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
        ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (customer_id),
    INDEX idx_customers_name (full_name),
    INDEX idx_customers_phone (phone),
    INDEX idx_customers_email (email)
) ENGINE=InnoDB;

ALTER TABLE sales
    ADD COLUMN customer_id BIGINT UNSIGNED NULL,
    ADD INDEX idx_sales_customer_id (customer_id),
    ADD CONSTRAINT fk_sales_customers
        FOREIGN KEY (customer_id)
        REFERENCES customers(customer_id)
        ON DELETE SET NULL;