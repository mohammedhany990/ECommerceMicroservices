using MediatR;

namespace ProductService.Application.Commands.DeleteProduct
{
    public class DeleteProductCommand : IRequest<bool>
    {
        public Guid ProductId { get; set; }
        public DeleteProductCommand()
        {

        }
    }
}
