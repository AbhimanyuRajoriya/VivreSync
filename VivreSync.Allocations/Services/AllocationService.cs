using VivreSync.Allocations.Repositories;
using VivreSync.Allocations.DTOs;
using VivreSync.Model.Entities;
using VivreSync.HR.Repositories;
using VivreSync.Projects.Repositories;
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
            return null;

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
            return null;

        if (dto.UtilizationPercentage <= 0 || dto.UtilizationPercentage > 100)
            return null;

        if (dto.StartDate > dto.EndDate)
            return null;

        var overlappingAllocations = _allocationRepository.GetOverlappingAllocations( dto.EmployeeId, dto.StartDate,dto.EndDate);

        var currentAllocation = overlappingAllocations.Sum(a => a.UtilizationPercentage);
        if (currentAllocation + dto.UtilizationPercentage > 100)
            return null;

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
            return false;

        _allocationRepository.Delete(allocation);

        return true;
    }

    public bool Update(int id, AllocationUpdateDTO dto)
    {
        var allocation = _allocationRepository.GetById(id);

        if (allocation == null)
            return false;

        var employee = _employeeRepository.GetById(dto.EmployeeId);
        var project = _projectRepository.GetById(dto.ProjectId);
        if (employee == null || project == null)
            return false;

        if (dto.UtilizationPercentage <= 0 || dto.UtilizationPercentage > 100)
            return false;

        if (dto.StartDate > dto.EndDate)
            return false;

        var overlappingAllocations = _allocationRepository.GetOverlappingAllocations(
            dto.EmployeeId,
            dto.StartDate,
            dto.EndDate);

        var currentAllocation = overlappingAllocations
            .Where(a => a.Id != id)
            .Sum(a => a.UtilizationPercentage);

        if (currentAllocation + dto.UtilizationPercentage > 100)
            return false;

        allocation.EmployeeId = dto.EmployeeId;
        allocation.ProjectId = dto.ProjectId;
        allocation.UtilizationPercentage = dto.UtilizationPercentage;
        allocation.StartDate = dto.StartDate;
        allocation.EndDate = dto.EndDate;

        _allocationRepository.Update(allocation);

        return true;
    }
}