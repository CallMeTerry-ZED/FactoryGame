using System.Numerics;
using Silk.NET.Maths;
using FactoryGame.Core.Camera;

namespace FactoryGame.Client.Render;

public class RenderContext
{
    public Camera Camera { get; }
    public Matrix4X4<float> View { get; }
    public Matrix4X4<float> Projection { get; }
    public double DeltaTime { get; }
    
    public RenderContext(Camera camera, double delta)
    {
        Camera = camera;
        View = camera.GetViewMatrix();
        Projection = camera.GetProjectionMatrix();
        DeltaTime = delta;
    }
}