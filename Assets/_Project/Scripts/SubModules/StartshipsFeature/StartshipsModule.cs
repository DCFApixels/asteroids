using Asteroids.Systems;
using DCFApixels.DragonECS;

namespace Asteroids.StartshipsFeature
{
    internal class StartshipsModule : IEcsModule
    {
        public void Import(EcsPipeline.Builder b)
        {
            b.Add(new StartshipDeathSystem());


            b.Add(new ImmunitySystem());
            b.Add(new SpawnBulletSystem());
            b.Add(new RespawnStarShipOnHitSystem());
        }
    }
}
