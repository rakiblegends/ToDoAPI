using System.Collections.Generic;
using System.Threading.Tasks;
using TodoAPI.Models;

namespace TodoAPI.Repositories
{
    public interface ITodoTaskRepository
    {
        Task<IEnumerable<TodoTask>> GetAllTasksAsync();
        Task<IEnumerable<TodoTask>> GetTasksByTitleAsync(string searchTerm);
        Task<IEnumerable<TodoTask>> GetTasksOrderedByPriorityAsync();
        Task<IEnumerable<TodoTask>> GetTasksOrderedByDueDateAsync();
        Task<TodoTask> GetTaskByIdAsync(int id);
        Task<TodoTask> CreateTaskAsync(TodoTask todoTask);
        Task<TodoTask> UpdateTaskAsync(TodoTask todoTask);
        Task DeleteTaskAsync(int id);
        Task<IEnumerable<TodoTask>> GetPagedTasksAsync(int pageNumber, int pageSize);
    }
}