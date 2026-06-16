using System.ComponentModel.DataAnnotations;

namespace VivreSync.HR.DTOs
{
    public class SkillCreateDTO
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}
