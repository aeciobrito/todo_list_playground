using System;
using TodoList.TestSupport.Fixtures;

namespace TodoList.Infrastructure.Tests.Collections;

// Esta classe serve apenas como "ponto de definição" para o xUnit.
// Ela não tem código, apenas as interfaces de marcação.
[CollectionDefinition("InfrastructureCollection")]
public class InfrastructureCollection : ICollectionFixture<TodoInMemoryFixture>
{
}
