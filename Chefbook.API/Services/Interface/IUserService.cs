using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chefbook.API.Controllers;
using Chefbook.API.Models;
using Chefbook.API.Repository;
using Chefbook.Model.Models;


namespace Chefbook.API.Services.RepositoryInterfaces
{
    public interface IUserService:IGenericRepository<User>
    {
        Task<User> Register(User user, string password);
        Task<User> Login(string userName, string password);
        Task<bool> UserExists(string userName);
        Task<bool> ChangePassword(ChangePasswordViewModel model, Guid userId);
        bool Exists(Guid postId);
        List<SearchUserModel> Users(string user);
        List<WallPostViewModel> Wall(Guid userId);
        Task<bool> MailExists(string mail);
        List<ExplorePostViewModel> Explore(Guid? userId);
        ProfileDto Profile(Guid userId);
    }
}