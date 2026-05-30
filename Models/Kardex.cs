using System;

namespace Sistema_Facturacion.Models;

public class Kardex
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int? InvoiceDetailId { get; set; }
    public int Quantity { get; set; }
    public int PreviousStock { get; set; }
    public int CurrentStock { get; set; }
    public string MovementType { get; set; } = string.Empty; // venta, devolucion, queja, etc
    public DateTime MovementDate { get; set; } = DateTime.Now;
    public virtual Product Product { get; set; }
    public virtual InvoiceDetail InvoiceDetail { get; set; }
}