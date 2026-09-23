using System;
using System.ComponentModel.DataAnnotations;
using ObrotoSklad.Domain.Entities;


namespace ObrotoSklad.Domain;

public class Order
{
    public int Id { get; set; }
    public required string OrderNumber { get; set; }
    
    public int CustomerId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public Customer? Customer { get; set; }
    public OrderStatus Status { get; set; }
    public required string CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
}