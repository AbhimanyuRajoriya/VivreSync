namespace VivreSync.Timesheets.DTOs
{
    public class TimesheetMissedDTO
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public DateOnly WeekStartDate { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
