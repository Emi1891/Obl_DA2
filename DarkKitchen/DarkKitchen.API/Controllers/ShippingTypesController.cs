using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.ShippingTypeDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.API.Controllers;

[ApiController]
[Route("api/shippingtypes")]
public class ShippingTypesController(IShippingTypeService shippingTypeService) : ControllerBase
{
    private readonly IShippingTypeService _shippingTypeService = shippingTypeService;

    [Authorize(Policy = nameof(Permission.GetShippingTypes))]
    [HttpGet]
    public IActionResult GetAll()
    {
        var shippingTypes = _shippingTypeService.GetAll();
        return Ok(shippingTypes);
    }

    [Authorize(Policy = nameof(Permission.GetShippingTypes))]
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var shippingType = _shippingTypeService.GetById(id);
        return Ok(shippingType);
    }

    [Authorize(Policy = nameof(Permission.CreateShippingType))]
    [HttpPost]
    public IActionResult Create([FromBody] ShippingTypeDto dto)
    {
        var created = _shippingTypeService.Create(dto);
        return Created(string.Empty, created);
    }

    [Authorize(Policy = nameof(Permission.UpdateShippingType))]
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] ShippingTypeDto dto)
    {
        var updated = _shippingTypeService.Update(id, dto);
        return Ok(updated);
    }
}
