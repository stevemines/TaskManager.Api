using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.Models
{
    public class TaskItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); // Unique identifier

        [Required]
        public string Title { get; set; } // Required

        public string? Description { get; set; } // Optional

        [Required]
        public TaskStatus Status { get; set; } = TaskStatus.Pending; // Default: Pending

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; } = null;
    }
}
