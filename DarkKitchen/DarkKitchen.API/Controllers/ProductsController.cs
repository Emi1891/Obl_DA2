using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.DateDTOs;
using DarkKitchen.Models.ProductDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.API.Controllers;

[Route("api/products")]
[ApiController]
public class ProductsController(IProductService productService) : ControllerBase
{
    private readonly IProductService _productService = productService;

    [Authorize(Policy = nameof(Permission.CreateProduct))]
    [HttpPost]
    public IActionResult CreateProduct([FromBody] CreateProductDto newProduct)
    {
        var responsibleUser = User.Identity!.Name!;
        _productService.CreateProduct(newProduct, responsibleUser);
        return Created("Product created correctly.", null);
    }

    [Authorize(Policy = nameof(Permission.UpdateProduct))]
    [HttpPut]
    public IActionResult UpdateProduct([FromBody] ProductDto product)
    {
        var responsibleUser = User.Identity!.Name!;
        _productService.UpdateProduct(product, responsibleUser);
        return Ok("Product updated correctly.");
    }

    [Authorize(Policy = nameof(Permission.GetProducts))]
    [HttpGet]
    public IActionResult GetProducts([FromQuery] ProductFilterDto filters)
    {
        var products = _productService.GetProducts(filters);
        return Ok(products);
    }

    [Authorize(Policy = nameof(Permission.GetMostPopularProducts))]
    [HttpGet("most-requested")]
    public IActionResult GetMostRequestedProducts([FromQuery] DateRangeDto dates)
    {
        var products = _productService.GetMostRequestedProducts(dates);
        return Ok(products);
    }

    [Authorize(Policy = nameof(Permission.UpdateProductStatus))]
    [HttpPatch("{id}")]
    public IActionResult UpdateStatus(int id, [FromBody] ProductStatusDto status)
    {
        var responsibleUser = User.Identity!.Name!;
        _productService.UpdateStatus(id, status, responsibleUser);
        return Ok("Product status updated correctly.");
    }
}
