using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Rendering.Compositing;

namespace TestLoading.Renderers
{
    [Display("CircleProgressBar")]
    public class CircleProgressBar : SceneRendererBase
    {
        [DataMember(1)]
        public Texture LoadingTexture { get; set; }

        [DataMember(2)]
        public float TextureScale { get; set; } = 0.25f;

        [DataMember(3)]
        public float RotationSpeed { get; set; } = 1f;

        private SpriteBatch spriteBatch;
        private Sprite sprite;
        private float rotation = 15f;
        protected override void InitializeCore() => base.InitializeCore();

        protected override void CollectCore(RenderContext context)
        {
            base.CollectCore(context);

            // Инициализация SpriteBatch
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Создаем спрайт, если текстура установлена
            if (LoadingTexture != null && sprite == null)
            {
                sprite = new Sprite(LoadingTexture);
            }
        }

        protected override void DrawCore(RenderContext context, RenderDrawContext drawContext)
        {
            // Loading image
            if (sprite != null)
            {
                DrawProgressRotation(drawContext.GraphicsContext);
            }
        }

        void DrawProgressRotation(GraphicsContext graphicsContext)
        {
            var screenWidth = GraphicsDevice.Presenter.BackBuffer.Width;
            var screenHeight = GraphicsDevice.Presenter.BackBuffer.Height;

            // Begin - Draw - End
            spriteBatch.Begin(graphicsContext);

            spriteBatch.Draw(
                sprite.Texture,
                position: new Vector2(screenWidth - LoadingTexture.Width / 4 - 10, screenHeight - LoadingTexture.Height / 4 - 10),
                color: Color.White,
                rotation: rotation,
                origin: new Vector2(LoadingTexture.Width / 2, LoadingTexture.Height / 2),
                scale: TextureScale
                );

            spriteBatch.End();

            rotation = rotation + RotationSpeed/100;
        }
    }
}
