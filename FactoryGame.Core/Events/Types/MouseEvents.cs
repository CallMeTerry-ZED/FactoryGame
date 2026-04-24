using Silk.NET.Input;

namespace FactoryGame.Core.Events.Types;

public class MouseMovedEvent : Event
{
    private float X { get; }
    private float Y { get; }
    private float DeltaX { get; }
    private float DeltaY { get; }

    public MouseMovedEvent(float x, float y, float deltaX, float deltaY)
    {
        X = x;
        Y = y;
        DeltaX = deltaX;
        DeltaY = deltaY;
    }

    protected override string Name => $"MouseMovedEvent({X:F1}, {Y:F1}, Delta={DeltaX:F1},{DeltaY:F1})";
}

public class MouseScrolledEvent : Event
{
    private float ScrollDelta { get; }

    public MouseScrolledEvent(float scrollDelta)
    {
        ScrollDelta = scrollDelta;
    }

    protected override string Name => $"MouseScrolledEvent({ScrollDelta})";
}

public class MouseButtonEvent : Event
{
    private MouseButton Button { get; }
    private bool IsPressed { get; }

    public MouseButtonEvent(MouseButton button, bool isPressed)
    {
        Button = button;
        IsPressed = isPressed;
    }

    protected override string Name => $"MouseButtonEvent({Button}, Pressed={IsPressed})";
}