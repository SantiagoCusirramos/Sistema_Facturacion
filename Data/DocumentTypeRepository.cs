using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using Sistema_Facturacion.Models;
using Sistema_Facturacion.Helpers;

namespace Sistema_Facturacion.Data;

public class DocumentTypeRepository
{
    public ObservableCollection<DocumentType> GetAll()
    {
        var types = new ObservableCollection<DocumentType>();
        
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "SELECT id, code, name FROM DocumentType";
        
        using var cmd = new SqlCommand(query, conn);
        using var reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            types.Add(new DocumentType
            {
                Id = reader.GetInt32(0),
                Code = reader.GetString(1),
                Name = reader.GetString(2)
            });
        }
        
        return types;
    }
    
    public DocumentType? GetById(int id)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "SELECT id, code, name FROM DocumentType WHERE id = @id";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        
        if (reader.Read())
        {
            return new DocumentType
            {
                Id = reader.GetInt32(0),
                Code = reader.GetString(1),
                Name = reader.GetString(2)
            };
        }
        
        return null;
    }
    
    public DocumentType? GetByCode(string code)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "SELECT id, code, name FROM DocumentType WHERE code = @code";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@code", code);
        using var reader = cmd.ExecuteReader();
        
        if (reader.Read())
        {
            return new DocumentType
            {
                Id = reader.GetInt32(0),
                Code = reader.GetString(1),
                Name = reader.GetString(2)
            };
        }
        
        return null;
    }
    
    public void Create(DocumentType type)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "INSERT INTO DocumentType (code, name) VALUES (@code, @name)";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@code", type.Code);
        cmd.Parameters.AddWithValue("@name", type.Name);
        
        cmd.ExecuteNonQuery();
    }
    
    public void Update(DocumentType type)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "UPDATE DocumentType SET code = @code, name = @name WHERE id = @id";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", type.Id);
        cmd.Parameters.AddWithValue("@code", type.Code);
        cmd.Parameters.AddWithValue("@name", type.Name);
        
        cmd.ExecuteNonQuery();
    }
    
    public void Delete(int id)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "DELETE FROM DocumentType WHERE id = @id";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", id);
        
        cmd.ExecuteNonQuery();
    }
}