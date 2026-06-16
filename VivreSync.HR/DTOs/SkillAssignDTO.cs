using System.ComponentModel.DataAnnotations;

namespace VivreSync.HR.DTOs
{
    public class SkillAssignDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Invalid EmployeeId")]
        [Required]
        public int EmployeeId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid SkillId")]
        [Required]
        public int SkillId { get; set; }

        [Required]
        [StringLength(30)]
        public string Level { get; set; } = string.Empty;
    }
}
