using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using SmartParkingLot.Application.Services;

namespace SmartParkingLot.Web.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ISessionStorageService _sessionService;
    private readonly IAuthenticationService _authService;

    public CustomAuthenticationStateProvider(
        ISessionStorageService sessionService,
        IAuthenticationService authService)
    {
        _sessionService = sessionService;
        _authService = authService;
        _authService.OnAuthStateChanged += NotifyAuthenticationStateChanged;
    }

    public void Dispose()
    {
        _authService.OnAuthStateChanged -= NotifyAuthenticationStateChanged;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = _authService.CurrentUser;
        
        // If not in memory, try to get from session
        if (user == null)
        {
            var userId = await _sessionService.GetAsync<int?>("UserId");
            if (userId.HasValue)
            {
                // In a real app, we'd fetch the user from DB here. 
                // For now, we might rely on AuthService to have it or re-fetch.
                // But AuthService.CurrentUser is the source of truth in this simple app.
                // If AuthService doesn't have it, we are effectively logged out.
                // To make this robust, AuthService should probably persist/restore state.
            }
        }

        if (user == null)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("FullName", user.FullName)
        };

        var identity = new ClaimsIdentity(claims, "CustomAuth");
        var principal = new ClaimsPrincipal(identity);

        return new AuthenticationState(principal);
    }

    public void NotifyAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
