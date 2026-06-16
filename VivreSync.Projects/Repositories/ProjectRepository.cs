using Microsoft.EntityFrameworkCore;
using VivreSync.Model.Entities;
using VivreSync.Structure.Data;
namespace VivreSync.Projects.Repositories;
public class ProjectRepository: IProjectRepository
{
    private readonly AppDbContext _context;
    public ProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<Project> GetAll()
    {
        return _context.Projects
            .Include(p => p.Manager)
            .Include(p => p.Milestones)
            .ToList();
    }
    public Project? GetById(int id)
    {
        return _context.Projects
            .Include(p => p.Manager)
            .Include(p => p.Milestones)
            .FirstOrDefault(p => p.Id == id);
    }
    public void Add(Project project)
    {
        _context.Projects.Add(project);
    }
    public void Update(Project project)
    {
        _context.Projects.Update(project);
    }
    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}
