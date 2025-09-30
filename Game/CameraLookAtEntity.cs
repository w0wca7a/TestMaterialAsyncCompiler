using Stride.Core.Mathematics;
using Stride.Engine;

namespace TestMaterialAsyncCompiler
{
    public class CameraLookAtEntitySimple : SyncScript
    {
        private Quaternion targetRotation;
        private BasicCameraController basicCameraController;

        public Entity EntityToFollow { get; set; }
        public float Speed { get; set; } = 1;
        public override void Start()
        {
            base.Start();
            basicCameraController = new();
            Entity.RemoveAll<BasicCameraController>();
        }
        public override void Update()
        {
            /// World positions
            Vector3 cameraWorldPos = Entity.Transform.WorldMatrix.TranslationVector;
            Vector3 targetWorldPos = EntityToFollow.Transform.WorldMatrix.TranslationVector;

            // Direction in world space
            Vector3 worldDir = Vector3.Normalize(targetWorldPos - cameraWorldPos);

            // Convert world direction into player's local space
            Matrix playerWorldMatrix = Entity.Transform.WorldMatrix;
            Matrix playerWorldInverse = Matrix.Invert(playerWorldMatrix);
            Vector3 localDir = Vector3.TransformNormal(worldDir, playerWorldInverse);
            localDir.Normalize();

            // Camera forward
            Vector3 fwd = -localDir;
            Vector3 up = Vector3.UnitY;
            Vector3 right = Vector3.Normalize(Vector3.Cross(up, fwd));
            up = Vector3.Cross(fwd, right);

            // Build rotation matrix in local space
            Matrix localRotationMatrix = new(
                            right.X, right.Y, right.Z, 0,
                            up.X, up.Y, up.Z, 0,
                            fwd.X, fwd.Y, fwd.Z, 0,
                            0, 0, 0, 1
            );

            targetRotation = Quaternion.RotationMatrix(localRotationMatrix);
            var go = Game.UpdateTime.Total.TotalSeconds;
            if (go > 10 && go < 28)
            {
                // Current rotation
                Quaternion currentRotation = Entity.Transform.Rotation;

                // Interpolate (higher = faster, lower = slower)
                float smoothFactor = 0.2f;
                Quaternion smoothed = Quaternion.Slerp(currentRotation, targetRotation, smoothFactor);

                // Apply smoothed rotation        
                Entity.Transform.Rotation = smoothed;
            }
            if (go > 30)
            {
                if (Entity.Components.Contains(basicCameraController)) return;
                Entity.Add(basicCameraController);
            }
        }
    }
}