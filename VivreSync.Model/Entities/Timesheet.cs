using VivreSync.Model.Enums;

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

        public int MondayHours { get; set; }
        public int TuesdayHours { get; set; }
        public int WednesdayHours { get; set; }
        public int ThursdayHours { get; set; }
        public int FridayHours { get; set; }
        public int SaturdayHours { get; set; }
        public int SundayHours { get; set; }

        public ActivityTags ActivityTag { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
