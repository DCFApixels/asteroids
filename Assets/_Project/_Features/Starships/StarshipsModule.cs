using DCFApixels.DragonECS;

namespace Asteroids.StarshipsFeature
{
    internal class StarshipsModule : IEcsModule
    {
        public void Import(EcsPipeline.Builder b)
        {
            b.Add(new SpawnStarshipSystem());
            b.Add(new ImmunitySystem());
            b.Add(new StarshipAsteroidCollisionSystem());
            b.Add(new SpawnBulletSystem());
            b.Add(new RespawnStarShipOnHitSystem());
        }
    }
}
