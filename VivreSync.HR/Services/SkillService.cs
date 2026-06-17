using VivreSync.HR.Repositories;
using VivreSync.Model.Entities;
using VivreSync.Model.Enums;
using VivreSync.HR.DTOs;
namespace VivreSync.HR.Services
{
    public class SkillService : ISkillService
    {
        private readonly ISkillRepository _skillRepository;
        private readonly IEmployeeRepository _employeeRepository;
        public SkillService(ISkillRepository repository, IEmployeeRepository employeeRepository)
        {
            _skillRepository = repository;
            _employeeRepository = employeeRepository;
        }

        public List<SkillResponseDTO> GetAllSkills()
        {
            var skills = _skillRepository.GetAll();

            return skills.Select(s => new SkillResponseDTO
            {
                Id = s.Id,
                Name = s.Name
            }).ToList();
        }
        public Skill CreateSkill(SkillCreateDTO dto)
        {
            var skill = new Skill
            {
                Name = dto.Name
            };
            _skillRepository.AddSkill(skill);
            return skill;
        }       
        public bool AssignSkillToEmployee(SkillAssignDTO dto)
        {
            var employee = _employeeRepository.GetById(dto.EmployeeId);
            var skill = _skillRepository.GetById(dto.SkillId);

            if (employee == null || skill == null)
                return false;

            if (!Enum.TryParse(dto.Level, true, out Levels level))
                return false;

            var employeeSkill = _skillRepository.GetEmployeeSkill(dto.EmployeeId, dto.SkillId);

            if (employeeSkill == null)
            {
                employeeSkill = new EmployeeSkill
                {
                    EmployeeId = dto.EmployeeId,
                    SkillId = dto.SkillId,
                    Level = level
                };

                _skillRepository.AssignSkill(employeeSkill);
            }
            else
                employeeSkill.Level = level;

            _skillRepository.SaveSkill();

            return true;
        }
    }
}
