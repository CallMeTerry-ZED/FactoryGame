namespace FactoryGame.Core.Net.Messages;

public class HandshakeMessage : NetMessage
{
    public override PacketType Type => PacketType.Handshake;
    
    public string PlayerName { get; }
    public string Version { get; }
    public int AssignedId { get; }

    public HandshakeMessage(string playerName, string version, int assignedId = 0)
    {
        PlayerName = playerName ?? throw new ArgumentNullException(nameof(playerName));
        Version = version ?? throw new ArgumentNullException(nameof(version));
        AssignedId = assignedId;
    }
    
    protected override void Write(BinaryWriter writer)
    {
        writer.Write(PlayerName);
        writer.Write(Version);
        writer.Write(AssignedId);
    }

    public static HandshakeMessage Read(BinaryReader reader)
    {
        var playerName = reader.ReadString();
        var version = reader.ReadString();
        var assignedId = reader.ReadInt32();
        return new HandshakeMessage(playerName, version, assignedId);
    }
}