namespace FactoryGame.Core.Net.Messages;

public class HandshakeMessage : NetMessage
{
    public override PacketType Type => PacketType.Handshake;
    
    public string PlayerName { get; }
    public string Version { get; }

    public HandshakeMessage(string playerName, string version)
    {
        PlayerName = playerName ?? throw new ArgumentNullException(nameof(playerName));
        Version = version ?? throw new ArgumentNullException(nameof(version));
    }
    
    protected override void Write(BinaryWriter writer)
    {
        writer.Write(PlayerName);
        writer.Write(Version);
    }

    public static HandshakeMessage Read(BinaryReader reader)
    {
        var playerName = reader.ReadString();
        var version = reader.ReadString();
        return new HandshakeMessage(playerName, version);
    }
}