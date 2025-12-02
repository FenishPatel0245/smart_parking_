// SignalR connection for real-time telemetry updates
let connection = null;

function initializeTelemetryConnection() {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/telemetryhub")
        .withAutomaticReconnect()
        .build();

    connection.on("TelemetryUpdate", function (telemetry) {
        console.log("Telemetry update received:", telemetry);
        // Updates are handled by Blazor components
    });

    connection.on("DeviceStatusChanged", function (device) {
        console.log("Device status changed:", device);
    });

    connection.on("AlertGenerated", function (alert) {
        console.log("Alert generated:", alert);
        // Could show browser notification here
        if (Notification.permission === "granted") {
            new Notification("Smart Parking Alert", {
                body: alert.message,
                icon: "/favicon.ico"
            });
        }
    });

    connection.start()
        .then(() => console.log("SignalR connected"))
        .catch(err => console.error("SignalR connection error:", err));
}

// Request notification permission
if ("Notification" in window && Notification.permission === "default") {
    Notification.requestPermission();
}

// Initialize on page load
if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initializeTelemetryConnection);
} else {
    initializeTelemetryConnection();
}
