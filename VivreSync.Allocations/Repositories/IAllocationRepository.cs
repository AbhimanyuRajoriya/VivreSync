using VivreSync.Model.Entities;
namespace VivreSync.Allocations.Repositories;

public interface IAllocationRepository
{
    List<Allocation> GetAll();
    Allocation? GetById(int id);
    List<Allocation> GetOverlappingAllocations(int employeeId, DateOnly startDate, DateOnly endDate);
    void Add(Allocation allocation);
    void Delete(Allocation allocation); 
    void Update(Allocation allocation);
}
