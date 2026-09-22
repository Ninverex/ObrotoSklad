namespace ObrotoSklad.Application.Products;

public interface IProductService
{
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task<ProductDto?> GetByIdAsync(int id);
    Task<List<ProductDto>> GetAllAsync();
    Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto);
    Task<bool> DeleteAsync(int id);

}
