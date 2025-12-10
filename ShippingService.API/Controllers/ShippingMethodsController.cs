using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingService.API.Models.Responses;
using ShippingService.Application.Commands.Methods.CalculateShippingCost;
using ShippingService.Application.Commands.Methods.CreateShippingMethod;
using ShippingService.Application.Commands.Methods.DeleteShippingMethod;
using ShippingService.Application.Commands.Methods.UpdateShippingMethod;
using ShippingService.Application.DTOs;
using ShippingService.Application.Queries.Methods.GetShippingMethodById;
using ShippingService.Application.Queries.Methods.GetShippingMethods;

namespace ShippingService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShippingMethodsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ShippingMethodsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ShippingMethodDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] CreateShippingMethodCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<ShippingMethodDto>.SuccessResponse(result, "Shipping method created successfully."));
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<ShippingMethodDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update([FromBody] UpdateShippingMethodCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<ShippingMethodDto>.SuccessResponse(result, "Shipping method updated successfully."));
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new DeleteShippingMethodCommand { Id = id });
            if (!result)
                return NotFound(ApiResponse<object>.FailResponse(new List<string> { "Shipping method not found." }, "Not found", 404));

            return Ok(ApiResponse<bool>.SuccessResponse(result, "Shipping method deleted successfully."));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ShippingMethodDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new GetShippingMethodByIdQuery { Id = id });
            if (result == null)
                return NotFound(ApiResponse<object>.FailResponse(new List<string> { "Shipping method not found." }, "Not found", 404));

            return Ok(ApiResponse<ShippingMethodDto>.SuccessResponse(result, "Shipping method retrieved successfully."));
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<List<ShippingMethodDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetShippingMethodsQuery());
            return Ok(ApiResponse<List<ShippingMethodDto>>.SuccessResponse(result, "Shipping methods retrieved successfully."));
        }

        [HttpPost("calculate")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<Shared.DTOs.ShippingCostResultDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CalculateShippingCost([FromBody] CalculateShippingCostCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<Shared.DTOs.ShippingCostResultDto>.SuccessResponse(result, "Shipping cost calculated successfully."));
        }
    }
}
