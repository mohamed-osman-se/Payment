using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Payment.Models;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> users { get; set; }

        public DbSet<Product> products { get; set; }

        public DbSet<Order> orders { get; set; }

        public DbSet<StripeEventLog> StripeEvents => Set<StripeEventLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<StripeEventLog>()
                .HasIndex(x => x.EventId)
                .IsUnique();
        }


    }
}