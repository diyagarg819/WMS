using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.Client;
using WMS.Application.Services;

namespace WMS.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        private int GetCurrentUserId()
        {
            return int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int id) ? id : 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SearchRequestDto request)
        {
            var result = await _clientService.GetAllClientsAsync(request);
            return Ok(new ApiResponse<List<ClientDto>>(true, "Clients retrieved successfully.", result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var (success, message, data) = await _clientService.GetClientByIdAsync(id);
            if (!success)
                return NotFound(new ApiResponse<ClientDto>(false, message));

            return Ok(new ApiResponse<ClientDto>(true, message, data));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] CreateClientDto request)
        {
            var userId = GetCurrentUserId();
            var (success, message, data) = await _clientService.CreateClientAsync(request, userId);
            
            if (!success)
                return BadRequest(new ApiResponse<ClientDto>(false, message));

            return CreatedAtAction(nameof(GetById), new { id = data!.ClientId }, new ApiResponse<ClientDto>(true, message, data));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateClientDto request)
        {
            var userId = GetCurrentUserId();
            var (success, message, data) = await _clientService.UpdateClientAsync(id, request, userId);
            
            if (!success)
                return BadRequest(new ApiResponse<ClientDto>(false, message));

            return Ok(new ApiResponse<ClientDto>(true, message, data));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            var (success, message) = await _clientService.DeleteClientAsync(id, userId);
            
            if (!success)
                return BadRequest(new ApiResponse<object>(false, message));

            return Ok(new ApiResponse<object>(true, message));
        }
    }
}
