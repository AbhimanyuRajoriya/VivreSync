using System.ComponentModel.DataAnnotations;

namespace VivreSync.Allocations.DTOs;

public class AllocationCreateDTO
{
    [Required(ErrorMessage = "Employee ID is required")]
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = "Project ID is reuired")]
    public int ProjectId { get; set; }

    [Range(1, 100, ErrorMessage = "Utilization percentage must be between 1 and 100")]
    [Required]
    public int UtilizationPercentage { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }
}
