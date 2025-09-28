using Stride.Core.Mathematics;
using Stride.Engine;

namespace TestLoading;

public class Rotation : SyncScript
{
    public float MainRot { get; set; } = 0.8f;
    public float ChildRot { get; set; } = 3f;
    public override void Start()
    {
        MainRot = MainRot / 1000;
        ChildRot = ChildRot / 1000;

        Quaternion.RotationZ(0.45f, out Quaternion mainRotate);
        Entity.Transform.Rotation = mainRotate;
    }

    public override void Update()
    {
        Quaternion.RotationY(MainRot, out Quaternion mainRotate);
        Entity.Transform.Rotation *= mainRotate;

        if (Entity.GetChildren() is null) return;
        Quaternion.RotationY(ChildRot, out Quaternion childRotate);
        Entity.GetChild(0).Transform.Rotation *= childRotate;
    }
}
