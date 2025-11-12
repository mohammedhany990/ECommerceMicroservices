using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.DTOs
{
    public class RestoreCartItemsDto
    {
        public List<CartItemDto> Items { get; set; } = new();
    }

}
