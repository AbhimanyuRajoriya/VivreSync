using Microsoft.EntityFrameworkCore;
using VivreSync.Model.Entities;
using VivreSync.Model.Enums;
using Microsoft.AspNetCore.Identity;
namespace VivreSync.Structure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Users> Users => Set<Users>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Skill> Skills => Set<Skill>();
        public DbSet<EmployeeSkill> EmployeeSkills => Set<EmployeeSkill>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<Milestone> Milestones => Set<Milestone>();
        public DbSet<Allocation> Allocations => Set<Allocation>();
        public DbSet<Timesheet> Timesheets => Set<Timesheet>();
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EmployeeSkill>()
                .HasOne(es => es.Employee)
                .WithMany(e => e.EmployeeSkills)
                .HasForeignKey(es => es.EmployeeId);

            modelBuilder.Entity<EmployeeSkill>()
                .HasOne(es => es.Skill)
                .WithMany(s => s.EmployeeSkills)
                .HasForeignKey(es => es.SkillId);
                
            modelBuilder.Entity<Allocation>()
                .HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Allocation>()
                .HasOne(a => a.Project)
                .WithMany()
                .HasForeignKey(a => a.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Timesheet>()
                .HasOne(t => t.Employee)
                .WithMany()
                .HasForeignKey(t => t.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Timesheet>()
                .HasOne(t => t.Project)
                .WithMany()
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Manager)
                .WithMany()
                .HasForeignKey(p => p.ManagerId)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<Milestone>()
                .HasOne(m => m.Project)
                .WithMany(p => p.Milestones)
                .HasForeignKey(m => m.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            var admin = new Users
            {
                Id = 1,
                UserName = "admin",
                Role = UserRoles.Admin,
                PasswordChangeRequired = true,
                IsActive = true
            };

            var hasher = new PasswordHasher<Users>();
            admin.PasswordHash = hasher.HashPassword(admin, "Password123");

            modelBuilder.Entity<Users>().HasData(admin);
        }
    }

}
