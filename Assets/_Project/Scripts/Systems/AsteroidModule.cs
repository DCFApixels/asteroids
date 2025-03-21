using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class AsteroidModule : IEcsModule
    {
        public void Import(EcsPipeline.Builder b)
        {
            b
                .Add(new RecalculateSpaceHashSystem())
                .Add(new CheckIntersectionWithAsteroidsSystem())
                .Add(new CheckAsteroidHitSystem())
                .Add(new AutoSpawnAsteroidSystem())
                .Add(new SpawnAsteroidSystem())
                ;

        }
    }
}