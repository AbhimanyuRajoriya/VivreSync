using System.ComponentModel.DataAnnotations;

namespace VivreSync.HR.DTOs
{
    public class EmployeeUpdateDTO
    {
        [Range(1, 1000, ErrorMessage = "Invalid Id")]
        [Required(ErrorMessage ="Employee Id is required")]
        public int EmployeeId { get; set; }
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        [Required]
        [StringLength(100)]
        public string Designation { get; set; } = string.Empty;
    }
}
