using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using Sistema_Facturacion.Models;
using Sistema_Facturacion.Helpers;

namespace Sistema_Facturacion.Data;

public class PaymentMethodRepository
{
    public ObservableCollection<PaymentMethod> GetAll()
    {
        var methods = new ObservableCollection<PaymentMethod>();
        
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "SELECT id, name FROM PaymentMethod";
        
        using var cmd = new SqlCommand(query, conn);
        using var reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            methods.Add(new PaymentMethod
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            });
        }
        
        return methods;
    }
    
    public PaymentMethod? GetById(int id)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "SELECT id, name FROM PaymentMethod WHERE id = @id";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        
        if (reader.Read())
        {
            return new PaymentMethod
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            };
        }
        
        return null;
    }
    
    public void Create(PaymentMethod method)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "INSERT INTO PaymentMethod (name) VALUES (@name)";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@name", method.Name);
        
        cmd.ExecuteNonQuery();
    }
    
    public void Update(PaymentMethod method)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "UPDATE PaymentMethod SET name = @name WHERE id = @id";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", method.Id);
        cmd.Parameters.AddWithValue("@name", method.Name);
        
        cmd.ExecuteNonQuery();
    }
    
    public void Delete(int id)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "DELETE FROM PaymentMethod WHERE id = @id";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", id);
        
        cmd.ExecuteNonQuery();
    }
}