using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using OrderManagement.Api.Contracts;
using OrderManagement.Application.Orders;

namespace OrderManagement.Api.Tests;

public sealed class OrdersApiTests(ApiWebApplicationFactory factory)
    : IClassFixture<ApiWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    [Fact]
    public async Task Create_and_read_order_preserves_exact_decimal_values()
    {
        using var client = await CreateAuthenticatedClientAsync();
        var unitPrice = 0.1234567890123456789012345678m;
        var customerId = Guid.NewGuid();

        var createResponse = await client.PostAsJsonAsync(
            "/api/orders",
            new
            {
                customerId,
                items = new[]
                {
                    new
                    {
                        productName = "Precision item",
                        quantity = 3,
                        unitPrice
                    }
                }
            });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<OrderDto>(JsonOptions);
        Assert.NotNull(created);
        Assert.Equal(unitPrice, Assert.Single(created.Items).UnitPrice);
        Assert.Equal(unitPrice * 3, created.TotalAmount);

        var getResponse = await client.GetAsync($"/api/orders/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var retrieved = await getResponse.Content.ReadFromJsonAsync<OrderDto>(JsonOptions);
        Assert.NotNull(retrieved);
        Assert.Equal(unitPrice, Assert.Single(retrieved.Items).UnitPrice);
        Assert.Equal(unitPrice * 3, retrieved.TotalAmount);
    }

    [Fact]
    public async Task Invalid_order_returns_bad_request()
    {
        using var client = await CreateAuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync(
            "/api/orders",
            new
            {
                customerId = Guid.Empty,
                items = Array.Empty<object>()
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Cancelling_an_already_cancelled_order_returns_conflict()
    {
        using var client = await CreateAuthenticatedClientAsync();

        var createResponse = await client.PostAsJsonAsync(
            "/api/orders",
            new
            {
                customerId = Guid.NewGuid(),
                items = new[]
                {
                    new
                    {
                        productName = "Conflict item",
                        quantity = 1,
                        unitPrice = 10m
                    }
                }
            });
        var created = await createResponse.Content.ReadFromJsonAsync<OrderDto>(JsonOptions);
        Assert.NotNull(created);

        var firstCancelResponse = await client.PatchAsync(
            $"/api/orders/{created.Id}/cancel",
            content: null);
        var secondCancelResponse = await client.PatchAsync(
            $"/api/orders/{created.Id}/cancel",
            content: null);

        Assert.Equal(HttpStatusCode.OK, firstCancelResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, secondCancelResponse.StatusCode);
        Assert.Contains(
            "Only pending orders can be cancelled.",
            await secondCancelResponse.Content.ReadAsStringAsync());
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = factory.CreateClient();
        var loginResponse = await client.PostAsJsonAsync(
            "/auth/login",
            new
            {
                email = "dev@mazzatech.com",
                password = "Senha@123"
            });
        loginResponse.EnsureSuccessStatusCode();

        var login = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions);
        Assert.NotNull(login);
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(login.TokenType, login.AccessToken);

        return client;
    }
}
