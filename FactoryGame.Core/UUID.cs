namespace FactoryGame.Core;

public readonly struct UUID : IEquatable<UUID>
{
    public static readonly UUID Empty = new (0);
    
    private readonly ulong _value;

    private UUID(ulong value)
    {
        _value = value;
    }
    
    // Generate a new unique ID from timestamp + random bits
    public static UUID New()
    {
        var timeStamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var random = (ulong)Random.Shared.NextInt64();
        
        return new UUID((timeStamp << 32) | (random & 0xFFFFFFFF));
    }
    
    // Reconstruct a UUID from a known value for deserialization
    public static UUID FromUlong(ulong value) => new(value);
    
    public ulong Value => _value;
    public bool Equals(UUID other) => _value == other._value;
    public override bool Equals(object? o) => o is UUID other && Equals(other);
    public override int GetHashCode() => _value.GetHashCode();
    public override string ToString() => _value.ToString("X16"); // Hex since its easier to read in logs

    public static bool operator ==(UUID a, UUID b) => a._value == b._value;
    public static bool operator !=(UUID a, UUID b) => a._value != b._value;
}