namespace VivreSync.Projects.DTOs
{
    public class ProjectCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Client { get; set; } = string.Empty;
        public int ManagerId { get; set; }
    }
}