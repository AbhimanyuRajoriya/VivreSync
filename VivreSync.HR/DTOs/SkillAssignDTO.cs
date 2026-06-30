using System.ComponentModel.DataAnnotations;

namespace VivreSync.HR.DTOs
{
    public class SkillAssignDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Invalid EmployeeId")]
        [Required(ErrorMessage ="Employee Id is required")]
        public int EmployeeId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid SkillId")]
        [Required(ErrorMessage ="Skill Id is required")]
        public int SkillId { get; set; }

        [Required(ErrorMessage ="Enter Proper Level of proficieny")]
        [StringLength(10, ErrorMessage = "Proficieny Level must be 50 characters or less")]
        public string Level { get; set; } = string.Empty;
    }
}
