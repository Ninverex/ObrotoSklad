using System;

namespace ObrotoSklad.Domain;

public class Product
{
    public int Id { get; set; }
    public required string SKU { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Unit { get; set; }
    public decimal Price { get; set; }

}
