using VivreSync.Projects.Repositories;
using VivreSync.Projects.DTOs;
using VivreSync.HR.Repositories;
using VivreSync.Model.Entities;
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
            ManagerName = p.Manager.FullName,
            Milestones = p.Milestones.Select(m => new ProjMilestoneResponseDTO
            {
                Title = m.Title,
                DueDate = m.DueDate
            }).ToList()
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
            ManagerName = project.Manager.FullName,
            Milestones = project.Milestones.Select(m => new ProjMilestoneResponseDTO
            {
                Title = m.Title,
                DueDate = m.DueDate
            }).ToList()
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

        if (project == null)
            return false;

        var manager = _employeeRepository.GetById(dto.ManagerId);

        if (manager == null)
            return false;

        project.Name = dto.Name;
        project.Client = dto.Client;
        project.ManagerId = dto.ManagerId;

        _projectRepository.Update(project);
        _projectRepository.SaveChanges();

        return true;
    }
}
