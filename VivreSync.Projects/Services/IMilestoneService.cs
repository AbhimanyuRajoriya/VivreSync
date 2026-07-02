using VivreSync.Projects.DTOs;

namespace VivreSync.Projects.Services;
public interface IMilestoneService
{
    List<MilestoneResponseDTO> GetAll(int userId);
    MilestoneResponseDTO? GetById(int id, int userId);
    List<MilestoneResponseDTO> GetByProjectId(int projectId, int userId);
    MilestoneResponseDTO? Create(MilestoneCreateDTO dto, int userId);
    bool Update(int id, MilestoneUpdateDTO dto, int userId);
}
