using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class AsteroidModule : IEcsModule
    {
        public void Import(EcsPipeline.Builder b)
        {
            b.Add(new RecalculateSpaceHashSystem());
            b.Add(new CheckIntersectionWithAsteroidsSystem());
            b.Add(new CheckAsteroidHitSystem());
            b.Add(new AutoSpawnAsteroidSystem());
            b.Add(new SpawnAsteroidSystem());
        }
    }
}