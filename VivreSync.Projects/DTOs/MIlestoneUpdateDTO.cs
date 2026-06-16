using System.ComponentModel.DataAnnotations;
using VivreSync.Model.Enums;

namespace VivreSync.Projects.DTOs
{
    public class MilestoneUpdateDTO
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DateOnly DueDate { get; set; }

        public MilestoneStatus Status { get; set; }
    }
}
