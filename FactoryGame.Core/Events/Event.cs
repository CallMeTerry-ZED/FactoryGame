namespace FactoryGame.Core.Events;

public abstract class Event
{
    public bool Handled { get; set; } = false;
    
    private DateTime Timestamp { get; set; } = DateTime.UtcNow; // Might want to look into just using DateTime.Now since it uses whatever the users local time is
    
    protected abstract string Name { get; }
    
    public override string ToString() => $"[{Timestamp:HH:mm:ss.fff}] {Name}";
}