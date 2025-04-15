using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoAPI.DTOs;
using TodoAPI.Models;
using TodoAPI.Repositories;

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoTasksController : ControllerBase
    {
        private readonly ITodoTaskRepository _repository;

        public TodoTasksController(ITodoTaskRepository repository)
        {
            _repository = repository;
        }

        // GET: api/TodoTasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoTaskDTO>>> GetTasks()
        {
            var tasks = await _repository.GetAllTasksAsync();
            var taskDtos = tasks.Select(MapToDto).ToList();
            return Ok(taskDtos);
        }

        // GET: api/TodoTasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoTaskDTO>> GetTask(int id)
        {
            var task = await _repository.GetTaskByIdAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            return MapToDto(task);
        }

        // GET: api/TodoTasks/search?title=meeting
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TodoTaskDTO>>> SearchTasks([FromQuery] string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return BadRequest("Search term cannot be empty");
            }

            var tasks = await _repository.GetTasksByTitleAsync(title);
            var taskDtos = tasks.Select(MapToDto).ToList();
            return Ok(taskDtos);
        }

        // GET: api/TodoTasks/priority
        [HttpGet("priority")]
        public async Task<ActionResult<IEnumerable<TodoTaskDTO>>> GetTasksByPriority()
        {
            var tasks = await _repository.GetTasksOrderedByPriorityAsync();
            var taskDtos = tasks.Select(MapToDto).ToList();
            return Ok(taskDtos);
        }

        // GET: api/TodoTasks/duedate
        [HttpGet("duedate")]
        public async Task<ActionResult<IEnumerable<TodoTaskDTO>>> GetTasksByDueDate()
        {
            var tasks = await _repository.GetTasksOrderedByDueDateAsync();
            var taskDtos = tasks.Select(MapToDto).ToList();
            return Ok(taskDtos);
        }

        // POST: api/TodoTasks
        [HttpPost]
        public async Task<ActionResult<TodoTaskDTO>> CreateTask(CreateTodoTaskDTO createDto)
        {
            var task = new TodoTask
            {
                Title = createDto.Title,
                Description = createDto.Description,
                IsCompleted = createDto.IsCompleted,
                DueDate = createDto.DueDate,
                Priority = createDto.Priority
                // CreatedDate is set in the repository
            };

            var createdTask = await _repository.CreateTaskAsync(task);

            return CreatedAtAction(
                nameof(GetTask),
                new { id = createdTask.Id },
                MapToDto(createdTask));
        }

        // PUT: api/TodoTasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, UpdateTodoTaskDTO updateDto)
        {
            var existingTask = await _repository.GetTaskByIdAsync(id);

            if (existingTask == null)
            {
                return NotFound();
            }

            // Update properties
            existingTask.Title = updateDto.Title;
            existingTask.Description = updateDto.Description;
            existingTask.IsCompleted = updateDto.IsCompleted;
            existingTask.DueDate = updateDto.DueDate;
            existingTask.Priority = updateDto.Priority;

            await _repository.UpdateTaskAsync(existingTask);

            return NoContent();
        }

        // DELETE: api/TodoTasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _repository.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            await _repository.DeleteTaskAsync(id);

            return NoContent();
        }

        // Helper method to map from entity to DTO
        private static TodoTaskDTO MapToDto(TodoTask task)
        {
            return new TodoTaskDTO
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                CreatedDate = task.CreatedDate,
                DueDate = task.DueDate,
                Priority = task.Priority
            };
        }

        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<TodoTaskDTO>>> GetPagedTasks(
    [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var tasks = await _repository.GetPagedTasksAsync(pageNumber, pageSize);
            var taskDtos = tasks.Select(MapToDto).ToList();
            return Ok(taskDtos);
        }
    }
}