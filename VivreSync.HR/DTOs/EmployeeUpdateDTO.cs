using System.ComponentModel.DataAnnotations;
using VivreSync.Model.Entities;

namespace VivreSync.HR.DTOs
{
    public class EmployeeUpdateDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Id")]
        [Required(ErrorMessage ="Employee Id is required")]
        public int EmployeeId { get; set; }
        [Required(ErrorMessage = "FullName is Required")]
        [StringLength(50, ErrorMessage = "Full name must be 50 characters or less")]
        public string FullName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Designation is required")]
        [StringLength(50, ErrorMessage = "Designation must be 50 characters or less")]
        public string Designation { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Invalid Manager Id")]
        public int? ManagerId { get; set; }
    }
}
