using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PaymentService.Application.DTOs;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;
using PaymentService.Infrastructure.Messaging;
using Shared.Enums;
using Stripe;

namespace PaymentService.Application.Commands.CreatePayment
{
    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, PaymentDto>
    {
        private readonly IPaymentRepository _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly OrderServiceRpcClient _orderServiceRpcClient;
        private readonly ILogger<CreatePaymentCommandHandler> _logger;

        public CreatePaymentCommandHandler(
            IPaymentRepository repository,
            IMapper mapper,
            IConfiguration configuration,
            OrderServiceRpcClient orderServiceRpcClient,
            ILogger<CreatePaymentCommandHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _configuration = configuration;
            _orderServiceRpcClient = orderServiceRpcClient;
            _logger = logger;
        }

        public async Task<PaymentDto> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting payment creation for OrderId: {OrderId}, UserId: {UserId}", request.OrderId, request.UserId);

            var order = await _orderServiceRpcClient.GetOrderByIdAsync(request.OrderId);
            if (order == null)
            {
                _logger.LogWarning("Order not found. OrderId: {OrderId}", request.OrderId);
                throw new Exception("Order not found or invalid.");
            }

            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
            var amountToCharge = order.TotalAmount;

            _logger.LogInformation("Creating Stripe PaymentIntent for amount: {Amount}", amountToCharge);

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amountToCharge * 100),
                Currency = "usd",
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

            _logger.LogInformation("Stripe PaymentIntent created successfully. PaymentIntentId: {PaymentIntentId}", intent.Id);

            var payment = new Payment
            {
                OrderId = request.OrderId,
                UserId = request.UserId,
                Amount = order.TotalAmount,
                Currency = "usd",
                PaymentIntentId = intent.Id,
                ClientSecret = intent.ClientSecret,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(payment);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Payment record created in database. PaymentId: {PaymentId}", payment.Id);

            return _mapper.Map<PaymentDto>(payment);
        }
    }
}