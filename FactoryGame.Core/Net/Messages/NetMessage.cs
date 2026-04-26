namespace FactoryGame.Core.Net.Messages;

public abstract class NetMessage
{
    public abstract PacketType Type { get; }
    
    // Serialize this message into a byte array to send over the net wire
    public byte[] Serialize()
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);
        
        writer.Write((byte)Type);
        Write(writer);
        return ms.ToArray();
    }
    
    // Deserialize incoming bytes back into a message object
    public static NetMessage Deserialize(byte[] data)
    {
        using var ms = new MemoryStream(data);
        using var reader = new BinaryReader(ms);
        var type = (PacketType)reader.ReadByte();

        return type switch
        {
            PacketType.Handshake => HandshakeMessage.Read(reader),
            PacketType.Disconnect => DisconnectMessage.Read(reader),
            PacketType.PlayerPosition => PlayerPositionMessage.Read(reader),
            PacketType.PlayerState => PlayerStateMessage.Read(reader),
            _=> throw new Exception($"Unknown packet type: {type}")
        };
    }
    
    // Subclasses implement these to write/read their own fields
    protected abstract void Write(BinaryWriter writer);
}