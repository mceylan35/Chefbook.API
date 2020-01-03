﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chefbook.API.Context;
using Chefbook.API.Repository;
using Chefbook.API.Services.Interface;
using Chefbook.Model.Models;

namespace Chefbook.API.Services.Service
{
    public class StarService:GenericRepository<Star,ChefContext>,IStarService
    {

    }
}
