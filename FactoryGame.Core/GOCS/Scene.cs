using FactoryGame.Core.Log;

namespace FactoryGame.Core.GOCS;

public sealed class Scene
{
    public string Name { get; }

    private readonly List<GameObject> _objects = new();
    private readonly List<GameObject> _pendingAdd = new();
    private readonly List<GameObject> _pendingDestroy = new();

    public Scene(string name)
    {
        Name = name;
        Logger.Info($"Scene '{Name}' created.");
    }
    
    // Create and reg a new GO in the scene
    public GameObject CreateObject(string name = "GameObject")
    {
        var go = new GameObject(UUID.New(), name, this);
        _pendingAdd.Add(go);
        
        Logger.Debug($"Scene '{Name}': Created GameObject '{name}' ({go.Id}).");
        return go;
    }
    
    // Mark an object for end-of-frame destruction
    public void Destroy(GameObject go)
    {
        go.Destroy();
        _pendingDestroy.Add(go);
    }

    public void Update()
    {
        // Flush any newly created objects into the main list
        if (_pendingAdd.Count > 0)
        {
            _objects.AddRange(_pendingAdd);
            _pendingAdd.Clear();
        }
        
        // Update all GOs
        foreach (var go in _objects)
            go.Update();
        
        // Flush destroyed objects
        if (_pendingDestroy.Count > 0)
        {
            foreach (var go in _pendingDestroy)
            {
                go.OnDestroyed();
                _objects.Remove(go);
            }
            
            _pendingDestroy.Clear();
        }
    }
    
    // Query helpers
    public GameObject? FindByName(string name) => _objects.FirstOrDefault(o => o.Name == name);
    public GameObject? FindById(UUID id) => _objects.FirstOrDefault(o => o.Id == id);
    public IEnumerable<T> GetAllComponents<T>() where T : Component => _objects.SelectMany(o => o.GetAllComponents()).OfType<T>();
    public IReadOnlyList<GameObject> GetAllComponents() => _objects;

    public void Clear()
    {
        foreach (var go in _objects)
            go.OnDestroyed();
        
        _objects.Clear();
        _pendingAdd.Clear();
        _pendingDestroy.Clear();
        
        Logger.Info($"Scene '{Name}' cleared.");
    }
}