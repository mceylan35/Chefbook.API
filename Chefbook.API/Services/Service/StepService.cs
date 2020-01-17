using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chefbook.API.Context;
using Chefbook.API.Repository;
using Chefbook.API.Services.Interface;
using Chefbook.API.Services.RepositoryInterfaces;
using Chefbook.Model.Models;

namespace Chefbook.API.Services.Service
{
    public class StepService : GenericRepository<Step, ChefContext>, IStepService
    {
        public async void AddRange(List<Step> steps)
        {
            using (var context=new ChefContext())
            {
                context.Step.AddRange(steps);
                
              await context.SaveChangesAsync();
            }
        }
    }
}
