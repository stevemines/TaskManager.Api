using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.Models
{
    public class TaskCreateUpdateDto
    {
        [Required]
        public string Title { get; set; } // Required

        [Required]
        public string? Description { get; set; } // Required

    }
}
