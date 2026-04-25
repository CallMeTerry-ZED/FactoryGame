using Silk.NET.Maths;
using FactoryGame.Core.Math;

namespace FactoryGame.Core.Camera;

public class Camera
{
    private static readonly Vector3D<float> WorldUp = new(0f, 1f, 0f);

    // Position in world space
    public Vector3D<float> Position { get; set; } = new(0f, 1.8f, 0f); // eye height

    // Euler angles in degrees
    public float Yaw { get; private set; } = -90f; // -90 so we start looking forward (down -Z)
    public float Pitch { get; private set; } = 0f;

    // Perspective settings
    public float Fov { get; set; } = 70f;
    public float AspectRatio { get; set; } = 16f / 9f;
    public float NearPlane { get; set; } = 0.1f;
    public float FarPlane { get; set; } = 1000f;

    // Derived direction vectors — rebuilt whenever yaw/pitch changes
    public Vector3D<float> Forward { get; private set; }
    public Vector3D<float> Right { get; private set; }
    public Vector3D<float> Up { get; private set; }

    public Camera()
    {
        UpdateVectors();
    }

    public void SetRotation(float yaw, float pitch)
    {
        Yaw = Math.Math.WrapAngle(yaw);
        Pitch = Math.Math.Clamp(pitch, -89f, 89f); // clamp so you cant flip over
        UpdateVectors();
    }

    public void AddRotation(float deltaYaw, float deltaPitch)
    {
        SetRotation(Yaw + deltaYaw, Pitch + deltaPitch);
    }

    // View matrix — where the camera is and what it's looking at
    public Matrix4X4<float> GetViewMatrix()
    {
        var target = new Vector3D<float>(
            Position.X + Forward.X,
            Position.Y + Forward.Y,
            Position.Z + Forward.Z);

        return CreateLookAt(Position, target, Up);
    }

    // Projection matrix — how 3D maps to 2D screen
    public Matrix4X4<float> GetProjectionMatrix()
    {
        return CreatePerspective(
            Math.Math.ToRadians(Fov),
            AspectRatio,
            NearPlane,
            FarPlane);
    }

    private void UpdateVectors()
    {
        float yawRad = Math.Math.ToRadians(Yaw);
        float pitchRad = Math.Math.ToRadians(Pitch);

        var forward = new Vector3D<float>(
            MathF.Cos(yawRad) * MathF.Cos(pitchRad),
            MathF.Sin(pitchRad),
            MathF.Sin(yawRad) * MathF.Cos(pitchRad));

        Forward = Math.Math.Normalize(forward);
        Right = Math.Math.Normalize(Math.Math.Cross(Forward, WorldUp));
        Up = Math.Math.Normalize(Math.Math.Cross(Right, Forward));
    }

    // Matrix helpers
    // Silk.NET.Maths has these, but it's easier to have them here so we aren't fighting with generic type args everywhere
    private static Matrix4X4<float> CreateLookAt(
        Vector3D<float> eye,
        Vector3D<float> target,
        Vector3D<float> up)
    {
        var f = Math.Math.Normalize(new Vector3D<float>(
            target.X - eye.X,
            target.Y - eye.Y,
            target.Z - eye.Z));

        var r = Math.Math.Normalize(Math.Math.Cross(f, up));
        var u = Math.Math.Cross(r, f);

        return new Matrix4X4<float>(
            r.X, u.X, -f.X, 0f,
            r.Y, u.Y, -f.Y, 0f,
            r.Z, u.Z, -f.Z, 0f,
            -Math.Math.Dot(r, eye), -Math.Math.Dot(u, eye), Math.Math.Dot(f, eye), 1f);
    }

    private static Matrix4X4<float> CreatePerspective(float fovRad, float aspect, float near, float far)
    {
        float tanHalfFov = MathF.Tan(fovRad / 2f);

        return new Matrix4X4<float>(
            1f / (aspect * tanHalfFov), 0f, 0f, 0f,
            0f, 1f / tanHalfFov, 0f, 0f,
            0f, 0f, -(far + near) / (far - near), -1f,
            0f, 0f, -(2f * far * near) / (far - near), 0f);
    }
}