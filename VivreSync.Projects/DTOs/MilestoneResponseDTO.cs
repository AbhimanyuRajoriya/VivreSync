using VivreSync.Model.Enums;
using System.ComponentModel.DataAnnotations;
namespace VivreSync.Projects.DTOs
{
    public class MilestoneResponseDTO
    {
        public int Id { get; set; }
        public string Progress { get; set; } = string.Empty;
        public DateOnly DueDate { get; set; }
        public string Status { get; set; } = MilestoneStatus.OnTrack.ToString();
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
    }
}