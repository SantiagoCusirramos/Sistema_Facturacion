using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using Sistema_Facturacion.Models;
using Sistema_Facturacion.Helpers;

namespace Sistema_Facturacion.Data;

public class InvoiceDetailRepository
{
    public ObservableCollection<InvoiceDetail> GetByInvoiceId(int invoiceId)
    {
        var details = new ObservableCollection<InvoiceDetail>();
    
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
    
        string query = @"SELECT d.id, d.invoice_id, d.product_id, d.quantity, d.unit_price, d.subtotal,
                            p.name as product_name
                     FROM InvoiceDetail d
                     LEFT JOIN Product p ON d.product_id = p.id
                     WHERE d.invoice_id = @invoiceId";
    
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
        using var reader = cmd.ExecuteReader();
    
        while (reader.Read())
        {
            var detail = new InvoiceDetail
            {
                Id = reader.GetInt32(0),
                InvoiceId = reader.GetInt32(1),
                ProductId = reader.GetInt32(2),
                Quantity = reader.GetInt32(3),
                UnitPrice = reader.GetDecimal(4),
                Subtotal = reader.GetDecimal(5)
            };
            
            if (!reader.IsDBNull(6))
            {
                detail.Product = new Product
                {
                    Id = detail.ProductId,
                    Name = reader.GetString(6)
                };
            }
        
            details.Add(detail);
        }
    
        return details;
    }
    
    public int Create(InvoiceDetail detail)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
    
        string query = @"INSERT INTO InvoiceDetail (invoice_id, product_id, quantity, unit_price, subtotal)
                     VALUES (@invoice_id, @product_id, @quantity, @unit_price, @subtotal);
                     SELECT SCOPE_IDENTITY();";
    
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@invoice_id", detail.InvoiceId);
        cmd.Parameters.AddWithValue("@product_id", detail.ProductId);
        cmd.Parameters.AddWithValue("@quantity", detail.Quantity);
        cmd.Parameters.AddWithValue("@unit_price", detail.UnitPrice);
        cmd.Parameters.AddWithValue("@subtotal", detail.Subtotal);
    
        int newId = Convert.ToInt32(cmd.ExecuteScalar());
        detail.Id = newId;
        return newId;
    }
    
    public void CreateMultiple(List<InvoiceDetail> details)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        using var transaction = conn.BeginTransaction();
        
        try
        {
            string query = @"INSERT INTO InvoiceDetail (invoice_id, product_id, quantity, unit_price, subtotal)
                             VALUES (@invoice_id, @product_id, @quantity, @unit_price, @subtotal)";
            
            foreach (var detail in details)
            {
                using var cmd = new SqlCommand(query, conn, transaction);
                cmd.Parameters.AddWithValue("@invoice_id", detail.InvoiceId);
                cmd.Parameters.AddWithValue("@product_id", detail.ProductId);
                cmd.Parameters.AddWithValue("@quantity", detail.Quantity);
                cmd.Parameters.AddWithValue("@unit_price", detail.UnitPrice);
                cmd.Parameters.AddWithValue("@subtotal", detail.Subtotal);
                cmd.ExecuteNonQuery();
            }
            
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
    
    public void DeleteByInvoiceId(int invoiceId)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "DELETE FROM InvoiceDetail WHERE invoice_id = @invoiceId";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
        
        cmd.ExecuteNonQuery();
    }
    
    public decimal GetInvoiceTotal(int invoiceId)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "SELECT ISNULL(SUM(subtotal), 0) FROM InvoiceDetail WHERE invoice_id = @invoiceId";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@invoiceId", invoiceId);
        
        return Convert.ToDecimal(cmd.ExecuteScalar());
    }
}