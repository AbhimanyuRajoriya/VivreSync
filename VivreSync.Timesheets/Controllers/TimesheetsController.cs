using Microsoft.AspNetCore.Mvc;
using VivreSync.Timesheets.DTOs;
using VivreSync.Timesheets.Services;
namespace VivreSync.Timesheets.Controllers
{
    [ApiController]
    [Route("api/timesheets")]
    public class TimesheetsController : ControllerBase
    {
        private readonly ITimesheetsService _service;
        public TimesheetsController(ITimesheetsService service)
        {
            _service = service;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAllTimesheets()
        {
            var timesheets = _service.GetAllTimesheets();
            if (timesheets == null)
                return BadRequest("No Data");
            return Ok(timesheets);
        }

        [HttpGet("GetTimeSheetById")]
        public IActionResult GetTimesheetEmployee(int id)
        {
            var timesheet = _service.GetTimesheetById(id);
            if(timesheet == null)
                return NotFound();
            return Ok(timesheet);
        }

        [HttpGet("GetEmployeeById")]
        public IActionResult GetTimesheetsofEmployee(int id)
        {
            var timesheet = _service.GetByEmployeeId(id);
            if (timesheet == null)
                return BadRequest();
            return Ok(timesheet);
        }

        [HttpPost("SubmitTimesheet")]
        public IActionResult CreateTimesheets(TimesheetCreateDTO dto)
        {
            _service.CreateTimesheet(dto);
            return Ok("Timesheet Submitted");
        }

        [HttpPost("EditTimesheet")]
        public IActionResult UpdateTimesheets(int id,TimesheetUpdateDTO dto)
        {
            var result = _service.UpdateTimesheet(id,dto);
            if (!result)
                return BadRequest();
            return Ok("Timesheet Updated");
        }
        [HttpGet("missed")]
        public IActionResult GetMissedTimesheets([FromQuery] DateOnly weekStartDate)
        {
            if (weekStartDate.DayOfWeek != DayOfWeek.Monday)
                return BadRequest("Week start date must be Monday");

            var result = _service.GetMissedTimesheets(weekStartDate);

            return Ok(result);
        }
    }
}
