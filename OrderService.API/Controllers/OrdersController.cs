using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderService.API.Models.Responses;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Application.DTOs;
using OrderService.Application.Queries.GetOrdersByUser;
using System.Security.Claims;

namespace OrderService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
           
            command.AuthToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", ""); ;

            var order = await _mediator.Send(command);

            var response = ApiResponse<OrderDto>.SuccessResponse(order, "Order created successfully");

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderByUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", ""); 

            var order = await _mediator.Send(new GetOrderByUserIdQuery(Guid.Parse(userId) ,token));
            var response = ApiResponse<OrderDto>.SuccessResponse(order, "Order retrieved successfully");
            return Ok(response);
        }

    }
}
