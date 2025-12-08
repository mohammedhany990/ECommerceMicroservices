using MediatR;

namespace OrderService.Application.Commands.DeleteOrder
{
    public class DeleteOrderCommand : IRequest<bool>
    {
        public Guid OrderId { get; set; }
        public DeleteOrderCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
