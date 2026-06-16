using VivreSync.HR.Repositories;
using VivreSync.Model.Entities;
using VivreSync.HR.DTOs;
using VivreSync.Model.Enums;
using Microsoft.AspNetCore.Identity;
namespace VivreSync.HR.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly IUserRepository _userRepository;
        public EmployeeService(IEmployeeRepository employeeRepository, IUserRepository userRepository)
        {
            _repository = employeeRepository;
            _userRepository = userRepository;
        }

        public List<EmployeeResponseDTO> GetAll()
        {
            var employees = _repository.GetAll();

            return employees.Select(e => new EmployeeResponseDTO
            {
                Id = e.Id,
                FullName = e.FullName,
                Email = e.Email,
                Designation = e.Designation,
                IsActive = e.IsActive,

                Skills = e.EmployeeSkills.Select(es => new EmployeeSkillResponseDTO
                {
                    SkillId = es.SkillId,
                    SkillName = es.Skill.Name,
                    Level = es.Level.ToString()
                }).ToList()

            }).ToList();
        }

        public EmployeeResponseDTO? GetById(int id)
        {
            var employee = _repository.GetById(id);

            if (employee == null)
                return null;

            return new EmployeeResponseDTO
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Email = employee.Email,
                Designation = employee.Designation,
                IsActive = employee.IsActive,

                Skills = employee.EmployeeSkills.Select(es => new EmployeeSkillResponseDTO
                {
                    SkillId = es.SkillId,
                    SkillName = es.Skill.Name,
                    Level = es.Level.ToString()
                }).ToList()
            };
        }
        public Employee? Create(EmployeeCreateDTO dto)
        {
            var existingUser = _userRepository.GetUser(dto.UserName);

            if (existingUser != null)
                return null;
            
                var isValidRole = Enum.TryParse<UserRoles>(
                dto.Role,
                ignoreCase: true,
                out var parsedRole);

            if (!isValidRole)
                return null;

            if (parsedRole == UserRoles.Admin)
                return null;

            var user = new Users
            {
                UserName = dto.UserName,
                Role = parsedRole,
                IsActive = true,
                PasswordChangeRequired = true
            };

            var hasher = new PasswordHasher<Users>();
            user.PasswordHash = hasher.HashPassword(user, dto.Password);

            _userRepository.Add(user);
            _userRepository.Save();

            var employee = new Employee
            {
                FullName = dto.Name,
                Email = dto.Email,
                Designation = dto.Designation,
                IsActive = true,
                UserId = user.Id
            };

            _repository.Add(employee);
            _repository.SaveChanges();

            return employee;
        }
        public Employee? Update(EmployeeUpdateDTO dto)
        {
            var employee = _repository.GetById(dto.Id);

            if (employee == null)
                return null;

            employee.IsActive = dto.IsActive;

            _repository.Update(employee);
            _repository.SaveChanges();

            return employee;
        }
    }
}