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

    public List<MilestoneResponseDTO> GetAll(int userId)
    {
        UpdateOverdueMilestones();
        var managerEmployee = GetManagerEmployee(userId);
        var milestones = _milestoneRepository.GetMilestonesByManager(managerEmployee.Id);

        return milestones.Select(MapToResponse).ToList();
    }

    public MilestoneResponseDTO? GetById(int id, int userId)
    {
        UpdateOverdueMilestones();
        var milestone = _milestoneRepository.GetById(id);
        if (milestone == null)
            throw new NotFoundException("Milestone does not exist");

        ValidateManagerAccessMilestone(userId, milestone);

        return MapToResponse(milestone);
    }

    public List<MilestoneResponseDTO> GetByProjectId(int projectId, int userId)
    {
        ValidateManagerAccessProject(userId, projectId);
        var milestones = _milestoneRepository.GetByProjectId(projectId);

        return milestones.Select(MapToResponse).ToList();
    }

    public MilestoneResponseDTO? Create(MilestoneCreateDTO dto, int userId)
    {
        ValidateManagerAccessProject(userId, dto.ProjectId);

        var project = _projectRepository.GetById(dto.ProjectId);
        if (project == null)
            throw new NotFoundException("Project does not exist");

        if (string.IsNullOrWhiteSpace(dto.Progress))
            throw new BadRequestException("Progress is required");

        if (dto.DueDate == null)
            throw new BadRequestException("Due date is required");

        if (dto.DueDate.Value < DateOnly.FromDateTime(DateTime.Today))
            throw new BadRequestException("Due date cannot be older than today");

        var milestone = new Milestone
        {
            Title = dto.Progress.Trim(),
            DueDate = dto.DueDate.Value,
            ProjectId = dto.ProjectId
        };

        _milestoneRepository.Add(milestone);
        _milestoneRepository.SaveChanges();

        var savedMilestone = _milestoneRepository.GetById(milestone.Id);
        if (savedMilestone == null)
            throw new NotFoundException("Milestone could not be loaded after save");

        return MapToResponse(savedMilestone);
    }

    public bool Update(int id, MilestoneUpdateDTO dto, int userId)
    {
        var milestone = _milestoneRepository.GetById(id);
        if (milestone == null)
            throw new NotFoundException("Milestone does not exist");

        ValidateManagerAccessMilestone(userId, milestone);

        if (string.IsNullOrWhiteSpace(dto.Progress))
            throw new BadRequestException("Progress is required");

        if (dto.DueDate == null)
            throw new BadRequestException("Due date is required");

        if (dto.DueDate.Value < DateOnly.FromDateTime(DateTime.Today))
            throw new BadRequestException("Due date cannot be older than today");

        if (dto.DueDate.Value < milestone.DueDate)
            throw new BadRequestException("Milestone due date can only be postponed, not shortened");

        if (string.IsNullOrWhiteSpace(dto.Status))
            throw new BadRequestException("Status is required");

        var isValidStatus = Enum.TryParse<MilestoneStatus>(dto.Status, true, out var parsedStatus) && Enum.IsDefined(typeof(MilestoneStatus), parsedStatus);
        if (!isValidStatus)
            throw new BadRequestException("Invalid status");

        milestone.Title = dto.Progress.Trim();
        milestone.DueDate = dto.DueDate.Value;
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

    private Employee GetManagerEmployee(int userId)
    {
        var managerEmployee = _employeeRepository.GetByUserId(userId);
        if (managerEmployee == null)
            throw new BadRequestException("Manager employee profile not found");
        return managerEmployee;
    }

    private void ValidateManagerAccessProject(int userId, int projectId)
    {
        var managerEmployee = GetManagerEmployee(userId);
        var project = _projectRepository.GetById(projectId);
        if (project == null)
            throw new NotFoundException("Project does not exist");
        if (project.ManagerId != managerEmployee.Id)
            throw new BadRequestException("Cannot access this project");
    }

    private void ValidateManagerAccessMilestone(int userId, Milestone milestone)
    {
        var managerEmployee = GetManagerEmployee(userId);
        if (milestone.Project.ManagerId != managerEmployee.Id)
            throw new BadRequestException("Cannot access this milestone");
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
