using System.ComponentModel.DataAnnotations;

namespace VivreSync.HR.DTOs
{
    public class EmployeeCreateDTO
    {
        [Required(ErrorMessage ="Full Name is required")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage ="Email is required")]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Designation { get; set; } = string.Empty;

        [Required(ErrorMessage ="Username is required")]
        [StringLength(50, MinimumLength = 3)]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage ="Password is required")]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string Role { get; set; } = string.Empty;
    }
}
