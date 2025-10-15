using JwtInfrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace JwtInfrastructure.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {
        }

        public DbSet<LoginModel>? LoginModels { get; set; }

        public DbSet<Product>? Products { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoginModel>().HasData(new LoginModel
            {
                Id = 1,
                UserName = "johndoe",
                Email="mohammedfathi0810@gmail.com",
                Password = "def@123",
                Role = "Admin",

            });
        }
    }
}
