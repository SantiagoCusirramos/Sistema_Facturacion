using System;
using Sistema_Facturacion.Data;
using Sistema_Facturacion.Models;
using Sistema_Facturacion.Helpers;

Console.WriteLine("=".Repeat(50));
Console.WriteLine("🧪 VERIFICACIÓN DE REPOSITORIOS");
Console.WriteLine("=".Repeat(50));

// 1. Probar conexión
Console.WriteLine("\n1. Probando conexión a SQL Server...");
if (DatabaseHelper.TestConnection())
{
    Console.WriteLine("   ✅ Conexión exitosa");
}
else
{
    Console.WriteLine("   ❌ Error de conexión");
    return;
}

// 2. PaymentMethodRepository
Console.WriteLine("\n2. PaymentMethodRepository...");
var paymentRepo = new PaymentMethodRepository();
var payments = paymentRepo.GetAll();
Console.WriteLine($"   ✅ {payments.Count} métodos de pago encontrados");
foreach (var p in payments)
{
    Console.WriteLine($"      - {p.Name}");
}

// 3. DocumentTypeRepository
Console.WriteLine("\n3. DocumentTypeRepository...");
var docRepo = new DocumentTypeRepository();
var docs = docRepo.GetAll();
Console.WriteLine($"   ✅ {docs.Count} tipos de documento encontrados");
foreach (var d in docs)
{
    Console.WriteLine($"      - {d.Name} ({d.Code})");
}

// 4. CategoryRepository
Console.WriteLine("\n4. CategoryRepository...");
var catRepo = new CategoryRepository();
var cats = catRepo.GetAll();
Console.WriteLine($"   ✅ {cats.Count} categorías encontradas");

// 5. ProductRepository
Console.WriteLine("\n5. ProductRepository...");
var productRepo = new ProductRepository();
var products = productRepo.GetAll();
Console.WriteLine($"   ✅ {products.Count} productos encontrados");

// 6. CustomerRepository
Console.WriteLine("\n6. CustomerRepository...");
var customerRepo = new CustomerRepository();
var customers = customerRepo.GetAll();
Console.WriteLine($"   ✅ {customers.Count} clientes encontrados");

// 7. AppUserRepository
Console.WriteLine("\n7. AppUserRepository...");
var userRepo = new AppUserRepository();
var users = userRepo.GetAll();
Console.WriteLine($"   ✅ {users.Count} usuarios encontrados");

// 8. InvoiceRepository
Console.WriteLine("\n8. InvoiceRepository...");
var invoiceRepo = new InvoiceRepository();
var invoices = invoiceRepo.GetAll();
Console.WriteLine($"   ✅ {invoices.Count} facturas encontradas");

// 9. InvoiceDetailRepository
Console.WriteLine("\n9. InvoiceDetailRepository...");
var detailRepo = new InvoiceDetailRepository();
if (invoices.Count > 0)
{
    var details = detailRepo.GetByInvoiceId(invoices[0].Id);
    Console.WriteLine($"   ✅ {details.Count} detalles en primera factura");
}
else
{
    Console.WriteLine("   ⚠️ No hay facturas para verificar detalles");
}

// 10. KardexRepository
Console.WriteLine("\n10. KardexRepository...");
var kardexRepo = new KardexRepository();
var movements = kardexRepo.GetAll();
Console.WriteLine($"   ✅ {movements.Count} movimientos de inventario");

// 11. AuditLogRepository
Console.WriteLine("\n11. AuditLogRepository...");
var auditRepo = new AuditLogRepository();
var logs = auditRepo.GetAll();
Console.WriteLine($"   ✅ {logs.Count} registros de auditoría");

Console.WriteLine("\n" + "=".Repeat(50));
Console.WriteLine("✅ VERIFICACIÓN COMPLETADA");
Console.WriteLine("=".Repeat(50));

// Extensión para Repeat (si no existe)
public static class StringExtensions
{
    public static string Repeat(this string str, int count)
    {
        return new string(str[0], count);
    }
}