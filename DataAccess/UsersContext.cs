using Microsoft.EntityFrameworkCore;
using Models;

namespace DataAccess
{
    public class UsersContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UsersContext() : base() => Database.EnsureCreated();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasAlternateKey(u => u.Login);
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Login = "admin",
                    Password = "admin",
                    Name = "Admin",
                    Gender = 2,
                    Admin = true,
                    CreatedOn = DateTime.Now,
                    CreatedBy = "system"
                }
            );
        }
    }
}
