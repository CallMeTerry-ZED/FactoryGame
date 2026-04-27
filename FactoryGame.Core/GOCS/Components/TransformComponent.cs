using Silk.NET.Maths;
using FactoryGame.Core.Math;

namespace FactoryGame.Core.GOCS.Components;

public class TransformComponent : Component
{
    public Vector3D<float> Position { get; set; } = Vector3D<float>.Zero;
    public Vector3D<float> Scale { get; set; } = new(1f, 1f, 1f);
    
    // Euler angles in degrees
    public float Yaw { get; set; } = 0f;
    public float Pitch { get; set; } = 0f;
    public float Roll { get; set; } = 0f;
    
    // Build a model matrix from current position/rotation/scale
    public Matrix4X4<float> GetModelMatrix()
    {
        var translation = Matrix4X4.CreateTranslation(Position);
        var rotX = Matrix4X4.CreateRotationX(Math.Math.ToRadians(Pitch));
        var rotY = Matrix4X4.CreateRotationY(Math.Math.ToRadians(Yaw));
        var rotZ = Matrix4X4.CreateRotationZ(Math.Math.ToRadians(Roll));
        var scale = Matrix4X4.CreateScale(Scale);
        
        return scale * rotZ * rotX * rotY * translation;
    }

    public override string ToString() => $"Transform(Pos={Position}, Yaw={Yaw:F1}, Pitch={Pitch:F1}, Scale={Scale})";
}