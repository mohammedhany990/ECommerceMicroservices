using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.API.Models.Requests;
using ProductService.API.Models.Responses;
using ProductService.Application.Commands.CreateProduct;
using ProductService.Application.Commands.DeleteProduct;
using ProductService.Application.Commands.UpdateProduct;
using ProductService.Application.DTOs;
using ProductService.Application.Queries.GetProductById;
using ProductService.Application.Queries.GetProducts;

namespace ProductService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<ProductDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetProducts()
        {
            var result = await _mediator.Send(new GetProductsQuery());
            return Ok(ApiResponse<List<ProductDto>>.SuccessResponse(result, "Products retrieved successfully"));
        }




        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProductDto>>> GetProduct(Guid id)
        {
            var result = await _mediator.Send(new GetProductByIdQuery(id));

            if (result == null)
                return NotFound(ApiResponse<ProductDto>.FailResponse(new List<string> { "Product not found" }, "Product not found", 404));

            return Ok(ApiResponse<ProductDto>.SuccessResponse(result, "Product retrieved successfully"));
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
        public async Task<ActionResult<ApiResponse<ProductDto>>> CreateProduct([FromForm] CreateProductRequest request)
        {
            byte[]? imageBytes = null;
            string? imageName = null;

            if (request.Image != null)
            {
                using var ms = new MemoryStream();
                await request.Image.CopyToAsync(ms);
                imageBytes = ms.ToArray();
                imageName = request.Image.FileName;
            }

            var command = new CreateProductCommand
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                DiscountPrice = request.DiscountPrice,
                QuantityInStock = request.QuantityInStock,
                CategoryId = request.CategoryId,
                ImageBytes = imageBytes,
                ImageName = imageName
            };

            var result = await _mediator.Send(command);

            return StatusCode(201, ApiResponse<ProductDto>.SuccessResponse(result, "Product created successfully", 201));
        }


        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<ProductDto>>> UpdateProduct(Guid id, [FromForm] UpdateProductRequest request)
        {
            byte[]? imageBytes = null;
            string? imageName = null;

            if (request.Image != null)
            {
                using var ms = new MemoryStream();
                await request.Image.CopyToAsync(ms);
                imageBytes = ms.ToArray();
                imageName = request.Image.FileName;
            }

            var command = new UpdateProductCommand
            {
                Id = id,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                DiscountPrice = request.DiscountPrice,
                QuantityInStock = request.QuantityInStock,
                CategoryId = request.CategoryId,
                ImageBytes = imageBytes,
                ImageName = imageName
            };

            var result = await _mediator.Send(command);

            return Ok(ApiResponse<ProductDto>.SuccessResponse(result, "Product updated successfully"));
        }


        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteProduct(Guid id)
        {
            var result = await _mediator.Send(new DeleteProductCommand { ProductId = id });

            if (!result)
                return NotFound(ApiResponse<bool>.FailResponse(new List<string> { "Product not found" }, "Product not found", 404));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Product deleted successfully"));
        }


        
        

    }

}
