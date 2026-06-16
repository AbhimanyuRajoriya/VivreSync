using VivreSync.Model.Enums;

namespace VivreSync.Model.Entities
{
    public class EmployeeSkill
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public int SkillId { get; set; }
        public Skill Skill { get; set; } = null!;
        public Levels Level { get; set; }
    }
}
