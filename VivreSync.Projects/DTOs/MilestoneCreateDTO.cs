using System.ComponentModel.DataAnnotations;

namespace VivreSync.Projects.DTOs
{
    public class MilestoneCreateDTO
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DateOnly DueDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid ProjectId")]
        [Required]
        public int ProjectId { get; set; }
    }
}
