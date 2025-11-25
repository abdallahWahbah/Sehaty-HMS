namespace Sehaty.Core.Repository.Contracts
{
    public interface IUserRepository
    {
        Task<ApplicationUser> GetByIdAsync(int id);
        Task<ApplicationUser> GetByEmailAsync(string email);
        Task<IEnumerable<ApplicationUser>> GetAllAsync();

        // Checks
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByUsernameAsync(string username);

        // Queries
        Task<IEnumerable<ApplicationUser>> FindAsync(Expression<Func<ApplicationUser, bool>> predicate);
        Task<ApplicationUser> GetFirstOrDefaultAsync(Expression<Func<ApplicationUser, bool>> predicate);

        // CRUD
        Task AddAsync(ApplicationUser user);
        Task UpdateAsync(ApplicationUser user);
        Task DeleteAsync(ApplicationUser user);
    }
}
