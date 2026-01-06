using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TodoList.API.Controllers;
using TodoList.API.DTOs;
using TodoList.Infrastructure.Models;
using TodoList.Infrastructure.Services;
using TodoList.TestSupport.Fixtures;
using TodoList.TestSupport.Spies;

namespace TodoList.API.Tests.Controllers;

[Collection("ApiControllerCollection")]
public class TodoControllerTests
{
    private readonly ApiControllerFixture _fixture;
    
    // MOCK: Objeto falso que simula o serviço
    private readonly Mock<ITodoService> _serviceMock;
    
    // SPY: Logger real (em memória) para inspecionarmos mensagens
    private readonly SpyLogger<TodoController> _spyLogger;
    
    // SUT: System Under Test (O Controller real)
    private readonly TodoController _sut;

    public TodoControllerTests(ApiControllerFixture fixture)
    {
        _fixture = fixture;
        
        // 1. Criamos o Mock
        _serviceMock = new Mock<ITodoService>();
        
        // 2. Instanciamos o Spy
        _spyLogger = new SpyLogger<TodoController>();
        
        // 3. Injetamos as dependências falsas no Controller real
        _sut = new TodoController(_serviceMock.Object, _spyLogger); // aqui seria DTO
    }

    [Fact(DisplayName = "Create deve retornar 201 Created e Logar sucesso")]
    public async Task Create_ShouldReturnCreated_WhenDataIsValid()
    {
        // ARRANGE
        // Mudança 1: A entrada agora é um DTO
        var dtoEntrada = new TodoItemDto 
        { 
            Title = "Teste com DTO", 
            IsCompleted = false 
        };

        // O serviço continua retornando uma entidade de Domínio (TodoItem)
        var itemRetorno = new TodoItem 
        { 
            Id = Guid.NewGuid(), 
            Title = "Teste com DTO",
            IsCompleted = false
        };

        _serviceMock
            .Setup(s => s.AddAsync(It.IsAny<TodoItem>()))
            .ReturnsAsync(itemRetorno);

        // ACT
        // Mudança 2: Passamos o DTO para o método
        var resultado = await _sut.Create(dtoEntrada);

        // ASSERT
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(resultado);
        Assert.Equal(201, createdAtResult.StatusCode);
        
        var model = Assert.IsType<TodoItem>(createdAtResult.Value);
        Assert.Equal(itemRetorno.Id, model.Id);

        // Validando o Mapeamento:
        // Verificamos se o serviço foi chamado com um TodoItem que tem o mesmo título do DTO.
        // Isso garante que o Controller fez o "de-para" corretamente.
        _serviceMock.Verify(s => s.AddAsync(It.Is<TodoItem>(i => i.Title == dtoEntrada.Title)), Times.Once);

        Assert.Contains(_spyLogger.Logs, l => 
            l.Level == LogLevel.Information && 
            l.Message.Contains(itemRetorno.Id.ToString()));
    }

    [Fact(DisplayName = "Create deve retornar 400 BadRequest quando Serviço lança Exceção")]
    public async Task Create_ShouldReturnBadRequest_WhenServiceFails()
    {
        // ARRANGE
        // Mudança 1: Entrada é DTO inválido (regra de negócio simulada)
        var dtoInvalido = new TodoItemDto { Title = "" }; 
        var mensagemErro = "O título da tarefa não pode ser vazio.";

        _serviceMock
            .Setup(s => s.AddAsync(It.IsAny<TodoItem>()))
            .ThrowsAsync(new ArgumentException(mensagemErro));

        // ACT
        // Mudança 2: Passamos o DTO
        var resultado = await _sut.Create(dtoInvalido);

        // ASSERT
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Equal(mensagemErro, badRequestResult.Value);

        Assert.Contains(_spyLogger.Logs, l => 
            l.Level == LogLevel.Warning && 
            l.Message.Contains("Tentativa de criar item inválido"));
    }
    
    // Teste Extra (Bônus): Update com DTO
    [Fact(DisplayName = "Update deve retornar 204 NoContent quando sucesso")]
    public async Task Update_ShouldReturnNoContent_WhenSuccess()
    {
        // ARRANGE
        var id = Guid.NewGuid();
        var dtoUpdate = new TodoItemDto { Title = "Update DTO", IsCompleted = true };

        _serviceMock
            .Setup(s => s.UpdateAsync(id, It.IsAny<TodoItem>()))
            .ReturnsAsync(true);

        // ACT
        var resultado = await _sut.Update(id, dtoUpdate);

        // ASSERT
        var noContentResult = Assert.IsType<NoContentResult>(resultado);
        Assert.Equal(204, noContentResult.StatusCode);

        // Verifica se o mapeamento ocorreu e o ID correto foi passado
        _serviceMock.Verify(s => s.UpdateAsync(id, It.Is<TodoItem>(x => x.Title == dtoUpdate.Title)), Times.Once);
    }
}