using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.Announcement;
using WMS.Application.Services;

namespace WMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AnnouncementController : ControllerBase
    {
        private readonly IAnnouncementService _service;

        public AnnouncementController(IAnnouncementService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] SearchRequestDto request,
            [FromQuery] bool? isActive = null)
        {
            var result = await _service.GetAllAsync(request, isActive);
            return Ok(new ApiResponse<List<AnnouncementListDto>>(true, "OK", result));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] CreateAnnouncementDto request)
        {
            int userId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int id) ? id : 0;
            var result = await _service.CreateAsync(request, userId);
            return result.Success
                ? StatusCode(201, new ApiResponse<object?>(true, result.Message))
                : BadRequest(new ApiResponse<object?>(false, result.Message));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAnnouncementDto request)
        {
            var result = await _service.UpdateAsync(id, request);
            return result.Success
                ? Ok(new ApiResponse<object?>(true, result.Message))
                : BadRequest(new ApiResponse<object?>(false, result.Message));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result.Success
                ? Ok(new ApiResponse<object?>(true, result.Message))
                : NotFound(new ApiResponse<object?>(false, result.Message));
        }
    }
}
