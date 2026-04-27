using FactoryGame.Core.Log;

namespace FactoryGame.Core.Config;

public class ServerConfig
{
    private readonly CfgFile _cfg;

    public string ServerName { get; private set; } = "FactoryGame - Server";
    public int Port { get; private set; } = 7777;
    public int MaxPlayers { get; private set; } = 4;
    public int TickRate { get; private set; } = 64;
    public bool Verbose { get; private set; } = false;
    
    public ServerConfig(string path = "Configs/Server/Server.cfg")
    {
        _cfg = new  CfgFile(path);
    }

    public void Load()
    {
        _cfg.Load();
        
        ServerName = _cfg.GetString("ServerName", ServerName);
        Port = _cfg.GetInt("Port", Port);
        MaxPlayers = _cfg.GetInt("MaxPlayers", MaxPlayers);
        TickRate = _cfg.GetInt("TickRate", TickRate);
        Verbose = _cfg.GetBool("Verbose", Verbose);
        
        // Validate
        if (MaxPlayers is < 1 or > 24)
        {
            Logger.Warn($"ServerConfig: MaxPlayers '{MaxPlayers}' out of range (1-24), clamping.");
            MaxPlayers = Math.Math.Clamp(MaxPlayers, 1, 24);
        }

        if (TickRate is < 8 or > 128)
        {
            Logger.Warn($"ServerConfig: TickRate '{TickRate}' out of range (8-128), clamping.");
            TickRate = Math.Math.Clamp(TickRate, 8, 128);
        }
        
        Logger.Info($"ServerConfig: Name='{ServerName}' Port={Port} MaxPlayers={MaxPlayers} TickRate={TickRate}");
    }

    // Write current values to disk
    public void Save()
    {
        _cfg.SetString("ServerName", ServerName);
        _cfg.SetInt("Port", Port);
        _cfg.SetInt("MaxPlayers", MaxPlayers);
        _cfg.SetInt("TickRate", TickRate);
        _cfg.SetBool("Verbose", Verbose);
        _cfg.Save();
    }
    
    // Gen a new config with default values
    public static void GenerateDefault(string path = "Configs/Server/Server.cfg")
    {
        var config = new ServerConfig(path);

        var lines = new[]
        {
            "# FactoryGame Server Configuration",
            "# -----------------------------------------------------------------",
            "# ServerName   - Name shown in server browser",
            "# Port         - UDP port to listen on",
            "# MaxPlayers   - Max connected players (1-24)",
            "# TickRate     - Server simulation ticks per second (8-128)",
            "# Verbose      - Enable verbose logging (true/false)",
            "# -----------------------------------------------------------------",
            "",
            "ServerName=FactoryGame Server",
            "Port=7777",
            "MaxPlayers=4",
            "TickRate=64",
            "Verbose=false",
        };

        var dir = Path.GetDirectoryName(path);
        
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        
        File.WriteAllLines(path, lines);
        Logger.Info($"ServerConfig: Generated default config at '{path}'.");
    }
}