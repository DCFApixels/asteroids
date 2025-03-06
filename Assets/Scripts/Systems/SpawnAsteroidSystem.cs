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

        class Aspect : EcsAspect
        {
            public readonly EcsPool<SpawnAsteroid> SpawnAsteroids = Inc;
        }

        public void Run()
        {
            foreach (var e in _world.Where(out Aspect a))
            {
                var spawnAsteroid = a.SpawnAsteroids.Get(e);
            
                var prefab = _staticData.AsteroidView;

                var instance = _poolService.Get(prefab, out var id);
                instance.transform.position = spawnAsteroid.Position;
                instance.transform.rotation = spawnAsteroid.Rotation;

                var entity = _world.NewEntity();
                ref var asteroid = ref _world.GetPool<Asteroid>().Add(entity);
                asteroid.View = instance;
                asteroid.DeathsLeft = spawnAsteroid.DeathsLeft;
                asteroid.Radius = spawnAsteroid.StartRadius;
                ref var poolId = ref _world.GetPool<PoolId>().Add(entity);
                poolId.Id = id;
                poolId.Component = instance;

                asteroid.View.transform.localScale = Vector3.one * spawnAsteroid.StartRadius / instance.Radius;
            
                _world.GetPool<TransformRef>().Add(entity).Value = instance.transform;
                _world.GetPool<DeleteOutside>().Add(entity);
            
                ref var moveInfo = ref _world.GetPool<MoveInfo>().Add(entity);
                moveInfo.Speed = Random.Range(_staticData.AsteroidMinSpeed, _staticData.AsteroidMaxSpeed);
                moveInfo.Position = instance.transform.position;
                moveInfo.Forward = instance.transform.forward;
            
                a.SpawnAsteroids.Del(e);
            }
        }

    }
}