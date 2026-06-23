using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VivreSync.HR.Services;
using VivreSync.Shared.Exceptions;
using VivreSync.Timesheets.DTOs;
using VivreSync.Timesheets.Services;
namespace VivreSync.Timesheets.Controllers
{
    [ApiController]
    [Route("api/timesheets")]
    [Authorize]
    public class TimesheetsController : ControllerBase
    {
        private readonly ITimesheetsService _service;
        private readonly IEmployeeService _employeeService;
        public TimesheetsController(ITimesheetsService service, IEmployeeService employeeService)
        {
            _service = service;
            _employeeService = employeeService;
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult GetAllTimesheets()
        {
            var timesheets = _service.GetAllTimesheets();
            if (timesheets == null)
                return BadRequest("No Data");
            return Ok(timesheets);
        }

        [HttpGet("GetTimeSheetById/{id}")]
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult GetTimesheetById(int id)
        {
            var timesheet = _service.GetTimesheetById(id);
            if(timesheet == null)
                return NotFound();
            return Ok(timesheet);
        }

        [HttpGet("GetTimesheetOfEmployeeById/{id}")]
        [Authorize(Roles = "Admin, Manager, Employee")]
        public IActionResult GetTimesheetsofEmployee(int id)
        {
            if (User.IsInRole("Employee"))
            {
                var currentUserId = GetCurrentUserId();
                var isOwnTimesheet = _employeeService.IsEmployeeLinkedToUser(id, currentUserId);
                if (!isOwnTimesheet)
                    throw new UnauthorizedException("Cannot see Timesheet of other employee");
            }
            var timesheet = _service.GetByEmployeeId(id);
            if (timesheet == null)
                return BadRequest();
            return Ok(timesheet);
        }

        [HttpPost("SubmitTimesheet")]
        [Authorize(Roles = "Admin,Employee")]
        public IActionResult CreateTimesheets(TimesheetCreateDTO dto)
        {
            if (User.IsInRole("Employee"))
            {
                var currentUserId = GetCurrentUserId();
                var isOwnTimesheet = _employeeService.IsEmployeeLinkedToUser(dto.EmployeeId, currentUserId);
                if (!isOwnTimesheet)
                    throw new UnauthorizedException("Cannot Create Timesheet of other employee");
            }
            var timesheet =  _service.CreateTimesheet(dto);
            if (timesheet == null)
                return BadRequest("Cannot accept this request");
            return Ok("Timesheet Submitted");
        }

        [HttpPost("EditTimesheet/{id}")]
        [Authorize(Roles = "Employee")]
        public IActionResult UpdateTimesheets(int id,TimesheetUpdateDTO dto)
        {
            var currentUserId = GetCurrentUserId();
            var isOwnTimesheet = _service.IsTimesheetLinkedToUser(id, currentUserId);
            if (!isOwnTimesheet)
                throw new UnauthorizedException("Cannot Update Timesheet of other employee");

            var result = _service.UpdateTimesheet(id, dto);
            if (!result)
                return BadRequest();

            return Ok("Timesheet Updated");
        }
        [HttpGet("missed")]
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult GetMissedTimesheets([FromQuery] DateOnly weekStartDate)
        {
            if (weekStartDate.DayOfWeek != DayOfWeek.Monday)
                return BadRequest("Week start date must be Monday");

            var result = _service.GetMissedTimesheets(weekStartDate);

            return Ok(result);
        }
        private int GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out var currentUserId))
                throw new UnauthorizedAccessException("Invalid token");

            return currentUserId;
        }
    }
}
