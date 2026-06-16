using VivreSync.HR.DTOs;
using VivreSync.Model.Entities;

namespace VivreSync.HR.Services
{
    public interface ISkillService
    {
        List<SkillResponseDTO> GetAllSkills();
        void CreateSkill(SkillCreateDTO dto);
        bool AssignSkillToEmployee(SkillAssignDTO dto);
    }
}
