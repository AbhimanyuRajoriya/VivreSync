using Microsoft.AspNetCore.Identity;
using VivreSync.Model.Entities;

namespace VivreSync.Authentication.Security
{
    public class PasswordHasherService : IPasswordHasherService
    {
        private readonly IPasswordHasher<Users> _passwordHasher;
        public PasswordHasherService(IPasswordHasher<Users> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }
        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(null!, password);
        }
        public bool VerifyPassword(string password, string hash)
        {
            var result = _passwordHasher.VerifyHashedPassword(
                null!,
                hash,
                password
            );
            return result == PasswordVerificationResult.Success ||
                   result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
