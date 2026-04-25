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
using FactoryGame.Core.Camera;
using FactoryGame.Core.Assets;

namespace FactoryGame.Client;

public class Window
{
    private readonly IWindow _window;
    private Renderer? _renderer;
    private Input? _input;
    private ClientNet? _net;
    private Camera? _camera;
    private bool _mouseCaptured = false;

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
        
        _camera = new Camera
        {
            AspectRatio = (float)_window.Size.X / _window.Size.Y
        };
        
        EventBus.Subscribe<KeyPressedEvent>(OnKeyPressed);
        EventBus.Subscribe<WindowResizedEvent>(OnWindowResized);
        EventBus.Subscribe<MouseMovedEvent>(OnMouseMoved);
        
        _net = new ClientNet();
        _net.Connect("127.0.0.1", "Player1");
        
        // Capture mouse on start
        SetMouseCaptured(true);
        
        Logger.Info("Window loaded.");
    }

    private void OnUpdate(double delta)
    {
        Time.Update(delta);
        _net?.Poll();
        
        if (_camera == null || _input == null) return;
        
        const float speed = 5f;
        float velocity = speed * Time.DeltaTime;
        
        if (_input.IsKeyDown(Key.W))
            _camera.Position += _camera.Forward * velocity;
        if (_input.IsKeyDown(Key.S))
            _camera.Position -= _camera.Forward * velocity;
        if (_input.IsKeyDown(Key.A))
            _camera.Position -= _camera.Right * velocity;
        if (_input.IsKeyDown(Key.D))
            _camera.Position += _camera.Right * velocity;
        if (_input.IsKeyDown(Key.Space))
            _camera.Position += new Vector3D<float>(0, 1, 0) * velocity;
        if (_input.IsKeyDown(Key.ShiftLeft))
            _camera.Position -= new Vector3D<float>(0, 1, 0) * velocity;
    }
    
    private void OnRender(double delta)
    {
        _renderer?.Render(delta, _camera);
    }

    private void OnKeyPressed(KeyPressedEvent e)
    {
        if (e.Key == Key.Escape)
            _window.Close();
        
        if (e.Key == Key.Tab)
            SetMouseCaptured(!_mouseCaptured);
    }
    
    private void OnMouseMoved(MouseMovedEvent e)
    {
        if (!_mouseCaptured || _camera == null) return;

        const float sensitivity = 0.1f;
        _camera.AddRotation(e.DeltaX * sensitivity, -e.DeltaY * sensitivity);
    }
    
    private void OnWindowResized(WindowResizedEvent e)
    {
        _renderer?.OnResize(new Vector2D<int>(e.Width, e.Height));
        
        if (_camera != null)
            _camera.AspectRatio = (float)e.Width / e.Height;
    }
    
    private void SetMouseCaptured(bool captured)
    {
        _mouseCaptured = captured;

        if (_input == null) return;

        var mouse = _input.SilkMouse;
        if (mouse == null) return;

        mouse.Cursor.CursorMode = captured ? CursorMode.Raw : CursorMode.Normal;
    }
    
    private void OnClose()
    {
        EventBus.Clear();
        _net?.Dispose();
        _input?.Dispose();
        _renderer?.Dispose();
        AssetManager.UnloadAll();
        Logger.Info("Window closed.");
    }
}