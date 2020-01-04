using System;
using System.Collections.Generic;
using System.Text;

namespace Chefbook.Model.Models
{
    public class Star:IEntity
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? PostId { get; set; }
        public byte? RateNumber { get; set; }

    }
}
