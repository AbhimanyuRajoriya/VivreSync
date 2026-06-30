using VivreSync.Model.Entities;
namespace VivreSync.Projects.Repositories;

public interface IMilestoneRepository
{
    List<Milestone> GetAll();
    Milestone? GetById(int id);
    List<Milestone> GetByProjectId(int projectId);
    void Add(Milestone milestone);
    void Update(Milestone milestone);
    void SaveChanges();
    List<Milestone> GetMilestonesByManager(int managerEmployeeId);
    bool IsMilestoneManagedBy(int milestoneId, int managerEmployeeId);
}
