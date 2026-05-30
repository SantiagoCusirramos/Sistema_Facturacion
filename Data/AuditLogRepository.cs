using System;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using Sistema_Facturacion.Models;
using Sistema_Facturacion.Helpers;
using System.Text.Json;

namespace Sistema_Facturacion.Data;

public class AuditLogRepository
{
    public ObservableCollection<AuditLog> GetAll()
    {
        var logs = new ObservableCollection<AuditLog>();
        
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = @"SELECT a.id, a.user_id, a.table_name, a.action, a.record_id, 
                                a.old_data, a.new_data, a.action_date,
                                u.username as user_name
                         FROM AuditLog a
                         LEFT JOIN AppUser u ON a.user_id = u.id
                         ORDER BY a.action_date DESC";
        
        using var cmd = new SqlCommand(query, conn);
        using var reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            var log = new AuditLog
            {
                Id = reader.GetInt64(0),
                UserId = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                TableName = reader.GetString(2),
                Action = reader.GetString(3),
                RecordId = reader.GetInt32(4),
                OldData = reader.IsDBNull(5) ? null : reader.GetString(5),
                NewData = reader.IsDBNull(6) ? null : reader.GetString(6),
                ActionDate = reader.GetDateTime(7)
            };
            
            if (!reader.IsDBNull(8))
            {
                log.User = new AppUser { Id = log.UserId ?? 0, Username = reader.GetString(8) };
            }
            
            logs.Add(log);
        }
        
        return logs;
    }
    
    public ObservableCollection<AuditLog> GetByTable(string tableName, int recordId)
    {
        var logs = new ObservableCollection<AuditLog>();
        
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = @"SELECT id, user_id, table_name, action, record_id, 
                                old_data, new_data, action_date
                         FROM AuditLog 
                         WHERE table_name = @tableName AND record_id = @recordId
                         ORDER BY action_date DESC";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@tableName", tableName);
        cmd.Parameters.AddWithValue("@recordId", recordId);
        using var reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            logs.Add(new AuditLog
            {
                Id = reader.GetInt64(0),
                UserId = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                TableName = reader.GetString(2),
                Action = reader.GetString(3),
                RecordId = reader.GetInt32(4),
                OldData = reader.IsDBNull(5) ? null : reader.GetString(5),
                NewData = reader.IsDBNull(6) ? null : reader.GetString(6),
                ActionDate = reader.GetDateTime(7)
            });
        }
        
        return logs;
    }
    
    public void LogInsert(string tableName, int recordId, object newData, int? userId = null, string? ipAddress = null)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string newDataJson = JsonSerializer.Serialize(newData);
        
        string query = @"INSERT INTO AuditLog (user_id, table_name, action, record_id, new_data, ip_address)
                         VALUES (@user_id, @table_name, 'INSERT', @record_id, @new_data, @ip_address)";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@user_id", (object?)userId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@table_name", tableName);
        cmd.Parameters.AddWithValue("@record_id", recordId);
        cmd.Parameters.AddWithValue("@new_data", newDataJson);
        cmd.Parameters.AddWithValue("@ip_address", (object?)ipAddress ?? DBNull.Value);
        
        cmd.ExecuteNonQuery();
    }
    
    public void LogUpdate(string tableName, int recordId, object oldData, object newData, int? userId = null, string? ipAddress = null)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string oldDataJson = JsonSerializer.Serialize(oldData);
        string newDataJson = JsonSerializer.Serialize(newData);
        
        string query = @"INSERT INTO AuditLog (user_id, table_name, action, record_id, old_data, new_data, ip_address)
                         VALUES (@user_id, @table_name, 'UPDATE', @record_id, @old_data, @new_data, @ip_address)";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@user_id", (object?)userId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@table_name", tableName);
        cmd.Parameters.AddWithValue("@record_id", recordId);
        cmd.Parameters.AddWithValue("@old_data", oldDataJson);
        cmd.Parameters.AddWithValue("@new_data", newDataJson);
        cmd.Parameters.AddWithValue("@ip_address", (object?)ipAddress ?? DBNull.Value);
        
        cmd.ExecuteNonQuery();
    }
    
    public void LogDelete(string tableName, int recordId, object oldData, int? userId = null, string? ipAddress = null)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string oldDataJson = JsonSerializer.Serialize(oldData);
        
        string query = @"INSERT INTO AuditLog (user_id, table_name, action, record_id, old_data, ip_address)
                         VALUES (@user_id, @table_name, 'DELETE', @record_id, @old_data, @ip_address)";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@user_id", (object?)userId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@table_name", tableName);
        cmd.Parameters.AddWithValue("@record_id", recordId);
        cmd.Parameters.AddWithValue("@old_data", oldDataJson);
        cmd.Parameters.AddWithValue("@ip_address", (object?)ipAddress ?? DBNull.Value);
        
        cmd.ExecuteNonQuery();
    }
    
    public ObservableCollection<AuditLog> GetByUser(int userId)
    {
        var logs = new ObservableCollection<AuditLog>();
        
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = @"SELECT id, user_id, table_name, action, record_id, old_data, new_data, action_date
                         FROM AuditLog 
                         WHERE user_id = @userId
                         ORDER BY action_date DESC";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@userId", userId);
        using var reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            logs.Add(new AuditLog
            {
                Id = reader.GetInt64(0),
                UserId = reader.GetInt32(1),
                TableName = reader.GetString(2),
                Action = reader.GetString(3),
                RecordId = reader.GetInt32(4),
                OldData = reader.IsDBNull(5) ? null : reader.GetString(5),
                NewData = reader.IsDBNull(6) ? null : reader.GetString(6),
                ActionDate = reader.GetDateTime(7)
            });
        }
        
        return logs;
    }
}