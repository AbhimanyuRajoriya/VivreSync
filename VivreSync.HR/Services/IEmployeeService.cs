using VivreSync.Model.Entities;
using VivreSync.HR.DTOs;
namespace VivreSync.HR.Services
{
    public interface IEmployeeService
    {
        List<EmployeeResponseDTO> GetAll();
        EmployeeResponseDTO? GetById(int id);
        Employee? Create(EmployeeCreateDTO dto);
        Employee? Update(EmployeeUpdateDTO dto);
        bool Deactivate(int id);
        bool IsEmployeeLinkedToUser(int employeeId, int userId);
    }
}
