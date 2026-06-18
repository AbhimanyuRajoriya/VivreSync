using System.ComponentModel.DataAnnotations;
using VivreSync.Model.Enums;

namespace VivreSync.Projects.DTOs
{
    public class ProjectUpdateDTO
    {
        [Range(1, 1000, ErrorMessage = "Invalid Id")]
        [Required(ErrorMessage ="Enter Project ID")]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Client { get; set; } = string.Empty;
        public ProjectStatus Status { get; set; }

        [Range(1, 1000, ErrorMessage = "Invalid ManagerId")]
        [Required]
        public int ManagerId { get; set; }
    }
}
