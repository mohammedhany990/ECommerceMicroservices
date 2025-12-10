using CartService.API.Models.Responses;
using CartService.Application.Commands.AddItemToCart;
using CartService.Application.Commands.ClearCart;
using CartService.Application.Commands.RemoveItem;
using CartService.Application.Commands.RestoreItemsToCart;
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
        [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddItem([FromBody] AddItemToCartDto dto)
        {
            var userId = GetUserId();

            var result = await _mediator.Send(new AddItemToCartCommand(userId, dto.ProductId, dto.Quantity));
            return Ok(ApiResponse<CartDto>.SuccessResponse(result, "Item added successfully"));
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCart(Guid? shippingAddressId, Guid? shippingMethodId)
        {
            var userId = GetUserId();

            var result = await _mediator.Send(new GetCartQuery(userId, shippingAddressId, shippingMethodId));
            return Ok(ApiResponse<CartDto>.SuccessResponse(result, "Cart retrieved successfully"));
        }

        [HttpDelete("{productId}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveItem(Guid productId)
        {
            var userId = GetUserId();

            var result = await _mediator.Send(new RemoveItemCommand(userId, productId));
            if (!result)
                return NotFound(ApiResponse<object>.FailResponse(
                    new List<string> { "Item not found in cart" },
                    "Remove item failed",
                    StatusCodes.Status404NotFound
                ));

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Item removed successfully"));
        }

        [HttpPut("{productId}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateItemQuantity(Guid productId, [FromQuery] int quantity)
        {
            var userId = GetUserId();

            var result = await _mediator.Send(new UpdateItemQuantityCommand(userId, productId, quantity));
            if (result == null)
                return NotFound(ApiResponse<object>.FailResponse(
                    new List<string> { "Item not found in cart" },
                    "Update failed",
                    StatusCodes.Status404NotFound
                ));

            return Ok(ApiResponse<object>.SuccessResponse(result, "Item quantity updated successfully"));
        }

        [HttpDelete("clear")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();

            await _mediator.Send(new ClearCartCommand(userId));
            return Ok(ApiResponse<object>.SuccessResponse(null!, "Cart cleared successfully"));
        }

        [HttpPost("restore")]
        [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RestoreItemsToCart([FromBody] List<CartItemDto> items)
        {
            var userId = GetUserId();

            var cart = await _mediator.Send(new RestoreItemsToCartCommand(userId, items));
            return Ok(ApiResponse<CartDto>.SuccessResponse(cart, "Items restored to cart successfully"));
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                throw new InvalidOperationException("User ID claim is missing or invalid.");

            return userId;
        }

    }
}
