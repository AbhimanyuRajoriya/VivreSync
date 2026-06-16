using VivreSync.Model.Enums;

namespace VivreSync.Projects.DTOs
{
    public class MilestoneUpdateDTO
    {
        public string Title { get; set; } = string.Empty;
        public DateOnly DueDate { get; set; }
        public MilestoneStatus Status { get; set; }
    }
}