using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.Employee;
using WMS.Application.Services;

namespace WMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SearchRequestDto request)
        {
            var result = await _employeeService.GetAllEmployeesAsync(request);
            return Ok(new ApiResponse<List<EmployeeListDto>>(true, "Employees retrieved", result));
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound(new ApiResponse<object>(false, "Employee not found"));

            return Ok(new ApiResponse<EmployeeDetailDto>(true, "Employee retrieved", employee));
        }

        [HttpGet("my-profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            int userId = GetUserIdFromToken();
            if (userId == 0)
                return Unauthorized(new ApiResponse<object>(false, "Unable to identify user"));

            var employee = await _employeeService.GetEmployeeByIdAsync(userId);
            if (employee == null)
                return NotFound(new ApiResponse<object>(false, "Profile not found"));

            return Ok(new ApiResponse<EmployeeDetailDto>(true, "Profile retrieved", employee));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDto request)
        {
            int userId = GetUserIdFromToken();
            var created = await _employeeService.CreateEmployeeAsync(request, userId);

            if (created == null)
                return Conflict(new ApiResponse<object>(false, "Email already in use or validation failed"));

            return StatusCode(201, new ApiResponse<EmployeeDetailDto>(true, "Employee created", created));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDto request)
        {
            int userId = GetUserIdFromToken();
            var success = await _employeeService.UpdateEmployeeAsync(id, request, userId);

            if (!success)
                return Conflict(new ApiResponse<object>(false, "Employee not found, email in use, or validation failed"));

            return Ok(new ApiResponse<object>(true, "Employee updated"));
        }

        [HttpPut("my-profile")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateMyProfileDto request)
        {
            int userId = GetUserIdFromToken();
            if (userId == 0)
                return Unauthorized(new ApiResponse<object>(false, "Unable to identify user"));

            var success = await _employeeService.UpdateMyProfileAsync(userId, request, userId);

            if (!success)
                return NotFound(new ApiResponse<object>(false, "Profile not found"));

            return Ok(new ApiResponse<object>(true, "Profile updated"));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = GetUserIdFromToken();
            var success = await _employeeService.DeleteEmployeeAsync(id, userId);

            if (!success)
                return NotFound(new ApiResponse<object>(false, "Employee not found"));

            return Ok(new ApiResponse<object>(true, "Employee deactivated"));
        }

        private int GetUserIdFromToken()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out int id) ? id : 0;
        }
    }
}
