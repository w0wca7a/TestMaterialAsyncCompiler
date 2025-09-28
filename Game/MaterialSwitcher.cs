using System.Collections.Generic;
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Core.Serialization;
using Stride.Engine;
using Stride.Rendering;
using Stride.Rendering.Materials.ComputeColors;
using Stride.Rendering.Materials;
using Stride.UI.Controls;
using Stride.Rendering.Sprites;

namespace TestLoading
{
    [DataContract("UIHandler")]
    [Display("UI Page Handler")]
    [ComponentCategory("UI")]
    public class UIHandler : SyncScript
    {
        [DataMember]
        public List<UrlReference<Material>> Materials { get; set; } = new List<UrlReference<Material>>(5);
        public ModelComponent TargetModel { get; set; }
        private Button leftButton;
        private Button rightButton;
        private ToggleButton fullScreenButton;
        //private ToggleButton additionalButton;
        private int matCount;
        private int curMaterial = 0;
        private int oldMaterial = 0;
        public override void Start()
        {
            if (TargetModel == null)
            {
                var _en = Entity.Scene.Entities;
                foreach (var entity in _en)
                {
                    if (entity.Name == "GeoSphere")
                    {
                        TargetModel = entity.Get<ModelComponent>();
                        TargetModel.Model.Materials.Clear();
                        Material mat = Material.New(GraphicsDevice, new MaterialDescriptor
                        {
                            Attributes =
                            {
                                Diffuse = new MaterialDiffuseMapFeature(new ComputeColor(new Color4(new Color3(0.65f, 0.02f, 0.02f)))),
                                DiffuseModel = new MaterialDiffuseLambertModelFeature(),
                                Specular = new MaterialSpecularMapFeature(),
                                SpecularModel = new MaterialSpecularMicrofacetModelFeature()
                            }
                        });
                        TargetModel.Model.Materials.Add(mat);
                    }
                }
            }
            if(Materials.Count > 0) matCount = Materials.Count;
            FindButtons();
            //ApplyCurrentMaterial();
        }

        private void FindButtons()
        {
            if (leftButton is not null && rightButton is not null) return;
            var page = Entity.Get<UIComponent>()?.Page;
            if (page is not null)
            {
                var root = page.RootElement;
                leftButton = (Button)root.FindName("LeftButton");
                rightButton = (Button)root.FindName("RightButton");
                fullScreenButton = (ToggleButton)root.FindName("FullScreenButton");
                //additionalButton = (ToggleButton)root.FindName("AdditionalButton");


                leftButton.Click += (s, e) => { curMaterial--; };
                rightButton.Click += (s, e) => { curMaterial++; };
                fullScreenButton.Click += (s, e) => { ChangeWindowMode(); };
            }
        }

        private void ChangeWindowMode()
        {
            if (!Game.Window.IsFullscreen
                && fullScreenButton.State == Stride.UI.ToggleState.Checked)
            {
                // Change Window
                var screenWidth = GraphicsDevice.Presenter.BackBuffer.Width;
                var screenHeight = GraphicsDevice.Presenter.BackBuffer.Height;
                var resolution = new Int2(screenWidth, screenHeight);
                Game.Window.Visible = false;
                Game.Window.SetSize(resolution);
                Game.Window.PreferredFullscreenSize = resolution;
                Game.Window.IsFullscreen = true;
                Game.Window.Visible = true;

                //Change icon to Fullscreen as 1 in Sprite Sheet
                ImageElement image = fullScreenButton.VisualChildren[0].VisualChildren[0] as ImageElement;
                var imageSource = image.Source as SpriteFromSheet;
                imageSource.CurrentFrame = 1;

                (fullScreenButton.VisualChildren[0].VisualChildren[0] as ImageElement).Source = imageSource;
            }
            else
            {
                Game.Window.Visible = false;
                Game.Window.IsFullscreen = false;
                Game.Window.Visible = true;

                //Change icon to Windowed as 0 in Sprite Sheet
                ImageElement image = fullScreenButton.VisualChildren[0].VisualChildren[0] as ImageElement;
                var imageSource = image.Source as SpriteFromSheet;
                imageSource.CurrentFrame = 0;
                (fullScreenButton.VisualChildren[0].VisualChildren[0] as ImageElement).Source = imageSource;
            }
        }

        public override void Update()
        {
            DebugText.Print("cur mat ind" + curMaterial + "old mat ind" + oldMaterial, new Int2(40, 40));
            if (Materials.Count == 0) return;
            if (curMaterial < 0) curMaterial = matCount-1;
            if(curMaterial > matCount - 1) curMaterial = 0;
            if(curMaterial != oldMaterial)
            {
                TargetModel.Model.Materials.Clear();
                var mat = Content.LoadAsync<Material>(Materials[curMaterial]).Result;
                TargetModel.Model.Materials.Add(mat);
                oldMaterial = curMaterial;
            }
        }
    }
}
