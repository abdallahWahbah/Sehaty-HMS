namespace Sehaty.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<ApplicationUser> GetByIdAsync(int id)
            => await userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

        public async Task<ApplicationUser> GetByEmailAsync(string email)
            => await userManager.FindByEmailAsync(email);

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
            => await userManager.Users.ToListAsync();

        // ✔️ Checks
        public async Task<bool> ExistsByIdAsync(int id)
            => await userManager.Users.AnyAsync(u => u.Id == id);

        public async Task<bool> ExistsByEmailAsync(string email)
            => await userManager.Users.AnyAsync(u => u.Email == email);

        public async Task<bool> ExistsByUsernameAsync(string username)
            => await userManager.FindByNameAsync(username) != null;


        // ✔️ Queries
        public async Task<IEnumerable<ApplicationUser>> FindAsync(Expression<Func<ApplicationUser, bool>> predicate)
            => await userManager.Users.Where(predicate).ToListAsync();

        public async Task<ApplicationUser> GetFirstOrDefaultAsync(Expression<Func<ApplicationUser, bool>> predicate)
            => await userManager.Users.FirstOrDefaultAsync(predicate);


        // ✔️ CRUD
        public async Task AddAsync(ApplicationUser user)
            => await userManager.CreateAsync(user);

        public async Task UpdateAsync(ApplicationUser user)
            => await userManager.UpdateAsync(user);

        public async Task DeleteAsync(ApplicationUser user)
            => await userManager.DeleteAsync(user);
    }

}
