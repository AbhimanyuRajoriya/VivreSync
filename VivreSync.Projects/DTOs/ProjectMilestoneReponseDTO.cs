namespace VivreSync.Projects.DTOs
{
    public class ProjectMilestoneReponseDTO
    {
        public int MilestoneId { get; set; }
        public string Progress { get; set; } = string.Empty;
        public DateOnly DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
