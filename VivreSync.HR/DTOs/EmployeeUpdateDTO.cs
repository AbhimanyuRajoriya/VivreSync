using System.ComponentModel.DataAnnotations;

namespace VivreSync.HR.DTOs
{
    public class EmployeeUpdateDTO
    {
        [Range(1, 1000, ErrorMessage = "Invalid Id")]
        [Required(ErrorMessage ="Employee Id is required")]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;
    }
}
