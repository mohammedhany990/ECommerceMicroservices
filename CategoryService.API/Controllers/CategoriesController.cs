using CategoryService.API.Models.Responses;
using CategoryService.Application.DTOs;
using CategoryService.Application.Queries.GetCategoryQuery;
using CategoryService.Application.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> GetCategories()
        {
            var result = await _mediator.Send(new GetCategoriesQuery());

            if (result is null || !result.Any())
            {
                return NotFound(ApiResponse<List<CategoryDto>>.FailResponse(
                    new List<string> { "No categories found." },
                    "No data available",
                    404));
            }
            return Ok(ApiResponse<List<CategoryDto>>.SuccessResponse(result, "Categories retrieved successfully"));
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var result = await _mediator.Send(new GetCategoryQuery(id));

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
