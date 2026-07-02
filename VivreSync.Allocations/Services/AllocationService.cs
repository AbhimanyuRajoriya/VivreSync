using VivreSync.Allocations.DTOs;
using VivreSync.Allocations.Repositories;
using VivreSync.HR.Repositories;
using VivreSync.Model.Entities;
using VivreSync.Model.Enums;
using VivreSync.Projects.Repositories;
using VivreSync.Shared.Exceptions;
namespace VivreSync.Allocations.Services;
public class AllocationService : IAllocationService
{
    private readonly IAllocationRepository _allocationRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IProjectRepository _projectRepository;

    public AllocationService(IAllocationRepository allocationRepository,IEmployeeRepository employeeRepository, IProjectRepository projectRepository)
    {
        _allocationRepository = allocationRepository;
        _employeeRepository = employeeRepository;
        _projectRepository = projectRepository;
    }

    public List<AllocationResponseDTO> GetAll(int userId, string role)
    {
        if (role == "Manager")
        {
            var managerEmployee = GetManagerEmployee(userId);
            var allocations = _allocationRepository.GetAll()
                .Where(a => a.Employee.ManagerId == managerEmployee.Id && a.Project.ManagerId == managerEmployee.Id && a.Employee.IsActive).ToList();

            return allocations.Select(MapToResponse).ToList();
        }

        if (role == "Employee")
        {
            var employee = GetEmployeeByUserId(userId);
            var allocations = _allocationRepository.GetAll()
                .Where(a => a.EmployeeId == employee.Id)
                .ToList();

            return allocations.Select(MapToResponse).ToList();
        }

        throw new BadRequestException("Access denied");
    }

    public AllocationResponseDTO GetById(int id, int userId, string role)
    {
        var allocation = _allocationRepository.GetById(id);
        if (allocation == null)
            throw new NotFoundException("Allocation not found");

        if (role == "Manager")
            ValidateManagerAccessAllocation(userId, allocation);
        else if (role == "Employee")
            ValidateEmployeeAccessAllocation(userId, allocation);
        else
            throw new BadRequestException("Access denied");

        return MapToResponse(allocation);
    }

    public AllocationResponseDTO Create(AllocationCreateDTO dto, int userId)
    {
        ValidateManagerAccessEmployee(userId, dto.EmployeeId);
        ValidateManagerAccessProject(userId, dto.ProjectId);

        if (dto.StartDate == null)
            throw new BadRequestException("Start date is required");

        if (dto.EndDate == null)
            throw new BadRequestException("End date is required");

        var today = DateOnly.FromDateTime(DateTime.Today);

        if (dto.StartDate.Value < today)
            throw new BadRequestException("Allocation start date cannot be before today");

        if (dto.StartDate.Value > dto.EndDate.Value)
            throw new BadRequestException("End date should be after the start date");

        if (dto.UtilizationPercentage <= 0 || dto.UtilizationPercentage > 100)
            throw new BadRequestException("Enter valid utilization percentage");

        var employee = _employeeRepository.GetById(dto.EmployeeId);
        var project = _projectRepository.GetById(dto.ProjectId);

        if (employee == null || project == null)
            throw new BadRequestException("Enter valid employee and project ID");

        if (!employee.IsActive || employee.User == null || !employee.User.IsActive)
            throw new BadRequestException("Employee is not active");

        if (project.Status == ProjectStatus.Completed)
            throw new BadRequestException("Project is completed");

        if (project.Status == ProjectStatus.OnHold)
            throw new BadRequestException("Project is currently on hold");

        var overlappingAllocations = _allocationRepository.GetOverlappingAllocations(dto.EmployeeId, dto.StartDate.Value, dto.EndDate.Value);
        var canAllocate = CanAllocate(overlappingAllocations, dto.StartDate.Value, dto.EndDate.Value, dto.UtilizationPercentage);

        if (!canAllocate)
            throw new BadRequestException("Employee cannot be allocated more than 100% on overlapping dates");

        var allocation = new Allocation
        {
            EmployeeId = dto.EmployeeId,
            ProjectId = dto.ProjectId,
            UtilizationPercentage = dto.UtilizationPercentage,
            StartDate = dto.StartDate.Value,
            EndDate = dto.EndDate.Value
        };

        _allocationRepository.Add(allocation);

        return new AllocationResponseDTO
        {
            Id = allocation.Id,
            EmployeeId = allocation.EmployeeId,
            EmployeeName = employee.FullName,
            ProjectId = allocation.ProjectId,
            ProjectName = project.Name,
            UtilizationPercentage = allocation.UtilizationPercentage,
            StartDate = allocation.StartDate,
            EndDate = allocation.EndDate
        };
    }

    public bool EndAllocation(int id, int userId)
    {
        var allocation = _allocationRepository.GetById(id);
        if (allocation == null)
            throw new NotFoundException("Allocation not found");

        ValidateManagerAccessAllocation(userId, allocation);

        var today = DateOnly.FromDateTime(DateTime.Today);
        if (allocation.EndDate < today)
            throw new BadRequestException("Allocation already ended");

        if (allocation.StartDate > today)
        {
            _allocationRepository.Delete(allocation);
            return true;
        }
        allocation.EndDate = today;

        _allocationRepository.Update(allocation);

        return true;
    }

    public bool Update(int id, AllocationUpdateDTO dto, int userId)
    {
        var allocation = _allocationRepository.GetById(id);
        if (allocation == null)
            throw new NotFoundException("Allocation not found");

        ValidateManagerAccessAllocation(userId, allocation);

        if (dto.StartDate == null)
            throw new BadRequestException("Start date is required");
        if (dto.EndDate == null)
            throw new BadRequestException("End date is required");

        if (dto.UtilizationPercentage == null)
            throw new BadRequestException("Utilization percentage is required");

        var today = DateOnly.FromDateTime(DateTime.Today);
        if (allocation.EndDate < today)
            throw new BadRequestException("Past allocation cannot be updated");
        if (dto.StartDate.Value > dto.EndDate.Value)
            throw new BadRequestException("Invalid date range");

        if (dto.UtilizationPercentage <= 0 || dto.UtilizationPercentage > 100)
            throw new BadRequestException("Invalid utilization percentage");

        if (!allocation.Employee.IsActive || allocation.Employee.User == null || !allocation.Employee.User.IsActive)
            throw new BadRequestException("Employee is not active");

        if (allocation.Project.Status == ProjectStatus.Completed)
            throw new BadRequestException("Project is completed");
        if (allocation.Project.Status == ProjectStatus.OnHold)
            throw new BadRequestException("Project is currently on hold");

        if (allocation.StartDate <= today)
        {
            if (dto.StartDate.Value != allocation.StartDate)
                throw new BadRequestException("Start date of an ongoing allocation cannot be changed");

            if (dto.EndDate.Value < today)
                throw new BadRequestException("End date cannot be before today");
        }
        else
        {
            if (dto.StartDate.Value < today)
                throw new BadRequestException("Allocation start date cannot be before today");
        }

        var overlappingAllocations = _allocationRepository.GetOverlappingAllocations(
            allocation.EmployeeId,
            dto.StartDate.Value,
            dto.EndDate.Value);

        var existingAllocations = overlappingAllocations.Where(a => a.Id != id).ToList();

        var canAllocate = CanAllocate(existingAllocations,dto.StartDate.Value, dto.EndDate.Value, dto.UtilizationPercentage);
        if (!canAllocate)
            throw new BadRequestException("Employee cannot be allocated more than 100% on overlapping dates");

        allocation.StartDate = dto.StartDate.Value;
        allocation.EndDate = dto.EndDate.Value;
        allocation.UtilizationPercentage = dto.UtilizationPercentage;

        _allocationRepository.Update(allocation);

        return true;
    }

    public List<EmployeeAllocationDTO>? GetFreeEmployee(int userId)
    {
        var employees = GetEmployeeTable(userId);
        return employees?.Where(e => e.AvailableCapacity > 0).ToList();
    }
    public List<EmployeeAllocationDTO>? GetOccupiedEmployee(int userId)
    {
        var employees = GetEmployeeTable(userId);
        return employees?.Where(e => e.TotalAllocation == 100).ToList();
    }
    public List<EmployeeAllocationDTO>? GetEmployeeTable(int userId)
    {
        var managerEmployee = _employeeRepository.GetByUserId(userId);
        if (managerEmployee == null)
            throw new BadRequestException("Manager employee profile not found");

        var employees = _employeeRepository.GetAll()
            .Where(e =>
                e.IsActive && e.ManagerId == managerEmployee.Id).ToList();

        var allocations = _allocationRepository.GetAll();
        var today = DateOnly.FromDateTime(DateTime.Today);
        var result = employees.Select(e =>
        {
            var activeAllocations = allocations
                .Where(a =>
                    a.EmployeeId == e.Id &&
                    a.StartDate <= today &&
                    a.EndDate >= today).ToList();

            var totalAllocation = activeAllocations.Sum(a => a.UtilizationPercentage);
            return new EmployeeAllocationDTO
            {
                EmployeeId = e.Id,
                EmployeeName = e.FullName,
                TotalAllocation = totalAllocation,
                AvailableCapacity = 100 - totalAllocation,
                Projects = activeAllocations.Select(a => a.Project.Name).ToList()
            };
        }).ToList();
        return result;
    }

    private Employee GetManagerEmployee(int userId)
    {
        var managerEmployee = _employeeRepository.GetByUserId(userId);
        if (managerEmployee == null)
            throw new BadRequestException("Manager employee profile not found");

        return managerEmployee;
    }

    private Employee GetEmployeeByUserId(int userId)
    {
        var employee = _employeeRepository.GetByUserId(userId);
        if (employee == null)
            throw new BadRequestException("Employee profile not found");

        return employee;
    }

    private void ValidateManagerAccessEmployee(int userId, int employeeId)
    {
        var managerEmployee = GetManagerEmployee(userId);
        var employee = _employeeRepository.GetById(employeeId);
        if (employee == null)
            throw new NotFoundException("Employee does not exist");

        if (!employee.IsActive || employee.User == null || !employee.User.IsActive)
            throw new BadRequestException("Employee is not active");

        if (employee.ManagerId != managerEmployee.Id)
            throw new BadRequestException("Cannot access this employee");
    }

    private void ValidateManagerAccessProject(int userId, int projectId)
    {
        var managerEmployee = GetManagerEmployee(userId);
        var project = _projectRepository.GetById(projectId);
        if (project == null)
            throw new NotFoundException("Project does not exist");

        if (project.ManagerId != managerEmployee.Id)
            throw new BadRequestException("Cannot access this project");
    }

    private void ValidateManagerAccessAllocation(int userId, Allocation allocation)
    {
        var managerEmployee = GetManagerEmployee(userId);
        if (allocation.Employee.ManagerId != managerEmployee.Id)
            throw new BadRequestException("Cannot access this allocation employee");

        if (allocation.Project.ManagerId != managerEmployee.Id)
            throw new BadRequestException("Cannot access this allocation project");
    }

    private void ValidateEmployeeAccessAllocation(int userId, Allocation allocation)
    {
        var employee = GetEmployeeByUserId(userId);
        if (allocation.EmployeeId != employee.Id)
            throw new BadRequestException("Cannot check the allocation of other user");
    }

    private AllocationResponseDTO MapToResponse(Allocation allocation)
    {
        return new AllocationResponseDTO
        {
            Id = allocation.Id,
            EmployeeId = allocation.EmployeeId,
            EmployeeName = allocation.Employee.FullName,
            ProjectId = allocation.ProjectId,
            ProjectName = allocation.Project.Name,
            UtilizationPercentage = allocation.UtilizationPercentage,
            StartDate = allocation.StartDate,
            EndDate = allocation.EndDate
        };
    }

    private bool CanAllocate(
    List<Allocation> existingAllocations, DateOnly newStartDate, DateOnly newEndDate, int newUtilization)
    {
        for (var date = newStartDate; date <= newEndDate; date = date.AddDays(1))
        {
            var allocationOnThisDay = existingAllocations.Where(a => a.StartDate <= date && a.EndDate >= date).Sum(a => a.UtilizationPercentage);
            if (allocationOnThisDay + newUtilization > 100)
                return false;
        }
        return true;
    }
}