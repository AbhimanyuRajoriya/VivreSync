using System.ComponentModel.DataAnnotations;

namespace VivreSync.Allocations.DTOs;

public class AllocationUpdateDTO
{
    [Range(1, 100, ErrorMessage = "Utilization percentage must be between 1 and 100")]
    [Required(ErrorMessage = "Enter the Utilization Percentage")]
    public int UtilizationPercentage { get; set; }

    [Required(ErrorMessage = "Enter the Start Date")]
    public DateOnly? StartDate { get; set; }

    [Required(ErrorMessage = "Enter the End Date")]
    public DateOnly? EndDate { get; set; }
}
