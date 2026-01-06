using Microsoft.AspNetCore.Mvc;
using TodoList.API.Controllers;
using Xunit;

namespace TodoList.API.Tests.Controllers;

public class MathControllerTests
{
    // SUT: System Under Test (o próprio controller)
    // Como não tem dependências, instanciamos direto aqui ou dentro de cada método.
    private readonly MathController _controller = new ();

    // ---------------------------------------------------------
    // SOMA
    // ---------------------------------------------------------
    [Theory(DisplayName = "Soma: Deve calcular corretamente valores positivos e negativos")]
    [InlineData(10, 20, 30)]   // 10 + 20 = 30
    [InlineData(-5, 5, 0)]     // -5 + 5 = 0
    [InlineData(1.5, 2.5, 4.0)]// 1.5 + 2.5 = 4
    public void Sum_ShouldReturnCorrectResult(double n1, double n2, double esperado)
    {
        // Act
        var resultado = _controller.Sum(n1, n2);

        // Assert
        // 1. Verificamos se o resultado é do tipo OkObjectResult (HTTP 200)
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        
        // 2. Verificamos se o valor dentro do Ok é o esperado
        Assert.Equal(esperado, okResult.Value);
    }

    // ---------------------------------------------------------
    // SUBTRAÇÃO
    // ---------------------------------------------------------
    [Theory(DisplayName = "Subtração: Deve calcular a diferença corretamente")]
    [InlineData(10, 5, 5)]
    [InlineData(0, 10, -10)]
    [InlineData(100, 100, 0)]
    public void Subtract_ShouldReturnCorrectResult(double n1, double n2, double esperado)
    {
        // Act
        var resultado = _controller.Subtract(n1, n2);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        Assert.Equal(esperado, okResult.Value);
    }

    // ---------------------------------------------------------
    // MULTIPLICAÇÃO
    // ---------------------------------------------------------
    [Theory(DisplayName = "Multiplicação: Deve calcular o produto corretamente")]
    [InlineData(5, 5, 25)]
    [InlineData(10, 0, 0)] // Qualquer coisa x 0 deve ser 0
    [InlineData(-2, 5, -10)]
    public void Multiply_ShouldReturnCorrectResult(double n1, double n2, double esperado)
    {
        // Act
        var resultado = _controller.Multiply(n1, n2);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        Assert.Equal(esperado, okResult.Value);
    }

    // ---------------------------------------------------------
    // DIVISÃO
    // ---------------------------------------------------------
    
    // Cenário 1: Caminho Feliz (Theory)
    [Theory(DisplayName = "Divisão: Deve calcular o quociente quando divisor não é zero")]
    [InlineData(10, 2, 5)]
    [InlineData(9, 3, 3)]
    [InlineData(5, 2, 2.5)]
    public void Divide_ShouldReturnCorrectResult(double n1, double n2, double esperado)
    {
        // Act
        var resultado = _controller.Divide(n1, n2);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        Assert.Equal(esperado, okResult.Value);
    }

    // Cenário 2: Caminho de Erro (Fact)
    // Usamos Fact pois é um caso único e específico que quebra a regra matemática
    [Fact(DisplayName = "Divisão: Deve retornar BadRequest (400) ao dividir por zero")]
    public void Divide_ShouldReturnBadRequest_WhenDivisorIsZero()
    {
        // Arrange
        double n1 = 10;
        double n2 = 0;

        // Act
        var resultado = _controller.Divide(n1, n2);

        // Assert
        // Esperamos um BadRequestObjectResult, não um OkObjectResult
        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado.Result);
        
        // Verificamos a mensagem de erro
        Assert.Equal("Não é possível dividir por zero.", badRequest.Value);
    }
}