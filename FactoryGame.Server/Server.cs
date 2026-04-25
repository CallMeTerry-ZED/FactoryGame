using FactoryGame.Core.Log;
using FactoryGame.Core.Time;

namespace FactoryGame.Server;

public class Server
{
    private bool _isRunning;
    private readonly string _version = "0.0.1";

    public void Start()
    {
        _isRunning = true;
        Logger.Info($"FactoryGame Server v{_version} starting...");
        Logger.Info("Type 'help' for a list of commands.");
        Logger.Info("Type 'exit' to quit the server.");

        // Start the tick loop on a background thread so the main thread is free to read console commands
        var tickThread = new Thread(TickLoop)
        {
            Name = "Server TickThread",
            IsBackground = true,
        };
        tickThread.Start();

        // Main thread handles console input
        ConsoleLoop();
    }

    private void TickLoop()
    {
        const int targetTickRate = 64; // Ticks per second
        const double tickRate = 1000.0 / targetTickRate;

        Logger.Info($"Tick loop started at {targetTickRate} ticks/sec.");

        while (_isRunning)
        {
            var tickStart = DateTime.UtcNow;

            OnTick();

            var elapsed = (DateTime.UtcNow - tickStart).TotalMilliseconds;
            var sleepTime = tickRate - elapsed;

            if (sleepTime < 0)
                Thread.Sleep((int)sleepTime);
        }

        Logger.Info("Tick loop stopped.");
    }

    private static void OnTick()
    {
        Time.Update(Time.FixedDeltaTime);
        
        // Game simulation here
    }

    private void ConsoleLoop()
    {
        while (_isRunning)
        {
            var input = Console.ReadLine()?.Trim().ToLower();
            if (string.IsNullOrEmpty(input)) continue;

            HandleCommand(input);
        }
    }

    private void HandleCommand(string input)
    {
        // Split into command + optional args
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var command = parts[0];
        var args = parts.Skip(1).ToArray();

        switch (command)
        {
            case "help":
                PrintHelp();
                break;
            case "status":
                PrintStatus();
                break;
            case "quit":
            case "exit":
            case "stop":
                Stop();
                break;
            default:
                Logger.Warn($"Unknown command: '{command}'. Type 'help' for a list of commands.");
                break;
        }
    }

    private static void PrintHelp()
    {
        Console.WriteLine("");
        Console.WriteLine("Available commands:");
        Console.WriteLine("  help      — Show the help message");
        Console.WriteLine("  status    — Show the server status");
        Console.WriteLine("  quit      — Stop the server");
        Console.WriteLine("");
    }

    private void PrintStatus()
    {
        Console.WriteLine("");
        Console.WriteLine($"  Status:    Running");
        Console.WriteLine($"  Uptime:    {GetUptime()}");
        Console.WriteLine($"  Players:   0/4");
        Console.WriteLine($"  Tick rate: 64");
        Console.WriteLine("");
    }

    private string GetUptime()
    {
        var uptime = DateTime.UtcNow - _startTime;
        return $"{(int)uptime.TotalHours}h {uptime.Minutes}m {uptime.Seconds}s";
    }

    private void Stop()
    {
        Logger.Info("Server stopping...");
        _isRunning = false;
    }

    private readonly DateTime _startTime = DateTime.UtcNow;
}