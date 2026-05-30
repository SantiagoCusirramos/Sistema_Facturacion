namespace Sistema_Facturacion.Models;

public class DocumentType
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty; // factura, recibo
}