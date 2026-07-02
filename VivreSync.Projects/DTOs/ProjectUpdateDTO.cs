using System.ComponentModel.DataAnnotations;

namespace VivreSync.Projects.DTOs
{
    public class ProjectUpdateDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Project Id")]
        [Required(ErrorMessage ="Enter Project ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter name of project")]
        [StringLength(100, ErrorMessage = "Project Name must be 100 characters or less")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter client of the project")]
        [StringLength(100, ErrorMessage = "Client Name must be 100 characters or less")]
        public string Client { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter Status")]
        [StringLength(100, ErrorMessage = "Status must be 100 characters or less")]
        public string Status { get; set; } = string.Empty;
    }
}
