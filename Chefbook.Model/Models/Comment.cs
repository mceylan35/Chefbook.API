﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using Chefbook.Model;

namespace Chefbook.Model.Models
{
    public partial class Comment:IEntity
    {
        public Guid Id { get; set; }
        public Guid? PostId { get; set; }
        public string Description { get; set; }
        public DateTime? CommentDate { get; set; }
        public long? LikeCount { get; set; }
        public Guid? UserId { get; set; }

        public virtual User User { get; set; }
    }
}