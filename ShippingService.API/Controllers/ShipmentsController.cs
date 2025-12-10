using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingService.API.Models.Responses;
using ShippingService.Application.Commands.Shipments.CreateShipment;
using ShippingService.Application.Commands.Shipments.DeleteShipment;
using ShippingService.Application.Commands.Shipments.UpdateShipment;
using ShippingService.Application.DTOs;
using ShippingService.Application.Queries.Shipments.GetShipmentById;
using ShippingService.Application.Queries.Shipments.GetShipments;

namespace ShippingService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ShipmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<ShipmentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] CreateShipmentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<ShipmentDto>.SuccessResponse(result, "Shipment created successfully."));
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<ShipmentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update([FromBody] UpdateShipmentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<ShipmentDto>.SuccessResponse(result, "Shipment updated successfully."));
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new DeleteShipmentCommand { Id = id });
            if (!result)
                return NotFound(ApiResponse<object>.FailResponse(new List<string> { "Shipment not found." }, "Not found", 404));

            return Ok(ApiResponse<bool>.SuccessResponse(result, "Shipment deleted successfully."));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ShipmentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new GetShipmentByIdQuery { Id = id });
            if (result == null)
                return NotFound(ApiResponse<object>.FailResponse(new List<string> { "Shipment not found." }, "Not found", 404));

            return Ok(ApiResponse<ShipmentDto>.SuccessResponse(result, "Shipment retrieved successfully."));
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<ShipmentDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetShipmentsQuery());
            return Ok(ApiResponse<List<ShipmentDto>>.SuccessResponse(result, "Shipments retrieved successfully."));
        }
    }
}
