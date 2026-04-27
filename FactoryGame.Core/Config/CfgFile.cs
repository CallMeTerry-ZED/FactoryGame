using FactoryGame.Core.Log;

namespace FactoryGame.Core.Config;

public class CfgFile
{
    private readonly Dictionary<string, string> _values = new(StringComparer.OrdinalIgnoreCase);
    private readonly string _path;

    public CfgFile(string path)
    {
        _path = path;
    }
    
    // Load and parse a cfg file, Creates the file with defaults if it doesn't exist
    public void Load()
    {
        if (!File.Exists(_path))
        {
            Logger.Warn($"CfgFile: '{_path}' not found, it will be created with defaults on save.");
            return;
        }
        
        _values.Clear();

        foreach (var line in File.ReadAllLines(_path))
        {
            // Skip blank lines and comments
            var trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;
            if (trimmed.StartsWith("#") || trimmed.StartsWith("//")) continue;
            
            var separatorIndex = trimmed.IndexOf('=');

            if (separatorIndex < 1)
            {
                Logger.Warn($"CfgFile: Skipping malformed line: '{trimmed}'");
                continue;
            }
            
            var key = trimmed[..separatorIndex].Trim();
            var value = trimmed[(separatorIndex + 1)..].Trim();

            if (_values.ContainsKey(key))
                Logger.Warn($"CfgFile: Duplicate key '{key}', overwriting.");
            
            _values[key] = value;
        }
        
        Logger.Info($"CfgFile: Loaded '{_path}' ({_values.Count} entries).");
    }
    
    // Save current values back to the file
    public void Save()
    {
        var dir = Path.GetDirectoryName(_path);
        
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        
        var lines = _values.Select(kv => $"{kv.Key}={kv.Value}").ToList();
        
        File.WriteAllLines(_path, lines);
        Logger.Info($"CfgFile: Saved '{_path}' ({lines.Count} entries).");
    }
    
    // Getters with fallbacks to default values
    public string GetString(string key, string defaultValue = "")
    {
        if (_values.TryGetValue(key, out var value))
            return value;
        
        Logger.Debug($"CfgFile: Key '{key}' not found, using default '{defaultValue}'.");
        return defaultValue;
    }

    public int GetInt(string key, int defaultValue = 0)
    {
        if (_values.TryGetValue(key, out var value) && int.TryParse(value, out int result))
            return result;
        
        Logger.Debug($"CfgFile: Key '{key}' not found or invalid, using default '{defaultValue}'.");
        return defaultValue;
    }

    public float GetFloat(string key, float defaultValue = 0f)
    {
        if (_values.TryGetValue(key, out var value) && float.TryParse(value, out var result))
            return result;
        
        Logger.Debug($"CfgFile: Key '{key}' not found or invalid, using default '{defaultValue}'.");
        return defaultValue;
    }

    public bool GetBool(string key, bool defaultValue = false)
    {
        if (_values.TryGetValue(key, out var value))
        {
            if (value.Equals("true", StringComparison.OrdinalIgnoreCase) || value.Equals("1") || value.Equals("yes", StringComparison.OrdinalIgnoreCase))
                return true;

            if (value.Equals("false", StringComparison.OrdinalIgnoreCase) || value.Equals("0") || value.Equals("no", StringComparison.OrdinalIgnoreCase))
                return false;
        }
        
        Logger.Debug($"CfgFile: Key '{key}' not found or invalid, using default '{defaultValue}'.");
        return defaultValue;
    }
    
    // Setters
    public void SetString(string key, string value) => _values[key] = value;
    public void SetInt(string key, int value) => _values[key] = value.ToString();
    public void SetFloat(string key, float value) => _values[key] = value.ToString("G");
    public void SetBool(string key, bool value) => _values[key] = value ? "true" : "false";
    
    public bool HasKey(string key) => _values.ContainsKey(key);
}