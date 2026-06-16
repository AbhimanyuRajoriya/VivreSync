using VivreSync.Projects.DTOs;
namespace VivreSync.Projects.Services;
public interface IProjectService
{
    List<ProjectResponseDTO> GetAll();
    ProjectResponseDTO? GetById(int id);
    ProjectResponseDTO? Create(ProjectCreateDTO dto);
    bool Update(ProjectUpdateDTO dto);
}