using Silk.NET.Maths;

namespace FactoryGame.Core.Player;

public class Player
{
    public int Id { get; }
    public string Name { get; }
    public Vector3D<float> Position { get; set; } = new(0, 0, 0);
    public Vector2D<float> Rotation { get; set; } = new(0, 0); // Yaw, Pitch

    public Player(int id, string name)
    {
        Id = id;
        Name = name;
    }
    
    public override string ToString() => $"Player(id={Id}, name={Name})";
}