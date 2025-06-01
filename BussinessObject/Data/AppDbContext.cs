using BussinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=TUYETCHINH;Database=TelecomStore;User ID=sa;Password=123456;Trust Server Certificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.UserID);
            modelBuilder.Entity<Category>().HasKey(c => c.CategoryID);
            modelBuilder.Entity<Product>().HasKey(p => p.ProductID);
            modelBuilder.Entity<Cart>().HasKey(c => c.CartID);
            modelBuilder.Entity<Order>().HasKey(o => o.OrderID);
            modelBuilder.Entity<OrderDetail>().HasKey(od => od.OrderDetailID);
            modelBuilder.Entity<Coupon>().HasKey(c => c.CouponID);
            modelBuilder.Entity<Review>().HasKey(r => r.ReviewID);
            modelBuilder.Entity<Feedback>().HasKey(f => f.FeedbackID);
        }
    }
}
