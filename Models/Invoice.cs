using System;
using System.Collections.Generic;

namespace Sistema_Facturacion.Models;

public class Invoice
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public int PaymentMethodId { get; set; }
    public int DocumentTypeId { get; set; }
    public DateTime IssueDate { get; set; } = DateTime.Now;
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public virtual Customer? Customer { get; set; }
    public virtual PaymentMethod? PaymentMethod { get; set; }
    public virtual DocumentType? DocumentType { get; set; }
    public virtual ICollection<InvoiceDetail>? Details { get; set; }
}