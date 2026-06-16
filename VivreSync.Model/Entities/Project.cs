using VivreSync.Model.Enums;
namespace VivreSync.Model.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Client { get; set; } = null!;
        public ProjectStatus Status { get; set; } = ProjectStatus.Active;
        public int ManagerId { get; set; }
        public Employee Manager { get; set; } = null!;
        public List<Milestone> Milestones { get; set; } = new();
    }
}
