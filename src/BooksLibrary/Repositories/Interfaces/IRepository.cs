namespace BooksLibrary.Repositories.Interfaces
{
    public interface IRepository<T>
    {
        Task<IList<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Func<T, bool> predicate);
        Task AddAsync(T item);
        Task UpdateAsync(Func<T, bool> predicate, T updatedItem);
        Task DeleteAsync(Func<T, bool> predicate);
    }
}
