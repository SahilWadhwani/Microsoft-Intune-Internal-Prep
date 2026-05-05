using System.Net;
using System.Net.Http.Json;
using EndpointGuardian.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;

namespace EndpointGuardian.Tests.Integration;

// Integration tests for the Device API endpoints
// These tests use the WebApplicationFactory to host the API in-memory and test the endpoints end-to-end
public class DeviceApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public DeviceApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateDevice_WhenRequestIsValid_ReturnsCreated()
    {
        var request = new CreateDeviceRequest(
            "Integration Test Device",
            DevicePlatform.Windows,
            14,
            true,
            true,
            true);

        var response = await _client.PostAsJsonAsync("/api/devices", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}