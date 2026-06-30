using VivreSync.Authentication.DTOs;

namespace VivreSync.Authentication.Services
{
    public interface IAuthService
    {
        LoginResponseDTO? Login(LoginDTO loginDTO);
        bool ChangePassword(int userId, ChangePasswordDTO dto);
    }
}
