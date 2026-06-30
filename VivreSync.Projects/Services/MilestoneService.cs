using VivreSync.HR.Repositories;
using VivreSync.Model.Entities;
using VivreSync.Model.Enums;
using VivreSync.Projects.DTOs;
using VivreSync.Projects.Repositories;
using VivreSync.Shared.Exceptions;

namespace VivreSync.Projects.Services;
public class MilestoneService : IMilestoneService
{
    private readonly IMilestoneRepository _milestoneRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IProjectRepository _projectRepository;

    public MilestoneService(IMilestoneRepository milestoneRepository, IEmployeeRepository employeeRepository, IProjectRepository projectRepository)
    {
        _milestoneRepository = milestoneRepository;
        _employeeRepository = employeeRepository;
        _projectRepository = projectRepository;
    }

    public List<MilestoneResponseDTO> GetAll()
    {
        UpdateOverdueMilestones();
        var milestones = _milestoneRepository.GetAll();
        return milestones.Select(m => new MilestoneResponseDTO
        {
            Id = m.Id,
            Progress = m.Title,
            DueDate = m.DueDate,
            Status = m.Status.ToString(),
            ProjectId = m.ProjectId,
            ProjectName = m.Project.Name
        }).ToList();
    }

    public MilestoneResponseDTO? GetById(int id)
    {
        UpdateOverdueMilestones();
        var milestone = _milestoneRepository.GetById(id);
        if (milestone == null)
            throw new NotFoundException("Milestone does not exists");

        return new MilestoneResponseDTO
        {
            Id = milestone.Id,
            Progress = milestone.Title,
            DueDate = milestone.DueDate,
            Status = milestone.Status.ToString(),
            ProjectId = milestone.ProjectId,
            ProjectName = milestone.Project.Name
        };
    }

    public List<MilestoneResponseDTO> GetByProjectId(int projectId)
    {
        var milestones = _milestoneRepository.GetByProjectId(projectId);
        if (milestones == null)
            throw new BadRequestException("Invalid project ID");

        return milestones.Select(m => new MilestoneResponseDTO
        {
            Id = m.Id,
            Progress = m.Title,
            DueDate = m.DueDate,
            Status = m.Status.ToString(),
            ProjectId = m.ProjectId,
            ProjectName = m.Project.Name
        }).ToList();
    }

    public MilestoneResponseDTO? Create(MilestoneCreateDTO dto)
    {
        var project = _projectRepository.GetById(dto.ProjectId);
        if (project == null)
            throw new NotFoundException("Project does not exist");

        var milestone = new Milestone
        {
            Title = dto.Progress,
            DueDate = dto.DueDate,
            ProjectId = dto.ProjectId
        };
        _milestoneRepository.Add(milestone);
        _milestoneRepository.SaveChanges();

        return new MilestoneResponseDTO
        {
            Id = milestone.Id,
            Progress = milestone.Title,
            DueDate = milestone.DueDate,
            Status = milestone.Status.ToString(),
            ProjectId = milestone.ProjectId,
            ProjectName = project.Name
        };
    }

    public bool Update(int id, MilestoneUpdateDTO dto)
    {
        var milestone = _milestoneRepository.GetById(id);
        if (milestone == null)
            throw new NotFoundException("Milestone does not exist");

        var isValidStatus = Enum.TryParse<MilestoneStatus>(dto.Status, ignoreCase: true, out var parsedStatus)
                         && Enum.IsDefined(typeof(MilestoneStatus), parsedStatus);
        if (!isValidStatus)
            throw new BadRequestException("Invalid status");

        milestone.Title = dto.Progress;
        milestone.DueDate = dto.DueDate;
        milestone.Status = parsedStatus;

        _milestoneRepository.Update(milestone);
        _milestoneRepository.SaveChanges();
        return true;
    }

    private void UpdateOverdueMilestones()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var milestones = _milestoneRepository.GetAll();

        foreach (var milestone in milestones)
        {
            if (milestone.DueDate < today && milestone.Status != MilestoneStatus.Completed)
            {
                milestone.Status = MilestoneStatus.Delayed;
                _milestoneRepository.Update(milestone);
            }
        }
        _milestoneRepository.SaveChanges();
    }

    public List<MilestoneResponseDTO> GetMilestonesForManager(int userId)
    {
        var managerEmployee = _employeeRepository.GetByUserId(userId);
        if (managerEmployee == null)
            throw new BadRequestException("Manager employee profile not found");

        var milestones = _milestoneRepository.GetMilestonesByManager(managerEmployee.Id);
        return milestones.Select(MapToResponse).ToList();
    }

    public bool CanManagerAccessMilestone(int userId, int milestoneId)
    {
        var managerEmployee = _employeeRepository.GetByUserId(userId);
        if (managerEmployee == null)
            return false;

        return _milestoneRepository.IsMilestoneManagedBy(milestoneId, managerEmployee.Id);
    }

    public bool CanManagerAccessProject(int userId, int projectId)
    {
        var managerEmployee = _employeeRepository.GetByUserId(userId);
        if (managerEmployee == null)
            return false;

        return _projectRepository.IsProjectManagedBy(projectId, managerEmployee.Id);
    }

    private MilestoneResponseDTO MapToResponse(Milestone milestone)
    {
        return new MilestoneResponseDTO
        {
            Id = milestone.Id,
            ProjectId = milestone.ProjectId,
            ProjectName = milestone.Project.Name,
            Progress = milestone.Title,
            DueDate = milestone.DueDate,
            Status = milestone.Status.ToString()
        };
    }
}
