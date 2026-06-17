namespace VivreSync.Projects.DTOs
{
    public class ProjectHealthResponseDTO
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string Health { get; set; } = string.Empty;
        public List<string> Reasons { get; set; } = new();
        public List<ProjectMilestoneReponseDTO> Milestones { get; set; } = new();
        public List<ProjectTeamResponseDTO> Team { get; set; } = new();
    }
}
