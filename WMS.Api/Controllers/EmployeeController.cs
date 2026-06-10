using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.Employee;
using WMS.Application.Services;

namespace WMS.Api.Controllers
{
    /// <summary>
    /// Employee CRUD operations with role-based access control.
    /// Admin: full CRUD + search. Manager: read + search. Employee: own profile only.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(IEmployeeService employeeService, ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        // GET /api/employee — Admin and Manager get paginated employee list
        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PagedRequestDto request)
        {
            var result = await _employeeService.GetAllEmployeesAsync(request);
            return Ok(new ApiResponse<PagedResponseDto<EmployeeListDto>>(true, "Employees retrieved", result));
        }

        // GET /api/employee/{id} — Admin and Manager can view any employee
        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);

            if (employee == null)
                return NotFound(new ApiResponse<object>(false, "Employee not found"));

            return Ok(new ApiResponse<EmployeeDetailDto>(true, "Employee retrieved", employee));
        }

        // GET /api/employee/my-profile — any authenticated user can view their own profile
        [HttpGet("my-profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            int? employeeId = GetEmployeeIdFromToken();
            if (employeeId == null)
                return Unauthorized(new ApiResponse<object>(false, "Unable to identify user"));

            var employee = await _employeeService.GetEmployeeByIdAsync(employeeId.Value);

            if (employee == null)
                return NotFound(new ApiResponse<object>(false, "Profile not found"));

            return Ok(new ApiResponse<EmployeeDetailDto>(true, "Profile retrieved", employee));
        }

        // POST /api/employee — Admin creates a new employee
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDto request)
        {
            var created = await _employeeService.CreateEmployeeAsync(request);

            if (created == null)
                return Conflict(new ApiResponse<object>(false, "Email already in use or validation failed"));

            return StatusCode(201, new ApiResponse<EmployeeDetailDto>(true, "Employee created", created));
        }

        // PUT /api/employee/{id} — Admin updates any employee
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDto request)
        {
            var success = await _employeeService.UpdateEmployeeAsync(id, request);

            if (!success)
                return Conflict(new ApiResponse<object>(false, "Employee not found, email in use, or validation failed"));

            return Ok(new ApiResponse<object>(true, "Employee updated"));
        }

        // PUT /api/employee/my-profile — any authenticated user updates their own phone number
        [HttpPut("my-profile")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateMyProfileDto request)
        {
            int? employeeId = GetEmployeeIdFromToken();
            if (employeeId == null)
                return Unauthorized(new ApiResponse<object>(false, "Unable to identify user"));

            var success = await _employeeService.UpdateMyProfileAsync(employeeId.Value, request);

            if (!success)
                return NotFound(new ApiResponse<object>(false, "Profile not found"));

            return Ok(new ApiResponse<object>(true, "Profile updated"));
        }

        // DELETE /api/employee/{id} — Admin soft-deletes an employee
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int deletedByUserId = GetUserIdFromToken();

            var success = await _employeeService.DeleteEmployeeAsync(id, deletedByUserId);

            if (!success)
                return NotFound(new ApiResponse<object>(false, "Employee not found"));

            return Ok(new ApiResponse<object>(true, "Employee deactivated"));
        }

        // Extract the user's EmployeeId from the JWT claims (NameIdentifier = UserId)
        // Note: In the current auth setup, the JWT stores UserId, not EmployeeId.
        // For now we use UserId as a proxy — a mapping will be added if needed.
        private int? GetEmployeeIdFromToken()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (claim != null && int.TryParse(claim, out int id))
                return id;
            return null;
        }

        // Extract the UserId for audit logging
        private int GetUserIdFromToken()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out int id) ? id : 0;
        }
    }
}
