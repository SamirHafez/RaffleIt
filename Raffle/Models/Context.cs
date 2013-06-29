using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Raffle.Models
{
    public class Context : DbContext
    {
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserProfile>().HasMany(up => up.Raffles).WithRequired(r => r.UserProfile);
            modelBuilder.Entity<UserProfile>().HasMany(up => up.Items).WithRequired(i => i.Owner).WillCascadeOnDelete(false);

            modelBuilder.Entity<Item>().HasMany(i => i.Raffles).WithRequired(r => r.Item);

            base.OnModelCreating(modelBuilder);
        }
    }
}