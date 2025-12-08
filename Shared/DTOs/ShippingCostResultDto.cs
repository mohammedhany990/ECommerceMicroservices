namespace Shared.DTOs
{
    public class ShippingCostResultDto
    {
        public Guid ShippingMethodId { get; set; }
        public decimal Cost { get; set; }
        public string MethodName { get; set; }
        public int EstimatedDeliveryDays { get; set; }
    }
}
