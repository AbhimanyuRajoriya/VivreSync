using VivreSync.Projects.DTOs;
namespace VivreSync.Projects.Services;
public interface IProjectService
{
    List<ProjectResponseDTO> GetAll(int userId, string role);
    ProjectResponseDTO? GetById(int id, int userId, string role);
    ProjectResponseDTO? Create(ProjectCreateDTO dto);
    bool Update(ProjectUpdateDTO dto);
    ProjectHealthResponseDTO? GetProjectHealth(int projectId, int userId);
}