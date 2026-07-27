using DCFApixels.DragonECS;

namespace Asteroids.StarshipsFeature
{
    [MetaGroup(META_GROUP, EcsConsts.MODULES_GROUP)]
    [MetaColor(META_COLOR)]
    class StarshipsModule : IEcsModule, IEcsDefaultAddParams
    {
        public const string META_GROUP = nameof(Asteroids) + "/" + nameof(StarshipsFeature);
        public const uint META_COLOR = MetaColor.Lime;
        public AddParams AddParams => META_GROUP;

        public void Import(EcsPipeline.Builder b)
        {
            b.Add(new SpawnStarshipSystem());
            b.Add(new ImmunitySystem());
            b.Add(new StarshipAsteroidCollisionSystem());
            b.Add(new SpawnBulletSystem());
            b.Add(new RespawnStarshipOnDeathSystem());
        }
    }
}
