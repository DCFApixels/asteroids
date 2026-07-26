using DCFApixels.DragonECS;

namespace Asteroids.AsteroidsFeature
{
    internal class AsteroidModule : IEcsModule
    {
        public void Import(EcsPipeline.Builder b)
        {
            b.Add(new CheckAsteroidHitSystem());
            b.Add(new AutoSpawnAsteroidSystem());
            b.Add(new SpawnAsteroidSystem());
        }
    }
}