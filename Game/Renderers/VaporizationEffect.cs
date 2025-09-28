using Stride.Core;
using Stride.Rendering;
using Stride.Rendering.Images;

namespace TestMaterialAsyncCompiler.Renderers;

[DataContract("VaporizationEffect")]
public class VaporizationEffect : ColorTransform
{
    public VaporizationEffect() : base("VaporizationEffect") { }

    private float Time = 1f;

    public override void UpdateParameters(ColorTransformContext context)
    {
        Time += 0.1f;
        if (Time > 100f) Time = 1f;

        Parameters.Set(VaporizationEffectKeys.Time, value: Time);
        base.UpdateParameters(context);
    }
}