using System;
using Chefbook.API.Controllers;
using Chefbook.API.Repository;
using Chefbook.Model.Models;


namespace Chefbook.API.Services.Interface
{
    public interface IPostService: IGenericRepository<Post>
    {
        bool Like(Guid postId);
        bool DisLike(Guid postId);
        bool Exists(Guid postId);
    }
}
