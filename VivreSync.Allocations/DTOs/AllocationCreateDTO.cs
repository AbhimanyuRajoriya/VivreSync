using System.ComponentModel.DataAnnotations;

namespace VivreSync.Allocations.DTOs;

public class AllocationCreateDTO
{
    [Required(ErrorMessage = "Employee ID is required")]
    [Range(1, int.MaxValue, ErrorMessage ="Enter the Valid Range")]
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = "Project ID is reuired")]
    [Range(1, int.MaxValue, ErrorMessage = "Enter the Valid Range")]
    public int ProjectId { get; set; }

    [Range(1, 100, ErrorMessage = "Utilization percentage must be between 1 and 100")]
    [Required(ErrorMessage ="Enter the utilization percentage")]
    public int UtilizationPercentage { get; set; }

    [Required(ErrorMessage ="Enter the Start Date")]
    public DateOnly StartDate { get; set; }

    [Required(ErrorMessage = "Enter the End Date")]
    public DateOnly EndDate { get; set; }
}
