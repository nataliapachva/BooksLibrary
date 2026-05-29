namespace Xml.IO.Interfaces
{
    public interface IXmlReader
    {
        Task<T?> ReadAsync<T>(string filePath, CancellationToken cancellationToken = default);
    }
}
