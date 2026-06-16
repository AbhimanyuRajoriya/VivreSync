using Microsoft.EntityFrameworkCore;
using VivreSync.Model.Entities;
using VivreSync.Structure.Data;

namespace VivreSync.Projects.Repositories;
public class MilestoneRepository : IMilestoneRepository
{
    private readonly AppDbContext _context;
    public MilestoneRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<Milestone> GetAll()
    {
        return _context.Milestones
            .Include(m => m.Project)
            .ToList();
    }

    public Milestone? GetById(int id)
    {
        return _context.Milestones
            .Include(m => m.Project)
            .FirstOrDefault(m => m.Id == id);
    }

    public List<Milestone> GetByProjectId(int projectId)
    {
        return _context.Milestones
            .Include(m => m.Project)
            .Where(m => m.ProjectId == projectId)
            .ToList();
    }

    public void Add(Milestone milestone)
    {
        _context.Milestones.Add(milestone);
    }

    public void Update(Milestone milestone)
    {
        _context.Milestones.Update(milestone);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}