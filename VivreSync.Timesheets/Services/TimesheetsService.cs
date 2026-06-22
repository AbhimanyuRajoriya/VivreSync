using VivreSync.HR.Repositories;
using VivreSync.Model.Entities;
using VivreSync.Model.Enums;
using VivreSync.Projects.Repositories;
using VivreSync.Shared.Exceptions;
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
                throw new NotFoundException("Timesheet does not exist");
            return TimesheetMap(timesheet);
        }
        public TimesheetResponseDTO? CreateTimesheet(TimesheetCreateDTO dto)
        {
            if (!IsValidDate(dto.WeekStartDate))
                throw new BadRequestException("Enter Valid Date");

            if (!IsMonday(dto.WeekStartDate))
                throw new BadRequestException("Invalid week start date");

            var employee = _employeeRepository.GetById(dto.EmployeeId);
            if (employee == null || !employee.IsActive)
                throw new NotFoundException("Employee does not exist or Inactive");

            var project = _projectRepository.GetById(dto.ProjectId);
            if (project == null)
                throw new NotFoundException("Project does not exist");

            var weekEndDate = dto.WeekStartDate.AddDays(6);

            var isAllocated = _repository.IsEmployeeAllocatedToProject(dto.EmployeeId, dto.ProjectId, dto.WeekStartDate, weekEndDate);
            if (!isAllocated)
                throw new BadRequestException("Employee is not allocacted to project");

            var alreadySubmitted = _repository.ExistsForWeek(dto.EmployeeId, dto.ProjectId,dto.WeekStartDate);
            if (alreadySubmitted)
                throw new BadRequestException("Timesheet already submitted");

            if (!AreHoursValid(dto.MondayHours, dto.TuesdayHours, dto.WednesdayHours,
                dto.ThursdayHours, dto.FridayHours, dto.SaturdayHours, dto.SundayHours))
                throw new BadRequestException("Invalid weekly hours");

            if (string.IsNullOrWhiteSpace(dto.ActivityTag))
                throw new BadRequestException("Invalid activity tag");

            var isvalidactivity = Enum.TryParse<ActivityTags>(dto.ActivityTag, true, out var activityTags) || !Enum.IsDefined(typeof(ActivityTags), activityTags);
            if(!isvalidactivity)
                throw new BadRequestException("Invalid activity tag");

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

            var savedTimesheet = _repository.GetById(timesheet.Id);
            if (savedTimesheet == null)
                throw new NotFoundException("Timesheet could not be loaded after save");

            return TimesheetMap(savedTimesheet);
        }
        public bool UpdateTimesheet(int id,TimesheetUpdateDTO dto)
        {
            var timesheet = _repository.GetById(id);

            if (timesheet == null)
                throw new NotFoundException("Timesheet does not exist");

            if (!IsValidDate(dto.WeekStartDate))
                throw new BadRequestException("Invalid date");

            if (!IsMonday(dto.WeekStartDate))
                throw new BadRequestException("Invalid week start date");

            if (!AreHoursValid(dto.MondayHours,dto.TuesdayHours,dto.WednesdayHours,
                dto.ThursdayHours,dto.FridayHours,dto.SaturdayHours,dto.SundayHours))
                throw new BadRequestException("Invalid weekly hours");

            if (string.IsNullOrWhiteSpace(dto.ActivityTag))
                throw new BadRequestException("Invalid Activity Tag");

            var isvalidactivity = Enum.TryParse<ActivityTags>(dto.ActivityTag, true, out var activityTags) || !Enum.IsDefined(typeof(ActivityTags), activityTags);
            if (!isvalidactivity)
                throw new BadRequestException("Enter Valid Activity Tag");

            timesheet.WeekStartDate = dto.WeekStartDate;
            timesheet.MondayHours = dto.MondayHours;
            timesheet.TuesdayHours = dto.TuesdayHours;
            timesheet.WednesdayHours = dto.WednesdayHours;
            timesheet.ThursdayHours = dto.ThursdayHours;
            timesheet.FridayHours = dto.FridayHours;
            timesheet.SaturdayHours = dto.SaturdayHours;
            timesheet.SundayHours = dto.SundayHours;
            timesheet.ActivityTag = activityTags;

            _repository.Update(timesheet);
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

            if (!IsValidDate(weekStartDate))
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
                        Reason = $"Timesheet not submitted for {allocation.Project.Name} project"
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

            if (saturdayHours > 0 || sundayHours > 0)
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

        private bool IsValidDate(DateOnly date)
        {
            var minDate = new DateOnly(2026, 1, 1);
            var maxDate = new DateOnly(2100, 12, 31);

            return date >= minDate && date <= maxDate;
        }

    }
}
