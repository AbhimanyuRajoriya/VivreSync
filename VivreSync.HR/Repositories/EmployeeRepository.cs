using VivreSync.Structure.Data;
using VivreSync.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace VivreSync.HR.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;
        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Employee> GetAll()
        {
            return _context.Employees
                .Include(e => e.User)
                .Include(e => e.EmployeeSkills)
                .ThenInclude(es => es.Skill)
                .ToList();
        }

        public Employee? GetById(int id)
        {
            return _context.Employees
                .Include(e => e.User)
                .Include(e => e.EmployeeSkills)
                .ThenInclude(es => es.Skill)
                .FirstOrDefault(e => e.Id == id);
        }
        public void Add(Employee employee)
        {
            _context.Employees.Add(employee);
        }
        public void Update(Employee employee)
        {
            _context.Employees.Update(employee);
        }
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
        public bool IsEmployeeLinkedToUser(int employeeId, int userId)
        {
            return _context.Employees.Any(e =>
                e.Id == employeeId &&
                e.UserId == userId &&
                e.IsActive);
        }

        public List<Employee> GetInactiveEmployees()
        {
            return _context.Employees
                .Include(e => e.User)
                .Include(e => e.EmployeeSkills)
                .ThenInclude(es => es.Skill)
                .Where(e => !e.IsActive || !e.User.IsActive)
                .ToList();
        }
    }
}