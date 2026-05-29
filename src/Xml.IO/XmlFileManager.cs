using Xml.IO.Interfaces;

namespace Xml.IO
{
    public class XmlFileManager : IXmlFileManager
    {
        private readonly IXmlReader _xmlReader;
        private readonly IXmlWriter _xmlWriter;

        public XmlFileManager(IXmlReader xmlReader, IXmlWriter xmlWriter)
        {
            _xmlReader = xmlReader;
            _xmlWriter = xmlWriter;
        }
        public async Task<T?> ReadAsync<T>(string filePath, CancellationToken cancellationToken = default)
        {
            return await _xmlReader.ReadAsync<T>(filePath, cancellationToken);
        }

        public async Task WriteAsync<T>(string filePath, T data, CancellationToken cancellationToken = default)
        {
            await _xmlWriter.WriteAsync(filePath, data, cancellationToken);
        }
    }
}
