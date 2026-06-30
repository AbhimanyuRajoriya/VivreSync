using VivreSync.Allocations.DTOs;

namespace VivreSync.Allocations.Services;
public interface IAllocationService
{
    List<AllocationResponseDTO> GetAll();
    AllocationResponseDTO? GetById(int id);
    AllocationResponseDTO? Create(AllocationCreateDTO dto);
    bool Delete(int id);
    bool Update(int id, AllocationUpdateDTO dto);
    List<EmployeeAllocationDTO>? GetEmployeeTable(int userId);
    List<EmployeeAllocationDTO>? GetFreeEmployee(int userId);
    List<EmployeeAllocationDTO>? GetOccupiedEmployee(int userId);
    bool CanManagerAccessProject(int userId, int projectId);
    bool CanManagerAccessAllocation(int userId, int allocationId);
    bool CanManagerAccessEmployee(int userId, int employeeId);
    List<AllocationResponseDTO> GetAllocationsForManager(int userId);
    List<AllocationResponseDTO> GetAllocationsForEmployee(int employeeId);

}