using VivreSync.Allocations.DTOs;

namespace VivreSync.Allocations.Services;
public interface IAllocationService
{
    List<AllocationResponseDTO> GetAll();
    AllocationResponseDTO? GetById(int id);
    AllocationResponseDTO? Create(AllocationCreateDTO dto);
    List<EmployeeAllocationDTO>? GetEmployeeTable();
    List<EmployeeAllocationDTO>? GetFreeEmployee();
    List<EmployeeAllocationDTO>? GetOccupiedEmployee();
    bool CanManagerAccessProject(int userId, int projectId);
    bool CanManagerAccessAllocation(int userId, int allocationId);
    bool Delete(int id);
    bool Update(int id, AllocationUpdateDTO dto);
}