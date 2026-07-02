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

        public List<TimesheetResponseDTO> GetAllTimesheets(int userId, string role)
        {
            if (role == "Manager")
            {
                var manager = _employeeRepository.GetByUserId(userId);
                if (manager == null)
                    throw new BadRequestException("Manager profile not found");

                return _repository.GetAll()
                    .Where(t => t.Employee.ManagerId == manager.Id)
                    .Select(TimesheetMap)
                    .ToList();
            }

            var employee = _employeeRepository.GetByUserId(userId);
            if (employee == null)
                throw new BadRequestException("Employee profile not found");

            return _repository.GetByEmployeeId(employee.Id)
                .Select(TimesheetMap)
                .ToList();
        }
        public TimesheetResponseDTO? GetTimesheetById(int id, int userId, string role)
        {
            if (id <= 0)
                throw new BadRequestException("Enter valid Timesheet Id");

            var timesheet = _repository.GetById(id);
            if (timesheet == null)
                throw new NotFoundException("Timesheet not found");

            if (role == "Manager")
            {
                var manager = _employeeRepository.GetByUserId(userId);
                if (manager == null)
                    throw new BadRequestException("Manager profile not found");

                if (timesheet.Employee.ManagerId != manager.Id)
                    throw new UnauthorizedException("Cannot access this timesheet");
            }
            if (role == "Employee")
            {
                var employee = _employeeRepository.GetByUserId(userId);
                if (employee == null)
                    throw new BadRequestException("Employee profile not found");

                if (timesheet.EmployeeId != employee.Id)
                    throw new UnauthorizedException("Cannot access this timesheet");
            }

            return TimesheetMap(timesheet);
        }
        public TimesheetResponseDTO? CreateTimesheet(TimesheetCreateDTO dto, int userId)
        {
            if (dto == null)
                throw new BadRequestException("Enter the required data");

            if (dto.WeekStartDate == null)
                throw new BadRequestException("Week Start Date is required");

            var loggedInEmployee = _employeeRepository.GetByUserId(userId);
            if (loggedInEmployee == null)
                throw new BadRequestException("Employee profile not found");

            if (dto.EmployeeId != loggedInEmployee.Id)
                throw new UnauthorizedException("Cannot create timesheet for other employee");

            if (!IsValidDate(dto.WeekStartDate.Value))
                throw new BadRequestException("Enter valid date");

            if (!IsMonday(dto.WeekStartDate.Value))
                throw new BadRequestException("Invalid week start date");

            if (dto.WeekStartDate > GetCurrentWeekStartDate())
                throw new BadRequestException("Future timesheets cannot be submitted");

            var employee = _employeeRepository.GetById(dto.EmployeeId);
            if (employee == null || !employee.IsActive)
                throw new NotFoundException("Employee does not exist");

            var project = _projectRepository.GetById(dto.ProjectId);
            if (project == null)
                throw new NotFoundException("Project does not exist");

            var weekEndDate = dto.WeekStartDate.Value.AddDays(6);
            var isAllocated = _repository.IsEmployeeAllocatedToProject(dto.EmployeeId, dto.ProjectId, dto.WeekStartDate.Value, weekEndDate);
            if (!isAllocated)
                throw new BadRequestException("Employee is not allocated to project");

            var alreadySubmitted = _repository.ExistsForWeek(dto.EmployeeId, dto.ProjectId, dto.WeekStartDate.Value);
            if (alreadySubmitted)
                throw new BadRequestException("Timesheet already submitted");

            if (!AreHoursValid(dto.MondayHours.Value, dto.TuesdayHours.Value, dto.WednesdayHours.Value, dto.ThursdayHours.Value, dto.FridayHours.Value, dto.SaturdayHours.Value, dto.SundayHours.Value))
            {
                throw new BadRequestException("Invalid weekly hours");
            }

            if (string.IsNullOrWhiteSpace(dto.ActivityTag))
                throw new BadRequestException("Invalid activity tag");

            var isValidActivity = Enum.TryParse<ActivityTags>(dto.ActivityTag, true, out var activityTags) && Enum.IsDefined(typeof(ActivityTags), activityTags);
            if (!isValidActivity)
                throw new BadRequestException("Invalid activity tag");

            var timesheet = new Timesheet
            {
                EmployeeId = dto.EmployeeId,
                ProjectId = dto.ProjectId,
                WeekStartDate = dto.WeekStartDate.Value,
                MondayHours = dto.MondayHours.Value,
                TuesdayHours = dto.TuesdayHours.Value,
                WednesdayHours = dto.WednesdayHours.Value,
                ThursdayHours = dto.ThursdayHours.Value,
                FridayHours = dto.FridayHours.Value,
                SaturdayHours = dto.SaturdayHours.Value,
                SundayHours = dto.SundayHours.Value,
                ActivityTag = activityTags,
                SubmittedAt = DateTime.UtcNow
            };

            _repository.Add(timesheet);

            var savedTimesheet = _repository.GetById(timesheet.Id);
            if (savedTimesheet == null)
                throw new NotFoundException("Timesheet could not be loaded after save");

            return TimesheetMap(savedTimesheet);
        }

        public bool UpdateTimesheet(int id, TimesheetUpdateDTO dto, int userId)
        {
            if (id <= 0)
                throw new BadRequestException("Enter valid Timesheet Id");

            if (dto == null)
                throw new BadRequestException("Enter the required data");

            var loggedInEmployee = _employeeRepository.GetByUserId(userId);

            if (loggedInEmployee == null)
                throw new BadRequestException("Employee profile not found");

            var timesheet = _repository.GetById(id);

            if (timesheet == null)
                throw new NotFoundException("Timesheet does not exist");

            if (timesheet.EmployeeId != loggedInEmployee.Id)
                throw new UnauthorizedException("Cannot update timesheet of other employee");

            var currentWeekStartDate = GetCurrentWeekStartDate();

            if (timesheet.WeekStartDate < currentWeekStartDate)
                throw new BadRequestException("Previous week's submitted timesheet cannot be updated");

            if (timesheet.WeekStartDate > currentWeekStartDate)
                throw new BadRequestException("Future timesheets cannot be updated");

            if (!AreHoursValid(dto.MondayHours.Value, dto.TuesdayHours.Value, dto.WednesdayHours.Value, dto.ThursdayHours.Value, dto.FridayHours.Value, dto.SaturdayHours.Value, dto.SundayHours.Value))
                throw new BadRequestException("Invalid weekly hours");

            if (string.IsNullOrWhiteSpace(dto.ActivityTag))
                throw new BadRequestException("Activity tag is required");

            var isValidActivity = Enum.TryParse<ActivityTags>(dto.ActivityTag, true, out var activityTags) && Enum.IsDefined(typeof(ActivityTags), activityTags);

            if (!isValidActivity)
                throw new BadRequestException("Enter valid activity tag");

            var weekEndDate = timesheet.WeekStartDate.AddDays(6);

            var isAllocated = _repository.IsEmployeeAllocatedToProject(
                timesheet.EmployeeId,
                timesheet.ProjectId,
                timesheet.WeekStartDate,
                weekEndDate);

            if (!isAllocated)
                throw new BadRequestException("Employee is not allocated to this project for this week");

            timesheet.MondayHours = dto.MondayHours.Value;
            timesheet.TuesdayHours = dto.TuesdayHours.Value;
            timesheet.WednesdayHours = dto.WednesdayHours.Value;
            timesheet.ThursdayHours = dto.ThursdayHours.Value;
            timesheet.FridayHours = dto.FridayHours.Value;
            timesheet.SaturdayHours = dto.SaturdayHours.Value;
            timesheet.SundayHours = dto.SundayHours.Value;
            timesheet.ActivityTag = activityTags;

            _repository.Update(timesheet);

            return true;
        }

        public List<TimesheetResponseDTO> GetByEmployeeId(int employeeId, int userId, string role)
        {
            if (employeeId <= 0)
                throw new BadRequestException("Enter valid Employee Id");

            if (role == "Manager")
            {
                var manager = _employeeRepository.GetByUserId(userId);
                if (manager == null)
                    throw new BadRequestException("Manager employee profile not found");

                var isOwnEmployee = _employeeRepository.IsEmployeeUnderManager(employeeId, manager.Id);
                if (!isOwnEmployee)
                    throw new UnauthorizedException("Cannot see timesheets of this employee");
            }
            if (role == "Employee")
            {
                var employee = _employeeRepository.GetByUserId(userId);
                if (employee == null)
                    throw new BadRequestException("Employee profile not found");
                if (employee.Id != employeeId)
                    throw new UnauthorizedException("Cannot see timesheets of other employee");
            }
            return _repository.GetByEmployeeId(employeeId).Select(TimesheetMap).ToList();
        }

        public List<TimesheetMissedDTO> GetMissedTimesheets(DateOnly weekStartDate, int userId)
        {
            var manager = _employeeRepository.GetByUserId(userId);
            if (manager == null)
                throw new BadRequestException("Manager employee profile not found");

            if (!IsMonday(weekStartDate))
                throw new BadRequestException("Week start date must be Monday");

            if (!IsValidDate(weekStartDate))
                throw new BadRequestException("Enter valid date");

            var weekEndDate = weekStartDate.AddDays(6);
            var allocations = _repository.GetAllocationsForWeek(weekStartDate, weekEndDate)
                .Where(a =>
                    a.Employee.ManagerId == manager.Id &&
                    a.Employee.IsActive)
                .ToList();

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

        private DateOnly GetCurrentWeekStartDate()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var daysSinceMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;

            return today.AddDays(-daysSinceMonday);
        }
    }
}
