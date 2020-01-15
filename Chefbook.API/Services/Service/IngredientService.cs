using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chefbook.API.Context;
using Chefbook.API.Repository;
using Chefbook.API.Services.Interface;
using Chefbook.Model.Models;

namespace Chefbook.API.Services.Service
{
    public class IngredientService:GenericRepository<Ingredients,ChefContext>,IIngredientService
    {
        public void AddRange(List<Ingredients> ingredientses)
        {
            using (var context = new ChefContext())
            {
                context.Ingredients.AddRange(ingredientses);
                context.SaveChanges();


            }
        }
    }
}
