namespace FactoryGame.Core.Net;

public static class NetProtocol
{
    public const int Port = 7777;
    public const string AppId = "FactoryGame";
    public const string Version = "0.0.1";
    public const int MaxPlayers = 4;
    public const int PollInterval = 15; // ms, how often LiteNetLib polls for packets
}