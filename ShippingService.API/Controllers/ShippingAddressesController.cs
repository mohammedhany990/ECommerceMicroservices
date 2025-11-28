using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShippingService.API.Models.Responses;
using ShippingService.Application.Commands.Addresses.CreateShippingAddress;
using ShippingService.Application.Commands.Addresses.DeleteShippingAddress;
using ShippingService.Application.Commands.Addresses.UpdateShippingAddress;
using ShippingService.Application.DTOs;
using ShippingService.Application.Queries.Addresses.GetShippingAddressById;
using ShippingService.Application.Queries.Addresses.GetShippingAddresses;

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

        // POST: api/ShippingAddresses
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShippingAddressCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<ShippingAddressDto>.SuccessResponse(result, "Shipping address created successfully."));
        }

        // PUT: api/ShippingAddresses
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateShippingAddressCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<ShippingAddressDto>.SuccessResponse(result, "Shipping address updated successfully."));
        }

        // DELETE: api/ShippingAddresses/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new DeleteShippingAddressCommand { Id = id });
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Shipping address deleted successfully."));
        }

        // GET: api/ShippingAddresses/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new GetShippingAddressByIdQuery { ID = id });
            return Ok(ApiResponse<ShippingAddressDto>.SuccessResponse(result, "Shipping address retrieved successfully."));
        }

        // GET: api/ShippingAddresses
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetShippingAddressesQuery());
            return Ok(ApiResponse<List<ShippingAddressDto>>.SuccessResponse(result, "Shipping addresses retrieved successfully."));
        }
    }
}
