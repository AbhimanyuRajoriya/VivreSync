using VivreSync.Model.Entities;

namespace VivreSync.HR.Repositories
{
    public interface IEmployeeRepository
    {
        List<Employee> GetAll();
        Employee? GetById(int id);
        Employee? GetByUserId(int userId);
        void Add(Employee employee);
        void Update(Employee employee);
        void SaveChanges();
        bool IsEmployeeLinkedToUser(int employeeId, int userId);
        List<Employee> GetInactiveEmployees();
    }
}
