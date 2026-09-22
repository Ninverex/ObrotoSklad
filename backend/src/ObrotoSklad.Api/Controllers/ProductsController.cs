using Microsoft.AspNetCore.Mvc;
using ObrotoSklad.Application.Products;

namespace ObrotoSklad.Api.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }
    
    
    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto dto)
    {
        var product = await _productService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }

    [HttpGet]
    public async Task<ActionResult<ProductDto>> GetProducts()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);

        }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product is null)
            {
                return NotFound();
            }
            return Ok(product);
        }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(int id, UpdateProductDto dto)
        {
            var product = await _productService.UpdateAsync(id, dto);
            if (product is null)
            {
                return NotFound();
            }
            return Ok(product);
        }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
        {
            var deleted = await _productService.DeleteAsync(id);
            if (deleted is false)
            {
                return NotFound();
            }
            return NoContent();
        }

}
}
