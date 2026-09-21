namespace ObrotoSklad.Application.Products;

public record ProductDto(int Id, string SKU, string Name, string Description, string Unit, decimal Price);
