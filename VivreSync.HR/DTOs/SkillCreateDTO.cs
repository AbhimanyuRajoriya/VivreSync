using System.ComponentModel.DataAnnotations;

namespace VivreSync.HR.DTOs
{
    public class SkillCreateDTO
    {
        [Required(ErrorMessage ="Enter Skill Name")]
        [StringLength(50, ErrorMessage = "Skill name must be 50 characters or less")]
        public string Name { get; set; } = string.Empty;
    }
}
