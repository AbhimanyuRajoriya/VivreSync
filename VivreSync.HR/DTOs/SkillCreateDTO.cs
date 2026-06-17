using System.ComponentModel.DataAnnotations;

namespace VivreSync.HR.DTOs
{
    public class SkillCreateDTO
    {
        [Required(ErrorMessage ="Enter Skill Name")]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}
