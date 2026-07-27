using DCFApixels.DragonECS;

namespace Asteroids.AsteroidsFeature
{
    [MetaGroup(META_GROUP, EcsConsts.MODULES_GROUP)]
    [MetaColor(META_COLOR)]
    class AsteroidsModule : IEcsModule, IEcsDefaultAddParams
    {
        public const string META_GROUP = nameof(Asteroids) + "/" + nameof(AsteroidsFeature);
        public const uint META_COLOR = MetaColor.Orange;
        public AddParams AddParams => META_GROUP;

        public void Import(EcsPipeline.Builder b)
        {
            b.Add(new CheckAsteroidHitSystem());
            b.Add(new AutoSpawnAsteroidSystem());
            b.Add(new SpawnAsteroidSystem());
        }
    }
}
