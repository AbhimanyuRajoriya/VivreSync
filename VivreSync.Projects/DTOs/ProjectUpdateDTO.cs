using System.ComponentModel.DataAnnotations;

namespace VivreSync.Projects.DTOs
{
    public class ProjectUpdateDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Id")]
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Client { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Invalid ManagerId")]
        [Required]
        public int ManagerId { get; set; }
    }
}
