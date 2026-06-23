using VivreSync.HR.Repositories;
using VivreSync.Model.Entities;
using VivreSync.HR.DTOs;
using VivreSync.Model.Enums;
using Microsoft.AspNetCore.Identity;
using VivreSync.Shared.Exceptions;
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

            return employees.Where(e => e.IsActive)
                .Select(e => new EmployeeResponseDTO
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
                throw new NotFoundException("Employee not found");

            if (!employee.IsActive)
                throw new NotFoundException("Employee not active");

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
            var username = dto.UserName.Trim().ToLower();
            var existingUser = _userRepository.GetUser(username);

            if (existingUser != null)
                throw new BadRequestException("Username Already occupied");

            var isValidRole = Enum.TryParse<UserRoles>(dto.Role, true, out var parsedRole) && Enum.IsDefined(typeof(UserRoles), parsedRole);

            if (!isValidRole)
                throw new BadRequestException("Invalid Role"); ;

            if (parsedRole == UserRoles.Admin)
                throw new BadRequestException("Cannot Grant Admin Role"); ;

            var user = new Users
            {
                UserName = username,
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
            var employee = _repository.GetById(dto.EmployeeId);

            if (employee == null)
                throw new NotFoundException("Employee does not exists");

            employee.FullName = dto.FullName;
            employee.Designation = dto.Designation;
            employee.IsActive = dto.IsActive;

            if (employee.User != null)
            {
                employee.User.IsActive = dto.IsActive;
            }

            _repository.Update(employee);
            _repository.SaveChanges();

            return employee;
        }
        public bool Deactivate(int id)
        {
            var employee = _repository.GetById(id);
            if (employee == null)
                throw new NotFoundException("Employee does not exists");

            employee.IsActive = false;
            if (employee.User != null)
            {
                employee.User.IsActive = false;
            }

            _repository.Update(employee);
            _repository.SaveChanges();

            return true;
        }
        public bool IsEmployeeLinkedToUser(int employeeId, int userId)
        {
            return _repository.IsEmployeeLinkedToUser(employeeId, userId);
        }
    }
}