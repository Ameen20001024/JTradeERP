using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using JTradeERP.Infrastructure.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Concurrent;

namespace JTradeERP.Api.IntegrationTests;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    public const string TestDatabaseName = "JTradeERP_MD01_IntegrationTests";
    public const string TestConnectionString = "Server=.\\SQLEXPRESS;Database=JTradeERP_MD01_IntegrationTests;Trusted_Connection=True;TrustServerCertificate=True;";
    private readonly SemaphoreSlim databaseResetLock = new(1, 1);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:DefaultConnection", TestConnectionString);
        builder.UseSetting("Jwt:Issuer", "JTradeERP.Tests");
        builder.UseSetting("Jwt:Audience", "JTradeERP.Tests.Client");
        builder.UseSetting("Jwt:SigningKey", "integration-test-signing-key-not-for-production-0123456789");
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
        });
        builder.ConfigureAppConfiguration((_, configuration) =>
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = TestConnectionString
            }));
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IEmailSender>();
            services.AddSingleton<IEmailSender>(TestEmailSender.Instance);
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await databaseResetLock.WaitAsync();
        try
        {
            await using var scope = Services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
            EnsureDedicatedTestDatabase(dbContext);
            dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(2));
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.MigrateAsync();
            TestEmailSender.Instance.Clear();
        }
        finally
        {
            databaseResetLock.Release();
        }
    }

    public async Task<HttpClient> CreateAuthenticatedClientAsync(string roleName = "System Administrator")
    {
        await using var scope = Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
        var role = await db.Roles.SingleAsync(x => x.Name == roleName);
        var user = new User($"integration-{Guid.NewGuid():N}", $"{Guid.NewGuid():N}@example.test", passwordService.Hash("integration-only-password"));
        db.Users.Add(user);
        db.UserRoles.Add(new UserRole(user.Id, role.Id));
        await db.SaveChangesAsync();
        var access = tokenService.CreateAccessToken(user, [role.Name]);
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access.AccessToken);
        return client;
    }

    private static void EnsureDedicatedTestDatabase(JTradeERPDbContext dbContext)
    {
        var connection = dbContext.Database.GetDbConnection();
        if (!string.Equals(connection.DataSource, @".\SQLEXPRESS", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(connection.Database, TestDatabaseName, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Integration tests may only reset the dedicated JTradeERP_MD01_IntegrationTests database on .\\SQLEXPRESS.");
        }
    }
}

internal sealed class TestEmailSender : IEmailSender
{
    public static TestEmailSender Instance { get; } = new();
    private readonly ConcurrentQueue<(string Email,string Otp)> sent = new();
    public IReadOnlyCollection<(string Email,string Otp)> Sent => sent.ToArray();
    public Task SendPasswordResetOtpAsync(string email,string otp,CancellationToken cancellationToken){sent.Enqueue((email,otp));return Task.CompletedTask;}
    public void Clear(){while(sent.TryDequeue(out _)){} }
}

[CollectionDefinition(nameof(SqlServerIntegrationCollection), DisableParallelization = true)]
public sealed class SqlServerIntegrationCollection : ICollectionFixture<ApiFactory>;

[Collection(nameof(SqlServerIntegrationCollection))]
public sealed class ApiFoundationTests
{
    private readonly ApiFactory factory;

    public ApiFoundationTests(ApiFactory factory) => this.factory = factory;
    [Fact]
    public async Task HealthEndpoint_ReturnsSuccess_WhenAppHealthy()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UnhandledException_IsConvertedToProblemDetails()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/_foundation/throw");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Contains("application/problem+json", response.Content.Headers.ContentType?.ToString());
        Assert.Contains("unexpected error", content, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Foundation test exception", content);
    }

    [Fact]
    public async Task ValidationFailure_ReturnsClientError()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/_foundation/validation");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("validation", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Value", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DbContext_IsScopedAndResolvable()
    {
        using var scopeOne = factory.Services.CreateScope();
        using var scopeTwo = factory.Services.CreateScope();
        var first = scopeOne.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        var second = scopeOne.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        var third = scopeTwo.ServiceProvider.GetRequiredService<JTradeERPDbContext>();

        Assert.Same(first, second);
        Assert.NotSame(first, third);
    }
}
