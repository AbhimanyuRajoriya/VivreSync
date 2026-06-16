using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VivreSync.Authentication.DTOs
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class ChangePasswordDTO
    {
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 20 characters.")]
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = null!;
        [StringLength(256, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 256 characters.")]
        [Required(ErrorMessage = "Password is required")]
        public string OldPassword { get; set; } = null!;
        [StringLength(256, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 256 characters.")]
        [Required(ErrorMessage = "Password is required")]
        public string NewPassword { get; set; } = null!;
    }
}
