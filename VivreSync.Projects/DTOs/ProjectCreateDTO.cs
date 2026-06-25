using System.ComponentModel.DataAnnotations;

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
        public string Status { get; set; } = string.Empty;

        [Range(1, 1000, ErrorMessage = "Invalid ManagerId")]
        [Required(ErrorMessage ="Manager ID is required")]
        public int ManagerId { get; set; }
    }
}
