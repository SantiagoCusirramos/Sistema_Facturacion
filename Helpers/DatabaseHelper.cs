using System;
using Microsoft.Data.SqlClient;

namespace Sistema_Facturacion.Helpers;

public class DatabaseHelper {
    private static string connectionString = "Server=localhost; Database=InvoiceSystem; User Id=sa; Password=fdx_santi@#282805; TrustServerCertificate=True;";

    public static SqlConnection GetConnection() {
        return new SqlConnection(connectionString);
    }

    public static bool TestConnection()
    {
        try
        {
            using var conn = GetConnection();
            conn.Open();
            return true;
            
        }
        catch (Exception e)
        {
            return false;
        }
    }

    
    
}