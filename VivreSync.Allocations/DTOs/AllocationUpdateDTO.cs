namespace VivreSync.Allocations.DTOs;

public class AllocationUpdateDTO
{
    public int EmployeeId { get; set; }
    public int ProjectId {get; set;}
    public int UtilizationPercentage { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}
