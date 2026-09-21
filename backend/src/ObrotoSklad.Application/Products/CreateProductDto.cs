using System.ComponentModel;
using System.Reflection;

namespace ObrotoSklad.Application.Products;

public record CreateProductDto(string SKU, string Name, string Description, string Unit, decimal Price);
