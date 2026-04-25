using Silk.NET.Input;

namespace FactoryGame.Core.Events.Types;

public class KeyPressedEvent : Event
{
    public Key Key { get; }
    public bool IsRepeat { get; }

    public KeyPressedEvent(Key key, bool isRepeat = false)
    {
        Key = key;
        IsRepeat = isRepeat;
    }

    protected override string Name => $"KeyPressedEvent({Key}, Repeat={IsRepeat})";
}

public class KeyReleasedEvent : Event
{
    public Key Key { get; }

    public KeyReleasedEvent(Key key)
    {
        Key = key;
    }

    protected override string Name => $"KeyReleasedEvent({Key})";
}