using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Domain.Entities
{
    public class Cart
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public decimal TotalPrice => Items.Sum(i => i.TotalPrice);
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

        public void RemoveItem(string productId)
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
