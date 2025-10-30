using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Infrastructure.Messaging.RabbitMQ
{
    public class CategoryEvent
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty; // "Created", "Updated", "Deleted"
    }

}
