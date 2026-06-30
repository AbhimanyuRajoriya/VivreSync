using VivreSync.Model.Entities;
using VivreSync.HR.DTOs;
namespace VivreSync.HR.Services
{
    public interface IEmployeeService
    {
        List<EmployeeResponseDTO> GetAll();
        EmployeeResponseDTO? GetById(int id);
        EmployeeResponseDTO? Create(EmployeeCreateDTO dto);
        EmployeeResponseDTO? Update(EmployeeUpdateDTO dto);
        bool Deactivate(int? id);
        EmployeeResponseDTO ActivateEmployee(int? id);
        List<EmployeeResponseDTO> GetInactiveEmployees();
        List<EmployeeResponseDTO> GetEmployeesUnderManager(int managerEmployeeId);
        int GetEmployeeIdByUserId(int userId);
        bool IsEmployeeUnderManager(int employeeId, int managerEmployeeId);
        bool IsEmployeeLinkedToUser(int employeeId, int userId);
    }
}
