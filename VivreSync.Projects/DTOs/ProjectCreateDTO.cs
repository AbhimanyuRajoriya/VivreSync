using System.ComponentModel.DataAnnotations;

namespace VivreSync.Projects.DTOs
{
    public class ProjectCreateDTO
    {
        [Required(ErrorMessage ="Enter Project Title")]
        [StringLength(100, ErrorMessage = "Project Name must be 100 characters or less")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage ="Enter client Name")]
        [StringLength(100, ErrorMessage = "Client Name must be 100 characters or less")]
        public string Client { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Invalid Manager Id")]
        [Required(ErrorMessage ="Manager ID is required")]
        public int ManagerId { get; set; }
    }
}
