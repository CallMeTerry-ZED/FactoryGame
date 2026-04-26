using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using FactoryGame.Core.Log;

namespace FactoryGame.Client.UI;

public class ImGuiController : IDisposable
{
    private Silk.NET.OpenGL.Extensions.ImGui.ImGuiController _controller;

    public ImGuiController(GL gl, IView window, IInputContext input)
    {
        _controller = new Silk.NET.OpenGL.Extensions.ImGui.ImGuiController(gl, window, input);
        Logger.Debug("ImGuiController Initialized.");
    }
    
    public void Update(float delta) => _controller.Update(delta);
    public void Render() => _controller.Render();

    public void Dispose()
    {
        _controller?.Dispose();
        Logger.Debug("ImGuiController Disposed.");
    }
}