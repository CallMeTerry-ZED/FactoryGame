using Silk.NET.Maths;

namespace FactoryGame.Core.Math;

// Convenient type aliases so game code never imports Silk.NET.Maths directly
using Vec2 = Vector2D<float>;
using Vec3 = Vector3D<float>;
using Vec4 = Vector4D<float>;
using Mat4 = Matrix4X4<float>;
using Quat = Quaternion<float>;

public static class Math
{
    // Constants
    public const float Pi = MathF.PI;
    public const float TwoPi = MathF.PI * 2f;
    public const float HalfPi = MathF.PI / 2f;
    public const float Deg2Rad = MathF.PI / 180f;
    public const float Rad2Deg = 180f / MathF.PI;
    public const float Epsilon = 1e-6f; //float.Epsilon;
    
    // Angle conversion
    public static float ToRadians(float degrees) => degrees * Deg2Rad;
    public static float ToDegrees(float radians) => radians * Rad2Deg;
    
    // Clamping
    public static float Clamp(float value, float min, float max) => MathF.Max(min, MathF.Min(max, value));
    public static int Clamp(int value, int min, int max) => System.Math.Max(min, System.Math.Min(max, value));
    
    // Interpolation
    public static float Lerp(float a, float b, float t) => a + (b - a) * Clamp(t, 0f, 1f);
    public static Vec3 Lerp(Vec3 a, Vec3 b, float t) => new(Lerp(a.X, b.X, t), Lerp(a.Y, b.Y, t), Lerp(a.Z, b.Z, t));
    
    // Unclamped lerp, for extrapolation in networking
    public static float LerpUnclamped(float a, float b, float t) => a + (b - a) * t;
    
    // Smooth step — nice for animations and UI
    public static float SmoothStep(float a, float b, float t)
    {
        t = Clamp((t - a) / (b - a), 0f, 1f);
        return t * t * (3f - 2f * t);
    }

    // Approximately equal — avoids floating point comparison pitfalls
    public static bool Approx(float a, float b) => MathF.Abs(a - b) < Epsilon;
    
    // Wrap angle to -180..180 range
    public static float WrapAngle(float degrees)
    {
        degrees %= 360f;
        if (degrees > 180f)  degrees -= 360f;
        if (degrees < -180f) degrees += 360f;
        return degrees;
    }

    // Common vector helpers
    public static float Dot(Vec3 a, Vec3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    public static Vec3 Cross(Vec3 a, Vec3 b) => new(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
    public static float Length(Vec3 v) => MathF.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);

    public static Vec3 Normalize(Vec3 v)
    {
        float len = Length(v);
        return len < Epsilon ? new Vec3(0, 0, 0) : new Vec3(v.X / len, v.Y / len, v.Z / len);
    }

    public static float Distance(Vec3 a, Vec3 b) => Length(new Vec3(b.X - a.X, b.Y - a.Y, b.Z - a.Z));
}