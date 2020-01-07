using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chefbook.API.SignalR.Abstract
{
    public interface IHubNotification
    {
        Task Send(Guid? gondericiId, Guid? aliciId, string message);
    }
}
