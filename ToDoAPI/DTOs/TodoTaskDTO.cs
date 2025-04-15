using System;
using System.ComponentModel.DataAnnotations;

namespace TodoAPI.DTOs
{
    public class TodoTaskDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? DueDate { get; set; }

        [Range(1, 5)]
        public int Priority { get; set; }
    }

    public class CreateTodoTaskDTO
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? DueDate { get; set; }

        [Range(1, 5)]
        public int Priority { get; set; } = 1;
    }

    public class UpdateTodoTaskDTO
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? DueDate { get; set; }

        [Range(1, 5)]
        public int Priority { get; set; }
    }
}