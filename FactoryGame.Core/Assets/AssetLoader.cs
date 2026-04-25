using FactoryGame.Core.Log;

namespace FactoryGame.Core.Assets;

public static class AssetLoader
{
    private static string _assetRoot = "Assets";
    
    // Call on startup to override default path
    public static void SetAssetRoot(string path)
    {
        _assetRoot = path;
        Logger.Info($"AssetLoader: Asset root set to '{Path.GetFullPath(_assetRoot)}'");
    }
    
    public static string ResolvePath(string relativePath)
    {
        return Path.Combine(_assetRoot, relativePath);
    }
    
    public static string LoadText(string relativePath)
    {
        var fullPath = ResolvePath(relativePath);

        if (!File.Exists(fullPath))
        {
            Logger.Error($"AssetLoader: File not found: '{fullPath}'");
            throw new FileNotFoundException($"Asset not found: {relativePath}", fullPath);
        }

        Logger.Debug($"AssetLoader: Loading text asset '{relativePath}'");
        return File.ReadAllText(fullPath);
    }

    public static byte[] LoadBytes(string relativePath)
    {
        var fullPath = ResolvePath(relativePath);

        if (!File.Exists(fullPath))
        {
            Logger.Error($"AssetLoader: File not found: '{fullPath}'");
            throw new FileNotFoundException($"Asset not found: {relativePath}", fullPath);
        }
        
        Logger.Debug($"AssetLoader: Loading binary asset '{relativePath}'");
        return File.ReadAllBytes(fullPath);
    }
    
    public static bool Exists(string relativePath) => File.Exists(ResolvePath(relativePath));
}