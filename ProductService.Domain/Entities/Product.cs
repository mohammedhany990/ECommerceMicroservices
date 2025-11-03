namespace ProductService.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }

        public int QuantityInStock { get; set; }

        public Guid CategoryId { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;



        public void DecreaseStock(int amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive.");

            if (QuantityInStock < amount)
                throw new InvalidOperationException("Not enough stock available.");

            QuantityInStock -= amount;
        }

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice <= 0)
                throw new ArgumentException("Price must be positive.");

            Price = newPrice;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
