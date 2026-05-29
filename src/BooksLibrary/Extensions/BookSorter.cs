using BooksLibrary.Models;

namespace BooksLibrary.Extensions
{
    public static class BookSorter
    {
        public static IEnumerable<Book> SortByAuthorAndTitle(this IEnumerable<Book> books)
        {
            return books
                .OrderBy(p => p.Author)
                .ThenBy(p => p.Title);
        }
    }
}
