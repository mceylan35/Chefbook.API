using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Chefbook.API.Context;
using Chefbook.API.SignalR.Abstract;
using Chefbook.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;


namespace Chefbook.API.SignalR.Concrete
{
   // [Authorize]
    //[HubName("emergyHub")]
    public class NotificationHub : Hub
    {


        public override async Task OnConnectedAsync()
        {
            var currentUserId = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            using (var context = new ChefContext())
            {
                var connection = context.Connection.FirstOrDefault(i=>i.UserId==Guid.Parse(currentUserId));
                if (connection != null)
                {
                    connection.ConnectionId = Context.ConnectionId;
                    connection.Connected = true;

                }
                else
                {
                    context.Connection.Add(new Connection
                    {
                        Id = Guid.NewGuid(),
                        UserId = Guid.Parse(currentUserId),
                        ConnectionId = Context.ConnectionId
                    });
                    context.SaveChanges();
                }


               
            }

            await Clients.Caller.SendAsync("GetConnectionId", this.Context.ConnectionId); //Client tarafına ConnectionId yolladım

            // await base.OnConnectedAsync();

        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            using (var context = new ChefContext())
            {
                var connection = context.Connection.FirstOrDefault(i => i.ConnectionId == Context.ConnectionId);
                connection.Connected = false;
                context.SaveChanges();
                
            }

            return base.OnDisconnectedAsync(exception);
        }
        [AllowAnonymous]
        public  Task SendNotification(Guid whoId, Guid sendId, string message)
        {
            using (var context=new ChefContext())
            {
                var who = context.Connection.FirstOrDefault(i => i.UserId == whoId &&i.Connected==true);
                if (who!=null)
                {
                   return Clients.User(who.ConnectionId).SendAsync("NotificationGuncelle");
                }
                else
                {
                    return Clients.All.SendAsync("NotificationGuncelle");
                }
            }
        }
        // public override async Task OnConnectedAsync()
        // {
        //    //var currentUserId = Context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        //    await Clients.Users(Context.ConnectionId).SendAsync("onConnectThisUser");

        //    await base.OnConnectedAsync();

        //     //await Clients.Caller.SendAsync("GetUserId", currentUserId);

        // }
        ////[HubMethodName("sendNotification")]
        // public async Task SendNotification(Guid TargetGuid)
        // {

        //     await Clients.Client(TargetGuid.ToString())
        //         .SendAsync("NotificationGetir", "Notification Fonksiyonunu Yenile ya da Kullanıcı Senin Porfilini Beğendi diyebiliriz.");



        // }









    }
}
