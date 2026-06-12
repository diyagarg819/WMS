using Microsoft.Extensions.Logging;
using WMS.Application.Common;
using WMS.Application.DTOs.Employee;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using BCrypt.Net;

namespace WMS.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserLoginRepository _userLoginRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IUserLoginRepository userLoginRepository,
            ILogger<EmployeeService> logger)
        {
            _employeeRepository = employeeRepository;
            _userLoginRepository = userLoginRepository;
            _logger = logger;
        }

        public async Task<List<EmployeeListDto>> GetAllEmployeesAsync(SearchRequestDto request)
        {
            var employees = await _employeeRepository.GetAllAsync(request.SearchTerm);
            return employees.Select(e => MapToListDto(e)).ToList();
        }

        public async Task<List<EmployeeListDto>> GetEmployeesByDepartmentAsync(
            int departmentId, SearchRequestDto request)
        {
            var employees = await _employeeRepository.GetByDepartmentAsync(
                departmentId, request.SearchTerm);
            return employees.Select(e => MapToListDto(e)).ToList();
        }

        public async Task<EmployeeDetailDto?> GetEmployeeByIdAsync(int employeeId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null) return null;
            return await MapToDetailDto(employee);
        }

        public async Task<EmployeeDetailDto?> CreateEmployeeAsync(CreateEmployeeDto request, int userId)
        {
            bool emailExists = await _employeeRepository.EmailExistsAsync(request.Email);
            if (emailExists)
            {
                _logger.LogWarning("Create employee failed — email already in use: {Email}", request.Email);
                return null;
            }

            bool phoneExists = await _employeeRepository.PhoneNumberExistsAsync(request.PhoneNumber);
            if (phoneExists)
            {
                _logger.LogWarning("Create employee failed — phone number already in use: {PhoneNumber}", request.PhoneNumber);
                return null;
            }

            // Check if the username is already taken
            bool usernameExists = await _userLoginRepository.UsernameExistsAsync(request.Username);
            if (usernameExists)
            {
                _logger.LogWarning("Create employee failed — username already in use: {Username}", request.Username);
                return null;
            }

            var employee = new Employee
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Gender = request.Gender,
                DOB = request.DOB,
                DOJ = request.DOJ,
                DepartmentId = request.DepartmentId,
                RoleId = request.RoleId,
                Status = "Active",
                CreatedOn = DateTime.Now
            };

            var createdEmployee = await _employeeRepository.AddAsync(employee, userId);
            _logger.LogInformation("Employee created: {EmployeeId} — {FirstName} {LastName}",
                createdEmployee.EmployeeId, createdEmployee.FirstName, createdEmployee.LastName);

            // Create UserLogin so the new employee can log in
            var userLogin = new UserLogin
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RoleId = request.RoleId ?? 3 // Default to Employee role
            };
            await _userLoginRepository.AddAsync(userLogin);
            _logger.LogInformation("UserLogin created for employee {EmployeeId} with username: {Username}",
                createdEmployee.EmployeeId, request.Username);

            var result = await _employeeRepository.GetByIdAsync(createdEmployee.EmployeeId);
            return result != null ? await MapToDetailDto(result) : null;
        }

        public async Task<bool> UpdateEmployeeAsync(int employeeId, UpdateEmployeeDto request, int userId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null) return false;

            bool emailExists = await _employeeRepository.EmailExistsAsync(request.Email, employeeId);
            if (emailExists)
            {
                _logger.LogWarning("Update employee failed — email already in use: {Email}", request.Email);
                return false;
            }

            bool phoneExists = await _employeeRepository.PhoneNumberExistsAsync(request.PhoneNumber, employeeId);
            if (phoneExists)
            {
                _logger.LogWarning("Update employee failed — phone number already in use: {PhoneNumber}", request.PhoneNumber);
                return false;
            }

            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.Email = request.Email;
            employee.PhoneNumber = request.PhoneNumber;
            employee.Gender = request.Gender;
            employee.DOB = request.DOB;
            employee.DOJ = request.DOJ;
            employee.DepartmentId = request.DepartmentId;
            employee.RoleId = request.RoleId;
            employee.Status = request.Status ?? employee.Status;
            employee.UpdatedOn = DateTime.Now;

            await _employeeRepository.UpdateAsync(employee, userId);

            // Update UserLogin credentials if username is provided
            if (!string.IsNullOrWhiteSpace(request.Username))
            {
                // Find existing UserLogin by EmployeeId (UserId matches EmployeeId in our schema)
                var existingLogin = await _userLoginRepository.GetByIdAsync(employeeId);

                if (existingLogin != null)
                {
                    // Check if new username is taken by someone else
                    if (existingLogin.Username != request.Username)
                    {
                        bool usernameTaken = await _userLoginRepository.UsernameExistsAsync(request.Username);
                        if (!usernameTaken)
                        {
                            existingLogin.Username = request.Username;
                        }
                        else
                        {
                            _logger.LogWarning("Update employee failed — username already in use: {Username}", request.Username);
                            return false;
                        }
                    }

                    // Update password if a new one is provided
                    if (!string.IsNullOrWhiteSpace(request.Password))
                    {
                        existingLogin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                    }
                    existingLogin.RoleId = request.RoleId ?? existingLogin.RoleId;
                    await _userLoginRepository.UpdateAsync(existingLogin);
                    _logger.LogInformation("UserLogin updated for employee {EmployeeId}", employeeId);
                }
            }

            _logger.LogInformation("Employee updated: {EmployeeId}", employeeId);
            return true;
        }

        public async Task<bool> UpdateMyProfileAsync(int employeeId, UpdateMyProfileDto request, int userId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null) return false;

            bool phoneExists = await _employeeRepository.PhoneNumberExistsAsync(request.PhoneNumber, employeeId);
            if (phoneExists)
            {
                _logger.LogWarning("Update profile failed — phone number already in use: {PhoneNumber}", request.PhoneNumber);
                return false;
            }

            employee.PhoneNumber = request.PhoneNumber;
            employee.UpdatedOn = DateTime.Now;

            await _employeeRepository.UpdateAsync(employee, userId);
            _logger.LogInformation("Employee updated own profile: {EmployeeId}", employeeId);
            return true;
        }

        public async Task<bool> DeleteEmployeeAsync(int employeeId, int deletedByUserId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null) return false;

            await _employeeRepository.DeleteAsync(employee, deletedByUserId);
            _logger.LogInformation("Employee soft-deleted: {EmployeeId} by user: {DeletedBy}",
                employeeId, deletedByUserId);
            return true;
        }

        private EmployeeListDto MapToListDto(Employee employee)
        {
            return new EmployeeListDto
            {
                EmployeeId = employee.EmployeeId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                DepartmentName = employee.Department?.DepartmentName,
                RoleName = employee.Role?.RoleName,
                Status = employee.Status
            };
        }

        private async Task<EmployeeDetailDto> MapToDetailDto(Employee employee)
        {
            // Look up the username from UserLogin using the EmployeeId (UserId)
            var userLogin = await _userLoginRepository.GetByIdAsync(employee.EmployeeId);

            return new EmployeeDetailDto
            {
                EmployeeId = employee.EmployeeId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Gender = employee.Gender,
                DOB = employee.DOB,
                DOJ = employee.DOJ,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.DepartmentName,
                RoleId = employee.RoleId,
                RoleName = employee.Role?.RoleName,
                Status = employee.Status,
                Username = userLogin?.Username,
                CreatedOn = employee.CreatedOn,
                UpdatedOn = employee.UpdatedOn
            };
        }
    }
}
