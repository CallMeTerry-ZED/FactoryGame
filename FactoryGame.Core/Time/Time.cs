namespace FactoryGame.Core.Time;

public static class Time
{
    // Time since last frame/tick in seconds
    public static float DeltaTime { get; private set; }

    // Total time since the game started in seconds
    public static float Elapsed { get; private set; }

    // Total number of frames/ticks processed
    public static ulong TickCount { get; private set; }

    // Frames per second — smoothed over recent frames
    public static float Fps { get; private set; }

    // Fixed timestep for physics and server simulation (1/64 = 64tps)
    public const float FixedDeltaTime = 1f / 64f;

    private static float _fpsTimer;
    private static int _fpsFrameCount;

    // Called each frame by the client game loop or each tick by the server tick loop
    public static void Update(double deltaTime)
    {
        DeltaTime = (float)deltaTime;
        Elapsed += DeltaTime;
        TickCount++;

        // Update FPS counter every second
        _fpsTimer += DeltaTime;
        _fpsFrameCount++;

        if (_fpsTimer >= 1f)
        {
            Fps = _fpsFrameCount / _fpsTimer;
            _fpsFrameCount = 0;
            _fpsTimer = 0f;
        }
    }

    public static void Reset()
    {
        DeltaTime = 0f;
        Elapsed = 0f;
        TickCount = 0;
        Fps = 0f;
        _fpsTimer = 0f;
        _fpsFrameCount = 0;
    }
}