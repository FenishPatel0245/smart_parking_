namespace SmartParkingLot.Domain.Enums;

/// <summary>
/// User roles for role-based access control
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Administrator with full system access
    /// </summary>
    Admin = 1,
    
    /// <summary>
    /// Operator with monitoring and limited control access
    /// </summary>
    Operator = 2
}
