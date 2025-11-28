using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendAsync(string to, string subject, string body);
    }
}
