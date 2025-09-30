// Based on Stride Community Toolkit Example #9
// https://github.com/stride3d/stride-community-toolkit/blob/08218340fb99b6d994e160d2d0af05bcce43fab1/examples/code-only/Example09_Renderer/MyCustomSceneRenderer.cs

using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Rendering.Compositing;
using TestMaterialAsyncCompiler.Helper;

namespace TestMaterialAsyncCompiler.Renderers
{
    public class TargetRenderer : SceneRendererBase, ISharedRenderer
    {
        public Texture TargetTexture;
        private SpriteBatch _spriteBatch;
        private Scene _scene;
        private CameraComponent _camera;

        protected override void InitializeCore()
        {
            base.InitializeCore();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void DrawCore(RenderContext context, RenderDrawContext drawContext)
        {
            var graphicsCompositor = context.Tags.Get(GraphicsCompositor.Current);

            if (graphicsCompositor is null) return;

            _camera ??= context.GetCurrentCamera();
            //_camera = context.GetCurrentCamera(); // if camera can be changed

            _scene ??= SceneInstance.GetCurrent(context).RootScene;
            //_scene = SceneInstance.GetCurrent(context).RootScene; // if scene can be changed

            if (_spriteBatch is null || _camera is null || _scene is null) return;

            var viewProjection = _camera.ViewProjectionMatrix;

            _spriteBatch.Begin(
                drawContext.GraphicsContext);

            foreach (var entity in _scene.Entities)
            {
                /* using Stride.Core;
                 * using Stride.Engine;
                 * [DataContract]
                 * public class TargetComponent : EntityComponent {}
                 * 
                 * can be add from code
                 */

                var agrComponent = entity.GetComponentInChildren<TargetComponent>();

                if (agrComponent is null) continue;
                if (!agrComponent.Enabled) continue;

                // original
                // var screenPosition = _camera.WorldToScreenPoint(ref entity.Transform.Position, GraphicsDevice);

                var worldPos = agrComponent.Entity.Transform.WorldMatrix.TranslationVector;

                // Transform to clip space
                var clipPosition = Vector4.Transform(new Vector4(worldPos, 1f), viewProjection);
                if (clipPosition.W <= 0f)
                    continue; // behind the camera

                // Perspective divide -> Normalized Device Coordinates (NDC)
                var inverseW = 1f / clipPosition.W;
                var normalizedDeviceCoordinatesX = clipPosition.X * inverseW;
                var normalizedDeviceCoordinatesY = clipPosition.Y * inverseW;
                var normalizedDeviceCoordinatesZ = clipPosition.Z * inverseW;

                // Clip test in NDC: x and y in [-1,1], z in [0,1]
                var outsideNdc =
                    normalizedDeviceCoordinatesZ < 0f ||
                    normalizedDeviceCoordinatesZ > 1f ||
                    normalizedDeviceCoordinatesX < -1f ||
                    normalizedDeviceCoordinatesX > 1f ||
                    normalizedDeviceCoordinatesY < -1f ||
                    normalizedDeviceCoordinatesY > 1f;

                // culled
                if (outsideNdc) continue;

                // implementation
                // start 
                Vector3.TransformCoordinate(ref worldPos, ref _camera.ViewProjectionMatrix, out Vector3 viewProject);

                Vector3.TransformCoordinate(ref worldPos, ref _camera.ViewMatrix, out Vector3 viewSpace);

                Vector3 result = new()
                {
                    X = (viewProject.X + 1f) / 2f,
                    Y = 1f - (viewProject.Y + 1f) / 2f,
                    Z = viewSpace.Z + _camera.NearClipPlane,
                };

                var windowSize = new Int2(
                    drawContext.GraphicsDevice.Presenter.BackBuffer.Width,
                    drawContext.GraphicsDevice.Presenter.BackBuffer.Height);

                Vector2 screenPosition = new()
                {
                    X = result.X * windowSize.X,
                    Y = result.Y * windowSize.Y
                };
                // end 

                // Draw a texture
                _spriteBatch.Draw(
                    TargetTexture,
                    new Rectangle(
                        (int)screenPosition.X - TargetTexture.Width / 2,  // horizontal center of texture in center of object
                        (int)screenPosition.Y - TargetTexture.Height / 2, // vertical center of texture in center of object
                        TargetTexture.Width,                            // rectangle width same as texture
                        TargetTexture.Height),                          // rectangle height same as texture
                    new(1.0f, 1.0f, 1.0f, 1.0f));                       // Color
            }

            // End the SpriteBatch
            _spriteBatch.End();
        }
    }
}