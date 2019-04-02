using Microsoft.EntityFrameworkCore;
using MyLinks.Web.Models;
using System.Collections.Generic;

namespace MyLinks.Web.Data
{
    public class MyLinksContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Link> Links { get; set; }

        public MyLinksContext(DbContextOptions<MyLinksContext> options): base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(new Category
            {
                Id = 1,
                Name = "Tech"
            });

            modelBuilder.Entity<Link>().HasData(new
            {
                Id = 1,
                CategoryId = 1,
                Url = "http://uneurldetest.com"
            });

        }
    }
}