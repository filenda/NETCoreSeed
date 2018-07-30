using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NETCoreSeed.Shared.Interfaces
{
    public interface IPushNotificationService
    {
        Task<bool> SendByPlayerAsync(string[] PlayerIds, string Message);
    }
}