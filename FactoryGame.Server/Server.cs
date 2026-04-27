using FactoryGame.Core.Log;
using FactoryGame.Core.Time;
using FactoryGame.Core.Net;
using FactoryGame.Core.Config;
namespace FactoryGame.Server;

public class Server
{
    private ServerConfig? _config;
    
    private bool _isRunning;
    private ServerNet? _net;
    private int _tickCount = 0;
    private const int BroadcastRate = 64 / 20; // About 20 updates per second

    public void Start()
    {
        _isRunning = true;
        
        _config = new ServerConfig();
        if (!File.Exists("Configs/Server/Server.cfg"))
        {
            ServerConfig.GenerateDefault();
        }
        _config.Load();

        Logger.Info($"FactoryGame Server v{NetProtocol.Version} starting...");
        Logger.Info($"Server name: {_config.ServerName}");
        
        _net = new ServerNet(_config.MaxPlayers);
        _net.Start(_config.Port);
        
        // Start the tick loop on a background thread so the main thread is free to read console commands
        var tickThread = new Thread(TickLoop)
        {
            Name = "ServerTickThread",
            IsBackground = true,
        };
        tickThread.Start();

        Logger.Info("Type 'help' for a list of commands.");
        Logger.Info("Type 'exit' to quit the server.");
        
        // Main thread handles console input
        ConsoleLoop();
    }

    private void TickLoop()
    {
        int tickRate = _config?.TickRate ?? 64; // Ticks per second
        double tickRateInterval = 1000.0 / tickRate;

        Logger.Info($"Tick loop started at {tickRate} ticks/sec.");

        while (_isRunning)
        {
            var tickStart = DateTime.UtcNow;

            OnTick();

            var elapsed = (DateTime.UtcNow - tickStart).TotalMilliseconds;
            var sleepTime = tickRateInterval - elapsed;

            if (sleepTime > 0)
                Thread.Sleep((int)sleepTime);
        }

        Logger.Info("Tick loop stopped.");
    }

    private void OnTick()
    {
        Time.Update(Time.FixedDeltaTime);
        _net?.Poll();
        
        _tickCount++;
        if (_tickCount >= BroadcastRate)
        {
            _net?.BroadcastPlayerStates();
            _tickCount = 0;
        }
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
        Console.WriteLine($"Name:      {_config?.ServerName ?? "FactoryGame Server"}");
        Console.WriteLine($"Status:    Running");
        Console.WriteLine($"Uptime:    {GetUptime()}");
        Console.WriteLine($"Players:   {_net?.PlayerCount ?? 0}/{_config?.MaxPlayers ?? 4}");
        Console.WriteLine($"Tick rate: {_config?.TickRate ?? 64}");

        if (_net?.PlayerCount > 0)
        {
            Console.WriteLine("Connected:");
            foreach (var name in _net.GetPlayerNames())
                Console.WriteLine($"    - {name}");
        }

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
        _net?.Dispose();
    }

    private readonly DateTime _startTime = DateTime.UtcNow;
}