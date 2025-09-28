using System;
using Stride.Core;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Rendering.Compositing;
using Stride.Rendering.Images;

namespace TestLoading.Renderers
{
    public class WIPImageEffectRenderer : IImageEffectRenderer, ISharedRenderer, IImageEffect
    {
        public string Name => typeof(WIPImageEffectRenderer).Name;

        public Guid Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool Enabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool Initialized => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Draw(RenderDrawContext context)
        {
            throw new NotImplementedException();
        }

        public void Initialize(RenderContext context)
        {
            throw new NotImplementedException();
        }

        public void SetInput(int slot, Texture texture)
        {
            throw new NotImplementedException();
        }

        public void SetOutput(Texture view)
        {
            throw new NotImplementedException();
        }

        public void SetOutput(params Texture[] views)
        {
            throw new NotImplementedException();
        }

        public void SetViewport(Viewport? viewport)
        {
            throw new NotImplementedException();
        }
    }
}
