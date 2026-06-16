namespace VivreSync.Authentication.DTOs
{
    public class LoginResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool PasswordChangeRequired { get; set; }
    }
}
