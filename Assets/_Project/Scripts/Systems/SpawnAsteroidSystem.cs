using Asteroids.Components;
using Asteroids.Data;
using Asteroids.MovementFeature;
using Asteroids.Utils;
using DCFApixels.DragonECS;
using UnityEngine;

namespace Asteroids.Systems
{
    internal class SpawnAsteroidSystem : IEcsRun
    {
        [DI] private StaticData _staticData;
        [DI] private EcsDefaultWorld _world;
        [DI] private RuntimeData _runtimeData;
        [DI] private PoolService _poolService;

        private class EventAspect : EcsAspect
        {
            public readonly EcsPool<SpawnAsteroidEvent> SpawnAsteroidEvents = Inc;
        }

        public void Run()
        {
            foreach (var eventE in _world.Where(out EventAspect eventA))
            {
                var spawnAsteroidEvent = eventA.SpawnAsteroidEvents.Get(eventE);
            
                var asteroidViewInstance = _poolService.Get(_staticData.AsteroidView, out var asteroidViewInstanceID);
                asteroidViewInstance.SetRadius(spawnAsteroidEvent.StartRadius);

                var asteroidE = _world.NewEntity(_staticData.AsteroidTemplate);
                ref var asteroid = ref _world.GetPool<Asteroid>().TryAddOrGet(asteroidE);
              
                asteroid.DeathsLeft = spawnAsteroidEvent.DeathsLeft;
                asteroid.Radius = spawnAsteroidEvent.StartRadius;
                ref var poolId = ref _world.GetPool<PoolId>().TryAddOrGet(asteroidE);
                poolId.Id = asteroidViewInstanceID;
                poolId.Component = asteroidViewInstance;

                asteroidViewInstance.Connect((_world, asteroidE), false);
                //_world.GetPool<UnityComponent<Transform>>().Add(asteroidE).obj = asteroidViewInstance.transform;
                _world.GetPool<KillOutsideMarker>().TryAdd(asteroidE);
            
                ref var transformData = ref _world.GetPool<TransformData>().TryAddOrGet(asteroidE);
                ref var velocity = ref _world.GetPool<Velocity>().TryAddOrGet(asteroidE);
                transformData.position = spawnAsteroidEvent.Position;
                transformData.rotation = spawnAsteroidEvent.Rotation;
                velocity.lineral = asteroidViewInstance.transform.forward * Random.Range(_staticData.AsteroidMinSpeed, _staticData.AsteroidMaxSpeed);
            
                eventA.SpawnAsteroidEvents.Del(eventE);
            }
        }

    }
}