using System;

namespace ObrotoSklad.Domain.Entities;

public class Invoice
{
    public int Id { get; set; }
    public string? InvoiceNumber { get; set; }
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public DateOnly IssueDate { get; set; }
    public DateOnly DueDate { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalNet { get; set; }
    public decimal TotalVat { get; set; }
    public decimal TotalGross { get; set; }
}
