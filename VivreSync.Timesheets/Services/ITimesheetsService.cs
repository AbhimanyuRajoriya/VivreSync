using VivreSync.Timesheets.DTOs;
namespace VivreSync.Timesheets.Services
{
    public interface ITimesheetsService
    {
        List<TimesheetResponseDTO> GetAllTimesheets();
        TimesheetResponseDTO? GetTimesheetById(int id);
        List<TimesheetResponseDTO> GetByEmployeeId(int id);
        TimesheetResponseDTO? CreateTimesheet(TimesheetCreateDTO dto);
        bool UpdateTimesheet(int id, TimesheetUpdateDTO timesheet);
        List<TimesheetMissedDTO> GetMissedTimesheets(DateOnly weekStartDate);
    }
}
