using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Chefbook.API.SignalR.Abstract;
using Chefbook.Model.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
namespace Chefbook.API.SignalR.Concrete
{
    public class HubNotification : IHubNotification
    {
        private readonly IHubContext<NotificationHub> _hubNotification;

        public HubNotification(IHubContext<NotificationHub> hubNotification)
        {
            _hubNotification = hubNotification;
        }
        public async Task Send(Guid? gondericiId, Guid? aliciId, string message)
        {
            await this._hubNotification.Clients.All.SendAsync("Send", gondericiId, aliciId, message);
        }

      
        //[HubMethodName("sendNotification")]
        public async Task SendNotification(Guid TargetGuid)
        {

            await this._hubNotification.Clients.Client(TargetGuid.ToString()).SendAsync("NotificationYenile", "Yenile");


            //var targetConnections = Connections.GetConnections(notification.TargetId);
            //targetConnections.ForEach(async (connection) =>
            //{
            //    await Clients.Client(connection).pushNotification(notificationId);
            //});

        }

    }
}
