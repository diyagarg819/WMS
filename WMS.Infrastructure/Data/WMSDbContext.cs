using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;

namespace WMS.Infrastructure.Data
{
    /// <summary>
    /// The single database context for the WMS application.
    /// Lives only in the Infrastructure layer — no other project may reference this directly.
    /// </summary>
    public class WMSDbContext : DbContext
    {
        public WMSDbContext(DbContextOptions<WMSDbContext> options) : base(options)
        {
        }

        // One DbSet per entity — these map to database tables
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<EmployeeProjectAllocation> EmployeeProjectAllocations { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ──────────────────────────────────────────────
            // Employee configuration
            // ──────────────────────────────────────────────

            // Email must be unique across all employees
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            // Index on DepartmentId for faster lookups by department
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.DepartmentId);

            // Default value for Status column
            modelBuilder.Entity<Employee>()
                .Property(e => e.Status)
                .HasDefaultValue("Active");

            // Default value for CreatedOn column
            modelBuilder.Entity<Employee>()
                .Property(e => e.CreatedOn)
                .HasDefaultValueSql("GETDATE()");

            // Gender is stored as CHAR(1) in SQL Server
            modelBuilder.Entity<Employee>()
                .Property(e => e.Gender)
                .HasColumnType("char(1)");

            // ──────────────────────────────────────────────
            // Department configuration
            // ──────────────────────────────────────────────

            modelBuilder.Entity<Department>()
                .Property(d => d.CreatedOn)
                .HasDefaultValueSql("GETDATE()");

            // ──────────────────────────────────────────────
            // Attendance configuration
            // ──────────────────────────────────────────────

            // Index on EmpId for faster lookups by employee
            modelBuilder.Entity<Attendance>()
                .HasIndex(a => a.EmpId);

            // TotalHours is a computed column — calculated by SQL Server, not by the app
            modelBuilder.Entity<Attendance>()
                .Property(a => a.TotalHours)
                .HasComputedColumnSql(
                    "CAST((DATEDIFF(MINUTE, CheckIn, CheckOut)) / 60.0 AS FLOAT)",
                    stored: true);

            // AttendanceDate is stored as DATE (no time component)
            modelBuilder.Entity<Attendance>()
                .Property(a => a.AttendanceDate)
                .HasColumnType("date");

            // ──────────────────────────────────────────────
            // Leave configuration
            // ──────────────────────────────────────────────

            // Index on EmpId for faster lookups by employee
            modelBuilder.Entity<Leave>()
                .HasIndex(l => l.EmpId);

            // Default status for new leave requests
            modelBuilder.Entity<Leave>()
                .Property(l => l.Status)
                .HasDefaultValue("Pending");

            modelBuilder.Entity<Leave>()
                .Property(l => l.AppliedOn)
                .HasDefaultValueSql("GETDATE()");

            // FromDate and ToDate are stored as DATE (no time component)
            modelBuilder.Entity<Leave>()
                .Property(l => l.FromDate)
                .HasColumnType("date");

            modelBuilder.Entity<Leave>()
                .Property(l => l.ToDate)
                .HasColumnType("date");

            // ──────────────────────────────────────────────
            // Announcement configuration
            // ──────────────────────────────────────────────

            modelBuilder.Entity<Announcement>()
                .Property(a => a.CreatedOn)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Announcement>()
                .Property(a => a.IsActive)
                .HasDefaultValue(true);

            // Message column is VARCHAR(MAX) — mapped to NVARCHAR(MAX) by default in EF Core
            modelBuilder.Entity<Announcement>()
                .Property(a => a.Message)
                .HasColumnType("nvarchar(max)");

            // ──────────────────────────────────────────────
            // Project configuration
            // ──────────────────────────────────────────────

            modelBuilder.Entity<Project>()
                .Property(p => p.Status)
                .HasDefaultValue("Active");

            // StartDate and EndDate are stored as DATE
            modelBuilder.Entity<Project>()
                .Property(p => p.StartDate)
                .HasColumnType("date");

            modelBuilder.Entity<Project>()
                .Property(p => p.EndDate)
                .HasColumnType("date");

            // ──────────────────────────────────────────────
            // Client configuration
            // ──────────────────────────────────────────────

            // ClientAdress is VARCHAR(MAX) — spelling matches the PDF schema
            modelBuilder.Entity<Client>()
                .Property(c => c.ClientAdress)
                .HasColumnType("nvarchar(max)");

            modelBuilder.Entity<Client>()
                .Property(c => c.Status)
                .HasDefaultValue(true);

            // ──────────────────────────────────────────────
            // EmployeeProjectAllocation configuration
            // ──────────────────────────────────────────────

            modelBuilder.Entity<EmployeeProjectAllocation>()
                .Property(epa => epa.Status)
                .HasDefaultValue(true);

            // AssignedOn and CreateDate are stored as DATE
            modelBuilder.Entity<EmployeeProjectAllocation>()
                .Property(epa => epa.AssignedOn)
                .HasColumnType("date");

            modelBuilder.Entity<EmployeeProjectAllocation>()
                .Property(epa => epa.CreateDate)
                .HasColumnType("date");

            modelBuilder.Entity<EmployeeProjectAllocation>()
                .Property(epa => epa.UpdatedDate)
                .HasColumnType("date");

            // ──────────────────────────────────────────────
            // UserLogin configuration
            // ──────────────────────────────────────────────

            // Username must be unique
            modelBuilder.Entity<UserLogin>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // ──────────────────────────────────────────────
            // AuditLog configuration
            // ──────────────────────────────────────────────

            // EntityName is NVARCHAR(MAX) as per the PDF schema
            modelBuilder.Entity<AuditLog>()
                .Property(a => a.EntityName)
                .HasColumnType("nvarchar(max)");

            modelBuilder.Entity<AuditLog>()
                .Property(a => a.CreatedOn)
                .HasDefaultValueSql("GETDATE()");

            // ──────────────────────────────────────────────
            // Seed data — Roles and Departments
            // ──────────────────────────────────────────────

            // Seed the three system roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin", Description = "Full system access — HR and system administration" },
                new Role { RoleId = 2, RoleName = "Manager", Description = "Team-scoped access — manages team leaves and project allocations" },
                new Role { RoleId = 3, RoleName = "Employee", Description = "Self-only access — own profile, attendance, and leaves" }
            );

            // Seed initial departments
            modelBuilder.Entity<Department>().HasData(
                new Department { DepartmentId = 1, DepartmentName = "Engineering", Description = "Software development and engineering" },
                new Department { DepartmentId = 2, DepartmentName = "Human Resources", Description = "Employee relations and HR operations" },
                new Department { DepartmentId = 3, DepartmentName = "Finance", Description = "Financial planning and accounting" }
            );
        }
    }
}
