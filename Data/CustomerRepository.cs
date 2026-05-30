using System;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using Sistema_Facturacion.Models;
using Sistema_Facturacion.Helpers;

namespace Sistema_Facturacion.Data;

public class CustomerRepository
{
    public ObservableCollection<Customer> GetAll()
    {
        var customers = new ObservableCollection<Customer>();
        
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "SELECT id, document_id, name, address, phone, email, is_deleted, created_date FROM Customer WHERE is_deleted = 0";
        
        using var sqlreader = new SqlCommand(query, conn);
        using var reader = sqlreader.ExecuteReader();
        
        while (reader.Read())
        {
            customers.Add(new Customer
            {
                Id = reader.GetInt32(0),
                DocumentId = reader.GetString(1),
                Name = reader.GetString(2),
                Address = reader.IsDBNull(3) ? null : reader.GetString(3),
                Phone = reader.IsDBNull(4) ? null : reader.GetString(4),
                Email = reader.IsDBNull(5) ? null : reader.GetString(5),
                IsDeleted = reader.GetBoolean(6),
                CreatedDate = reader.GetDateTime(7)
            });
        }
        
        conn.Close();
        return customers;
    }
    
    public Customer? GetById(int id)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "SELECT id, document_id, name, address, phone, email, is_deleted, created_date FROM Customer WHERE id = @id AND is_deleted = 0";
        
        using var sqlreader = new SqlCommand(query, conn);
        sqlreader.Parameters.AddWithValue("@id", id);
        using var reader = sqlreader.ExecuteReader();
        
        if (reader.Read())
        {
            return new Customer
            {
                Id = reader.GetInt32(0),
                DocumentId = reader.GetString(1),
                Name = reader.GetString(2),
                Address = reader.IsDBNull(3) ? null : reader.GetString(3),
                Phone = reader.IsDBNull(4) ? null : reader.GetString(4),
                Email = reader.IsDBNull(5) ? null : reader.GetString(5),
                IsDeleted = reader.GetBoolean(6),
                CreatedDate = reader.GetDateTime(7)
            };
        }
        
        conn.Close();
        return null;
    }
    
    public void Create(Customer customer)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = @"INSERT INTO Customer (document_id, name, address, phone, email) 
                         VALUES (@document_id, @name, @address, @phone, @email)";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@document_id", customer.DocumentId);
        cmd.Parameters.AddWithValue("@name", customer.Name);
        cmd.Parameters.AddWithValue("@address", (object?)customer.Address ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@phone", (object?)customer.Phone ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@email", (object?)customer.Email ?? DBNull.Value);
        
        cmd.ExecuteNonQuery();
        conn.Close();
    }
    
    public void Update(Customer customer)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = @"UPDATE Customer SET 
                            document_id = @document_id, 
                            name = @name, 
                            address = @address, 
                            phone = @phone, 
                            email = @email 
                         WHERE id = @id";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", customer.Id);
        cmd.Parameters.AddWithValue("@document_id", customer.DocumentId);
        cmd.Parameters.AddWithValue("@name", customer.Name);
        cmd.Parameters.AddWithValue("@address", (object?)customer.Address ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@phone", (object?)customer.Phone ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@email", (object?)customer.Email ?? DBNull.Value);
        
        cmd.ExecuteNonQuery();
        conn.Close();
    }
    
    public void Delete(int id)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "UPDATE Customer SET is_deleted = 1 WHERE id = @id";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", id);
        
        cmd.ExecuteNonQuery();
        conn.Close();
    }
}