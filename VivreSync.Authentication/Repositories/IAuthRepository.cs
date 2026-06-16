using VivreSync.Model.Entities;

namespace VivreSync.Authentication.Repositories
{
    public interface IAuthRepository
    {
        Users? GetUserByUsernameAsync(string username);
        void SaveChanges();
    }
}
