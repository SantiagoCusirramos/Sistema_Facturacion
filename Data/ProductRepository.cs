using System;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using Sistema_Facturacion.Models;
using Sistema_Facturacion.Helpers;

namespace Sistema_Facturacion.Data;

public class ProductRepository
{
    public ObservableCollection<Product> GetAll()
    {
        var products = new ObservableCollection<Product>();
        
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = @"SELECT p.id, p.name, p.description, p.price, p.stock, p.category_id, 
                                p.is_deleted, p.created_date, c.name as category_name
                         FROM Product p
                         LEFT JOIN Category c ON p.category_id = c.id
                         WHERE p.is_deleted = 0";
        
        using var cmd = new SqlCommand(query, conn);
        using var reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            var product = new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                Price = reader.GetDecimal(3),
                Stock = reader.GetInt32(4),
                CategoryId = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                IsDeleted = reader.GetBoolean(6),
                CreatedDate = reader.GetDateTime(7)
            };
            
            // Cargar la categoría si existe
            if (!reader.IsDBNull(8))
            {
                product.Category = new Category
                {
                    Id = product.CategoryId ?? 0,
                    Name = reader.GetString(8)
                };
            }
            
            products.Add(product);
        }
        
        return products;
    }
    
    public ObservableCollection<Product> GetByCategory(int categoryId)
    {
        var products = new ObservableCollection<Product>();
        
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = @"SELECT id, name, description, price, stock, category_id, is_deleted, created_date
                         FROM Product 
                         WHERE category_id = @categoryId AND is_deleted = 0";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@categoryId", categoryId);
        using var reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            products.Add(new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                Price = reader.GetDecimal(3),
                Stock = reader.GetInt32(4),
                CategoryId = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                IsDeleted = reader.GetBoolean(6),
                CreatedDate = reader.GetDateTime(7)
            });
        }
        
        return products;
    }
    
    public Product? GetById(int id)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = @"SELECT p.id, p.name, p.description, p.price, p.stock, p.category_id, 
                                p.is_deleted, p.created_date, c.name as category_name
                         FROM Product p
                         LEFT JOIN Category c ON p.category_id = c.id
                         WHERE p.id = @id AND p.is_deleted = 0";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        
        if (reader.Read())
        {
            var product = new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                Price = reader.GetDecimal(3),
                Stock = reader.GetInt32(4),
                CategoryId = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                IsDeleted = reader.GetBoolean(6),
                CreatedDate = reader.GetDateTime(7)
            };
            
            if (!reader.IsDBNull(8))
            {
                product.Category = new Category
                {
                    Id = product.CategoryId ?? 0,
                    Name = reader.GetString(8)
                };
            }
            
            return product;
        }
        
        return null;
    }
    
    public void Create(Product product)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = @"INSERT INTO Product (name, description, price, stock, category_id) 
                         VALUES (@name, @description, @price, @stock, @categoryId)";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@name", product.Name);
        cmd.Parameters.AddWithValue("@description", (object?)product.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@price", product.Price);
        cmd.Parameters.AddWithValue("@stock", product.Stock);
        cmd.Parameters.AddWithValue("@categoryId", (object?)product.CategoryId ?? DBNull.Value);
        
        cmd.ExecuteNonQuery();
    }
    
    public void Update(Product product)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = @"UPDATE Product SET 
                            name = @name, 
                            description = @description, 
                            price = @price, 
                            stock = @stock, 
                            category_id = @categoryId
                         WHERE id = @id";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", product.Id);
        cmd.Parameters.AddWithValue("@name", product.Name);
        cmd.Parameters.AddWithValue("@description", (object?)product.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@price", product.Price);
        cmd.Parameters.AddWithValue("@stock", product.Stock);
        cmd.Parameters.AddWithValue("@categoryId", (object?)product.CategoryId ?? DBNull.Value);
        
        cmd.ExecuteNonQuery();
    }
    
    public void UpdateStock(int productId, int newStock)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "UPDATE Product SET stock = @stock WHERE id = @id";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", productId);
        cmd.Parameters.AddWithValue("@stock", newStock);
        
        cmd.ExecuteNonQuery();
    }
    
    public void Delete(int id)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "UPDATE Product SET is_deleted = 1 WHERE id = @id";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", id);
        
        cmd.ExecuteNonQuery();
    }
    
    public bool HasStock(int productId, int quantity)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "SELECT stock FROM Product WHERE id = @id AND is_deleted = 0";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", productId);
        
        var stock = (int?)cmd.ExecuteScalar();
        return stock.HasValue && stock.Value >= quantity;
    }
}