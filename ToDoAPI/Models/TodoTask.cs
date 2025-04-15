using System;
using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Models
{
    public class TodoTask
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? DueDate { get; set; }

        [Range(1, 5)]
        public int Priority { get; set; } // 1 = Low, 5 = High
    }
}