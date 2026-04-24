using System.Numerics;
using Silk.NET.Input;
using FactoryGame.Core.Events;
using FactoryGame.Core.Events.Types;
using FactoryGame.Core.Log;

namespace FactoryGame.Core.Input;

public class Input : IDisposable
{
    private readonly IInputContext _context;
    private readonly IKeyboard? _keyboard;
    private readonly IMouse? _mouse;

    // Polling state
    private readonly HashSet<Key> _heldKeys = new();
    private readonly HashSet<MouseButton> _heldMouseButtons = new();
    private Vector2 _lastMousePos;

    public Input(IInputContext context)
    {
        _context = context;

        _keyboard = _context.Keyboards.FirstOrDefault();
        _mouse = _context.Mice.FirstOrDefault();

        if (_keyboard != null)
        {
            _keyboard.KeyDown += OnKeyDown;
            _keyboard.KeyUp += OnKeyUp;
        }
        else
        {
            Logger.Warn("Input: No keyboard detected.");
        }

        if (_mouse != null)
        {
            _mouse.MouseMove += OnMouseMove;
            _mouse.Scroll += OnMouseScroll;
            _mouse.MouseDown += OnMouseDown;
            _mouse.MouseUp += OnMouseUp;
        }
        else
        {
            Logger.Warn("Input: No mouse detected.");
        }

        Logger.Info("Input system initialized.");
    }

    // Polling API
    public bool IsKeyDown(Key key) => _heldKeys.Contains(key);
    public bool IsMouseButtonDown(MouseButton button) => _heldMouseButtons.Contains(button);

    // Raw Silk.NET handlers — translate to events
    private void OnKeyDown(IKeyboard kb, Key key, int scanCode)
    {
        bool isRepeat = _heldKeys.Contains(key);
        _heldKeys.Add(key);
        EventBus.Publish(new KeyPressedEvent(key, isRepeat));
    }

    private void OnKeyUp(IKeyboard kb, Key key, int scanCode)
    {
        _heldKeys.Remove(key);
        EventBus.Publish(new KeyReleasedEvent(key));
    }

    private void OnMouseMove(IMouse mouse, Vector2 pos)
    {
        float dx = pos.X - _lastMousePos.X;
        float dy = pos.Y - _lastMousePos.Y;
        _lastMousePos = new Vector2(pos.X, pos.Y);
        EventBus.Publish(new MouseMovedEvent(pos.X, pos.Y, dx, dy));
    }

    private void OnMouseScroll(IMouse mouse, ScrollWheel scroll)
    {
        EventBus.Publish(new MouseScrolledEvent(scroll.Y));
    }

    private void OnMouseDown(IMouse mouse, MouseButton button)
    {
        _heldMouseButtons.Add(button);
        EventBus.Publish(new MouseButtonEvent(button, true));
    }

    private void OnMouseUp(IMouse mouse, MouseButton button)
    {
        _heldMouseButtons.Remove(button);
        EventBus.Publish(new MouseButtonEvent(button, false));
    }

    public void Dispose()
    {
        if (_keyboard != null)
        {
            _keyboard.KeyDown -= OnKeyDown;
            _keyboard.KeyUp -= OnKeyUp;
        }

        if (_mouse != null)
        {
            _mouse.MouseMove -= OnMouseMove;
            _mouse.Scroll -= OnMouseScroll;
            _mouse.MouseDown -= OnMouseDown;
            _mouse.MouseUp -= OnMouseUp;
        }

        _context.Dispose();
        Logger.Info("Input system disposed.");
    }
}