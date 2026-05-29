using System.Xml.Serialization;
using Xml.IO.Interfaces;

namespace Xml.IO
{
    public class XmlReader : IXmlReader
    {
        public async Task<T?> ReadAsync<T>(
            string filePath,
            CancellationToken cancellationToken = default)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(
                   $"XML file was not found: {filePath}",
                   filePath);

            await using var stream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            var serializer = new XmlSerializer(typeof(T));

            return (T?)serializer.Deserialize(stream);
        }
    }
}
