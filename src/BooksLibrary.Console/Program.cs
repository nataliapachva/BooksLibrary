using BooksLibrary.Extensions;
using BooksLibrary.Models;
using BooksLibrary.Repositories;
using Xml.IO;

var filePath = "..\\..\\..\\books.xml";

var xmlFileManager = new XmlFileManager(new XmlReader(), new XmlWriter());
var xmlRepository = new XmlRepository<Book>(filePath, xmlFileManager);

var books = await xmlRepository.GetAllAsync();


await xmlRepository.AddAsync(new Book
{
    Author = "Shevchenko",
    Title = "Kobzar",
    Pages = 250
});

var kobzar = await xmlRepository.SearchAsync(x => x.Title.Contains("bzar", StringComparison.OrdinalIgnoreCase));

books = await xmlRepository.GetAllAsync();

var sortedBooks = books.SortByAuthorAndTitle().ToList();

var  sortedBooksFilePath = Path.Combine("..\\..\\..\\", "sortedBooks.xml");
await xmlFileManager.WriteAsync(sortedBooksFilePath, sortedBooks);