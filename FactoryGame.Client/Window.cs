using Silk.NET.Maths;
using Silk.NET.Windowing;
using FactoryGame.Core.Log;

namespace FactoryGame.Client;

public class Window
{
    private readonly IWindow _window;

    public Window(string title = "FactoryGame", int width = 800, int height = 600, bool vsync = false)
    {
        var options = WindowOptions.Default with
        {
            Title = title,
            Size = new Vector2D<int>(width, height),
            VSync = vsync,
        };
        
        _window = Silk.NET.Windowing.Window.Create(options);

        _window.Load += OnLoad;
        _window.Update += OnUpdate;
        _window.Closing += OnClose;
    }
    
    public void Run() => _window.Run();

    private void OnLoad()
    {
        Logger.Info("Window loaded.");
    }

    private void OnUpdate(double delta)
    {
        
    }

    private void OnClose()
    {
        Logger.Info("Window closed.");
    }
}