using BooksLibrary.Repositories.Interfaces;
using Xml.IO.Interfaces;

namespace BooksLibrary.Repositories
{
    public class XmlRepository<T> : IRepository<T>
    {
        private readonly string _filePath;
        private readonly IXmlFileManager _xmlFileManager;

        public XmlRepository(string filePath, IXmlFileManager xmlFileManager)
        {
            _filePath = filePath;
            _xmlFileManager = xmlFileManager;

        }

        public async Task<IList<T>> GetAllAsync()
        {
            return await LoadDataAsync();
        }

        public async Task<T?> GetByIdAsync(Func<T, bool> predicate)
        {
            var data = await LoadDataAsync();

            return data.FirstOrDefault(predicate);
        }

        public async Task AddAsync(T item)
        {
            var items = await LoadDataAsync();
            items.Add(item);
            await SaveDataAsync(items);
        }

        public async Task UpdateAsync(Func<T, bool> predicate, T updatedItem)
        {
            var items = await LoadDataAsync();

            var index = items.FindIndex(x => predicate(x));

            if (index == -1)
            {
                throw new Exception("Item not found.");
            }

            if (index >= 0)
            {
                items[index] = updatedItem;
                await SaveDataAsync(items);
            }
        }

        public async Task<IList<T>> SearchAsync(Func<T, bool> predicate)
        {
            var data = await LoadDataAsync();

            return data.Where(x => predicate(x)).ToList();
        }

        public async Task DeleteAsync(Func<T, bool> predicate)
        {
            var items = await LoadDataAsync();

            var item = items.FirstOrDefault(predicate);

            if (item != null)
            {
                items.Remove(item);
                await SaveDataAsync(items);
            }
        }

        private async Task<List<T>> LoadDataAsync()
        {
            return await _xmlFileManager.ReadAsync<List<T>>(_filePath);
        }

        private async Task SaveDataAsync(List<T> items)
        {
            await _xmlFileManager.WriteAsync(_filePath, items);
        }
    }
}
