using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using OrderService.API.Models.Responses;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Application.Commands.DeleteOrder;
using OrderService.Application.Commands.UpdateOrder;
using OrderService.Application.DTOs;
using OrderService.Application.Queries.GetOrderById;
using OrderService.Application.Queries.GetOrdersByUser;
using OrderService.Domain.Entities;
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
        [EnableRateLimiting("order-per-user")]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var command = new CreateOrderCommand
            {

                UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                Email = User.FindFirstValue(ClaimTypes.Email),
                ShippingAddressId = dto.ShippingAddressId,
                ShippingMethodId = dto.ShippingMethodId,
            };
            var order = await _mediator.Send(command);
            return Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Order created successfully"));
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<OrderDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrdersForCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var orders = await _mediator.Send(new GetOrderByUserIdQuery(Guid.Parse(userId)));

            if (orders == null || !orders.Any())
                return NotFound(ApiResponse<object>.FailResponse(null!, "No orders found for this user."));

            return Ok(ApiResponse<IReadOnlyList<OrderDto>>.SuccessResponse(orders, "Orders retrieved successfully."));
        }

        [HttpGet("{orderId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderById(Guid orderId)
        {
            var order = await _mediator.Send(new GetOrderByIdQuery(orderId));
            if (order is null)
                return NotFound(ApiResponse<object>.FailResponse(null!, "Order not found"));

            return Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Order retrieved successfully"));
        }

        [HttpGet("statuses")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderStatusDto>>), StatusCodes.Status200OK)]
        public IActionResult GetOrderStatuses()
        {
            var statuses = Enum.GetValues<OrderStatus>()
                               .Select(s => new OrderStatusDto { Id = (int)s, Name = s.ToString() });
            return Ok(ApiResponse<IEnumerable<OrderStatusDto>>.SuccessResponse(statuses, "Order statuses retrieved successfully"));
        }

        [HttpPut("{orderId:guid}")]
        public async Task<IActionResult> UpdateOrder([FromRoute] Guid orderId, [FromBody] UpdateOrderCommand request)
        {
            if (orderId != request.OrderId)
                return BadRequest(ApiResponse<object>.FailResponse(null!, "OrderId in route and body must match."));

            if (request.Status.HasValue && !Enum.IsDefined(typeof(OrderStatus), request.Status.Value))
                return BadRequest(ApiResponse<object>.FailResponse(null!, "Invalid status value."));


            var order = await _mediator.Send(request);

            if (order is null)
                return NotFound(ApiResponse<object>.FailResponse(null!, "Order not found."));

            return Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Order updated successfully."));
        }


        [HttpDelete("{orderId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOrder(Guid orderId)
        {
            var result = await _mediator.Send(new DeleteOrderCommand(orderId));

            if (!result)
                return NotFound(ApiResponse<object>.FailResponse(null!, "Order not found or cannot be deleted."));

            return Ok(ApiResponse<object>.SuccessResponse(null, "Order deleted successfully."));
        }



        //[HttpPost("{orderId:guid}/pay")]
        //public async Task<IActionResult> PayOrder(Guid orderId, [FromBody] PaymentDto payment)
        //{
        //    var order = await _mediator.Send(new PayOrderCommand(orderId, payment));
        //    if (order is null)
        //        return NotFound(ApiResponse<object>.FailResponse(null!, "Order not found."));

        //    return Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Payment processed successfully."));
        //}



    }
}
