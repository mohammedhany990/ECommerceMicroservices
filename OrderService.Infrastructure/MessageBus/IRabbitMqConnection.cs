using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.MessageBus
{
    public interface IRabbitMqConnection
    {
        IModel CreateChannel();
    }
}
