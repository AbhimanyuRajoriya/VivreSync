using VivreSync.Allocations.DTOs;

namespace VivreSync.Allocations.Services;
public interface IAllocationService
{
    List<AllocationResponseDTO> GetAll();
    AllocationResponseDTO? GetById(int id);
    AllocationResponseDTO? Create(AllocationCreateDTO dto);
    bool Delete(int id);
    bool Update(int id, AllocationUpdateDTO dto);
}