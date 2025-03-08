using DCFApixels.DragonECS;

namespace Asteroids.MovementFeature
{
    [MetaGroup(nameof(MovementModule), EcsConsts.MODULES_GROUP)]
    [MetaColor(MetaColor.Cyan)]
    public class MovementModule : IEcsModule
    {
        public void Import(EcsPipeline.Builder b)
        {
            b.Add(new ReadTransformSystem());
            b.Add(new ApplyTransformSystem());
            b.Add(new TeleportEntitySystem());
            b.Add(new ApplyVelocitySystem());
        }
    }
}