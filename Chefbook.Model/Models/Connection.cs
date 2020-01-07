using System;
using System.Collections.Generic;
using System.Text;

namespace Chefbook.Model.Models
{
    public class Connection:IEntity
    {
        public Guid Id { get; set; }
        public string ConnectionId { get; set; }
        public Guid UserId { get; set; }
        public bool Connected { get; set; }
    }
}
