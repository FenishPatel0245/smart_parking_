using System;
using System.Collections.Generic;
using SmartParkingLot.Domain.Models;

namespace SmartParkingLot.Application.Services;

public interface IActivityLogService
{
    event Action<ActivityLogEntry>? OnLogAdded;
    IEnumerable<ActivityLogEntry> GetLogs();
    void LogActivity(string title, string description, string severity = "Info", string actionType = "System", string icon = "fas fa-info-circle");
    void ClearLogs();
}
