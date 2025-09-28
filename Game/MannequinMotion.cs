using Stride.Core.Mathematics;
using Stride.Engine;

namespace TestLoading;

public class MannequinMotion : SyncScript
{
    private Quaternion StartRotation
        = new(
            -0.19749902f,
            -0.71465103f,
             0.22301053f,
             0.63288910f
            );
    private Quaternion EndRotation
        = new(
             0.39422834f,
             0.39637357f,
            -0.19383362f,
             0.80616410f
            );

    private Vector3 StartPosition = new(-0.032f, 0.028f, -0.022f);
    private Vector3 EndPosition = new(0.022f, -0.03f, -0.01f);

    public override void Start()
    {
        base.Start();
        Entity.Transform.Rotation = StartRotation;
        Entity.Transform.Position = StartPosition;
    }

    public override void Update()
    {
        float totalTime = 60.0f;
        float elapsedTime = (float)Game.UpdateTime.Total.TotalSeconds;
        float t = MathUtil.Clamp(elapsedTime / totalTime, 0.0f, 1.0f);
        t *= 1.8f;
        Quaternion.Slerp(in StartRotation, in EndRotation, t, out Quaternion currentRotation);
        Vector3.Lerp(in StartPosition, in EndPosition, t, out Vector3 currentposition);
        Entity.Transform.Rotation = currentRotation;
        Entity.Transform.Position = currentposition;
    }
}