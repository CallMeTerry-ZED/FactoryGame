using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using FactoryGame.Core.Log;
using FactoryGame.Client.Render;

namespace FactoryGame.Client;

public class Window
{
    private readonly IWindow _window;
    private Renderer? _renderer;

    public Window(string title = "FactoryGame", int width = 800, int height = 600, bool vsync = false)
    {
        var options = WindowOptions.Default with
        {
            Title = title,
            Size = new Vector2D<int>(width, height),
            VSync = vsync,
            API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.Default, new APIVersion(4, 6))
            //API = GraphicsAPI.Default // Default is OpenGL 3.3 core will want to look into changing this soon
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
        var gl = GL.GetApi(_window);
        _renderer = new Renderer(gl);
        Logger.Info("Window loaded.");
    }

    private void OnUpdate(double delta)
    {
        // game loop
    }
    
    private void OnRender(double delta)
    {
        _renderer?.Render(delta);
    }

    private void OnClose()
    {
        _renderer?.Dispose();
        Logger.Info("Window closed.");
    }
}