using Common.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Common.Persistence
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }

    [Obsolete]
    public class DataGenerator
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationContext>>());
            if (context.Users.Any())
            {
                return;
            }

            context.Users.AddRange(
                    new User { Id = 1, Email = "Test1@gmail.com", PasswordHash = "test" },
                    new User { Id = 2, Email = "Test2@gmail.com", PasswordHash = "test" },
                    new User { Id = 3, Email = "Test3@gmail.com", PasswordHash = "test" },
                    new User { Id = 4, Email = "Test4@gmail.com", PasswordHash = "test" });

            context.SaveChanges();
        }

    }
}
