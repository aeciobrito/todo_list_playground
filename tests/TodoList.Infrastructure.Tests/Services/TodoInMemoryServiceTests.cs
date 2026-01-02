using TodoList.Infrastructure.Models;
using TodoList.Infrastructure.Services;
using TodoList.TestSupport.Fixtures;
using Xunit;

namespace TodoList.Infrastructure.Tests.Services;

// A marcação [Collection] vincula este teste à fixture definida anteriormente
[Collection("InfrastructureCollection")]
public class TodoInMemoryServiceTests
{
    private readonly TodoInMemoryFixture _fixture;
    private readonly TodoInMemoryService _sut; // SUT = System Under Test

    public TodoInMemoryServiceTests(TodoInMemoryFixture fixture)
    {
        _fixture = fixture;
        // Como o serviço não tem dependências (não usa Logger ou DbContext real),
        // instanciamos diretamente. Se tivesse, usaríamos Mocks aqui.
        _sut = new TodoInMemoryService();
    }

    [Fact(DisplayName = "AddAsync deve criar item quando dados são válidos")]
    public async Task AddAsync_ShouldCreateItem_WhenDataIsValid()
    {
        // ARRANGE
        // Preparamos o objeto usando dados que fazem sentido para o teste
        var novoItem = new TodoItem 
        { 
            Title = "Aprender Testes Unitários", 
            IsCompleted = false 
        };

        // ACT
        // Executamos a ação principal que queremos validar
        var resultado = await _sut.AddAsync(novoItem);

        // ASSERT
        // Validamos o retorno e o "efeito colateral" (persistência)
        Assert.NotNull(resultado);
        Assert.NotEqual(Guid.Empty, resultado.Id); // Garante que gerou ID
        Assert.Equal("Aprender Testes Unitários", resultado.Title);

        // Verificação dupla: busca no "banco" para ver se salvou mesmo
        var itemNoBanco = await _sut.GetByIdAsync(resultado.Id);
        Assert.NotNull(itemNoBanco);
    }

    [Fact(DisplayName = "AddAsync deve lançar exceção quando título é vazio")]
    public async Task AddAsync_ShouldThrowException_WhenTitleIsEmpty()
    {
        // ARRANGE
        var itemInvalido = new TodoItem 
        { 
            Title = "", // Título inválido propositalmente
            IsCompleted = false 
        };

        // ACT & ASSERT
        // No xUnit, quando testamos exceções, o Act e Assert acontecem juntos nesta sintaxe
        var excecao = await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddAsync(itemInvalido));

        // Validamos se a mensagem de erro é a esperada (importante para não confundir erros)
        Assert.Equal("O título da tarefa não pode ser vazio.", excecao.Message);
    }

    [Fact(DisplayName = "UpdateAsync deve atualizar status quando item existe")]
    public async Task UpdateAsync_ShouldUpdateStatus_WhenItemExists()
    {
        // ARRANGE
        // Primeiro precisamos garantir que algo existe para ser atualizado.
        // Criamos um item inicial.
        var itemInicial = await _sut.AddAsync(new TodoItem { Title = "Tarefa Antiga" });
        
        // Modificamos o objeto para o update
        itemInicial.IsCompleted = true; 
        itemInicial.Title = "Tarefa Atualizada";

        // ACT
        var sucesso = await _sut.UpdateAsync(itemInicial.Id, itemInicial);

        // ASSERT
        Assert.True(sucesso); // O método retorna boolean

        // Validação de estado
        var itemAtualizado = await _sut.GetByIdAsync(itemInicial.Id);
        Assert.True(itemAtualizado?.IsCompleted);
        Assert.Equal("Tarefa Atualizada", itemAtualizado?.Title);
    }
}