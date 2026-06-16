namespace VivreSync.HR.DTOs
{
    public class SkillAssignDTO
    {
        public int EmployeeId { get; set; }
        public int SkillId { get; set; }
        public string Level { get; set; } = string.Empty;
    }
}
