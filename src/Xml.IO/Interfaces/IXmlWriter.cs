namespace Xml.IO.Interfaces
{
    public interface IXmlWriter
    {
        Task WriteAsync<T>(string filePath, T data, CancellationToken cancellationToken = default);
    }
}
