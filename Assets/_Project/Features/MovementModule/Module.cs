using DCFApixels.DragonECS;

namespace Modules.Movement
{
    [MetaGroup(META_GROUP, EcsConsts.MODULES_GROUP)]
    [MetaColor(META_COLOR)]
    public class MovementModule : IEcsModule, IEcsDefaultAddParams
    {
        public const string META_GROUP = nameof(Modules) + "/" + nameof(Movement);
        public const uint META_COLOR = MetaColor.Cyan;
        public AddParams AddParams => META_GROUP;
        public void Import(EcsPipeline.Builder b)
        {
            b.Layers.Add(META_GROUP).After(EcsConsts.BASIC_LAYER);
            b.Add(new TransformTickSystem());
           // b.Add(new ReadTransformSystem());
            b.Add(new ApplyVelocitySystem());
            b.Add(new DebugVelocitySystem());
            b.Add(new ApplyTransformSystem());
        }
    }
}