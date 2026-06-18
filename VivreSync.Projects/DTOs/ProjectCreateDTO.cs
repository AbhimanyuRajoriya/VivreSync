using System.ComponentModel.DataAnnotations;
using VivreSync.Model.Enums;

namespace VivreSync.Projects.DTOs
{
    public class ProjectCreateDTO
    {
        [Required(ErrorMessage ="Enter Project Title")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage ="Enter client Name")]
        [StringLength(100)]
        public string Client { get; set; } = string.Empty;
        public ProjectStatus Status { get; set; } = ProjectStatus.Active;

        [Range(1, int.MaxValue, ErrorMessage = "Invalid ManagerId")]
        [Required(ErrorMessage ="Manager ID is required")]
        public int ManagerId { get; set; }
    }
}
