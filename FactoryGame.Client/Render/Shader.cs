using System.Numerics;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using FactoryGame.Core.Assets;
using FactoryGame.Core.Log;

namespace FactoryGame.Client.Render;

public class Shader : IDisposable
{
    private readonly GL _gl;
    private readonly uint _program;
    // Cache uniform locations so we don't call GetUniformLocation every frame
    private readonly Dictionary<string, int> _uniformCache = new();

    public Shader(GL gl, string vertPath, string fragPath)
    {
        _gl = gl;
        var vertSource = AssetManager.LoadText(vertPath);
        var fragSource = AssetManager.LoadText(fragPath);
        var vert = Compile(ShaderType.VertexShader, vertSource);
        var frag = Compile(ShaderType.FragmentShader, fragSource);
        
        _program = gl.CreateProgram();
        _gl.AttachShader(_program, vert);
        _gl.AttachShader(_program, frag);
        _gl.LinkProgram(_program);
        
        _gl.GetProgram(_program, ProgramPropertyARB.LinkStatus, out var status);
        if (status == 0)
            Logger.Error($"Shader link error: {_gl.GetProgramInfoLog(_program)}");
        
        _gl.DeleteShader(vert);
        _gl.DeleteShader(frag);
        
        Logger.Debug($"Shader loaded: {vertPath}, {fragPath}");
    }
    
    public void Use() => _gl.UseProgram(_program);

    public void SetInt(string name, int value) => _gl.Uniform1(GetLocation(name), value);
    public void SetFloat(string name, float value) => _gl.Uniform1(GetLocation(name), value);
    public void SetVec3(string name, Vector3D<float> value) => _gl.Uniform3(GetLocation(name),  value.X, value.Y, value.Z);

    public unsafe void SetMatrix(string name, Matrix4X4<float> matrix)
    {
        // Pin the matrix directly to avoid manual float array flattening which can produce the wrong memory layout
        _gl.UniformMatrix4(GetLocation(name), 1, false, (float*)&matrix);
    }

    private int GetLocation(string name)
    {
        if (_uniformCache.TryGetValue(name, out var location))
            return location;
        
        location = _gl.GetUniformLocation(_program, name);
        if (location < 0)
            Logger.Warn($"Shader: Uniform '{name}' not found.");
        
        _uniformCache[name] = location;
        return location;
    }
    
    private uint Compile(ShaderType type, string source)
    {
        var shader = _gl.CreateShader(type);
        _gl.ShaderSource(shader, source);
        _gl.CompileShader(shader);

        _gl.GetShader(shader, ShaderParameterName.CompileStatus, out int status);
        if (status == 0)
            Logger.Error($"Shader compile error ({type}): {_gl.GetShaderInfoLog(shader)}");

        return shader;
    }

    public void Dispose()
    {
        _gl.DeleteProgram(_program);
        Logger.Debug("Shader disposed.");
    }
}