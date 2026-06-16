namespace VivreSync.Projects.DTOs
{
    public class MilestoneCreateDTO
    {
        public string Title { get; set; } = string.Empty;
        public DateOnly DueDate { get; set; }
        public int ProjectId { get; set; }
    }
}