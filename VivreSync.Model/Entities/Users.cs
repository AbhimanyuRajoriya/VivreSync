using VivreSync.Model.Enums;
namespace VivreSync.Model.Entities
{
    public class Users
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRoles Role { get; set; }
        public bool PasswordChangeRequired { get; set; }
        public bool IsActive { get; set; }
        public Employee? Employee { get; set; }
    }
}
