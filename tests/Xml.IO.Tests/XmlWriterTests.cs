using BooksLibrary.Models;
using System.Xml.Serialization;
using Xml.IO.Interfaces;

namespace Xml.IO.Tests
{
    public class XmlWriterTests
    {
        private readonly IXmlWriter _xmlWriter;

        public XmlWriterTests()
        {
            _xmlWriter = new XmlWriter();
        }
        [Fact]
        public async Task WriteAsync_ShouldCreateXmlFile()
        {
            // Arrange
            var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xml");

            var data = new Book
            {
                Title = "Test",
                Author = "Author",
                Pages = 150
            };

            try
            {

                // Act
                await _xmlWriter.WriteAsync(filePath, data);

                // Assert
                Assert.True(File.Exists(filePath));

                var content = await File.ReadAllTextAsync(filePath);

                Assert.Contains("Test", content);
                Assert.Contains("Author", content);
                Assert.Contains("150", content);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Fact]
        public async Task WriteAsync_ShouldSerializeCorrectly()
        {
            // Arrange
            var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xml");

            var expected = new Book
            {
                Title = "Serialized",
                Author = "Author",
                Pages = 250
            };

            try
            {
                // Act
                await _xmlWriter.WriteAsync(filePath, expected);

                // Assert
                await using var stream = new FileStream(filePath, FileMode.Open);

                var serializer = new XmlSerializer(typeof(Book));

                var actual = (Book)serializer.Deserialize(stream)!;

                Assert.Equal(expected.Title, actual.Title);
                Assert.Equal(expected.Author, actual.Author);
                Assert.Equal(expected.Pages, actual.Pages);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Fact]
        public async Task WriteAsync_ShouldOverwriteExistingFile()
        {
            // Arrange
            var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xml");

            await File.WriteAllTextAsync(filePath, "Old title");

            var data = new Book
            {
                Title = "New title",
                Author = "Author",
                Pages = 250
            };

            try
            {
                // Act
                await _xmlWriter.WriteAsync(filePath, data);

                // Assert
                var content = await File.ReadAllTextAsync(filePath);

                Assert.DoesNotContain("Old title", content);
                Assert.Contains("New title", content);
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Fact]
        public async Task WriteAsync_ShouldThrow_WhenPathIsInvalid()
        {
            // Arrange
            var invalidPath = "?:\\invalid\\file.xml";

            var data = new Book
            {
                Title = "Title",
                Author = "Author",
                Pages = 250
            };

            // Act & Assert
            await Assert.ThrowsAnyAsync<IOException>(() =>
                _xmlWriter.WriteAsync(invalidPath, data));
        }

        [Fact]
        public async Task WriteAsync_ShouldRespectCancellationToken_WhenFlushing()
        {
            // Arrange
            var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xml");

            var data = new Book
            {
                Title = "Title",
                Author = "Author",
                Pages = 250
            };

            using var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                _xmlWriter.WriteAsync(filePath, data, cts.Token));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
