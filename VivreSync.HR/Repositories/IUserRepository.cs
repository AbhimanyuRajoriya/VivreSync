using VivreSync.Model.Entities;

namespace VivreSync.HR.Repositories;

public interface IUserRepository
{
    Users? GetUser(string username);
    void Add(Users user);
    void Save();
}
