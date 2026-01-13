using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace SkiGogglesShop.Tests.Helpers;

public static class MockHttpContext
{
    public static ControllerContext CreateControllerContext(string? sessionId = null)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new MockSession(sessionId);

        return new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    public static ITempDataDictionary CreateTempData()
    {
        var tempDataProvider = new Mock<ITempDataProvider>();
        var tempDataDictionaryFactory = new TempDataDictionaryFactory(tempDataProvider.Object);
        return tempDataDictionaryFactory.GetTempData(new DefaultHttpContext());
    }
}

public class MockSession : ISession
{
    private readonly Dictionary<string, byte[]> _sessionStorage = new();

    public MockSession(string? initialSessionId = null)
    {
        if (initialSessionId != null)
        {
            _sessionStorage["CartSessionId"] = System.Text.Encoding.UTF8.GetBytes(initialSessionId);
        }
    }

    public string Id => Guid.NewGuid().ToString();
    public bool IsAvailable => true;
    public IEnumerable<string> Keys => _sessionStorage.Keys;

    public void Clear() => _sessionStorage.Clear();

    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public void Remove(string key) => _sessionStorage.Remove(key);

    public void Set(string key, byte[] value) => _sessionStorage[key] = value;

    public bool TryGetValue(string key, out byte[]? value) => _sessionStorage.TryGetValue(key, out value);
}
