using System;
using System.Dynamic;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;

namespace ObrotoSklad.Domain.Entities;


public class StockMovement
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public MovementType Type { get; set; }
    public int Quantity { get; set; }
    public int? OrderId { get; set; }
    public Order? Order { get; set; }
    public string? CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
