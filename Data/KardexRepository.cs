using System;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using Sistema_Facturacion.Models;
using Sistema_Facturacion.Helpers;

namespace Sistema_Facturacion.Data;

public class KardexRepository
{
    public ObservableCollection<Kardex> GetAll()
    {
        var entries = new ObservableCollection<Kardex>();
        
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = @"SELECT k.id, k.product_id, k.invoice_detail_id, k.quantity, 
                                k.previous_stock, k.current_stock, k.movement_type, k.movement_date,
                                p.name as product_name
                         FROM Kardex k
                         LEFT JOIN Product p ON k.product_id = p.id
                         ORDER BY k.movement_date DESC";
        
        using var cmd = new SqlCommand(query, conn);
        using var reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            var entry = new Kardex
            {
                Id = reader.GetInt32(0),
                ProductId = reader.GetInt32(1),
                InvoiceDetailId = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                Quantity = reader.GetInt32(3),
                PreviousStock = reader.GetInt32(4),
                CurrentStock = reader.GetInt32(5),
                MovementType = reader.GetString(6),
                MovementDate = reader.GetDateTime(7)
            };
            
            if (!reader.IsDBNull(8))
            {
                entry.Product = new Product { Id = entry.ProductId, Name = reader.GetString(8) };
            }
            
            entries.Add(entry);
        }
        
        return entries;
    }
    
    public ObservableCollection<Kardex> GetByProduct(int productId)
    {
        var entries = new ObservableCollection<Kardex>();
        
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = @"SELECT id, product_id, invoice_detail_id, quantity, 
                                previous_stock, current_stock, movement_type, movement_date
                         FROM Kardex 
                         WHERE product_id = @productId
                         ORDER BY movement_date DESC";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@productId", productId);
        using var reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            entries.Add(new Kardex
            {
                Id = reader.GetInt32(0),
                ProductId = reader.GetInt32(1),
                InvoiceDetailId = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                Quantity = reader.GetInt32(3),
                PreviousStock = reader.GetInt32(4),
                CurrentStock = reader.GetInt32(5),
                MovementType = reader.GetString(6),
                MovementDate = reader.GetDateTime(7)
            });
        }
        
        return entries;
    }
    
    public int GetCurrentStock(int productId)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "SELECT TOP 1 current_stock FROM Kardex WHERE product_id = @productId ORDER BY movement_date DESC";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@productId", productId);
        
        var result = cmd.ExecuteScalar();
        return result != null ? Convert.ToInt32(result) : 0;
    }
    
    public void RegisterMovement(int productId, int quantity, string movementType, int? invoiceDetailId = null)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        // Obtener stock actual
        int currentStock = GetCurrentStock(productId);
        int previousStock = currentStock;
        int newStock = movementType == "SALE" ? currentStock - quantity : currentStock + quantity;
        
        if (newStock < 0) newStock = 0;
        
        string query = @"INSERT INTO Kardex (product_id, invoice_detail_id, quantity, previous_stock, current_stock, movement_type)
                         VALUES (@product_id, @invoice_detail_id, @quantity, @previous_stock, @current_stock, @movement_type)";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@product_id", productId);
        cmd.Parameters.AddWithValue("@invoice_detail_id", (object?)invoiceDetailId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@quantity", quantity);
        cmd.Parameters.AddWithValue("@previous_stock", previousStock);
        cmd.Parameters.AddWithValue("@current_stock", newStock);
        cmd.Parameters.AddWithValue("@movement_type", movementType);
        
        cmd.ExecuteNonQuery();
        
        // Actualizar stock en Product
        var productRepo = new ProductRepository();
        productRepo.UpdateStock(productId, newStock);
    }
    
    public void RegisterSale(int productId, int quantity, int invoiceDetailId)
    {
        RegisterMovement(productId, quantity, "SALE", invoiceDetailId);
    }
    
    public void RegisterReturn(int productId, int quantity, int invoiceDetailId)
    {
        RegisterMovement(productId, quantity, "RETURN", invoiceDetailId);
    }
    
    public void RegisterAdjustment(int productId, int quantity, string reason)
    {
        RegisterMovement(productId, quantity, "ADJUSTMENT", null);
    }
    
    public ObservableCollection<Kardex> GetMovementsByDateRange(DateTime startDate, DateTime endDate)
    {
        var entries = new ObservableCollection<Kardex>();
        
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = @"SELECT k.id, k.product_id, k.invoice_detail_id, k.quantity, 
                                k.previous_stock, k.current_stock, k.movement_type, k.movement_date,
                                p.name as product_name
                         FROM Kardex k
                         LEFT JOIN Product p ON k.product_id = p.id
                         WHERE k.movement_date BETWEEN @startDate AND @endDate
                         ORDER BY k.movement_date DESC";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@startDate", startDate);
        cmd.Parameters.AddWithValue("@endDate", endDate);
        using var reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            var entry = new Kardex
            {
                Id = reader.GetInt32(0),
                ProductId = reader.GetInt32(1),
                InvoiceDetailId = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                Quantity = reader.GetInt32(3),
                PreviousStock = reader.GetInt32(4),
                CurrentStock = reader.GetInt32(5),
                MovementType = reader.GetString(6),
                MovementDate = reader.GetDateTime(7)
            };
            
            if (!reader.IsDBNull(8))
            {
                entry.Product = new Product { Id = entry.ProductId, Name = reader.GetString(8) };
            }
            
            entries.Add(entry);
        }
        
        return entries;
    }
}