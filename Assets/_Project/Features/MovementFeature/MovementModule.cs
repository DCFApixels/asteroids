using DCFApixels.DragonECS;

namespace Asteroids.MovementFeature
{
    [MetaGroup(META_GROUP, EcsConsts.MODULES_GROUP)]
    [MetaColor(META_COLOR)]
    public class MovementModule : IEcsModule, IEcsDefaultAddParams
    {
        public const string META_GROUP = nameof(Asteroids) + "." + nameof(MovementFeature);
        public const uint META_COLOR = MetaColor.Cyan;
        public AddParams AddParams => META_GROUP;
        public void Import(EcsPipeline.Builder b)
        {
            b.Layers.InsertAfter(EcsConsts.BASIC_LAYER, META_GROUP);
            b.Add(new ReadTransformSystem());
            b.Add(new ApplyTransformSystem());
            b.Add(new ApplyVelocitySystem());
            b.Add(new DebugVelocitySystem());
        }
    }
}