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

        // POST: api/Shipments
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create([FromBody] CreateShipmentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<ShipmentDto>.SuccessResponse(result, "Shipment created successfully."));
        }

        // PUT: api/Shipments
        [HttpPut]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Update([FromBody] UpdateShipmentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<ShipmentDto>.SuccessResponse(result, "Shipment updated successfully."));
        }

        // DELETE: api/Shipments/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new DeleteShipmentCommand { Id = id });
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Shipment deleted successfully."));
        }

        // GET: api/Shipments/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new GetShipmentByIdQuery { Id = id });
            return Ok(ApiResponse<ShipmentDto>.SuccessResponse(result, "Shipment retrieved successfully."));
        }

        // GET: api/Shipments
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetShipmentsQuery());
            return Ok(ApiResponse<List<ShipmentDto>>.SuccessResponse(result, "Shipments retrieved successfully."));
        }
    }
}
