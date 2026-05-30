using System;

using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using Sistema_Facturacion.Models;
using Sistema_Facturacion.Helpers;

namespace Sistema_Facturacion.Data;

public class CategoryRepository
{
    public ObservableCollection<Category> GetAll()
    {
        var categories = new ObservableCollection<Category>();
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        String query = "SELECT id, name, description, created_date from Category";

        using var sqlreader = new SqlCommand(query, conn);
        using var reader = sqlreader.ExecuteReader();

        while (reader.Read())
        {
            categories.Add(new Category
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                CreatedDate = reader.GetDateTime(3)
            });
        }
        return categories;
    }

    public Category? GetById(int id)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "SELECT id, name, description, created_date FROM Category WHERE id = @id";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        
        if (reader.Read())
        {
            return new Category
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                CreatedDate = reader.GetDateTime(3)
            };
        }
        
        return null;
    }
    
    public void Create(Category category)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "INSERT INTO Category (name, description) VALUES (@name, @description)";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@name", category.Name);
        cmd.Parameters.AddWithValue("@description", (object?)category.Description ?? DBNull.Value);
        
        cmd.ExecuteNonQuery();
    }
    
    public void Update(Category category)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "UPDATE Category SET name = @name, description = @description WHERE id = @id";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", category.Id);
        cmd.Parameters.AddWithValue("@name", category.Name);
        cmd.Parameters.AddWithValue("@description", (object?)category.Description ?? DBNull.Value);
        
        cmd.ExecuteNonQuery();
    }
    
    public void Delete(int id)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "DELETE FROM Category WHERE id = @id";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", id);
        
        cmd.ExecuteNonQuery();
    }
    
}