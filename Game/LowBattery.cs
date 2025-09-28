using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering;

namespace TestLoading
{
    public class LowBattery : StartupScript
    {
        public Texture Texture { get; set; }

    public override void Start()
        {
            base.Start();
            var model = Entity.Components.Get<ModelComponent>();
            model.Materials[1].Passes[0].Parameters.Set(LowBatteryKeys.Texture, Texture);
        }
    }
}
