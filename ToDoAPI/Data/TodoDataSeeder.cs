using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using ToDoAPI.Data;
using TodoAPI.Data;
using TodoAPI.Models;

namespace ToDoAPI.Data
{
    public static class TodoDataSeeder
    {
        public static void Initialize(IServiceProvider serviceProvider, int count = 100000)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            // Check if we already have todos
            //if (context.TodoTasks.Any())
            //{
            //    return; // Data was already seeded
            //}

            Console.WriteLine($"Starting to seed {count} todo items...");
            var startTime = DateTime.Now;

            // Create batches for better performance (5,000 records per batch)
            const int batchSize = 5000;
            var totalBatches = (int)Math.Ceiling(count / (double)batchSize);

            for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
            {
                var todos = new List<TodoTask>();
                var currentBatchSize = Math.Min(batchSize, count - (batchIndex * batchSize));

                for (int i = 0; i < currentBatchSize; i++)
                {
                    int todoNumber = batchIndex * batchSize + i + 1;

                    todos.Add(new TodoTask
                    {
                        Title = $"Todo {todoNumber}",
                        Description = $"This is the description for todo #{todoNumber}",
                        IsCompleted = todoNumber % 3 == 0, // Every third item is completed
                        Priority = todoNumber % 5 + 1, // Priority between 1-5
                        DueDate = DateTime.Now.AddDays(todoNumber % 30), // Due within next 30 days
                    });
                }

                context.TodoTasks.AddRange(todos);
                context.SaveChanges();

                Console.WriteLine($"Batch {batchIndex + 1}/{totalBatches} completed. Added {todos.Count} records.");
            }

            var endTime = DateTime.Now;
            Console.WriteLine($"Seeding completed! Added {count} todos in {(endTime - startTime).TotalSeconds} seconds.");
        }
    }
}