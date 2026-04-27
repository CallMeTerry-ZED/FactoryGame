namespace FactoryGame.Core.GOCS;

public abstract class Component
{
    // Set the GO that the component is attached to currently as its owner
    public GameObject Owner { get; internal set; } = null!;
    
    // Helper so components dont need to always do Owner.Scene
    public Scene Scene => Owner.Scene;
    
    public virtual void OnAttach() {}
    public virtual void OnUpdate() { }
    public virtual void OnDestroy() {}
    
    // Find siblings through owner
    public T? GetComponent<T>() where T : Component => Owner.GetComponent<T>();
}