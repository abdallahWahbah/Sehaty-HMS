using Sehaty.Core.Entites;
using Sehaty.Core.Repository.Contracts;

namespace Sehaty.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
    }
}
