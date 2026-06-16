using VivreSync.Model.Entities;

namespace VivreSync.HR.Repositories
{
    public interface ISkillRepository
    {
        List<Skill> GetAll();
        Skill? GetById(int id);
        void AddSkill(Skill skill);
        void SaveSkill();
        EmployeeSkill? GetEmployeeSkill(int employeeId, int skillId);
        void AssignSkill(EmployeeSkill employeeSkill);
    }
}
