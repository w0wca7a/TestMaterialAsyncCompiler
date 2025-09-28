// Data: !float3 {X: -0.061862756, Y: 0.49075866, Z: 2.1321352}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;

namespace TestLoading;

public class SkyboxRotation : SyncScript
{
    public float MainRot { get; set; } = 0.15f;
    public override void Start()
    {
        MainRot = MainRot / 1000;
        Quaternion.RotationX(0.15f, out Quaternion mainRotate);
        Entity.Transform.Rotation = mainRotate;
    }

    public override void Update()
    {
        Quaternion.RotationY(MainRot, out Quaternion mainRotate);
        Entity.Transform.Rotation *= mainRotate;
    }
}
