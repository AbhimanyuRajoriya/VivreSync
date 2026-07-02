using VivreSync.Timesheets.DTOs;
namespace VivreSync.Timesheets.Services
{
    public interface ITimesheetsService
    {
        List<TimesheetResponseDTO> GetAllTimesheets(int userId, string role);
        TimesheetResponseDTO? GetTimesheetById(int id, int userId, string role);
        List<TimesheetResponseDTO> GetByEmployeeId(int employeeId, int userId, string role);
        TimesheetResponseDTO? CreateTimesheet(TimesheetCreateDTO dto, int userId);
        bool UpdateTimesheet(int id, TimesheetUpdateDTO timesheet, int userId);
        List<TimesheetMissedDTO> GetMissedTimesheets(DateOnly weekStartDate, int userId);
    }
}
