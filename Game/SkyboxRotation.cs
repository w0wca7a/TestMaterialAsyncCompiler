using Stride.Core.Mathematics;
using Stride.Engine;

namespace TestMaterialAsyncCompiler;

public class SkyboxRotation : SyncScript
{
    public float MainRot { get; set; } = 0.15f;
    public override void Start()
    {
        MainRot /= 1000;
        Quaternion.RotationX(0.15f, out Quaternion mainRotate);
        Entity.Transform.Rotation = mainRotate;
    }

    public override void Update()
    {
        Quaternion.RotationY(MainRot, out Quaternion mainRotate);
        Entity.Transform.Rotation *= mainRotate;
    }
}
