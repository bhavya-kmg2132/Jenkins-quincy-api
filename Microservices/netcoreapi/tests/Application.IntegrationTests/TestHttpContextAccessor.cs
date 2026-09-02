using Microsoft.AspNetCore.Http;

namespace Application.IntegrationTests;

/// <summary>
/// Replaces the real HttpContextAccessor (which uses AsyncLocal) with a plain
/// field so the mocked HttpContext is visible across all NUnit test threads.
/// </summary>
public sealed class TestHttpContextAccessor : IHttpContextAccessor
{
    public HttpContext HttpContext { get; set; }
}
