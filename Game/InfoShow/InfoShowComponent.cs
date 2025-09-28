using System.ComponentModel;
using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Mathematics;
using Stride.Core.Serialization.Contents;
using Stride.Engine;
using Stride.Engine.Design;
using Stride.Graphics;

namespace TestLoading.InfoShow
{
    [DataContract("QuaternionDisplayComponent")]
    [Display("Position Display")]
    [ComponentCategory("Debug")]
    [DefaultEntityComponentProcessor(typeof(InfoShowProcessor),ExecutionMode = ExecutionMode.Editor)]
    public class InfoShowComponent : EntityComponent
    {
        [DataMember]
        [Display("Auto Update Info")]
        [DefaultValue(true)]
        public bool AutoUpdate { get; set; }

        [DataMember]
        [Display("Hiden Info")]
        [DefaultValue(false)]
        public bool HideInfo { get; set; }

        public SpriteFont SpriteFont => 
            Entity.EntityManager.Services.GetService<ContentManager>().LoadAsync<SpriteFont>("infofont").Result;
    }
}
