namespace VivreSync.Allocations.DTOs
{
    public class EmployeeAllocationDTO
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int TotalAllocation { get; set; }
        public int AvailableCapacity { get; set; }
        public List<string> Projects { get; set; } = new();
    }
}
