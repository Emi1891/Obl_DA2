using System.Text.Json;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.ProductDTOs;

namespace DarkKitchen.Services.Importers;

public class JsonProductImporter : IProductImporter
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public string Name => "JSON";

    public string Extension => ".json";

    public IEnumerable<ImportedProductDto> Import(Stream source)
    {
        try
        {
            return JsonSerializer.Deserialize<List<ImportedProductDto>>(source, Options)
                ?? throw new BadRequestException("JSON content was empty.");
        }
        catch(JsonException ex)
        {
            throw new BadRequestException($"Invalid JSON: {ex.Message}");
        }
    }
}
