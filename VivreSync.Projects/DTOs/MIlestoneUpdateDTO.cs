using System.ComponentModel.DataAnnotations;

namespace VivreSync.Projects.DTOs
{
    public class MilestoneUpdateDTO
    {
        [Required(ErrorMessage = "Enter Progress of the Project")]
        [StringLength(100,ErrorMessage = "Enter Progress of the Project under 100 characters")]
        public string Progress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter Due Date of Milestone")]
        public DateOnly DueDate { get; set; }

        [Required(ErrorMessage = "Enter Status")]
        [StringLength(100, ErrorMessage = "Enter Status of the Project under 100 characters")]
        public string Status { get; set; } = string.Empty;
    }
}
