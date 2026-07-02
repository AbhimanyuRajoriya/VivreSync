using VivreSync.Allocations.DTOs;

namespace VivreSync.Allocations.Services;
public interface IAllocationService
{
    List<AllocationResponseDTO> GetAll(int userId, string role);
    AllocationResponseDTO GetById(int id, int userId, string role);
    AllocationResponseDTO Create(AllocationCreateDTO dto, int userId);
    bool Update(int id, AllocationUpdateDTO dto, int userId);
    bool EndAllocation(int id, int userId);
    List<EmployeeAllocationDTO>? GetEmployeeTable(int userId);
    List<EmployeeAllocationDTO>? GetFreeEmployee(int userId);
    List<EmployeeAllocationDTO>? GetOccupiedEmployee(int userId);
}