using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Chefbook.API.Models;
using Chefbook.Model.Models;

namespace Chefbook.API.Helpers
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Step, StepDTO>(); //burda aynı olanlar eşitlenir.
            //CreateMap<User, UserFollowDTO>().ForMember(dest => dest.FollowingId,
            //    opt => { opt.MapFrom(src => src.photos.firstordefault()); });
        }
    }
}
