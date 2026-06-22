using VivreSync.Structure.Data;
using VivreSync.Model.Entities;
using Microsoft.EntityFrameworkCore;
namespace VivreSync.HR.Repositories
{
    public class SkillRepository: ISkillRepository
    {
        private readonly AppDbContext _context;
        public SkillRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Skill> GetAll()
        {
            return _context.Skills.ToList();
        }
        public Skill? GetById(int id)
        {   
            return _context.Skills.FirstOrDefault(s => s.Id == id);
        }
        public Skill? ExistingSkill(string name)
        {
            return _context.Skills.FirstOrDefault(s => s.Name == name);
        }
        public void AddSkill(Skill skill)
        {
            _context.Skills.Add(skill);
            SaveSkill();
        }
        public void SaveSkill()
        {
            _context.SaveChanges();
        }
        public EmployeeSkill? GetEmployeeSkill(int employeeId, int skillId)
        {
            return _context.EmployeeSkills
                .FirstOrDefault(x => x.EmployeeId == employeeId && x.SkillId == skillId);
        }

        public void AssignSkill(EmployeeSkill employeeSkill)
        {
            _context.EmployeeSkills.Add(employeeSkill);
        }
    }
}
