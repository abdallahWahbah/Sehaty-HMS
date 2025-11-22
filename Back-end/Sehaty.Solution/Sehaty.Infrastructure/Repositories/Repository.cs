using Microsoft.EntityFrameworkCore;
using Sehaty.Core.Entites;
using Sehaty.Core.Repository.Contracts;
using Sehaty.Core.Specefications;
using Sehaty.Infrastructure.Data.Contexts;
using Sehaty.Infrastructure.Specifications;
using System.Linq.Expressions;

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
            => await GenerateSpec(spec).AsNoTracking().ToListAsync();


        public async Task<T> GetByIdWithSpecAsync(ISpecefication<T> spec)
            => await GenerateSpec(spec).FirstOrDefaultAsync();

        public async Task<T> GetByIdAsync(int id, bool asNoTracking = false)
        {
            if (asNoTracking)
                return await Set.AsNoTracking().FirstOrDefaultAsync(E => E.Id == id);
            return await Set.FindAsync(id);
        }
        public IQueryable<T> FindByAsync(Expression<Func<T, bool>> predicate)
            => Set.Where(predicate);
        public async Task AddAsync(T entity)
            => await Set.AddAsync(entity);
        public void Delete(T entity)
            => Set.Remove(entity);
        public void Update(T entity)
            => Set.Update(entity);


        IQueryable<T> GenerateSpec(ISpecefication<T> spec) => SpecificationEvaluator<T>.GetQuery(Set, spec);
    }
}

