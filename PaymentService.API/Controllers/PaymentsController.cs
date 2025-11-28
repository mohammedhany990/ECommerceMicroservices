using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PaymentService.API.Models.Responses;
using PaymentService.Application.Commands.CancelPayment;
using PaymentService.Application.Commands.CreatePayment;
using PaymentService.Application.Commands.RefundPayment;
using PaymentService.Application.DTOs;
using PaymentService.Application.Events;
using PaymentService.Application.Queries.GetPaymentById;
using PaymentService.Application.Queries.GetPaymentByOrderId;
using PaymentService.Application.Queries.GetPaymentsByUser;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PaymentService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IStripeWebhookService _stripeWebhookService;

        public PaymentsController(IMediator mediator, IStripeWebhookService stripeWebhookService)
        {
            _mediator = mediator;
            _stripeWebhookService = stripeWebhookService;
        }

        [HttpPost]
        [EnableRateLimiting("payment-concurrency")]
        [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePayment([FromBody][Required] Guid orderId)
        {
            var command = new CreatePaymentCommand
            {
                OrderId = orderId,
                UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                AuthToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim(),
            };

            var result = await _mediator.Send(command);
            return Ok(ApiResponse<PaymentDto>.SuccessResponse(result, "Payment created successfully."));
        }


        [HttpGet("by-order/{orderId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<PaymentResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPaymentByOrderId(Guid orderId)
        {
            var payment = await _mediator.Send(new GetPaymentByOrderIdQuery(orderId));
            if (payment == null)
                return NotFound(ApiResponse<object>.FailResponse(null!, "Payment not found for this order."));

            return Ok(ApiResponse<PaymentResultDto>.SuccessResponse(payment, "Payment retrieved successfully."));
        }

        [HttpPost("cancel")]
        [ProducesResponseType(typeof(ApiResponse<PaymentResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelPayment([FromBody] CancelPaymentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<PaymentResultDto>.SuccessResponse(result, "Payment canceled successfully."));
        }

        [HttpPost("refund")]
        [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RefundPayment([FromBody] RefundPaymentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<PaymentDto>.SuccessResponse(result, "Payment refunded successfully."));
        }




        [HttpGet("{paymentId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPaymentById([FromRoute] Guid paymentId)
        {
            var payment = await _mediator.Send(new GetPaymentByIdQuery { PaymentId = paymentId });
            if (payment == null)
                return NotFound(ApiResponse<object>.FailResponse(null!, "Payment not found."));

            return Ok(ApiResponse<PaymentDto>.SuccessResponse(payment, "Payment retrieved successfully."));
        }


        [HttpGet("user-payments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaymentsByUser()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var payments = await _mediator.Send(new GetPaymentsByUserQuery { UserId = userId });

            return Ok(ApiResponse<List<PaymentDto>>.SuccessResponse(payments, "User payments retrieved successfully."));
        }


        [HttpPost("stripe")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> StripeWebhook()
        {
            Request.EnableBuffering();

            string json;
            using (var reader = new StreamReader(Request.Body, leaveOpen: true))
            {
                json = await reader.ReadToEndAsync();
                Request.Body.Position = 0;
            }

            if (!Request.Headers.TryGetValue("Stripe-Signature", out var signatureHeader))
            {
                return BadRequest(ApiResponse<object>.FailResponse(
                    new List<string> { "Missing Stripe-Signature header." },
                    "Invalid request"));
            }

            var signature = signatureHeader.FirstOrDefault();
            if (string.IsNullOrEmpty(signature))
            {
                return BadRequest(ApiResponse<object>.FailResponse(
                    new List<string> { "Invalid Stripe-Signature header." },
                    "Invalid request"));
            }

            try
            {
                var payment = await _stripeWebhookService.HandleEventAsync(json, signature);

                if (payment != null && (payment.Status == PaymentStatus.Succeeded || payment.Status == PaymentStatus.Confirmed))
                {
                    await _mediator.Publish(new PaymentSucceededEvent
                    {
                        OrderId = payment.OrderId,
                        PaymentId = payment.Id,
                        Amount = payment.Amount
                    });
                }

                return Ok(ApiResponse<object>.SuccessResponse(null, "Webhook processed successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.FailResponse(
                    new List<string> { ex.Message },
                    "Webhook processing failed"));
            }
        }

    }
}
