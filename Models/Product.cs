using System;
namespace Sistema_Facturacion.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int? CategoryId { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    public virtual Category? Category { get; set; }
}