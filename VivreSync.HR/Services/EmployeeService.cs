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
        public EmployeeResponseDTO Create(EmployeeCreateDTO dto)
        {
            var username = dto.UserName.Trim().ToLower();

            var existingUser = _userRepository.GetUser(username);

            if (existingUser != null)
                throw new BadRequestException("Username already occupied");

            var isValidRole = Enum.TryParse<UserRoles>(
                dto.Role,
                true,
                out var parsedRole
            ) && Enum.IsDefined(typeof(UserRoles), parsedRole);

            if (!isValidRole)
                throw new BadRequestException("Invalid role");

            if (parsedRole == UserRoles.Admin)
                throw new BadRequestException("Cannot grant Admin role");

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
                FullName = dto.Name.Trim(),
                Email = dto.Email.Trim().ToLower(),
                Designation = dto.Designation.Trim(),
                IsActive = true,
                UserId = user.Id
            };

            _repository.Add(employee);
            _repository.SaveChanges();

            return new EmployeeResponseDTO
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Email = employee.Email,
                Designation = employee.Designation,
                IsActive = employee.IsActive,
                Skills = new List<EmployeeSkillResponseDTO>()
            };
        }
        public EmployeeResponseDTO Update(EmployeeUpdateDTO dto)
        {
            if(dto == null) 
                throw new BadRequestException("Enter the Data required data");

            var employee = _repository.GetById(dto.EmployeeId);
            if (employee == null)
                throw new NotFoundException("Employee does not exist");

            employee.FullName = dto.FullName.Trim();
            employee.Designation = dto.Designation.Trim();

            _repository.Update(employee);
            _repository.SaveChanges();

            return MapToResponse(employee);
        }
        public bool Deactivate(int? id)
        {
            if (id == null) throw new BadRequestException("Enter the required data");

            var employee = _repository.GetById(id.Value);
            if (employee == null)
                throw new NotFoundException("Employee does not exists");
            if (!employee.IsActive)
                throw new BadRequestException("Employee is Deactivated Already");

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

        private EmployeeResponseDTO MapToResponse(Employee employee)
        {
            return new EmployeeResponseDTO
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Email = employee.Email,
                Designation = employee.Designation,
                IsActive = employee.IsActive,
                Skills = employee.EmployeeSkills?
                    .Select(es => new EmployeeSkillResponseDTO
                    {
                        SkillId = es.SkillId,
                        SkillName = es.Skill.Name,
                        Level = es.Level.ToString()
                    })
                    .ToList() ?? new List<EmployeeSkillResponseDTO>()
            };
        }

        public List<EmployeeResponseDTO> GetInactiveEmployees()
        {
            var employees = _repository.GetInactiveEmployees();

            return employees
                .Select(MapToResponse)
                .ToList();
        }

        public EmployeeResponseDTO ActivateEmployee(int? id)
        {
            if (id == null) throw new BadRequestException("Enter th required Detail");

            var employee = _repository.GetById(id.Value);
            if (employee == null)
                throw new NotFoundException("Employee does not exist");

            if (employee.IsActive)
                throw new BadRequestException("Employee is already active");

            employee.IsActive = true;

            if (employee.User != null)
                employee.User.IsActive = true;

            _repository.Update(employee);
            _repository.SaveChanges();

            return MapToResponse(employee);
        }
    }
}