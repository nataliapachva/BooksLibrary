using BooksLibrary.Extensions;
using BooksLibrary.Models;

namespace BooksLibrary.Tests.Extensions
{
    public class BookSorterTests
    {
        [Fact]
        public void SortByAuthorAndTitle_ShouldSortBooksByAuthorFirst_ThenByTitle()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book { Author = "Sofia", Title = "Summer" },
                new Book { Author = "Alex", Title = "Spring" },
                new Book { Author = "Sofia", Title = "Autumn" },
                new Book { Author = "Bogdan", Title = "Winter" }
            };

            // Act
            var result = books.SortByAuthorAndTitle().ToList();

            // Assert
            Assert.Equal("Alex", result[0].Author);
            Assert.Equal("Spring", result[0].Title);

            Assert.Equal("Bogdan", result[1].Author);
            Assert.Equal("Winter", result[1].Title);

            Assert.Equal("Sofia", result[2].Author);
            Assert.Equal("Autumn", result[2].Title);

            Assert.Equal("Sofia", result[3].Author);
            Assert.Equal("Summer", result[3].Title);
        }

        [Fact]
        public void SortByAuthorAndTitle_ShouldReturnEmptyCollection_WhenInputIsEmpty()
        {
            // Arrange
            var books = new List<Book>();

            // Act
            var result = books.SortByAuthorAndTitle();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void SortByAuthorAndTitle_ShouldKeepSingleItemCollectionUnchanged()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book { Author = "Sofia", Title = "Autumn" }
            };

            // Act
            var result = books.SortByAuthorAndTitle().ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("Sofia", result[0].Author);
            Assert.Equal("Autumn", result[0].Title);
        }
    }
}
