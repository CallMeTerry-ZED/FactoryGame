using Serilog;
using Serilog.Exceptions;

namespace FactoryGame.Core.Log;

public class Logger
{
    private static ILogger? _logger;

    public static void Initialize(string logDirectory = "Logs")
    {
        Directory.CreateDirectory(logDirectory);

        _logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.WithThreadId()
            .Enrich.WithExceptionDetails()
            .WriteTo.Async(a =>
            {
                // Colored console output
                a.Console(
                    outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] ({ThreadId}) {Message:lj}{NewLine}{Exception}");

                // Rolling file — new file each day, keeps 7 days
                a.File(
                    path: Path.Combine(logDirectory, "factorygame-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate:
                    "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] ({ThreadId}) {Message:lj}{NewLine}{Exception}");
            })
            .CreateLogger();
    }

    // Logging
    public static void Debug(string message) => _logger?.Debug(message);
    public static void Info(string message) => _logger?.Information(message);
    public static void Warn(string message) => _logger?.Warning(message);
    public static void Error(string message) => _logger?.Error(message);
    public static void Fatal(string message) => _logger?.Fatal(message);
    
    // Logging with args overloads
    public static void Debug(string message, params object[] args) => _logger?.Information(message, args);
    public static void Info(string message, params object[] args) => _logger?.Information(message, args);
    public static void Warn(string message, params object[] args) => _logger?.Information(message, args);
    public static void Error(string message, params object[] args) => _logger?.Information(message, args);
    public static void Fatal(string message, params object[] args) => _logger?.Information(message, args);

    // Log Exceptions/Exceptions with args overloads
    public static void Error(string message, Exception ex) => _logger?.Error(ex, message);
    public static void Error(string message, Exception ex, params object[] args) => _logger?.Error(ex, message, args);
    public static void Fatal(string message, Exception ex) => _logger?.Fatal(ex, message);
    public static void Fatal(string message, Exception ex, params object[] args) => _logger?.Fatal(ex, message, args);
    
    public static void Shutdown() => Serilog.Log.CloseAndFlush();
}