namespace VivreSync.Projects.DTOs
{
    public class ProjectResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Client { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int ManagerId { get; set; }
        public string ManagerName { get; set; } = string.Empty;
        //public List<ProjMilestoneResponseDTO> Milestones { get; set; } = new();
    }
}