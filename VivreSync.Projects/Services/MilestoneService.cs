using VivreSync.Model.Entities;
using VivreSync.Model.Enums;
using VivreSync.Projects.DTOs;
using VivreSync.Projects.Repositories;

namespace VivreSync.Projects.Services;
public class MilestoneService : IMilestoneService
{
    private readonly IMilestoneRepository _milestoneRepository;
    private readonly IProjectRepository _projectRepository;
    public MilestoneService(IMilestoneRepository milestoneRepository,IProjectRepository projectRepository)
    {
        _milestoneRepository = milestoneRepository;
        _projectRepository = projectRepository;
    }

    public List<MilestoneResponseDTO> GetAll()
    {
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
        var milestone = _milestoneRepository.GetById(id);
        if (milestone == null)
            return null;

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
            return null;

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
            return false;

        var isValidStatus = Enum.TryParse<MilestoneStatus>(dto.Status, ignoreCase: true, out var parsedStatus);
        if (!isValidStatus)
            return false;

        milestone.Title = dto.Progress;
        milestone.DueDate = dto.DueDate;
        milestone.Status = parsedStatus;

        _milestoneRepository.Update(milestone);
        _milestoneRepository.SaveChanges();
        return true;
    }
}
