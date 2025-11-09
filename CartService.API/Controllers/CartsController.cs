using CartService.API.Models.Responses;
using CartService.Application.Commands.AddItemToCart;
using CartService.Application.Commands.RemoveItem;
using CartService.Application.Commands.UpdateItemQuantity;
using CartService.Application.DTOs;
using CartService.Application.Queries.GetCart;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CartService.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CartsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddItem([FromBody] CartItemDto item)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized(ApiResponse<object>.FailResponse(
                    new List<string> { "Invalid or missing user ID in token." },
                    "Unauthorized",
                    StatusCodes.Status401Unauthorized
                ));

            var result = await _mediator.Send(new AddItemToCartCommand(userId.Value, item));
            return Ok(ApiResponse<CartDto>.SuccessResponse(result, "Item added successfully"));
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized(ApiResponse<object>.FailResponse(
                    new List<string> { "Invalid or missing user ID in token." },
                    "Unauthorized",
                    StatusCodes.Status401Unauthorized
                ));

            var result = await _mediator.Send(new GetCartQuery(userId.Value));
            return Ok(ApiResponse<CartDto>.SuccessResponse(result, "Cart retrieved successfully"));
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveItem(string productId)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized(ApiResponse<object>.FailResponse(
                    new List<string> { "Invalid or missing user ID in token." },
                    "Unauthorized",
                    StatusCodes.Status401Unauthorized
                ));

            var result = await _mediator.Send(new RemoveItemCommand(userId.Value, productId));
            if (!result)
                return NotFound(ApiResponse<object>.FailResponse(
                    new List<string> { "Item not found in cart" },
                    "Remove item failed",
                    StatusCodes.Status404NotFound));

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Item removed successfully"));
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateItemQuantity(string productId, [FromQuery] int quantity)
        {
            var userId = GetUserId();
            if (userId is null)
                return Unauthorized(ApiResponse<object>.FailResponse(
                    new List<string> { "Invalid or missing user ID in token." },
                    "Unauthorized",
                    StatusCodes.Status401Unauthorized
                ));

            var result = await _mediator.Send(new UpdateItemQuantityCommand(userId.Value, productId, quantity));
            if (!result)
                return NotFound(ApiResponse<object>.FailResponse(
                    new List<string> { "Item not found in cart" },
                    "Update failed",
                    StatusCodes.Status404NotFound));

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Item quantity updated successfully"));
        }


        private Guid? GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return null;

            return userId;
        }
    }
}
