using BooksLibrary.Models;
using System.Xml.Serialization;
using Xml.IO.Interfaces;

namespace Xml.IO.Tests
{
    public class XmlReaderTests
    {
        private readonly IXmlReader _xmlReader;

        public XmlReaderTests()
        {
            _xmlReader = new XmlReader();
        }

        [Fact]
        public async Task ReadAsync_ShouldDeserializeXml_WhenFileExists()
        {
            // Arrange
            var model = new Book
            {
                Title = "Test",
                Author = "Author",
                Pages = 150
            };

            var path = Path.GetTempFileName();

            try
            {
                var serializer = new XmlSerializer(typeof(Book));

                await using (var stream = new FileStream(path, FileMode.Create))
                {
                    serializer.Serialize(stream, model);
                }

                // Act
                var result = await _xmlReader.ReadAsync<Book>(path);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(model.Title, result.Title);
                Assert.Equal(model.Author, result.Author);
                Assert.Equal(model.Pages, result.Pages);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public async Task ReadAsync_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
        {
            // Arrange
            var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xml");

            // Act
            var act = () => _xmlReader.ReadAsync<Book>(path);

            // Assert
            var exception = await Assert.ThrowsAsync<FileNotFoundException>(act);

            Assert.Contains("XML file was not found", exception.Message);
            Assert.Equal(path, exception.FileName);
        }


        [Fact]
        public async Task ReadAsync_ShouldReturnNull_WhenXmlContainsNullObject()
        {
            // Arrange
            var path = Path.GetTempFileName();

            try
            {
                await File.WriteAllTextAsync(path, string.Empty);

                // Act
                var act = () => _xmlReader.ReadAsync<Book>(path);

                // Assert
                await Assert.ThrowsAsync<InvalidOperationException>(act);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public async Task ReadAsync_ShouldSupportCancellationToken()
        {
            // Arrange
            var model = new Book
            {
                Title = "Test",
                Author = "Author",
                Pages = 150
            };

            var path = Path.GetTempFileName();

            try
            {
                var serializer = new XmlSerializer(typeof(Book));

                await using (var stream = new FileStream(path, FileMode.Create))
                {
                    serializer.Serialize(stream, model);
                }

                using var cancellationTokenSource = new CancellationTokenSource();

                // Act
                var result = await _xmlReader.ReadAsync<Book>(
                    path,
                    cancellationTokenSource.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(150, result.Pages);
                Assert.Equal("Test", result.Title);
                Assert.Equal("Author", result.Author);
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}
