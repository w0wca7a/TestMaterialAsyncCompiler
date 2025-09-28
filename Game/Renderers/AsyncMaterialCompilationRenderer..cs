using System;
using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Rendering;
using Stride.Rendering.Compositing;
using Stride.Rendering.Materials.ComputeColors;
using Stride.Rendering.Materials;
using Stride.Shaders;
using Stride.Graphics;
using Stride.Shaders.Compiler;

namespace TestLoading.Renderers;

[Display("AsyncMaterialCompiler")]
public class AsyncMaterialCompilationRenderer : SceneRendererBase
{
    [DataMember(1)]
    [DataMemberRange(-1, 100, 1, 10, 0)]
    public int CompileTaskPriority { get; set; } = -1;
        
    private Material fallbackColorMaterial;
    private GraphicsCompositor compositor;

    protected override void InitializeCore()
    {
        base.InitializeCore();

        compositor = Services.GetService<SceneSystem>().GraphicsCompositor;

        foreach (var feature in compositor.RenderFeatures)
        {
            if (feature is MeshRenderFeature meshRf)
            {
                meshRf.ComputeFallbackEffect += FallbackForAsyncCompilation;
            }
        }
    }

    protected override void CollectCore(RenderContext context) => base.CollectCore(context);

    protected override void DrawCore(RenderContext context, RenderDrawContext drawContext){}    

    // Like in https://gist.github.com/Eideren/ef6be9508d8d3b0e460d8a6d15f0937b
    Effect FallbackForAsyncCompilation(RenderObject renderObject, RenderEffect renderEffect, RenderEffectState renderEffectState)
    {
        try
        {
            var renderMesh = (RenderMesh)renderObject;
            fallbackColorMaterial ??= Material.New(GraphicsDevice, new MaterialDescriptor
            {
                Attributes =
                {
                    Diffuse = new MaterialDiffuseMapFeature(new ComputeColor(new Color4(new Color3(0.65f, 0.02f, 0.02f)))),
                    DiffuseModel = new MaterialDiffuseLambertModelFeature(),
                    Emissive = new MaterialEmissiveMapFeature(new ComputeColor(new Color4(new Color3(0.65f, 0.02f, 0.02f)))),
                    Specular = new MaterialSpecularMapFeature(),
                    SpecularModel = new MaterialSpecularMicrofacetModelFeature()
                }
            });


            var compilerParameters = new CompilerParameters { EffectParameters = { TaskPriority = CompileTaskPriority } };

            compilerParameters.Set(MaterialKeys.PixelStageSurfaceShaders, fallbackColorMaterial.Passes[0].Parameters.Get(MaterialKeys.PixelStageSurfaceShaders));
            compilerParameters.Set(MaterialKeys.PixelStageStreamInitializer, fallbackColorMaterial.Passes[0].Parameters.Get(MaterialKeys.PixelStageStreamInitializer));

            compilerParameters.Set(LightingKeys.EnvironmentLights, [new ShaderClassSource("LightConstantWhite")]);

            renderEffect.FallbackParameters = new ParameterCollection(renderMesh.MaterialPass.Parameters);

            if (renderEffectState == RenderEffectState.Error)
            {
                renderEffect.RetryTime = DateTime.UtcNow + TimeSpan.FromSeconds(5);
            }

            return EffectSystem.LoadEffect(renderEffect.EffectSelector.EffectName, compilerParameters).WaitForResult();
        }
        catch (Exception e)
        {
            renderEffect.State = RenderEffectState.Error;
            Console.WriteLine(e.ToString());
            return null;
        }
    }    
}
