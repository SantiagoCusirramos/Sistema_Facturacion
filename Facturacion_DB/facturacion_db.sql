-- MODULE 1: SECURITY & CONFIGURATION

CREATE TABLE AppUser (
                         id INT IDENTITY(1,1) PRIMARY KEY,
                         username VARCHAR(50) NOT NULL UNIQUE,
                         password_hash VARCHAR(255) NOT NULL,
                         full_name VARCHAR(150) NOT NULL,
                         is_active BIT DEFAULT 1,
                         registration_date DATETIME DEFAULT GETDATE()
);

CREATE TABLE Company (
                         tax_id VARCHAR(11) PRIMARY KEY,
                         name VARCHAR(150) NOT NULL,
                         address VARCHAR(255) NOT NULL,
                         phone_number VARCHAR(20),
                         email VARCHAR(100),
                         digital_certificate_path VARCHAR(255),
                         sunat_sol_user VARCHAR(50),
                         created_date DATETIME DEFAULT GETDATE()
);

CREATE TABLE Currency (
                          id INT IDENTITY(1,1) PRIMARY KEY,
                          symbol VARCHAR(10) NOT NULL,
                          tax_rate DECIMAL(5, 2) DEFAULT 18.00,
                          description VARCHAR(100) NOT NULL
);

-- MODULE 2: CUSTOMERS & PRODUCT CATALOG

CREATE TABLE Customer (
                          document_id VARCHAR(15) PRIMARY KEY,
                          name VARCHAR(150) NOT NULL,
                          address VARCHAR(255),
                          phone_number VARCHAR(20),
                          email VARCHAR(100),
                          is_deleted BIT DEFAULT 0,
                          created_date DATETIME DEFAULT GETDATE()
);

CREATE TABLE Category (
                          id INT IDENTITY(1,1) PRIMARY KEY,
                          name VARCHAR(100) NOT NULL,
                          description VARCHAR(MAX),
    is_deleted BIT DEFAULT 0,
    created_date DATETIME DEFAULT GETDATE()
);

CREATE TABLE Product (
                         id INT IDENTITY(1,1) PRIMARY KEY,
                         name VARCHAR(150) NOT NULL,
                         description VARCHAR(MAX),
    brand VARCHAR(100),
    stock INT NOT NULL DEFAULT 0,
    min_stock INT NOT NULL DEFAULT 0,
    price DECIMAL(12, 2) NOT NULL,
    is_deleted BIT DEFAULT 0,
    category_id INT,
    created_date DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Product_Category FOREIGN KEY (category_id) REFERENCES Category(id)
);

-- MODULE 3: SALES DOCUMENTS (INVOICES & CREDIT NOTES)

CREATE TABLE Document_Type (
                               id INT IDENTITY(1,1) PRIMARY KEY,
                               sunat_code VARCHAR(2) NOT NULL UNIQUE,
                               description VARCHAR(50) NOT NULL
);

CREATE TABLE Series_Numbering (
                                  id INT IDENTITY(1,1) PRIMARY KEY,
                                  document_type_id INT NOT NULL,
                                  series VARCHAR(4) NOT NULL,
                                  next_number INT NOT NULL DEFAULT 1,
                                  CONSTRAINT FK_Series_DocumentType FOREIGN KEY (document_type_id) REFERENCES Document_Type(id)
);

CREATE TABLE Payment_Method (
                                id INT IDENTITY(1,1) PRIMARY KEY,
                                description VARCHAR(50) NOT NULL,
                                requires_installments BIT DEFAULT 0
);

CREATE TABLE Invoice (
                         id INT IDENTITY(1,1) PRIMARY KEY,
                         company_tax_id VARCHAR(11) NOT NULL,
                         customer_document_id VARCHAR(15) NOT NULL,
                         currency_id INT NOT NULL,
                         document_type_id INT NOT NULL,
                         payment_method_id INT NOT NULL,
                         series VARCHAR(4) NOT NULL,
                         number INT NOT NULL,
                         issue_date DATETIME DEFAULT GETDATE(),
                         source_invoice_id INT NULL,
                         taxable_total DECIMAL(12, 2) NOT NULL DEFAULT 0.00,
                         tax_total DECIMAL(12, 2) NOT NULL DEFAULT 0.00,
                         total_to_pay DECIMAL(12, 2) NOT NULL DEFAULT 0.00,
                         status VARCHAR(30) DEFAULT 'ISSUED',
                         CONSTRAINT FK_Invoice_Company FOREIGN KEY (company_tax_id) REFERENCES Company(tax_id),
                         CONSTRAINT FK_Invoice_Customer FOREIGN KEY (customer_document_id) REFERENCES Customer(document_id),
                         CONSTRAINT FK_Invoice_Currency FOREIGN KEY (currency_id) REFERENCES Currency(id),
                         CONSTRAINT FK_Invoice_DocumentType FOREIGN KEY (document_type_id) REFERENCES Document_Type(id),
                         CONSTRAINT FK_Invoice_PaymentMethod FOREIGN KEY (payment_method_id) REFERENCES Payment_Method(id),
                         CONSTRAINT FK_Invoice_Source FOREIGN KEY (source_invoice_id) REFERENCES Invoice(id)
);

CREATE TABLE Invoice_Detail (
                                id INT IDENTITY(1,1) PRIMARY KEY,
                                invoice_id INT NOT NULL,
                                product_id INT NOT NULL,
                                quantity INT NOT NULL,
                                unit_price DECIMAL(12, 2) NOT NULL,
                                subtotal DECIMAL(12, 2) NOT NULL,
                                is_deleted BIT DEFAULT 0,
                                CONSTRAINT FK_Detail_Invoice FOREIGN KEY (invoice_id) REFERENCES Invoice(id),
                                CONSTRAINT FK_Detail_Product FOREIGN KEY (product_id) REFERENCES Product(id)
);

CREATE TABLE Installment (
                             id INT IDENTITY(1,1) PRIMARY KEY,
                             invoice_id INT NOT NULL,
                             installment_number INT NOT NULL,
                             amount DECIMAL(12, 2) NOT NULL,
                             due_date DATE NOT NULL,
                             status VARCHAR(20) DEFAULT 'PENDING',
                             CONSTRAINT FK_Installment_Invoice FOREIGN KEY (invoice_id) REFERENCES Invoice(id) ON DELETE CASCADE
);

-- MODULE 4: INVENTORY MOVEMENTS (KARDEX)

CREATE TABLE Inventory_Movement_Type (
                                         id INT IDENTITY(1,1) PRIMARY KEY,
                                         description VARCHAR(100) NOT NULL,
                                         effect INT NOT NULL
);

CREATE TABLE Kardex (
                        id INT IDENTITY(1,1) PRIMARY KEY,
                        product_id INT NOT NULL,
                        movement_type_id INT NOT NULL,
                        document_id INT NULL,
                        quantity INT NOT NULL,
                        previous_stock INT NOT NULL,
                        current_stock INT NOT NULL,
                        movement_date DATETIME DEFAULT GETDATE(),
                        CONSTRAINT FK_Kardex_Product FOREIGN KEY (product_id) REFERENCES Product(id),
                        CONSTRAINT FK_Kardex_MovementType FOREIGN KEY (movement_type_id) REFERENCES Inventory_Movement_Type(id),
                        CONSTRAINT FK_Kardex_Document FOREIGN KEY (document_id) REFERENCES Invoice(id)
);

-- MODULE 5: ADVANCED AUDIT LOG

CREATE TABLE Audit_Log (
                           id BIGINT IDENTITY(1,1) PRIMARY KEY,
                           user_id INT NULL,
                           affected_table VARCHAR(50) NOT NULL,
                           action VARCHAR(10) NOT NULL,
                           record_id INT NOT NULL,
                           old_value NVARCHAR(MAX) NULL,
                           new_value NVARCHAR(MAX) NULL,
                           ip_address VARCHAR(45),
                           action_date DATETIME DEFAULT GETDATE(),
                           CONSTRAINT FK_Audit_User FOREIGN KEY (user_id) REFERENCES AppUser(id)
);

INSERT INTO Document_Type (sunat_code, description) VALUES ('01', 'Invoice'), ('03', 'Receipt'), ('07', 'Credit Note'), ('08', 'Debit Note');
INSERT INTO Payment_Method (description, requires_installments) VALUES ('Cash', 0), ('Card', 0), ('Transfer', 0), ('Credit', 1);
INSERT INTO Inventory_Movement_Type (description, effect) VALUES ('Sale', -1), ('Return', 1), ('Stock Adjustment', 1), ('Cancellation', -1);
INSERT INTO Currency (symbol, tax_rate, description) VALUES ('S/', 18.00, 'Peruvian Sol'), ('$', 0.00, 'US Dollar');