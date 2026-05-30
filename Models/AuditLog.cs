using System;

namespace Sistema_Facturacion.Models;

public class AuditLog
{
    public long Id { get; set; }
    public int? UserId { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // acciones del usuario
    public int RecordId { get; set; }
    public string? OldData { get; set; }
    public string? NewData { get; set; }
    public DateTime ActionDate { get; set; } = DateTime.Now;
    public virtual AppUser? User { get; set; }
}