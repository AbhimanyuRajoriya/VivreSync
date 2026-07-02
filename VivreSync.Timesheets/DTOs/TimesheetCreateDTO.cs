using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace VivreSync.Timesheets.DTOs
{
    public class TimesheetCreateDTO
    {
        [Required(ErrorMessage ="Enter the Employee ID")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid employee Id")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage ="Enter Project ID")]
        [Range(1, int.MaxValue,ErrorMessage ="Invalid project Id")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage ="Enter Start of week")]
        public DateOnly? WeekStartDate { get; set; }

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

        [Required(ErrorMessage = "Enter the proper Activity tag")]
        [StringLength(20,ErrorMessage = "Enter valid length of activity")]
        public string ActivityTag { get; set; } = string.Empty;
    }
}
