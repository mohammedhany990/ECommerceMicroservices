using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.DTOs;
using Shared.Enums;
using Shared.Messaging;
using System.Text;
using System.Text.Json;

namespace PaymentService.Infrastructure.Messaging
{
    public class OrderServiceRpcClient
    {
        private readonly RpcClient _rpcClient;

        public OrderServiceRpcClient(RpcClient rpcClient)
        {
            _rpcClient = rpcClient;
        }

        public async Task<OrderDto?> GetOrderByIdAsync(Guid orderId)
        {
            return await _rpcClient.CallAsync<OrderDto>("order.request", orderId);
        }

        public async Task<bool> UpdateOrderPaymentStatusAsync(Guid orderId, PaymentStatus status)
        {
            var request = new UpdateOrderPaymentStatusRpcRequest
            {
                OrderId = orderId,
                PaymentStatus = status.ToString()
            };

            return await _rpcClient.CallAsync<bool>(
                routingKey: "order.updatePaymentStatus",
                message: request
            );
        }
    }

    public class UpdateOrderPaymentStatusRpcRequest
    {
        public Guid OrderId { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
    }
}

