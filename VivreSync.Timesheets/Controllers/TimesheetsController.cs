using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
        public TimesheetsController(ITimesheetsService service)
        {
            _service = service;
        }

        [HttpGet("GetAllTimesheets")]
        [Authorize(Roles = "Manager,Employee")]
        public IActionResult GetAllTimesheets()
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentRole();
            var timesheets = _service.GetAllTimesheets(userId, role);
            return Ok(timesheets);
        }

        [HttpGet("GetTimeSheetById/{timesheetId}")]
        [Authorize(Roles = "Manager,Employee")]
        public IActionResult GetTimesheetById(int timesheetId)
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentRole();
            var timesheet = _service.GetTimesheetById(timesheetId, userId, role);
            if (timesheet == null)
                return NotFound("Timesheet not found");

            return Ok(timesheet);
        }

        [HttpGet("GetTimesheetOfEmployeeById/{employeeId}")]
        [Authorize(Roles = "Manager,Employee")]
        public IActionResult GetTimesheetsOfEmployeeById(int employeeId)
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentRole();
            var timesheets = _service.GetByEmployeeId(employeeId, userId, role);
            return Ok(timesheets);
        }

        [HttpPost("SubmitTimesheet")]
        [Authorize(Roles = "Employee")]
        public IActionResult SubmitTimesheet(TimesheetCreateDTO dto)
        {
            var userId = GetCurrentUserId();
            var timesheet = _service.CreateTimesheet(dto, userId);
            if (timesheet == null)
                throw new BadRequestException("Timesheet not submitted");

            return Ok("Timesheet submitted");
        }

        [HttpPost("EditTimesheet/{timesheetId}")]
        [Authorize(Roles = "Employee")]
        public IActionResult UpdateTimesheet(int timesheetId, TimesheetUpdateDTO dto)
        {
            var userId = GetCurrentUserId();
            var result = _service.UpdateTimesheet(timesheetId, dto, userId);
            if (!result)
                throw new BadRequestException("Timesheets is not updated");

            return Ok("Timesheet updated");
        }

        [HttpGet("GetMissedTimesheets")]
        [Authorize(Roles = "Manager")]
        public IActionResult GetMissedTimesheets([FromQuery] DateOnly? weekStartDate)
        {
            if (weekStartDate == null)
                throw new BadRequestException("Enter the date");

            if (weekStartDate.Value.DayOfWeek != DayOfWeek.Monday)
                throw new BadRequestException("Week start date must be Monday");

            var userId = GetCurrentUserId();
            var result = _service.GetMissedTimesheets(weekStartDate.Value, userId);
            return Ok(result);
        }
        private int GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userId, out var currentUserId))
                throw new UnauthorizedException("Invalid token");

            return currentUserId;
        }
        private string GetCurrentRole()
        {
            if (User.IsInRole("Manager"))
                return "Manager";
            if (User.IsInRole("Employee"))
                return "Employee";
            throw new UnauthorizedException("Invalid role");
        }
    }
}
