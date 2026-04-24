using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using FactoryGame.Core.Log;

namespace FactoryGame.Client;

public class Window
{
    private readonly IWindow _window;
    private GL? _gl;

    public Window(string title = "FactoryGame", int width = 800, int height = 600, bool vsync = false)
    {
        var options = WindowOptions.Default with
        {
            Title = title,
            Size = new Vector2D<int>(width, height),
            VSync = vsync,
            API = GraphicsAPI.Default // Default is OpenGL 3.3 core will want to look into changing this soon
        };
        
        _window = Silk.NET.Windowing.Window.Create(options);

        _window.Load += OnLoad;
        _window.Update += OnUpdate;
        _window.Render += OnRender;
        _window.Closing += OnClose;
    }
    
    public void Run() => _window.Run();

    private void OnLoad()
    {
        _gl = GL.GetApi(_window);
        Logger.Info($"OpenGL version: {_gl.GetStringS(StringName.Version)}");
        Logger.Info($"GPU: {_gl.GetStringS(StringName.Renderer)}");
        
        Logger.Info("Window loaded.");
    }

    private void OnUpdate(double delta)
    {
        // game loop
    }
    
    private void OnRender(double delta)
    {
        _gl!.ClearColor(0.1f, 0.1f, 0.1f, 1.0f); // dark gray
        _gl!.Clear(ClearBufferMask.ColorBufferBit);
    }

    private void OnClose()
    {
        Logger.Info("Window closed.");
    }
}