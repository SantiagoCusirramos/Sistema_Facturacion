using Microsoft.Data.SqlClient;

namespace Sistema_Facturacion.Helpers;

public class DatabaseHelper {
    private static string connectionString = "Server=localhost; Database=InvoiceSystem; Id=sa; Password=fdx_santi@#282805; TrustSerserCertificate=true";

    public static SqlConnection GetConexion() {
        return new SqlConnection(connectionString);
    }
    
}