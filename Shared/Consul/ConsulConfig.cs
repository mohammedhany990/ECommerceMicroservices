using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Consul
{
    public class ConsulConfig
    {
        public string ServiceId { get; set; } = Guid.NewGuid().ToString(); // Default unique ID
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceAddress { get; set; } = "localhost";
        public int ServicePort { get; set; }
        public int HealthCheckIntervalSeconds { get; set; } = 10;
        public int HealthCheckTimeoutSeconds { get; set; } = 5;
    }
}
