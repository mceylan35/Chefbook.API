using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chefbook.API.Controllers;
using Chefbook.API.Repository;
using Chefbook.Model.Models;


namespace Chefbook.API.Services.RepositoryInterfaces
{
    public interface IUserService:IGenericRepository<User>
    {
        Task<User> Register(User user, string password);
        Task<User> Login(string userName, string password);
        Task<bool> UserExists(string userName);
        bool Exists(Guid postId);
        List<User> Users(string user);
        List<Post> Wall(Guid userId);
    }
}