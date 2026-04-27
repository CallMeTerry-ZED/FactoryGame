using FactoryGame.Core.GOCS;
using FactoryGame.Core.GOCS.Components;
using FactoryGame.Core.Log;
using FactoryGame.Client.Render;

namespace FactoryGame.Client.Components;

public class MeshComponent : Component
{
    public Mesh? Mesh { get; set; }
    public Shader? Shader { get; set; }
    
    public bool Visible { get; set; } = true;
    
    // Called by renderer
    public void Draw(RenderContext ctx)
    {
        if (!Visible || Mesh == null || Shader == null) return;
        
        var transform = GetComponent<TransformComponent>();
        if (transform == null)
        {
            Logger.Warn($"MeshComponent on '{Owner.Name}' has no TransformComponent.");
            return;
        }
        
        Shader.Use();
        Shader.SetMatrix("uModel", transform.GetModelMatrix());
        Shader.SetMatrix("uView", ctx.View);
        Shader.SetMatrix("uProjection", ctx.Projection);
        
        Mesh.Draw();
    }
    
    public override void OnAttach() => Logger.Debug($"MeshComponent attached to '{Owner.Name}'.");
    public override void OnDestroy() => Logger.Debug($"MeshComponent detached from '{Owner.Name}'.");
}