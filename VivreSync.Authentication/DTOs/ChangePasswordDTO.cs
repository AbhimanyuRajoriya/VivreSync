using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VivreSync.Authentication.DTOs
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Old Password is required")]
        public string OldPassword { get; set; } = null!;

        [Required(ErrorMessage = "New Password is required")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 20 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
            ErrorMessage = "Password must contain uppercase, lowercase, number, and special character")]
        public string NewPassword { get; set; } = null!;
    }
}
