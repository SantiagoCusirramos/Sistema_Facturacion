using System;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using Sistema_Facturacion.Models;
using Sistema_Facturacion.Helpers;

namespace Sistema_Facturacion.Data;

public class AppUserRepository
{
    public ObservableCollection<AppUser> GetAll()
    {
        var users = new ObservableCollection<AppUser>();
        
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "SELECT id, username, password_hash, full_name, is_active, created_date FROM AppUser";
        
        using var cmd = new SqlCommand(query, conn);
        using var reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            users.Add(new AppUser
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                PasswordHash = reader.GetString(2),
                FullName = reader.GetString(3),
                IsActive = reader.GetBoolean(4),
                CreatedDate = reader.GetDateTime(5)
            });
        }
        
        return users;
    }
    
    public AppUser? GetById(int id)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "SELECT id, username, password_hash, full_name, is_active, created_date FROM AppUser WHERE id = @id";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        
        if (reader.Read())
        {
            return new AppUser
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                PasswordHash = reader.GetString(2),
                FullName = reader.GetString(3),
                IsActive = reader.GetBoolean(4),
                CreatedDate = reader.GetDateTime(5)
            };
        }
        
        return null;
    }
    
    public AppUser? GetByUsername(string username)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "SELECT id, username, password_hash, full_name, is_active, created_date FROM AppUser WHERE username = @username";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@username", username);
        using var reader = cmd.ExecuteReader();
        
        if (reader.Read())
        {
            return new AppUser
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                PasswordHash = reader.GetString(2),
                FullName = reader.GetString(3),
                IsActive = reader.GetBoolean(4),
                CreatedDate = reader.GetDateTime(5)
            };
        }
        
        return null;
    }
    
    public void Create(AppUser user)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "INSERT INTO AppUser (username, password_hash, full_name, is_active) VALUES (@username, @password_hash, @full_name, @is_active)";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@username", user.Username);
        cmd.Parameters.AddWithValue("@password_hash", user.PasswordHash);
        cmd.Parameters.AddWithValue("@full_name", user.FullName);
        cmd.Parameters.AddWithValue("@is_active", user.IsActive);
        
        cmd.ExecuteNonQuery();
    }
    
    public void Update(AppUser user)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "UPDATE AppUser SET username = @username, password_hash = @password_hash, full_name = @full_name, is_active = @is_active WHERE id = @id";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", user.Id);
        cmd.Parameters.AddWithValue("@username", user.Username);
        cmd.Parameters.AddWithValue("@password_hash", user.PasswordHash);
        cmd.Parameters.AddWithValue("@full_name", user.FullName);
        cmd.Parameters.AddWithValue("@is_active", user.IsActive);
        
        cmd.ExecuteNonQuery();
    }
    
    public void Delete(int id)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "DELETE FROM AppUser WHERE id = @id";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", id);
        
        cmd.ExecuteNonQuery();
    }
}