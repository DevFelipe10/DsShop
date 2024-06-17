using DsShop.ProductApi.DTOs;
using DsShop.ProductApi.Roles;
using DsShop.ProductApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DsShop.ProductApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : Controller
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> Get()
    {
        var products = await _productService.GetProducts();

        if (products == null) return NotFound();

        return Ok(products);

    }

    [HttpGet("{id:int}", Name = "GetProduct")]
    public async Task<ActionResult<ProductDTO>> Get(int id)
    {
        var product = await _productService.GetProductById(id);

        if (product is null)
            return NotFound($"Product is not found with ID={id}");

        return Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = Role.Admin)]
    public async Task<ActionResult<ProductDTO>> Post([FromBody] ProductDTO productDTO)
    {
        if (productDTO is null)
            return BadRequest("Invalid Data");

        await _productService.AddProduct(productDTO);

        return new CreatedAtRouteResult("GetCategory",
                        new { id = productDTO.Id }, productDTO);

    }

    [HttpPut]
    [Authorize(Roles = Role.Admin)]
    public async Task<ActionResult<ProductDTO>> Put([FromBody] ProductDTO productDTO)
    {
        if (productDTO is null)
            return BadRequest("Data Invalid");

        await _productService.UpdateProduct(productDTO);

        return Ok(productDTO);

    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Role.Admin)]
    public async Task<ActionResult<ProductDTO>> Delete(int id)
    {
        var productDto = await _productService.GetProductById(id);

        if (productDto is null)
            return NotFound("Product is not found");

        await _productService.RemoveProduct(id);

        return Ok(productDto);
    }
}
