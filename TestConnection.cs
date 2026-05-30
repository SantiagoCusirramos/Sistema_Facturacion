// using System;
// using Sistema_Facturacion.Data;
// using Sistema_Facturacion.Helpers;
//
// if (DatabaseHelper.TestConnection())
// {
//     Console.WriteLine("✅ Conexión exitosa a SQL Server");
//     
//     var repo = new CustomerRepository();
//     var customers = repo.GetAll();
//     Console.WriteLine($"📊 Clientes encontrados: {customers.Count}");
// }
// else
// {
//     Console.WriteLine("❌ Error de conexión. Verifica:");
//     Console.WriteLine("   1. SQL Server está corriendo: sudo systemctl status mssql-server");
//     Console.WriteLine("   2. La contraseña en DatabaseHelper.cs es correcta");
//     Console.WriteLine("   3. La base de datos 'InvoiceSystem' existe");
// }