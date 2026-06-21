using VivreSync.Model.Entities;
using VivreSync.Model.Enums;
using VivreSync.HR.Repositories;
using VivreSync.Projects.Repositories;
using VivreSync.Timesheets.DTOs;
using VivreSync.Timesheets.Repositories;

namespace VivreSync.Timesheets.Services
{
    public class TimesheetsService: ITimesheetsService
    {
        private readonly ITimesheetsRepository _repository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IProjectRepository _projectRepository;
        public TimesheetsService(ITimesheetsRepository repository, IEmployeeRepository employeeRepository, IProjectRepository projectRepository)
        {
            _repository = repository;
            _employeeRepository = employeeRepository;
            _projectRepository = projectRepository;
        }

        public List<TimesheetResponseDTO> GetAllTimesheets()
        {
            var timesheets = _repository.GetAll();
            return timesheets.Select(TimesheetMap).ToList();
        }
        public TimesheetResponseDTO? GetTimesheetById(int id)
        {
            var timesheet = _repository.GetById(id);
            if (timesheet == null)
                return null;
            return TimesheetMap(timesheet);
        }
        public TimesheetResponseDTO? CreateTimesheet(TimesheetCreateDTO dto)
        {
            if (!IsMonday(dto.WeekStartDate))
                return null;

            var employee = _employeeRepository.GetById(dto.EmployeeId);
            if (employee == null || !employee.IsActive)
                return null;

            var project = _projectRepository.GetById(dto.ProjectId);
            if (project == null)
                return null;

            var weekEndDate = dto.WeekStartDate.AddDays(6);

            var isAllocated = _repository.IsEmployeeAllocatedToProject(dto.EmployeeId, dto.ProjectId, dto.WeekStartDate, weekEndDate);
            if (!isAllocated)
                return null;

            var alreadySubmitted = _repository.ExistsForWeek(dto.EmployeeId, dto.ProjectId,dto.WeekStartDate);

            if (alreadySubmitted)
                return null;

            if (!AreHoursValid(dto.MondayHours, dto.TuesdayHours, dto.WednesdayHours,
                dto.ThursdayHours, dto.FridayHours, dto.SaturdayHours, dto.SundayHours))
                return null;

            if (string.IsNullOrWhiteSpace(dto.ActivityTag))
                return null;

            var isvalidactivity = Enum.TryParse<ActivityTags>(dto.ActivityTag, true, out var activityTags);
            var timesheet = new Timesheet
            {
                EmployeeId = dto.EmployeeId,
                ProjectId = dto.ProjectId,
                WeekStartDate = dto.WeekStartDate,
                MondayHours = dto.MondayHours,
                TuesdayHours = dto.TuesdayHours,
                WednesdayHours = dto.WednesdayHours,
                ThursdayHours = dto.ThursdayHours,
                FridayHours = dto.FridayHours,
                SaturdayHours = dto.SaturdayHours,
                SundayHours = dto.SundayHours,
                ActivityTag = activityTags,
                SubmittedAt = DateTime.UtcNow
            };
            _repository.Add(timesheet);

            return TimesheetMap(_repository.GetById(timesheet.Id));
        }
        public bool UpdateTimesheet(int id,TimesheetUpdateDTO dto)
        {
            var timesheet = _repository.GetById(id);

            if (timesheet == null)
                return false;

            if (!IsMonday(dto.WeekStartDate))
                return false;

            if (!AreHoursValid(dto.MondayHours,dto.TuesdayHours,dto.WednesdayHours,
                dto.ThursdayHours,dto.FridayHours,dto.SaturdayHours,dto.SundayHours))
                return false;

            if (string.IsNullOrWhiteSpace(dto.ActivityTag))
                return false;

            var timesheets = _repository.GetById(id);
            if (timesheets == null) return false;

            Enum.TryParse<ActivityTags>(dto.ActivityTag, true, out var activityTags);

            timesheets.WeekStartDate = dto.WeekStartDate;
            timesheets.MondayHours = dto.MondayHours;
            timesheets.TuesdayHours = dto.TuesdayHours;
            timesheets.WednesdayHours = dto.WednesdayHours;
            timesheets.ThursdayHours = dto.ThursdayHours;
            timesheets.FridayHours = dto.FridayHours;
            timesheets.SaturdayHours = dto.SaturdayHours;
            timesheets.SundayHours = dto.SundayHours;
            timesheets.ActivityTag = activityTags;

            _repository.Update(timesheets);
            return true;
        }

        public List<TimesheetResponseDTO> GetByEmployeeId(int id)
        {
            var timesheet = _repository.GetByEmployeeId(id);
            return timesheet.Select(TimesheetMap).ToList();
        }

        public List<TimesheetMissedDTO> GetMissedTimesheets(DateOnly weekStartDate)
        {
            if (!IsMonday(weekStartDate))
                return new List<TimesheetMissedDTO>();

            var weekEndDate = weekStartDate.AddDays(6);
            var allocations = _repository.GetAllocationsForWeek(weekStartDate, weekEndDate);

            var missedTimesheets = new List<TimesheetMissedDTO>();
            foreach (var allocation in allocations)
            {
                var submitted = _repository.ExistsForWeek(allocation.EmployeeId, allocation.ProjectId, weekStartDate);

                if (!submitted)
                {
                    missedTimesheets.Add(new TimesheetMissedDTO
                    {
                        EmployeeId = allocation.EmployeeId,
                        EmployeeName = allocation.Employee.FullName,
                        ProjectId = allocation.ProjectId,
                        ProjectName = allocation.Project.Name,
                        WeekStartDate = weekStartDate,
                        Reason = "Timesheet not submitted for allocated project"
                    });
                }
            }
            return missedTimesheets;
        }

        private TimesheetResponseDTO TimesheetMap(Timesheet timesheet)
        {
            var totalHours = GetTotalHours(timesheet.MondayHours, timesheet.TuesdayHours, timesheet.WednesdayHours,
                timesheet.ThursdayHours, timesheet.FridayHours, timesheet.SaturdayHours, timesheet.SundayHours);

            return new TimesheetResponseDTO
            {
                Id = timesheet.Id,
                EmployeeId = timesheet.EmployeeId,
                EmployeeName = timesheet.Employee.FullName,
                ProjectId = timesheet.ProjectId,
                ProjectName = timesheet.Project.Name,
                TotalHours = totalHours,
                ActivityTag = timesheet.ActivityTag.ToString(),
                SubmittedAt = timesheet.SubmittedAt.AddHours(5).AddMinutes(30).ToString("dd/MM/yyyy hh:mm tt")
            };
        }

        private bool IsMonday
        (DateOnly weekStartDate)
        {
            return weekStartDate.DayOfWeek == DayOfWeek.Monday;
        }

        private bool AreHoursValid
        (int mondayHours, int tuesdayHours,
        int wednesdayHours, int thursdayHours, int fridayHours, int saturdayHours, int sundayHours)
        {
            var hours = new List<int>
            {
                mondayHours, tuesdayHours,
                wednesdayHours, thursdayHours,
                fridayHours, saturdayHours, sundayHours
            };

            if (hours.Any(h => h < 0 || h > 8))
                return false;

            if (saturdayHours > 0)
                return false;

            var totalHours = GetTotalHours(mondayHours, tuesdayHours, wednesdayHours, thursdayHours,
                fridayHours, saturdayHours, sundayHours);

            if (totalHours <= 0)
                return false;

            if (totalHours > 40)
                return false;

            return true;
        }

        private int GetTotalHours
        (int mondayHours, int tuesdayHours,
        int wednesdayHours,int thursdayHours, int fridayHours, int saturdayHours, int sundayHours)
        {
            return mondayHours + tuesdayHours + wednesdayHours + thursdayHours + fridayHours + saturdayHours + sundayHours;
        }
    }
}
