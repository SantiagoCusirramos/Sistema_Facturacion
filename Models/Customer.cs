using System;

namespace Sistema_Facturacion.Models;

public class Customer
{
    public int Id { get; set; }
    public string DocumentId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}