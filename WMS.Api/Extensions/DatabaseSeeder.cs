using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Infrastructure.Data;

namespace WMS.Api.Extensions
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<WMSDbContext>();

            // Ensure database is created and up to date
            await context.Database.MigrateAsync();

            // Check if we already have employees
            if (await context.Employees.AnyAsync())
            {
                return; // Database is already seeded
            }

            var firstNames = new[] { "John", "Jane", "Alice", "Bob", "Charlie", "Diana", "Ethan", "Fiona", "George", "Hannah", "Ian", "Julia", "Kevin", "Laura", "Michael", "Nina", "Oliver", "Paula", "Quinn", "Rachel" };
            var lastNames = new[] { "Smith", "Johnson", "Williams", "Jones", "Brown", "Davis", "Miller", "Wilson", "Moore", "Taylor", "Anderson", "Thomas", "Jackson", "White", "Harris", "Martin", "Thompson", "Garcia", "Martinez", "Robinson" };
            
            var random = new Random(12345); // Fixed seed for reproducible data
            string defaultPasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!");

            var newEmployees = new List<Employee>();
            var newUserLogins = new List<UserLogin>();

            for (int i = 1; i <= 50; i++)
            {
                string firstName = firstNames[random.Next(firstNames.Length)];
                string lastName = lastNames[random.Next(lastNames.Length)];
                
                // Ensure unique emails by appending index
                string email = $"{firstName.ToLower()}.{lastName.ToLower()}{i}@wms.local";
                string username = $"{firstName.ToLower()}.{lastName.ToLower()}{i}";

                // Assign random department (1-3) and role (2=Manager, 3=Employee)
                int departmentId = random.Next(1, 4);
                int roleId = random.Next(2, 4);

                // Make the very first user an Admin for testing
                if (i == 1)
                {
                    firstName = "Admin";
                    lastName = "User";
                    email = "admin@wms.local";
                    username = "admin";
                    roleId = 1; // Admin Role
                }

                var employee = new Employee
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PhoneNumber = $"555-01{i:D2}",
                    Gender = random.Next(2) == 0 ? "M" : "F",
                    DOB = DateTime.Today.AddYears(-random.Next(20, 50)),
                    DOJ = DateTime.Today.AddMonths(-random.Next(1, 60)),
                    DepartmentId = departmentId,
                    RoleId = roleId,
                    Status = "Active",
                    CreatedOn = DateTime.Now
                };

                newEmployees.Add(employee);
            }

            // Save employees to get their IDs
            await context.Employees.AddRangeAsync(newEmployees);
            await context.SaveChangesAsync();

            // Create logins for the seeded employees
            foreach (var emp in newEmployees)
            {
                var username = emp.Email.Split('@')[0];
                var login = new UserLogin
                {
                    Username = username,
                    PasswordHash = defaultPasswordHash,
                    RoleId = emp.RoleId ?? 3
                };

                // In a real scenario UserLogin might have a foreign key to Employee, 
                // but per AGENTS (4).md schema, UserLogin doesn't have an EmpId, it just has RoleId.
                // However, our AuthService creates it.

                newUserLogins.Add(login);
            }

            await context.UserLogins.AddRangeAsync(newUserLogins);
            await context.SaveChangesAsync();

            // Generate the markdown file for the user
            var mdPath = Path.Combine(Directory.GetCurrentDirectory(), "seeded_users.md");
            using var writer = new StreamWriter(mdPath);
            await writer.WriteLineAsync("# Seeded Test Users");
            await writer.WriteLineAsync("This file contains the 50 generated test users. All users have the password: **Password123!**");
            await writer.WriteLineAsync();
            await writer.WriteLineAsync("| Employee Name | Username | Email | Role | Department |");
            await writer.WriteLineAsync("|---|---|---|---|---|");

            var roleNames = new Dictionary<int, string> { { 1, "Admin" }, { 2, "Manager" }, { 3, "Employee" } };
            var deptNames = new Dictionary<int, string> { { 1, "Engineering" }, { 2, "Human Resources" }, { 3, "Finance" } };

            foreach (var emp in newEmployees)
            {
                string username = emp.Email.Split('@')[0];
                string roleName = roleNames.GetValueOrDefault(emp.RoleId ?? 3, "Unknown");
                string deptName = deptNames.GetValueOrDefault(emp.DepartmentId ?? 3, "Unknown");
                await writer.WriteLineAsync($"| {emp.FirstName} {emp.LastName} | `{username}` | {emp.Email} | {roleName} | {deptName} |");
            }
        }
    }
}
