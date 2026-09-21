using System;
using System.ComponentModel.DataAnnotations;

namespace ObrotoSklad.Domain;

public class StockItem
{
    [Key]
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int QuantityOnHand { get; set; }
    public int QuantityReserved { get; set; }
}
