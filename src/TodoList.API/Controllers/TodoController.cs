using Microsoft.AspNetCore.Mvc;
using TodoList.API.DTOs;
using TodoList.Infrastructure.Models;
using TodoList.Infrastructure.Services;

namespace TodoList.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
    private readonly ITodoService _service;
    private readonly ILogger<TodoController> _logger;

    public TodoController(ITodoService service, ILogger<TodoController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _service.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TodoItemDto dto)
    {
        try 
        {
            var item = new TodoItem
            {
                Title = dto.Title,
                IsCompleted = dto.IsCompleted
            };

            var createdItem = await _service.AddAsync(item);
            _logger.LogInformation("Item criado com ID: {Id}", createdItem.Id);
            return CreatedAtAction(nameof(GetById), new { id = createdItem.Id }, createdItem);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Tentativa de criar item inválido: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TodoItemDto dto)
    {
        try
        {
            var item = new TodoItem
            {
                Title = dto.Title,
                IsCompleted = dto.IsCompleted
            };

            var updated = await _service.UpdateAsync(id, item);
            if (!updated) return NotFound();
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        
        return NoContent();
    }
}