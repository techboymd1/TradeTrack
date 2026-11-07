USE tradetrack;

CREATE TABLE IF NOT EXISTS expenses (

    expense_id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,

    title VARCHAR(150) NOT NULL,

    category VARCHAR(80) NOT NULL,

    description VARCHAR(500) NULL,

    amount DECIMAL(12,2) NOT NULL,

    payment_method VARCHAR(30) NOT NULL,

    expense_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    reference_number VARCHAR(60) NULL,

    notes VARCHAR(500) NULL,

    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    PRIMARY KEY (expense_id),

    INDEX idx_expenses_date (expense_date),

    INDEX idx_expenses_category (category),

    CONSTRAINT chk_expenses_amount
        CHECK (amount > 0)

) ENGINE=InnoDB;