﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chefbook.API.Repository;
using Chefbook.Model.Models;

namespace Chefbook.API.Services.Interface
{
    public interface IStepService : IGenericRepository<Step>
    {
        void AddRange(List<Step> steps);

    }
}
