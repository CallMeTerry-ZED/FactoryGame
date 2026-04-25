using FactoryGame.Core.Log;

namespace FactoryGame.Core.Assets;

public static class AssetManager
{
    // Generic cache keyed by the asset path, can store any type of asset
    private static readonly Dictionary<string, object> _cache = new();
    
    // Load and cache a text based asset
    public static string LoadText(string path)
    {
        if (_cache.TryGetValue(path, out var cached))
        {
            Logger.Debug($"AssetManager: Cache hit for '{path}'");
            return (string)cached;
        }

        var text = AssetLoader.LoadText(path);
        _cache[path] = text;
        return text;
    }
    
    // Load and cache a binary based asset
    public static byte[] LoadBytes(string path)
    {
        if (_cache.TryGetValue(path, out var cached))
        {
            Logger.Debug($"AssetManager: Cache hit for '{path}'");
            return (byte[])cached;
        }

        var bytes = AssetLoader.LoadBytes(path);
        _cache[path] = bytes;
        return bytes;
    }
    
    // Remove a specific asset from the cache to free memory
    public static void Unload(string path)
    {
        if (_cache.Remove(path))
            Logger.Debug($"AssetManager: Unloaded '{path}'");
    }
    
    // Clear everything - Call on shutdown or any level unload
    public static void UnloadAll()
    {
        _cache.Clear();
        Logger.Info("AssetManager: All assets unloaded.");
    }
    
    public static bool IsCached(string path) => _cache.ContainsKey(path);
    public static int CachedCount => _cache.Count;
}