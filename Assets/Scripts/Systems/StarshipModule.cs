using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class StarshipModule : IEcsModule
    {
        public void Import(EcsPipeline.Builder b)
        {
            b
                .Add(new SpawnStarshipSystem())
                .Add(new InputSystem())
                .Add(new ImmunitySystem())
                .Add(new SetMoveInfoFromInputDataSystem())
                .Add(new SpawnBulletSystem())
                .Add(new RespawnStarShipOnHitSystem())
                ;
        }
    }
}