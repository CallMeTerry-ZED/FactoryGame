using Silk.NET.Maths;
using Silk.NET.OpenGL;
using FactoryGame.Core.Camera;
using FactoryGame.Core.Log;
using FactoryGame.Core.Net.Messages;
using FactoryGame.Core.GOCS;
using FactoryGame.Client.Components;

namespace FactoryGame.Client.Render;

public class Renderer : IDisposable
{
    private readonly GL _gl;
    private readonly Shader _basicShader;
    private readonly Mesh _cubeMesh;
    private Scene? _scene;
    
    public Mesh   CubeMesh    => _cubeMesh;
    public Shader BasicShader => _basicShader;
    
    // 8 unique vertices of a cube
    private static readonly float[] CubeVertices =
    {
        // Position
        -0.5f, -0.5f, -0.5f, // 0 back bottom left
        0.5f, -0.5f, -0.5f, // 1 back bottom right
        0.5f, 0.5f, -0.5f, // 2 back top right
        -0.5f, 0.5f, -0.5f, // 3 back top left
        -0.5f, -0.5f, 0.5f, // 4 front bottom left
        0.5f, -0.5f, 0.5f, // 5 front bottom right
        0.5f, 0.5f, 0.5f, // 6 front top right
        -0.5f, 0.5f, 0.5f, // 7 front top left
    };

    // 6 faces, 2 triangles each, 3 indices per triangle. 36 indices
    private static readonly uint[] CubeIndices =
    {
        0, 1, 2, 2, 3, 0, // back
        4, 5, 6, 6, 7, 4, // front
        0, 4, 7, 7, 3, 0, // left
        1, 5, 6, 6, 2, 1, // right
        3, 2, 6, 6, 7, 3, // top
        0, 1, 5, 5, 4, 0, // bottom
    };

    public Renderer(GL gl)
    {
        _gl = gl;
        Logger.Info($"OpenGL version: {_gl.GetStringS(StringName.Version)}");
        Logger.Info($"GPU: {_gl.GetStringS(StringName.Renderer)}");

        _basicShader = new Shader(_gl, "Shaders/basic.vert", "Shaders/basic.frag");
        _cubeMesh = new Mesh(_gl, CubeVertices, CubeIndices);
    }

    public void SetScene(Scene scene) => _scene = scene;
    public void ClearColor(float r, float g, float b, float a) => _gl.ClearColor(r, g, b, a);

    public void Render(double delta, Camera? camera, Dictionary<int, PlayerState>? remotePlayers, int localPlayerId)
    {
        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        _gl.Enable(EnableCap.DepthTest);

        if (camera == null) return;

        var context = new RenderContext(camera, delta);
        DrawScene(context, remotePlayers, localPlayerId);
    }

    private void DrawScene(RenderContext ctx, Dictionary<int, PlayerState>? remotePlayers, int localPlayerId)
    {
        // Draw scene GameObjects that have a MeshComponent
        if (_scene != null)
        {
            foreach (var mesh in _scene.GetAllComponents<MeshComponent>())
                mesh.Draw(ctx);
        }

        // Draw remote players as cubes — temporary until players are GameObjects too
        if (remotePlayers == null) return;
        _basicShader.Use();
        _basicShader.SetMatrix("uView", ctx.View);
        _basicShader.SetMatrix("uProjection", ctx.Projection);
        foreach (var (id, state) in remotePlayers)
        {
            if (id == localPlayerId) continue;
            DrawCube(state.Position);
        }
    }

    private void DrawCube(Vector3D<float> position)
    {
        _basicShader.SetMatrix("uModel", Matrix4X4.CreateTranslation(position));
        _cubeMesh.Draw();
    }

    public void OnResize(Vector2D<int> size)
    {
        _gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);
        Logger.Debug($"Viewport resized to {size.X}x{size.Y}");
    }

    public void Dispose()
    {
        _basicShader.Dispose();
        _cubeMesh.Dispose();
        Logger.Info("Renderer disposed.");
    }
}