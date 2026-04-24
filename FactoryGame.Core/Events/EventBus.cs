using FactoryGame.Core.Log;

namespace FactoryGame.Core.Events;

public static class EventBus
{
    // Maps event type -> list of handlers
    private static readonly Dictionary<Type, List<Delegate>> _handlers = new();

    public static void Subscribe<T>(Action<T> handler) where T : Event
    {
        var type = typeof(T);
        if (!_handlers.ContainsKey(type))
            _handlers[type] = new List<Delegate>();

        _handlers[type].Add(handler);
        Logger.Debug($"EventBus: Subscribed to {type.Name}");
    }
    
    public static void Unsubscribe<T>(Action<T> handler) where T : Event
    {
        var type = typeof(T);
        if (_handlers.TryGetValue(type, out var list))
        {
            list.Remove(handler);
            Logger.Debug($"EventBus: Unsubscribed from {type.Name}");
        }
    }
    
    public static void Publish<T>(T e) where T : Event
    {
        var type = typeof(T);
        if (!_handlers.TryGetValue(type, out var list))
            return;

        // Iterate a copy so handlers can safely unsubscribe during dispatch
        foreach (var handler in list.ToArray())
        {
            if (e.Handled) break;
            ((Action<T>)handler)(e);
        }
    }
    
    public static void Clear()
    {
        _handlers.Clear();
        Logger.Debug("EventBus: All handlers cleared.");
    }
}