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
            .ToList();
    }
    public Project? GetById(int id)
    {
        return _context.Projects
            .Include(p => p.Manager)
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
    public Project? GetProjectHealth(int projectid)
    {
        return _context.Projects
       .Include(p => p.Milestones)
       .FirstOrDefault(p => p.Id == projectid);
    }

    public List<Allocation> GetEmployeesinProject(int projectId, DateOnly today)
    {
        return _context.Allocations
       .Include(a => a.Employee)
       .Where(a =>
           a.ProjectId == projectId && a.Employee.IsActive &&
           a.StartDate <= today && a.EndDate >= today).ToList();
    }

    public bool IsProjectManagedBy(int projectId, int managerEmployeeId)
    {
        return _context.Projects.Any(p =>
            p.Id == projectId &&
            p.ManagerId == managerEmployeeId);
    }

    public List<Project> GetProjectsByManager(int managerEmployeeId)
    {
        return _context.Projects
            .Where(p => p.ManagerId == managerEmployeeId)
            .ToList();
    }
}
