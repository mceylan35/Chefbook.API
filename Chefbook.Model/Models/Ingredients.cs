using System;
using System.Collections.Generic;
using System.Text;

namespace Chefbook.Model.Models
{
    public class Ingredients:IEntity
    {
        public Guid Id { get; set; }
        public Guid? PostId { get; set; }
        public string Ingredient { get; set; }
    }
}
