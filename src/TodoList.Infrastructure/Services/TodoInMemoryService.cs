using System;
using TodoList.Infrastructure.Models;

namespace TodoList.Infrastructure.Services;

public class TodoInMemoryService : ITodoService
{
    private static readonly List<TodoItem> _database = new();

    public Task<TodoItem> AddAsync(TodoItem item)
    {
        if(string.IsNullOrWhiteSpace(item.Title))
            throw new ArgumentNullException("O título da tarefa não pode ser vazio.");
        // ArgumentException

        item.Id = Guid.NewGuid();
        item.CreatedAt = DateTime.Now;

        _database.Add(item);
        return Task.FromResult(item);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var item = _database.FirstOrDefault(x => x.Id == id);
        if (item == null) return Task.FromResult(false);

        _database.Remove(item);
        return Task.FromResult(true);
    }

    public Task<IEnumerable<TodoItem>> GetAllAsync() 
        => Task.FromResult<IEnumerable<TodoItem>>(_database);

    public Task<TodoItem?> GetByIdAsync(Guid id) 
        => Task.FromResult(_database.FirstOrDefault(x => x.Id == id));    

    public Task<bool> UpdateAsync(Guid id, TodoItem item)
    {
        var existingItem = _database.FirstOrDefault(x => x.Id == id);
        if (existingItem == null) return Task.FromResult(false);

        if (string.IsNullOrWhiteSpace(item.Title))
            throw new ArgumentException("O título da tarefa não pode ser vazio.");

        existingItem.Title = item.Title;
        existingItem.IsCompleted = item.IsCompleted;

        return Task.FromResult(true);
    }
}
