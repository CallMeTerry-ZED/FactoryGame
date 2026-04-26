using FactoryGame.Core.Camera;
using Silk.NET.OpenGL;
using Silk.NET.Maths;
using FactoryGame.Core.Log;
using FactoryGame.Core.Assets;
using FactoryGame.Core.Net.Messages;

namespace FactoryGame.Client.Render;

public class Renderer : IDisposable
{
    private readonly GL _gl;

    // 8 unique vertices of a cube
    private static readonly float[] Vertices =
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
    private static readonly uint[] Indices =
    {
        0, 1, 2, 2, 3, 0, // back
        4, 5, 6, 6, 7, 4, // front
        0, 4, 7, 7, 3, 0, // left
        1, 5, 6, 6, 2, 1, // right
        3, 2, 6, 6, 7, 3, // top
        0, 1, 5, 5, 4, 0, // bottom
    };

    private uint _vao;
    private uint _vbo;
    private uint _ebo;
    private uint _shaderProgram;

    // Cube position in world space
    private Vector3D<float> _cubePosition = new(0f, 0f, -3f);
    
    public Renderer(GL gl)
    {
        _gl = gl;
        Logger.Info($"OpenGL version: {_gl.GetStringS(StringName.Version)}");
        Logger.Info($"GPU: {_gl.GetStringS(StringName.Renderer)}");

        SetupGeometry();
        SetupShaders();
    }

    private void SetupGeometry()
    {
        // VAO
        _vao = _gl.GenVertexArray();
        _gl.BindVertexArray(_vao);

        // VBO
        _vbo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);

        unsafe
        {
            fixed (float* v = Vertices)
            {
                _gl.BufferData(BufferTargetARB.ArrayBuffer,
                    (nuint)(Vertices.Length * sizeof(float)),
                    v,
                    BufferUsageARB.StaticDraw);
            }
        }
        
        // EBO
        _ebo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
        unsafe
        {
            fixed (uint* i = Indices)
            {
                _gl.BufferData(BufferTargetARB.ElementArrayBuffer,
                    (nuint)(Indices.Length * sizeof(uint)),
                    i,
                    BufferUsageARB.StaticDraw);
            }
        }

        // Tell OpenGL the vertex layout: attribute 0, 3 floats, not normalized, stride of 3 floats
        unsafe
        {
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*)0);
        }

        _gl.EnableVertexAttribArray(0);

        Logger.Debug("Geometry setup complete.");
    }

    private void SetupShaders()
    {
        var vertSource = AssetManager.LoadText("Shaders/basic.vert");
        var fragSource = AssetManager.LoadText("Shaders/basic.frag");

        var vert = CompileShader(ShaderType.VertexShader, vertSource);
        var frag = CompileShader(ShaderType.FragmentShader, fragSource);

        _shaderProgram = _gl.CreateProgram();
        _gl.AttachShader(_shaderProgram, vert);
        _gl.AttachShader(_shaderProgram, frag);
        _gl.LinkProgram(_shaderProgram);

        _gl.GetProgram(_shaderProgram, ProgramPropertyARB.LinkStatus, out int linkStatus);
        if (linkStatus == 0)
            Logger.Error($"Shader link error: {_gl.GetProgramInfoLog(_shaderProgram)}");

        _gl.DeleteShader(vert);
        _gl.DeleteShader(frag);

        Logger.Debug("Shaders compiled and linked.");
    }

    private uint CompileShader(ShaderType type, string source)
    {
        var shader = _gl.CreateShader(type);
        _gl.ShaderSource(shader, source);
        _gl.CompileShader(shader);

        _gl.GetShader(shader, ShaderParameterName.CompileStatus, out int status);
        if (status == 0)
            Logger.Error($"Shader compile error ({type}): {_gl.GetShaderInfoLog(shader)}");

        return shader;
    }

    public void Render(double delta, Camera? camera, Dictionary<int, PlayerState>? remotePlayers, int localPlayerId)
    {
        _gl.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        _gl.Enable(EnableCap.DepthTest);

        _gl.UseProgram(_shaderProgram);
        _gl.BindVertexArray(_vao);

        if (camera != null)
        {
            SetMatrix("uView", camera.GetViewMatrix());
            SetMatrix("uProjection", camera.GetProjectionMatrix());
            
            // Draw the test cube at world origin
            SetMatrix("uModel", Matrix4X4.CreateTranslation(_cubePosition));
            DrawCube();
            
            // TEMP: Draw a cube for each remote player, skip ourselves
            if (remotePlayers != null)
            {
                foreach (var (id, state) in remotePlayers)
                {
                    if (id == localPlayerId) continue;

                    var model = Matrix4X4.CreateTranslation(state.Position);
                    SetMatrix("uModel", model);
                    DrawCube();
                }
            }
        }

        unsafe
        {
            _gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
        }
        
        //_gl.DrawArrays(PrimitiveType.Triangles, 0, 3);
    }
    
    private unsafe void DrawCube()
    {
        _gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
    }

    private unsafe void SetMatrix(string name, Matrix4X4<float> matrix)
    {
        int location = _gl.GetUniformLocation(_shaderProgram, name);
        if (location < 0)
        {
            Logger.Warn($"Uniform '{name}' not found in shader.");
            return;
        }

        float[] values =
        {
            matrix.M11, matrix.M12, matrix.M13, matrix.M14,
            matrix.M21, matrix.M22, matrix.M23, matrix.M24,
            matrix.M31, matrix.M32, matrix.M33, matrix.M34,
            matrix.M41, matrix.M42, matrix.M43, matrix.M44,
        };
        
        fixed (float* ptr = values)
            _gl.UniformMatrix4(location, 1, false, ptr);
    }

    public void OnResize(Vector2D<int> size)
    {
        _gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);
        Logger.Debug($"Viewport resized to {size.X}x{size.Y}");
    }
    
    public void Dispose()
    {
        _gl.DeleteVertexArray(_vao);
        _gl.DeleteBuffer(_vbo);
        _gl.DeleteBuffer(_ebo);
        _gl.DeleteProgram(_shaderProgram);
        Logger.Info("Renderer disposed.");
    }
}