using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Chefbook.API.Context;
using Chefbook.API.Models;
using Chefbook.API.Repository;
using Chefbook.API.Services.Interface;
using Chefbook.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Chefbook.API.Services.Service
{
    public class ImageService : GenericRepository<Image, ChefContext>, IImageService
    {
        public async void AddRange(List<Image> images)
        {
            using (var context=new ChefContext())
            {
                    context.Image.AddRange(images);
                   await context.SaveChangesAsync();
                
                
            }
        }

        public List<ProfilePostsDto> FindImage(Guid userId)
        {
            using (var context = new ChefContext())
            {
                var findImages = (from p in context.Post
                    join i in context.Image on p.Id equals i.PostId
                   
                    where p.UserId==userId
                    select new ProfilePostsDto
                    {
                        CommentCount = "2",
                        Description = p.Description,
                        Id = p.Id,
                        PictureUrl = i.ImageUrls,
                        RateNumber = "2",
                        Title = p.Title,
                        LikeCount = p.LikeCount.ToString()

                    });
                return findImages.ToList();
            }
            
        }

       

    }
}
