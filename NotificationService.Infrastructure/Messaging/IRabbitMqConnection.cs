using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Messaging
{
    public interface IRabbitMqConnection
    {
        IModel CreateChannel();
    }
}
