using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using JTradeERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;

namespace JTradeERP.Api.IntegrationTests;

[Collection(nameof(SqlServerIntegrationCollection))]
public sealed class AuthenticationAndAuthorizationTests(ApiFactory factory)
{
    private const string Password = "test-password-never-used-outside-integration";

    [Fact]
    public async Task ProtectedEndpoints_Return401_WithoutAccessToken()
    {
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions { HandleCookies = false });
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/master-data/categories")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/admin/users")).StatusCode);
    }

    [Fact]
    public async Task DatabasePermissionChanges_ApplyToExistingJwt_AndAuditorRemainsReadOnly()
    {
        await factory.ResetDatabaseAsync();
        using var warehouseUser = await factory.CreateAuthenticatedClientAsync("Warehouse User");
        var denied = await warehouseUser.PostAsJsonAsync("/api/master-data/categories", new { name = "Denied" });
        Assert.Equal(HttpStatusCode.Forbidden, denied.StatusCode);

        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
            var token = new JwtSecurityTokenHandler().ReadJwtToken(warehouseUser.DefaultRequestHeaders.Authorization!.Parameter);
            var userId = Guid.Parse(token.Subject);
            var userRoles = db.UserRoles.Where(x => x.UserId == userId).ToArray();
            db.UserRoles.RemoveRange(userRoles);
            var adminRole = await db.Roles.SingleAsync(x => x.Name == "System Administrator");
            db.UserRoles.Add(new UserRole(userId, adminRole.Id));
            await db.SaveChangesAsync();
        }
        Assert.Equal(HttpStatusCode.Created, (await warehouseUser.PostAsJsonAsync("/api/master-data/categories", new { name = "Granted after DB update" })).StatusCode);

        using var auditor = await factory.CreateAuthenticatedClientAsync("Auditor");
        Assert.Equal(HttpStatusCode.Forbidden, (await auditor.PostAsJsonAsync("/api/master-data/categories", new { name = "Auditor cannot write" })).StatusCode);
    }

    [Fact]
    public async Task SecuritySeeds_ExactlyMatchAllPublishedRolePermissionCapabilityCells()
    {
        await factory.ResetDatabaseAsync();
        var matrixRoles = new[] { "System Administrator", "Finance Manager", "Sales Manager", "Sales User", "Warehouse Manager", "Warehouse User", "Purchase User", "Auditor" };
        var rows = new (string Code, Capability?[] Cells)[]
        {
            ("MasterData.View",[Capability.Full,Capability.Full,Capability.Full,Capability.View,Capability.Full,Capability.View,Capability.Full,Capability.View]),
            ("MasterData.Manage",[Capability.Full,Capability.Full,Capability.Full,null,Capability.Full,null,Capability.Full,null]),
            ("Customers.Manage",[Capability.Full,Capability.Full,Capability.Full,Capability.CreateEdit,Capability.View,null,Capability.View,Capability.View]),
            ("Suppliers.Manage",[Capability.Full,Capability.Full,null,null,Capability.View,null,Capability.CreateEdit,Capability.View]),
            ("Billing.Quotation",[Capability.Full,Capability.Full,Capability.Full,Capability.CreateEdit,Capability.View,null,null,Capability.View]),
            ("Billing.SalesOrder",[Capability.Full,Capability.Full,Capability.Full,Capability.CreateEdit,Capability.View,null,null,Capability.View]),
            ("Billing.Invoice",[Capability.Full,Capability.Full,Capability.Post,Capability.CreateEdit,Capability.View,null,null,Capability.View]),
            ("Billing.Post",[Capability.Full,Capability.Full,Capability.Post,null,null,null,null,null]),
            ("Billing.Cancel",[Capability.Full,Capability.Full,Capability.Post,null,null,null,null,Capability.View]),
            ("Billing.CreditNote",[Capability.Full,Capability.Full,Capability.Post,Capability.CreateEdit,Capability.View,null,null,Capability.View]),
            ("Billing.Payment",[Capability.Full,Capability.Full,Capability.View,Capability.View,null,null,null,Capability.View]),
            ("Purchasing.PurchaseOrder",[Capability.Full,Capability.Full,Capability.View,null,Capability.View,null,Capability.Full,Capability.View]),
            ("Purchasing.GRN",[Capability.Full,Capability.Full,Capability.View,null,Capability.Full,Capability.CreateEdit,Capability.View,Capability.View]),
            ("Stock.View",[Capability.Full,Capability.Full,Capability.View,Capability.View,Capability.Full,Capability.Full,Capability.View,Capability.View]),
            ("Stock.OpeningBalance",[Capability.Full,Capability.View,null,null,Capability.Full,null,null,Capability.View]),
            ("Stock.Adjust",[Capability.Full,Capability.View,null,null,Capability.Full,Capability.CreateEdit,null,Capability.View]),
            ("Stock.Transfer",[Capability.Full,Capability.View,null,null,Capability.Full,Capability.CreateEdit,null,Capability.View]),
            ("Stock.Post",[Capability.Full,Capability.View,null,null,Capability.Full,Capability.Post,null,Capability.View]),
            ("Reports.View",[Capability.Full,Capability.Full,Capability.Full,Capability.View,Capability.Full,Capability.View,Capability.View,Capability.Full]),
            ("AuditLog.View",[Capability.Full,Capability.View,Capability.View,null,Capability.View,null,null,Capability.Full]),
            ("Admin.Users.Manage",[Capability.Full,null,null,null,null,null,null,null]),
            ("Admin.Roles.Manage",[Capability.Full,null,null,null,null,null,null,null]),
            ("Admin.NumberSeries.Manage",[Capability.Full,Capability.Full,null,null,null,null,null,Capability.View]),
            ("Admin.Configuration.Manage",[Capability.Full,Capability.Full,null,null,null,null,null,Capability.View])
        };
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        Assert.Equal(matrixRoles.Order(), await db.Roles.Where(x => matrixRoles.Contains(x.Name)).Select(x => x.Name).Order().ToArrayAsync());
        Assert.Equal(9, await db.Roles.CountAsync());
        Assert.Equal(24, await db.Permissions.CountAsync());
        Assert.Empty(await (from ur in db.UserRoles join r in db.Roles on ur.RoleId equals r.Id join rp in db.RolePermissions on r.Id equals rp.RoleId where r.Name == "Read-Only User" select rp).ToListAsync());
        foreach (var row in rows)
        for (var index = 0; index < matrixRoles.Length; index++)
        {
            var value = await (from rp in db.RolePermissions join p in db.Permissions on rp.PermissionId equals p.Id join r in db.Roles on rp.RoleId equals r.Id where r.Name == matrixRoles[index] && p.Code == row.Code select (Capability?)rp.Capability).SingleOrDefaultAsync();
            Assert.Equal(row.Cells[index], value);
        }
    }

    [Fact]
    public async Task AdminCanManageRolesAndCapabilityAssignments_Idempotently()
    {
        await factory.ResetDatabaseAsync();
        using var admin = await factory.CreateAuthenticatedClientAsync();
        using var list = await admin.GetAsync("/api/admin/permissions");
        Assert.Equal(HttpStatusCode.OK, list.StatusCode);
        var permissions = await list.Content.ReadFromJsonAsync<JsonDocument>();
        var viewId = permissions!.RootElement.GetProperty("items").EnumerateArray().Single(x => x.GetProperty("code").GetString() == "MasterData.View").GetProperty("id").GetGuid();
        var roleResponse = await admin.PostAsJsonAsync("/api/admin/roles", new { name = "Master Data Reader" });
        Assert.Equal(HttpStatusCode.OK, roleResponse.StatusCode);
        var roleId = (await roleResponse.Content.ReadFromJsonAsync<JsonDocument>())!.RootElement.GetProperty("id").GetGuid();
        var grants = new { roleId, grants = new[] { new { permissionId = viewId, capability = (int)Capability.View } } };
        Assert.Equal(HttpStatusCode.OK, (await admin.PutAsJsonAsync($"/api/admin/roles/{roleId}/permissions", grants)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await admin.PutAsJsonAsync($"/api/admin/roles/{roleId}/permissions", grants)).StatusCode);
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        Assert.Equal(1, await db.RolePermissions.CountAsync(x => x.RoleId == roleId && x.PermissionId == viewId));
        var audit = await db.SecurityAuditEntries.Where(x => x.Entity == "Role" && x.EntityId == roleId.ToString()).ToListAsync();
        Assert.Contains(audit, x => x.Action == "RolePermissionsAssigned" && !string.IsNullOrWhiteSpace(x.CorrelationId));
    }

    [Fact]
    public async Task Login_ReturnsSignedJwtAndSecureHttpOnlyRefreshCookie_AndDoesNotExposePasswordHash()
    {
        await factory.ResetDatabaseAsync();
        var user = await CreateUserAsync("System Administrator");
        using var client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions { HandleCookies = false });
        var response = await client.PostAsJsonAsync("/api/auth/login", new { userName = user.UserName, password = Password });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain(user.PasswordHash, body);
        Assert.DoesNotContain(Password, body);
        using var json = JsonDocument.Parse(body);
        Assert.False(string.IsNullOrWhiteSpace(json.RootElement.GetProperty("accessToken").GetString()));
        var setCookie = Assert.Single(response.Headers.GetValues("Set-Cookie"));
        Assert.Contains("HttpOnly", setCookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Secure", setCookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("SameSite=Strict", setCookie, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("refreshToken", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task InvalidAndInactiveCredentials_ReturnGenericFailureWithoutTokens()
    {
        await factory.ResetDatabaseAsync();
        var user = await CreateUserAsync("System Administrator");
        using var client = factory.CreateClient();
        var invalid = await client.PostAsJsonAsync("/api/auth/login", new { userName = user.UserName, password = "wrong" });
        Assert.Equal(HttpStatusCode.Unauthorized, invalid.StatusCode);
        Assert.Empty(invalid.Headers.TryGetValues("Set-Cookie", out var invalidCookies) ? invalidCookies : []);

        await SetStatusAsync(user.Id, false);
        var inactive = await client.PostAsJsonAsync("/api/auth/login", new { userName = user.UserName, password = Password });
        Assert.Equal(HttpStatusCode.Unauthorized, inactive.StatusCode);
        Assert.Empty(inactive.Headers.TryGetValues("Set-Cookie", out var inactiveCookies) ? inactiveCookies : []);
    }

    [Fact]
    public async Task CreateUser_PersistsOnlyPasswordHash_AndAdminPolicyProtectsIt()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        using var anonymous = factory.CreateClient();
        Assert.Equal(HttpStatusCode.Unauthorized, (await anonymous.PostAsJsonAsync("/api/admin/users", new { userName = "x", email = "x@example.test", password = "secret" })).StatusCode);
        var password = "Unique-Integration-Password-42";
        var response = await client.PostAsJsonAsync("/api/admin/users", new { userName = "created-user", email = "created@example.test", password });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain(password, content);
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        var stored = await db.Users.SingleAsync(x => x.UserName == "created-user");
        Assert.NotEqual(password, stored.PasswordHash);
        Assert.True(scope.ServiceProvider.GetRequiredService<IPasswordService>().Verify(stored.PasswordHash, password));
    }

    [Fact]
    public async Task RefreshRotatesCookieAndOldCredentialCannotCreateAnotherSuccessor()
    {
        await factory.ResetDatabaseAsync();
        var user = await CreateUserAsync("System Administrator");
        using var client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions { HandleCookies = false });
        var login = await client.PostAsJsonAsync("/api/auth/login", new { userName = user.UserName, password = Password });
        var oldCookie = CookieValue(Assert.Single(login.Headers.GetValues("Set-Cookie")));
        client.DefaultRequestHeaders.Add("Cookie", $"jtrade_refresh={oldCookie}");
        var refresh = await client.PostAsync("/api/auth/refresh", null);
        Assert.Equal(HttpStatusCode.OK, refresh.StatusCode);
        var newCookie = CookieValue(Assert.Single(refresh.Headers.GetValues("Set-Cookie")));
        Assert.NotEqual(oldCookie, newCookie);
        Assert.DoesNotContain(oldCookie, await refresh.Content.ReadAsStringAsync());

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        var hashedOld = scope.ServiceProvider.GetRequiredService<ITokenService>().HashOpaqueToken(oldCookie);
        var tokenRows = await db.RefreshTokens.Where(x => x.UserId == user.Id).ToListAsync();
        Assert.Equal(2, tokenRows.Count);
        Assert.Contains(tokenRows, x => x.TokenHash == hashedOld && x.RevocationReason == "rotated");

        client.DefaultRequestHeaders.Remove("Cookie");
        client.DefaultRequestHeaders.Add("Cookie", $"jtrade_refresh={oldCookie}");
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.PostAsync("/api/auth/refresh", null)).StatusCode);
    }

    [Fact]
    public async Task ConcurrentRefreshDoubleUse_CannotCreateTwoSuccessors()
    {
        await factory.ResetDatabaseAsync();
        var user = await CreateUserAsync("System Administrator");
        using var loginClient = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions { HandleCookies = false });
        var login = await loginClient.PostAsJsonAsync("/api/auth/login", new { userName = user.UserName, password = Password });
        var cookie = CookieValue(Assert.Single(login.Headers.GetValues("Set-Cookie")));
        using var first = factory.CreateClient(); using var second = factory.CreateClient();
        first.DefaultRequestHeaders.Add("Cookie", $"jtrade_refresh={cookie}"); second.DefaultRequestHeaders.Add("Cookie", $"jtrade_refresh={cookie}");
        var results = await Task.WhenAll(first.PostAsync("/api/auth/refresh", null), second.PostAsync("/api/auth/refresh", null));
        Assert.InRange(results.Count(x => x.IsSuccessStatusCode), 0, 1);
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        var family = await db.RefreshTokens.Where(x => x.UserId == user.Id).ToListAsync();
        Assert.InRange(family.Count(x => x.RevokedAt == null), 0, 1);
    }

    [Fact]
    public async Task LogoutIsIdempotent_RevokesRefreshSessionAndExpiresCookie()
    {
        await factory.ResetDatabaseAsync();
        var user = await CreateUserAsync("System Administrator");
        using var client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions { HandleCookies = false });
        var login = await client.PostAsJsonAsync("/api/auth/login", new { userName = user.UserName, password = Password });
        var body = await login.Content.ReadFromJsonAsync<JsonDocument>();
        var access = body!.RootElement.GetProperty("accessToken").GetString();
        var cookie = CookieValue(Assert.Single(login.Headers.GetValues("Set-Cookie")));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access);
        client.DefaultRequestHeaders.Add("Cookie", $"jtrade_refresh={cookie}");
        var logout = await client.PostAsync("/api/auth/logout", null);
        Assert.Equal(HttpStatusCode.NoContent, logout.StatusCode);
        Assert.Contains("expires=", Assert.Single(logout.Headers.GetValues("Set-Cookie")), StringComparison.OrdinalIgnoreCase);
        Assert.Equal(HttpStatusCode.NoContent, (await client.PostAsync("/api/auth/logout", null)).StatusCode);
    }

    [Fact]
    public async Task MeRequiresAuthenticationAndReturnsOnlySafeIdentityAndAuthorization()
    {
        await factory.ResetDatabaseAsync();
        using var anonymous = factory.CreateClient();
        Assert.Equal(HttpStatusCode.Unauthorized, (await anonymous.GetAsync("/api/auth/me")).StatusCode);
        using var client = await factory.CreateAuthenticatedClientAsync();
        var response = await client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("permissions", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("passwordHash", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("tokenHash", json, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ForgotPasswordIsNonEnumeratingAndStoresOnlyOtpHash_ThenResetRevokesSessions()
    {
        await factory.ResetDatabaseAsync();
        var user = await CreateUserAsync("System Administrator");
        var rawRefresh = scopeToken();
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
            var tokens = scope.ServiceProvider.GetRequiredService<ITokenService>();
            db.RefreshTokens.Add(new RefreshToken(user.Id, tokens.HashOpaqueToken(rawRefresh), DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(3)));
            await db.SaveChangesAsync();
        }
        using var client = factory.CreateClient();
        var known = await client.PostAsJsonAsync("/api/auth/forgot-password", new { email = user.Email });
        var unknown = await client.PostAsJsonAsync("/api/auth/forgot-password", new { email = "unknown@example.test" });
        Assert.Equal(known.StatusCode, unknown.StatusCode);
        Assert.Equal(await known.Content.ReadAsStringAsync(), await unknown.Content.ReadAsStringAsync());
        var otp = Assert.Single(TestEmailSender.Instance.Sent).Otp;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
            var challenge = await db.PasswordResetOtps.SingleAsync(x => x.UserId == user.Id);
            Assert.NotEqual(otp, challenge.OtpHash);
        }
        var reset = await client.PostAsJsonAsync("/api/auth/reset-password", new { email = user.Email, otp, newPassword = "new-reset-password" });
        Assert.Equal(HttpStatusCode.OK, reset.StatusCode);
        using var scopeAfter = factory.Services.CreateScope();
        var dbAfter = scopeAfter.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        Assert.All(await dbAfter.RefreshTokens.Where(x => x.UserId == user.Id).ToListAsync(), x => Assert.NotNull(x.RevokedAt));
        var updatedUser = await dbAfter.Users.SingleAsync(x => x.Id == user.Id);
        var passwordService = scopeAfter.ServiceProvider.GetRequiredService<IPasswordService>();
        Assert.True(passwordService.Verify(updatedUser.PasswordHash, "new-reset-password"));
        Assert.False(passwordService.Verify(updatedUser.PasswordHash, Password));
    }

    [Fact]
    public async Task PasswordResetOtp_IsThrottled_AndInvalidatedAfterMaximumFailedAttempts()
    {
        await factory.ResetDatabaseAsync();
        var user = await CreateUserAsync("System Administrator");
        using var client = factory.CreateClient();

        var first = await client.PostAsJsonAsync("/api/auth/forgot-password", new { email = user.Email });
        var second = await client.PostAsJsonAsync("/api/auth/forgot-password", new { email = user.Email });
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(await first.Content.ReadAsStringAsync(), await second.Content.ReadAsStringAsync());
        var sent = Assert.Single(TestEmailSender.Instance.Sent);
        var wrongOtp = sent.Otp == "999999" ? "000000" : "999999";

        for (var attempt = 0; attempt < 5; attempt++)
        {
            var response = await client.PostAsJsonAsync("/api/auth/reset-password", new { email = user.Email, otp = wrongOtp, newPassword = "a-strong-new-password" });
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        var challenge = await db.PasswordResetOtps.SingleAsync(x => x.UserId == user.Id);
        Assert.Equal(5, challenge.FailedAttemptCount);
        Assert.NotNull(challenge.InvalidatedAt);
        Assert.NotEqual(sent.Otp, challenge.OtpHash);
        Assert.DoesNotContain(await db.SecurityAuditEntries.Select(x => x.NewValue).ToListAsync(), value => value?.Contains(sent.Otp, StringComparison.Ordinal) == true);
    }

    [Fact]
    public async Task SecurityDatabaseConstraints_RejectDuplicateUsersAndRoles()
    {
        await factory.ResetDatabaseAsync();
        using var admin = await factory.CreateAuthenticatedClientAsync();
        var first = await admin.PostAsJsonAsync("/api/admin/users", new { userName = "same-user", email = "same@example.test", password = "a-long-test-password" });
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var duplicateUserName = await admin.PostAsJsonAsync("/api/admin/users", new { userName = "same-user", email = "different@example.test", password = "a-long-test-password" });
        var duplicateEmail = await admin.PostAsJsonAsync("/api/admin/users", new { userName = "different-user", email = "same@example.test", password = "a-long-test-password" });
        Assert.Equal(HttpStatusCode.Conflict, duplicateUserName.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, duplicateEmail.StatusCode);

        var role = await admin.PostAsJsonAsync("/api/admin/roles", new { name = "Unique Role" });
        Assert.Equal(HttpStatusCode.OK, role.StatusCode);
        var duplicateRole = await admin.PostAsJsonAsync("/api/admin/roles", new { name = "Unique Role" });
        Assert.Equal(HttpStatusCode.Conflict, duplicateRole.StatusCode);
    }

    [Fact]
    public async Task DeactivatingUserRevokesRefreshSessionsAndImmediatelyDeniesAuthorization()
    {
        await factory.ResetDatabaseAsync();
        using var admin = await factory.CreateAuthenticatedClientAsync();
        using var target = await factory.CreateAuthenticatedClientAsync("Sales User");
        var token = new JwtSecurityTokenHandler().ReadJwtToken(target.DefaultRequestHeaders.Authorization!.Parameter);
        var userId = Guid.Parse(token.Subject);
        using var result = await admin.PutAsJsonAsync($"/api/admin/users/{userId}/status", new { isActive = false });
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await target.GetAsync("/api/master-data/categories")).StatusCode);
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        Assert.All(await db.RefreshTokens.Where(x => x.UserId == userId).ToListAsync(), x => Assert.NotNull(x.RevokedAt));
    }

    private async Task<User> CreateUserAsync(string roleName)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordService>();
        var role = await db.Roles.SingleAsync(x => x.Name == roleName);
        var user = new User($"auth-{Guid.NewGuid():N}", $"{Guid.NewGuid():N}@example.test", hasher.Hash(Password));
        db.Users.Add(user); db.UserRoles.Add(new UserRole(user.Id, role.Id)); await db.SaveChangesAsync(); return user;
    }

    private async Task SetStatusAsync(Guid id,bool active)
    { await using var scope=factory.Services.CreateAsyncScope();var db=scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();var user=await db.Users.SingleAsync(x=>x.Id==id);user.SetStatus(active);await db.SaveChangesAsync(); }
    private static string CookieValue(string header){var item=header.Split(';',2)[0];return Uri.UnescapeDataString(item[(item.IndexOf('=')+1)..]);}
    private static string scopeToken()=>Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(48));
}
