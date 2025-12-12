using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingService.API.Models.Responses;
using ShippingService.Application.Commands.Addresses.CreateShippingAddress;
using ShippingService.Application.Commands.Addresses.DeleteShippingAddress;
using ShippingService.Application.Commands.Addresses.UpdateShippingAddress;
using ShippingService.Application.DTOs;
using ShippingService.Application.Queries.Addresses.GetShippingAddressById;
using ShippingService.Application.Queries.Addresses.GetShippingAddresses;
using System.Security.Claims;

namespace ShippingService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShippingAddressesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ShippingAddressesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<ShippingAddressDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _mediator.Send(new GetShippingAddressesQuery(userId));
            return Ok(ApiResponse<List<ShippingAddressDto>>.SuccessResponse(result, "Shipping addresses retrieved successfully."));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ShippingAddressDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] CreateShippingAddressRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var command = new CreateShippingAddressCommand
            {
                UserId = userId,
                FullName = request.FullName,
                AddressLine1 = request.AddressLine1,
                AddressLine2 = request.AddressLine2,
                City = request.City,
                State = request.State,
                PostalCode = request.PostalCode,
                Country = request.Country,
                IsDefault = request.IsDefault
            };
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<ShippingAddressDto>.SuccessResponse(result, "Shipping address created successfully."));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ShippingAddressDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateShippingAddressRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var command = new UpdateShippingAddressCommand
            {
                Id = id,
                UserId = userId,
                FullName = request.FullName,
                AddressLine1 = request.AddressLine1,
                AddressLine2 = request.AddressLine2,
                City = request.City,
                State = request.State,
                PostalCode = request.PostalCode,
                Country = request.Country,
                IsDefault = request.IsDefault
            };

            var result = await _mediator.Send(command);

            return Ok(ApiResponse<ShippingAddressDto>.SuccessResponse(
                result,
                "Shipping address updated successfully."
            ));
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new DeleteShippingAddressCommand { Id = id });
            if (!result)
                return NotFound(ApiResponse<object>.FailResponse(new List<string> { "Shipping address not found." }, "Not found", 404));

            return Ok(ApiResponse<bool>.SuccessResponse(result, "Shipping address deleted successfully."));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ShippingAddressDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new GetShippingAddressByIdQuery { ID = id });
            if (result == null)
                return NotFound(ApiResponse<object>.FailResponse(new List<string> { "Shipping address not found." }, "Not found", 404));

            return Ok(ApiResponse<ShippingAddressDto>.SuccessResponse(result, "Shipping address retrieved successfully."));
        }

        
    }
}
