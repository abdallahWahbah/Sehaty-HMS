namespace Sehaty.Core.Repository.Contracts
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id, bool asNoTracking = false);
        Task<IEnumerable<T>> GetAllWithSpecAsync(ISpecefication<T> spec);
        Task<T> GetByIdWithSpecAsync(ISpecefication<T> spec);
        IQueryable<T> FindByAsync(Expression<Func<T, bool>> predicate);
        void Update(T entity);
        void Delete(T entity);
        Task AddAsync(T entity);
    }
}
