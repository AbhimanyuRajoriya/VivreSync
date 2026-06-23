using VivreSync.Model.Entities;

namespace VivreSync.HR.Repositories
{
    public interface IEmployeeRepository
    {
        List<Employee> GetAll();
        Employee? GetById(int id);
        void Add(Employee employee);
        void Update(Employee employee);
        void SaveChanges();
        bool IsEmployeeLinkedToUser(int employeeId, int userId);
    }
}
