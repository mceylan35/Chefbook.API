using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chefbook.API.Context;
using Chefbook.API.Repository;
using Chefbook.API.Services.Interface;
using Chefbook.API.Services.RepositoryInterfaces;
using Chefbook.API.SignalR.Abstract;
using Chefbook.API.SignalR.Concrete;
using Chefbook.Model.Models;
using Microsoft.AspNetCore.SignalR;

namespace Chefbook.API.Services.Service
{
    public class NotificationService : GenericRepository<Notification, ChefContext>, INotificationService
    {
        private IHubNotification _hubNotification;
        private IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubNotification hubNotification, IHubContext<NotificationHub> hubContext)
        {
            _hubNotification = hubNotification;
            _hubContext = hubContext;
        }
        public void Create(Notification notification)
        {
            using (var context = new ChefContext())
            {
                context.Notification.Add(notification);
                
               NotificationHub notificationHub=new NotificationHub();
              // notificationHub.OnConnectedAsync();
              // notificationHub.SendNotification(Guid.Parse(notification.TriggerUserId.ToString()));


                _hubContext.Clients.Users(notification.UserId.ToString()).SendAsync("onConnectThisUser", notification.UserId.ToString());

               
                _hubContext.Clients.Clients(notification.TriggerUserId.ToString()).SendAsync("NotificationGetir", "Notification Fonksiyonunu Yenile");

              



            }
        }

        public void ReadNotification(Guid notificationId, Guid userId)
        {
            using (var context=new ChefContext())
            {
                var notification = context.Notification.FirstOrDefault(i=>i.UserId==userId && i.Id==notificationId);
                if (notification!=null)
                {
                    notification.TargetType = false;
                    Update(notification);
                    //Kendi Clientımdaki Notification alanını güncelle
                }
                else
                {
                    Console.Write("hata");
                }
              

            }
            
        }
        public List<Notification> GetUserNotifications(Guid userId)
        {
            using (var context=new ChefContext())
            {
                return context.Notification.Where(i => i.UserId == userId && i.TargetType == true).ToList();
                
            }
        }

        public void AddNotification(Guid aliciId, Guid gondericiId, string mesaj)
        {
            throw new NotImplementedException();
        }
    }
}
