using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Silk.NET.Input;
using FactoryGame.Core.Log;
using FactoryGame.Client.Render;
using FactoryGame.Core.Input;
using FactoryGame.Core.Events;
using FactoryGame.Core.Events.Types;
using FactoryGame.Core.Time;

namespace FactoryGame.Client;

public class Window
{
    private readonly IWindow _window;
    private Renderer? _renderer;
    private Input? _input;
    private ClientNet? _net;

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
        
        _input = new Input(_window.CreateInput());
        
        _window.Resize += size => EventBus.Publish(new WindowResizedEvent(size.X, size.Y));
        _window.FocusChanged += hasFocus => EventBus.Publish(new WindowFocusEvent(hasFocus));
        
        EventBus.Subscribe<KeyPressedEvent>(OnKeyPressed);
        EventBus.Subscribe<WindowResizedEvent>(OnWindowResized);
        
        _net = new ClientNet();
        _net.Connect("127.0.0.1", "Player1");
        
        Logger.Info("Window loaded.");
    }

    private void OnUpdate(double delta)
    {
        Time.Update(delta);
        _net?.Poll();
    }
    
    private void OnRender(double delta)
    {
        _renderer?.Render(delta);
    }

    private void OnKeyPressed(KeyPressedEvent e)
    {
        if (e.Key == Key.Escape)
            _window.Close();
    }
    
    private void OnWindowResized(WindowResizedEvent e)
    {
        _renderer?.OnResize(new Vector2D<int>(e.Width, e.Height));
    }
    
    private void OnClose()
    {
        EventBus.Clear();
        _net?.Dispose();
        _input?.Dispose();
        _renderer?.Dispose();
        Logger.Info("Window closed.");
    }
}