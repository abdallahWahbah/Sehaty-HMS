using Sehaty.Core.Entites;
using Sehaty.Core.Repository.Contracts;
using Sehaty.Core.UnitOfWork.Contract;
using Sehaty.Infrastructure.Data.Contexts;
using Sehaty.Infrastructure.Repositories;

namespace Sehaty.Infrastructure.UnitOfWork
{

    public class UnitOfWork : IUnitOfWork
    {
        private readonly SehatyDbContext context;

        // Dictionary Of Repos That Every Repo Created To Pass It To User If He Ask For It Again
        private readonly Dictionary<string, object> repositories = new();

        public UnitOfWork(SehatyDbContext context)
        {
            this.context = context;
        }

        // To Check If The User Asked For This Repo Before And If Not Then Create New One And Pass It To Him
        public IRepository<T> Repository<T>() where T : BaseEntity
        {
            var key = typeof(T).Name;
            if (!repositories.ContainsKey(key))
            {
                var repo = new Repository<T>(context);
                repositories.Add(key, repo);
            }

            return (IRepository<T>)repositories[key];
        }

        public async Task<int> CommitAsync() // This Is To Save All Changes Happened At Once
            => await context.SaveChangesAsync();

        public void Dispose() // To Close Connection With Database Once The Request Has Handeled Successfully
            => context.Dispose();
    }
}
