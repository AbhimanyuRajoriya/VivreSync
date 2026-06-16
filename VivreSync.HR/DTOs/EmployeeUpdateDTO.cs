using System.ComponentModel.DataAnnotations;

namespace VivreSync.HR.DTOs
{
    public class EmployeeUpdateDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Id")]
        public int Id { get; set; }

        public bool IsActive { get; set; } = false;
    }
}
