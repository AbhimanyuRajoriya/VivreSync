using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VivreSync.Authentication.DTOs;
using VivreSync.Authentication.Repositories;
using VivreSync.Authentication.Security;
using VivreSync.Model.Entities;
using VivreSync.Shared.Exceptions;

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
            var username = dto.Username.Trim().ToLower();
            var user = _authRepository.GetUserByUsernameAsync(username);
            if (user == null)
                throw new NotFoundException("User does not exist");

            bool isPasswordValid = _passwordHasherService.VerifyPassword(
                dto.Password,
                user.PasswordHash
            );

            if (!isPasswordValid)
                throw new BadRequestException("Invalid Password");

            if (!user.IsActive)
                throw new BadRequestException("User is not active");

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
            var username = dto.Username.Trim().ToLower();
            var user = _authRepository.GetUserByUsernameAsync(username);

            if (user == null)
                throw new NotFoundException("User does not exist");
            if (!user.IsActive)
                throw new BadRequestException("User is inactive");

            bool isPasswordValid = _passwordHasherService.VerifyPassword(
                dto.OldPassword,
                user.PasswordHash
            );

            if (!isPasswordValid)
                throw new BadRequestException("Invalid Password");

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
            var expiryMinutes = _configuration.GetValue<int>("Jwt:ExpireMinutes");

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
