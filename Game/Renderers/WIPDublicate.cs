//private DebugTextSystem DebugText;
//var game = Services.GetService<IGame>().GameSystems;
//foreach (var _sys in game)
//{
//    if (_sys is DebugTextSystem system) DebugText = system;
//};

using System;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Rendering;
using Stride.Rendering.Compositing;
using Stride.Rendering.Materials;
using Stride.Graphics;
using Stride.Shaders.Compiler;
using Stride.Games;
using Stride.Profiling;
using Stride.Rendering.Materials.ComputeColors;
using Stride.Shaders;

namespace TestLoading.Renderers;

public class WIPDublicate : SceneRendererBase
{
    private Material fallbackColorMaterial;
    private GraphicsCompositor compositor;
    private bool showCompilingText = false;
    private DebugTextSystem DebugText;
    private MeshRenderFeature meshRenderFeature;

    protected override void InitializeCore()
    {
        base.InitializeCore();
        var game = Services.GetService<IGame>().GameSystems;
        foreach (var _sys in game)
        {
            if (_sys is DebugTextSystem system) DebugText = system;
        };
        compositor = Services.GetService<SceneSystem>().GraphicsCompositor;

        foreach (var feature in compositor.RenderFeatures)
        {
            if (feature is MeshRenderFeature _meshRenderFeature)
            {
                meshRenderFeature = _meshRenderFeature;
                _meshRenderFeature.ComputeFallbackEffect += FallbackForAsyncCompilation;
            }
        }
    }

    Effect FallbackForAsyncCompilation(RenderObject renderObject, RenderEffect renderEffect, RenderEffectState renderEffectState)
    {
        try
        {
            showCompilingText = true;
            var renderMesh = (RenderMesh)renderObject;
            fallbackColorMaterial ??= Material.New(GraphicsDevice, new MaterialDescriptor
            {
                Attributes = { Diffuse = new MaterialDiffuseMapFeature(new ComputeColor(new Color4(new Color3(0.65f, 0.02f, 0.02f)))),
                    DiffuseModel = new MaterialDiffuseLambertModelFeature(),
                    Emissive = new MaterialEmissiveMapFeature(new ComputeColor(new Color4(new Color3(0.65f, 0.02f, 0.02f)))),
                    Specular = new MaterialSpecularMapFeature(),
                    SpecularModel = new MaterialSpecularMicrofacetModelFeature() }
            });

            var compilerParameters = new CompilerParameters { EffectParameters = { TaskPriority = -1 } };
            compilerParameters.Set(MaterialKeys.PixelStageSurfaceShaders, fallbackColorMaterial.Passes[0].Parameters.Get(MaterialKeys.PixelStageSurfaceShaders));
            compilerParameters.Set(MaterialKeys.PixelStageStreamInitializer, fallbackColorMaterial.Passes[0].Parameters.Get(MaterialKeys.PixelStageStreamInitializer));
            compilerParameters.Set(LightingKeys.EnvironmentLights, [new ShaderClassSource("LightConstantWhite")]);
            renderEffect.FallbackParameters = new ParameterCollection(renderMesh.MaterialPass.Parameters);

            if (renderEffectState == RenderEffectState.Error)
            {
                renderEffect.RetryTime = DateTime.UtcNow + TimeSpan.FromSeconds(5);
            }
            // Показываем текст когда начинается компиляция           
            var compilationTask = EffectSystem.LoadEffect(renderEffect.EffectSelector.EffectName, compilerParameters);


            return compilationTask.WaitForResult();
        }
        catch (Exception)
        {
            return null;
        }
    }


    protected override void DrawCore(RenderContext context, RenderDrawContext drawContext)
    {
        if (showCompilingText)
        {
            DebugText.Print("Shader compiling...", new Int2(40, 40));
        }

        if (meshRenderFeature == null) return;

        meshRenderFeature.EffectCompiled = (RenderSystem, Effect, RenderEffectReflection) =>
        {
            DebugText.Print("Shader compiling...", new Int2(40, 80));
        };
        var feat = meshRenderFeature.ComputeFallbackEffect;

    }
}
