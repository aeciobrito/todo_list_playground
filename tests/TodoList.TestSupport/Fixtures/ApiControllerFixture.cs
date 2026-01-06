using System;

namespace TodoList.TestSupport.Fixtures;

public class ApiControllerFixture : IDisposable
{
    // Em um projeto real, aqui configuraríamos ClaimsPrincipal (Auth fake),
    // HttpContext ou configurações de appsettings.json para testes de integração.
    
    public ApiControllerFixture()
    {
        // Setup global para controllers (se houver)
    }

    public void Dispose()
    {
        // Limpeza
    }
}
