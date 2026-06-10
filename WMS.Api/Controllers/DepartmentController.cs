using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.Department;
using WMS.Application.Services;

namespace WMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PagedRequestDto request)
        {
            var result = await _departmentService.GetAllAsync(request);
            return Ok(new ApiResponse<PagedResponseDto<DepartmentDto>>(true, "Departments retrieved successfully.", result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _departmentService.GetByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(new ApiResponse<object?>(false, result.Message));
            }

            return Ok(new ApiResponse<DepartmentDto>(true, result.Message, result.Data!));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateDepartmentDto request)
        {
            var result = await _departmentService.CreateAsync(request);

            if (!result.Success)
            {
                return BadRequest(new ApiResponse<object?>(false, result.Message));
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.DepartmentId }, new ApiResponse<DepartmentDto>(true, result.Message, result.Data));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDepartmentDto request)
        {
            var result = await _departmentService.UpdateAsync(id, request);

            if (!result.Success)
            {
                return BadRequest(new ApiResponse<object?>(false, result.Message));
            }

            return Ok(new ApiResponse<DepartmentDto>(true, result.Message, result.Data!));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _departmentService.DeleteAsync(id);

            if (!result.Success)
            {
                return BadRequest(new ApiResponse<object?>(false, result.Message));
            }

            return Ok(new ApiResponse<object?>(true, result.Message));
        }
    }
}
