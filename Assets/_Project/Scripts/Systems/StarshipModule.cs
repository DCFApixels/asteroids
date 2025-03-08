using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class StarshipModule : IEcsModule
    {
        public void Import(EcsPipeline.Builder b)
        {
            b.Add(new SpawnStarshipSystem());
            b.Add(new InputSystem());
            b.Add(new SetMoveInfoFromInputDataSystem());
            b.Add(new SpawnBulletSystem());
            b.Add(new RespawnStarShipOnHitSystem());
        }
    }
}