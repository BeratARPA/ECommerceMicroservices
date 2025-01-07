using Microsoft.AspNetCore.Mvc;
using Product.Domain.DTOs;
using Product.Infrastructure.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Product.API.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDTO productDto)
        {
            var response = await _productService.CreateProductAsync(productDto);
            if (!response.Success)
                return BadRequest(response.Message);

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockDTO stockDto)
        {
            var response = await _productService.UpdateStockAsync(id, stockDto.Stock);
            if (!response.Success)
                return BadRequest(response.Message);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var response = await _productService.DeleteProductAsync(id);
            if (!response.Success)
                return BadRequest(response.Message);

            return Ok(response);
        }
    }
}
