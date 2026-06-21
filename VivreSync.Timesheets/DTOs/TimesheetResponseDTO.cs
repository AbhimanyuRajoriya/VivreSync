namespace VivreSync.Timesheets.DTOs
{
    public class TimesheetResponseDTO
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int TotalHours { get; set; }
        public string ActivityTag { get; set; } = string.Empty;
        public string SubmittedAt { get; set; } = string.Empty;
    }
}
