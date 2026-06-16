namespace VivreSync.Model.Entities
{
    public class Timesheet
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public DateOnly WeekStartDate { get; set; }
        public int HoursWorked { get; set; }
        public string ActivityTag { get; set; } = string.Empty;
    }
}
