using Microsoft.EntityFrameworkCore;
using SerMis.Models;

namespace SerMis.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<PeopleModel> People { get; set; }
    }
}
