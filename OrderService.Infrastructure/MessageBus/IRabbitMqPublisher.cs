using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.Interfaces
{
    public interface IRabbitMqPublisher<T>
    {
        void Publish<T>(T message);
    }
}
