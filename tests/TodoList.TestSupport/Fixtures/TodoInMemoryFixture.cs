using System;
using TodoList.Infrastructure.Models;

namespace TodoList.TestSupport.Fixtures;

public class TodoInMemoryFixture : IDisposable
{
    public List<TodoItem> DefaultItems { get; private set; }

    public TodoInMemoryFixture()
    {
        DefaultItems = new List<TodoItem>
        {
            new() { Title = "Task Global 1", IsCompleted = false },
            new() { Title = "Task Global 2", IsCompleted = true }
        };
    }

    public void Dispose()
    {
        DefaultItems.Clear();
    }
}
