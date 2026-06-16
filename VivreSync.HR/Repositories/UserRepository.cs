using VivreSync.Model.Entities;
using VivreSync.Structure.Data;

namespace VivreSync.HR.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
    public Users? GetUser(string username)
    {
        return _context.Users.FirstOrDefault(x => x.UserName == username);
    }
    public void Add(Users user)
    {
        _context.Users.Add(user);
    }
    public void Save()
    {
        _context.SaveChanges();
    }
}
