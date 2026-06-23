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

    public AllocationResponseDTO? Create(AllocationCreateDTO dto)
    {
        var employee = _employeeRepository.GetById(dto.EmployeeId);
        var project = _projectRepository.GetById(dto.ProjectId);
        if (employee == null || project == null)
            throw new BadRequestException("Enter vaid employee and project ID");

        if (dto.UtilizationPercentage <= 0 || dto.UtilizationPercentage > 100)
            throw new BadRequestException("Enter valid Valid Utilizaition Percentage");

        if (dto.StartDate > dto.EndDate)
            throw new BadRequestException("Enter valid Date Range");

        var overlappingAllocations = _allocationRepository.GetOverlappingAllocations( dto.EmployeeId, dto.StartDate,dto.EndDate);

        var currentAllocation = overlappingAllocations.Sum(a => a.UtilizationPercentage);
        if (currentAllocation + dto.UtilizationPercentage > 100)
            throw new BadRequestException("Cannot Allocate current Utilization");

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
            throw new NotFoundException("Allocation Not Found");

        var employee = _employeeRepository.GetById(dto.EmployeeId);
        var project = _projectRepository.GetById(dto.ProjectId);
        if (employee == null || project == null)
            throw new BadRequestException("Enter Valid Employee and Prject ID");

        if (dto.UtilizationPercentage <= 0 || dto.UtilizationPercentage > 100)
            throw new BadRequestException("Invalid Utilization Percentage");

        if (dto.StartDate > dto.EndDate)
            throw new BadRequestException("Invalid Date Range");

        var overlappingAllocations = _allocationRepository.GetOverlappingAllocations(
            dto.EmployeeId,
            dto.StartDate,
            dto.EndDate);

        var currentAllocation = overlappingAllocations
            .Where(a => a.Id != id)
            .Sum(a => a.UtilizationPercentage);

        if (currentAllocation + dto.UtilizationPercentage > 100)
            throw new BadRequestException("Cannot Allocate requested utilization");

        allocation.EmployeeId = dto.EmployeeId;
        allocation.ProjectId = dto.ProjectId;
        allocation.UtilizationPercentage = dto.UtilizationPercentage;
        allocation.StartDate = dto.StartDate;
        allocation.EndDate = dto.EndDate;

        _allocationRepository.Update(allocation);

        return true;
    }
    public List<EmployeeAllocationDTO>? GetFreeEmployee()
    {
        return GetEmployeeTable()?.Where(e => e.AvailableCapacity > 0).ToList(); ;
    }
    public List<EmployeeAllocationDTO>? GetOccupiedEmployee()
    {
        return GetEmployeeTable()?.Where(e => e.TotalAllocation == 100).ToList();
    }
    public List<EmployeeAllocationDTO>? GetEmployeeTable()
    {
        var employee = _employeeRepository.GetAll();
        if (employee == null)
            throw new BadRequestException("Employee cannot be loaded");

        var allocation = _allocationRepository.GetAll();
        if (allocation == null)
            throw new BadRequestException("Allocations cannot be loaded");

        var result = employee.Where(e=> e.IsActive && e.User.Role == UserRoles.Employee && e.User.IsActive).Select(e =>
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var active = allocation
                .Where(a =>a.EmployeeId == e.Id &&
                    a.StartDate <= today && a.EndDate >= today).ToList();
                    
            var totalallocation = active.Sum(a => a.UtilizationPercentage);
            return new EmployeeAllocationDTO
            {
                EmployeeId = e.Id,
                EmployeeName = e.FullName,
                TotalAllocation = totalallocation,
                AvailableCapacity = 100 - totalallocation,
                Projects = active.Select(a => $"{a.Project.Name} - {a.UtilizationPercentage}%").ToList()
            };
        }).ToList();

        return result;
    }
}