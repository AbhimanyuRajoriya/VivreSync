using VivreSync.Structure.Data;
using VivreSync.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace VivreSync.Allocations.Repositories;

public class AllocationRepository : IAllocationRepository
{
    private readonly AppDbContext _context;

    public AllocationRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<Allocation> GetAll()
    {
        return _context.Allocations
            .Include(a => a.Employee)
            .Include(a => a.Project)
            .ToList();
    }

    public Allocation? GetById(int id)
    {
        return _context.Allocations
            .Include(a => a.Employee)
            .Include(a => a.Project)
            .FirstOrDefault(a => a.Id == id);
    }

    public List<Allocation> GetOverlappingAllocations(int employeeId, DateOnly startDate, DateOnly endDate)
    {
        return _context.Allocations
            .Where(a =>
                a.EmployeeId == employeeId &&
                a.StartDate <= endDate &&
                a.EndDate >= startDate)
            .ToList();
    }

    public void Add(Allocation allocation)
    {
        _context.Allocations.Add(allocation);
        _context.SaveChanges();
    }

    public void Delete(Allocation allocation)
    {
        _context.Allocations.Remove(allocation);
        _context.SaveChanges();
    }

    public void Update(Allocation allocation)
    {
        _context.Allocations.Update(allocation);
        _context.SaveChanges();
    }
}
