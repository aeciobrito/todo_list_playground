# TodoList Playground - Guia de Arquitetura e Testes

Este projeto é um ambiente controlado (**Playground**) desenvolvido em **.NET 7**. O objetivo é servir como referência prática para a implementação de testes unitários e de integração, isolando a complexidade das regras de negócio reais para focar nos conceitos e padrões de qualidade.

## 1. Visão Geral da Solução

A solução segue uma arquitetura em camadas simplificada, separando responsabilidades de apresentação (API) e lógica de persistência (Infrastructure), com uma estratégia robusta de testes espelhada.

```text
TodoList.Playground/
├── src/
│   ├── TodoList.API/            # Entrada HTTP, Controllers e DTOs
│   └── TodoList.Infrastructure/ # Implementação de Serviços e Banco em Memória
│
└── tests/
    ├── TodoList.API.Tests/            # Testes de Unidade dos Controllers
    ├── TodoList.Infrastructure.Tests/ # Testes de Unidade/Estado dos Serviços
    └── TodoList.TestSupport/          # Biblioteca compartilhada (Fixtures, Spies, Mocks)

```

---

## 2. Decisões de Arquitetura

### 2.1 Camada de Infraestrutura (Infrastructure)

* **Persistência em Memória:** Para simplificar o playground e evitar dependências de Docker ou bancos locais, utilizamos uma implementação `static List<TodoItem>` dentro do `TodoInMemoryService`.
* *Vantagem:* Testes extremamente rápidos e zero configuração de ambiente.


* **Contrato via Interface:** Todo o acesso a dados é feito via `ITodoService`. Isso é crucial para permitir o uso de **Mocks** na camada da API.

### 2.2 Camada de API

* **Uso de DTOs (Data Transfer Objects):** A API não expõe diretamente as entidades de domínio no `POST` ou `PUT`. Usamos `TodoItemDto` para receber dados.
* *Objetivo:* Ensinar como testar o mapeamento (De/Para) dentro dos controllers.


* **Tratamento de Erros:** O Controller captura exceções de domínio (`ArgumentException`) e as converte em respostas HTTP adequadas (`400 Bad Request`), evitando que erros internos vazem como `500 Internal Server Error`.

---

## 3. Estratégia de Testes

Utilizamos o framework **xUnit** combinado com **Moq**. A estrutura dos testes segue rigorosamente o padrão **AAA**.

### 3.1 O Padrão AAA

Todo teste deve ser legível como uma história com três atos:

1. **Arrange (Preparar):** Configura variáveis, mocks e o estado inicial.
2. **Act (Agir):** Executa o método que está sendo testado.
3. **Assert (Validar):** Verifica o retorno e os efeitos colaterais.

### 3.2 Projeto Compartilhado (`TestSupport`)

Para evitar duplicação de código (DRY), criamos um projeto dedicado a utilitários de teste.

* **Fixtures:** Configurações globais reutilizáveis (ex: `TodoInMemoryFixture`).
* **Spies:** Implementações manuais para capturar efeitos colaterais difíceis de "mockar", como Logs.

---

## 4. Conceitos Implementados

### 4.1 Collections e Fixtures (Contexto Compartilhado)

No xUnit, testes em classes diferentes rodam em paralelo e isolados. Quando precisamos compartilhar um contexto (como nosso "banco em memória"), usamos **Collections**.

* **Implementação:** `InfrastructureCollection` define o escopo.
* **Uso:** A classe de teste recebe `[Collection("InfrastructureCollection")]` e injeta a `TodoInMemoryFixture` no construtor.
* *Benefício:* Garante que o ambiente seja preparado uma única vez (ou controlado) para um grupo de testes.

### 4.2 Spies (Espiões) vs. Mocks

Embora usemos o **Moq** para serviços, utilizamos um **Spy** para o `ILogger`.

* **O Problema:** Mockar `ILogger` com Moq é verboso porque a maioria dos métodos de log são *extension methods* estáticos.
* **A Solução (`SpyLogger`):** Uma classe real em `TestSupport` que implementa `ILogger` mas apenas guarda as mensagens em uma `List<LogEntry>`.
* **Assert:** Nos testes, verificamos se a lista contém a mensagem esperada.

### 4.3 Mocks e Verificação de Comportamento

Nos testes da API (`TodoList.API.Tests`), não queremos testar o serviço (já testado na infra), mas sim se o Controller **conversa** corretamente com ele.

Utilizamos o **Moq** para:

1. **Stubbing (Setup):** Ensinar o mock a retornar um valor específico (`ReturnsAsync`) ou lançar um erro (`ThrowsAsync`).
2. **Interaction Testing (Verify):** Garantir que o serviço foi chamado com os parâmetros corretos.

**Exemplo de validação de DTO:**

```csharp
// Verifica se o Controller converteu corretamente o DTO para a Entidade
_serviceMock.Verify(s => s.AddAsync(It.Is<TodoItem>(i => i.Title == dto.Title)), Times.Once);

```

---

## 5. Guia dos Testes Implementados

### 5.1 Testes de Infraestrutura (`TodoInMemoryServiceTests`)

Focam em **Estado**.

* **Cenário de Sucesso:** Adiciona um item e verifica se ele foi salvo na lista estática.
* **Cenário de Exceção:** Tenta adicionar um item com título vazio e valida se a exceção `ArgumentException` é lançada com a mensagem correta.

### 5.2 Testes de API (`TodoControllerTests`)

Focam em **Comportamento e Contratos HTTP**.

* **Sucesso (201 Created):**
* Configura o Mock para retornar sucesso.
* Verifica se o Status Code é 201.
* Verifica se o `SpyLogger` registrou a operação.


* **Erro de Negócio (400 Bad Request):**
* Configura o Mock para lançar `ArgumentException`.
* Verifica se o Controller capturou o erro e retornou 400 (e não 500).



---

## 6. Como Executar

Para rodar a suíte completa de testes e garantir que não houve regressão:

```bash
# Na raiz da solução
dotnet test

```

Para rodar apenas um projeto específico (ex: API):

```bash
dotnet test tests/TodoList.API.Tests/TodoList.API.Tests.csproj

```

---

**Próximos Passos para o Desenvolvedor:**

1. Clone este repositório.
2. Tente adicionar um novo campo no `TodoItem` (ex: `Description`).
3. Quebre o teste: rode `dotnet test` e veja a falha.
4. Corrija a implementação até o teste passar (Red-Green-Refactor).
