using Microsoft.Extensions.Logging;
using WMS.Application.Common;
using WMS.Application.DTOs.Department;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ILogger<DepartmentService> _logger;

        public DepartmentService(IDepartmentRepository departmentRepository, ILogger<DepartmentService> logger)
        {
            _departmentRepository = departmentRepository;
            _logger = logger;
        }

        public async Task<PagedResponseDto<DepartmentDto>> GetAllAsync(PagedRequestDto request)
        {
            var (records, totalCount) = await _departmentRepository.GetAllAsync(
                request.PageNumber, request.PageSize, request.SearchTerm);

            var dtos = records.Select(d => new DepartmentDto
            {
                DepartmentId = d.DepartmentId,
                DepartmentName = d.DepartmentName,
                Description = d.Description,
                CreatedOn = d.CreatedOn
            }).ToList();

            return new PagedResponseDto<DepartmentDto>
            {
                Data = dtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<(bool Success, string Message, DepartmentDto? Data)> GetByIdAsync(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
            {
                return (false, "Department not found.", null);
            }

            return (true, "Department found.", MapToDto(department));
        }

        public async Task<(bool Success, string Message, DepartmentDto? Data)> CreateAsync(CreateDepartmentDto request)
        {
            // Enforce unique department name
            var existing = await _departmentRepository.GetByNameAsync(request.DepartmentName);
            if (existing != null)
            {
                return (false, "A department with this name already exists.", null);
            }

            var department = new Department
            {
                DepartmentName = request.DepartmentName,
                Description = request.Description,
                CreatedOn = DateTime.Now
            };

            var created = await _departmentRepository.AddAsync(department);

            _logger.LogInformation("Department created: {DepartmentName}", created.DepartmentName);

            return (true, "Department created successfully.", MapToDto(created));
        }

        public async Task<(bool Success, string Message, DepartmentDto? Data)> UpdateAsync(int id, UpdateDepartmentDto request)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
            {
                return (false, "Department not found.", null);
            }

            // Enforce unique department name if changing
            if (department.DepartmentName.ToLower() != request.DepartmentName.ToLower())
            {
                var existing = await _departmentRepository.GetByNameAsync(request.DepartmentName);
                if (existing != null)
                {
                    return (false, "Another department with this name already exists.", null);
                }
            }

            department.DepartmentName = request.DepartmentName;
            department.Description = request.Description;

            await _departmentRepository.UpdateAsync(department);

            _logger.LogInformation("Department updated: {DepartmentId}", department.DepartmentId);

            return (true, "Department updated successfully.", MapToDto(department));
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
            {
                return (false, "Department not found.");
            }

            // Ensure no employees are assigned to this department
            if (department.Employees != null && department.Employees.Any())
            {
                return (false, "Cannot delete department. There are employees assigned to it.");
            }

            await _departmentRepository.DeleteAsync(department);

            _logger.LogInformation("Department deleted: {DepartmentId}", id);

            return (true, "Department deleted successfully.");
        }

        private DepartmentDto MapToDto(Department department)
        {
            return new DepartmentDto
            {
                DepartmentId = department.DepartmentId,
                DepartmentName = department.DepartmentName,
                Description = department.Description,
                CreatedOn = department.CreatedOn
            };
        }
    }
}
