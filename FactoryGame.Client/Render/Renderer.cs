using FactoryGame.Core.Camera;
using Silk.NET.OpenGL;
using Silk.NET.Maths;
using FactoryGame.Core.Log;
using FactoryGame.Core.Assets;

namespace FactoryGame.Client.Render;

public class Renderer : IDisposable
{
    private readonly GL _gl;

    // Triangle geom, X,Y,Z
    private static readonly float[] Vertices =
    {
        -0.5f, -0.5f, 0.0f, // bottom left
        0.5f, -0.5f, 0.0f, // bottom right
        0.0f, 0.5f, 0.0f, // top center
    };

    private uint _vao;
    private uint _vbo;
    private uint _shaderProgram;

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
        // VAO — remembers how the vertex data is laid out
        _vao = _gl.GenVertexArray();
        _gl.BindVertexArray(_vao);

        // VBO — sends the vertex data to the GPU
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

    public void Render(double delta, Camera? camera)
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
        }

        _gl.DrawArrays(PrimitiveType.Triangles, 0, 3);
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
    
    

    public void OnResize(Silk.NET.Maths.Vector2D<int> size)
    {
        _gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);
        Logger.Debug($"Viewport resized to {size.X}x{size.Y}");
    }
    
    public void Dispose()
    {
        _gl.DeleteVertexArray(_vao);
        _gl.DeleteBuffer(_vbo);
        _gl.DeleteProgram(_shaderProgram);
        Logger.Info("Renderer disposed.");
    }
}