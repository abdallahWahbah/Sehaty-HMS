namespace Sehaty.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly SehatyDbContext context;
        private readonly DbSet<T> Set;

        public Repository(SehatyDbContext context)
        {
            this.context = context;
            Set = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await Set.AsNoTracking().ToListAsync();

        public async Task<IEnumerable<T>> GetAllWithSpecAsync(ISpecefication<T> spec)
            => await GenerateQuery(spec).AsNoTracking().ToListAsync();


        public async Task<T> GetByIdWithSpecAsync(ISpecefication<T> spec)
            => await GenerateQuery(spec).FirstOrDefaultAsync();

        public async Task<T> GetByIdAsync(int id, bool asNoTracking = false)
        {
            if (asNoTracking)
                return await Set.AsNoTracking().FirstOrDefaultAsync(E => E.Id == id);
            return await Set.FindAsync(id);
        }


        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
            => Set.Where(predicate);

        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await Set.FirstOrDefaultAsync(predicate);
        }


        public async Task AddAsync(T entity)
            => await Set.AddAsync(entity);
        public void Delete(T entity)
            => Set.Remove(entity);
        public void Update(T entity)
            => Set.Update(entity);

        public async Task<int> CountAsync(ISpecefication<T> spec)
        {
            return await GenerateQuery(spec).CountAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await Set.AnyAsync(predicate);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await AddRangeAsync(entities);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            Set.RemoveRange(entities);
        }
        IQueryable<T> GenerateQuery(ISpecefication<T> spec) => SpecificationEvaluator<T>.GetQuery(Set, spec);

    }
}

