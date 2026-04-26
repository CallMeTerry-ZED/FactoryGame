using Silk.NET.Maths;

namespace FactoryGame.Core.Net.Messages;

public class PlayerPositionMessage : NetMessage
{
    public override PacketType Type => PacketType.PlayerPosition;
    
    public int PlayerId { get; }
    public Vector3D<float> Position { get; }
    public float Yaw { get; }
    public float Pitch { get; }

    public PlayerPositionMessage(int playerId, Vector3D<float> position, float yaw, float pitch)
    {
        PlayerId = playerId;
        Position = position;
        Yaw = yaw;
        Pitch = pitch;
    }

    protected override void Write(BinaryWriter writer)
    {
        writer.Write(PlayerId);
        writer.Write(Position.X);
        writer.Write(Position.Y);
        writer.Write(Position.Z);
        writer.Write(Yaw);
        writer.Write(Pitch);
    }

    public static PlayerPositionMessage Read(BinaryReader reader)
    {
        var playerId = reader.ReadInt32();
        var x = reader.ReadSingle();
        var y = reader.ReadSingle();
        var z = reader.ReadSingle();
        var yaw = reader.ReadSingle();
        var pitch = reader.ReadSingle();
        
        return new PlayerPositionMessage(playerId, new Vector3D<float>(x, y, z), yaw, pitch);
    }
}