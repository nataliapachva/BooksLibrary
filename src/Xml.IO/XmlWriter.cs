using System.Xml.Serialization;
using Xml.IO.Interfaces;

namespace Xml.IO
{
    public class XmlWriter : IXmlWriter
    {
        public async Task WriteAsync<T>(
            string filePath,
            T data,
            CancellationToken cancellationToken = default)
        {
            await using var stream = new FileStream(
                filePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None);

            var serializer = new XmlSerializer(typeof(T));

            serializer.Serialize(stream, data);

            await stream.FlushAsync(cancellationToken);
        }
    }
}
