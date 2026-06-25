using VivreSync.Model.Entities;

namespace VivreSync.Timesheets.Repositories
{
    public interface ITimesheetsRepository
    {
        List<Timesheet> GetAll();
        Timesheet? GetById(int id);
        List<Timesheet> GetByEmployeeId(int employeeId);
        bool ExistsForWeek(int employeeId, int projectId, DateOnly weekStartDate);
        bool IsEmployeeAllocatedToProject(int employeeId, int projectId, DateOnly weekStartDate, DateOnly weekEndDate);
        List<Allocation> GetAllocationsForWeek(DateOnly weekStartDate, DateOnly weekEndDate);
        bool ExistsForWeekExceptCurrent(int timesheetId, int employeeId, int projectId, DateOnly weekStartDate);
        void Add(Timesheet timesheet);
        void Update(Timesheet timesheet);
    }
}
