using System.Reflection;
using Bank.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Xunit;

namespace Bank.Api.Tests;

public class SecurityContractTests
{
    [Theory]
    [InlineData(typeof(ClientsController))]
    [InlineData(typeof(ComptesController))]
    [InlineData(typeof(GuichetsController))]
    [InlineData(typeof(TransactionsController))]
    [InlineData(typeof(TypesComptesController))]
    public void BusinessController_RequiresAuthenticatedUser(Type controllerType) =>
        Assert.NotNull(controllerType.GetCustomAttribute<AuthorizeAttribute>());

    [Fact]
    public void Login_AllowsAnonymousAccess()
    {
        var action = typeof(AuthController).GetMethod("Login");
        Assert.NotNull(action);
        Assert.NotNull(action!.GetCustomAttribute<AllowAnonymousAttribute>());
    }

    [Fact]
    public void CreateAgent_RequiresAdminRole()
    {
        var action = typeof(AuthController).GetMethod("CreateAgent");
        Assert.NotNull(action);
        Assert.Equal("Admin", action!.GetCustomAttribute<AuthorizeAttribute>()?.Roles);
    }

    [Theory]
    [InlineData("Depot")]
    [InlineData("Retrait")]
    [InlineData("Virement")]
    public void TransactionAction_DoesNotAcceptAgentIdFromRequest(string actionName)
    {
        var action = typeof(TransactionsController).GetMethod(actionName);
        Assert.NotNull(action);
        Assert.DoesNotContain(action!.GetParameters(), parameter => parameter.Name == "agentId");
    }
}
