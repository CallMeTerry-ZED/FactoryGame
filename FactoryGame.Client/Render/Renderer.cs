using Silk.NET.OpenGL;
using FactoryGame.Core.Log;

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
        // Vertex shader — runs once per vertex, positions it on screen
        const string vertexSource = @"
            #version 460 core
            layout (location = 0) in vec3 aPosition;
            void main()
            {
                gl_Position = vec4(aPosition, 1.0);
            }";

        // Fragment shader — runs once per pixel, colors it
        const string fragmentSource = @"
            #version 460 core
            out vec4 FragColor;
            void main()
            {
                FragColor = vec4(1.0, 0.5, 0.2, 1.0); // orange
            }";

        var vert = CompileShader(ShaderType.VertexShader, vertexSource);
        var frag = CompileShader(ShaderType.FragmentShader, fragmentSource);

        _shaderProgram = _gl.CreateProgram();
        _gl.AttachShader(_shaderProgram, vert);
        _gl.AttachShader(_shaderProgram, frag);
        _gl.LinkProgram(_shaderProgram);

        // Check for link errors
        _gl.GetProgram(_shaderProgram, ProgramPropertyARB.LinkStatus, out int linkStatus);
        if (linkStatus == 0)
            Logger.Error($"Shader link error: {_gl.GetProgramInfoLog(_shaderProgram)}");

        // Individual shaders no longer needed once linked
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

    public void Render(double delta)
    {
        _gl.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        _gl.Clear(ClearBufferMask.ColorBufferBit);

        _gl.UseProgram(_shaderProgram);
        _gl.BindVertexArray(_vao);
        _gl.DrawArrays(PrimitiveType.Triangles, 0, 3);
    }

    public void Dispose()
    {
        _gl.DeleteVertexArray(_vao);
        _gl.DeleteBuffer(_vbo);
        _gl.DeleteProgram(_shaderProgram);
        Logger.Info("Renderer disposed.");
    }
}