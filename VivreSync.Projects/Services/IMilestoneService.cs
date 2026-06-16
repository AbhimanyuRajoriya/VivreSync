using VivreSync.Projects.DTOs;

namespace VivreSync.Projects.Services;
public interface IMilestoneService
{
    List<MilestoneResponseDTO> GetAll();
    MilestoneResponseDTO? GetById(int id);
    List<MilestoneResponseDTO> GetByProjectId(int projectId);
    MilestoneResponseDTO? Create(MilestoneCreateDTO dto);
    bool Update(int id, MilestoneUpdateDTO dto);
}
