using System.ComponentModel.DataAnnotations;

namespace VivreSync.Timesheets.DTOs
{
    public class TimesheetUpdateDTO
    {
        [Required(ErrorMessage = "Enter working hours of monday")]
        [Range(0, 24, ErrorMessage = "Monday hours must be between 0 and 24")]
        public int? MondayHours { get; set; }

        [Required(ErrorMessage = "Enter working hours of tuesday")]
        [Range(0, 24, ErrorMessage = "Tuesday hours must be between 0 and 24")]
        public int? TuesdayHours { get; set; }

        [Required(ErrorMessage = "Enter working hours of wednesday")]
        [Range(0, 24, ErrorMessage = "Wednesday hours must be between 0 and 24")]
        public int? WednesdayHours { get; set; }

        [Required(ErrorMessage = "Enter working hours of thursday")]
        [Range(0, 24, ErrorMessage = "Thursday hours must be between 0 and 24")]
        public int? ThursdayHours { get; set; }

        [Required(ErrorMessage = "Enter working hours of friday")]
        [Range(0, 24, ErrorMessage = "Friday hours must be between 0 and 24")]
        public int? FridayHours { get; set; }

        [Required(ErrorMessage = "Enter working hours of saturday")]
        [Range(0, 24, ErrorMessage = "Saturday hours must be between 0 and 24")]
        public int? SaturdayHours { get; set; }

        [Required(ErrorMessage = "Enter working hours of sunday")]
        [Range(0, 24, ErrorMessage = "Sunday hours must be between 0 and 24")]
        public int? SundayHours { get; set; }
        [Required(ErrorMessage = "Enter the Activity Tag")]
        [StringLength(20,ErrorMessage = "Activity tag must be 20 characters or less")]
        public string ActivityTag { get; set; } = string.Empty;
    }
}
