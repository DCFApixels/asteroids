using DCFApixels.DragonECS;

namespace Asteroids.MovementFeature
{
    [MetaGroup(META_GROUP, EcsConsts.MODULES_GROUP)]
    [MetaColor(META_COLOR)]
    public class MovementModule : IEcsModule
    {
        public const string META_GROUP = nameof(MovementModule);
        public const uint META_COLOR = MetaColor.Cyan;
        public void Import(EcsPipeline.Builder b)
        {
            b.Add(new ReadTransformSystem());
            b.Add(new ApplyTransformSystem());
            b.Add(new ApplyVelocitySystem());
            b.Add(new DebugVelocitySystem());
        }
    }
}