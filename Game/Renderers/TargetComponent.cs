using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;

namespace TestMaterialAsyncCompiler.Renderers
{
    [DataContract]
    public class TargetComponent : ActivableEntityComponent
    {
        [DataMemberIgnore]
        public Vector3 EntityPosition => GetCenter();

        private Vector3 GetCenter()
        {
            ModelComponent modelComponent = Entity.Components.Get<ModelComponent>();
            if (modelComponent != null)
            {
                var local = ModelCenter(Entity.Components.Get<ModelComponent>());
                //Vector3.TransformCoordinate(local,Transform);
                return local;
            }
            else
            {
                return Entity.Transform.Position;
            }
        }

        private static Vector3 ModelCenter(ModelComponent modelComponent)
        {
            var model = modelComponent.Model;
            if (model != null)
            {
                BoundingSphere sphere = model.BoundingSphere;
                var pos = sphere.Center;
                return pos;
            }
            return Vector3.Zero;
        }
    }
}