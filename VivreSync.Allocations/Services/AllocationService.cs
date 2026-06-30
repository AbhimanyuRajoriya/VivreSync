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

    public List<AllocationResponseDTO> GetAll()
    {
        var allocations = _allocationRepository.GetAll();

        return allocations.Select(a => new AllocationResponseDTO
        {
            Id = a.Id,
            EmployeeId = a.EmployeeId,
            EmployeeName = a.Employee.FullName,
            ProjectId = a.ProjectId,
            ProjectName = a.Project.Name,
            UtilizationPercentage = a.UtilizationPercentage,
            StartDate = a.StartDate,
            EndDate = a.EndDate
        }).ToList();
    }

    public AllocationResponseDTO? GetById(int id)
    {
        var allocation = _allocationRepository.GetById(id);

        if (allocation == null)
            throw new NotFoundException("Allocation Not Found");

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

    public AllocationResponseDTO Create(AllocationCreateDTO dto)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        if (dto.StartDate < today)
            throw new BadRequestException("Allocation start date cannot be before today");

        if (dto.StartDate > dto.EndDate)
            throw new BadRequestException("Enter valid date range");

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

        var overlappingAllocations = _allocationRepository.GetOverlappingAllocations(dto.EmployeeId, dto.StartDate,dto.EndDate);
        var canAllocate = CanAllocate(overlappingAllocations, dto.StartDate, dto.EndDate, dto.UtilizationPercentage);
        if (!canAllocate)
            throw new BadRequestException("Employee cannot be allocated more than 100% on overlapping dates");

        var allocation = new Allocation
        {
            EmployeeId = dto.EmployeeId,
            ProjectId = dto.ProjectId,
            UtilizationPercentage = dto.UtilizationPercentage,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
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

    public bool Delete(int id)
    {
        var allocation = _allocationRepository.GetById(id);

        if (allocation == null)
            throw new NotFoundException("Allcation Not Found");

        _allocationRepository.Delete(allocation);

        return true;
    }

    public bool Update(int id, AllocationUpdateDTO dto)
    {
        var allocation = _allocationRepository.GetById(id);

        if (allocation == null)
            throw new NotFoundException("Allocation not found");

        var today = DateOnly.FromDateTime(DateTime.Today);

        if (allocation.StartDate < today)
            throw new BadRequestException("Past allocation cannot be updated");

        if (dto.StartDate < today)
            throw new BadRequestException("Allocation start date cannot be before today");

        if (dto.StartDate > dto.EndDate)
            throw new BadRequestException("Invalid date range");

        if (dto.UtilizationPercentage <= 0 || dto.UtilizationPercentage > 100)
            throw new BadRequestException("Invalid utilization percentage");

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

        var overlappingAllocations = _allocationRepository.GetOverlappingAllocations(dto.EmployeeId, dto.StartDate, dto.EndDate);

        var existingAllocations = overlappingAllocations.Where(a => a.Id != id).ToList();

        var canAllocate = CanAllocate(existingAllocations, dto.StartDate, dto.EndDate, dto.UtilizationPercentage);
        if (!canAllocate)
            throw new BadRequestException("Employee cannot be allocated more than 100% on overlapping dates");

        allocation.EmployeeId = dto.EmployeeId;
        allocation.ProjectId = dto.ProjectId;
        allocation.UtilizationPercentage = dto.UtilizationPercentage;
        allocation.StartDate = dto.StartDate;
        allocation.EndDate = dto.EndDate;

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
                e.IsActive &&
                e.ManagerId == managerEmployee.Id).ToList();

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
    public bool CanManagerAccessProject(int userId, int projectId)
    {
        var managerEmployee = _employeeRepository.GetByUserId(userId);
        if (managerEmployee == null)
            return false;

        return _projectRepository.IsProjectManagedBy(projectId, managerEmployee.Id);
    }
    public bool CanManagerAccessAllocation(int userId, int allocationId)
    {
        var managerEmployee = _employeeRepository.GetByUserId(userId);
        if (managerEmployee == null)
            return false;

        var allocation = _allocationRepository.GetById(allocationId);
        if (allocation == null)
            return false;

        var ownsEmployee = _employeeRepository.IsEmployeeUnderManager(allocation.EmployeeId, managerEmployee.Id);
        var ownsProject = _projectRepository.IsProjectManagedBy(allocation.ProjectId, managerEmployee.Id);
        return ownsEmployee && ownsProject;
    }

    public bool CanManagerAccessEmployee(int userId, int employeeId)
    {
        var managerEmployee = _employeeRepository.GetByUserId(userId);
        if (managerEmployee == null)
            return false;

        return _employeeRepository.IsEmployeeUnderManager(employeeId, managerEmployee.Id);
    }

    public List<AllocationResponseDTO> GetAllocationsForManager(int userId)
    {
        var managerEmployee = _employeeRepository.GetByUserId(userId);
        if (managerEmployee == null)
            throw new BadRequestException("Manager employee profile not found");

        var allocations = _allocationRepository.GetAll()
            .Where(a =>
                a.Employee.ManagerId == managerEmployee.Id &&
                a.Project.ManagerId == managerEmployee.Id &&
                a.Employee.IsActive).ToList();

        return allocations.Select(MapToResponse).ToList();
    }

    public List<AllocationResponseDTO> GetAllocationsForEmployee(int userId)
    {
        var employee = _employeeRepository.GetByUserId(userId);
        if (employee == null)
            throw new BadRequestException("Employee profile not found");

        var allocations = _allocationRepository.GetAll()
            .Where(a => a.EmployeeId == employee.Id).ToList();

        return allocations.Select(MapToResponse).ToList();
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
}