using VivreSync.Model.Entities;
using VivreSync.Structure.Data;
namespace VivreSync.Authentication.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public Users? GetUserByUsernameAsync(string username)
        {
            return _context.Users.FirstOrDefault(u => u.UserName == username);
        }
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
