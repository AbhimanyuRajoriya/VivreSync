using System.ComponentModel.DataAnnotations;

namespace VivreSync.Timesheets.DTOs
{
    public class TimesheetUpdateDTO
    {
        [Required(ErrorMessage = "Enter the week start date")]
        public DateOnly WeekStartDate { get; set; }
        [Required(ErrorMessage = "Enter the proper Monday Hours")]
        public int MondayHours { get; set; }
        [Required(ErrorMessage = "Enter the proper Tuesday Hours")]
        public int TuesdayHours { get; set; }
        [Required(ErrorMessage = "Enter the proper Wednesday Hours")]
        public int WednesdayHours { get; set; }
        [Required(ErrorMessage = "Enter the proper Thrusday Hours")]
        public int ThursdayHours { get; set; }
        [Required(ErrorMessage = "Enter the proper Friday Hours")]
        public int FridayHours { get; set; }
        [Required(ErrorMessage = "Enter the proper Saturday Hours")]
        public int SaturdayHours { get; set; }
        [Required(ErrorMessage = "Enter the proper Sunday Hours")]
        public int SundayHours { get; set; }
        [Required(ErrorMessage = "Enter the Activity Tag")]
        [StringLength(20,ErrorMessage = "Activity tag must be 20 characters or less")]
        public string ActivityTag { get; set; } = string.Empty;
    }
}
