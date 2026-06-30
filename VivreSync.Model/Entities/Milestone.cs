using VivreSync.Model.Enums;

namespace VivreSync.Model.Entities
{
    public class Milestone
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateOnly DueDate { get; set; }
        public MilestoneStatus Status { get; set; } = MilestoneStatus.OnTrack;
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;
    }
}
