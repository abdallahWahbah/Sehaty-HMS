namespace Sehaty.Core.UnitOfWork.Contract
{
    public interface IUnitOfWork : IDisposable
    {

        IRepository<T> Repository<T>() where T : BaseEntity; // This Function To Create And Get The Repositry U Need When You Ask
        IUserRepository UserRepository();
        Task<int> CommitAsync(); // To Save All Changes At Once
    }
}
