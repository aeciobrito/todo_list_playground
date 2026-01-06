using System;
using TodoList.TestSupport.Fixtures;

namespace TodoList.API.Tests.Collections;

[CollectionDefinition("ApiControllerCollection")]
public class ApiControllerCollection : ICollectionFixture<ApiControllerFixture>
{
}