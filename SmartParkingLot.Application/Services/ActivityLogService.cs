using System;
using System.Collections.Generic;
using System.Linq;
using SmartParkingLot.Domain.Models;

namespace SmartParkingLot.Application.Services;

public class ActivityLogService : IActivityLogService
{
    private readonly List<ActivityLogEntry> _logs = new();
    private readonly IAuthenticationService _authService;

    public event Action<ActivityLogEntry>? OnLogAdded;

    public ActivityLogService(IAuthenticationService authService)
    {
        _authService = authService;
    }

    public IEnumerable<ActivityLogEntry> GetLogs()
    {
        return _logs.OrderByDescending(l => l.Timestamp);
    }

    public void LogActivity(string title, string description, string severity = "Info", string actionType = "System", string icon = "fas fa-info-circle")
    {
        var entry = new ActivityLogEntry
        {
            Timestamp = DateTime.Now,
            Title = title,
            Description = description,
            Severity = severity,
            ActionType = actionType,
            Icon = icon,
            TriggeredBy = _authService.CurrentUser?.FullName ?? "System"
        };

        _logs.Add(entry);
        OnLogAdded?.Invoke(entry);
    }

    public void ClearLogs()
    {
        _logs.Clear();
        LogActivity("Logs Cleared", "Activity logs have been cleared.", "Info", "System", "fas fa-trash");
    }
}
