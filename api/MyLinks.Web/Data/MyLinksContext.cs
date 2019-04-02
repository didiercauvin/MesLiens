using Microsoft.EntityFrameworkCore;
using MyLinks.Web.Models;

namespace MyLinks.Web.Data
{
    public class MyLinksContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Link> Links { get; set; }

        public MyLinksContext(DbContextOptions<MyLinksContext> options): base(options)
        {

        }
    }
}