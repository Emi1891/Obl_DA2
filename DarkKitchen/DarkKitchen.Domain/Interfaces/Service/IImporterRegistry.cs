namespace DarkKitchen.Domain.Interfaces.Service;

public interface IImporterRegistry
{
    IEnumerable<IProductImporter> GetAll();
    IProductImporter Get(string name);
    void Refresh();
    void InstallImporter(string fileName, Stream content);
}
