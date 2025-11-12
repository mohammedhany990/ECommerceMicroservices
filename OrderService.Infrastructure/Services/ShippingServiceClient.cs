using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.Services
{
    public class ShippingServiceClient
    {
        private readonly HttpClient _httpClient;

        public ShippingServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


    }
}
