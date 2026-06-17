using System.ComponentModel.DataAnnotations;

namespace VivreSync.Projects.DTOs
{
    public class MilestoneCreateDTO
    {
        [Required(ErrorMessage ="Enter Milestone")]
        [StringLength(100)]
        public string Progress { get; set; } = string.Empty;

        [Required]
        public DateOnly DueDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid ProjectId")]
        [Required(ErrorMessage ="Enter Project ID")]
        public int ProjectId { get; set; }
    }
}
