using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserService.API.Models.Responses;
using UserService.Application.Commands.LoginUser;
using UserService.Application.Commands.RefreshToken;
using UserService.Application.Commands.RegisterUser;
using UserService.Application.DTOs;
using UserService.Application.Queries.GetUsers;
using UserService.Domain.Interfaces;

namespace UserService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<UserDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetAllUsers()
        {
            var result = await _mediator.Send(new GetUsersQuery());
            return Ok(ApiResponse<List<UserDto>>.SuccessResponse(result, "Users retrieved successfully", 200));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "User created successfully", 200));
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Login successful", 200));
        }




        [HttpPost("refresh")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Refresh([FromBody] RefreshTokenCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Token refreshed successfully", 200));
        }








        //[HttpPost("revoke")]
        //public async Task<IActionResult> Revoke([FromBody] string email)
        //{
        //    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        //    if (user == null)
        //        return NotFound();

        //    user.RefreshToken = null;
        //    await _context.SaveChangesAsync();
        //    return Ok("Token revoked.");
        //}

    }

   
}

