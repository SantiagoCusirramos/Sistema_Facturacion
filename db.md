# Consultas y Verificación de Datos

## Conectarse a SQL Server

Inicia una sesión con SQL Server utilizando `sqlcmd`:

```bash
sqlcmd -S localhost -U sa -P "fdx_santi@#282805" -C -d InvoiceSystem
```

---

## Ver el contenido de las tablas

Una vez dentro de `sqlcmd`, ejecuta las siguientes consultas según la información que necesites revisar.

### 1. Usuarios

```sql
SELECT * FROM AppUser;
GO
```

### 2. Clientes

```sql
SELECT * FROM Customer;
GO
```

### 3. Categorías

```sql
SELECT * FROM Category;
GO
```

### 4. Productos

```sql
SELECT * FROM Product;
GO
```

### 5. Métodos de Pago

```sql
SELECT * FROM PaymentMethod;
GO
```

### 6. Tipos de Documento

```sql
SELECT * FROM DocumentType;
GO
```

### 7. Facturas

```sql
SELECT * FROM Invoice;
GO
```

### 8. Detalles de Factura

```sql
SELECT * FROM InvoiceDetail;
GO
```

### 9. Kardex (Movimientos de Inventario)

```sql
SELECT * FROM Kardex;
GO
```

### 10. Auditoría

```sql
SELECT * FROM AuditLog;
GO
```

---

## Salir de `sqlcmd`

```sql
QUIT
```

---

# Consultas Útiles

## Facturas con nombre del cliente

```bash
sqlcmd -S localhost -U sa -P "<PASSWORD>" -C -d InvoiceSystem -Q "
SELECT
    i.id,
    i.invoice_number,
    c.name AS customer,
    i.total,
    i.status
FROM Invoice i
INNER JOIN Customer c
    ON i.customer_id = c.id
ORDER BY i.id DESC;
"
```

## Kardex con información del producto

```bash
sqlcmd -S localhost -U sa -P "<PASSWORD>" -C -d InvoiceSystem -Q "
SELECT
    k.id,
    p.name AS product,
    k.quantity,
    k.previous_stock,
    k.current_stock,
    k.movement_type,
    k.movement_date
FROM Kardex k
INNER JOIN Product p
    ON k.product_id = p.id
ORDER BY k.movement_date DESC;
"
```

## Detalle de una factura específica

> Modifica el valor de `invoice_id` según la factura que desees consultar.

```bash
sqlcmd -S localhost -U sa -P "<PASSWORD>" -C -d InvoiceSystem -Q "
SELECT
    d.id,
    p.name,
    d.quantity,
    d.unit_price,
    d.subtotal
FROM InvoiceDetail d
INNER JOIN Product p
    ON d.product_id = p.id
WHERE d.invoice_id = 17;
"
```

---

# Estadísticas Rápidas

Consulta el número de registros almacenados en las principales tablas del sistema:

```bash
sqlcmd -S localhost -U sa -P "<PASSWORD>" -C -d InvoiceSystem -Q "
SELECT 'Customers'      AS TableName, COUNT(*) AS Total FROM Customer
UNION ALL
SELECT 'Products',      COUNT(*) FROM Product
UNION ALL
SELECT 'Invoices',      COUNT(*) FROM Invoice
UNION ALL
SELECT 'InvoiceDetails',COUNT(*) FROM InvoiceDetail
UNION ALL
SELECT 'Kardex',        COUNT(*) FROM Kardex;
"
```

---

## Notas

- Todas las consultas son de solo lectura.
- Asegúrate de que el servicio de SQL Server esté en ejecución.
- Verifica que la base de datos `InvoiceSystem` exista antes de ejecutar los comandos.
- Si utilizas certificados autofirmados, mantén la opción `-C` habilitada.