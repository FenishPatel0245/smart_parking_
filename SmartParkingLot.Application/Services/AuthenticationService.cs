using SmartParkingLot.Application.DTOs;
using SmartParkingLot.Domain.Models;
using SmartParkingLot.Infrastructure.Repositories;

namespace SmartParkingLot.Application.Services;

public interface IAuthenticationService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<bool> ValidateSessionAsync(int userId);
    Task LogoutAsync(int userId);
    Task<UserDto?> GetUserByIdAsync(int userId);
    Task InitializeAsync();
    UserDto? CurrentUser { get; }
    event Action? OnAuthStateChanged;
}

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IEventLogRepository _eventLogRepository;
    private readonly ISessionStorageService _sessionStorage;

    public AuthenticationService(
        IUserRepository userRepository,
        IEventLogRepository eventLogRepository,
        ISessionStorageService sessionStorage)
    {
        _userRepository = userRepository;
        _eventLogRepository = eventLogRepository;
        _sessionStorage = sessionStorage;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);

            if (user == null || !user.IsActive)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }

            // Verify password using BCrypt
            bool isMatch = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            
            if (!isMatch)
            {
                await LogEventAsync("LoginFailed", $"Failed login attempt for user: {request.Username}", user.Id);
                
                return new LoginResponse
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }

            // Update last login time
            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            await LogEventAsync("Login", $"User {user.Username} logged in successfully", user.Id);

            CurrentUser = MapToDto(user);
            
            // Persist session
            await _sessionStorage.SetAsync("authToken", user.Id);

            return new LoginResponse
            {
                Success = true,
                Message = "Login successful",
                User = CurrentUser
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login Error: {ex}");
            return new LoginResponse
            {
                Success = false,
                Message = $"Login failed: {ex.Message}"
            };
        }
        finally
        {
            OnAuthStateChanged?.Invoke();
        }
    }

    public UserDto? CurrentUser { get; private set; }
    public event Action? OnAuthStateChanged;

    public async Task InitializeAsync()
    {
        try
        {
            var userId = await _sessionStorage.GetAsync<int>("authToken");
            if (userId != 0)
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user != null && user.IsActive)
                {
                    CurrentUser = MapToDto(user);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Session restoration failed: {ex.Message}");
        }
        finally
        {
            OnAuthStateChanged?.Invoke();
        }
    }

    public async Task<bool> ValidateSessionAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null && user.IsActive;
    }

    public async Task LogoutAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user != null)
        {
            await LogEventAsync("Logout", $"User {user.Username} logged out", userId);
        }
        CurrentUser = null;
        await _sessionStorage.DeleteAsync("authToken");
        OnAuthStateChanged?.Invoke();
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null ? MapToDto(user) : null;
    }

    private UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }

    private async Task LogEventAsync(string eventType, string description, int? userId = null)
    {
        var log = new EventLog
        {
            EventType = eventType,
            Description = description,
            UserId = userId,
            Timestamp = DateTime.UtcNow
        };

        await _eventLogRepository.AddAsync(log);
    }
}
