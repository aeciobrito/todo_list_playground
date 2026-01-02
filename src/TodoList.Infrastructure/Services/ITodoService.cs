using System;
using TodoList.Infrastructure.Models;

namespace TodoList.Infrastructure.Services;

public interface ITodoService
{
    Task<IEnumerable<TodoItem>> GetAllAsync();
    Task<TodoItem?> GetByIdAsync(Guid id);
    Task<TodoItem> AddAsync(TodoItem item);
    Task<bool> UpdateAsync (Guid id, TodoItem item);
    Task<bool> DeleteAsync(Guid id);
}
