using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chefbook.API.Context;
using Chefbook.API.Models;
using Chefbook.API.Repository;
using Chefbook.API.Services.Interface;
using Chefbook.Model.Models;
using Microsoft.EntityFrameworkCore.Query.Expressions;

namespace Chefbook.API.Services.Service
{
    public class StoryService : GenericRepository<Story, ChefContext>, IStoryService
    {
        public void AddRange(List<Story> stories)
        {
            using (var context=new ChefContext())
            {
                context.Story.AddRange(stories);
            }
        }

        public List<StoryViewModel> Stories(Guid Id)
        {
            using (var context=new ChefContext())
            {
                var storylist = from s in context.Story
                    join u in context.User on s.UserId equals u.Id
                    join f in context.Follow on Id equals f.FollowingId
                    group new { s, u } by new {u.UserName,s.Id,s.CreatedDate,u.ProfileImage,s.StoryUrl } into g

                    select new StoryModel
                    {
                        StoryId = g.Key.Id,
                        CreatedDate = g.Key.CreatedDate,
                        StoryUrl =g.Key.StoryUrl,
                        UserName = g.Key.UserName,
                        ProfilePicture = g.Key.ProfileImage
                        
                    };
                var stories = storylist.ToList();

                List<StoryViewModel> storyViewModels=new List<StoryViewModel>();

                foreach (var storyModel in stories.ToList())
                {
                    var nesne = storyViewModels.Where(i => i.UserName == storyModel.UserName).ToList();
                    if (nesne.Count ==0 )
                    {
                        storyViewModels.Add(new StoryViewModel
                        {
                            UserName = storyModel.UserName,
                            UserProfilePicture = storyModel.ProfilePicture
                        });
                    }
                   
                }

                foreach (var stlist in storyViewModels)
                {

                    var nesnem = stories.Where(i => i.UserName == stlist.UserName).ToList().OrderByDescending(i=>i.CreatedDate);
                    List<StoryMap> storyMaps=new List<StoryMap>();
                    foreach (var storyModel in nesnem)
                    {
                     storyMaps.Add(new StoryMap
                     {
                         CreatedDate = storyModel.CreatedDate,
                         StoryId = storyModel.StoryId,
                         StoryUrl = storyModel.StoryUrl
                     });
                        
                    }
                    stlist.Stories = storyMaps.ToList();



                }


                return storyViewModels;
            }

           
        }
    }
}
