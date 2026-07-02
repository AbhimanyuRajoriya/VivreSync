using VivreSync.HR.Repositories;
using VivreSync.Model.Entities;
using VivreSync.Model.Enums;
using VivreSync.Projects.DTOs;
using VivreSync.Projects.Repositories;
using VivreSync.Shared.Exceptions;
namespace VivreSync.Projects.Services;
public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IEmployeeRepository _employeeRepository;
    public ProjectService(IProjectRepository projectRepository, IEmployeeRepository employeeRepository)
    {
        _projectRepository = projectRepository;
        _employeeRepository = employeeRepository;
    }
    public List<ProjectResponseDTO> GetAll(int userId, string role)
    {
        if (role == "Admin")
        {
            var projects = _projectRepository.GetAll();
            return projects.Select(MapToResponse).ToList();
        }
        if (role == "Manager")
        {
            var managerEmployee = GetManagerEmployee(userId);
            var projects = _projectRepository.GetProjectsByManager(managerEmployee.Id);

            return projects.Select(MapToResponse).ToList();
        }
        throw new BadRequestException("Access denied");
    }

    public ProjectResponseDTO? GetById(int id, int userId, string role)
    {
        var project = _projectRepository.GetById(id);
        if (project == null)
            throw new NotFoundException("Project does not exist");
        if (role == "Manager")
            ValidateManagerAccessProject(userId, project);
        else if (role != "Admin")
            throw new BadRequestException("Access denied");

        return MapToResponse(project);
    }

    public ProjectResponseDTO? Create(ProjectCreateDTO dto)
    {
        var manager = _employeeRepository.GetById(dto.ManagerId);
        if (manager == null || !manager.IsActive)
            throw new BadRequestException("Manager must exist and be an active employee");
        if (manager.User == null || !manager.User.IsActive)
            throw new BadRequestException("Manager user account is inactive");
        if (manager.User.Role != UserRoles.Manager)
            throw new BadRequestException("Employee must have Manager role");

        var project = new Project
        {
            Name = dto.Name.Trim(),
            Client = dto.Client.Trim(),
            Status = ProjectStatus.Active,
            ManagerId = dto.ManagerId
        };

        _projectRepository.Add(project);
        _projectRepository.SaveChanges();

        return new ProjectResponseDTO
        {
            Id = project.Id,
            Name = project.Name,
            Client = project.Client,
            Status = project.Status.ToString(),
            ManagerId = project.ManagerId,
            ManagerName = manager.FullName
        };
    }
    public bool Update(ProjectUpdateDTO dto)
    {
        var project = _projectRepository.GetById(dto.Id);
        if (project == null)
            throw new NotFoundException("Invalid project ID");

        var isValidStatus = Enum.TryParse<ProjectStatus>(dto.Status, true, out var parsedStatus) && Enum.IsDefined(typeof(ProjectStatus), parsedStatus);
        if (!isValidStatus)
            throw new BadRequestException("Invalid project status");

        project.Name = dto.Name.Trim();
        project.Client = dto.Client.Trim();
        project.Status = parsedStatus;

        _projectRepository.Update(project);
        _projectRepository.SaveChanges();

        return true;
    }

    public ProjectHealthResponseDTO? GetProjectHealth(int projectId, int userId)
    {
        var project = _projectRepository.GetProjectHealth(projectId);
        if (project == null)
            throw new NotFoundException("Project does not exist");

        ValidateManagerAccessProject(userId, project);

        var today = DateOnly.FromDateTime(DateTime.Today);
        var activeAllocations = _projectRepository.GetEmployeesinProject(projectId, today);
        var reasons = new List<string>();
        var delayedMilestones = project.Milestones
            .Where(m => m.Status == MilestoneStatus.Delayed).ToList();

        var overdueMilestones = project.Milestones
            .Where(m => m.DueDate < today && m.Status != MilestoneStatus.Completed).ToList();

        string health;

        if (project.Status == ProjectStatus.Completed)
        {
            reasons.Add($"{project.Name} is completed");
            return new ProjectHealthResponseDTO
            {
                ProjectID = project.Id,
                ProjectName = project.Name,
                Health = "Completed",
                Reasons = reasons
            };
        }

        if (project.Status == ProjectStatus.OnHold)
        {
            reasons.Add($"{project.Name} is currently on hold");
            return new ProjectHealthResponseDTO
            {
                ProjectID = project.Id,
                ProjectName = project.Name,
                Health = "OnHold",
                Reasons = reasons
            };
        }

        if (delayedMilestones.Any() || overdueMilestones.Any())
        {
            health = "Delayed";

            if (delayedMilestones.Any())
                reasons.Add($"{delayedMilestones.Count} milestone is marked delayed");

            if (overdueMilestones.Any())
                reasons.Add($"{overdueMilestones.Count} milestone is overdue");
        }
        else if (!activeAllocations.Any())
        {
            health = "AtRisk";
            reasons.Add("No employee is allocated for this project");
        }
        else
        {
            health = "OnTrack";
            reasons.Add("Project is on track");
        }

        return new ProjectHealthResponseDTO
        {
            ProjectID = project.Id,
            ProjectName = project.Name,
            Health = health,
            Reasons = reasons,

            Milestones = project.Milestones.Select(m => new ProjectMilestoneReponseDTO
            {
                MilestoneId = m.Id,
                Progress = m.Title,
                DueDate = m.DueDate,
                Status = m.Status.ToString()
            }).ToList(),

            Team = activeAllocations.Select(a => new ProjectTeamResponseDTO
            {
                EmployeeId = a.EmployeeId,
                EmployeeName = a.Employee.FullName,
                UtilizationPercentage = a.UtilizationPercentage,
                StartDate = a.StartDate,
                EndDate = a.EndDate
            }).ToList()
        };
    }

    private Employee GetManagerEmployee(int userId)
    {
        var managerEmployee = _employeeRepository.GetByUserId(userId);
        if (managerEmployee == null)
            throw new BadRequestException("Manager employee profile not found");

        return managerEmployee;
    }

    private void ValidateManagerAccessProject(int userId, Project project)
    {
        var managerEmployee = GetManagerEmployee(userId);
        if (project.ManagerId != managerEmployee.Id)
            throw new BadRequestException("Cannot access this project");
    }

    private ProjectResponseDTO MapToResponse(Project project)
    {
        return new ProjectResponseDTO
        {
            Id = project.Id,
            Name = project.Name,
            Client = project.Client,
            ManagerId = project.ManagerId,
            ManagerName = project.Manager.FullName,
            Status = project.Status.ToString()
        };
    }
}
