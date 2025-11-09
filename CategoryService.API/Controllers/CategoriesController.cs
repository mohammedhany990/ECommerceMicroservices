using CategoryService.API.Models.Responses;
using CategoryService.Application.DTOs;
using CategoryService.Application.Queries.GetCategoryQuery;
using CategoryService.Application.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CategoryService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<CategoryDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetCategories()
        {
            var result = await _mediator.Send(new GetCategoriesQuery());

            return Ok(ApiResponse<List<CategoryDto>>
                .SuccessResponse(result ?? new List<CategoryDto>(), "Categories retrieved successfully"));
        }


        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> GetCategoryById(Guid id)
        {
            var result = await _mediator.Send(new GetCategoryByIdQuery(id));

            if (result is null)
            {
                return NotFound(ApiResponse<CategoryDto>.FailResponse(
                    new List<string> { $"Category with ID {id} not found." },
                    "Not Found",
                    404));
            }

            return Ok(ApiResponse<CategoryDto>.SuccessResponse(result, "Category retrieved successfully"));
        }


    }
}
