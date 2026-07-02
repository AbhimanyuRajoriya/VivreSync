using System.ComponentModel.DataAnnotations;

namespace VivreSync.HR.DTOs
{
    public class EmployeeCreateDTO
    {
        [Required(ErrorMessage ="Full Name is required")]
        [StringLength(50, ErrorMessage = "Name must be 50 characters or less")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage ="Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage ="Designation is required")]
        [StringLength(50, ErrorMessage = "Designation must be 50 characters or less")]
        public string Designation { get; set; } = string.Empty;

        [Required(ErrorMessage ="Username is required")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Username must be between 6 to 20 characters")]
        [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z0-9._]+$", ErrorMessage = "Username must contain at least one letter and can only contain letters, numbers, dot, and underscore")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage ="Password is required")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 6 to 20 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
            ErrorMessage = "Password must contain uppercase, lowercase, number, and special character")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        [StringLength(50,ErrorMessage = "Role must be 50 characters or less")]
        public string Role { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Invalid Manager Id")]
        public int? ManagerId { get; set; }
    }
}
