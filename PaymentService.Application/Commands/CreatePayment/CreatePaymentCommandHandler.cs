using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using PaymentService.Application.DTOs;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;
using PaymentService.Infrastructure.Messaging;
using Stripe;

namespace PaymentService.Application.Commands.CreatePayment
{
    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, PaymentDto>
    {
        private readonly IPaymentRepository _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly OrderServiceRpcClient _orderServiceRpcClient;

        public CreatePaymentCommandHandler(
            IPaymentRepository repository,
            IMapper mapper,
             IConfiguration configuration,
             OrderServiceRpcClient orderServiceRpcClient
            )
        {
            _repository = repository;
            _mapper = mapper;
            _configuration = configuration;
            _orderServiceRpcClient = orderServiceRpcClient;
        }

        public async Task<PaymentDto> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderServiceRpcClient.GetOrderByIdAsync(request.OrderId);

            if (order == null)
                throw new Exception("Order not found or invalid.");

            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

            var amountToCharge = order.TotalAmount;

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amountToCharge * 100),
                Currency = request.Currency,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                    AllowRedirects = "never"
                },
                Metadata = new Dictionary<string, string>
                    {
                        { "order_id", request.OrderId.ToString() },
                        { "user_id", request.UserId.ToString() }
                    }
            };

            var service = new PaymentIntentService();

            var intent = await service.CreateAsync(options, cancellationToken: cancellationToken);

            var payment = new Payment
            {
                OrderId = request.OrderId,
                UserId = request.UserId,
                Amount = order.TotalAmount,
                Currency = request.Currency,
                PaymentIntentId = intent.Id,
                ClientSecret = intent.ClientSecret,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(payment);
            await _repository.SaveChangesAsync();

            var paymentDto = _mapper.Map<PaymentDto>(payment);

            return paymentDto;
        }
    }
}
