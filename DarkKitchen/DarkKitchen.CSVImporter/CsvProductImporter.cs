using System.Globalization;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Interfaces.Service;
using DarkKitchen.Models.ProductDTOs;

namespace DarkKitchen.CSVImporter;

public class CsvProductImporter : IProductImporter
{
    public string Name => "CSV";

    public string Extension => ".csv";

    public IEnumerable<ImportedProductDto> Import(Stream source)
    {
        using var reader = new StreamReader(source);

        var headerLine = reader.ReadLine();
        if(headerLine == null)
        {
            throw new BadRequestException("CSV content was empty.");
        }

        var headers = ParseLine(headerLine);
        var columnIndex = BuildColumnIndex(headers);

        var results = new List<ImportedProductDto>();
        string? line;
        var lineNumber = 1;

        while((line = reader.ReadLine()) != null)
        {
            lineNumber++;
            if(string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var fields = ParseLine(line);
            results.Add(ParseRow(fields, columnIndex, lineNumber));
        }

        return results;
    }

    private static ImportedProductDto ParseRow(
        IList<string> fields,
        IReadOnlyDictionary<string, int> columnIndex,
        int lineNumber)
    {
        var code = GetField(fields, columnIndex, "code", lineNumber);
        var name = GetField(fields, columnIndex, "name", lineNumber);
        var priceText = GetField(fields, columnIndex, "price", lineNumber);

        if(!decimal.TryParse(priceText, NumberStyles.Number, CultureInfo.InvariantCulture, out var price))
        {
            throw new BadRequestException($"Invalid price value '{priceText}' on line {lineNumber}.");
        }

        var description = GetOptionalField(fields, columnIndex, "description");
        var productLine = GetOptionalField(fields, columnIndex, "productline");
        var category = GetOptionalField(fields, columnIndex, "category");
        var imagePathsRaw = GetOptionalField(fields, columnIndex, "imagepaths");

        var imagePaths = string.IsNullOrWhiteSpace(imagePathsRaw)
            ? (IReadOnlyList<string>)[]
            : imagePathsRaw.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return new ImportedProductDto(
            code: code,
            name: name,
            price: price,
            description: string.IsNullOrWhiteSpace(description) ? null : description,
            productLine: string.IsNullOrWhiteSpace(productLine) ? null : productLine,
            category: string.IsNullOrWhiteSpace(category) ? null : category,
            imagePaths: imagePaths);
    }

    private static string GetField(
        IList<string> fields,
        IReadOnlyDictionary<string, int> columnIndex,
        string columnName,
        int lineNumber)
    {
        if(!columnIndex.TryGetValue(columnName, out var idx))
        {
            throw new BadRequestException($"Missing required column '{columnName}' in CSV header.");
        }

        if(idx >= fields.Count || string.IsNullOrWhiteSpace(fields[idx]))
        {
            throw new BadRequestException($"Missing required value for '{columnName}' on line {lineNumber}.");
        }

        return fields[idx].Trim();
    }

    private static string? GetOptionalField(
        IList<string> fields,
        IReadOnlyDictionary<string, int> columnIndex,
        string columnName)
    {
        if(!columnIndex.TryGetValue(columnName, out var idx) || idx >= fields.Count)
        {
            return null;
        }

        return fields[idx].Trim();
    }

    private static Dictionary<string, int> BuildColumnIndex(IList<string> headers)
    {
        var index = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for(var i = 0; i < headers.Count; i++)
        {
            var name = headers[i].Trim().ToLowerInvariant();
            if(!string.IsNullOrEmpty(name))
            {
                index.TryAdd(name, i);
            }
        }

        return index;
    }

    private static List<string> ParseLine(string line)
    {
        var fields = new List<string>();
        var i = 0;

        while(i <= line.Length)
        {
            if(i == line.Length)
            {
                fields.Add(string.Empty);
                break;
            }

            if(line[i] == '"')
            {
                var sb = new System.Text.StringBuilder();
                i++;
                while(i < line.Length)
                {
                    if(line[i] == '"')
                    {
                        i++;
                        if(i < line.Length && line[i] == '"')
                        {
                            sb.Append('"');
                            i++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        sb.Append(line[i]);
                        i++;
                    }
                }

                fields.Add(sb.ToString());
                if(i < line.Length && line[i] == ',')
                {
                    i++;
                }
            }
            else
            {
                var start = i;
                while(i < line.Length && line[i] != ',')
                {
                    i++;
                }

                fields.Add(line[start..i]);
                if(i < line.Length)
                {
                    i++;
                }
            }
        }

        return fields;
    }
}
