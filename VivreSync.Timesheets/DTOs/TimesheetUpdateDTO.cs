namespace VivreSync.Timesheets.DTOs
{
    public class TimesheetUpdateDTO
    {
        public DateOnly WeekStartDate { get; set; }
        public int MondayHours { get; set; }
        public int TuesdayHours { get; set; }
        public int WednesdayHours { get; set; }
        public int ThursdayHours { get; set; }
        public int FridayHours { get; set; }
        public int SaturdayHours { get; set; }
        public int SundayHours { get; set; }
        public string ActivityTag { get; set; } = string.Empty;
    }
}
