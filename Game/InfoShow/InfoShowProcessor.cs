using Stride.Core.Annotations;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Games;
using Stride.UI;
using Stride.UI.Controls;

namespace TestLoading.InfoShow
{
    public class InfoShowProcessor : EntityProcessor<InfoShowComponent>
    {
        private UIComponent uicomp;
        public InfoShowProcessor() : base(typeof(TransformComponent)) { }

        public override void Update(GameTime time)
        {
            base.Update(time);
            TextBlock textBlock = null;
            if (uicomp != null)
            {
                textBlock = uicomp.Page.RootElement.FindVisualChildOfType<TextBlock>("DebugText");
                if (textBlock == null) return;
            }
            foreach (var kvp in ComponentDatas)
            {
                var component = kvp.Key;
                if (component.HideInfo)
                {
                    textBlock.Visibility = Visibility.Hidden;
                    return;
                }
                else
                {
                    textBlock.Visibility = Visibility.Visible;
                }
                if (!component.AutoUpdate) return;
                UpdateDisplayInfo(component, ref textBlock);
            }
        }

        private static void UpdateDisplayInfo(InfoShowComponent component, ref TextBlock textBlock)
        {
            component.Entity.Transform.GetWorldTransformation(
                out Vector3 worldPosition, 
                out Quaternion worldRotation, 
                out _);

            textBlock.Text =
                "Local ---\n"
                + "Rotation" + component.Entity.Transform.Rotation.ToString() + "\n"
                + "Position" + component.Entity.Transform.Position.ToString() + "\n"
                + "World ---\n"
                + "Rotation" + worldRotation.ToString() + "\n"
                + "Position" + worldPosition.ToString();
        }

        protected override void OnEntityComponentAdding(Entity entity, [NotNull] InfoShowComponent component, [NotNull] InfoShowComponent data)
        {
            base.OnEntityComponentAdding(entity, component, data);
            uicomp = entity.Components.Get<UIComponent>();
        }

        protected override void OnEntityComponentRemoved(Entity entity, [NotNull] InfoShowComponent component, [NotNull] InfoShowComponent data)
        {
            base.OnEntityComponentRemoved(entity, component, data);
            uicomp = null;
        }
    }
}
