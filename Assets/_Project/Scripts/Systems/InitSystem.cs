using Asteroids.Components;
using Asteroids.Data;
using Asteroids.Utils;
using DCFApixels.DragonECS;

namespace Asteroids.Systems
{
    internal class InitSystem : IEcsInit
    {
        [DI] EcsDefaultWorld _world;
        [DI] StaticData _staticData;
        [DI] PoolService _poolService;

        public void Init()
        {
            _world.GetPool<SpawnStarshipEvent>().Add(_world.NewEntity());

            _poolService.PreWarm(_staticData.AsteroidView, 20);
            _poolService.PreWarm(_staticData.BulletView, 20);
            _poolService.PreWarm(_staticData.StarshipView, 1);
        }
    }
}