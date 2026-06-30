using VivreSync.Model.Enums;

namespace VivreSync.Model.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Users User { get; set; } = null!;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int? ManagerId { get; set; }
        public Employee? Manager { get; set; }
        public List<Employee> TeamMembers { get; set; } = new();
        public Designation Designation { get; set; }
        public List<EmployeeSkill> EmployeeSkills { get; set; } = new();
    }
}