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
        public DateOnly WeekStartDate { get; set; }

        [Required(ErrorMessage ="Enter working hours of monday")]
        public int MondayHours { get; set; }

        [Required(ErrorMessage = "Enter working hours of tuesday")]
        public int TuesdayHours { get; set; }

        [Required(ErrorMessage = "Enter working hours of wednesday")]
        public int WednesdayHours{ get; set; }

        [Required(ErrorMessage = "Enter working hours of thrusday")]
        public int ThursdayHours { get; set; }

        [Required(ErrorMessage = "Enter working hours of friday")]
        public int FridayHours { get;set; }

        [Required(ErrorMessage = "Enter working hours of saturday")]
        public int SaturdayHours { get;set; }

        [Required(ErrorMessage = "Enter working hours of sunday")]
        public int SundayHours { get; set; }

        [Required(ErrorMessage = "Enter the proper Activity tag")]
        [StringLength(20,ErrorMessage = "Enter valid length of activity")]
        public string ActivityTag { get; set; } = string.Empty;
    }
}
