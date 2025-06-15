using Microsoft.EntityFrameworkCore;
using Models;

namespace DataAccess
{
    public class UsersContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UsersContext(DbContextOptions<UsersContext> opt) : base(opt) => Database.EnsureCreated();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Guid = Guid.NewGuid(),
                    Login = "admin",
                    Password = "admin",
                    Name = "Admin",
                    Gender = 2,
                    Admin = true,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = "system"
                }
            );
        }
    }
}
