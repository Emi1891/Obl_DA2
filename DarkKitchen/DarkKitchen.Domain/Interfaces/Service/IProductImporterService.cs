using DarkKitchen.Models.ProductDTOs;

namespace DarkKitchen.Domain.Interfaces.Service;

public interface IProductImporterService
{
    IEnumerable<ImporterInfoDto> GetAvailableImporters();
    int ImportProducts(string importerName, Stream source);
    void UploadImporter(string fileName, Stream content);
}
