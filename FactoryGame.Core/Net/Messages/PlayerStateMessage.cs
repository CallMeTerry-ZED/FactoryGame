using Silk.NET.Maths;

namespace FactoryGame.Core.Net.Messages;

public class PlayerState
{
    public int PlayerId { get; }
    public string Name { get; }
    public Vector3D<float> Position { get; }
    public float Yaw { get; }
    public float Pitch { get; }

    public PlayerState(int playerId, string name, Vector3D<float> position, float yaw, float pitch)
    {
        PlayerId = playerId;
        Name = name;
        Position = position;
        Yaw = yaw;
        Pitch = pitch;
    }
}

public class PlayerStateMessage : NetMessage
{
    public override PacketType Type => PacketType.PlayerState;
    public List<PlayerState> Players { get; }

    public PlayerStateMessage(List<PlayerState> players)
    {
        Players = players;
    }

    protected override void Write(BinaryWriter writer)
    {
        writer.Write(Players.Count);
        foreach (var p in Players)
        {
            writer.Write(p.PlayerId);
            writer.Write(p.Name);
            writer.Write(p.Position.X);
            writer.Write(p.Position.Y);
            writer.Write(p.Position.Z);
            writer.Write(p.Yaw);
            writer.Write(p.Pitch);
        }
    }

    public static PlayerStateMessage Read(BinaryReader reader)
    {
        var count = reader.ReadInt32();
        var players = new List<PlayerState>(count);

        for (int i = 0; i < count; i++)
        {
            var id = reader.ReadInt32();
            var name = reader.ReadString();
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            var yaw = reader.ReadSingle();
            var pitch = reader.ReadSingle();
            
            players.Add(new PlayerState(id, name, new Vector3D<float>(x, y, z), yaw, pitch));
        }
        
        return new PlayerStateMessage(players);
    }
}