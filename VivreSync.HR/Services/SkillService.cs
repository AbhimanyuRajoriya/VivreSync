using VivreSync.HR.Repositories;
using VivreSync.Model.Entities;
using VivreSync.Model.Enums;
using VivreSync.HR.DTOs;
using VivreSync.Shared.Exceptions;
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
        public Skill? CreateSkill(SkillCreateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new BadRequestException("Enter Proper Skill");
            
            var skillName = dto.Name.Trim().ToLower();

            if (_skillRepository.ExistingSkill(skillName) != null)
                throw new BadRequestException("Skill Already Exists");

            var skill = new Skill
            {
                Name = skillName
            };
            
            _skillRepository.AddSkill(skill);
            return skill;
        }       
        public bool AssignSkillToEmployee(SkillAssignDTO dto)
        {
            var employee = _employeeRepository.GetById(dto.EmployeeId);
            var skill = _skillRepository.GetById(dto.SkillId);

            if (employee == null || !employee.IsActive)
                throw new BadRequestException("Enter valid Employee ID");

            if (skill == null)
                throw new BadRequestException("Enter Valid Skill ID");

            if (!Enum.TryParse(dto.Level, true, out Levels level) || !Enum.IsDefined(typeof(Levels), level))
                throw new BadRequestException("Enter Valid Level of Skill");

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
