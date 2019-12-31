﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using Chefbook.Model;

namespace Chefbook.Model.Models
{
    public partial class Post : IEntity
    {
        public Post()
        {
            Like = new HashSet<Like>();
            Step = new HashSet<Step>();
            Sticker = new HashSet<Sticker>();
        }

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime? PostDate { get; set; }
        public string Description { get; set; }
        public long? LikeCount { get; set; }
        public string Title { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Like> Like { get; set; }
        public virtual ICollection<Step> Step { get; set; }
        public virtual ICollection<Sticker> Sticker { get; set; }
    }
}