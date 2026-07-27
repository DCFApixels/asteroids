using DCFApixels.DragonECS;

namespace Asteroids.BulletsFeature
{
    [MetaGroup(META_GROUP, EcsConsts.MODULES_GROUP)]
    [MetaColor(META_COLOR)]
    class BulletsModule : IEcsModule, IEcsDefaultAddParams
    {
        public const string META_GROUP = nameof(Asteroids) + "/" + nameof(BulletsFeature);
        public const uint META_COLOR = MetaColor.Yellow;
        public AddParams AddParams => META_GROUP;

        public void Import(EcsPipeline.Builder b)
        {
            b.Add(new BulletLifetimeSystem());
            b.Add(new BulletAsteroidCollisionSystem());
        }
    }
}
