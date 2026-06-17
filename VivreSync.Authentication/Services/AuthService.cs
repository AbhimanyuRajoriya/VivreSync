using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using VivreSync.Authentication.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VivreSync.Authentication.DTOs;
using VivreSync.Authentication.Repositories;
using VivreSync.Model.Entities;

namespace VivreSync.Authentication.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasherService _passwordHasherService;
        public AuthService(IAuthRepository authRepository, IConfiguration configuration, IPasswordHasherService passwordHasherService)
        {
            _authRepository = authRepository;
            _configuration = configuration;
            _passwordHasherService = passwordHasherService;
        }

        public LoginResponseDTO? Login(LoginDTO dto)
        {
            var user = _authRepository.GetUserByUsernameAsync(dto.Username);

            if (user == null)
                return null;

            bool isPasswordValid = _passwordHasherService.VerifyPassword(
                dto.Password,
                user.PasswordHash
            );

            if (!isPasswordValid)
                return null;

            if (!user.IsActive)
                return null;

            var token = GenerateJwtToken(user);

            return new LoginResponseDTO
            {
                Token = token,
                Role = user.Role.ToString(),
                PasswordChangeRequired = user.PasswordChangeRequired
            };
        }

        public bool ChangePassword(ChangePasswordDTO dto)
        {
            var user = _authRepository.GetUserByUsernameAsync(dto.Username);

            if (user == null)
                return false;
            if (!user.IsActive)
                return false;

            bool isPasswordValid = _passwordHasherService.VerifyPassword(
                dto.OldPassword,
                user.PasswordHash
            );

            if (!isPasswordValid)
                return false;

            user.PasswordHash = _passwordHasherService.HashPassword(dto.NewPassword);
            user.PasswordChangeRequired = false;

            _authRepository.SaveChanges();

            return true;
        }

        private string GenerateJwtToken(Users user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];
            var expiryMinutes = _configuration.GetValue<int>("Jwt:ExpiryMinutes");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey!)
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

                new Claim(ClaimTypes.Name, user.UserName),

                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
