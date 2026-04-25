using Silk.NET.Input;

namespace FactoryGame.Core.Events.Types;

public class MouseMovedEvent : Event
{
    public float X { get; }
    public float Y { get; }
    public float DeltaX { get; }
    public float DeltaY { get; }

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
    public float ScrollDelta { get; }

    public MouseScrolledEvent(float scrollDelta)
    {
        ScrollDelta = scrollDelta;
    }

    protected override string Name => $"MouseScrolledEvent({ScrollDelta})";
}

public class MouseButtonEvent : Event
{
    public MouseButton Button { get; }
    public bool IsPressed { get; }

    public MouseButtonEvent(MouseButton button, bool isPressed)
    {
        Button = button;
        IsPressed = isPressed;
    }

    protected override string Name => $"MouseButtonEvent({Button}, Pressed={IsPressed})";
}