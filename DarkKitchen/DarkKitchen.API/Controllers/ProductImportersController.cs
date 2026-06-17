using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.API.Controllers;

[Route("api/products")]
[ApiController]
public class ProductImportersController(IProductImporterService productImporterService) : ControllerBase
{
    private readonly IProductImporterService _productImporterService = productImporterService;

    [Authorize(Policy = nameof(Permission.ImportProducts))]
    [HttpGet("importers")]
    public IActionResult GetAvailableImporters()
    {
        var importers = _productImporterService.GetAvailableImporters();
        return Ok(importers);
    }

    [Authorize(Policy = nameof(Permission.ImportProducts))]
    [HttpPost("import")]
    public IActionResult ImportProducts([FromForm] string importer, IFormFile file)
    {
        if(file == null || file.Length == 0)
        {
            return BadRequest("File is required.");
        }

        using var stream = file.OpenReadStream();
        var imported = _productImporterService.ImportProducts(importer, stream);
        return Ok(new { imported, message = $"{imported} products imported correctly." });
    }

    [Authorize(Policy = nameof(Permission.ImportProducts))]
    [HttpPost("importers")]
    public IActionResult UploadImporter(IFormFile file)
    {
        if(file == null || file.Length == 0)
        {
            return BadRequest("File is required.");
        }

        using var stream = file.OpenReadStream();
        _productImporterService.UploadImporter(file.FileName, stream);
        return Ok(new { message = $"Importer '{file.FileName}' uploaded correctly." });
    }
}
