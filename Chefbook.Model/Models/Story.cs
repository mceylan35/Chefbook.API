using System;
using System.Collections.Generic;
using System.Text;

namespace Chefbook.Model.Models
{
    public class Story:IEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string StoryUrl { get; set; }
        public string PublicId { get; set; }
        public bool IsActive { get; set; }
    }
}
