using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chefbook.API.Models;
using Chefbook.API.Repository;
using Chefbook.Model.Models;

namespace Chefbook.API.Services.Interface
{
    public interface IImageService : IGenericRepository<Image>
    {
        void AddRange(List<Image> images);
        List<ProfilePostsDto> FindImage(Guid userId);
    }
}
