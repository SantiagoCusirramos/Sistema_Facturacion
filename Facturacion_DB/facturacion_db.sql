-- AppUser - Referencia a seguridad y login
CREATE TABLE AppUser ( 
    id INT IDENTITY(1,1) PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(150) NOT NULL,
    is_active BIT DEFAULT 1,
    created_date DATETIME DEFAULT GETDATE()
);  

-- Customer
CREATE TABLE Customer (
    id INT IDENTITY(1,1) PRIMARY KEY,
    document_id VARCHAR(15) NOT NULL UNIQUE, -- DNI - PASSAPORTE - VISA
    name VARCHAR(150) NOT NULL,
    address VARCHAR(255),
    phone VARCHAR(20),
    email VARCHAR(100),
    is_deleted BIT DEFAULT 0,
    created_date DATETIME DEFAULT GETDATE()
);

-- Category
CREATE TABLE Category (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description VARCHAR(MAX),
    created_date DATETIME DEFAULT GETDATE()
);

-- Product
CREATE TABLE Product (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    description VARCHAR(MAX),
    price DECIMAL(12,2) NOT NULL,
    stock INT NOT NULL DEFAULT 0,
    category_id INT,
    is_deleted BIT DEFAULT 0,
    created_date DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (category_id) REFERENCES Category(id)
);

-- PaymentMethod
CREATE TABLE PaymentMethod (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name VARCHAR(50) NOT NULL
);

-- DocumentType
CREATE TABLE DocumentType (
    id INT IDENTITY(1,1) PRIMARY KEY,
    code VARCHAR(2) NOT NULL,
    name VARCHAR(50) NOT NULL
);

-- Invoice - Factura
CREATE TABLE Invoice (
    id INT IDENTITY(1,1) PRIMARY KEY,
    invoice_number VARCHAR(20) NOT NULL UNIQUE,
    customer_id INT NOT NULL,
    payment_method_id INT NOT NULL,
    document_type_id INT NOT NULL,
    issue_date DATETIME DEFAULT GETDATE(), -- Fecha de asunto o x evento
    subtotal DECIMAL(12,2) NOT NULL DEFAULT 0,
    tax DECIMAL(12,2) NOT NULL DEFAULT 0,
    total DECIMAL(12,2) NOT NULL DEFAULT 0,
    status VARCHAR(20) DEFAULT 'ACTIVE',
    FOREIGN KEY (customer_id) REFERENCES Customer(id),
    FOREIGN KEY (payment_method_id) REFERENCES PaymentMethod(id),
    FOREIGN KEY (document_type_id) REFERENCES DocumentType(id)
);

-- InvoiceDetail
CREATE TABLE InvoiceDetail (
    id INT IDENTITY(1,1) PRIMARY KEY,
    invoice_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL,
    unit_price DECIMAL(12,2) NOT NULL,
    subtotal DECIMAL(12,2) NOT NULL,
    FOREIGN KEY (invoice_id) REFERENCES Invoice(id),
    FOREIGN KEY (product_id) REFERENCES Product(id)
);

-- Kardex -- Registro del Sistema ANtes - Despues
CREATE TABLE Kardex (
    id INT IDENTITY(1,1) PRIMARY KEY,
    product_id INT NOT NULL,
    invoice_detail_id INT NULL,
    quantity INT NOT NULL,
    previous_stock INT NOT NULL,
    current_stock INT NOT NULL,
    movement_type VARCHAR(20) NOT NULL, -- 'SALE', 'RETURN', 'ADJUSTMENT'
    movement_date DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (product_id) REFERENCES Product(id),
    FOREIGN KEY (invoice_detail_id) REFERENCES InvoiceDetail(id)
);

-- AuditLog
CREATE TABLE AuditLog (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NULL,
    table_name VARCHAR(50) NOT NULL,
    action VARCHAR(10) NOT NULL,
    record_id INT NOT NULL,
    old_data NVARCHAR(MAX) NULL,
    new_data NVARCHAR(MAX) NULL,
    action_date DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES AppUser(id)
);


INSERT INTO PaymentMethod (name) VALUES ('Cash'), ('Card'), ('Transfer');
INSERT INTO DocumentType (code, name) VALUES ('01', 'Invoice'), ('03', 'Receipt');
INSERT INTO AppUser (username, password_hash, full_name) VALUES ('admin', 'admin123', 'Administrator');