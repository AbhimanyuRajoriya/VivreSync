using System.ComponentModel.DataAnnotations;

namespace VivreSync.Allocations.DTOs;

public class AllocationUpdateDTO
{
    [Range(1, 1000, ErrorMessage = "Invalid EmployeeId")]
    [Required]
    public int EmployeeId { get; set; }

    [Range(1, 1000, ErrorMessage = "Invalid ProjectId")]
    [Required]
    public int ProjectId { get; set; }

    [Range(1, 100, ErrorMessage = "Utilization percentage must be between 1 and 100")]
    [Required]
    public int UtilizationPercentage { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }
}
