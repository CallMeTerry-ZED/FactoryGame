using ImGuiNET;
using FactoryGame.Core.Camera;
using FactoryGame.Core.Time;
using FactoryGame.Core.Net.Messages;
using System.Numerics;

namespace FactoryGame.Client.UI;

public class DebugOverlay
{
    private bool _visible = true;

    public bool Visible
    {
        get => _visible;
        set => _visible = value;
    }

    public void Toggle() => _visible = !_visible;

    public void Draw(Camera? camera, Dictionary<int, PlayerState>? remotePlayers, int localId, bool connected)
    {
        if (!_visible) return;

        ImGui.SetNextWindowPos(new System.Numerics.Vector2(10, 10), ImGuiCond.Always);
        ImGui.SetNextWindowBgAlpha(0.75f);

        ImGui.Begin("Debug",
            ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.NoCollapse |
            ImGuiWindowFlags.NoTitleBar |
            ImGuiWindowFlags.AlwaysAutoResize);

        // Performance
        ImGui.TextColored(new System.Numerics.Vector4(0.4f, 1f, 0.4f, 1f), "Performance");
        ImGui.Separator();
        ImGui.Text($"FPS:        {Time.Fps:F1}");
        ImGui.Text($"Delta:      {Time.DeltaTime * 1000f:F2} ms");
        ImGui.Text($"Tick:       {Time.TickCount}");
        ImGui.Spacing();

        // Local player
        ImGui.TextColored(new System.Numerics.Vector4(0.4f, 0.8f, 1f, 1f), "Local Player");
        ImGui.Separator();
        if (camera != null)
        {
            var pos = camera.Position;
            ImGui.Text($"ID:         {localId}");
            ImGui.Text($"Pos:        {pos.X:F2}, {pos.Y:F2}, {pos.Z:F2}");
            ImGui.Text($"Yaw:        {camera.Yaw:F1}");
            ImGui.Text($"Pitch:      {camera.Pitch:F1}");
        }

        ImGui.Spacing();

        // Network
        ImGui.TextColored(new System.Numerics.Vector4(1f, 0.8f, 0.4f, 1f), "Network");
        ImGui.Separator();
        ImGui.Text($"Connected:  {(connected ? "Yes" : "No")}");
        ImGui.Text($"Players:    {(remotePlayers?.Count ?? 0) + (connected ? 1 : 0)}");

        // Remote players — scrollable so it never overflows offscreen
        if (remotePlayers != null && remotePlayers.Count > 0)
        {
            ImGui.Spacing();
            ImGui.TextColored(new System.Numerics.Vector4(1f, 0.8f, 0.4f, 1f), "Remote Players");
            ImGui.Separator();

            // Max height of 150px before it scrolls
            ImGui.BeginChild("RemotePlayers", new System.Numerics.Vector2(280, 150));
            foreach (var (id, state) in remotePlayers)
            {
                if (id == localId) continue;
                ImGui.Text($"[{id}] {state.Name}");
                ImGui.Text($"     {state.Position.X:F1}, {state.Position.Y:F1}, {state.Position.Z:F1}");
                ImGui.Spacing();
            }

            ImGui.EndChild();
        }

        ImGui.End();
    }
}