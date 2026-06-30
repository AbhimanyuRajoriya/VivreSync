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

            return employees
                .Where(e => e.IsActive)
                .Select(MapToResponse)
                .ToList();
        }

        public EmployeeResponseDTO? GetById(int id)
        {
            var employee = _repository.GetById(id);

            if (employee == null)
                throw new NotFoundException("Employee not found");

            if (!employee.IsActive)
                throw new NotFoundException("Employee not active");

            return MapToResponse(employee);
        }
        public EmployeeResponseDTO Create(EmployeeCreateDTO dto)
        {
            var username = dto.UserName.Trim().ToLower();
            var existingUser = _userRepository.GetUser(username);
            if (existingUser != null)
                throw new BadRequestException("Username already occupied");

            var email = dto.Email.Trim().ToLower();
            if (_repository.EmailExists(email))
                throw new BadRequestException("Email already occupied");

            var isValidRole = Enum.TryParse<UserRoles>(
                dto.Role,
                true,
                out var parsedRole
            ) && Enum.IsDefined(typeof(UserRoles), parsedRole);

            if (!isValidRole)
                throw new BadRequestException("Invalid role");

            if (parsedRole == UserRoles.Admin)
                throw new BadRequestException("Cannot grant Admin role");

            var designation = ParseDesignation(dto.Designation);

            ValidateManager(dto.ManagerId, parsedRole);

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
                Designation = designation,
                IsActive = true,
                ManagerId = dto.ManagerId,
                UserId = user.Id
            };

            _repository.Add(employee);
            _repository.SaveChanges();

            return new EmployeeResponseDTO
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Email = employee.Email,
                Designation = employee.Designation.ToString(),
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

            var designation = ParseDesignation(dto.Designation);

            employee.FullName = dto.FullName.Trim();
            employee.Designation = designation;

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

        public List<EmployeeResponseDTO> GetEmployeesUnderManager(int managerEmployeeId)
        {
            var employees = _repository.GetEmployeesUnderManager(managerEmployeeId);
            return employees
                .Select(MapToResponse)
                .ToList();
        }

        public int GetEmployeeIdByUserId(int userId)
        {
            var employee = _repository.GetByUserId(userId);
            if (employee == null)
                throw new BadRequestException("Employee profile not found for current user");

            return employee.Id;
        }

        public bool IsEmployeeUnderManager(int employeeId, int managerEmployeeId)
        {
            return _repository.IsEmployeeUnderManager(employeeId, managerEmployeeId);
        }

        private EmployeeResponseDTO MapToResponse(Employee employee)
        {
            return new EmployeeResponseDTO
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Email = employee.Email,
                Designation = employee.Designation.ToString(),
                IsActive = employee.IsActive,
                ManagerId = employee.ManagerId,
                ManagerName = employee.Manager != null ? employee.Manager.FullName : null,
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

        private Designation ParseDesignation(string designation)
        {
            if (string.IsNullOrWhiteSpace(designation))
                throw new BadRequestException("Designation is required");

            var isValidDesignation = Enum.TryParse<Designation>(designation, true, out var parsedDesignation) && Enum.IsDefined(typeof(Designation), parsedDesignation);
            if (!isValidDesignation)
                throw new BadRequestException("Enter valid designation");

            return parsedDesignation;
        }
        private void ValidateManager(int? managerId, UserRoles role)
        {
            if (role == UserRoles.Admin)
            {
                if (managerId != null)
                    throw new BadRequestException("Admin should not have a manager");
                return;
            }

            if (role == UserRoles.Manager)
            {
                if (managerId != null)
                    throw new BadRequestException("Manager should not have a manager");
                return;
            }

            if (role == UserRoles.Employee && managerId == null)
                throw new BadRequestException("Manager is required for employee");

            var manager = _repository.GetById(managerId.Value);

            if (manager == null || !manager.IsActive)
                throw new BadRequestException("Enter valid active manager");

            if (manager.User == null || !manager.User.IsActive)
                throw new BadRequestException("Manager user account is inactive");

            if (manager.User.Role != UserRoles.Manager)
                throw new BadRequestException("Selected manager must have Manager role");
        }
    }
}