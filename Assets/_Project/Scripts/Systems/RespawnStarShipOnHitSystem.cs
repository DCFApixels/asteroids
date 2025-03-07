using Asteroids.Components;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class RespawnStarShipOnHitSystem : IEcsRun
    {
        [DI] private EcsDefaultWorld _world;

        class Aspect : EcsAspect
        {
            public readonly EcsPool<Starship> Starships = Inc;
        }
        public void Run()
        {
            var starShipPool = _world.Where(out Aspect _);
            if (starShipPool.Count == 0)
            {
                _world.GetPool<SpawnStarshipEvent>().Add(_world.NewEntity());
            }
        }
    }
}