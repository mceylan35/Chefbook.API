using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chefbook.Model;
using Chefbook.Model.Models;
using Microsoft.EntityFrameworkCore;


namespace Chefbook.API.Context
{
    public partial class ChefContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=tcp:bankappim.database.windows.net,1433;Initial Catalog=Chefbook;Persist Security Info=False;User ID=bankappim;Password=657193960Amc;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        }


        public ChefContext()
        {
        }

        public ChefContext(DbContextOptions<ChefContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Follow> Follow { get; set; }
        public virtual DbSet<Image> Image { get; set; }
        public virtual DbSet<Like> Like { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<Post> Post { get; set; }
        public virtual DbSet<Step> Step { get; set; }
        public virtual DbSet<Sticker> Sticker { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Star> Star { get; set; }

     
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CommentDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Comment_User");
            });

            modelBuilder.Entity<Follow>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FollowDate).HasColumnType("datetime");

                entity.HasOne(d => d.Followers)
                    .WithMany(p => p.FollowFollowers)
                    .HasForeignKey(d => d.FollowersId)
                    .HasConstraintName("FK_Follow_User1");

                entity.HasOne(d => d.Following)
                    .WithMany(p => p.FollowFollowing)
                    .HasForeignKey(d => d.FollowingId)
                    .HasConstraintName("FK_Follow_User");
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Like>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Begenen)
                    .WithMany(p => p.Like)
                    .HasForeignKey(d => d.BegenenId)
                    .HasConstraintName("FK_Like_User");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Like)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("FK_Like_Post");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ImageUrl).HasMaxLength(10);

                entity.Property(e => e.NotificationDate).HasColumnType("datetime");

                entity.Property(e => e.NotificationDescription).HasMaxLength(50);
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description).HasMaxLength(150);

                entity.Property(e => e.PostDate).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Post)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Post_User1");
            });

            modelBuilder.Entity<Step>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Step)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("FK_Step_Post");
            });

            modelBuilder.Entity<Sticker>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Sticker)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("FK_Sticker_Post");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Birthday).HasColumnType("date");

                entity.Property(e => e.RegisterDate).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);


    }
}
