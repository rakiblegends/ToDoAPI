using Microsoft.EntityFrameworkCore;
using TodoAPI.Models;

namespace TodoAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TodoTask> TodoTasks { get; set; }
        // In ApplicationDbContext.cs OnModelCreating method
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TodoTask>()
                .HasIndex(t => t.Title);

            modelBuilder.Entity<TodoTask>()
                .HasIndex(t => t.Priority);

            modelBuilder.Entity<TodoTask>()
                .HasIndex(t => t.DueDate);
        }
    }
}