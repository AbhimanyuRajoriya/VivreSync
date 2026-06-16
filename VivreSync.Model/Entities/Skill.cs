namespace VivreSync.Model.Entities
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<EmployeeSkill> EmployeeSkills { get; set; } = new();
    }
}
