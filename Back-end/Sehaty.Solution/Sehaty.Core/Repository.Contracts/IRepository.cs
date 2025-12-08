namespace Sehaty.Core.Repository.Contracts
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id, bool asNoTracking = false);
        Task<IEnumerable<T>> GetAllWithSpecAsync(ISpecefication<T> spec);
        Task<T> GetByIdWithSpecAsync(ISpecefication<T> spec);
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        void Update(T entity);
        void Delete(T entity);
        Task AddAsync(T entity);
        Task<int> CountAsync(ISpecefication<T> spec);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task AddRangeAsync(IEnumerable<T> entities);
        void DeleteRange(IEnumerable<T> entities);
    }
}
