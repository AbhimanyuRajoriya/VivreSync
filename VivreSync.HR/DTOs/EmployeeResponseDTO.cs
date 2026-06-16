namespace VivreSync.HR.DTOs;
public class EmployeeResponseDTO
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<EmployeeSkillResponseDTO> Skills { get; set; } = new();
}
