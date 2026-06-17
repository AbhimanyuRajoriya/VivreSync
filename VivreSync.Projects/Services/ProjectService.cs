using VivreSync.HR.Repositories;
using VivreSync.Model.Entities;
using VivreSync.Model.Enums;
using VivreSync.Projects.DTOs;
using VivreSync.Projects.Repositories;
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
    public List<ProjectResponseDTO> GetAll()
    {
        var projects = _projectRepository.GetAll();

        return projects.Select(p => new ProjectResponseDTO
        {
            Id = p.Id,
            Name = p.Name,
            Client = p.Client,
            ManagerId = p.ManagerId,
            ManagerName = p.Manager.FullName
        }).ToList();
    }
    public ProjectResponseDTO? GetById(int id)
    {
        var project = _projectRepository.GetById(id);

        if (project == null)
            return null;

        return new ProjectResponseDTO
        {
            Id = project.Id,
            Name = project.Name,
            Client = project.Client,
            ManagerId = project.ManagerId,
            ManagerName = project.Manager.FullName
        };
    }
    public ProjectResponseDTO? Create(ProjectCreateDTO dto)
    {
        var manager = _employeeRepository.GetById(dto.ManagerId);

        if (manager == null)
            return null;

        var project = new Project
        {
            Name = dto.Name,
            Client = dto.Client,
            ManagerId = dto.ManagerId
        };

        _projectRepository.Add(project);
        _projectRepository.SaveChanges();

        return new ProjectResponseDTO
        {
            Id = project.Id,
            Name = project.Name,
            Client = project.Client,
            ManagerId = project.ManagerId,
            ManagerName = manager.FullName
        };
    }
    public bool Update(ProjectUpdateDTO dto)
    {
        var project = _projectRepository.GetById(dto.Id);
        var manager = _employeeRepository.GetById(dto.ManagerId);
        if (project == null || manager == null)
            return false;

        project.Name = dto.Name;
        project.Client = dto.Client;
        project.ManagerId = dto.ManagerId;

        _projectRepository.Update(project);
        _projectRepository.SaveChanges();

        return true;
    }

    public ProjectHealthResponseDTO? GetProjectHealth(int projectId)
    {
        var project = _projectRepository.GetProjectHealth(projectId);
        if (project == null)
            return null;

        var today = DateOnly.FromDateTime(DateTime.Today);

        var activeAllocations = _projectRepository
            .GetEmployeesinProject(projectId, today);

        var reasons = new List<string>();

        var delayedMilestones = project.Milestones
            .Where(m => m.Status.ToString() == "Delayed").ToList();

        var overdueMilestones = project.Milestones
            .Where(m =>
                m.DueDate < today &&
                m.Status.ToString() != "Completed").ToList();

        string health;

        if (delayedMilestones.Any() || overdueMilestones.Any())
        {
            health = "Delayed";

            if (delayedMilestones.Any())
                reasons.Add($"{delayedMilestones.Count} milestone is marked delayed");

            if (overdueMilestones.Any())
                reasons.Add($"{overdueMilestones.Count} milestone is overdue");
        }
        else
        {
            health = "OnTrack";
            reasons.Add("Project milestones and team allocation are on track");
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
}
