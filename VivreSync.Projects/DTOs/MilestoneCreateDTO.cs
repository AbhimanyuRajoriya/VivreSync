using System.ComponentModel.DataAnnotations;

namespace VivreSync.Projects.DTOs
{
    public class MilestoneCreateDTO
    {
        [Required(ErrorMessage ="Enter Milestone Progress")]
        [StringLength(100, ErrorMessage = "Progress name must be 100 characters or less")]
        public string Progress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter due date of milestone")]
        public DateOnly? DueDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Invalid ProjectId")]
        [Required(ErrorMessage ="Enter Milestone's Project ID")]
        public int ProjectId { get; set; }
    }
}
