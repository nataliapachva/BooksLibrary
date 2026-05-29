using BooksLibrary.Models;
using Moq;
using Xml.IO.Interfaces;

namespace Xml.IO.Tests
{
    public class XmlFileManagerTests
    {
        private readonly Mock<IXmlReader> _xmlReaderMock;
        private readonly Mock<IXmlWriter> _xmlWriterMock;
        private readonly IXmlFileManager _xmlFileManager;

        public XmlFileManagerTests()
        {
            _xmlReaderMock = new Mock<IXmlReader>();
            _xmlWriterMock = new Mock<IXmlWriter>();
            _xmlFileManager = new XmlFileManager(_xmlReaderMock.Object, _xmlWriterMock.Object);
        }

        [Fact]
        public async Task ReadAsync_ShouldCallXmlReaderAndReturnResult()
        {
            // Arrange
            var filePath = "test.xml";

            var book = new Book
            {
                Title = "Test",
                Author = "Author",
                Pages = 150
            };

            _xmlReaderMock
                .Setup(x => x.ReadAsync<Book>(filePath))
                .ReturnsAsync(book);

            // Act
            var result = await _xmlFileManager.ReadAsync<Book>(filePath);

            // Assert
            _xmlReaderMock.Verify(
                 x => x.ReadAsync<Book>(filePath),
                 Times.Once);
            Assert.NotNull(result);
            Assert.Same(book, result);
        }


        [Fact]
        public async Task WriteAsync_ShouldCallXmlWriter()
        {
            // Arrange
            var filePath = "test.xml";

            var book = new Book
            {
                Title = "Test",
                Author = "Author",
                Pages = 150
            };

            _xmlWriterMock
                .Setup(x => x.WriteAsync(filePath, book))
                .Returns(Task.CompletedTask);

            // Act
            await _xmlFileManager.WriteAsync<Book>(filePath, book);

            // Assert
            _xmlWriterMock.Verify(
                 x => x.WriteAsync(filePath, book),
                 Times.Once);
        }
    }
}
