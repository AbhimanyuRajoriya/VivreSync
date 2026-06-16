using VivreSync.Model.Entities;
namespace VivreSync.Projects.Repositories;
public interface IProjectRepository
{
     List<Project> GetAll();
    Project? GetById(int id);
    void Add(Project project);
    void Update(Project project);
    void SaveChanges();
}
