using Silk.NET.OpenGL;
using FactoryGame.Core.Log;

namespace FactoryGame.Client.Render;

public class Mesh : IDisposable
{
    private readonly GL _gl;
    private readonly uint _vao;
    private readonly uint _vbo;
    private readonly uint _ebo;
    private readonly uint _indexCount;

    public Mesh(GL gl, float[] vertices, uint[] indices)
    {
        _gl = gl;
        _indexCount = (uint)indices.Length;
        _vao = _gl.GenVertexArray();
        _gl.BindVertexArray(_vao);
        
        _vbo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);

        unsafe
        {
            fixed (void* v = &vertices[0])
            {
                _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), v, BufferUsageARB.StaticDraw);
            }
        }
        
        _ebo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);

        unsafe
        {
            fixed (void* i = &indices[0])
            {
                _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Length * sizeof(uint)), i, BufferUsageARB.StaticDraw);
            }
        }
        
        // Position attributes - location 0, 3 floats, stride 3 floats
        unsafe
        {
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*)0);
        }
        _gl.EnableVertexAttribArray(0);
        
        Logger.Debug($"Mesh created ({vertices.Length / 3} vertices, {indices.Length / 3} triangles).");
    }

    public void Draw()
    {
        _gl.BindVertexArray(_vao);
        unsafe
        {
            _gl.DrawElements(PrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, null);
        }
    }

    public void Dispose()
    {
        _gl.DeleteVertexArray(_vao);
        _gl.DeleteBuffer(_vbo);
        _gl.DeleteBuffer(_ebo);
        Logger.Debug("Mesh disposed.");
    }
}