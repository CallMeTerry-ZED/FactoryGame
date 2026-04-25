namespace FactoryGame.Core.Events.Types;

public class WindowResizedEvent : Event
{
    public int Width { get; }
    public int Height { get; }

    public WindowResizedEvent(int width, int height)
    {
        Width = width;
        Height = height;
    }

    protected override string Name => $"WindowResizedEvent({Width}x{Height})";
}

public class WindowClosedEvent : Event
{
    protected override string Name => "WindowClosedEvent";
}

public class WindowFocusEvent : Event
{
    public bool HasFocus { get; }

    public WindowFocusEvent(bool hasFocus)
    {
        HasFocus = hasFocus;
    }

    protected override string Name => $"WindowFocusEvent(HasFocus={HasFocus})";
}