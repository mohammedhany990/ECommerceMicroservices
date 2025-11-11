using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShippingService.API.Models.Responses;
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
    public class ShippingMethodsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ShippingMethodsController(IMediator mediator)
        {
            _mediator = mediator;
        }


        // POST: api/ShippingMethods
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShippingMethodCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<ShippingMethodDto>.SuccessResponse(result, "Shipping method created successfully."));
        }

        // PUT: api/ShippingMethods
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateShippingMethodCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<ShippingMethodDto>.SuccessResponse(result, "Shipping method updated successfully."));
        }

        // DELETE: api/ShippingMethods/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new DeleteShippingMethodCommand { Id = id });
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Shipping method deleted successfully."));
        }

        // GET: api/ShippingMethods/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new GetShippingMethodByIdQuery { Id = id });
            return Ok(ApiResponse<ShippingMethodDto>.SuccessResponse(result, "Shipping method retrieved successfully."));
        }

        // GET: api/ShippingMethods
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetShippingMethodsQuery());
            return Ok(ApiResponse<List<ShippingMethodDto>>.SuccessResponse(result, "Shipping methods retrieved successfully."));
        }

    }

       
}
