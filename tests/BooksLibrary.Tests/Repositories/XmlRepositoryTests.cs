using BooksLibrary.Models;
using BooksLibrary.Repositories;
using Moq;
using Xml.IO.Interfaces;

namespace BooksLibrary.Tests.Repositories;

public class XmlRepositoryTests
{
    private readonly Mock<IXmlFileManager> _xmlFileManagerMock;
    private readonly XmlRepository<Book> _repository;

    private const string FilePath = "test.xml";

    public XmlRepositoryTests()
    {
        _xmlFileManagerMock = new Mock<IXmlFileManager>();
        _repository = new XmlRepository<Book>(FilePath, _xmlFileManagerMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_Returns_AllItems()
    {
        // Arrange
        var items = new List<Book>
        {
             new() { Title = "Book1", Author = "Author1", Pages = 100 },
             new() { Title = "Book2", Author = "Author2", Pages = 200 }
        };

        _xmlFileManagerMock
            .Setup(x => x.ReadAsync<List<Book>>(FilePath))
            .ReturnsAsync(items);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Book1", result[0].Title);
        Assert.Equal("Book2", result[1].Title);
    }

    [Fact]
    public async Task GetByIdAsync_Returns_MatchingItem()
    {
        // Arrange
        var items = new List<Book>
        {
            new() { Title = "Book1", Author = "Author1", Pages = 100 },
            new() { Title = "Book2", Author = "Author2", Pages = 200 }
        };

        _xmlFileManagerMock
            .Setup(x => x.ReadAsync<List<Book>>(FilePath))
            .ReturnsAsync(items);

        // Act
        var result = await _repository.GetByIdAsync(x => x.Pages == 200);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Book2", result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_Returns_NullWhenItemNotFound()
    {
        // Arrange
        _xmlFileManagerMock
            .Setup(x => x.ReadAsync<List<Book>>(FilePath))
            .ReturnsAsync(new List<Book>());

        // Act
        var result = await _repository.GetByIdAsync(x => x.Pages == 100);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_AddsItemAndSaves()
    {
        // Arrange
        var items = new List<Book>();

        _xmlFileManagerMock
            .Setup(x => x.ReadAsync<List<Book>>(FilePath))
            .ReturnsAsync(items);

        // Act
        await _repository.AddAsync(new Book
        {
            Title = "New Book",
            Author = "Author",
            Pages = 100,
        });

        // Assert
        _xmlFileManagerMock.Verify(
            x => x.WriteAsync(
                FilePath,
                It.Is<List<Book>>(l =>
                    l.Count == 1 &&
                    l[0].Title == "New Book")),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesItemAndSaves()
    {
        // Arrange
        var items = new List<Book>
        {
            new() { Title = "Old Title", Author = "Author", Pages = 100 }
        };

        _xmlFileManagerMock
            .Setup(x => x.ReadAsync<List<Book>>(FilePath))
            .ReturnsAsync(items);

        var updated = new Book
        {
            Title = "New Title",
            Author = "Author",
            Pages = 100
        };

        // Act
        await _repository.UpdateAsync(x => x.Title == "Old Title", updated);

        // Assert
        _xmlFileManagerMock.Verify(
            x => x.WriteAsync(
                FilePath,
                It.Is<List<Book>>(l =>
                    l[0].Title == "New Title")),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsExceptionWhenItemNotFound()
    {
        // Arrange
        _xmlFileManagerMock
            .Setup(x => x.ReadAsync<List<Book>>(FilePath))
            .ReturnsAsync(new List<Book>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            _repository.UpdateAsync(
                x => x.Pages == 1,
                new Book()));

        Assert.Equal("Item not found.", exception.Message);
    }

    [Fact]
    public async Task SearchAsync_ReturnsFilteredItems()
    {
        // Arrange
        var items = new List<Book>
        {
            new() { Title = "Summer", Author = "A", Pages = 100},
            new() { Title = "Winter" , Author = "B", Pages = 100},
            new() { Title = "Summer 2026", Author = "C", Pages = 100}
        };

        _xmlFileManagerMock
            .Setup(x => x.ReadAsync<List<Book>>(FilePath))
            .ReturnsAsync(items);

        // Act
        var result = await _repository.SearchAsync(x => x.Title.Contains("Summer"));

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task DeleteAsync_RemovesItemAndSaves()
    {
        // Arrange
        var items = new List<Book>
        {
            new() { Title = "Summer", Author = "A", Pages = 100},
            new() { Title = "Winter" , Author = "B", Pages = 100},
            new() { Title = "Summer 2026", Author = "C", Pages = 100}
        };

        _xmlFileManagerMock
            .Setup(x => x.ReadAsync<List<Book>>(FilePath))
            .ReturnsAsync(items);

        // Act
        await _repository.DeleteAsync(x => x.Title == "Winter");

        // Assert
        _xmlFileManagerMock.Verify(
            x => x.WriteAsync(
                FilePath,
                It.Is<List<Book>>(l =>
                    l.Count == 2 &&
                    l.All(b => b.Title != "Winter"))),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_DoesNothingWhenItemNotFound()
    {
        // Arrange
        var items = new List<Book>
        {
           new() { Title = "Summer", Author = "A", Pages = 100},
        };

        _xmlFileManagerMock
            .Setup(x => x.ReadAsync<List<Book>>(FilePath))
            .ReturnsAsync(items);

        // Act
        await _repository.DeleteAsync(x => x.Pages == 99);

        // Assert
        _xmlFileManagerMock.Verify(
            x => x.WriteAsync(
                It.IsAny<string>(),
                It.IsAny<List<Book>>()),
            Times.Never);
    }
}


