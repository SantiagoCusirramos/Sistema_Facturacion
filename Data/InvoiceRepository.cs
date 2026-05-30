using System;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using Sistema_Facturacion.Models;
using Sistema_Facturacion.Helpers;

namespace Sistema_Facturacion.Data;

public class InvoiceRepository
{
    public ObservableCollection<Invoice> GetAll()
    {
        var invoices = new ObservableCollection<Invoice>();
        
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = @"SELECT i.id, i.invoice_number, i.customer_id, i.payment_method_id, 
                                i.document_type_id, i.issue_date, i.subtotal, i.tax, i.total, i.status,
                                c.name as customer_name, pm.name as payment_method_name, dt.name as document_type_name
                         FROM Invoice i
                         LEFT JOIN Customer c ON i.customer_id = c.id
                         LEFT JOIN PaymentMethod pm ON i.payment_method_id = pm.id
                         LEFT JOIN DocumentType dt ON i.document_type_id = dt.id
                         ORDER BY i.issue_date DESC";
        
        using var cmd = new SqlCommand(query, conn);
        using var reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            var invoice = new Invoice
            {
                Id = reader.GetInt32(0),
                InvoiceNumber = reader.GetString(1),
                CustomerId = reader.GetInt32(2),
                PaymentMethodId = reader.GetInt32(3),
                DocumentTypeId = reader.GetInt32(4),
                IssueDate = reader.GetDateTime(5),
                Subtotal = reader.GetDecimal(6),
                Tax = reader.GetDecimal(7),
                Total = reader.GetDecimal(8),
                Status = reader.GetString(9)
            };
            
            if (!reader.IsDBNull(10))
            {
                invoice.Customer = new Customer { Id = invoice.CustomerId, Name = reader.GetString(10) };
            }
            if (!reader.IsDBNull(11))
            {
                invoice.PaymentMethod = new PaymentMethod { Id = invoice.PaymentMethodId, Name = reader.GetString(11) };
            }
            if (!reader.IsDBNull(12))
            {
                invoice.DocumentType = new DocumentType { Id = invoice.DocumentTypeId, Name = reader.GetString(12) };
            }
            
            invoices.Add(invoice);
        }
        
        return invoices;
    }
    
    public ObservableCollection<Invoice> GetByCustomer(int customerId)
    {
        var invoices = new ObservableCollection<Invoice>();
        
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = @"SELECT id, invoice_number, customer_id, payment_method_id, 
                                document_type_id, issue_date, subtotal, tax, total, status
                         FROM Invoice 
                         WHERE customer_id = @customerId
                         ORDER BY issue_date DESC";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@customerId", customerId);
        using var reader = cmd.ExecuteReader();
        
        while (reader.Read())
        {
            invoices.Add(new Invoice
            {
                Id = reader.GetInt32(0),
                InvoiceNumber = reader.GetString(1),
                CustomerId = reader.GetInt32(2),
                PaymentMethodId = reader.GetInt32(3),
                DocumentTypeId = reader.GetInt32(4),
                IssueDate = reader.GetDateTime(5),
                Subtotal = reader.GetDecimal(6),
                Tax = reader.GetDecimal(7),
                Total = reader.GetDecimal(8),
                Status = reader.GetString(9)
            });
        }
        
        return invoices;
    }
    
    public Invoice? GetById(int id)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = @"SELECT i.id, i.invoice_number, i.customer_id, i.payment_method_id, 
                                i.document_type_id, i.issue_date, i.subtotal, i.tax, i.total, i.status,
                                c.name as customer_name, c.document_id as customer_document,
                                pm.name as payment_method_name, dt.name as document_type_name
                         FROM Invoice i
                         LEFT JOIN Customer c ON i.customer_id = c.id
                         LEFT JOIN PaymentMethod pm ON i.payment_method_id = pm.id
                         LEFT JOIN DocumentType dt ON i.document_type_id = dt.id
                         WHERE i.id = @id";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        
        if (reader.Read())
        {
            var invoice = new Invoice
            {
                Id = reader.GetInt32(0),
                InvoiceNumber = reader.GetString(1),
                CustomerId = reader.GetInt32(2),
                PaymentMethodId = reader.GetInt32(3),
                DocumentTypeId = reader.GetInt32(4),
                IssueDate = reader.GetDateTime(5),
                Subtotal = reader.GetDecimal(6),
                Tax = reader.GetDecimal(7),
                Total = reader.GetDecimal(8),
                Status = reader.GetString(9)
            };
            
            if (!reader.IsDBNull(10))
            {
                invoice.Customer = new Customer 
                { 
                    Id = invoice.CustomerId, 
                    Name = reader.GetString(10),
                    DocumentId = reader.IsDBNull(11) ? "" : reader.GetString(11)
                };
            }
            if (!reader.IsDBNull(12))
            {
                invoice.PaymentMethod = new PaymentMethod { Id = invoice.PaymentMethodId, Name = reader.GetString(12) };
            }
            if (!reader.IsDBNull(13))
            {
                invoice.DocumentType = new DocumentType { Id = invoice.DocumentTypeId, Name = reader.GetString(13) };
            }
            
            return invoice;
        }
        
        return null;
    }
    
    public string GenerateInvoiceNumber()
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string year = DateTime.Now.Year.ToString();
        string query = @"SELECT COUNT(*) + 1 FROM Invoice 
                         WHERE YEAR(issue_date) = @year";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@year", year);
        
        int nextNumber = (int)cmd.ExecuteScalar();
        return $"FACT-{year}-{nextNumber:D4}";
    }
    
    public int Create(Invoice invoice)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = @"INSERT INTO Invoice (invoice_number, customer_id, payment_method_id, 
                                              document_type_id, issue_date, subtotal, tax, total, status)
                         VALUES (@invoice_number, @customer_id, @payment_method_id, 
                                 @document_type_id, @issue_date, @subtotal, @tax, @total, @status);
                         SELECT SCOPE_IDENTITY();";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@invoice_number", invoice.InvoiceNumber);
        cmd.Parameters.AddWithValue("@customer_id", invoice.CustomerId);
        cmd.Parameters.AddWithValue("@payment_method_id", invoice.PaymentMethodId);
        cmd.Parameters.AddWithValue("@document_type_id", invoice.DocumentTypeId);
        cmd.Parameters.AddWithValue("@issue_date", invoice.IssueDate);
        cmd.Parameters.AddWithValue("@subtotal", invoice.Subtotal);
        cmd.Parameters.AddWithValue("@tax", invoice.Tax);
        cmd.Parameters.AddWithValue("@total", invoice.Total);
        cmd.Parameters.AddWithValue("@status", invoice.Status);
        
        return Convert.ToInt32(cmd.ExecuteScalar());
    }
    
    public void Update(Invoice invoice)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = @"UPDATE Invoice SET 
                            customer_id = @customer_id, 
                            payment_method_id = @payment_method_id, 
                            document_type_id = @document_type_id, 
                            subtotal = @subtotal, 
                            tax = @tax, 
                            total = @total, 
                            status = @status
                         WHERE id = @id";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", invoice.Id);
        cmd.Parameters.AddWithValue("@customer_id", invoice.CustomerId);
        cmd.Parameters.AddWithValue("@payment_method_id", invoice.PaymentMethodId);
        cmd.Parameters.AddWithValue("@document_type_id", invoice.DocumentTypeId);
        cmd.Parameters.AddWithValue("@subtotal", invoice.Subtotal);
        cmd.Parameters.AddWithValue("@tax", invoice.Tax);
        cmd.Parameters.AddWithValue("@total", invoice.Total);
        cmd.Parameters.AddWithValue("@status", invoice.Status);
        
        cmd.ExecuteNonQuery();
    }
    
    public void UpdateStatus(int id, string status)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        
        string query = "UPDATE Invoice SET status = @status WHERE id = @id";
        
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@status", status);
        
        cmd.ExecuteNonQuery();
    }
    
    public void Cancel(int id)
    {
        UpdateStatus(id, "CANCELLED");
    }
}