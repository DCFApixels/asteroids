using Asteroids.Components;
using Asteroids.Data;
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

        private class Aspect : EcsAspect
        {
            public readonly EcsPool<SpawnAsteroidEvent> SpawnAsteroidEvents = Inc;
        }

        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                var spawnAsteroid = a.SpawnAsteroidEvents.Get(e);
            
                var prefab = _staticData.AsteroidView;

                var instance = _poolService.Get(prefab, out var id);
                instance.transform.position = spawnAsteroid.Position;
                instance.transform.rotation = spawnAsteroid.Rotation;
                instance.SetRadius(spawnAsteroid.StartRadius);

                var entity = _world.NewEntity();
                ref var asteroid = ref _world.GetPool<Asteroid>().Add(entity);
              
                asteroid.DeathsLeft = spawnAsteroid.DeathsLeft;
                asteroid.Radius = spawnAsteroid.StartRadius;
                ref var poolId = ref _world.GetPool<PoolId>().Add(entity);
                poolId.Id = id;
                poolId.Component = instance;
                
                _world.GetPool<TransformRef>().Add(entity).Value = instance.transform;
                _world.GetPool<KillOutsideMarker>().Add(entity);
            
                ref var moveInfo = ref _world.GetPool<MoveInfo>().Add(entity);
                moveInfo.MaxSpeed = moveInfo.Speed = Random.Range(_staticData.AsteroidMinSpeed, _staticData.AsteroidMaxSpeed);
                moveInfo.Position = instance.transform.position;
                moveInfo.Forward = instance.transform.forward;
                moveInfo.Power = 1;
            
                a.SpawnAsteroidEvents.Del(e);
            }
        }

    }
}