using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.API.Models.Requests;
using ProductService.API.Models.Responses;
using ProductService.Application.Commands.CreateProduct;
using ProductService.Application.Commands.DeleteProduct;
using ProductService.Application.Commands.UpdateProduct;
using ProductService.Application.DTOs;
using ProductService.Application.Queries.GetProductById;
using ProductService.Application.Queries.GetProducts;
using System.Net;

namespace ProductService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductsController(IMediator mediator, IHttpClientFactory httpClientFactory)
        {
            _mediator = mediator;
            _httpClientFactory = httpClientFactory;
        }



        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductRequest request)
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
        public async Task<IActionResult> UpdateProduct(Guid id, UpdateProductRequest request)
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
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var result = await _mediator.Send(new DeleteProductCommand { ProductId = id });

            if (!result)
            {
                return NotFound(ApiResponse<bool>.FailResponse(new List<string> { "Product not found" }, "Product not found", 404));
            }


            return Ok(ApiResponse<bool>.SuccessResponse(true, "Product deleted successfully"));
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var result = await _mediator.Send(new GetProductsQuery());

            var response = ApiResponse<List<ProductDto>>.SuccessResponse(result, "Products retrieved successfully");

            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetProduct(Guid id)
        {    

            var result = await _mediator.Send(new GetProductByIdQuery(id));

            if (result == null)
                return NotFound(ApiResponse<ProductDto>.FailResponse(new List<string> { "Product not found" }, "Product not found", 404));

            return Ok(ApiResponse<ProductDto>.SuccessResponse(result, "Product retrieved successfully"));
        }


    }

}
