using FactoryGame.Core.Log;
using FactoryGame.Core.GOCS.Components;

namespace FactoryGame.Core.GOCS;

public sealed class GameObject
{
    public UUID Id { get; }
    public string Name { get; set; }
    public Scene Scene { get; }
    public bool Active { get; set; } = true;
    
    // Marked for end-of-frame destruction
    internal bool PendingDestroy { get; private set; } = false;

    private readonly List<Component> _components = new();
    
    public TransformComponent Transform { get; }

    internal GameObject(UUID id, string name, Scene scene)
    {
        Id = id;
        Name = name;
        Scene = scene;
        
        // To avoid circular call during construction the transform comp is always present
        Transform = new TransformComponent();
        Transform.Owner = this;
        _components.Add(Transform);
        Transform.OnAttach();
    }

    public T AddComponent<T>() where T : Component, new()
    {
        var component = new T();
        component.Owner = this;
        _components.Add(component);
        component.OnAttach();
        
        Logger.Debug($"GameObject '{Name}': Added component {typeof(T).Name}.");
        return component;
    }

    public T? GetComponent<T>() where T : Component
    {
        foreach (var c in _components)
            if (c is T typed)
                return typed;
        return null;
    }

    public bool HasComponent<T>() where T : Component => GetComponent<T>() != null;

    public void RemoveComponent<T>() where T : Component
    {
        var component = GetComponent<T>();
        if (component == null) return;
        
        component.OnDestroy();
        _components.Remove(component);
        
        Logger.Debug($"GameObject '{Name}': Removed component {typeof(T).Name}.");
    }
    
    public IReadOnlyList<Component> GetAllComponents() => _components;

    internal void Update()
    {
        if (!Active) return;

        foreach (var c in _components)
            c.OnUpdate();
    }

    internal void Destroy()
    {
        PendingDestroy = true;
    }

    internal void OnDestroyed()
    {
        foreach (var c in _components)
            c.OnDestroy();
        
        Logger.Debug($"GameObject '{Name}' ({Id}) destroyed.");
    }
}