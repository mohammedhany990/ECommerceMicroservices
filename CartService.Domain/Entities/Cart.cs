namespace CartService.Domain.Entities
{
    public class Cart
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public decimal Subtotal => Items.Sum(i => i.UnitPrice * i.Quantity);
        public decimal ShippingCost { get; set; } = 0;
        public decimal TotalPrice => Subtotal + ShippingCost;

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public void AddItem(CartItem item)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                Items.Add(item);
            }

            LastUpdated = DateTime.UtcNow;
        }

        public void RemoveItem(Guid productId)
        {
            var item = Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                Items.Remove(item);
                LastUpdated = DateTime.UtcNow;
            }
        }

        public void Clear()
        {
            Items.Clear();
            LastUpdated = DateTime.UtcNow;
        }
    }

}
