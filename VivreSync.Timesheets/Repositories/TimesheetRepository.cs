using Microsoft.EntityFrameworkCore;
using VivreSync.Model.Entities;
using VivreSync.Structure.Data;
namespace VivreSync.Timesheets.Repositories
{
    public class TimesheetRepository : ITimesheetsRepository
    {
        private readonly AppDbContext _Context;
        public TimesheetRepository(AppDbContext context)
        {
            _Context = context;
        }

        public List<Timesheet> GetAll()
        {
            return _Context.Timesheets
                    .Include(t => t.Employee)
                    .Include(t => t.Project)
                    .ToList();
        }
        public Timesheet? GetById(int id)
        {
            return _Context.Timesheets
                    .Include(t => t.Employee)
                    .Include(t => t.Project)
                    .FirstOrDefault(t => t.Id == id);
        }

        public List<Timesheet> GetByEmployeeId(int id)
        {
            return _Context.Timesheets
                    .Include(t => t.Employee)
                    .Include(t => t.Project)
                    .Where(t => t.EmployeeId == id)
                    .ToList();
        }

        public void Add(Timesheet timesheet)
        {
            _Context.Timesheets.Add(timesheet);
            _Context.SaveChanges();
        }
        public void Update(Timesheet timesheet)
        {
            _Context.Timesheets.Update(timesheet);
            _Context.SaveChanges();
        }

        public bool ExistsForWeek(int employeeId, int projectId, DateOnly weekStartDate)
        {
            return _Context.Timesheets.Any(t =>
                t.EmployeeId == employeeId && t.ProjectId == projectId &&
                t.WeekStartDate == weekStartDate);
        }

        public bool IsEmployeeAllocatedToProject(int employeeId, int projectId, DateOnly weekStartDate, DateOnly weekEndDate)
        {
            return _Context.Allocations.Any(a =>
                a.EmployeeId == employeeId && a.ProjectId == projectId &&
                a.StartDate <= weekEndDate && a.EndDate >= weekStartDate);
        }

        public List<Allocation> GetAllocationsForWeek(DateOnly weekStartDate, DateOnly weekEndDate)
        {
            return _Context.Allocations
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .Where(a => a.StartDate <= weekEndDate && a.EndDate >= weekStartDate).ToList();
        }

        public bool ExistsForWeekExceptCurrent(int timesheetId, int employeeId, int projectId, DateOnly weekStartDate)
        {
            return _Context.Timesheets.Any(t =>
                t.Id != timesheetId &&
                t.EmployeeId == employeeId &&
                t.ProjectId == projectId &&
                t.WeekStartDate == weekStartDate);
        }
    }
}