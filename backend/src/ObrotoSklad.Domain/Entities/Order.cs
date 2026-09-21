using System;
using System.ComponentModel.DataAnnotations;


namespace ObrotoSklad.Domain;

public class Order
{
    public int Id { get; set; }
    public string? OrderNumber { get; set; }
    
    public int CustomerId { get; set; }
    
    public Customer? Customer { get; set; }
    public OrderStatus Status { get; set; }
    public string? CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }

}
