using Asteroids.Components;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class RespawnStarShipOnHitSystem : IEcsRun
    {
        [DI] private EcsDefaultWorld _world;
    
        public void Run()
        {
            var starShipPool = _world.GetPool<Starship>();
            if  (starShipPool.Count == 0)
            {
                _world.GetPool<SpawnStarship>().Add(_world.NewEntity());
            }
        }
    }
}