using System.ComponentModel.DataAnnotations;

namespace TodoList.API.DTOs;

public class TodoItemDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }
}
