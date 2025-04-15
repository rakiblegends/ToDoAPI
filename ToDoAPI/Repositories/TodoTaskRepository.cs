using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoAPI.Data;
using TodoAPI.Models;

namespace TodoAPI.Repositories
{
    public class TodoTaskRepository : ITodoTaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TodoTaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TodoTask>> GetAllTasksAsync()
        {
            return await _context.TodoTasks.ToListAsync();
        }

        public async Task<IEnumerable<TodoTask>> GetTasksByTitleAsync(string searchTerm)
        {
            return await _context.TodoTasks
                .Where(t => t.Title.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<IEnumerable<TodoTask>> GetTasksOrderedByPriorityAsync()
        {
            return await _context.TodoTasks
                .OrderByDescending(t => t.Priority)
                .ToListAsync();
        }

        public async Task<IEnumerable<TodoTask>> GetTasksOrderedByDueDateAsync()
        {
            return await _context.TodoTasks
                .Where(t => t.DueDate.HasValue)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<TodoTask> GetTaskByIdAsync(int id)
        {
            return await _context.TodoTasks.FindAsync(id);
        }

        public async Task<TodoTask> CreateTaskAsync(TodoTask todoTask)
        {
            todoTask.CreatedDate = DateTime.Now;
            _context.TodoTasks.Add(todoTask);
            await _context.SaveChangesAsync();
            return todoTask;
        }

        public async Task<TodoTask> UpdateTaskAsync(TodoTask todoTask)
        {
            _context.Entry(todoTask).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return todoTask;
        }

        public async Task DeleteTaskAsync(int id)
        {
            var todoTask = await _context.TodoTasks.FindAsync(id);
            if (todoTask != null)
            {
                _context.TodoTasks.Remove(todoTask);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<TodoTask>> GetPagedTasksAsync(int pageNumber, int pageSize)
        {
            return await _context.TodoTasks
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}