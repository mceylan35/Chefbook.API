using System;
using System.Collections.Generic;
using Chefbook.API.Controllers;
using Chefbook.API.Models;
using Chefbook.API.Repository;
using Chefbook.Model.Models;


namespace Chefbook.API.Services.Interface
{
    public interface IPostService: IGenericRepository<Post>
    {
        bool Like(Guid postId);
        bool DisLike(Guid postId);
        bool Exists(Guid postId);
        IcerikDetayViewModel Detay(Guid postId);
    }
}
