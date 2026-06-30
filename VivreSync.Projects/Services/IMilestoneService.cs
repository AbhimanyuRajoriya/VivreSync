using VivreSync.Projects.DTOs;

namespace VivreSync.Projects.Services;
public interface IMilestoneService
{
    List<MilestoneResponseDTO> GetAll();
    MilestoneResponseDTO? GetById(int id);
    List<MilestoneResponseDTO> GetByProjectId(int projectId);

    MilestoneResponseDTO? Create(MilestoneCreateDTO dto);
    bool Update(int id, MilestoneUpdateDTO dto);

    List<MilestoneResponseDTO> GetMilestonesForManager(int userId);
    bool CanManagerAccessMilestone(int userId, int milestoneId);
    bool CanManagerAccessProject(int userId, int projectId);
}
