using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chefbook.API.Repository;
using Chefbook.Model.Models;

namespace Chefbook.API.Services.Interface
{
    public interface INotificationService : IGenericRepository<Notification>
    {
        void Create(Notification notification);
        List<Notification> GetUserNotifications(Guid userId);
        void AddNotification(Guid aliciId, Guid gondericiId, string mesaj);
    }
}
