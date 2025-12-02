using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SmartParkingLot.Application.Services;

namespace SmartParkingLot.Web.Services;

public class SessionStorageService : ISessionStorageService
{
    private readonly ProtectedSessionStorage _sessionStorage;

    public SessionStorageService(ProtectedSessionStorage sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public async Task SetAsync<T>(string key, T value)
    {
        await _sessionStorage.SetAsync(key, value);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var result = await _sessionStorage.GetAsync<T>(key);
            return result.Success ? result.Value : default;
        }
        catch
        {
            return default;
        }
    }

    public async Task DeleteAsync(string key)
    {
        await _sessionStorage.DeleteAsync(key);
    }
}
